// Query di reportistica: raggruppamenti e navigazione eseguiti da SharePoint (JOIN / GROUP BY).
using System.Collections.Generic;
using System.Linq;
using Dapper;
using GDG.SharePoint.ADOProvider;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    internal class GruppoReport
    {
        public long Id { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public long Conteggio { get; set; }
        public decimal Totale { get; set; }
    }

    internal class RigaReport
    {
        public int LineNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductTitle { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal LineAmount { get; set; }
    }

    internal partial class DatiSharePoint
    {
        // vista: 0 = cliente, 1 = agente, 2 = prodotto
        internal static string SqlGruppi(int vista)
        {
            if (vista == 0)
                return "SELECT C.ID AS Id, C.Code AS Codice, C.Title AS Descrizione, COUNT(O.ID) AS Conteggio, SUM(O.Amount) AS Totale\r\n" +
                       "FROM [TutorialCustomers] C\r\n" +
                       "INNER JOIN [TutorialOrders] O ON O.Customer$Id = C.ID\r\n" +
                       "GROUP BY C.ID, C.Code, C.Title\r\n" +
                       "ORDER BY C.Title";
            if (vista == 1)
                return "SELECT A.ID AS Id, A.AgentCode AS Codice, A.Title AS Descrizione, COUNT(O.ID) AS Conteggio, SUM(O.Amount) AS Totale\r\n" +
                       "FROM [TutorialAgents] A\r\n" +
                       "INNER JOIN [TutorialOrders] O ON O.Agent$Id = A.ID\r\n" +
                       "GROUP BY A.ID, A.AgentCode, A.Title\r\n" +
                       "ORDER BY A.Title";
            return "SELECT P.ID AS Id, P.Code AS Codice, P.Title AS Descrizione, COUNT(L.ID) AS Conteggio, SUM(L.LineAmount) AS Totale\r\n" +
                   "FROM [TutorialProducts] P\r\n" +
                   "INNER JOIN [TutorialOrderLines] L ON L.Product$Id = P.ID\r\n" +
                   "GROUP BY P.ID, P.Code, P.Title\r\n" +
                   "ORDER BY P.Title";
        }

        internal static string SqlOrdiniGruppo(int vista)
        {
            const string colonne = "O.ID, O.Title, O.Customer$Id AS CustomerId, O.Agent$Id AS AgentId, " +
                                   "O.CustomerCode, O.AgentCode, O.Status, O.Amount\r\n";
            if (vista == 0)
                return "SELECT " + colonne + "FROM [TutorialOrders] O\r\nWHERE O.Customer$Id = @Id\r\nORDER BY O.Title";
            if (vista == 1)
                return "SELECT " + colonne + "FROM [TutorialOrders] O\r\nWHERE O.Agent$Id = @Id\r\nORDER BY O.Title";
            return "SELECT " + colonne + "FROM [TutorialOrders] O\r\n" +
                   "INNER JOIN [TutorialOrderLines] L ON L.OrderRef$Id = O.ID\r\n" +
                   "WHERE L.Product$Id = @Id\r\nORDER BY O.Title";
        }

        internal static string SqlRigheOrdine()
        {
            return "SELECT L.LineNumber, P.Code AS ProductCode, P.Title AS ProductTitle, L.Quantity, L.UnitPrice,\r\n" +
                   "       L.DiscountPercent, L.LineAmount\r\n" +
                   "FROM [TutorialOrderLines] L\r\n" +
                   "INNER JOIN [TutorialProducts] P ON L.Product$Id = P.ID\r\n" +
                   "WHERE L.OrderRef$Id = @Id\r\nORDER BY L.LineNumber";
        }

        public List<GruppoReport> ReportGruppi(int vista)
        {
            using (SharePointConnection cn = ApriConnessione())
                return cn.Query<GruppoReport>(SqlGruppi(vista)).ToList();
        }

        public List<Ordine> ReportOrdini(int vista, long id)
        {
            using (SharePointConnection cn = ApriConnessione())
                return cn.Query<Ordine>(SqlOrdiniGruppo(vista), new { Id = id }).ToList();
        }

        public List<RigaReport> ReportRighe(long ordineId)
        {
            using (SharePointConnection cn = ApriConnessione())
                return cn.Query<RigaReport>(SqlRigheOrdine(), new { Id = ordineId }).ToList();
        }
    }
}
