using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MakeJsonFile
{
    public class WebManifest
    {
        public DateOnly ColorsLastUpdated { get; set; }
        public DateOnly VendorsLastUpdated { get; set; }
        public DateOnly MaterialLastUpdated { get; set; }

        public static async Task<Manifest> GetWebManifestAsync()
        {
            var http = new HttpClient() { Timeout = new TimeSpan(0, 0, 20) };
            //var shareId = ToShareId("https://onedrive.live.com/?id=%2Fpersonal%2Fc4fbbe9a9de3eb18%2FDocuments%2FDocuments2%2FFilamentSharedContent%2FManifest%2Ejson&parent=%2Fpersonal%2Fc4fbbe9a9de3eb18%2FDocuments%2FDocuments2%2FFilamentSharedContent");
            //var downloadUrl = $"https://graph.microsoft.com/v1.0/shares/{shareId}/root/content";
            var json = await http.GetStringAsync("https://github.com/markinnh/MakeJsonFile/blob/master/MakeJsonFile/SharedFilamentData/Manifest.json?raw=true");
            var manifest = System.Text.Json.JsonSerializer.Deserialize<Manifest>(json) ?? new Manifest();
            return manifest;
        }
        public static string ToShareId(string shareUrl)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(shareUrl);
            var base64 = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            return "u!" + base64;
        }
        public static async Task<string> DownloadJsonFromOneDriveAsync(string shareUrl)
        {
            var shareId = ToShareId(shareUrl);

            var downloadUrl =
                $"https://graph.microsoft.com/v1.0/shares/{shareId}/root/content";

            using var http = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };

            var response = await http.GetStringAsync(downloadUrl);



            return response;
        }
        public static async Task<string> ResolveOneDriveDirectUrlAsync(string shareUrl)
        {
            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            using var client = new HttpClient(handler);

            string currentUrl = shareUrl;

            for (int i = 0; i < 10; i++)
            {
                var response = await client.GetAsync(currentUrl);

                // If we get a 200, we reached the real file
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return currentUrl;
                }

                // If we get a redirect, follow it
                if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                {
                    currentUrl = response.Headers.Location.ToString();
                    continue;
                }

                throw new Exception($"Unexpected status code: {response.StatusCode}");
            }

            throw new Exception("Too many redirects.");
        }
        public static async Task<string> ResolveOneDriveDownloadUrlAsync(string shareUrl)
        {
            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            using var client = new HttpClient(handler) { BaseAddress=new Uri("https://onedrive.live.com") };

            string current = shareUrl;

            for (int i = 0; i < 10; i++)
            {
                var response = await client.GetAsync(current);

                // If OneDrive gives us the file directly
                if (response.StatusCode == HttpStatusCode.OK)
                    return current;

                // Follow redirects
                if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                {
                    var next = response.Headers.Location?.ToString();

                    // If redirected to Microsoft login → link is NOT anonymous
                    if (next != null && next.Contains("login.live.com"))
                        throw new InvalidOperationException(
                            "This OneDrive link requires authentication. You must create an 'Anyone with the link can view' link."
                        );

                    current = next;
                    continue;
                }

                throw new Exception($"Unexpected status code: {response.StatusCode}");
            }

            throw new Exception("Too many redirects.");
        }
        public static async Task<string> DownloadJsonAsync(string shareUrl)
        {
            var directUrl = await ResolveOneDriveDownloadUrlAsync(shareUrl);
            var downloadUrl = $"https://onedrive.live.com/{directUrl}";
            using var http = new HttpClient();
            return await http.GetStringAsync(directUrl);
        }
    }
}
