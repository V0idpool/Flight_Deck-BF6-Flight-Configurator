using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlightDeck_BF6_Flight_Configurator.Helpers
{
    internal class UpdateEngine
    {
        public static Version? currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static string tempDir = Path.Combine(Path.GetTempPath(), "FlightDeckUpdate");
        public static string updateUrl = "https://voidtechstudios.us/FlightDeckApp/version.txt";
        public static string nexusModsUrl = "https://www.nexusmods.com/battlefield6/mods/25";
        public static string? releaseSummary = null;
        public static async Task CheckForUpdatesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    HttpResponseMessage response = await client.GetAsync(updateUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string rawContent = (await response.Content.ReadAsStringAsync()).Trim();
                        if (string.IsNullOrWhiteSpace(rawContent))
                            return;

                        string[] lines = rawContent.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                        string latestVersionTxt = lines[0].Trim().TrimStart('v', 'V');
                        string releaseNotes = lines.Length > 1
                            ? string.Join(Environment.NewLine + Environment.NewLine, lines[1..]).Trim()
                            : string.Empty;

                        if (Version.TryParse(latestVersionTxt, out Version? onlineVersion))
                        {
                            if (onlineVersion > currentVersion)
                            {
                                ExceptionHandler.LogMessage($"[UPDATE] New version v{onlineVersion} is available on NexusMods (Installed: v{currentVersion}).");
                                ShowUpdateAvailableDialog(currentVersion.ToString(), onlineVersion.ToString(), releaseNotes);
                            }
                            else
                            {
                                ExceptionHandler.LogMessage($"[UPDATE] Flight Deck v{currentVersion} is up to date.");
                            }
                        }
                    }
                    else
                    {
                        Helpers.ExceptionHandler.LogMessage($"Update Check: Server returned {response.StatusCode}. Continuing offline.");
                    }
                }
            }
            catch (HttpRequestException)
            {
                Helpers.ExceptionHandler.LogMessage("Update Check: No internet connection detected. Continuing offline.");
            }
            catch (TaskCanceledException)
            {
                Helpers.ExceptionHandler.LogMessage("Update Check: Server timed out. Continuing offline.");
            }
            catch (Exception ex)
            {
                Helpers.ExceptionHandler.LogError(ex);
            }
        }

        private static void ShowUpdateAvailableDialog(string currentVer, string latestVer, string releaseNotes)
        {
            using var dlg = new UpdateDlg(currentVer, latestVer, releaseNotes);
            DialogResult result = MainForm.Instance is not null
                ? dlg.ShowDialog(MainForm.Instance)
                : dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                Process.Start(new ProcessStartInfo(nexusModsUrl) { UseShellExecute = true });
            }
        }
    }
}
