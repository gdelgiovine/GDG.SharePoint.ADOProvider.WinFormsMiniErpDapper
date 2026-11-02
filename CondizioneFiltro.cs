using System;
using System.Data;
using System.Globalization;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    internal enum TipoColonnaFiltro { Testo, Intero, Decimale, Booleano }

    // Colonna filtrabile: Nome e' quello mostrato (proprieta' del Model),
    // ColonnaSql e' il nome da usare nel WHERE (per i lookup differisce).
    internal class ColonnaFiltro
    {
        private static readonly string[] ConfrontiTesto = { "contiene", "inizia con", "uguale a", "diverso da" };
        private static readonly string[] ConfrontiNumero = { "=", "<>", ">", ">=", "<", "<=" };
        private static readonly string[] ConfrontiBool = { "=", "<>" };

        public string Nome { get; private set; }
        public string ColonnaSql { get; private set; }
        public TipoColonnaFiltro Tipo { get; private set; }

        public ColonnaFiltro(string nome, TipoColonnaFiltro tipo, string colonnaSql = null)
        {
            Nome = nome;
            Tipo = tipo;
            ColonnaSql = colonnaSql ?? nome;
        }

        public string[] Confronti
        {
            get
            {
                if (Tipo == TipoColonnaFiltro.Testo) return ConfrontiTesto;
                if (Tipo == TipoColonnaFiltro.Booleano) return ConfrontiBool;
                return ConfrontiNumero;
            }
        }

        public override string ToString()
        {
            return Nome;
        }

        public bool TryConverti(string testo, out object valore, out DbType dbType)
        {
            testo = (testo ?? "").Trim();
            valore = null;
            dbType = DbType.String;
            if (testo.Length == 0) return false;

            switch (Tipo)
            {
                case TipoColonnaFiltro.Intero:
                    long l;
                    dbType = DbType.Int64;
                    if (!long.TryParse(testo, NumberStyles.Integer, CultureInfo.CurrentCulture, out l)) return false;
                    valore = l;
                    return true;
                case TipoColonnaFiltro.Decimale:
                    decimal d;
                    dbType = DbType.Decimal;
                    if (!decimal.TryParse(testo, NumberStyles.Number, CultureInfo.CurrentCulture, out d)) return false;
                    valore = d;
                    return true;
                case TipoColonnaFiltro.Booleano:
                    dbType = DbType.Boolean;
                    string t = testo.ToLowerInvariant();
                    if (t == "true" || t == "vero" || t == "si" || t == "s\u00EC" || t == "yes" || t == "1")
                        valore = true;
                    else if (t == "false" || t == "falso" || t == "no" || t == "0")
                        valore = false;
                    else
                        return false;
                    return true;
                default:
                    valore = testo;
                    return true;
            }
        }
    }

    // Singola condizione; Operatore (AND/OR) la lega alla precedente.
    internal class CondizioneFiltro
    {
        public string Operatore { get; set; } = "AND";
        public string Colonna { get; set; } = "";
        public string Confronto { get; set; } = "contiene";
        public string Testo { get; set; } = "";

        public override string ToString()
        {
            return Descrizione;
        }

        // Testo mostrato nell'elenco; l'operatore del primo filtro non ha effetto.
        public string Descrizione
        {
            get { return Operatore + " " + Colonna + " " + Confronto + " " + Testo; }
        }
    }
}
