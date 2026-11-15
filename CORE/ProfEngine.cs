using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using static FlightDeck_BF6_Flight_Configurator.CORE.FlightPresets;

namespace FlightDeck_BF6_Flight_Configurator.CORE
{
    public sealed record PatchSummary(bool Success, string Message, int UpdatedCount, int InjectedCount, string BackupPath);

    public sealed record ProfileTuningSnapshot(
        bool InvertPitch,
        StickLayoutType StickLayout,
        decimal HeliSensitivity,
        decimal JetSensitivity,
        decimal CenterDeadzone
    );

    public static class ProfEngine
    {
        private static readonly string[] GameProcessNames = ["BF6", "Battlefield6", "bf6mp", "EAAntiCheat.GameServiceLauncher"];

        public static List<string> FindProfilePaths()
        {
            var found = new List<string>();
            string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string[] roots =
            [
                docs,
                Path.Combine(user, "Documents"),
                Path.Combine(user, "OneDrive", "Documents")
            ];

            foreach (string root in roots.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(root)) continue;

                try
                {
                    foreach (string bfDir in Directory.EnumerateDirectories(root, "*Battlefield*6*", SearchOption.TopDirectoryOnly))
                    {
                        foreach (string file in Directory.EnumerateFiles(bfDir, "PROFSAVE*", SearchOption.AllDirectories))
                        {
                            if (IsValidProfileFile(file))
                                found.Add(file);
                        }
                    }
                }
                catch
                {
                    // Do nothing
                }
            }

            return found.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static bool IsValidProfileFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (fileName.Contains(".bak_", StringComparison.OrdinalIgnoreCase) ||
                fileName.EndsWith(".snapshot_temp", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return fileName.EndsWith("_profile", StringComparison.OrdinalIgnoreCase) ||
                   fileName.Contains("_profile", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBattlefieldRunning() =>
            GameProcessNames.Any(name => Process.GetProcessesByName(name).Length > 0);

        public static ProfileTuningSnapshot ReadCurrentTuning(string profilePath)
        {
            var map = ReadMap(profilePath);

            bool invert = !map.TryGetValue("GstInput.Scheme5FlipY", out string? flip) || flip.Trim() == "1";

            int stickIdx = 0;
            if (map.TryGetValue("GstInput.Scheme5Sticks", out string? sSticks) && int.TryParse(sSticks.Trim(), out int parsedStick))
                stickIdx = Math.Clamp(parsedStick, 0, 3);

            decimal heliSens = 1.00m;
            if (map.TryGetValue("GstInput.VehicleSensitivityHelicopterPilot", out string? sHeli) &&
                decimal.TryParse(sHeli.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal pHeli))
            {
                heliSens = Math.Clamp(pHeli, 0.10m, 3.00m);
            }

            decimal jetSens = 1.00m;
            if (map.TryGetValue("GstInput.VehicleSensitivityPlanePilot", out string? sJet) &&
                decimal.TryParse(sJet.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal pJet))
            {
                jetSens = Math.Clamp(pJet, 0.10m, 3.00m);
            }

            decimal deadzone = 0.08m;
            if (map.TryGetValue("GstInput.GamepadLeftStickDeadzoneCenter", out string? sDz) &&
                decimal.TryParse(sDz.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal pDz))
            {
                deadzone = Math.Clamp(pDz, 0.00m, 0.50m);
            }

            return new ProfileTuningSnapshot(invert, (StickLayoutType)stickIdx, heliSens, jetSens, deadzone);
        }

        public static PatchSummary ApplyScheme(string profilePath, FlightScheme scheme, bool lockReadOnly)
        {
            if (!File.Exists(profilePath))
                return new(false, "Target profile file does not exist.", 0, 0, string.Empty);

            if (IsBattlefieldRunning())
                return new(false, "Battlefield 6 is currently running! Close the game first so it doesn't overwrite your profile on exit.", 0, 0, string.Empty);

            string firstLine = File.ReadLines(profilePath).FirstOrDefault() ?? string.Empty;
            if (firstLine.StartsWith("FBCHUNKS", StringComparison.Ordinal))
            {
                return new(
                    false,
                    $"'{Path.GetFileName(profilePath)}' is the binary FBCHUNKS file. Select 'PROFSAVEbf6mp_profile' in the same folder instead.",
                    0, 0, string.Empty);
            }

            var fileInfo = new FileInfo(profilePath);
            if (fileInfo.IsReadOnly)
                fileInfo.IsReadOnly = false;

            string backupPath = $"{profilePath}.bak_{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Copy(profilePath, backupPath, overwrite: true);

            List<string> lines = File.ReadAllLines(profilePath, Encoding.UTF8)
                .Where(l => !l.StartsWith("GstKeyBinding.", StringComparison.Ordinal))
                .ToList();

            var handledKeys = new HashSet<string>(StringComparer.Ordinal);
            int updatedCount = 0;
            int lastGstInputLineIndex = -1;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                int spaceIdx = line.IndexOf(' ');
                if (spaceIdx <= 0) continue;

                string key = line[..spaceIdx];

                if (key.StartsWith("GstInput.", StringComparison.Ordinal))
                    lastGstInputLineIndex = i;

                if (key.StartsWith("GstInput.", StringComparison.Ordinal) &&
                    scheme.RawVariables.TryGetValue(key, out string? targetValue))
                {
                    handledKeys.Add(key);
                    string currentValue = line[(spaceIdx + 1)..];
                    if (!string.Equals(currentValue, targetValue, StringComparison.Ordinal))
                    {
                        lines[i] = $"{key} {targetValue}";
                        updatedCount++;
                    }
                }
            }

            var missingInputs = scheme.RawVariables
                .Where(kv => !handledKeys.Contains(kv.Key) && kv.Key.StartsWith("GstInput.", StringComparison.Ordinal))
                .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                .Select(kv => $"{kv.Key} {kv.Value}")
                .ToList();

            int injectedCount = missingInputs.Count;

            if (missingInputs.Count > 0)
            {
                int insertPos = lastGstInputLineIndex >= 0 ? lastGstInputLineIndex + 1 : lines.Count;
                lines.InsertRange(insertPos, missingInputs);
            }

            using (var writer = new StreamWriter(profilePath, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                foreach (string line in lines)
                    writer.WriteLine(line);
            }

            if (lockReadOnly)
                fileInfo.IsReadOnly = true;

            return new(
                Success: true,
                Message: $"Applied '{scheme.Title}' to {Path.GetFileName(profilePath)}.",
                UpdatedCount: updatedCount,
                InjectedCount: injectedCount,
                BackupPath: backupPath
            );
        }

        public static void ExportPresetToFile(FlightScheme scheme, string filePath)
        {
            if (filePath.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
            {
                var sb = new StringBuilder();
                sb.AppendLine("; FlightDeck | BF6 Flight Configurator Preset");
                sb.AppendLine("[Metadata]");
                sb.AppendLine($"Title={scheme.Title}");
                sb.AppendLine($"Subtitle={scheme.Subtitle}");
                sb.AppendLine();
                sb.AppendLine("[Variables]");
                foreach (var (k, v) in scheme.RawVariables.OrderBy(x => x.Key, StringComparer.Ordinal))
                    sb.AppendLine($"{k}={v}");

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            else
            {
                using var fs = File.Create(filePath);
                using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true });

                writer.WriteStartObject();
                writer.WriteString("Title", scheme.Title);
                writer.WriteString("Subtitle", scheme.Subtitle);

                writer.WriteStartObject("RawVariables");
                foreach (var (k, v) in scheme.RawVariables.OrderBy(x => x.Key, StringComparer.Ordinal))
                    writer.WriteString(k, v);
                writer.WriteEndObject();

                writer.WriteEndObject();
            }
        }

        public static FlightScheme ImportPresetFromFile(string filePath)
        {
            var scheme = new FlightScheme
            {
                Title = Path.GetFileNameWithoutExtension(filePath),
                Subtitle = $"Imported from {Path.GetFileName(filePath)}"
            };

            if (filePath.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string rawLine in File.ReadLines(filePath, Encoding.UTF8))
                {
                    string line = rawLine.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith(';') || line.StartsWith('#') || line.StartsWith('['))
                        continue;

                    int eqIdx = line.IndexOf('=');
                    if (eqIdx <= 0) continue;

                    string k = line[..eqIdx].Trim();
                    string v = line[(eqIdx + 1)..].Trim();

                    if (k.Equals("Title", StringComparison.OrdinalIgnoreCase))
                        scheme.Title = v;
                    else if (k.Equals("Subtitle", StringComparison.OrdinalIgnoreCase))
                        scheme.Subtitle = v;
                    else
                        scheme.RawVariables[k] = v;
                }
                return scheme;
            }
            else
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                using var doc = JsonDocument.Parse(bytes);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("Title", out JsonElement tProp) && tProp.GetString() is string t)
                    scheme.Title = t;

                if (root.TryGetProperty("Subtitle", out JsonElement sProp) && sProp.GetString() is string s)
                    scheme.Subtitle = s;

                if (root.TryGetProperty("RawVariables", out JsonElement varsProp) && varsProp.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty prop in varsProp.EnumerateObject())
                        scheme.RawVariables[prop.Name] = prop.Value.GetString() ?? string.Empty;
                }

                return scheme;
            }
        }

        private static Dictionary<string, string> ReadMap(string path)
        {
            var dict = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!File.Exists(path)) return dict;

            foreach (string line in File.ReadLines(path, Encoding.UTF8))
            {
                int idx = line.IndexOf(' ');
                if (idx > 0)
                    dict[line[..idx]] = line[(idx + 1)..];
            }
            return dict;
        }

        public static List<string> GetBackupFiles(string profilePath)
        {
            if (string.IsNullOrWhiteSpace(profilePath))
                return [];

            string? dir = Path.GetDirectoryName(profilePath);
            string fileName = Path.GetFileName(profilePath);
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return [];

            return Directory.EnumerateFiles(dir, $"{fileName}.bak_*")
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .ToList();
        }

        public static PatchSummary RestoreLatestBackup(string profilePath)
        {
            var backups = GetBackupFiles(profilePath);
            if (backups.Count == 0)
                return new(false, "No backup files (.bak_*) found in the settings folder yet.", 0, 0, string.Empty);

            return RestoreFromSpecificBackup(profilePath, backups[0]);
        }

        public static PatchSummary RestoreFromSpecificBackup(string profilePath, string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(profilePath))
                return new(false, "No target profile path selected.", 0, 0, string.Empty);

            if (!File.Exists(backupFilePath))
                return new(false, $"Backup file not found: {Path.GetFileName(backupFilePath)}", 0, 0, string.Empty);

            if (IsBattlefieldRunning())
                return new(false, "Close Battlefield 6 before restoring a backup.", 0, 0, string.Empty);

            var targetInfo = new FileInfo(profilePath);
            if (targetInfo.Exists && targetInfo.IsReadOnly)
                targetInfo.IsReadOnly = false;

            File.Copy(backupFilePath, profilePath, overwrite: true);

            return new(true, $"Restored backup '{Path.GetFileName(backupFilePath)}'.", 0, 0, backupFilePath);
        }
    }
}