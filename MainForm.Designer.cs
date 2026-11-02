// Componenti modificabili tramite il Designer di Visual Studio (nessun ciclo nel metodo).
namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblConnessione;
        private System.Windows.Forms.ComboBox cmbListe;
        private System.Windows.Forms.Label lblLista;
        private System.Windows.Forms.Button btnConnessioni;
        private System.Windows.Forms.Button btnConnetti;
        private System.Windows.Forms.Button btnAggiorna;
        private System.Windows.Forms.Button btnNuovo;
        private System.Windows.Forms.Button btnModifica;
        private System.Windows.Forms.Button btnElimina;
        private System.Windows.Forms.Button btnAllegati;
        private System.Windows.Forms.DataGridView grdDati;
        private System.Windows.Forms.BindingSource bsDati;
        private System.Windows.Forms.Label lblStato;
        private System.Windows.Forms.Label lblFiltro;
        private System.Windows.Forms.TextBox txtFiltro;
        private System.Windows.Forms.Button btnPulisciFiltro;
        private System.Windows.Forms.ComboBox cmbColonna;
        private System.Windows.Forms.Button btnFiltra;
        private System.Windows.Forms.ComboBox cmbOperatore;
        private System.Windows.Forms.ComboBox cmbConfronto;
        private System.Windows.Forms.Button btnAggiungiFiltro;
        private System.Windows.Forms.ListBox lstFiltri;
        private System.Windows.Forms.Button btnRimuoviFiltro;
        private System.Windows.Forms.Button btnAggiornaFiltro;
        private System.Windows.Forms.Button btnMostraSql;
        private System.Windows.Forms.Button btnReport;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblConnessione = new System.Windows.Forms.Label();
            this.cmbListe = new System.Windows.Forms.ComboBox();
            this.lblLista = new System.Windows.Forms.Label();
            this.btnConnessioni = new System.Windows.Forms.Button();
            this.btnConnetti = new System.Windows.Forms.Button();
            this.btnAggiorna = new System.Windows.Forms.Button();
            this.btnNuovo = new System.Windows.Forms.Button();
            this.btnModifica = new System.Windows.Forms.Button();
            this.btnElimina = new System.Windows.Forms.Button();
            this.btnAllegati = new System.Windows.Forms.Button();
            this.grdDati = new System.Windows.Forms.DataGridView();
            this.bsDati = new System.Windows.Forms.BindingSource(this.components);
            this.lblStato = new System.Windows.Forms.Label();
            this.lblFiltro = new System.Windows.Forms.Label();
            this.txtFiltro = new System.Windows.Forms.TextBox();
            this.btnPulisciFiltro = new System.Windows.Forms.Button();
            this.cmbColonna = new System.Windows.Forms.ComboBox();
            this.btnFiltra = new System.Windows.Forms.Button();
            this.cmbOperatore = new System.Windows.Forms.ComboBox();
            this.cmbConfronto = new System.Windows.Forms.ComboBox();
            this.btnAggiungiFiltro = new System.Windows.Forms.Button();
            this.lstFiltri = new System.Windows.Forms.ListBox();
            this.btnRimuoviFiltro = new System.Windows.Forms.Button();
            this.btnAggiornaFiltro = new System.Windows.Forms.Button();
            this.btnMostraSql = new System.Windows.Forms.Button();
            this.btnReport = new System.Windows.Forms.Button();
            this.btnReport.Location = new System.Drawing.Point(815, 57);
            this.btnReport.Size = new System.Drawing.Size(105, 33);
            this.btnReport.Text = "Report...";
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            ((System.ComponentModel.ISupportInitialize)(this.grdDati)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDati)).BeginInit();
            this.SuspendLayout();
            //
            // lblConnessione
            //
            this.lblConnessione.Location = new System.Drawing.Point(14, 14);
            this.lblConnessione.Size = new System.Drawing.Size(630, 35);
            this.lblConnessione.Text = "Nessuna connessione";
            this.lblConnessione.AutoEllipsis = true;
            //
            // btnConnessioni
            //
            this.btnConnessioni.Location = new System.Drawing.Point(660, 12);
            this.btnConnessioni.Size = new System.Drawing.Size(136, 33);
            this.btnConnessioni.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnConnessioni.Text = "Connessioni...";
            this.btnConnessioni.Click += new System.EventHandler(this.btnConnessioni_Click);
            //
            // btnConnetti
            //
            this.btnConnetti.Location = new System.Drawing.Point(804, 12);
            this.btnConnetti.Size = new System.Drawing.Size(126, 33);
            this.btnConnetti.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnConnetti.Text = "Collega";
            this.btnConnetti.Click += new System.EventHandler(this.btnConnetti_Click);
            //
            // lblLista
            //
            this.lblLista.Location = new System.Drawing.Point(14, 63);
            this.lblLista.Size = new System.Drawing.Size(52, 25);
            this.lblLista.Text = "Lista:";
            //
            // cmbListe
            //
            this.cmbListe.Location = new System.Drawing.Point(67, 59);
            this.cmbListe.Size = new System.Drawing.Size(178, 28);
            this.cmbListe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbListe.Items.AddRange(new object[] { "Agenti", "Clienti", "Ordini", "Prodotti", "Righe ordine" });
            this.cmbListe.SelectedIndexChanged += new System.EventHandler(this.cmbListe_SelectedIndexChanged);
            //
            // btnAggiorna
            //
            this.btnAggiorna.Location = new System.Drawing.Point(255, 57);
            this.btnAggiorna.Size = new System.Drawing.Size(105, 33);
            this.btnAggiorna.Text = "Aggiorna";
            this.btnAggiorna.Click += new System.EventHandler(this.btnAggiorna_Click);
            //
            // btnNuovo
            //
            this.btnNuovo.Location = new System.Drawing.Point(368, 57);
            this.btnNuovo.Size = new System.Drawing.Size(96, 33);
            this.btnNuovo.Text = "Nuovo";
            this.btnNuovo.Click += new System.EventHandler(this.btnNuovo_Click);
            //
            // btnModifica
            //
            this.btnModifica.Location = new System.Drawing.Point(472, 57);
            this.btnModifica.Size = new System.Drawing.Size(96, 33);
            this.btnModifica.Text = "Modifica";
            this.btnModifica.Click += new System.EventHandler(this.btnModifica_Click);
            //
            // btnElimina
            //
            this.btnElimina.Location = new System.Drawing.Point(576, 57);
            this.btnElimina.Size = new System.Drawing.Size(96, 33);
            this.btnElimina.Text = "Elimina";
            this.btnElimina.Click += new System.EventHandler(this.btnElimina_Click);
            //
            // btnAllegati
            //
            this.btnAllegati.Location = new System.Drawing.Point(680, 57);
            this.btnAllegati.Size = new System.Drawing.Size(121, 33);
            this.btnAllegati.Text = "Allegati...";
            this.btnAllegati.Click += new System.EventHandler(this.btnAllegati_Click);
            //
            // lblFiltro
            //
            this.lblFiltro.Location = new System.Drawing.Point(14, 108);
            this.lblFiltro.Size = new System.Drawing.Size(52, 25);
            this.lblFiltro.Text = "Filtro:";
            //
            // cmbOperatore
            //
            this.cmbOperatore.Location = new System.Drawing.Point(67, 104);
            this.cmbOperatore.Size = new System.Drawing.Size(65, 28);
            this.cmbOperatore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOperatore.Items.AddRange(new object[] { "AND", "OR" });
            this.cmbOperatore.SelectedIndex = 0;
            //
            // txtFiltro
            //
            this.txtFiltro.Location = new System.Drawing.Point(395, 104);
            this.txtFiltro.Size = new System.Drawing.Size(100, 27);
            this.txtFiltro.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFiltro_KeyDown);
            //
            // cmbColonna
            //
            this.cmbColonna.Location = new System.Drawing.Point(140, 104);
            this.cmbColonna.Size = new System.Drawing.Size(118, 28);
            this.cmbColonna.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColonna.SelectedIndexChanged += new System.EventHandler(this.cmbColonna_SelectedIndexChanged);
            //
            // cmbConfronto
            //
            this.cmbConfronto.Location = new System.Drawing.Point(264, 104);
            this.cmbConfronto.Size = new System.Drawing.Size(125, 28);
            this.cmbConfronto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // btnAggiungiFiltro
            //
            this.btnAggiungiFiltro.Location = new System.Drawing.Point(503, 102);
            this.btnAggiungiFiltro.Size = new System.Drawing.Size(96, 30);
            this.btnAggiungiFiltro.Text = "Aggiungi";
            this.btnAggiungiFiltro.Click += new System.EventHandler(this.btnAggiungiFiltro_Click);
            //
            // btnFiltra
            //
            this.btnFiltra.Location = new System.Drawing.Point(607, 102);
            this.btnFiltra.Size = new System.Drawing.Size(96, 30);
            this.btnFiltra.Text = "Filtra";
            this.btnFiltra.Click += new System.EventHandler(this.btnFiltra_Click);
            //
            // btnPulisciFiltro
            //
            this.btnPulisciFiltro.Location = new System.Drawing.Point(711, 102);
            this.btnPulisciFiltro.Size = new System.Drawing.Size(96, 30);
            this.btnPulisciFiltro.Text = "Pulisci";
            this.btnPulisciFiltro.Click += new System.EventHandler(this.btnPulisciFiltro_Click);
            //
            // lstFiltri
            //
            this.lstFiltri.Location = new System.Drawing.Point(67, 138);
            this.lstFiltri.Size = new System.Drawing.Size(628, 52);
            this.lstFiltri.IntegralHeight = false;
            this.lstFiltri.SelectedIndexChanged += new System.EventHandler(this.lstFiltri_SelectedIndexChanged);
            //
            // btnAggiornaFiltro
            //
            this.btnAggiornaFiltro.Location = new System.Drawing.Point(815, 138);
            this.btnAggiornaFiltro.Size = new System.Drawing.Size(105, 30);
            this.btnAggiornaFiltro.Text = "Aggiorna cond.";
            this.btnAggiornaFiltro.Click += new System.EventHandler(this.btnAggiornaFiltro_Click);
            //
            // btnMostraSql
            //
            this.btnMostraSql.Location = new System.Drawing.Point(815, 102);
            this.btnMostraSql.Size = new System.Drawing.Size(105, 30);
            this.btnMostraSql.Text = "Mostra SQL";
            this.btnMostraSql.Click += new System.EventHandler(this.btnMostraSql_Click);
            //
            // btnRimuoviFiltro
            //
            this.btnRimuoviFiltro.Location = new System.Drawing.Point(711, 138);
            this.btnRimuoviFiltro.Size = new System.Drawing.Size(96, 30);
            this.btnRimuoviFiltro.Text = "Rimuovi";
            this.btnRimuoviFiltro.Click += new System.EventHandler(this.btnRimuoviFiltro_Click);
            //
            // grdDati
            //
            this.grdDati.Location = new System.Drawing.Point(14, 200);
            this.grdDati.Size = new System.Drawing.Size(916, 393);
            this.grdDati.Anchor = System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Bottom |
                System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.grdDati.AllowUserToAddRows = false;
            this.grdDati.AllowUserToDeleteRows = false;
            this.grdDati.ReadOnly = true;
            this.grdDati.RowHeadersVisible = false;
            this.grdDati.MultiSelect = false;
            this.grdDati.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDati.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdDati.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDati_CellDoubleClick);
            //
            // lblStato
            //
            this.lblStato.Location = new System.Drawing.Point(14, 603);
            this.lblStato.Size = new System.Drawing.Size(916, 26);
            this.lblStato.Anchor = System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.lblStato.Text = "Pronto";
            //
            // MainForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 637);
            this.MinimumSize = new System.Drawing.Size(835, 450);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Text = "GDG.SharePoint.ADOProvider - MiniERP Dapper";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.lblConnessione);
            this.Controls.Add(this.btnConnessioni);
            this.Controls.Add(this.btnConnetti);
            this.Controls.Add(this.lblLista);
            this.Controls.Add(this.cmbListe);
            this.Controls.Add(this.btnAggiorna);
            this.Controls.Add(this.btnNuovo);
            this.Controls.Add(this.btnModifica);
            this.Controls.Add(this.btnElimina);
            this.Controls.Add(this.btnAllegati);
            this.Controls.Add(this.lblFiltro);
            this.Controls.Add(this.cmbOperatore);
            this.Controls.Add(this.cmbConfronto);
            this.Controls.Add(this.btnAggiungiFiltro);
            this.Controls.Add(this.lstFiltri);
            this.Controls.Add(this.btnRimuoviFiltro);
            this.Controls.Add(this.btnAggiornaFiltro);
            this.Controls.Add(this.btnMostraSql);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.cmbColonna);
            this.Controls.Add(this.txtFiltro);
            this.Controls.Add(this.btnFiltra);
            this.Controls.Add(this.btnPulisciFiltro);
            this.Controls.Add(this.grdDati);
            this.Controls.Add(this.lblStato);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.grdDati)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsDati)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
