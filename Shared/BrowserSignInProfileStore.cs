// Autore: Gabriele Del Giovine
// (c) Gabriele Del Giovine 2026

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GDG.SharePoint.ADOProvider.WinFormsTutorial
{
    /// <summary>
    /// Isolates persistent WebView2 browser data by immutable connection-profile ID
    /// and SharePoint host. Never stores cookie values or passwords in Connections.xml.
    /// </summary>
    internal static class BrowserSignInProfileStore
    {
        internal static string GetDirectory(TutorialConnectionProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (profile.Id == Guid.Empty) throw new ArgumentException("Connection profile requires an ID.");
            var uri = new Uri(profile.SiteUrl.Trim(), UriKind.Absolute);
            if (uri.Scheme != Uri.UriSchemeHttps ||
                !uri.Host.EndsWith(".sharepoint.com", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only HTTPS SharePoint Online sites support browser session persistence.");
            var scope = profile.Id.ToString("N") + "|" + uri.Host.ToLowerInvariant();
            byte[] digest;
            using (var sha = SHA256.Create())
                digest = sha.ComputeHash(Encoding.UTF8.GetBytes(scope));
            var key = BitConverter.ToString(digest).Replace("-", "").ToLowerInvariant();
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GDG.SharePoint.ADOProvider", "WinFormsTutorial", "BrowserProfiles", key);
        }

        internal static string GetInPrivateDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GDG.SharePoint.ADOProvider", "WinFormsTutorial", "BrowserRuntime");
        }

        internal static void Forget(TutorialConnectionProfile profile)
        {
            // This can only delete the one dedicated profile directory computed
            // from a valid Guid and host; never delete the parent browser root.
            var directory = GetDirectory(profile);
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }
}
