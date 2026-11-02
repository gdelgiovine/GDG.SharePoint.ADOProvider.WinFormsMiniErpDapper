// Autore: Gabriele Del Giovine
// (c) Gabriele Del Giovine 2026

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace GDG.SharePoint.ADOProvider.WinFormsTutorial
{
    /// <summary>
    /// SharePoint browser authentication with optional per-profile WebView2 persistence.
    /// Never stores credentials in Connections.xml; only the current two cookies
    /// leave WebView2, and only through the in-memory provider connection.
    /// </summary>
    internal sealed partial class BrowserLoginForm : Form
    {
        private readonly Uri _site;
        private readonly TutorialConnectionProfile _profile;
        private readonly bool _forceInteractive;
        private bool _capturing;
        public string FedAuth { get; private set; } = string.Empty;
        public string RtFa { get; private set; } = string.Empty;

        public BrowserLoginForm(TutorialConnectionProfile profile, bool forceInteractive = false)
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _forceInteractive = forceInteractive;
            _site = new Uri(profile.SiteUrl.Trim(), UriKind.Absolute);
            if (_site.Scheme != Uri.UriSchemeHttps ||
                !_site.Host.EndsWith(".sharepoint.com", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Browser sign-in requires a SharePoint Online HTTPS URL (*.sharepoint.com).");
            InitializeComponent();
            // Avoid briefly displaying an unnecessary login dialog if the
            // previous remembered SharePoint session still works.
            if (_profile.RememberAuthentication && !_forceInteractive)
                Opacity = 0;
        }

        private async void BrowserLoginForm_Shown(object sender, EventArgs e)
        {
            try
            {
                // Stable isolated profile (remember ON) or InPrivate (remember OFF).
                // WebView2 owns and protects its browser cookies/password store;
                // the app never serializes them into connection profiles.
                var udf = _profile.RememberAuthentication
                    ? BrowserSignInProfileStore.GetDirectory(_profile)
                    : BrowserSignInProfileStore.GetInPrivateDirectory();
                Directory.CreateDirectory(udf);
                var environment = await CoreWebView2Environment.CreateAsync(null, udf);
                var options = environment.CreateCoreWebView2ControllerOptions();
                options.IsInPrivateModeEnabled = !_profile.RememberAuthentication;
                await browser.EnsureCoreWebView2Async(environment, options);
                browser.CoreWebView2.Settings.IsPasswordAutosaveEnabled = _profile.RememberAuthentication;
                browser.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;

                if (_profile.RememberAuthentication && !_forceInteractive &&
                    await TryGetValidatedCookiesAsync().ConfigureAwait(true))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                Opacity = 1;
                browser.CoreWebView2.NavigationCompleted += async (navigationSender, args) =>
                {
                    // If the persisted Microsoft 365 identity silently signs into
                    // SharePoint, finish automatically: no extra "Use session" click.
                    // An explicit "sign in again" still requires user confirmation.
                    if (_forceInteractive || _capturing || !args.IsSuccess || IsDisposed) return;
                    var current = browser.CoreWebView2.Source;
                    if (!Uri.TryCreate(current, UriKind.Absolute, out var at) ||
                        !string.Equals(at.Host, _site.Host, StringComparison.OrdinalIgnoreCase))
                        return;
                    _capturing = true;
                    try
                    {
                        if (await TryGetValidatedCookiesAsync().ConfigureAwait(true) && !IsDisposed)
                        {
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                    }
                    catch (Exception) { /* The manual Use Session action remains available. */ }
                    finally { _capturing = false; }
                };
                browser.CoreWebView2.Navigate(_site.AbsoluteUri);
                useSession.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Could not initialize the Edge WebView2 sign-in window. Check that WebView2 Runtime is installed.\r\n\r\n" + ex.Message,
                    "Browser authentication", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private async Task<bool> TryGetValidatedCookiesAsync()
        {
            var cookies = await browser.CoreWebView2.CookieManager.GetCookiesAsync(_site.AbsoluteUri);
            var fed = cookies.FirstOrDefault(c =>
                string.Equals(c.Name, "FedAuth", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(c.Value));
            var rtfa = cookies.FirstOrDefault(c =>
                string.Equals(c.Name, "rtFa", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(c.Value));
            if (fed == null || rtfa == null)
                return false;
            if (!await VerifySharePointSessionAsync(fed.Value, rtfa.Value).ConfigureAwait(true))
                return false;
            FedAuth = fed.Value;
            RtFa = rtfa.Value;
            return true;
        }

        private async Task<bool> VerifySharePointSessionAsync(string fedAuth, string rtFa)
        {
            try
            {
                // A cookie's presence alone is not evidence that it is unexpired:
                // authenticate a read-only SharePoint REST request before reusing it.
                var jar = new CookieContainer();
                jar.Add(new Cookie("FedAuth", fedAuth, "/", _site.Host) { Secure = true, HttpOnly = true });
                jar.Add(new Cookie("rtFa", rtFa, "/", _site.Host) { Secure = true, HttpOnly = true });
                using (var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = false, UseCookies = true, CookieContainer = jar
                })
                using (var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(18) })
                using (var request = new HttpRequestMessage(HttpMethod.Get,
                    _site.AbsoluteUri.TrimEnd('/') + "/_api/web?$select=Id"))
                {
                    request.Headers.Accept.ParseAdd("application/json;odata=nometadata");
                    using (var response = await client.SendAsync(request).ConfigureAwait(true))
                    {
                        if (!response.IsSuccessStatusCode) return false;
                        var type = response.Content?.Headers.ContentType?.MediaType ?? "";
                        if (type.IndexOf("json", StringComparison.OrdinalIgnoreCase) < 0)
                            return false; // Sign-in HTML or another non-REST response.
                        var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                        return data.IndexOf("\"Id\"", StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
            }
            catch (HttpRequestException) { return false; }
            catch (TaskCanceledException) { return false; }
            catch (WebException) { return false; }
            catch (ArgumentException) { return false; }
        }

        private async void UseSession_Click(object sender, EventArgs e)
        {
            if (_capturing || browser.CoreWebView2 == null) return;
            _capturing = true;
            useSession.Enabled = false;
            try
            {
                if (!await TryGetValidatedCookiesAsync().ConfigureAwait(true))
                {
                    MessageBox.Show(this,
                        "The browser sign-in is not complete, or the SharePoint session has expired. " +
                        "Complete Microsoft 365 sign-in and MFA, wait for the site to load, and retry. " +
                        "Tenant policies may prevent embedded-browser authentication.",
                        "Session not ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Could not read the SharePoint browser session: " + ex.Message,
                    "Browser authentication", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _capturing = false;
                if (!IsDisposed) useSession.Enabled = true;
            }
        }
    }
}
