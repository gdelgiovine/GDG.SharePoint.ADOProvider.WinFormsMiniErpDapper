namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    partial class AttachmentsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView grdAllegati;
        private System.Windows.Forms.Label lblIstruzioni;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Button btnAggiungi;
        private System.Windows.Forms.Button btnScarica;
        private System.Windows.Forms.Button btnSostituisci;
        private System.Windows.Forms.Button btnRinomina;
        private System.Windows.Forms.Button btnElimina;
        private System.Windows.Forms.Button btnChiudi;
        private System.Windows.Forms.Label lblStato;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grdAllegati = new System.Windows.Forms.DataGridView();
            this.lblIstruzioni = new System.Windows.Forms.Label();
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.btnAggiungi = new System.Windows.Forms.Button();
            this.btnScarica = new System.Windows.Forms.Button();
            this.btnSostituisci = new System.Windows.Forms.Button();
            this.btnRinomina = new System.Windows.Forms.Button();
            this.btnElimina = new System.Windows.Forms.Button();
            this.btnChiudi = new System.Windows.Forms.Button();
            this.lblStato = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdAllegati)).BeginInit();
            this.SuspendLayout();
            //
            // lblIstruzioni
            //
            this.lblIstruzioni.Location = new System.Drawing.Point(16, 15);
            this.lblIstruzioni.Size = new System.Drawing.Size(710, 36);
            this.lblIstruzioni.Text = "Selezionare un allegato. Dimensione massima caricamento: 20 MiB.";
            //
            // grdAllegati
            //
            this.grdAllegati.Location = new System.Drawing.Point(16, 55);
            this.grdAllegati.Size = new System.Drawing.Size(710, 290);
            this.grdAllegati.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right |
                System.Windows.Forms.AnchorStyles.Bottom;
            this.grdAllegati.AllowUserToAddRows = false;
            this.grdAllegati.AllowUserToDeleteRows = false;
            this.grdAllegati.ReadOnly = true;
            this.grdAllegati.MultiSelect = false;
            this.grdAllegati.RowHeadersVisible = false;
            this.grdAllegati.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdAllegati.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdAllegati.SelectionChanged += new System.EventHandler(this.grdAllegati_SelectionChanged);
            //
            // lblNome
            //
            this.lblNome.Location = new System.Drawing.Point(16, 366);
            this.lblNome.Size = new System.Drawing.Size(115, 25);
            this.lblNome.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
            this.lblNome.Text = "Nome allegato:";
            //
            // txtNome
            //
            this.txtNome.Location = new System.Drawing.Point(135, 362);
            this.txtNome.Size = new System.Drawing.Size(591, 28);
            this.txtNome.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Right;
            this.txtNome.MaxLength = 255;
            //
            // btnAggiungi
            //
            this.btnAggiungi.Location = new System.Drawing.Point(16, 410);
            this.btnAggiungi.Size = new System.Drawing.Size(111, 33);
            this.btnAggiungi.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAggiungi.Text = "Aggiungi";
            this.btnAggiungi.Click += new System.EventHandler(this.btnAggiungi_Click);
            //
            // btnScarica
            //
            this.btnScarica.Location = new System.Drawing.Point(135, 410);
            this.btnScarica.Size = new System.Drawing.Size(111, 33);
            this.btnScarica.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnScarica.Text = "Scarica";
            this.btnScarica.Click += new System.EventHandler(this.btnScarica_Click);
            //
            // btnSostituisci
            //
            this.btnSostituisci.Location = new System.Drawing.Point(254, 410);
            this.btnSostituisci.Size = new System.Drawing.Size(111, 33);
            this.btnSostituisci.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSostituisci.Text = "Sostituisci";
            this.btnSostituisci.Click += new System.EventHandler(this.btnSostituisci_Click);
            //
            // btnRinomina
            //
            this.btnRinomina.Location = new System.Drawing.Point(373, 410);
            this.btnRinomina.Size = new System.Drawing.Size(111, 33);
            this.btnRinomina.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRinomina.Text = "Rinomina";
            this.btnRinomina.Click += new System.EventHandler(this.btnRinomina_Click);
            //
            // btnElimina
            //
            this.btnElimina.Location = new System.Drawing.Point(492, 410);
            this.btnElimina.Size = new System.Drawing.Size(111, 33);
            this.btnElimina.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnElimina.Text = "Elimina";
            this.btnElimina.Click += new System.EventHandler(this.btnElimina_Click);
            //
            // btnChiudi
            //
            this.btnChiudi.Location = new System.Drawing.Point(615, 410);
            this.btnChiudi.Size = new System.Drawing.Size(111, 33);
            this.btnChiudi.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnChiudi.Text = "Chiudi";
            this.btnChiudi.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //
            // lblStato
            //
            this.lblStato.Location = new System.Drawing.Point(16, 458);
            this.lblStato.Size = new System.Drawing.Size(710, 26);
            this.lblStato.Anchor = System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.lblStato.Text = "Pronto";
            //
            // AttachmentsForm
            //
            this.ClientSize = new System.Drawing.Size(743, 492);
            this.MinimumSize = new System.Drawing.Size(743, 492);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Allegati";
            this.CancelButton = this.btnChiudi;
            this.Controls.Add(this.lblIstruzioni);
            this.Controls.Add(this.grdAllegati);
            this.Controls.Add(this.lblNome);
            this.Controls.Add(this.txtNome);
            this.Controls.Add(this.btnAggiungi);
            this.Controls.Add(this.btnScarica);
            this.Controls.Add(this.btnSostituisci);
            this.Controls.Add(this.btnRinomina);
            this.Controls.Add(this.btnElimina);
            this.Controls.Add(this.btnChiudi);
            this.Controls.Add(this.lblStato);
            this.Shown += new System.EventHandler(this.AttachmentsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.grdAllegati)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
