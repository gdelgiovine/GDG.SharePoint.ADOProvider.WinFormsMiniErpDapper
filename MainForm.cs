// Interfaccia WinForms: BindingSource collegato direttamente a BindingList<Model>.
// Non ci sono DataTable, DataRow o DataRowView nel progetto Dapper.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using GDG.SharePoint.ADOProvider.WinFormsTutorial;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    public partial class MainForm : Form
    {
        private TutorialConnectionStore archivioConnessioni;
        private TutorialConnectionProfile profilo;
        private readonly InteractiveBrowserAuthenticator browser = new InteractiveBrowserAuthenticator();
        private DatiSharePoint dati;
        private string connessioneDiretta = "";
        private bool collegamentoInCorso;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            try
            {
                archivioConnessioni = TutorialConnectionStore.LoadDefault();
                profilo = archivioConnessioni.FindSelectedProfile();
                connessioneDiretta = Environment.GetEnvironmentVariable("GDG_SP_CONNECTION") ?? "";
                cmbListe.SelectedIndex = 0;
                MostraConnessione();
            }
            catch (Exception ex)
            {
                archivioConnessioni = new TutorialConnectionStore();
                MostraErrore(ex);
            }
        }

        private int TipoLista()
        {
            return cmbListe.SelectedIndex;
        }

        private void MostraConnessione()
        {
            if (profilo != null)
                lblConnessione.Text = "Profilo: " + profilo.Name + " - " + profilo.SiteUrl;
            else if (!string.IsNullOrWhiteSpace(connessioneDiretta))
                lblConnessione.Text = "Connessione da GDG_SP_CONNECTION";
            else
                lblConnessione.Text = "Selezionare Connessioni...";
        }

        private void btnConnessioni_Click(object sender, EventArgs e)
        {
            if (collegamentoInCorso) return;
            using (ConnectionManagerForm finestra =
                new ConnectionManagerForm(archivioConnessioni, profilo, connessioneDiretta, browser))
            {
                if (finestra.ShowDialog(this) != DialogResult.OK) return;
                profilo = finestra.SelectedProfile;
                connessioneDiretta = finestra.UseRawConnectionString ? finestra.RawConnectionString : "";
                dati = null;
                bsDati.DataSource = null;
                MostraConnessione();
            }
        }

        private async void btnConnetti_Click(object sender, EventArgs e)
        {
            if (collegamentoInCorso) return;
            collegamentoInCorso = true;
            btnConnetti.Enabled = false;
            btnConnessioni.Enabled = false;
            try
            {
                string cs = connessioneDiretta.Trim();
                if (profilo != null)
                    cs = await TutorialConnectionStringFactory.BuildAsync(profilo, browser);

                if (string.IsNullOrWhiteSpace(cs))
                {
                    MessageBox.Show(this, "Selezionare una connessione.");
                    return;
                }

                dati = new DatiSharePoint(cs);

                DemoProvisioning provisioning = new DemoProvisioning(cs);
                StatoProvisioning stato = provisioning.Analizza();
                if (stato.RichiedeAzione &&
                    MessageBox.Show(this,
                        "Alcune liste demo sono mancanti o vuote:" + Environment.NewLine +
                        stato.Descrizione() + Environment.NewLine + Environment.NewLine +
                        "Creare le liste mancanti e caricare i dati iniziali? " +
                        "Le liste gia' valorizzate non verranno modificate.",
                        "Provisioning demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                {
                    provisioning.Esegui();
                }

                CaricaElenco();
            }
            catch (Exception ex)
            {
                dati = null;
                MostraErrore(ex);
            }
            finally
            {
                collegamentoInCorso = false;
                btnConnetti.Enabled = true;
                btnConnessioni.Enabled = true;
            }
        }

        private void cmbListe_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFiltro.Clear();
            filtriAttivi.Clear();
            RicaricaElencoFiltri();
            PopolaColonne();
            if (dati != null && !collegamentoInCorso) Aggiorna();
        }

        private void btnAggiorna_Click(object sender, EventArgs e)
        {
            Aggiorna();
        }

        private void Aggiorna()
        {
            if (dati == null)
            {
                MessageBox.Show(this, "Prima collegarsi a SharePoint.");
                return;
            }
            try
            {
                CaricaElenco();
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        private readonly BindingList<CondizioneFiltro> filtriAttivi = new BindingList<CondizioneFiltro>();

        private void btnAggiungiFiltro_Click(object sender, EventArgs e)
        {
            try
            {
                CondizioneFiltro c = CondizioneCorrente();
                if (c == null) return;
                filtriAttivi.Add(c);
                RicaricaElencoFiltri();
                txtFiltro.Clear();
                txtFiltro.Focus();
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        private void btnRimuoviFiltro_Click(object sender, EventArgs e)
        {
            int i = lstFiltri.SelectedIndex;
            if (i < 0) return;
            filtriAttivi.RemoveAt(i);
            RicaricaElencoFiltri();
            if (dati != null) Aggiorna();
        }

        private bool aggiornandoElencoFiltri;

        private void RicaricaElencoFiltri()
        {
            aggiornandoElencoFiltri = true;
            try
            {
                lstFiltri.DataSource = null;
                lstFiltri.DataSource = filtriAttivi;
                lstFiltri.DisplayMember = "Descrizione";
                lstFiltri.SelectedIndex = -1;
            }
            finally
            {
                aggiornandoElencoFiltri = false;
            }
        }

        // La condizione selezionata viene riproposta nei controlli di definizione.
        private void lstFiltri_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (aggiornandoElencoFiltri) return;
            int i = lstFiltri.SelectedIndex;
            if (i < 0 || i >= filtriAttivi.Count) return;
            CondizioneFiltro c = filtriAttivi[i];

            cmbOperatore.SelectedItem = c.Operatore;
            for (int k = 0; k < cmbColonna.Items.Count; k++)
            {
                ColonnaFiltro col = cmbColonna.Items[k] as ColonnaFiltro;
                if (col != null && col.Nome == c.Colonna) { cmbColonna.SelectedIndex = k; break; }
            }
            cmbConfronto.SelectedItem = c.Confronto;
            txtFiltro.Text = c.Testo;
        }

        private void btnMostraSql_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = DatiSharePoint.DescriviQuery(TipoLista(), FiltriDaApplicare());
                using (Form f = new Form())
                {
                    f.Text = "Query SQL";
                    f.StartPosition = FormStartPosition.CenterParent;
                    f.Size = new System.Drawing.Size(720, 320);
                    TextBox t = new TextBox
                    {
                        Multiline = true,
                        ReadOnly = true,
                        ScrollBars = ScrollBars.Both,
                        WordWrap = false,
                        Dock = DockStyle.Fill,
                        Font = new System.Drawing.Font("Consolas", 10F),
                        Text = sql.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)
                    };
                    Button copia = new Button { Text = "Copia", Dock = DockStyle.Bottom, Height = 32 };
                    copia.Click += (s, a) => Clipboard.SetText(t.Text);
                    f.Controls.Add(t);
                    f.Controls.Add(copia);
                    t.Select(0, 0);
                    f.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        private void btnAggiornaFiltro_Click(object sender, EventArgs e)
        {
            int i = lstFiltri.SelectedIndex;
            if (i < 0 || i >= filtriAttivi.Count)
            {
                MessageBox.Show(this, "Selezionare una condizione dall'elenco.");
                return;
            }
            try
            {
                CondizioneFiltro c = CondizioneCorrente();
                if (c == null) return;
                filtriAttivi[i] = c;
                RicaricaElencoFiltri();
                txtFiltro.Clear();
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        // Condizione in composizione nei controlli (null se il testo e' vuoto).
        private CondizioneFiltro CondizioneCorrente()
        {
            string testo = txtFiltro.Text.Trim();
            ColonnaFiltro col = cmbColonna.SelectedItem as ColonnaFiltro;
            if (testo.Length == 0 || col == null || cmbConfronto.SelectedItem == null) return null;

            object valore;
            System.Data.DbType tipo;
            if (!col.TryConverti(testo, out valore, out tipo))
                throw new InvalidOperationException(
                    "Valore non valido per " + col.Nome + " (" + col.Tipo + "): " + testo);

            return new CondizioneFiltro
            {
                Operatore = (string)cmbOperatore.SelectedItem,
                Colonna = col.Nome,
                Confronto = (string)cmbConfronto.SelectedItem,
                Testo = testo
            };
        }

        private System.Collections.Generic.List<CondizioneFiltro> FiltriDaApplicare()
        {
            System.Collections.Generic.List<CondizioneFiltro> l =
                new System.Collections.Generic.List<CondizioneFiltro>(filtriAttivi);
            CondizioneFiltro c = CondizioneCorrente();
            int sel = lstFiltri.SelectedIndex;
            if (c != null)
            {
                if (sel >= 0 && sel < l.Count) l[sel] = c;
                else l.Add(c);
            }
            return l;
        }

        private void txtFiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            Aggiorna();
        }

        private void btnFiltra_Click(object sender, EventArgs e)
        {
            Aggiorna();
        }

        private void btnPulisciFiltro_Click(object sender, EventArgs e)
        {
            txtFiltro.Clear();
            filtriAttivi.Clear();
            RicaricaElencoFiltri();
            if (dati != null) Aggiorna();
        }

        private void PopolaColonne()
        {
            ColonnaFiltro[] colonne = TipoLista() == 0 ? DatiSharePoint.ColonneAgenti
                : TipoLista() == 1 ? DatiSharePoint.ColonneClienti
                : TipoLista() == 2 ? DatiSharePoint.ColonneOrdini
                : TipoLista() == 3 ? DatiSharePoint.ColonneProdotti
                : DatiSharePoint.ColonneRighe;
            cmbColonna.Items.Clear();
            cmbColonna.Items.AddRange(colonne);
            cmbColonna.SelectedIndex = 1;
        }

        private void cmbColonna_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColonnaFiltro col = cmbColonna.SelectedItem as ColonnaFiltro;
            if (col == null) return;
            cmbConfronto.Items.Clear();
            cmbConfronto.Items.AddRange(col.Confronti);
            cmbConfronto.SelectedIndex = 0;
        }

        private readonly System.Collections.Generic.Dictionary<long, string> lookupAgenti =
            new System.Collections.Generic.Dictionary<long, string>();
        private readonly System.Collections.Generic.Dictionary<long, string> lookupClienti =
            new System.Collections.Generic.Dictionary<long, string>();

        private readonly System.Collections.Generic.Dictionary<long, string> lookupOrdini =
            new System.Collections.Generic.Dictionary<long, string>();
        private readonly System.Collections.Generic.Dictionary<long, string> lookupProdotti =
            new System.Collections.Generic.Dictionary<long, string>();

        // Colonna lookup (valore + ID) calcolata dalla colonna ID, che viene nascosta.
        private void AggiungiColonnaLookup(string intestazione, string colonnaId,
            System.Collections.Generic.Dictionary<long, string> mappa)
        {
            grdDati.Columns[colonnaId].Visible = false;
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = "lookup" + intestazione;
            col.HeaderText = intestazione;
            col.ReadOnly = true;
            col.Tag = colonnaId;
            col.DisplayIndex = grdDati.Columns[colonnaId].DisplayIndex;
            grdDati.Columns.Add(col);
            for (int i = 0; i < grdDati.Rows.Count; i++)
            {
                object v = grdDati.Rows[i].Cells[colonnaId].Value;
                string testo = "";
                if (v != null && mappa.TryGetValue(Convert.ToInt64(v), out testo))
                    grdDati.Rows[i].Cells[col.Name].Value = testo;
                else if (v != null)
                    grdDati.Rows[i].Cells[col.Name].Value = "[ID " + v + "]";
            }
        }

        private void CaricaElenco()
        {
            System.Collections.Generic.List<CondizioneFiltro> filtri = FiltriDaApplicare();

            // La query LIKE con AND/OR viene eseguita da SharePoint; ogni riga e' un Agente, Cliente o Ordine.
            var nessunFiltro = new System.Collections.Generic.List<CondizioneFiltro>();
            lookupAgenti.Clear();
            lookupClienti.Clear();
            if (TipoLista() == 3)
                bsDati.DataSource = new BindingList<Prodotto>(dati.CaricaProdotti(filtri));
            else if (TipoLista() == 4)
            {
                lookupOrdini.Clear();
                lookupProdotti.Clear();
                foreach (Ordine o in dati.CaricaOrdini(nessunFiltro)) lookupOrdini[o.ID] = o.Title;
                foreach (Prodotto pr in dati.CaricaProdotti(nessunFiltro)) lookupProdotti[pr.ID] = pr.Code;
                bsDati.DataSource = new BindingList<RigaOrdine>(dati.CaricaRigheOrdine(filtri));
            }
            else if (TipoLista() == 0)
                bsDati.DataSource = new BindingList<Agente>(dati.CaricaAgenti(filtri));
            else
            {
                foreach (Agente a in dati.CaricaAgenti(nessunFiltro))
                    lookupAgenti[a.ID] = a.AgentCode;
                if (TipoLista() == 1)
                    bsDati.DataSource = new BindingList<Cliente>(dati.CaricaClienti(filtri));
                else
                {
                    foreach (Cliente c in dati.CaricaClienti(nessunFiltro))
                        lookupClienti[c.ID] = c.Code;
                    bsDati.DataSource = new BindingList<Ordine>(dati.CaricaOrdini(filtri));
                }
            }

            grdDati.DataSource = null;
            grdDati.Columns.Clear();
            grdDati.DataSource = bsDati;
            if (TipoLista() == 1) AggiungiColonnaLookup("AccountAgent", "AccountAgentId", lookupAgenti);
            else if (TipoLista() == 4)
            {
                AggiungiColonnaLookup("OrderRef", "OrderId", lookupOrdini);
                AggiungiColonnaLookup("Product", "ProductId", lookupProdotti);
                if (grdDati.Columns.Contains("LineAmount"))
                    grdDati.Columns["LineAmount"].DefaultCellStyle.Format = "N2";
            }
            else if (TipoLista() == 3 && grdDati.Columns.Contains("UnitPrice"))
                grdDati.Columns["UnitPrice"].DefaultCellStyle.Format = "N2";
            else if (TipoLista() == 2)
            {
                AggiungiColonnaLookup("Customer", "CustomerId", lookupClienti);
                AggiungiColonnaLookup("Agent", "AgentId", lookupAgenti);
            }
            if (grdDati.Columns.Contains("ID"))
                grdDati.Columns["ID"].FillWeight = 35;
            if (grdDati.Columns.Contains("Amount"))
                grdDati.Columns["Amount"].DefaultCellStyle.Format = "N2";
            // Oltre ai lookup (Customer/Agent/AccountAgent) restano visibili anche ID e codici.
            lblStato.Text = (filtri.Count == 0 ? "Record caricati: " : "Record filtrati: ") + bsDati.Count;
        }

        private void btnNuovo_Click(object sender, EventArgs e)
        {
            ApriScheda(true);
        }

        private void btnModifica_Click(object sender, EventArgs e)
        {
            ApriScheda(false);
        }

        private void grdDati_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) ApriScheda(false);
        }

        private void ApriScheda(bool nuovo)
        {
            if (dati == null) return;
            object selezionato = nuovo ? null : bsDati.Current;
            if (!nuovo && selezionato == null) return;

            try
            {
                EditForm scheda;
                if (TipoLista() == 0)
                {
                    Agente agente = nuovo ? new Agente() : selezionato as Agente;
                    if (agente == null) return;
                    scheda = new EditForm(agente);
                }
                else if (TipoLista() == 1)
                {
                    Cliente cliente = nuovo ? new Cliente() : selezionato as Cliente;
                    if (cliente == null) return;
                    scheda = new EditForm(cliente, dati.CaricaAgenti());
                }
                else if (TipoLista() == 2)
                {
                    Ordine ordine = nuovo ? new Ordine() : selezionato as Ordine;
                    if (ordine == null) return;
                    scheda = new EditForm(ordine, dati.CaricaAgenti(), dati.CaricaClienti());
                }
                else if (TipoLista() == 3)
                {
                    Prodotto prodotto = nuovo ? new Prodotto() : selezionato as Prodotto;
                    if (prodotto == null) return;
                    using (CatalogoForm cat = new CatalogoForm(prodotto))
                    {
                        if (cat.ShowDialog(this) != DialogResult.OK) return;
                        dati.SalvaProdotto(cat.ProdottoModificato);
                    }
                    CaricaElenco();
                    return;
                }
                else if (TipoLista() == 4)
                {
                    RigaOrdine riga = nuovo ? new RigaOrdine() : selezionato as RigaOrdine;
                    if (riga == null) return;
                    using (CatalogoForm cat = new CatalogoForm(riga, dati.CaricaOrdini(), dati.CaricaProdotti()))
                    {
                        if (cat.ShowDialog(this) != DialogResult.OK) return;
                        dati.SalvaRigaOrdine(cat.RigaModificata);
                    }
                    CaricaElenco();
                    return;
                }
                else
                {
                    return;
                }

                using (scheda)
                {
                    if (scheda.ShowDialog(this) != DialogResult.OK) return;

                    // Il risultato della scheda e' un vero Model C#.
                    if (TipoLista() == 0)
                        dati.SalvaAgente(scheda.AgenteModificato);
                    else if (TipoLista() == 1)
                        dati.SalvaCliente(scheda.ClienteModificato);
                    else
                        dati.SalvaOrdine(scheda.OrdineModificato);
                }
                CaricaElenco();
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        private void btnElimina_Click(object sender, EventArgs e)
        {
            if (dati == null) return;

            Agente agente = bsDati.Current as Agente;
            Cliente cliente = bsDati.Current as Cliente;
            Ordine ordine = bsDati.Current as Ordine;
            Prodotto prodotto = bsDati.Current as Prodotto;
            RigaOrdine riga = bsDati.Current as RigaOrdine;
            if (agente == null && cliente == null && ordine == null && prodotto == null && riga == null) return;

            string titolo = agente != null ? agente.Title :
                cliente != null ? cliente.Title :
                ordine != null ? ordine.Title :
                prodotto != null ? prodotto.Title : riga.Title;
            if (MessageBox.Show(this,
                "Eliminare '" + titolo +
                "' e i relativi allegati?\r\nNon saranno eliminati i record collegati.",
                "Conferma eliminazione", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                if (agente != null) dati.EliminaAgente(agente);
                else if (cliente != null) dati.EliminaCliente(cliente);
                else if (ordine != null) dati.EliminaOrdine(ordine);
                else if (prodotto != null) dati.EliminaProdotto(prodotto);
                else dati.EliminaRigaOrdine(riga);
                CaricaElenco();
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (dati == null)
            {
                MessageBox.Show(this, "Prima collegarsi a SharePoint.");
                return;
            }
            try
            {
                using (ReportForm f = new ReportForm(dati))
                    f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        private void btnAllegati_Click(object sender, EventArgs e)
        {
            if (dati == null) return;

            Agente agente = bsDati.Current as Agente;
            Cliente cliente = bsDati.Current as Cliente;
            Ordine ordine = bsDati.Current as Ordine;
            Prodotto prodotto = bsDati.Current as Prodotto;
            RigaOrdine riga = bsDati.Current as RigaOrdine;

            long id;
            string titolo;
            if (agente != null) { id = agente.ID; titolo = agente.Title; }
            else if (cliente != null) { id = cliente.ID; titolo = cliente.Title; }
            else if (ordine != null) { id = ordine.ID; titolo = ordine.Title; }
            else if (prodotto != null) { id = prodotto.ID; titolo = prodotto.Title; }
            else if (riga != null) { id = riga.ID; titolo = riga.Title; }
            else return;

            using (AttachmentsForm allegati = new AttachmentsForm(dati, TipoLista(), id, titolo))
                allegati.ShowDialog(this);
        }

        private void MostraErrore(Exception ex)
        {
            lblStato.Text = "Errore. Verificare connessione e liste Tutorial.";
            MessageBox.Show(this, ex.Message, "MiniERP Dapper",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
