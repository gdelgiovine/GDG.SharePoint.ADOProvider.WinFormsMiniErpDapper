// Model: un oggetto C# rappresenta una riga della corrispondente lista SharePoint.
// Dapper associa automaticamente le colonne alle proprieta' con lo stesso nome.
// I nomi sono quelli INTERNI delle colonne SharePoint, non le etichette visualizzate.
namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    public class Agente
    {
        public long ID { get; set; }
        public string Title { get; set; } = "";
        public string AgentCode { get; set; } = "";
        public bool Active { get; set; } = true;
        public string Region { get; set; } = "";
    }

    public class Cliente
    {
        public long ID { get; set; }
        public string Title { get; set; } = "";
        public string Code { get; set; } = "";
        public bool Active { get; set; } = true;
        public string Country { get; set; } = "";

        // La lettura espone il lookup SharePoint con il nome AccountAgentId;
        // la scrittura usa la colonna AccountAgent (vedere DatiSharePoint).
        public long? AccountAgentId { get; set; }
    }

    public class Ordine
    {
        public long ID { get; set; }
        public string Title { get; set; } = "";
        public long? CustomerId { get; set; }
        public long? AgentId { get; set; }
        public string CustomerCode { get; set; } = "";
        public string AgentCode { get; set; } = "";
        public string Status { get; set; } = "Open";
        public decimal Amount { get; set; }
    }

    // Le proprieta' sono compatibili con i nomi ADO.NET (UseSimpleNames=true).
    // In particolare i lookup singoli usano alias SQL OrderId e ProductId.
    public class Prodotto
    {
        public long ID { get; set; }
        public string Title { get; set; } = "";
        public string Code { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public bool Active { get; set; } = true;
        public string Tags { get; set; } = "";
    }

    // Ogni riga ha quantità, prezzo storico, sconto e totale propri.
    public class RigaOrdine
    {
        public long ID { get; set; }
        public string Title { get; set; } = "";
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public int LineNumber { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal LineAmount { get; set; }
    }

    // Solo metadati. Il file binario viene caricato separatamente su richiesta.
    public class Allegato
    {
        public long ItemId { get; set; }
        public string FileName { get; set; } = "";
    }
}

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    internal static class ModelMappingSelfTest
    {
        public static void Run()
        {
            // Test offline: verifica il mapping nominale Dapper, senza aprire SharePoint.
            AssertMap(typeof(Agente), "ID", "Title", "AgentCode", "Active", "Region");
            AssertMap(typeof(Cliente), "ID", "Title", "Code", "Active", "Country", "AccountAgentId");
            AssertMap(typeof(Ordine), "ID", "Title", "CustomerId", "AgentId",
                "CustomerCode", "AgentCode", "Status", "Amount");
            AssertMap(typeof(Prodotto), "ID", "Title", "Code", "Category", "UnitPrice", "Active", "Tags");
            AssertMap(typeof(RigaOrdine), "ID", "Title", "OrderId", "ProductId", "LineNumber",
                "Quantity", "UnitPrice", "DiscountPercent", "LineAmount");
            AssertMap(typeof(Allegato), "ItemId", "FileName");

            if (typeof(Cliente).GetProperty("AccountAgentId").PropertyType != typeof(long?) ||
                typeof(Ordine).GetProperty("CustomerId").PropertyType != typeof(long?) ||
                typeof(Ordine).GetProperty("AgentId").PropertyType != typeof(long?))
                throw new System.InvalidOperationException("Il tipo del lookup SharePoint non e' Nullable<long>.");

            var lista = new System.ComponentModel.BindingList<Agente>(
                new System.Collections.Generic.List<Agente> { new Agente { ID = 7, Title = "Demo" } });
            if (lista.Count != 1 || lista[0].ID != 7)
                throw new System.InvalidOperationException("BindingList<Model> non funzionante.");
        }

        private static void AssertMap(System.Type tipo, params string[] colonne)
        {
            Dapper.SqlMapper.ITypeMap mapping = Dapper.SqlMapper.GetTypeMap(tipo);
            foreach (string colonna in colonne)
            {
                if (mapping.GetMember(colonna) == null)
                    throw new System.InvalidOperationException(
                        "Dapper non associa la colonna " + colonna + " al Model " + tipo.Name);
            }
        }
    }
}
