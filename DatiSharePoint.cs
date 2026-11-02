// Repository Dapper: ogni SELECT materializza Model C# tipizzati, mai DataTable.
// SharePointConnection e' il provider ADO.NET REST-only gia' presente nel progetto.
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using GDG.SharePoint.ADOProvider;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    internal partial class DatiSharePoint
    {
        private readonly string stringaConnessione;
        public const int DimensioneMassimaAllegato = 20 * 1024 * 1024;

        public DatiSharePoint(string connessione)
        {
            if (string.IsNullOrWhiteSpace(connessione))
                throw new ArgumentException("La stringa di connessione e' vuota.");
            stringaConnessione = connessione;
        }

        private SharePointConnection ApriConnessione()
        {
            SharePointConnection cn = new SharePointConnection(stringaConnessione);
            try
            {
                cn.Open();
                return cn;
            }
            catch
            {
                cn.Dispose();
                throw;
            }
        }

        private void EseguiComando(string sql, object parametri)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                if (cn.Execute(sql, parametri) == 0)
                    throw new InvalidOperationException("Nessun elemento modificato. Aggiornare l'elenco.");
            }
        }

        // SELECT -> Query<T> -> oggetti Model.
        // Il nome di ogni proprieta' e' il nome della colonna SharePoint restituita.
        // Lookup: letti come XxxId, ma filtrati con la colonna di scrittura/confronto Xxx.
        internal static readonly ColonnaFiltro[] ColonneAgenti =
        {
            new ColonnaFiltro("ID", TipoColonnaFiltro.Intero),
            new ColonnaFiltro("Title", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("AgentCode", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Active", TipoColonnaFiltro.Booleano),
            new ColonnaFiltro("Region", TipoColonnaFiltro.Testo)
        };
        internal static readonly ColonnaFiltro[] ColonneClienti =
        {
            new ColonnaFiltro("ID", TipoColonnaFiltro.Intero),
            new ColonnaFiltro("Title", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Code", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Active", TipoColonnaFiltro.Booleano),
            new ColonnaFiltro("Country", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("AccountAgent", TipoColonnaFiltro.Intero, "LOOKUP_ID(AccountAgent)")
        };
        internal static readonly ColonnaFiltro[] ColonneOrdini =
        {
            new ColonnaFiltro("ID", TipoColonnaFiltro.Intero),
            new ColonnaFiltro("Title", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Customer", TipoColonnaFiltro.Intero, "LOOKUP_ID(Customer)"),
            new ColonnaFiltro("Agent", TipoColonnaFiltro.Intero, "LOOKUP_ID(Agent)"),
            new ColonnaFiltro("CustomerCode", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("AgentCode", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Status", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Amount", TipoColonnaFiltro.Decimale)
        };
        internal static readonly ColonnaFiltro[] ColonneProdotti =
        {
            new ColonnaFiltro("ID", TipoColonnaFiltro.Intero),
            new ColonnaFiltro("Title", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Code", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Category", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("UnitPrice", TipoColonnaFiltro.Decimale),
            new ColonnaFiltro("Active", TipoColonnaFiltro.Booleano),
            new ColonnaFiltro("Tags", TipoColonnaFiltro.Testo)
        };
        internal static readonly ColonnaFiltro[] ColonneRighe =
        {
            new ColonnaFiltro("ID", TipoColonnaFiltro.Intero),
            new ColonnaFiltro("Title", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("OrderRef", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("Product", TipoColonnaFiltro.Testo),
            new ColonnaFiltro("LineNumber", TipoColonnaFiltro.Intero),
            new ColonnaFiltro("Quantity", TipoColonnaFiltro.Intero),
            new ColonnaFiltro("UnitPrice", TipoColonnaFiltro.Decimale),
            new ColonnaFiltro("DiscountPercent", TipoColonnaFiltro.Decimale),
            new ColonnaFiltro("LineAmount", TipoColonnaFiltro.Decimale)
        };

        // Colonna e confronto provengono da whitelist; il valore e' sempre un parametro tipizzato.
        // Precedenza SQL standard: AND lega piu' di OR.
        private static string Where(ColonnaFiltro[] ammesse, IList<CondizioneFiltro> filtri, DynamicParameters p)
        {
            if (filtri == null || filtri.Count == 0) return "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder(" WHERE ");
            for (int i = 0; i < filtri.Count; i++)
            {
                CondizioneFiltro f = filtri[i];
                ColonnaFiltro col = ammesse.FirstOrDefault(c => c.Nome == f.Colonna);
                if (col == null)
                    throw new ArgumentException("Colonna di filtro non valida: " + f.Colonna);
                if (!col.Confronti.Contains(f.Confronto))
                    throw new ArgumentException("Confronto non valido per " + col.Nome + ": " + f.Confronto);

                object valore;
                DbType tipo;
                if (!col.TryConverti(f.Testo, out valore, out tipo))
                    throw new InvalidOperationException("Valore non valido per " + col.Nome + ": " + f.Testo);

                string op = f.Confronto;
                if (col.Tipo == TipoColonnaFiltro.Testo)
                {
                    if (f.Confronto == "contiene") { op = "LIKE"; valore = "%" + valore + "%"; }
                    else if (f.Confronto == "inizia con") { op = "LIKE"; valore = valore + "%"; }
                    else if (f.Confronto == "uguale a") op = "=";
                    else op = "<>";
                }

                if (i > 0)
                    sb.Append(f.Operatore == "OR" ? " OR " : " AND ");
                sb.Append(col.ColonnaSql).Append(' ').Append(op).Append(" @F").Append(i);
                p.Add("F" + i, valore, tipo);
            }
            return sb.ToString();
        }

        // Unico punto in cui viene costruito l'SQL: usato sia per l'esecuzione sia per la visualizzazione.
        internal static string CostruisciQuery(int tipo, IList<CondizioneFiltro> filtri, DynamicParameters p)
        {
            if (tipo == 0)
                return "SELECT ID, Title, AgentCode, Active, Region FROM [TutorialAgents]" +
                    Where(ColonneAgenti, filtri, p) + " ORDER BY ID";
            if (tipo == 1)
                return "SELECT ID, Title, Code, Active, Country, LOOKUP_ID(AccountAgent) AS AccountAgentId FROM [TutorialCustomers]" +
                    Where(ColonneClienti, filtri, p) + " ORDER BY ID";
            if (tipo == 3)
                return "SELECT ID, Title, Code, Category, UnitPrice, Active, Tags FROM [TutorialProducts]" +
                    Where(ColonneProdotti, filtri, p) + " ORDER BY ID";
            if (tipo == 4)
                return "SELECT ID, Title, OrderRef$Id AS OrderId, Product$Id AS ProductId, " +
                    "LineNumber, Quantity, UnitPrice, DiscountPercent, LineAmount FROM [TutorialOrderLines]" +
                    Where(ColonneRighe, filtri, p) + " ORDER BY ID";
            return "SELECT ID, Title, LOOKUP_ID(Customer) AS CustomerId, LOOKUP_ID(Agent) AS AgentId, CustomerCode, AgentCode, Status, Amount " +
                "FROM [TutorialOrders]" + Where(ColonneOrdini, filtri, p) + " ORDER BY ID";
        }

        internal static string DescriviQuery(int tipo, IList<CondizioneFiltro> filtri)
        {
            DynamicParameters p = new DynamicParameters();
            string sql = CostruisciQuery(tipo, filtri, p);
            System.Text.StringBuilder sb = new System.Text.StringBuilder(sql);
            if (p.ParameterNames.Any())
            {
                sb.AppendLine().AppendLine().AppendLine("-- Parametri");
                foreach (string nome in p.ParameterNames)
                    sb.Append("-- @").Append(nome).Append(" = ")
                      .AppendLine(Convert.ToString(p.Get<object>(nome), System.Globalization.CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        public List<Agente> CaricaAgenti(IList<CondizioneFiltro> filtri = null)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                DynamicParameters p = new DynamicParameters();
                return cn.Query<Agente>(CostruisciQuery(0, filtri, p), p).ToList();
            }
        }

        public List<Cliente> CaricaClienti(IList<CondizioneFiltro> filtri = null)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                DynamicParameters p = new DynamicParameters();
                return cn.Query<Cliente>(CostruisciQuery(1, filtri, p), p).ToList();
            }
        }

        public List<Ordine> CaricaOrdini(IList<CondizioneFiltro> filtri = null)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                DynamicParameters p = new DynamicParameters();
                return cn.Query<Ordine>(CostruisciQuery(2, filtri, p), p).ToList();
            }
        }

        public List<Prodotto> CaricaProdotti(IList<CondizioneFiltro> filtri = null)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                DynamicParameters p = new DynamicParameters();
                return cn.Query<Prodotto>(CostruisciQuery(3, filtri, p), p).ToList();
            }
        }

        public List<RigaOrdine> CaricaRigheOrdine(IList<CondizioneFiltro> filtri = null)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                DynamicParameters p = new DynamicParameters();
                return cn.Query<RigaOrdine>(CostruisciQuery(4, filtri, p), p).ToList();
            }
        }

        public void SalvaProdotto(Prodotto prodotto)
        {
            if (prodotto == null || string.IsNullOrWhiteSpace(prodotto.Title) ||
                string.IsNullOrWhiteSpace(prodotto.Code))
                throw new InvalidOperationException("Titolo e codice del prodotto obbligatori.");
            if (prodotto.UnitPrice < 0) throw new InvalidOperationException("Prezzo negativo.");
            prodotto.Title = prodotto.Title.Trim();
            prodotto.Code = prodotto.Code.Trim();
            string sql = prodotto.ID == 0
                ? "INSERT INTO [TutorialProducts] (Title, Code, Category, UnitPrice, Active, Tags) " +
                  "VALUES (@Title, @Code, @Category, @UnitPrice, @Active, @Tags)"
                : "UPDATE [TutorialProducts] SET Title=@Title, Code=@Code, Category=@Category, " +
                  "UnitPrice=@UnitPrice, Active=@Active, Tags=@Tags WHERE ID=@ID";
            EseguiComando(sql, prodotto);
        }

        public void SalvaRigaOrdine(RigaOrdine riga)
        {
            if (riga == null || riga.OrderId <= 0 || riga.ProductId <= 0 ||
                riga.Quantity <= 0 || riga.LineNumber <= 0 || riga.UnitPrice < 0 ||
                riga.DiscountPercent < 0 || riga.DiscountPercent > 100)
                throw new InvalidOperationException("Dati riga ordine non validi.");
            riga.LineAmount = decimal.Round(riga.Quantity * riga.UnitPrice *
                (100m - riga.DiscountPercent) / 100m, 2, MidpointRounding.AwayFromZero);
            long? previousOrderId = null;
            if (riga.ID > 0)
            {
                using (SharePointConnection cn = ApriConnessione())
                    previousOrderId = cn.ExecuteScalar<long?>(
                        "SELECT OrderRef$Id FROM [TutorialOrderLines] WHERE ID=@ID", new { riga.ID });
            }

            // SharePoint non offre transazioni ADO.NET: ogni chiamata e' indipendente.
            string sql = riga.ID == 0
                ? "INSERT INTO [TutorialOrderLines] " +
                  "(Title, OrderRef, Product, LineNumber, Quantity, UnitPrice, DiscountPercent, LineAmount) " +
                  "VALUES (@Title, @OrderId, @ProductId, @LineNumber, @Quantity, @UnitPrice, @DiscountPercent, @LineAmount)"
                : "UPDATE [TutorialOrderLines] SET Title=@Title, OrderRef=@OrderId, Product=@ProductId, " +
                  "LineNumber=@LineNumber, Quantity=@Quantity, UnitPrice=@UnitPrice, " +
                  "DiscountPercent=@DiscountPercent, LineAmount=@LineAmount WHERE ID=@ID";
            EseguiComando(sql, riga);
            AggiornaTotaleOrdine(riga.OrderId);
            if (previousOrderId.HasValue && previousOrderId.Value != riga.OrderId)
                AggiornaTotaleOrdine(previousOrderId.Value);
        }

        private void AggiornaTotaleOrdine(long orderId)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                decimal totale = cn.ExecuteScalar<decimal?>(
                    "SELECT SUM(LineAmount) FROM [TutorialOrderLines] WHERE OrderRef$Id=@OrderId",
                    new { OrderId = orderId }) ?? 0m;
                cn.Execute("UPDATE [TutorialOrders] SET Amount=@Amount WHERE ID=@OrderId",
                    new { Amount = totale, OrderId = orderId });
            }
        }

        public void EliminaProdotto(Prodotto prodotto)
        {
            if (prodotto == null || prodotto.ID <= 0)
                throw new InvalidOperationException("Selezionare un prodotto salvato.");
            if (Conta("SELECT COUNT(*) FROM [TutorialOrderLines] WHERE Product$Id=@ID",
                    new { prodotto.ID }) > 0)
                throw new InvalidOperationException("Il prodotto e' referenziato da righe ordine.");
            EseguiComando("DELETE FROM [TutorialProducts] WHERE ID=@ID", new { prodotto.ID });
        }

        public void EliminaRigaOrdine(RigaOrdine riga)
        {
            if (riga == null || riga.ID <= 0)
                throw new InvalidOperationException("Selezionare una riga salvata.");
            EseguiComando("DELETE FROM [TutorialOrderLines] WHERE ID=@ID", new { riga.ID });
            AggiornaTotaleOrdine(riga.OrderId);
        }

        // INSERT e UPDATE ricevono un Model, non otto argomenti separati.
        public void SalvaAgente(Agente agente)
        {
            if (agente == null) throw new ArgumentNullException(nameof(agente));
            if (string.IsNullOrWhiteSpace(agente.Title) || string.IsNullOrWhiteSpace(agente.AgentCode))
                throw new InvalidOperationException("Titolo e codice agente sono obbligatori.");

            agente.Title = agente.Title.Trim();
            agente.AgentCode = agente.AgentCode.Trim();
            agente.Region = (agente.Region ?? "").Trim();

            string sql;
            if (agente.ID == 0)
                sql = "INSERT INTO [TutorialAgents] (Title, AgentCode, Active, Region) " +
                    "VALUES (@Title, @AgentCode, @Active, @Region)";
            else
                sql = "UPDATE [TutorialAgents] SET Title=@Title, AgentCode=@AgentCode, " +
                    "Active=@Active, Region=@Region WHERE ID=@ID";

            // I nomi dei parametri SQL coincidono con le proprieta' di Agente.
            EseguiComando(sql, agente);
        }

        public void SalvaCliente(Cliente cliente)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrWhiteSpace(cliente.Title) || string.IsNullOrWhiteSpace(cliente.Code))
                throw new InvalidOperationException("Titolo e codice cliente sono obbligatori.");
            if ((cliente.Country ?? "").Trim().Length > 10)
                throw new InvalidOperationException("Nazione: massimo 10 caratteri.");

            cliente.Title = cliente.Title.Trim();
            cliente.Code = cliente.Code.Trim();
            cliente.Country = (cliente.Country ?? "").Trim();

            string sql;
            if (cliente.ID == 0)
                sql = "INSERT INTO [TutorialCustomers] (Title, Code, Active, Country, AccountAgent) " +
                    "VALUES (@Title, @Code, @Active, @Country, @AccountAgent)";
            else
                sql = "UPDATE [TutorialCustomers] SET Title=@Title, Code=@Code, Active=@Active, " +
                    "Country=@Country, AccountAgent=@AccountAgent WHERE ID=@ID";

            // La proprieta' AccountAgentId del Model corrisponde alla colonna
            // di SCRITTURA AccountAgent, che accetta l'ID di un agente.
            DynamicParameters p = new DynamicParameters();
            p.Add("Title", cliente.Title, DbType.String);
            p.Add("Code", cliente.Code, DbType.String);
            p.Add("Active", cliente.Active, DbType.Boolean);
            p.Add("Country", cliente.Country, DbType.String);
            p.Add("AccountAgent", cliente.AccountAgentId, DbType.Int64);
            if (cliente.ID > 0) p.Add("ID", cliente.ID, DbType.Int64);
            EseguiComando(sql, p);
        }

        public void SalvaOrdine(Ordine ordine)
        {
            if (ordine == null) throw new ArgumentNullException(nameof(ordine));
            if (string.IsNullOrWhiteSpace(ordine.Title))
                throw new InvalidOperationException("Titolo ordine obbligatorio.");
            if (ordine.Amount < 0)
                throw new InvalidOperationException("L'importo non puo' essere negativo.");
            ordine.Title = ordine.Title.Trim();

            string sql;
            if (ordine.ID == 0)
                sql = "INSERT INTO [TutorialOrders] " +
                    "(Title, Customer, Agent, CustomerCode, AgentCode, Status, Amount) " +
                    "VALUES (@Title, @Customer, @Agent, @CustomerCode, @AgentCode, @Status, @Amount)";
            else
                sql = "UPDATE [TutorialOrders] SET Title=@Title, Customer=@Customer, Agent=@Agent, " +
                    "CustomerCode=@CustomerCode, AgentCode=@AgentCode, " +
                    "Status=@Status, Amount=@Amount WHERE ID=@ID";

            DynamicParameters p = new DynamicParameters();
            p.Add("Title", ordine.Title, DbType.String);
            // Lookup letti come CustomerId/AgentId; scritti come Customer/Agent.
            p.Add("Customer", ordine.CustomerId, DbType.Int64);
            p.Add("Agent", ordine.AgentId, DbType.Int64);
            p.Add("CustomerCode", string.IsNullOrWhiteSpace(ordine.CustomerCode)
                ? null : ordine.CustomerCode, DbType.String);
            p.Add("AgentCode", string.IsNullOrWhiteSpace(ordine.AgentCode)
                ? null : ordine.AgentCode, DbType.String);
            p.Add("Status", ordine.Status, DbType.String);
            p.Add("Amount", ordine.Amount, DbType.Decimal);
            if (ordine.ID > 0) p.Add("ID", ordine.ID, DbType.Int64);
            EseguiComando(sql, p);
        }

        private long Conta(string sql, object parametri)
        {
            using (SharePointConnection cn = ApriConnessione())
                return cn.ExecuteScalar<long>(sql, parametri);
        }

        public void EliminaAgente(Agente agente)
        {
            if (agente == null || agente.ID <= 0)
                throw new InvalidOperationException("Selezionare un agente salvato.");
            if (Conta("SELECT COUNT(*) FROM [TutorialCustomers] WHERE AccountAgent=@ID",
                    new { agente.ID }) > 0 ||
                Conta("SELECT COUNT(*) FROM [TutorialOrders] WHERE Agent=@ID",
                    new { agente.ID }) > 0 ||
                Conta("SELECT COUNT(*) FROM [TutorialOrders] WHERE AgentCode=@AgentCode",
                    new { agente.AgentCode }) > 0)
                throw new InvalidOperationException("Agente utilizzato da clienti o ordini.");
            EseguiComando("DELETE FROM [TutorialAgents] WHERE ID=@ID", new { agente.ID });
        }

        public void EliminaCliente(Cliente cliente)
        {
            if (cliente == null || cliente.ID <= 0)
                throw new InvalidOperationException("Selezionare un cliente salvato.");
            if (Conta("SELECT COUNT(*) FROM [TutorialOrders] WHERE Customer=@ID",
                    new { cliente.ID }) > 0 ||
                Conta("SELECT COUNT(*) FROM [TutorialOrders] WHERE CustomerCode=@Code",
                    new { cliente.Code }) > 0)
                throw new InvalidOperationException("Cliente utilizzato da ordini.");
            EseguiComando("DELETE FROM [TutorialCustomers] WHERE ID=@ID", new { cliente.ID });
        }

        public void EliminaOrdine(Ordine ordine)
        {
            if (ordine == null || ordine.ID <= 0)
                throw new InvalidOperationException("Selezionare un ordine salvato.");
            if (Conta("SELECT COUNT(*) FROM [TutorialOrderLines] WHERE OrderRef$Id=@ID", new { ordine.ID }) > 0)
                throw new InvalidOperationException("Ordine con righe: eliminare prima le righe ordine.");
            EseguiComando("DELETE FROM [TutorialOrders] WHERE ID=@ID", new { ordine.ID });
        }

        // Il nome lista viene esclusivamente dalla selezione del programma.
        internal static string TabellaAllegati(int tipo)
        {
            if (tipo == 0) return "$ATTACHMENTS([TutorialAgents])";
            if (tipo == 1) return "$ATTACHMENTS([TutorialCustomers])";
            if (tipo == 2) return "$ATTACHMENTS([TutorialOrders])";
            if (tipo == 3) return "$ATTACHMENTS([TutorialProducts])";
            if (tipo == 4) return "$ATTACHMENTS([TutorialOrderLines])";
            throw new ArgumentOutOfRangeException(nameof(tipo));
        }

        public List<Allegato> CaricaAllegati(int tipo, long id)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                return cn.Query<Allegato>(
                    "SELECT ItemId, FileName FROM " + TabellaAllegati(tipo) +
                    " WHERE ItemId=@Id", new { Id = id }).ToList();
            }
        }

        public byte[] ScaricaAllegato(int tipo, long id, string nome)
        {
            using (SharePointConnection cn = ApriConnessione())
            {
                byte[] dati = cn.ExecuteScalar<byte[]>(
                    "SELECT FileContent FROM " + TabellaAllegati(tipo) +
                    " WHERE ItemId=@Id AND FileName=@Nome", new { Id = id, Nome = nome });
                if (dati == null) throw new FileNotFoundException("Allegato non trovato.", nome);
                return dati;
            }
        }

        public void AggiungiAllegato(int tipo, long id, string nome, byte[] dati)
        {
            ControllaAllegato(nome, dati);
            DynamicParameters p = ParametriFile(id, nome, dati);
            EseguiComando("INSERT INTO " + TabellaAllegati(tipo) +
                " (ItemId, FileName, FileContent) VALUES (@Id, @Nome, @Dati)", p);
        }

        public void SostituisciAllegato(int tipo, long id, string nome, byte[] dati)
        {
            ControllaAllegato(nome, dati);
            DynamicParameters p = ParametriFile(id, nome, dati);
            EseguiComando("UPDATE " + TabellaAllegati(tipo) +
                " SET FileContent=@Dati WHERE ItemId=@Id AND FileName=@Nome", p);
        }

        private static DynamicParameters ParametriFile(long id, string nome, byte[] dati)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("Id", id, DbType.Int64);
            p.Add("Nome", nome, DbType.String);
            p.Add("Dati", dati, DbType.Binary);
            return p;
        }

        public void RinominaAllegato(int tipo, long id, string nomeOriginale, string nomeNuovo)
        {
            ControllaNomeFile(nomeNuovo);
            EseguiComando("UPDATE " + TabellaAllegati(tipo) +
                " SET FileName=@Nuovo WHERE ItemId=@Id AND FileName=@Originale",
                new { Nuovo = nomeNuovo, Id = id, Originale = nomeOriginale });
        }

        public void EliminaAllegato(int tipo, long id, string nome)
        {
            EseguiComando("DELETE FROM " + TabellaAllegati(tipo) +
                " WHERE ItemId=@Id AND FileName=@Nome", new { Id = id, Nome = nome });
        }

        internal static void ControllaNomeFile(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome) || nome == "." || nome == ".." ||
                nome != Path.GetFileName(nome) ||
                nome.IndexOf('/') >= 0 || nome.IndexOf('\\') >= 0)
                throw new InvalidOperationException("Indicare il nome del file, senza cartelle.");
        }

        private static void ControllaAllegato(string nome, byte[] dati)
        {
            ControllaNomeFile(nome);
            if (dati == null || dati.Length > DimensioneMassimaAllegato)
                throw new InvalidOperationException("Dimensione massima dell'allegato: 20 MiB.");
        }
    }
}
