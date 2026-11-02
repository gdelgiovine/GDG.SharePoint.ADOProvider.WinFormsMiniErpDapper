namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    partial class EditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitolo;
        private System.Windows.Forms.TextBox txtTitolo;
        private System.Windows.Forms.Label lblCodice;
        private System.Windows.Forms.TextBox txtCodice;
        private System.Windows.Forms.Label lblZona;
        private System.Windows.Forms.TextBox txtZona;
        private System.Windows.Forms.CheckBox chkAttivo;
        private System.Windows.Forms.Label lblAgente;
        private System.Windows.Forms.ComboBox cboAgente;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.Label lblStato;
        private System.Windows.Forms.ComboBox cboStato;
        private System.Windows.Forms.Label lblImporto;
        private System.Windows.Forms.NumericUpDown numImporto;
        private System.Windows.Forms.Button btnSalva;
        private System.Windows.Forms.Button btnAnnulla;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitolo = new System.Windows.Forms.Label();
            this.txtTitolo = new System.Windows.Forms.TextBox();
            this.lblCodice = new System.Windows.Forms.Label();
            this.txtCodice = new System.Windows.Forms.TextBox();
            this.lblZona = new System.Windows.Forms.Label();
            this.txtZona = new System.Windows.Forms.TextBox();
            this.chkAttivo = new System.Windows.Forms.CheckBox();
            this.lblAgente = new System.Windows.Forms.Label();
            this.cboAgente = new System.Windows.Forms.ComboBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.lblStato = new System.Windows.Forms.Label();
            this.cboStato = new System.Windows.Forms.ComboBox();
            this.lblImporto = new System.Windows.Forms.Label();
            this.numImporto = new System.Windows.Forms.NumericUpDown();
            this.btnSalva = new System.Windows.Forms.Button();
            this.btnAnnulla = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numImporto)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitolo
            //
            this.lblTitolo.Location = new System.Drawing.Point(20, 22);
            this.lblTitolo.Size = new System.Drawing.Size(165, 25);
            this.lblTitolo.Text = "Titolo *";
            //
            // txtTitolo
            //
            this.txtTitolo.Location = new System.Drawing.Point(190, 18);
            this.txtTitolo.Size = new System.Drawing.Size(365, 28);
            this.txtTitolo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtTitolo.MaxLength = 255;
            //
            // lblCodice
            //
            this.lblCodice.Location = new System.Drawing.Point(20, 71);
            this.lblCodice.Size = new System.Drawing.Size(165, 25);
            this.lblCodice.Text = "Codice agente *";
            //
            // txtCodice
            //
            this.txtCodice.Location = new System.Drawing.Point(190, 67);
            this.txtCodice.Size = new System.Drawing.Size(365, 28);
            this.txtCodice.MaxLength = 50;
            this.txtCodice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            //
            // lblZona
            //
            this.lblZona.Location = new System.Drawing.Point(20, 120);
            this.lblZona.Size = new System.Drawing.Size(165, 25);
            this.lblZona.Text = "Regione";
            //
            // txtZona
            //
            this.txtZona.Location = new System.Drawing.Point(190, 116);
            this.txtZona.Size = new System.Drawing.Size(365, 28);
            this.txtZona.MaxLength = 50;
            this.txtZona.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            //
            // chkAttivo
            //
            this.chkAttivo.Location = new System.Drawing.Point(190, 162);
            this.chkAttivo.Size = new System.Drawing.Size(160, 27);
            this.chkAttivo.Text = "Attivo";
            this.chkAttivo.Checked = true;
            //
            // lblAgente
            //
            this.lblAgente.Location = new System.Drawing.Point(20, 210);
            this.lblAgente.Size = new System.Drawing.Size(165, 25);
            this.lblAgente.Text = "Agente";
            //
            // cboAgente
            //
            this.cboAgente.Location = new System.Drawing.Point(190, 206);
            this.cboAgente.Size = new System.Drawing.Size(365, 28);
            this.cboAgente.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.cboAgente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // lblCliente
            //
            this.lblCliente.Location = new System.Drawing.Point(20, 260);
            this.lblCliente.Size = new System.Drawing.Size(165, 25);
            this.lblCliente.Text = "Cliente";
            //
            // cboCliente
            //
            this.cboCliente.Location = new System.Drawing.Point(190, 256);
            this.cboCliente.Size = new System.Drawing.Size(365, 28);
            this.cboCliente.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.cboCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // lblStato
            //
            this.lblStato.Location = new System.Drawing.Point(20, 310);
            this.lblStato.Size = new System.Drawing.Size(165, 25);
            this.lblStato.Text = "Stato";
            //
            // cboStato
            //
            this.cboStato.Location = new System.Drawing.Point(190, 306);
            this.cboStato.Size = new System.Drawing.Size(365, 28);
            this.cboStato.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // lblImporto
            //
            this.lblImporto.Location = new System.Drawing.Point(20, 360);
            this.lblImporto.Size = new System.Drawing.Size(165, 25);
            this.lblImporto.Text = "Importo";
            //
            // numImporto
            //
            this.numImporto.Location = new System.Drawing.Point(190, 356);
            this.numImporto.Size = new System.Drawing.Size(188, 28);
            this.numImporto.DecimalPlaces = 2;
            this.numImporto.Maximum = 1000000000M;
            this.numImporto.ThousandsSeparator = true;
            //
            // btnSalva
            //
            this.btnSalva.Location = new System.Drawing.Point(335, 420);
            this.btnSalva.Size = new System.Drawing.Size(104, 35);
            this.btnSalva.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSalva.Text = "Salva";
            this.btnSalva.Click += new System.EventHandler(this.btnSalva_Click);
            //
            // btnAnnulla
            //
            this.btnAnnulla.Location = new System.Drawing.Point(451, 420);
            this.btnAnnulla.Size = new System.Drawing.Size(104, 35);
            this.btnAnnulla.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAnnulla.Text = "Annulla";
            this.btnAnnulla.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //
            // EditForm
            //
            this.ClientSize = new System.Drawing.Size(575, 475);
            this.MinimumSize = new System.Drawing.Size(575, 475);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scheda";
            this.AcceptButton = this.btnSalva;
            this.CancelButton = this.btnAnnulla;
            this.Controls.Add(this.lblTitolo);
            this.Controls.Add(this.txtTitolo);
            this.Controls.Add(this.lblCodice);
            this.Controls.Add(this.txtCodice);
            this.Controls.Add(this.lblZona);
            this.Controls.Add(this.txtZona);
            this.Controls.Add(this.chkAttivo);
            this.Controls.Add(this.lblAgente);
            this.Controls.Add(this.cboAgente);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.cboCliente);
            this.Controls.Add(this.lblStato);
            this.Controls.Add(this.cboStato);
            this.Controls.Add(this.lblImporto);
            this.Controls.Add(this.numImporto);
            this.Controls.Add(this.btnSalva);
            this.Controls.Add(this.btnAnnulla);
            ((System.ComponentModel.ISupportInitialize)(this.numImporto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
