using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

namespace SparkCore.Editor.Setup
{
    public static class Packages
    {
        private const string MainfestGistID = "65d3a8c9be7d1a441419641af1b66701";
        public static async void InstallCoreDependencies()
        {
            var url = GetGistUrl(MainfestGistID);
            var contents = await GetContents(url);
            ReplacePackageFile(contents);
        }

        private static string GetGistUrl(string id, string user = "muammar-yacoob")
        {
            return $"https://gist.githubusercontent.com/{user}/{id}/raw";
        }

        private static async Task<string> GetContents(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }

        private static void ReplacePackageFile(string contents)
        {
            var existing = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            File.WriteAllText(existing, contents);
            Client.Resolve();
        }

        public static void InstallUnityPackage(string packageName)
        {
             UnityEditor.PackageManager.Client.Add($"com.unity.{packageName}");
        }
    }
}