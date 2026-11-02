// Scheda record: legge e restituisce oggetti Model, senza DataRow.
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GDG.SharePoint.ADOProvider.WinFormsMiniErpDapper
{
    public partial class EditForm : Form
    {
        private int tipo;
        private Agente agenteOriginale;
        private Cliente clienteOriginale;
        private Ordine ordineOriginale;

        public Agente AgenteModificato { get; private set; }
        public Cliente ClienteModificato { get; private set; }
        public Ordine OrdineModificato { get; private set; }

        // Il costruttore senza parametri serve al Designer Visual Studio.
        public EditForm()
        {
            InitializeComponent();
        }

        public EditForm(Agente agente) : this()
        {
            if (agente == null) throw new ArgumentNullException(nameof(agente));
            tipo = 0;
            agenteOriginale = agente;
            ConfiguraScheda();
            txtTitolo.Text = agente.Title;
            txtCodice.Text = agente.AgentCode;
            txtZona.Text = agente.Region;
            chkAttivo.Checked = agente.Active;
            txtCodice.ReadOnly = agente.ID > 0;
            Text = agente.ID == 0 ? "Nuovo agente" : "Modifica agente";
        }

        public EditForm(Cliente cliente, List<Agente> agenti) : this()
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            tipo = 1;
            clienteOriginale = cliente;
            ConfiguraScheda();
            CaricaAgenti(agenti);
            txtTitolo.Text = cliente.Title;
            txtCodice.Text = cliente.Code;
            txtZona.Text = cliente.Country;
            chkAttivo.Checked = cliente.Active;
            SelezionaAgente(cliente.AccountAgentId, null);
            txtCodice.ReadOnly = cliente.ID > 0;
            Text = cliente.ID == 0 ? "Nuovo cliente" : "Modifica cliente";
        }

        public EditForm(Ordine ordine, List<Agente> agenti, List<Cliente> clienti) : this()
        {
            if (ordine == null) throw new ArgumentNullException(nameof(ordine));
            tipo = 2;
            ordineOriginale = ordine;
            ConfiguraScheda();
            CaricaAgenti(agenti);
            CaricaClienti(clienti);
            txtTitolo.Text = ordine.Title;
            SelezionaAgente(ordine.AgentId, ordine.AgentCode);
            SelezionaCliente(ordine.CustomerId, ordine.CustomerCode);
            cboStato.SelectedItem = ordine.Status;
            if (cboStato.SelectedIndex < 0) cboStato.SelectedIndex = 0;
            numImporto.Value = Math.Max(numImporto.Minimum,
                Math.Min(numImporto.Maximum, ordine.Amount));
            Text = ordine.ID == 0 ? "Nuovo ordine" : "Modifica ordine";
        }

        private void ConfiguraScheda()
        {
            cboStato.Items.AddRange(new object[] { "Open", "Closed", "Cancelled" });
            cboStato.SelectedIndex = 0;

            if (tipo == 0)
            {
                lblZona.Text = "Regione";
                lblAgente.Visible = false;
                cboAgente.Visible = false;
                lblCliente.Visible = false;
                cboCliente.Visible = false;
                lblStato.Visible = false;
                cboStato.Visible = false;
                lblImporto.Visible = false;
                numImporto.Visible = false;
            }
            else if (tipo == 1)
            {
                lblCodice.Text = "Codice cliente";
                lblZona.Text = "Nazione";
                txtZona.MaxLength = 10;
                lblAgente.Text = "Agente di riferimento";
                lblCliente.Visible = false;
                cboCliente.Visible = false;
                lblStato.Visible = false;
                cboStato.Visible = false;
                lblImporto.Visible = false;
                numImporto.Visible = false;
            }
            else
            {
                lblCodice.Visible = false;
                txtCodice.Visible = false;
                lblZona.Visible = false;
                txtZona.Visible = false;
                chkAttivo.Visible = false;
                lblAgente.Text = "Agente";
            }
        }

        private void CaricaAgenti(List<Agente> agenti)
        {
            cboAgente.DisplayMember = nameof(Agente.Title);
            cboAgente.Format += (s, e) =>
            {
                Agente a = e.ListItem as Agente;
                if (a != null) e.Value = a.AgentCode + " [ID " + a.ID + "] " + a.Title;
            };
            cboAgente.ValueMember = nameof(Agente.ID);
            cboAgente.DataSource = agenti ?? new List<Agente>();
            cboAgente.SelectedIndex = -1;
        }

        private void CaricaClienti(List<Cliente> clienti)
        {
            cboCliente.DisplayMember = nameof(Cliente.Title);
            cboCliente.Format += (s, e) =>
            {
                Cliente c = e.ListItem as Cliente;
                if (c != null) e.Value = c.Code + " [ID " + c.ID + "] " + c.Title;
            };
            cboCliente.ValueMember = nameof(Cliente.ID);
            cboCliente.DataSource = clienti ?? new List<Cliente>();
            cboCliente.SelectedIndex = -1;
        }

        // Manteniamo il fallback sui codici per gli ordini legacy senza lookup ID.
        private void SelezionaAgente(long? id, string codice)
        {
            foreach (Agente agente in cboAgente.Items)
            {
                if ((id.HasValue && id.Value == agente.ID) ||
                    (!id.HasValue && !string.IsNullOrEmpty(codice) &&
                    string.Equals(codice, agente.AgentCode, StringComparison.OrdinalIgnoreCase)))
                {
                    cboAgente.SelectedItem = agente;
                    return;
                }
            }
            cboAgente.SelectedIndex = -1;
        }

        private void SelezionaCliente(long? id, string codice)
        {
            foreach (Cliente cliente in cboCliente.Items)
            {
                if ((id.HasValue && id.Value == cliente.ID) ||
                    (!id.HasValue && !string.IsNullOrEmpty(codice) &&
                    string.Equals(codice, cliente.Code, StringComparison.OrdinalIgnoreCase)))
                {
                    cboCliente.SelectedItem = cliente;
                    return;
                }
            }
            cboCliente.SelectedIndex = -1;
        }

        private void btnSalva_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitolo.Text))
            {
                MessageBox.Show(this, "Il titolo e' obbligatorio.");
                txtTitolo.Focus();
                return;
            }
            if (tipo != 2 && string.IsNullOrWhiteSpace(txtCodice.Text))
            {
                MessageBox.Show(this, "Il codice e' obbligatorio.");
                txtCodice.Focus();
                return;
            }

            // Creiamo un nuovo Model. Se l'utente annulla non modifichiamo il record
            // gia' selezionato nella griglia.
            if (tipo == 0)
            {
                AgenteModificato = new Agente
                {
                    ID = agenteOriginale.ID,
                    Title = txtTitolo.Text.Trim(),
                    AgentCode = txtCodice.Text.Trim(),
                    Region = txtZona.Text.Trim(),
                    Active = chkAttivo.Checked
                };
            }
            else if (tipo == 1)
            {
                Agente agente = cboAgente.SelectedItem as Agente;
                ClienteModificato = new Cliente
                {
                    ID = clienteOriginale.ID,
                    Title = txtTitolo.Text.Trim(),
                    Code = txtCodice.Text.Trim(),
                    Country = txtZona.Text.Trim(),
                    Active = chkAttivo.Checked,
                    AccountAgentId = agente == null ? (long?)null : agente.ID
                };
            }
            else
            {
                Agente agente = cboAgente.SelectedItem as Agente;
                Cliente cliente = cboCliente.SelectedItem as Cliente;
                OrdineModificato = new Ordine
                {
                    ID = ordineOriginale.ID,
                    Title = txtTitolo.Text.Trim(),
                    AgentId = agente == null ? (long?)null : agente.ID,
                    CustomerId = cliente == null ? (long?)null : cliente.ID,
                    AgentCode = agente == null ? "" : agente.AgentCode,
                    CustomerCode = cliente == null ? "" : cliente.Code,
                    Status = Convert.ToString(cboStato.SelectedItem) ?? "Open",
                    Amount = numImporto.Value
                };
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
