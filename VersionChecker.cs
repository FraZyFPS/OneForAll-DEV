using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

#nullable disable
namespace OneForAll.MiniMap
{
    public class VersionChecker
    {
        public static string latestVersion;

        private static async Task<bool> GetVersionFromUrlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", VersionChecker.BuildUserAgentString());
                try
                {
                    string response = await client.GetStringAsync(url);
                    VersionChecker.latestVersion = VersionChecker.ParseTagNameFromJson(response);
                    return true;
                }
                catch (Exception ex)
                {
                    MinimapMod.mls.LogError("Failed to get the latest version from " + url + " for Minimap Mod. Error: " + ex.Message);
                    return false;
                }
            }
        }

        public static async Task GetLatestVersionAsync()
        {
            string mainUrl = "https://FraZyFPS.OneForAll.com";
            string fallbackUrl = "https://api.github.com/FraZyFPS/OneForAll/releases/tag/v1.0.4";
            if (await VersionChecker.GetVersionFromUrlAsync(mainUrl))
            {
                mainUrl = null;
                fallbackUrl = null;
            }
            else
            {
                int num = await VersionChecker.GetVersionFromUrlAsync(fallbackUrl) ? 1 : 0;
                mainUrl = null;
                fallbackUrl = null;
            }
        }

        private static string ParseTagNameFromJson(string jsonString)
        {
            string str = "\"tag_name\":\"";
            int num1 = jsonString.IndexOf(str);
            if (num1 != -1)
            {
                int startIndex = num1 + str.Length;
                int num2 = jsonString.IndexOf("\"", startIndex);
                if (num2 != -1)
                    return VersionChecker.LStrip(jsonString.Substring(startIndex, num2 - startIndex), 'v');
            }
            return null;
        }

        private static string LStrip(string input, char charToStrip)
        {
            int num = 0;
            while (num < input.Length && input[num] == charToStrip)
                ++num;
            return input.Substring(num);
        }

        private static string BuildUserAgentString()
        {
            return "OneForAll/1.0.4 (" + VersionChecker.GetOperatingSystemInfo() + "; " + (Environment.Is64BitOperatingSystem ? "x64" : "x86") + ")";
        }

        private static string GetOperatingSystemInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return string.Format("Windows NT {0}", Environment.OSVersion.Version);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return string.Format("Mac OS X {0}", Environment.OSVersion.Version);
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? string.Format("Linux {0}", Environment.OSVersion.Version) : string.Format("Unknown OS {0}", Environment.OSVersion.Version);
        }
    }
}