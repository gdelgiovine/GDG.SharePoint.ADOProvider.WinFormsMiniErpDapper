// Avvio convenzionale WinForms, come in un progetto Visual Basic tradizionale.
using System;
using System.Windows.Forms;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args != null && args.Length == 1 &&
                string.Equals(args[0], "--self-test", StringComparison.OrdinalIgnoreCase))
            {
                // Verifica la presenza del pacchetto Dapper su entrambi i framework.
                if (typeof(Dapper.SqlMapper).Assembly.GetName().Name != "Dapper")
                    throw new InvalidOperationException("Dapper non disponibile.");
                ModelMappingSelfTest.Run();
                if (DatiSharePoint.TabellaAllegati(0) != "$ATTACHMENTS([TutorialAgents])" ||
                    DatiSharePoint.TabellaAllegati(1) != "$ATTACHMENTS([TutorialCustomers])" ||
                    DatiSharePoint.TabellaAllegati(2) != "$ATTACHMENTS([TutorialOrders])")
                    throw new InvalidOperationException("Mapping liste non valido.");
                DatiSharePoint.ControllaNomeFile("documento.pdf");
                try
                {
                    DatiSharePoint.ControllaNomeFile("../non-sicuro.pdf");
                    throw new InvalidOperationException("Nome pericoloso non bloccato.");
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message == "Nome pericoloso non bloccato.") throw;
                }
                return;
            }
#if NET48
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#else
            ApplicationConfiguration.Initialize();
#endif
            Application.Run(new MainForm());
        }
    }
}
