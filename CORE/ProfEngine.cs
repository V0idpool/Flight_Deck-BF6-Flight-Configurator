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
                    // Ignore inaccessible directories
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

        public static bool SyncLiveUserProfile(string profilePath, FlightScheme targetScheme)
        {
            var map = ReadMap(profilePath);
            if (map.Count == 0) return false;

            bool hasCustomHeli = map.TryGetValue("GstInput.Scheme5Buttons", out string? b5) && b5.Trim() == "4";
            bool hasCustomJet = map.TryGetValue("GstInput.Scheme2Buttons", out string? b2) && b2.Trim() == "4";

            if (hasCustomHeli)
            {
                foreach (var (k, v) in map)
                {
                    if (k.StartsWith("GstKeyBinding.helicopter.", StringComparison.Ordinal))
                        targetScheme.RawVariables[k] = v;
                }
                targetScheme.HasCustomHeliBinds = true;
                targetScheme.RawVariables["GstInput.Scheme5Buttons"] = "4";
            }

            if (hasCustomJet)
            {
                foreach (var (k, v) in map)
                {
                    if (k.StartsWith("GstKeyBinding.jet.", StringComparison.Ordinal))
                        targetScheme.RawVariables[k] = v;
                }
                targetScheme.HasCustomJetBinds = true;
                targetScheme.RawVariables["GstInput.Scheme2Buttons"] = "4";
            }

            return hasCustomHeli || hasCustomJet;
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

            bool writeCustomHeliBinds = scheme.HasCustomHeliBinds ||
                (scheme.RawVariables.TryGetValue("GstInput.Scheme5Buttons", out string? s5b) && s5b.Trim() == "4");
            bool writeCustomJetBinds = scheme.HasCustomJetBinds ||
                (scheme.RawVariables.TryGetValue("GstInput.Scheme2Buttons", out string? s2b) && s2b.Trim() == "4");

            List<string> lines = File.ReadAllLines(profilePath, Encoding.UTF8)
                .Where(l => !l.StartsWith("GstKeyBinding.heli.", StringComparison.Ordinal) &&
                            !l.StartsWith("GstKeyBinding.helicopter.", StringComparison.Ordinal) &&
                            !l.StartsWith("GstKeyBinding.jet.", StringComparison.Ordinal) &&
                            !l.StartsWith("GstKeyBinding.version ", StringComparison.Ordinal))
                .ToList();

            var handledInputKeys = new HashSet<string>(StringComparer.Ordinal);
            int updatedCount = 0;
            int lastGstInputLineIndex = -1;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                int spaceIdx = line.IndexOf(' ');
                if (spaceIdx <= 0) continue;

                string key = line[..spaceIdx];

                if (key.StartsWith("GstInput.", StringComparison.Ordinal))
                {
                    lastGstInputLineIndex = i;

                    if (scheme.RawVariables.TryGetValue(key, out string? targetValue))
                    {
                        handledInputKeys.Add(key);
                        string currentValue = line[(spaceIdx + 1)..];
                        if (!string.Equals(currentValue, targetValue, StringComparison.Ordinal))
                        {
                            lines[i] = $"{key} {targetValue}";
                            updatedCount++;
                        }
                    }
                }
            }

            var missingInputs = scheme.RawVariables
                .Where(kv => !handledInputKeys.Contains(kv.Key) && kv.Key.StartsWith("GstInput.", StringComparison.Ordinal))
                .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                .Select(kv => $"{kv.Key} {kv.Value}")
                .ToList();

            int injectedCount = missingInputs.Count;

            if (missingInputs.Count > 0)
            {
                int insertPos = lastGstInputLineIndex >= 0 ? lastGstInputLineIndex + 1 : lines.Count;
                lines.InsertRange(insertPos, missingInputs);
            }

            var keyBindLinesToInsert = new List<string>();

            if (writeCustomHeliBinds)
            {
                keyBindLinesToInsert.AddRange(
                    scheme.RawVariables
                        .Where(kv => kv.Key.StartsWith("GstKeyBinding.helicopter.", StringComparison.Ordinal))
                        .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                        .Select(kv => $"{kv.Key} {kv.Value}"));
            }

            if (writeCustomJetBinds)
            {
                keyBindLinesToInsert.AddRange(
                    scheme.RawVariables
                        .Where(kv => kv.Key.StartsWith("GstKeyBinding.jet.", StringComparison.Ordinal))
                        .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                        .Select(kv => $"{kv.Key} {kv.Value}"));
            }

            bool fileHasOtherKeyBinds = lines.Any(l => l.StartsWith("GstKeyBinding.", StringComparison.Ordinal));
            if (keyBindLinesToInsert.Count > 0 || fileHasOtherKeyBinds)
            {
                keyBindLinesToInsert.Add("GstKeyBinding.version 2000007");

                int bindInsertIndex = lines.FindIndex(l => l.StartsWith("GstInput.", StringComparison.Ordinal));
                if (bindInsertIndex < 0) bindInsertIndex = lines.Count;

                lines.InsertRange(bindInsertIndex, keyBindLinesToInsert);
                injectedCount += keyBindLinesToInsert.Count;
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
                sb.AppendLine($"HasCustomHeliBinds={scheme.HasCustomHeliBinds}");
                sb.AppendLine($"HasCustomJetBinds={scheme.HasCustomJetBinds}");
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
                writer.WriteBoolean("HasCustomHeliBinds", scheme.HasCustomHeliBinds);
                writer.WriteBoolean("HasCustomJetBinds", scheme.HasCustomJetBinds);

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
                Subtitle = $"Custom Preset ({Path.GetFileName(filePath)})",
                SourceFilePath = filePath,
                IsCustom = true
            }.EnsureBaselineTables();

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
                    else if (k.Equals("HasCustomHeliBinds", StringComparison.OrdinalIgnoreCase) && bool.TryParse(v, out bool ch))
                        scheme.HasCustomHeliBinds = ch;
                    else if (k.Equals("HasCustomJetBinds", StringComparison.OrdinalIgnoreCase) && bool.TryParse(v, out bool cj))
                        scheme.HasCustomJetBinds = cj;
                    else if (k.Equals("HasCustomButtonBinds", StringComparison.OrdinalIgnoreCase) && bool.TryParse(v, out bool cb))
                        scheme.HasCustomButtonBinds = cb;
                    else if (!k.StartsWith("GstKeyBinding.heli.", StringComparison.Ordinal))
                        scheme.RawVariables[k] = v;
                }
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

                if (root.TryGetProperty("HasCustomHeliBinds", out JsonElement chProp) &&
                    (chProp.ValueKind == JsonValueKind.True || chProp.ValueKind == JsonValueKind.False))
                {
                    scheme.HasCustomHeliBinds = chProp.GetBoolean();
                }

                if (root.TryGetProperty("HasCustomJetBinds", out JsonElement cjProp) &&
                    (cjProp.ValueKind == JsonValueKind.True || cjProp.ValueKind == JsonValueKind.False))
                {
                    scheme.HasCustomJetBinds = cjProp.GetBoolean();
                }

                if (root.TryGetProperty("RawVariables", out JsonElement varsProp) && varsProp.ValueKind == JsonValueKind.Object)
                {
                    foreach (JsonProperty prop in varsProp.EnumerateObject())
                    {
                        if (!prop.Name.StartsWith("GstKeyBinding.heli.", StringComparison.Ordinal))
                            scheme.RawVariables[prop.Name] = prop.Value.GetString() ?? string.Empty;
                    }
                }
            }

            if (scheme.RawVariables.TryGetValue("GstInput.Scheme5Buttons", out string? b5) && b5.Trim() == "4")
                scheme.HasCustomHeliBinds = true;
            if (scheme.RawVariables.TryGetValue("GstInput.Scheme2Buttons", out string? b2) && b2.Trim() == "4")
                scheme.HasCustomJetBinds = true;

            scheme.RawVariables["GstKeyBinding.version"] = "2000007";

            int stickIdx = 0;
            if (scheme.RawVariables.TryGetValue("GstInput.Scheme5Sticks", out string? sSticks) && int.TryParse(sSticks.Trim(), out int parsedStick))
                stickIdx = Math.Clamp(parsedStick, 0, 3);

            bool invert = !scheme.RawVariables.TryGetValue("GstInput.Scheme5FlipY", out string? sFlip) || sFlip.Trim() == "1";
            bool triggersAreThrottle = !scheme.RawVariables.TryGetValue("GstInput.Scheme5Buttons", out string? btnMode) || btnMode.Trim() != "0";
            FlightSchemeRegistry.ApplyStickAxes(scheme, (StickLayoutType)stickIdx, triggersAreThrottle, invert);

            return scheme;
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