// Autore: Gabriele Del Giovine
// (c) Gabriele Del Giovine 2026

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GDG.SharePoint.ADOProvider.WinFormsTutorial
{
    internal sealed partial class ConnectionManagerForm : Form
    {
        private readonly TutorialConnectionStore _store;
        private readonly InteractiveBrowserAuthenticator _authenticator;
        private readonly List<TutorialConnectionProfile> _workingProfiles;
        private string _rawConnectionString;
        private bool _loading;

        public TutorialConnectionProfile SelectedProfile { get; private set; }
        public bool UseRawConnectionString { get; private set; }
        public string RawConnectionString { get; private set; } = string.Empty;

        public ConnectionManagerForm(
            TutorialConnectionStore store,
            TutorialConnectionProfile activeProfile,
            string rawConnectionString,
            InteractiveBrowserAuthenticator authenticator)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
            _workingProfiles = store.Profiles.Select(p => p.Clone()).ToList();
            _rawConnectionString = rawConnectionString ?? string.Empty;

            InitializeComponent();
            _onPremAuthentication.Items.Add("DefaultNetworkCredentials");
            _onPremAuthentication.Items.Add("NetworkCredential");
            RefreshProfileList(activeProfile == null ? store.SelectedProfileName : activeProfile.Name);
        }

        private void Profiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            LoadSelectedProfile();
        }

        private void NewOnline_Click(object sender, EventArgs e)
        {
            AddProfile(TutorialConnectionTarget.Online, "SharePoint Online");
        }

        private void NewOnPrem_Click(object sender, EventArgs e)
        {
            AddProfile(TutorialConnectionTarget.OnPremSE, "SharePoint On-Premises");
        }

        private void DeleteProfile_Click(object sender, EventArgs e)
        {
            var profile = _profiles.SelectedItem as TutorialConnectionProfile;
            if (profile == null) return;

            if (MessageBox.Show(this,
                    "Delete connection profile '" + profile.Name + "'?",
                    "Delete connection profile",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            string selectedName = string.Equals(
                _store.SelectedProfileName,
                profile.Name,
                StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : _store.SelectedProfileName;

            if (profile.Target == TutorialConnectionTarget.Online)
            {
                try { _authenticator.ForgetBrowserSession(profile); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Cannot clear the saved browser session: " + ex.Message,
                        "Remove connection profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            _workingProfiles.Remove(profile);
            PersistWorkingProfiles(selectedName);
            RefreshProfileList(selectedName);
        }

        private void SaveProfile_Click(object sender, EventArgs e)
        {
            var profile = _profiles.SelectedItem as TutorialConnectionProfile;
            if (profile == null) return;

            try
            {
                var previous = profile.Clone();
                ApplyEditor(profile, requirePassword: false);
                EnsureUniqueName(profile);
                ForgetOldSessionIfNeeded(previous, profile);

                // Saving metadata must not implicitly activate a different profile.
                PersistWorkingProfiles(_store.SelectedProfileName);
                RefreshProfileList(profile.Name);
                MessageBox.Show(this,
                    "Profile saved. No passwords or access tokens are stored in Connections.xml.\r\n\r\nUse 'Use selected' to make this profile the active connection.",
                    "Connection profile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Connection profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UseSelected_Click(object sender, EventArgs e)
        {
            var profile = _profiles.SelectedItem as TutorialConnectionProfile;
            if (profile == null)
            {
                MessageBox.Show(this, "Select a connection profile first.", "Connection profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var previous = profile.Clone();
                ApplyEditor(profile, requirePassword: true);
                EnsureUniqueName(profile);
                ForgetOldSessionIfNeeded(previous, profile);
                PersistWorkingProfiles(profile.Name);
                SelectedProfile = profile.Clone();
                UseRawConnectionString = false;
                RawConnectionString = string.Empty;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Connection profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ForgetSavedSignIn_Click(object sender, EventArgs e)
        {
            var profile = _profiles.SelectedItem as TutorialConnectionProfile;
            if (profile == null || profile.Target != TutorialConnectionTarget.Online ||
                !string.IsNullOrWhiteSpace(profile.ClientId)) return;
            if (MessageBox.Show(this,
                "Forget the saved SharePoint sign-in for this connection profile?\r\n" +
                "The browser session, cached cookies and optional saved credentials for this profile will be deleted.",
                "Forget saved sign-in", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            try
            {
                _authenticator.ForgetBrowserSession(profile);
                MessageBox.Show(this, "Saved browser authentication cleared for this profile.",
                    "Forget saved sign-in", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Could not remove this profile's browser data. Close other Tutorial instances and retry.\r\n\r\n" + ex.Message,
                    "Forget saved sign-in", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ForgetOldSessionIfNeeded(TutorialConnectionProfile previous, TutorialConnectionProfile current)
        {
            if (previous.Target != TutorialConnectionTarget.Online)
                return;
            // Disabling persistence or switching the associated SharePoint host must
            // not leave a reusable browser profile behind.
            if (!Uri.TryCreate(previous.SiteUrl, UriKind.Absolute, out var oldUri) ||
                !Uri.TryCreate(current.SiteUrl, UriKind.Absolute, out var newUri))
                return; // New profile without a previous site cannot have saved browser data.
            if ((previous.RememberAuthentication && !current.RememberAuthentication) ||
                !string.Equals(oldUri.Host, newUri.Host, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(current.ClientId) && string.IsNullOrWhiteSpace(previous.ClientId)))
                _authenticator.ForgetBrowserSession(previous);
        }

        private void UseRaw_Click(object sender, EventArgs e)
        {
            string editedValue;
            if (!TryEditRawConnectionString(out editedValue))
                return;

            _rawConnectionString = editedValue;
            _store.SelectedProfileName = string.Empty;
            _store.Save();
            SelectedProfile = null;
            UseRawConnectionString = true;
            RawConnectionString = _rawConnectionString;
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool TryEditRawConnectionString(out string value)
        {
            using (var dialog = new Form())
            using (var layout = new TableLayoutPanel())
            using (var note = new Label())
            using (var editor = new TextBox())
            using (var buttons = new FlowLayoutPanel())
            using (var useButton = new Button())
            using (var cancelButton = new Button())
            {
                dialog.Text = "Raw / Environment connection - GDG.SharePoint.ADOProvider Tutorial";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ShowInTaskbar = false;
                dialog.ClientSize = new System.Drawing.Size(820, 360);
                dialog.MinimumSize = new System.Drawing.Size(700, 300);

                layout.Dock = DockStyle.Fill;
                layout.Padding = new Padding(12);
                layout.ColumnCount = 1;
                layout.RowCount = 3;
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));

                note.Dock = DockStyle.Fill;
                note.Text =
                    "Raw connection string. This value is session-only and is never shown on the MainForm. " +
                    "At startup the tutorial can initialize it from GDG_SP_CONNECTION.";

                editor.Dock = DockStyle.Fill;
                editor.Multiline = true;
                editor.ScrollBars = ScrollBars.Both;
                editor.WordWrap = false;
                editor.Font = new System.Drawing.Font("Consolas", 9F);
                editor.Text = _rawConnectionString ?? string.Empty;

                buttons.Dock = DockStyle.Fill;
                buttons.FlowDirection = FlowDirection.RightToLeft;
                buttons.WrapContents = false;

                useButton.Text = "Use connection";
                useButton.Size = new System.Drawing.Size(140, 32);
                useButton.DialogResult = DialogResult.OK;

                cancelButton.Text = "Cancel";
                cancelButton.Size = new System.Drawing.Size(100, 32);
                cancelButton.DialogResult = DialogResult.Cancel;

                buttons.Controls.Add(cancelButton);
                buttons.Controls.Add(useButton);
                layout.Controls.Add(note, 0, 0);
                layout.Controls.Add(editor, 0, 1);
                layout.Controls.Add(buttons, 0, 2);
                dialog.Controls.Add(layout);
                dialog.AcceptButton = useButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    value = string.Empty;
                    return false;
                }

                value = editor.Text.Trim();
                if (value.Length == 0)
                {
                    MessageBox.Show(
                        this,
                        "Enter a GDG.SharePoint.ADOProvider connection string, or cancel and select a saved profile.",
                        "Raw connection string required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
        }

        private void AddProfile(TutorialConnectionTarget target, string baseName)
        {
            var profile = new TutorialConnectionProfile
            {
                Name = CreateUniqueName(baseName),
                Target = target,
                OnPremAuthentication = "DefaultNetworkCredentials"
            };
            _workingProfiles.Add(profile);
            RefreshProfileList(profile.Name);
        }

        private string CreateUniqueName(string baseName)
        {
            string name = baseName;
            int suffix = 2;
            while (_workingProfiles.Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
                name = baseName + " " + suffix++;
            return name;
        }

        private void RefreshProfileList(string selectName)
        {
            _loading = true;
            try
            {
                _profiles.Items.Clear();
                foreach (var profile in _workingProfiles.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
                    _profiles.Items.Add(profile);

                int selectedIndex = -1;
                if (!string.IsNullOrWhiteSpace(selectName))
                {
                    for (int i = 0; i < _profiles.Items.Count; i++)
                    {
                        var candidate = (TutorialConnectionProfile)_profiles.Items[i];
                        if (string.Equals(candidate.Name, selectName, StringComparison.OrdinalIgnoreCase))
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                }

                if (selectedIndex < 0 && _profiles.Items.Count > 0)
                    selectedIndex = 0;
                _profiles.SelectedIndex = selectedIndex;
            }
            finally
            {
                _loading = false;
            }

            LoadSelectedProfile();
        }

        private void LoadSelectedProfile()
        {
            var profile = _profiles.SelectedItem as TutorialConnectionProfile;
            bool hasProfile = profile != null;

            // Keep the overall panel enabled so New/Use raw/Close always remain usable,
            // even on the first run when no profiles exist yet.
            _profileName.Enabled = hasProfile;
            _siteUrl.Enabled = hasProfile;
            _deleteProfile.Enabled = hasProfile;
            _saveProfile.Enabled = hasProfile;
            _useSelected.Enabled = hasProfile;
            _forgetSavedSignIn.Enabled = hasProfile &&
                ((TutorialConnectionProfile)_profiles.SelectedItem).Target == TutorialConnectionTarget.Online &&
                string.IsNullOrWhiteSpace(((TutorialConnectionProfile)_profiles.SelectedItem).ClientId);

            if (!hasProfile)
            {
                ClearEditor();
                _onlineGroup.Enabled = false;
                _onPremGroup.Enabled = false;
                return;
            }

            _loading = true;
            try
            {
                _profileName.Text = profile.Name;
                _targetValue.Text = profile.TargetDisplayName;
                _siteUrl.Text = profile.SiteUrl;
                _tenantId.Text = profile.TenantId;
                _clientId.Text = profile.ClientId;
                _rememberAuthentication.Checked = profile.RememberAuthentication;
                _onPremAuthentication.SelectedItem = profile.OnPremAuthentication;
                if (_onPremAuthentication.SelectedIndex < 0)
                    _onPremAuthentication.SelectedIndex = 0;
                _domain.Text = profile.Domain;
                _userName.Text = profile.UserName;
                _password.Text = profile.Password;
                UpdateEditorMode(profile.Target);
            }
            finally
            {
                _loading = false;
            }
        }

        private void OnPremAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            UpdateNetworkCredentialState();
        }

        private void UpdateEditorMode(TutorialConnectionTarget target)
        {
            bool online = target == TutorialConnectionTarget.Online;
            _onlineGroup.Enabled = online;
            _rememberAuthentication.Enabled = online;
            _onPremGroup.Enabled = !online;
            UpdateNetworkCredentialState();
        }

        private void UpdateNetworkCredentialState()
        {
            bool explicitCredential = _onPremGroup.Enabled &&
                string.Equals(Convert.ToString(_onPremAuthentication.SelectedItem), "NetworkCredential", StringComparison.OrdinalIgnoreCase);
            _domain.Enabled = explicitCredential;
            _userName.Enabled = explicitCredential;
            _password.Enabled = explicitCredential;
        }

        private void ApplyEditor(TutorialConnectionProfile profile, bool requirePassword)
        {
            profile.Name = _profileName.Text.Trim();
            profile.SiteUrl = _siteUrl.Text.Trim();
            profile.TenantId = _tenantId.Text.Trim();
            profile.ClientId = _clientId.Text.Trim();
            profile.RememberAuthentication = _rememberAuthentication.Checked;
            profile.OnPremAuthentication = Convert.ToString(_onPremAuthentication.SelectedItem) ?? "DefaultNetworkCredentials";
            profile.Domain = _domain.Text.Trim();
            profile.UserName = _userName.Text.Trim();
            profile.Password = _password.Text;
            profile.Validate(requirePassword);
        }

        private void EnsureUniqueName(TutorialConnectionProfile profile)
        {
            if (_workingProfiles.Any(p => !ReferenceEquals(p, profile) &&
                                          string.Equals(p.Name, profile.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Another connection profile already uses the name '" + profile.Name + "'.");
        }

        private void PersistWorkingProfiles(string selectedName)
        {
            _store.Profiles.Clear();
            foreach (var profile in _workingProfiles)
                _store.Profiles.Add(profile.Clone());
            _store.SelectedProfileName = selectedName ?? string.Empty;
            _store.Save();
        }

        private void ClearEditor()
        {
            _profileName.Clear();
            _targetValue.Text = string.Empty;
            _siteUrl.Clear();
            _tenantId.Clear();
            _clientId.Clear();
            _rememberAuthentication.Checked = true;
            _domain.Clear();
            _userName.Clear();
            _password.Clear();
        }
    }
}
