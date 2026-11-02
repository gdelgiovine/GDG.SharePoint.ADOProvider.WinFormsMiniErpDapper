// Autore: Gabriele Del Giovine
// (c) Gabriele Del Giovine 2026

namespace GDG.SharePoint.ADOProvider.WinFormsTutorial
{
    partial class ConnectionManagerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.leftLayout = new System.Windows.Forms.TableLayoutPanel();
            this.profilesLabel = new System.Windows.Forms.Label();
            this._profiles = new System.Windows.Forms.ListBox();
            this.newButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._newOnline = new System.Windows.Forms.Button();
            this._newOnPrem = new System.Windows.Forms.Button();
            this._deleteProfile = new System.Windows.Forms.Button();
            this._editorPanel = new System.Windows.Forms.TableLayoutPanel();
            this.profileNameLabel = new System.Windows.Forms.Label();
            this._profileName = new System.Windows.Forms.TextBox();
            this.targetLabel = new System.Windows.Forms.Label();
            this._targetValue = new System.Windows.Forms.Label();
            this.siteUrlLabel = new System.Windows.Forms.Label();
            this._siteUrl = new System.Windows.Forms.TextBox();
            this._onlineGroup = new System.Windows.Forms.GroupBox();
            this.onlineLayout = new System.Windows.Forms.TableLayoutPanel();
            this.onlineAuthLabel = new System.Windows.Forms.Label();
            this._onlineAuth = new System.Windows.Forms.TextBox();
            this.tenantIdLabel = new System.Windows.Forms.Label();
            this._tenantId = new System.Windows.Forms.TextBox();
            this.clientIdLabel = new System.Windows.Forms.Label();
            this._clientId = new System.Windows.Forms.TextBox();
            this.onlineNote = new System.Windows.Forms.Label();
            this._rememberAuthentication = new System.Windows.Forms.CheckBox();
            this._onPremGroup = new System.Windows.Forms.GroupBox();
            this.onPremLayout = new System.Windows.Forms.TableLayoutPanel();
            this.onPremAuthLabel = new System.Windows.Forms.Label();
            this._onPremAuthentication = new System.Windows.Forms.ComboBox();
            this.domainLabel = new System.Windows.Forms.Label();
            this._domain = new System.Windows.Forms.TextBox();
            this.userNameLabel = new System.Windows.Forms.Label();
            this._userName = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this._password = new System.Windows.Forms.TextBox();
            this.passwordNote = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._saveProfile = new System.Windows.Forms.Button();
            this._forgetSavedSignIn = new System.Windows.Forms.Button();
            this._useSelected = new System.Windows.Forms.Button();
            this._useRaw = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            this.leftLayout.SuspendLayout();
            this.newButtonsPanel.SuspendLayout();
            this._editorPanel.SuspendLayout();
            this._onlineGroup.SuspendLayout();
            this.onlineLayout.SuspendLayout();
            this._onPremGroup.SuspendLayout();
            this.onPremLayout.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplit.Location = new System.Drawing.Point(0, 0);
            this.mainSplit.Name = "mainSplit";
            this.mainSplit.Panel1.Controls.Add(this.leftLayout);
            this.mainSplit.Panel2.Controls.Add(this._editorPanel);
            this.mainSplit.Size = new System.Drawing.Size(980, 650);
            this.mainSplit.SplitterDistance = 300;
            // 
            // leftLayout
            // 
            this.leftLayout.ColumnCount = 1;
            this.leftLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftLayout.Controls.Add(this.profilesLabel, 0, 0);
            this.leftLayout.Controls.Add(this._profiles, 0, 1);
            this.leftLayout.Controls.Add(this.newButtonsPanel, 0, 2);
            this.leftLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftLayout.Padding = new System.Windows.Forms.Padding(10);
            this.leftLayout.RowCount = 3;
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            // 
            // profilesLabel
            // 
            this.profilesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profilesLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.profilesLabel.Text = "Saved connection profiles";
            // 
            // _profiles
            // 
            this._profiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this._profiles.IntegralHeight = false;
            this._profiles.SelectedIndexChanged += new System.EventHandler(this.Profiles_SelectedIndexChanged);
            // 
            // newButtonsPanel
            // 
            this.newButtonsPanel.Controls.Add(this._newOnline);
            this.newButtonsPanel.Controls.Add(this._newOnPrem);
            this.newButtonsPanel.Controls.Add(this._deleteProfile);
            this.newButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.newButtonsPanel.WrapContents = false;
            // 
            // _newOnline
            // 
            this._newOnline.Size = new System.Drawing.Size(260, 30);
            this._newOnline.Text = "New SharePoint Online profile";
            this._newOnline.UseVisualStyleBackColor = true;
            this._newOnline.Click += new System.EventHandler(this.NewOnline_Click);
            // 
            // _newOnPrem
            // 
            this._newOnPrem.Size = new System.Drawing.Size(260, 30);
            this._newOnPrem.Text = "New On-Premises profile";
            this._newOnPrem.UseVisualStyleBackColor = true;
            this._newOnPrem.Click += new System.EventHandler(this.NewOnPrem_Click);
            // 
            // _deleteProfile
            // 
            this._deleteProfile.Size = new System.Drawing.Size(260, 30);
            this._deleteProfile.Text = "Delete selected profile";
            this._deleteProfile.UseVisualStyleBackColor = true;
            this._deleteProfile.Click += new System.EventHandler(this.DeleteProfile_Click);
            // 
            // _editorPanel
            // 
            this._editorPanel.ColumnCount = 2;
            this._editorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this._editorPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorPanel.Controls.Add(this.profileNameLabel, 0, 0);
            this._editorPanel.Controls.Add(this._profileName, 1, 0);
            this._editorPanel.Controls.Add(this.targetLabel, 0, 1);
            this._editorPanel.Controls.Add(this._targetValue, 1, 1);
            this._editorPanel.Controls.Add(this.siteUrlLabel, 0, 2);
            this._editorPanel.Controls.Add(this._siteUrl, 1, 2);
            this._editorPanel.Controls.Add(this._onlineGroup, 0, 3);
            this._editorPanel.Controls.Add(this._onPremGroup, 0, 4);
            this._editorPanel.Controls.Add(this.bottomPanel, 0, 5);
            this._editorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editorPanel.Padding = new System.Windows.Forms.Padding(12);
            this._editorPanel.RowCount = 6;
            this._editorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this._editorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this._editorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this._editorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this._editorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this._editorPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorPanel.SetColumnSpan(this._onlineGroup, 2);
            this._editorPanel.SetColumnSpan(this._onPremGroup, 2);
            this._editorPanel.SetColumnSpan(this.bottomPanel, 2);
            // 
            // profileNameLabel
            // 
            this.profileNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileNameLabel.Text = "Profile name";
            // 
            // _profileName
            // 
            this._profileName.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // targetLabel
            // 
            this.targetLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.targetLabel.Text = "Target";
            // 
            // _targetValue
            // 
            this._targetValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._targetValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            // 
            // siteUrlLabel
            // 
            this.siteUrlLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.siteUrlLabel.Text = "Site URL";
            // 
            // _siteUrl
            // 
            this._siteUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // _onlineGroup
            // 
            this._onlineGroup.Controls.Add(this.onlineLayout);
            this._onlineGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._onlineGroup.Text = "SharePoint Online - web browser authentication";
            // 
            // onlineLayout
            // 
            this.onlineLayout.ColumnCount = 2;
            this.onlineLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.onlineLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.onlineLayout.Controls.Add(this.onlineAuthLabel, 0, 0);
            this.onlineLayout.Controls.Add(this._onlineAuth, 1, 0);
            this.onlineLayout.Controls.Add(this.tenantIdLabel, 0, 1);
            this.onlineLayout.Controls.Add(this._tenantId, 1, 1);
            this.onlineLayout.Controls.Add(this.clientIdLabel, 0, 2);
            this.onlineLayout.Controls.Add(this._clientId, 1, 2);
            this.onlineLayout.Controls.Add(this._rememberAuthentication, 0, 3);
            this.onlineLayout.Controls.Add(this.onlineNote, 0, 4);
            this.onlineLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onlineLayout.Padding = new System.Windows.Forms.Padding(8);
            this.onlineLayout.RowCount = 5;
            this.onlineLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.onlineLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.onlineLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.onlineLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.onlineLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.onlineLayout.SetColumnSpan(this._rememberAuthentication, 2);
            this.onlineLayout.SetColumnSpan(this.onlineNote, 2);
            // 
            // onlineAuthLabel
            // 
            this.onlineAuthLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.onlineAuthLabel.Text = "Authentication";
            // 
            // _onlineAuth
            // 
            this._onlineAuth.Dock = System.Windows.Forms.DockStyle.Fill;
            this._onlineAuth.ReadOnly = true;
            this._onlineAuth.Text = "WebView2 login (no Entra app); MSAL optional";
            // 
            // tenantIdLabel
            // 
            this.tenantIdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tenantIdLabel.Text = "Tenant ID (legacy)";
            // 
            // _tenantId
            // 
            this._tenantId.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // clientIdLabel
            // 
            this.clientIdLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.clientIdLabel.Text = "Client ID (legacy)";
            // 
            // _clientId
            // 
            this._clientId.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // _rememberAuthentication
            //
            this._rememberAuthentication.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rememberAuthentication.Name = "_rememberAuthentication";
            this._rememberAuthentication.Text = "Remember Microsoft 365 sign-in on this computer";
            this._rememberAuthentication.Checked = true;
            //
            // onlineNote
            // 
            this.onlineNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onlineNote.Text = "Leave Client ID/Tenant ID empty for browser login. Remember sign-in keeps an isolated local Edge WebView2 profile (cookies/optional browser password store). Switch it off to use InPrivate. No passwords or cookies are written to Connections.xml.";
            // 
            // _onPremGroup
            // 
            this._onPremGroup.Controls.Add(this.onPremLayout);
            this._onPremGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._onPremGroup.Text = "SharePoint On-Premises";
            // 
            // onPremLayout
            // 
            this.onPremLayout.ColumnCount = 2;
            this.onPremLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.onPremLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.onPremLayout.Controls.Add(this.onPremAuthLabel, 0, 0);
            this.onPremLayout.Controls.Add(this._onPremAuthentication, 1, 0);
            this.onPremLayout.Controls.Add(this.domainLabel, 0, 1);
            this.onPremLayout.Controls.Add(this._domain, 1, 1);
            this.onPremLayout.Controls.Add(this.userNameLabel, 0, 2);
            this.onPremLayout.Controls.Add(this._userName, 1, 2);
            this.onPremLayout.Controls.Add(this.passwordLabel, 0, 3);
            this.onPremLayout.Controls.Add(this._password, 1, 3);
            this.onPremLayout.Controls.Add(this.passwordNote, 0, 4);
            this.onPremLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onPremLayout.Padding = new System.Windows.Forms.Padding(8);
            this.onPremLayout.RowCount = 5;
            this.onPremLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.onPremLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.onPremLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.onPremLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.onPremLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.onPremLayout.SetColumnSpan(this.passwordNote, 2);
            // 
            // onPremAuthLabel
            // 
            this.onPremAuthLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.onPremAuthLabel.Text = "Authentication";
            // 
            // _onPremAuthentication
            // 
            this._onPremAuthentication.Dock = System.Windows.Forms.DockStyle.Fill;
            this._onPremAuthentication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._onPremAuthentication.SelectedIndexChanged += new System.EventHandler(this.OnPremAuthentication_SelectedIndexChanged);
            // 
            // domainLabel
            // 
            this.domainLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.domainLabel.Text = "Domain";
            // 
            // _domain
            // 
            this._domain.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // userNameLabel
            // 
            this.userNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.userNameLabel.Text = "User name";
            // 
            // _userName
            // 
            this._userName.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // passwordLabel
            // 
            this.passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.passwordLabel.Text = "Password";
            // 
            // _password
            // 
            this._password.Dock = System.Windows.Forms.DockStyle.Fill;
            this._password.UseSystemPasswordChar = true;
            // 
            // passwordNote
            // 
            this.passwordNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passwordNote.Text = "DefaultNetworkCredentials uses the current Windows identity. NetworkCredential uses Domain/UserName/Password. Passwords are session-only and are not saved in Connections.xml.";
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this._saveProfile);
            this.bottomPanel.Controls.Add(this._forgetSavedSignIn);
            this.bottomPanel.Controls.Add(this._useSelected);
            this.bottomPanel.Controls.Add(this._useRaw);
            this.bottomPanel.Controls.Add(this._cancel);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.bottomPanel.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.bottomPanel.WrapContents = true;
            // 
            // _saveProfile
            // 
            this._saveProfile.Size = new System.Drawing.Size(125, 34);
            this._saveProfile.Text = "Save profile";
            this._saveProfile.UseVisualStyleBackColor = true;
            this._saveProfile.Click += new System.EventHandler(this.SaveProfile_Click);
            // 
            // _forgetSavedSignIn
            //
            this._forgetSavedSignIn.Size = new System.Drawing.Size(166, 34);
            this._forgetSavedSignIn.Text = "Forget saved sign-in";
            this._forgetSavedSignIn.UseVisualStyleBackColor = true;
            this._forgetSavedSignIn.Click += new System.EventHandler(this.ForgetSavedSignIn_Click);
            //
            // _useSelected
            // 
            this._useSelected.Size = new System.Drawing.Size(150, 34);
            this._useSelected.Text = "Use selected";
            this._useSelected.UseVisualStyleBackColor = true;
            this._useSelected.Click += new System.EventHandler(this.UseSelected_Click);
            // 
            // _useRaw
            // 
            this._useRaw.Size = new System.Drawing.Size(180, 34);
            this._useRaw.Text = "Use raw / environment";
            this._useRaw.UseVisualStyleBackColor = true;
            this._useRaw.Click += new System.EventHandler(this.UseRaw_Click);
            // 
            // _cancel
            // 
            this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancel.Size = new System.Drawing.Size(100, 34);
            this._cancel.Text = "Close";
            this._cancel.UseVisualStyleBackColor = true;
            // 
            // ConnectionManagerForm
            // 
            this.AcceptButton = this._useSelected;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancel;
            this.ClientSize = new System.Drawing.Size(980, 650);
            this.Controls.Add(this.mainSplit);
            this.MinimumSize = new System.Drawing.Size(900, 620);
            this.Name = "ConnectionManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection Manager - GDG.SharePoint.ADOProvider Tutorial";
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.leftLayout.ResumeLayout(false);
            this.newButtonsPanel.ResumeLayout(false);
            this._editorPanel.ResumeLayout(false);
            this._editorPanel.PerformLayout();
            this._onlineGroup.ResumeLayout(false);
            this.onlineLayout.ResumeLayout(false);
            this.onlineLayout.PerformLayout();
            this._onPremGroup.ResumeLayout(false);
            this.onPremLayout.ResumeLayout(false);
            this.onPremLayout.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.TableLayoutPanel leftLayout;
        private System.Windows.Forms.Label profilesLabel;
        private System.Windows.Forms.ListBox _profiles;
        private System.Windows.Forms.FlowLayoutPanel newButtonsPanel;
        private System.Windows.Forms.Button _newOnline;
        private System.Windows.Forms.Button _newOnPrem;
        private System.Windows.Forms.Button _deleteProfile;
        private System.Windows.Forms.TableLayoutPanel _editorPanel;
        private System.Windows.Forms.Label profileNameLabel;
        private System.Windows.Forms.TextBox _profileName;
        private System.Windows.Forms.Label targetLabel;
        private System.Windows.Forms.Label _targetValue;
        private System.Windows.Forms.Label siteUrlLabel;
        private System.Windows.Forms.TextBox _siteUrl;
        private System.Windows.Forms.GroupBox _onlineGroup;
        private System.Windows.Forms.TableLayoutPanel onlineLayout;
        private System.Windows.Forms.Label onlineAuthLabel;
        private System.Windows.Forms.TextBox _onlineAuth;
        private System.Windows.Forms.Label tenantIdLabel;
        private System.Windows.Forms.TextBox _tenantId;
        private System.Windows.Forms.Label clientIdLabel;
        private System.Windows.Forms.TextBox _clientId;
        private System.Windows.Forms.Label onlineNote;
        private System.Windows.Forms.CheckBox _rememberAuthentication;
        private System.Windows.Forms.GroupBox _onPremGroup;
        private System.Windows.Forms.TableLayoutPanel onPremLayout;
        private System.Windows.Forms.Label onPremAuthLabel;
        private System.Windows.Forms.ComboBox _onPremAuthentication;
        private System.Windows.Forms.Label domainLabel;
        private System.Windows.Forms.TextBox _domain;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.TextBox _userName;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox _password;
        private System.Windows.Forms.Label passwordNote;
        private System.Windows.Forms.FlowLayoutPanel bottomPanel;
        private System.Windows.Forms.Button _saveProfile;
        private System.Windows.Forms.Button _forgetSavedSignIn;
        private System.Windows.Forms.Button _useSelected;
        private System.Windows.Forms.Button _useRaw;
        private System.Windows.Forms.Button _cancel;
    }
}
