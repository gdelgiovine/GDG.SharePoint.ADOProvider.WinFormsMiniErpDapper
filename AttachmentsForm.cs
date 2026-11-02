// Gestione allegati tramite Dapper e Model Allegato (niente DataTable).
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    public partial class AttachmentsForm : Form
    {
        private DatiSharePoint dati;
        private int tipoLista;
        private long idRecord;

        public AttachmentsForm()
        {
            InitializeComponent();
        }

        internal AttachmentsForm(DatiSharePoint archivio, int tipo, long id, string titolo) : this()
        {
            dati = archivio;
            tipoLista = tipo;
            idRecord = id;
            Text = "Allegati - " + titolo;
        }

        private void AttachmentsForm_Shown(object sender, EventArgs e)
        {
            if (dati != null) Aggiorna();
        }

        private void Aggiorna()
        {
            try
            {
                grdAllegati.DataSource = new BindingList<Allegato>(dati.CaricaAllegati(tipoLista, idRecord));
                if (grdAllegati.Columns.Contains("ItemId"))
                    grdAllegati.Columns["ItemId"].Visible = false;
                lblStato.Text = "Allegati caricati";
            }
            catch (Exception ex)
            {
                MostraErrore(ex);
            }
        }

        private string NomeSelezionato()
        {
            Allegato selezionato = grdAllegati.CurrentRow == null
                ? null : grdAllegati.CurrentRow.DataBoundItem as Allegato;
            return selezionato == null ? null : selezionato.FileName;
        }

        private void grdAllegati_SelectionChanged(object sender, EventArgs e)
        {
            string nome = NomeSelezionato();
            if (!string.IsNullOrEmpty(nome)) txtNome.Text = nome;
        }

        private byte[] LeggiFile(string percorso)
        {
            FileInfo info = new FileInfo(percorso);
            if (info.Length > DatiSharePoint.DimensioneMassimaAllegato)
                throw new InvalidOperationException("Dimensione massima 20 MiB.");
            return File.ReadAllBytes(percorso);
        }

        private void btnAggiungi_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                string nome = Path.GetFileName(dlg.FileName);
                BindingList<Allegato> elenco = grdAllegati.DataSource as BindingList<Allegato>;
                if (elenco != null)
                {
                    foreach (Allegato allegato in elenco)
                    {
                        if (string.Equals(allegato.FileName, nome,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show(this, "Il file esiste già. Usare Sostituisci.");
                            return;
                        }
                    }
                }
                try
                {
                    dati.AggiungiAllegato(tipoLista, idRecord, nome, LeggiFile(dlg.FileName));
                    Aggiorna();
                }
                catch (Exception ex) { MostraErrore(ex); }
            }
        }

        private void btnScarica_Click(object sender, EventArgs e)
        {
            string nome = NomeSelezionato();
            if (nome == null) return;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.FileName = nome;
                dlg.OverwritePrompt = true;
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    File.WriteAllBytes(dlg.FileName, dati.ScaricaAllegato(tipoLista, idRecord, nome));
                    lblStato.Text = "Salvato: " + nome;
                }
                catch (Exception ex) { MostraErrore(ex); }
            }
        }

        private void btnSostituisci_Click(object sender, EventArgs e)
        {
            string nome = NomeSelezionato();
            if (nome == null) return;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                if (MessageBox.Show(this, "Sostituire il contenuto di '" + nome + "'?",
                    "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
                try
                {
                    dati.SostituisciAllegato(tipoLista, idRecord, nome, LeggiFile(dlg.FileName));
                    Aggiorna();
                }
                catch (Exception ex) { MostraErrore(ex); }
            }
        }

        private void btnRinomina_Click(object sender, EventArgs e)
        {
            string nome = NomeSelezionato();
            if (nome == null || string.IsNullOrWhiteSpace(txtNome.Text)) return;
            if (string.Equals(nome, txtNome.Text.Trim(), StringComparison.Ordinal)) return;
            try
            {
                dati.RinominaAllegato(tipoLista, idRecord, nome, txtNome.Text.Trim());
                Aggiorna();
            }
            catch (Exception ex) { MostraErrore(ex); }
        }

        private void btnElimina_Click(object sender, EventArgs e)
        {
            string nome = NomeSelezionato();
            if (nome == null) return;
            if (MessageBox.Show(this, "Eliminare definitivamente '" + nome + "'?",
                "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            try
            {
                dati.EliminaAllegato(tipoLista, idRecord, nome);
                Aggiorna();
            }
            catch (Exception ex) { MostraErrore(ex); }
        }

        private void MostraErrore(Exception ex)
        {
            lblStato.Text = "Operazione non riuscita";
            MessageBox.Show(this, ex.Message, "Allegati", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
