// Autore: Gabriele Del Giovine
// (c) Gabriele Del Giovine 2026

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using GDG.SharePoint.ADOProvider;
using Microsoft.Identity.Client;

namespace GDG.SharePoint.ADOProvider.WinFormsTutorial
{
    internal enum TutorialConnectionTarget
    {
        Online,
        OnPremSE
    }

    internal sealed class TutorialConnectionProfile
    {
        public string Name { get; set; } = string.Empty;
        public TutorialConnectionTarget Target { get; set; } = TutorialConnectionTarget.Online;
        public string SiteUrl { get; set; } = string.Empty;
        // Stable per-profile identity: do not key browser persistence by mutable display names.
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool RememberAuthentication { get; set; } = true;

        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;

        public string OnPremAuthentication { get; set; } = "DefaultNetworkCredentials";
        public string Domain { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        // Intentionally session-only. TutorialConnectionStore never persists it.
        public string Password { get; set; } = string.Empty;

        public TutorialConnectionProfile Clone()
        {
            return new TutorialConnectionProfile
            {
                Name = Name,
                Target = Target,
                SiteUrl = SiteUrl,
                Id = Id,
                RememberAuthentication = RememberAuthentication,
                TenantId = TenantId,
                ClientId = ClientId,
                OnPremAuthentication = OnPremAuthentication,
                Domain = Domain,
                UserName = UserName,
                Password = Password
            };
        }

        public string TargetDisplayName
            => Target == TutorialConnectionTarget.Online
                ? "SharePoint Online"
                : "SharePoint On-Premises (OnPremSE)";

        public override string ToString()
            => string.IsNullOrWhiteSpace(Name) ? TargetDisplayName : Name;

        public void Validate(bool requireSessionSecret)
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException("Enter a profile name.");
            if (string.IsNullOrWhiteSpace(SiteUrl))
                throw new InvalidOperationException("Enter the SharePoint site URL.");

            Uri siteUri;
            if (!Uri.TryCreate(SiteUrl.Trim(), UriKind.Absolute, out siteUri) ||
                (siteUri.Scheme != Uri.UriSchemeHttp && siteUri.Scheme != Uri.UriSchemeHttps))
                throw new InvalidOperationException("The SharePoint site URL must be an absolute HTTP or HTTPS URL.");

            if (Target == TutorialConnectionTarget.Online)
            {
                if (!string.Equals(siteUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("SharePoint Online requires an HTTPS site URL.");
                // No app registration is required for the default browser-cookie flow.
                // Both legacy MSAL fields must be provided together if that older mode is selected.
                if (string.IsNullOrWhiteSpace(TenantId) != string.IsNullOrWhiteSpace(ClientId))
                    throw new InvalidOperationException("For legacy MSAL login enter BOTH Tenant ID and Client ID, or leave BOTH empty for browser-only login.");
                return;
            }

            if (!string.Equals(OnPremAuthentication, "DefaultNetworkCredentials", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(OnPremAuthentication, "Windows", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(OnPremAuthentication, "NetworkCredential", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("On-Premises authentication must be DefaultNetworkCredentials/Windows or NetworkCredential.");

            if (string.Equals(OnPremAuthentication, "NetworkCredential", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(UserName))
                    throw new InvalidOperationException("Enter the Windows user name for NetworkCredential authentication.");
                if (requireSessionSecret && string.IsNullOrEmpty(Password))
                    throw new InvalidOperationException("Enter the Windows password. Passwords are session-only and are never saved by the tutorial.");
            }
        }

        public string BuildSafeDisplayText()
        {
            if (Target == TutorialConnectionTarget.Online)
            {
                var auth = string.IsNullOrWhiteSpace(ClientId) ? "BrowserCookies (no Entra app)" : "MSAL AccessToken (legacy)";
                return Name + " — SharePoint Online / " + auth + Environment.NewLine +
                       "SiteUrl=" + SiteUrl + "; Mode=Online; Authentication=" + auth + ";";
            }

            var identity = string.Equals(OnPremAuthentication, "NetworkCredential", StringComparison.OrdinalIgnoreCase)
                ? (string.IsNullOrWhiteSpace(Domain) ? UserName : Domain + "\\" + UserName)
                : "current Windows identity";

            return Name + " — SharePoint On-Premises" + Environment.NewLine +
                   "SiteUrl=" + SiteUrl + "; Mode=OnPremSE; Authentication=" + OnPremAuthentication + "; Identity=" + identity + ";";
        }
    }

    internal sealed class TutorialConnectionStore
    {
        private const string RootElementName = "MiniErpConnections";

        public List<TutorialConnectionProfile> Profiles { get; } = new List<TutorialConnectionProfile>();
        public string SelectedProfileName { get; set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;

        public static string GetDefaultPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GDG.SharePoint.ADOProvider",
                "WinFormsMiniErpDapper",
                "Connections.xml");
        }

        public static TutorialConnectionStore LoadDefault()
            => Load(GetDefaultPath());

        public static TutorialConnectionStore Load(string path)
        {
            var store = new TutorialConnectionStore { FilePath = path ?? GetDefaultPath() };
            if (!File.Exists(store.FilePath))
                return store;

            var document = XDocument.Load(store.FilePath);
            var root = document.Root;
            if (root == null || !string.Equals(root.Name.LocalName, RootElementName, StringComparison.Ordinal))
                return store;

            store.SelectedProfileName = (string)root.Attribute("SelectedProfile") ?? string.Empty;

            bool migratedIds = false;
            foreach (var element in root.Elements("Connection"))
            {
                Guid id;
                if (!Guid.TryParse(Value(element, "Id"), out id) || id == Guid.Empty)
                {
                    id = Guid.NewGuid();
                    migratedIds = true;
                }
                TutorialConnectionTarget target;
                if (!Enum.TryParse(Value(element, "Target", "Online"), true, out target))
                    target = TutorialConnectionTarget.Online;

                store.Profiles.Add(new TutorialConnectionProfile
                {
                    Id = id,
                    RememberAuthentication = !bool.TryParse(Value(element, "RememberAuthentication", "true"), out var remember) || remember,
                    Name = Value(element, "Name"),
                    Target = target,
                    SiteUrl = Value(element, "SiteUrl"),
                    TenantId = Value(element, "TenantId"),
                    ClientId = Value(element, "ClientId"),
                    OnPremAuthentication = Value(element, "OnPremAuthentication", "DefaultNetworkCredentials"),
                    Domain = Value(element, "Domain"),
                    UserName = Value(element, "UserName"),
                    Password = string.Empty
                });
            }

            // Persist stable IDs for profiles created before browser persistence.
            // The store contains metadata only, never session cookies or passwords.
            if (migratedIds) store.Save();
            return store;
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                FilePath = GetDefaultPath();

            var directory = Path.GetDirectoryName(Path.GetFullPath(FilePath));
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            var root = new XElement(RootElementName,
                new XAttribute("SelectedProfile", SelectedProfileName ?? string.Empty));

            foreach (var profile in Profiles.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
            {
                root.Add(new XElement("Connection",
                    new XElement("Id", profile.Id.ToString("D")),
                    new XElement("RememberAuthentication", profile.RememberAuthentication),
                    new XElement("Name", profile.Name ?? string.Empty),
                    new XElement("Target", profile.Target.ToString()),
                    new XElement("SiteUrl", profile.SiteUrl ?? string.Empty),
                    new XElement("TenantId", profile.TenantId ?? string.Empty),
                    new XElement("ClientId", profile.ClientId ?? string.Empty),
                    new XElement("OnPremAuthentication", profile.OnPremAuthentication ?? string.Empty),
                    new XElement("Domain", profile.Domain ?? string.Empty),
                    new XElement("UserName", profile.UserName ?? string.Empty)));
            }

            new XDocument(root).Save(FilePath);
        }

        public TutorialConnectionProfile FindSelectedProfile()
        {
            if (string.IsNullOrWhiteSpace(SelectedProfileName))
                return null;

            var profile = Profiles.FirstOrDefault(p =>
                string.Equals(p.Name, SelectedProfileName, StringComparison.OrdinalIgnoreCase));
            return profile == null ? null : profile.Clone();
        }

        private static string Value(XElement parent, string name, string fallback = "")
        {
            var element = parent.Element(name);
            return element == null ? fallback : (element.Value ?? string.Empty).Trim();
        }
    }

    internal sealed class InteractiveBrowserAuthenticator
    {
        private readonly Dictionary<string, IPublicClientApplication> _applications
            = new Dictionary<string, IPublicClientApplication>(StringComparer.OrdinalIgnoreCase);
        private sealed class BrowserSession
        {
            public string FedAuth;
            public string RtFa;
            public bool RememberAuthentication;
        }

        private readonly Dictionary<string, BrowserSession> _browserSessions =
            new Dictionary<string, BrowserSession>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _forceInteractive = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static string SessionKey(TutorialConnectionProfile profile)
        {
            var uri = new Uri(profile.SiteUrl.Trim(), UriKind.Absolute);
            return profile.Id.ToString("N") + "|" + uri.Host.ToLowerInvariant();
        }

        public void ClearBrowserSession(TutorialConnectionProfile profile)
        {
            if (profile == null) return;
            var key = SessionKey(profile);
            _browserSessions.Remove(key);
            _forceInteractive.Add(key);
        }

        public void ForgetBrowserSession(TutorialConnectionProfile profile)
        {
            if (profile == null) return;
            ClearBrowserSession(profile);
            BrowserSignInProfileStore.Forget(profile);
        }

        public bool TryAcquireBrowserSession(TutorialConnectionProfile profile,
            out string fedAuth, out string rtFa)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            profile.Validate(requireSessionSecret: true);
            var key = SessionKey(profile);
            var force = _forceInteractive.Remove(key);
            BrowserSession session;
            if (_browserSessions.TryGetValue(key, out session) &&
                (force || session.RememberAuthentication != profile.RememberAuthentication))
            {
                _browserSessions.Remove(key);
                session = null;
            }
            if (session == null)
            {
                using (var dialog = new BrowserLoginForm(profile, force))
                {
                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        fedAuth = rtFa = string.Empty;
                        return false;
                    }
                    session = new BrowserSession { FedAuth = dialog.FedAuth, RtFa = dialog.RtFa,
                        RememberAuthentication = profile.RememberAuthentication };
                    _browserSessions[key] = session;
                }
            }
            fedAuth = session.FedAuth;
            rtFa = session.RtFa;
            return true;
        }

        public async Task<string> AcquireAccessTokenAsync(TutorialConnectionProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            profile.Validate(requireSessionSecret: true);

            var siteUri = new Uri(profile.SiteUrl.Trim());
            string authorityTenant = profile.TenantId.Trim();
            string clientId = profile.ClientId.Trim();
            string cacheKey = authorityTenant + "|" + clientId;

            IPublicClientApplication application;
            if (!_applications.TryGetValue(cacheKey, out application))
            {
                application = PublicClientApplicationBuilder
                    .Create(clientId)
                    .WithAuthority("https://login.microsoftonline.com/" + authorityTenant)
                    .WithRedirectUri("http://localhost")
                    .Build();
                _applications[cacheKey] = application;
            }

            // .default uses the delegated SharePoint permissions configured on the public-client app.
            // The actual provider connection remains Authentication=AccessToken.
            string[] scopes =
            {
                siteUri.Scheme + "://" + siteUri.Host + "/.default"
            };

            var accounts = (await application.GetAccountsAsync().ConfigureAwait(true)).ToList();
            if (accounts.Count > 0)
            {
                try
                {
                    var silent = await application.AcquireTokenSilent(scopes, accounts[0])
                        .ExecuteAsync()
                        .ConfigureAwait(true);
                    return silent.AccessToken;
                }
                catch (MsalUiRequiredException)
                {
                    // Fall through to the system-browser interactive flow.
                }
            }

            var interactive = await application.AcquireTokenInteractive(scopes)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync()
                .ConfigureAwait(true);
            return interactive.AccessToken;
        }
    }

    internal static class TutorialConnectionStringFactory
    {
        public static async Task<string> BuildAsync(
            TutorialConnectionProfile profile,
            InteractiveBrowserAuthenticator browserAuthenticator)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (browserAuthenticator == null) throw new ArgumentNullException(nameof(browserAuthenticator));
            profile.Validate(requireSessionSecret: true);

            var builder = new SharePointConnectionStringBuilder
            {
                SiteUrl = profile.SiteUrl.Trim(),
                DiagnosticsEnabled = true
            };

            if (profile.Target == TutorialConnectionTarget.Online)
            {
                builder.Mode = "Online";
                if (string.IsNullOrWhiteSpace(profile.ClientId))
                {
                    string fedAuth, rtFa;
                    if (!browserAuthenticator.TryAcquireBrowserSession(profile, out fedAuth, out rtFa))
                        return null; // Explicit user cancellation, not a connection failure.
                    builder.Authentication = "BrowserCookies";
                    builder.BrowserFedAuth = fedAuth;
                    builder.BrowserRtFa = rtFa;
                }
                else
                {
                    // Preserve previously configured public-client MSAL profiles.
                    var token = await browserAuthenticator.AcquireAccessTokenAsync(profile).ConfigureAwait(true);
                    builder.Authentication = "AccessToken";
                    builder.AccessToken = token;
                }
                return builder.ConnectionString;
            }

            builder.Mode = "OnPremSE";
            builder.Authentication = profile.OnPremAuthentication;
            if (string.Equals(profile.OnPremAuthentication, "NetworkCredential", StringComparison.OrdinalIgnoreCase))
            {
                builder.Domain = profile.Domain;
                builder.UserName = profile.UserName;
                builder.Password = profile.Password;
            }

            return builder.ConnectionString;
        }
    }
}
