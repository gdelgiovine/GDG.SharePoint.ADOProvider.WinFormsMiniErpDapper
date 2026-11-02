// Autore: Gabriele Del Giovine
// (c) Gabriele Del Giovine 2026
namespace GDG.SharePoint.ADOProvider.WinFormsTutorial
{
    partial class BrowserLoginForm
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label instructions;
        private Microsoft.Web.WebView2.WinForms.WebView2 browser;
        private System.Windows.Forms.FlowLayoutPanel footer;
        private System.Windows.Forms.Button useSession;
        private System.Windows.Forms.Button cancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.instructions = new System.Windows.Forms.Label();
            this.browser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.footer = new System.Windows.Forms.FlowLayoutPanel();
            this.useSession = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.browser)).BeginInit();
            this.SuspendLayout();
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowCount = 3;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.layout.Controls.Add(this.instructions, 0, 0);
            this.layout.Controls.Add(this.browser, 0, 1);
            this.layout.Controls.Add(this.footer, 0, 2);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instructions.Text = "Sign in to SharePoint using the browser. MFA is supported when permitted by your tenant.\r\nAfter login, click 'Use signed-in session'. No Client ID or Tenant ID is needed.";
            this.instructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.instructions.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.AllowExternalDrop = false;
            this.browser.CreationProperties = null;
            this.footer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.footer.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.footer.WrapContents = false;
            this.footer.Controls.Add(this.cancel);
            this.footer.Controls.Add(this.useSession);
            this.cancel.Text = "Cancel";
            this.cancel.Size = new System.Drawing.Size(115, 31);
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.useSession.Text = "Use signed-in session";
            this.useSession.Size = new System.Drawing.Size(195, 31);
            this.useSession.Enabled = false;
            this.useSession.Click += new System.EventHandler(this.UseSession_Click);
            this.ClientSize = new System.Drawing.Size(1050, 740);
            this.MinimumSize = new System.Drawing.Size(750, 550);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SharePoint Online — Browser sign-in";
            this.Controls.Add(this.layout);
            this.CancelButton = this.cancel;
            this.Shown += new System.EventHandler(this.BrowserLoginForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.browser)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
