// Provisioning opzionale e non distruttivo delle liste demo: crea solo le liste
// mancanti e inserisce i dati iniziali solo nelle liste vuote.
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using GDG.SharePoint.ADOProvider;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    internal class StatoProvisioning
    {
        public bool AgentiEsiste, ClientiEsiste, OrdiniEsiste, ProdottiEsiste, RigheEsiste;
        public long AgentiRighe, ClientiRighe, OrdiniRighe, ProdottiRighe, RigheRighe;

        public bool RichiedeAzione
        {
            get
            {
                return !AgentiEsiste || !ClientiEsiste || !OrdiniEsiste || !ProdottiEsiste || !RigheEsiste ||
                       AgentiRighe == 0 || ClientiRighe == 0 || OrdiniRighe == 0 ||
                       ProdottiRighe == 0 || RigheRighe == 0;
            }
        }

        public string Descrizione()
        {
            return Riga("TutorialAgents", AgentiEsiste, AgentiRighe) + Environment.NewLine +
                   Riga("TutorialCustomers", ClientiEsiste, ClientiRighe) + Environment.NewLine +
                   Riga("TutorialOrders", OrdiniEsiste, OrdiniRighe) + Environment.NewLine +
                   Riga("TutorialProducts", ProdottiEsiste, ProdottiRighe) + Environment.NewLine +
                   Riga("TutorialOrderLines", RigheEsiste, RigheRighe);
        }

        private static string Riga(string nome, bool esiste, long righe)
        {
            return "- " + nome + ": " + (!esiste ? "mancante" : righe == 0 ? "vuota" : "ok (" + righe + " elementi)");
        }
    }

    internal class DemoProvisioning
    {
        private readonly string stringaConnessione;

        public DemoProvisioning(string connessione)
        {
            if (string.IsNullOrWhiteSpace(connessione))
                throw new ArgumentException("La stringa di connessione e' vuota.");
            stringaConnessione = connessione;
        }

        private SharePointConnection Apri()
        {
            SharePointConnection cn = new SharePointConnection(stringaConnessione);
            try { cn.Open(); return cn; }
            catch { cn.Dispose(); throw; }
        }

        // Una lista inesistente fa fallire la SELECT: viene trattata come mancante.
        private static bool TryConta(SharePointConnection cn, string lista, out long righe)
        {
            try
            {
                righe = cn.ExecuteScalar<long>("SELECT COUNT(*) FROM [" + lista + "]");
                return true;
            }
            catch (Exception)
            {
                righe = 0;
                return false;
            }
        }

        public StatoProvisioning Analizza()
        {
            StatoProvisioning s = new StatoProvisioning();
            using (SharePointConnection cn = Apri())
            {
                s.AgentiEsiste = TryConta(cn, "TutorialAgents", out s.AgentiRighe);
                s.ClientiEsiste = TryConta(cn, "TutorialCustomers", out s.ClientiRighe);
                s.OrdiniEsiste = TryConta(cn, "TutorialOrders", out s.OrdiniRighe);
                s.ProdottiEsiste = TryConta(cn, "TutorialProducts", out s.ProdottiRighe);
                s.RigheEsiste = TryConta(cn, "TutorialOrderLines", out s.RigheRighe);
            }
            return s;
        }

        public void Esegui()
        {
            StatoProvisioning s = Analizza();
            using (SharePointConnection cn = Apri())
            {
                if (!s.AgentiEsiste)
                    cn.Execute("CREATE TABLE [TutorialAgents] (" +
                        "Title VARCHAR(255) NOT NULL, AgentCode VARCHAR(50), " +
                        "Active BOOLEAN DEFAULT TRUE, Region VARCHAR(100))");
                if (!s.ClientiEsiste)
                    cn.Execute("CREATE TABLE [TutorialCustomers] (" +
                        "Title VARCHAR(255) NOT NULL, Code VARCHAR(50), " +
                        "Active BOOLEAN DEFAULT TRUE, Country VARCHAR(10), " +
                        "AccountAgent LOOKUP([TutorialAgents], [Title]))");
                if (!s.OrdiniEsiste)
                    cn.Execute("CREATE TABLE [TutorialOrders] (" +
                        "Title VARCHAR(255) NOT NULL, " +
                        "Customer LOOKUP([TutorialCustomers], [Title]), " +
                        "Agent LOOKUP([TutorialAgents], [Title]), " +
                        "CustomerCode VARCHAR(50), AgentCode VARCHAR(50), " +
                        "Status VARCHAR(50), Amount DECIMAL(18,2))");
                if (!s.ProdottiEsiste)
                    cn.Execute("CREATE TABLE [TutorialProducts] (" +
                        "Title VARCHAR(255) NOT NULL, Code VARCHAR(50), Category VARCHAR(100), " +
                        "UnitPrice DECIMAL(18,2), Active BOOLEAN DEFAULT TRUE, " +
                        "Tags MULTICHOICE('Featured','Seasonal','Clearance'))");
                if (!s.RigheEsiste)
                    cn.Execute("CREATE TABLE [TutorialOrderLines] (" +
                        "Title VARCHAR(255) NOT NULL, OrderRef LOOKUP([TutorialOrders], [Title]), " +
                        "Product LOOKUP([TutorialProducts], [Title]), LineNumber INTEGER, Quantity INTEGER, " +
                        "UnitPrice DECIMAL(18,2), DiscountPercent DECIMAL(18,2), LineAmount DECIMAL(18,2))");

                if (!s.AgentiEsiste || s.AgentiRighe == 0)
                {
                    string[] nomiAgenti = { "Mario Bianchi", "Laura Verdi", "Paolo Neri", "Giulia Rossi", "Luca Ferrari",
                        "Sara Romano", "Marco Colombo", "Elena Ricci", "Andrea Marino", "Chiara Greco" };
                    string[] regioni = { "Nord", "Centro", "Sud", "Isole", "Estero" };
                    for (int i = 0; i < nomiAgenti.Length; i++)
                        cn.Execute("INSERT INTO [TutorialAgents] (Title, AgentCode, Active, Region) " +
                            "VALUES (@Title, @AgentCode, @Active, @Region)",
                            new { Title = nomiAgenti[i], AgentCode = "AG" + (i + 1).ToString("000"), Active = true, Region = regioni[i % regioni.Length] });
                }

                Dictionary<string, long> idAgenti = cn.Query<Agente>(
                    "SELECT ID, Title, AgentCode, Active, Region FROM [TutorialAgents]")
                    .Where(a => !string.IsNullOrEmpty(a.AgentCode))
                    .GroupBy(a => a.AgentCode).ToDictionary(g => g.Key, g => g.First().ID);

                if ((!s.ClientiEsiste || s.ClientiRighe == 0) && idAgenti.Count > 0)
                {
                    string[] ragioni = { "Acme", "Globex", "Initech", "Umbrella", "Hooli", "Stark", "Wayne", "Wonka", "Soylent", "Cyberdyne" };
                    string[] forme = { "Srl", "SpA", "GmbH" };
                    string[] nazioni = { "IT", "IT", "DE", "FR", "ES", "CH" };
                    string[] codiciAgente = idAgenti.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray();
                    for (int i = 0; i < 30; i++)
                    {
                        DynamicParameters p = new DynamicParameters();
                        p.Add("Title", ragioni[i % ragioni.Length] + " " + forme[(i / ragioni.Length) % forme.Length] + " " + (i + 1), System.Data.DbType.String);
                        p.Add("Code", "C" + (i + 1).ToString("000"), System.Data.DbType.String);
                        p.Add("Active", i % 10 != 9, System.Data.DbType.Boolean);
                        p.Add("Country", nazioni[i % nazioni.Length], System.Data.DbType.String);
                        p.Add("AccountAgent", idAgenti[codiciAgente[i % codiciAgente.Length]], System.Data.DbType.Int64);
                        cn.Execute("INSERT INTO [TutorialCustomers] (Title, Code, Active, Country, AccountAgent) " +
                            "VALUES (@Title, @Code, @Active, @Country, @AccountAgent)", p);
                    }
                }

                Dictionary<string, Cliente> clientiPerCodice = cn.Query<Cliente>(
                    "SELECT ID, Title, Code, Active, Country, AccountAgentId FROM [TutorialCustomers]")
                    .Where(c => !string.IsNullOrEmpty(c.Code))
                    .GroupBy(c => c.Code).ToDictionary(g => g.Key, g => g.First());
                Dictionary<long, string> codiceAgentePerId = idAgenti.ToDictionary(kv => kv.Value, kv => kv.Key);

                if ((!s.OrdiniEsiste || s.OrdiniRighe == 0) && clientiPerCodice.Count > 0)
                {
                    // Ogni ordine usa l'agente del proprio cliente: Agent/AgentCode coerenti con AccountAgent.
                    string[] codiciCliente = clientiPerCodice.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray();
                    string[] stati = { "Open", "Shipped", "Closed", "Cancelled" };
                    for (int i = 0; i < 120; i++)
                    {
                        Cliente cliente = clientiPerCodice[codiciCliente[i % codiciCliente.Length]];
                        long? idAgente = cliente.AccountAgentId;
                        string codiceAgente;
                        if (!idAgente.HasValue || !codiceAgentePerId.TryGetValue(idAgente.Value, out codiceAgente))
                        {
                            idAgente = null;
                            codiceAgente = null;
                        }
                        DynamicParameters p = new DynamicParameters();
                        p.Add("Title", "Ordine " + (i + 1).ToString("0000"), System.Data.DbType.String);
                        p.Add("Customer", cliente.ID, System.Data.DbType.Int64);
                        p.Add("Agent", idAgente, System.Data.DbType.Int64);
                        p.Add("CustomerCode", cliente.Code, System.Data.DbType.String);
                        p.Add("AgentCode", codiceAgente, System.Data.DbType.String);
                        p.Add("Status", stati[i % stati.Length], System.Data.DbType.String);
                        p.Add("Amount", 100m + (i * 37 % 900) + (i % 4) * 0.25m, System.Data.DbType.Decimal);
                        cn.Execute("INSERT INTO [TutorialOrders] " +
                            "(Title, Customer, Agent, CustomerCode, AgentCode, Status, Amount) " +
                            "VALUES (@Title, @Customer, @Agent, @CustomerCode, @AgentCode, @Status, @Amount)", p);
                    }
                }

                if (!s.ProdottiEsiste || s.ProdottiRighe == 0)
                {
                    string[] nomi = { "Tastiera", "Mouse", "Monitor", "Docking", "Webcam", "Cuffie", "Stampante", "Scanner",
                        "Router", "Switch", "Notebook", "Tablet", "Cavo HDMI", "Hub USB", "SSD", "Disco esterno",
                        "Alimentatore", "Zaino", "Lampada", "Supporto monitor" };
                    string[] categorie = { "Periferiche", "Rete", "Computer", "Accessori" };
                    string[] tags = { "Featured", "Seasonal", "Clearance", "" };
                    for (int i = 0; i < nomi.Length; i++)
                    {
                        DynamicParameters p = new DynamicParameters();
                        p.Add("Title", nomi[i], System.Data.DbType.String);
                        p.Add("Code", "P" + (i + 1).ToString("000"), System.Data.DbType.String);
                        p.Add("Category", categorie[i % categorie.Length], System.Data.DbType.String);
                        p.Add("UnitPrice", 9.90m + (i * 13 % 200) + (i % 3) * 0.5m, System.Data.DbType.Decimal);
                        p.Add("Active", i % 10 != 9, System.Data.DbType.Boolean);
                        p.Add("Tags", string.IsNullOrEmpty(tags[i % tags.Length]) ? null : tags[i % tags.Length], System.Data.DbType.String);
                        cn.Execute("INSERT INTO [TutorialProducts] (Title, Code, Category, UnitPrice, Active, Tags) " +
                            "VALUES (@Title, @Code, @Category, @UnitPrice, @Active, @Tags)", p);
                    }
                }

                if (!s.RigheEsiste || s.RigheRighe == 0)
                {
                    List<Prodotto> prodotti = cn.Query<Prodotto>(
                        "SELECT ID, Title, Code, Category, UnitPrice, Active, Tags FROM [TutorialProducts] ORDER BY ID").ToList();
                    List<Ordine> ordiniEsistenti = cn.Query<Ordine>(
                        "SELECT ID, Title, LOOKUP_ID(Customer) AS CustomerId, LOOKUP_ID(Agent) AS AgentId, " +
                        "CustomerCode, AgentCode, Status, Amount FROM [TutorialOrders] ORDER BY ID").ToList();
                    if (prodotti.Count > 0)
                    {
                        for (int o = 0; o < ordiniEsistenti.Count; o++)
                        {
                            decimal totale = 0m;
                            int righe = 1 + o % 3;
                            for (int n = 1; n <= righe; n++)
                            {
                                Prodotto prodotto = prodotti[(o * 3 + n) % prodotti.Count];
                                int quantita = 1 + (o + n) % 5;
                                decimal sconto = (o + n) % 4 * 5m;
                                decimal importo = decimal.Round(quantita * prodotto.UnitPrice * (100m - sconto) / 100m, 2,
                                    MidpointRounding.AwayFromZero);
                                totale += importo;
                                DynamicParameters p = new DynamicParameters();
                                p.Add("Title", ordiniEsistenti[o].Title + " - riga " + n, System.Data.DbType.String);
                                p.Add("OrderId", ordiniEsistenti[o].ID, System.Data.DbType.Int64);
                                p.Add("ProductId", prodotto.ID, System.Data.DbType.Int64);
                                p.Add("LineNumber", n, System.Data.DbType.Int32);
                                p.Add("Quantity", quantita, System.Data.DbType.Int32);
                                p.Add("UnitPrice", prodotto.UnitPrice, System.Data.DbType.Decimal);
                                p.Add("DiscountPercent", sconto, System.Data.DbType.Decimal);
                                p.Add("LineAmount", importo, System.Data.DbType.Decimal);
                                cn.Execute("INSERT INTO [TutorialOrderLines] " +
                                    "(Title, OrderRef, Product, LineNumber, Quantity, UnitPrice, DiscountPercent, LineAmount) " +
                                    "VALUES (@Title, @OrderId, @ProductId, @LineNumber, @Quantity, @UnitPrice, @DiscountPercent, @LineAmount)", p);
                            }
                            cn.Execute("UPDATE [TutorialOrders] SET Amount=@Amount WHERE ID=@ID",
                                new { Amount = totale, ID = ordiniEsistenti[o].ID });
                        }
                    }
                }
            }
        }
    }
}
