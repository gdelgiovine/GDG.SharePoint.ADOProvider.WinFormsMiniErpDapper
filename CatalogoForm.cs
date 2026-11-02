// Scheda di modifica per Prodotti e Righe ordine. Restituisce Model C#, come EditForm.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    internal class CatalogoForm : Form
    {
        private readonly TextBox txtTitolo = new TextBox();
        private readonly TextBox txtCodice = new TextBox();
        private readonly TextBox txtCategoria = new TextBox();
        private readonly TextBox txtTags = new TextBox();
        private readonly NumericUpDown numPrezzo = new NumericUpDown();
        private readonly CheckBox chkAttivo = new CheckBox();
        private readonly ComboBox cboOrdine = new ComboBox();
        private readonly ComboBox cboProdotto = new ComboBox();
        private readonly NumericUpDown numRiga = new NumericUpDown();
        private readonly NumericUpDown numQuantita = new NumericUpDown();
        private readonly NumericUpDown numSconto = new NumericUpDown();
        private readonly Label lblImporto = new Label();
        private readonly bool prodotto;
        private readonly Prodotto prodottoOriginale;
        private readonly RigaOrdine rigaOriginale;
        private int y = 12;

        public Prodotto ProdottoModificato { get; private set; }
        public RigaOrdine RigaModificata { get; private set; }

        public CatalogoForm(Prodotto p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            prodotto = true;
            prodottoOriginale = p;
            Inizializza(p.ID == 0 ? "Nuovo prodotto" : "Modifica prodotto");
            Aggiungi("Titolo", txtTitolo);
            Aggiungi("Codice", txtCodice);
            Aggiungi("Categoria", txtCategoria);
            Prezzo(numPrezzo);
            Aggiungi("Prezzo unitario", numPrezzo);
            Aggiungi("Tag (valori separati da ;)", txtTags);
            chkAttivo.Text = "Attivo";
            Aggiungi("", chkAttivo);
            txtTitolo.Text = p.Title;
            txtCodice.Text = p.Code;
            txtCategoria.Text = p.Category;
            numPrezzo.Value = Math.Min(numPrezzo.Maximum, Math.Max(0, p.UnitPrice));
            txtTags.Text = p.Tags;
            chkAttivo.Checked = p.Active;
            Pulsanti();
        }

        public CatalogoForm(RigaOrdine r, List<Ordine> ordini, List<Prodotto> prodotti)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));
            prodotto = false;
            rigaOriginale = r;
            Inizializza(r.ID == 0 ? "Nuova riga ordine" : "Modifica riga ordine");
            Aggiungi("Titolo", txtTitolo);

            cboOrdine.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOrdine.DisplayMember = nameof(Ordine.Title);
            cboOrdine.ValueMember = nameof(Ordine.ID);
            cboOrdine.DataSource = ordini ?? new List<Ordine>();
            Aggiungi("Ordine", cboOrdine);

            cboProdotto.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProdotto.DisplayMember = nameof(Prodotto.Title);
            cboProdotto.ValueMember = nameof(Prodotto.ID);
            cboProdotto.DataSource = prodotti ?? new List<Prodotto>();
            Aggiungi("Prodotto", cboProdotto);

            numRiga.Minimum = 1; numRiga.Maximum = 9999;
            Aggiungi("Numero riga", numRiga);
            numQuantita.Minimum = 1; numQuantita.Maximum = 100000;
            Aggiungi("Quantita'", numQuantita);
            Prezzo(numPrezzo);
            Aggiungi("Prezzo unitario", numPrezzo);
            numSconto.DecimalPlaces = 2; numSconto.Minimum = 0; numSconto.Maximum = 100;
            Aggiungi("Sconto %", numSconto);
            Aggiungi("Importo riga", lblImporto);

            txtTitolo.Text = r.Title;
            if (r.OrderId > 0) cboOrdine.SelectedValue = r.OrderId;
            if (r.ProductId > 0) cboProdotto.SelectedValue = r.ProductId;
            numRiga.Value = Math.Max(1, Math.Min(9999, r.LineNumber == 0 ? 1 : r.LineNumber));
            numQuantita.Value = Math.Max(1, Math.Min(100000, r.Quantity == 0 ? 1 : r.Quantity));
            numPrezzo.Value = Math.Min(numPrezzo.Maximum, Math.Max(0, r.UnitPrice));
            numSconto.Value = Math.Min(100, Math.Max(0, r.DiscountPercent));

            // Un nuovo prodotto selezionato propone il suo prezzo di listino.
            cboProdotto.SelectionChangeCommitted += (s, e) =>
            {
                Prodotto sel = cboProdotto.SelectedItem as Prodotto;
                if (sel != null) numPrezzo.Value = Math.Min(numPrezzo.Maximum, Math.Max(0, sel.UnitPrice));
            };
            numQuantita.ValueChanged += (s, e) => AggiornaImporto();
            numPrezzo.ValueChanged += (s, e) => AggiornaImporto();
            numSconto.ValueChanged += (s, e) => AggiornaImporto();
            AggiornaImporto();
            Pulsanti();
        }

        private static void Prezzo(NumericUpDown n)
        {
            n.DecimalPlaces = 2;
            n.Minimum = 0;
            n.Maximum = 10000000;
        }

        private void AggiornaImporto()
        {
            decimal importo = decimal.Round(numQuantita.Value * numPrezzo.Value *
                (100m - numSconto.Value) / 100m, 2, MidpointRounding.AwayFromZero);
            lblImporto.Text = importo.ToString("N2");
        }

        private void Inizializza(string titolo)
        {
            Text = titolo;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 400);
        }

        private void Aggiungi(string etichetta, Control c)
        {
            Label l = new Label { Text = etichetta, Left = 12, Top = y + 4, Width = 150, AutoSize = false };
            c.Left = 170;
            c.Top = y;
            c.Width = 230;
            Controls.Add(l);
            Controls.Add(c);
            y += 32;
        }

        private void Pulsanti()
        {
            Button ok = new Button { Text = "Salva", Left = 220, Top = y + 12, Width = 85 };
            Button annulla = new Button { Text = "Annulla", Left = 315, Top = y + 12, Width = 85, DialogResult = DialogResult.Cancel };
            ok.Click += Salva;
            Controls.Add(ok);
            Controls.Add(annulla);
            AcceptButton = ok;
            CancelButton = annulla;
            ClientSize = new Size(420, y + 56);
        }

        private void Salva(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitolo.Text))
            {
                MessageBox.Show(this, "Il titolo e' obbligatorio.");
                return;
            }
            if (prodotto)
            {
                if (string.IsNullOrWhiteSpace(txtCodice.Text))
                {
                    MessageBox.Show(this, "Il codice e' obbligatorio.");
                    return;
                }
                ProdottoModificato = new Prodotto
                {
                    ID = prodottoOriginale.ID,
                    Title = txtTitolo.Text.Trim(),
                    Code = txtCodice.Text.Trim(),
                    Category = txtCategoria.Text.Trim(),
                    UnitPrice = numPrezzo.Value,
                    Active = chkAttivo.Checked,
                    Tags = txtTags.Text.Trim()
                };
            }
            else
            {
                Ordine ordine = cboOrdine.SelectedItem as Ordine;
                Prodotto p = cboProdotto.SelectedItem as Prodotto;
                if (ordine == null || p == null)
                {
                    MessageBox.Show(this, "Selezionare ordine e prodotto.");
                    return;
                }
                RigaModificata = new RigaOrdine
                {
                    ID = rigaOriginale.ID,
                    Title = txtTitolo.Text.Trim(),
                    OrderId = ordine.ID,
                    ProductId = p.ID,
                    LineNumber = (int)numRiga.Value,
                    Quantity = (int)numQuantita.Value,
                    UnitPrice = numPrezzo.Value,
                    DiscountPercent = numSconto.Value
                };
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
