using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    // Report di navigazione: gruppi (cliente/agente/prodotto) -> ordini -> righe ordine.
    // Ogni livello esegue una query SQL dinamica (JOIN / GROUP BY) su SharePoint.
    internal class ReportForm : Form
    {
        private readonly DatiSharePoint dati;
        private readonly ComboBox cmbVista = new ComboBox();
        private readonly DataGridView grdGruppi = new DataGridView();
        private readonly DataGridView grdOrdini = new DataGridView();
        private readonly DataGridView grdRighe = new DataGridView();
        private readonly Label lblTotale = new Label();
        private bool caricamento;

        public ReportForm(DatiSharePoint dati)
        {
            this.dati = dati;
            Text = "Report ordini";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new System.Drawing.Size(1000, 640);

            Label l = new Label { Text = "Raggruppa per:", Left = 12, Top = 16, Width = 100 };
            cmbVista.Left = 115; cmbVista.Top = 12; cmbVista.Width = 180;
            cmbVista.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVista.Items.AddRange(new object[] { "Cliente", "Agente", "Prodotto" });
            Button btnSql = new Button { Text = "SQL", Left = 303, Top = 11, Width = 70 };
            btnSql.Click += MostraSql;
            lblTotale.Left = 385; lblTotale.Top = 16; lblTotale.Width = 595; lblTotale.AutoSize = false;

            Configura(grdGruppi, 12, 48, 400, 290);
            Configura(grdOrdini, 424, 48, 564, 290);
            Configura(grdRighe, 12, 350, 976, 280);
            grdGruppi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            grdOrdini.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grdRighe.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grdGruppi.Height = 290; grdOrdini.Height = 290;

            Controls.AddRange(new Control[] { l, cmbVista, btnSql, lblTotale, grdGruppi, grdOrdini, grdRighe });
            cmbVista.SelectedIndexChanged += (s, e) => Esegui(CaricaGruppi);
            grdGruppi.SelectionChanged += (s, e) => { if (!caricamento) Esegui(CaricaOrdini); };
            grdOrdini.SelectionChanged += (s, e) => { if (!caricamento) Esegui(CaricaRighe); };
            Shown += (s, e) => cmbVista.SelectedIndex = 0;
        }

        private static void Configura(DataGridView g, int x, int y, int w, int h)
        {
            g.Left = x; g.Top = y; g.Width = w; g.Height = h;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.RowHeadersVisible = false;
            g.AutoGenerateColumns = true;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Esegui(Action azione)
        {
            Cursor old = Cursor;
            try
            {
                Cursor = Cursors.WaitCursor;
                azione();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = old;
            }
        }

        private void CaricaGruppi()
        {
            List<GruppoReport> gruppi = dati.ReportGruppi(cmbVista.SelectedIndex);
            caricamento = true;
            try
            {
                grdGruppi.DataSource = new BindingList<GruppoReport>(gruppi);
                grdGruppi.Columns["Id"].Visible = false;
                grdGruppi.Columns["Conteggio"].HeaderText = cmbVista.SelectedIndex == 2 ? "Righe" : "Ordini";
                grdGruppi.Columns["Totale"].DefaultCellStyle.Format = "N2";
            }
            finally { caricamento = false; }
            decimal tot = 0; long n = 0;
            foreach (GruppoReport g in gruppi) { tot += g.Totale; n += g.Conteggio; }
            lblTotale.Text = "Gruppi: " + gruppi.Count + "   Elementi: " + n + "   Totale: " + tot.ToString("N2");
            CaricaOrdini();
        }

        private void CaricaOrdini()
        {
            GruppoReport g = grdGruppi.CurrentRow == null ? null : grdGruppi.CurrentRow.DataBoundItem as GruppoReport;
            List<Ordine> ordini = g == null ? new List<Ordine>() : dati.ReportOrdini(cmbVista.SelectedIndex, g.Id);
            caricamento = true;
            try
            {
                grdOrdini.DataSource = new BindingList<Ordine>(ordini);
                foreach (string c in new[] { "ID", "CustomerId", "AgentId" })
                    if (grdOrdini.Columns.Contains(c)) grdOrdini.Columns[c].Visible = false;
                if (grdOrdini.Columns.Contains("Amount")) grdOrdini.Columns["Amount"].DefaultCellStyle.Format = "N2";
            }
            finally { caricamento = false; }
            CaricaRighe();
        }

        private void CaricaRighe()
        {
            Ordine o = grdOrdini.CurrentRow == null ? null : grdOrdini.CurrentRow.DataBoundItem as Ordine;
            List<RigaReport> righe = o == null ? new List<RigaReport>() : dati.ReportRighe(o.ID);
            grdRighe.DataSource = new BindingList<RigaReport>(righe);
            foreach (string c in new[] { "UnitPrice", "LineAmount" })
                if (grdRighe.Columns.Contains(c)) grdRighe.Columns[c].DefaultCellStyle.Format = "N2";
        }

        private void MostraSql(object sender, EventArgs e)
        {
            int v = cmbVista.SelectedIndex < 0 ? 0 : cmbVista.SelectedIndex;
            string[] nomi = { "cliente", "agente", "prodotto" };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("-- 1) Gruppi per " + nomi[v]);
            sb.AppendLine(DatiSharePoint.SqlGruppi(v) + ";").AppendLine();
            sb.AppendLine("-- 2) Ordini del gruppo selezionato (@Id = ID del gruppo)");
            sb.AppendLine(DatiSharePoint.SqlOrdiniGruppo(v) + ";").AppendLine();
            sb.AppendLine("-- 3) Righe dell'ordine selezionato (@Id = ID ordine)");
            sb.AppendLine(DatiSharePoint.SqlRigheOrdine() + ";");
            using (Form f = new Form())
            {
                f.Text = "Query SQL del report";
                f.StartPosition = FormStartPosition.CenterParent;
                f.Size = new System.Drawing.Size(900, 480);
                TextBox tb = new TextBox
                {
                    Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, WordWrap = false,
                    Dock = DockStyle.Fill, Font = new System.Drawing.Font("Consolas", 10F),
                    Text = sb.ToString().Replace("\r\n", "\n").Replace("\n", Environment.NewLine)
                };
                Button copia = new Button { Text = "Copia", Dock = DockStyle.Bottom, Height = 32 };
                copia.Click += (s, a) => Clipboard.SetText(tb.Text);
                f.Controls.Add(tb);
                f.Controls.Add(copia);
                tb.Select(0, 0);
                f.ShowDialog(this);
            }
        }
    }
}
