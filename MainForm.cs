#pragma warning disable WFO1000

using FlightDeck_BF6_Flight_Configurator.CORE;
using FlightDeck_BF6_Flight_Configurator.GUI;
using FlightDeck_BF6_Flight_Configurator.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlightDeck_BF6_Flight_Configurator
{
    public partial class MainForm : Form
    {
        public static MainForm? Instance { get; private set; }
        private bool _suppressPreviewUpdate;
        private static string CustomPresetsDirectory => Path.Combine(AppContext.BaseDirectory, "CustomPresets");
        private static string AppSettingsFilePath => Path.Combine(AppContext.BaseDirectory, "flightdeck.settings.json");
        private string _nexusApiKey = string.Empty;
        private string _lastSelectedPresetTitle = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            Instance = this;
            EnsureCustomControls();
            _btnBrowse.SendToBack();
            _cmbProfilePath.BringToFront();
            _cmsBackups.Renderer = new ToolStripProfessionalRenderer(new ColorTable());
            _cmsBackups.ForeColor = Color.FromArgb(240, 244, 248);
            _cmsBackups.Font = new Font("Segoe UI", 9.25f, FontStyle.Regular);

            _suppressPreviewUpdate = true;

            foreach (var stickLayout in Enum.GetValues<FlightPresets.StickLayoutType>())
                _cmbSticks.Items.Add(stickLayout);

            if (_cmbSticks.Items.Count > 0)
                _cmbSticks.SelectedIndex = 0;

            LoadAppPreferences();
            ReloadPresetDropdown(_lastSelectedPresetTitle);

            if (_cmbScheme.Items.Count > 0 && _cmbScheme.SelectedIndex < 0)
                _cmbScheme.SelectedIndex = 0;

            SyncUIFromProfile();
            LoadDetectedProfiles();

            _suppressPreviewUpdate = false;

            RefreshBindingPreview();

            Shown += async (_, _) =>
            {
                if (_chkCheckUpdates is not null && _chkCheckUpdates.Checked)
                    await UpdateEngine.CheckForUpdatesAsync();
            };
        }

        private void EnsureCustomControls()
        {
            _padVisualizer.BringToFront();
            _padVisualizer.RemapRequested -= PadVisualizer_RemapRequested;
            _padVisualizer.RemapRequested += PadVisualizer_RemapRequested;
            _padVisualizer.StickLayoutSelected -= PadVisualizer_StickLayoutSelected;
            _padVisualizer.StickLayoutSelected += PadVisualizer_StickLayoutSelected;

            bool tuningEnabled = _chkOverrideTuning.Checked;
            _sldHeliSens.Enabled = tuningEnabled;
            _numHeliSens.Enabled = tuningEnabled;
            _sldJetSens.Enabled = tuningEnabled;
            _numJetSens.Enabled = tuningEnabled;
            _sldDeadzone.Enabled = tuningEnabled;
            _numDeadzone.Enabled = tuningEnabled;
        }

        private void LoadAppPreferences()
        {
            try
            {
                if (!File.Exists(AppSettingsFilePath)) return;

                using var doc = JsonDocument.Parse(File.ReadAllBytes(AppSettingsFilePath));
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("CheckUpdatesOnStart", out JsonElement chkProp) &&
                    (chkProp.ValueKind == JsonValueKind.True || chkProp.ValueKind == JsonValueKind.False) &&
                    _chkCheckUpdates is not null)
                {
                    _chkCheckUpdates.Checked = chkProp.GetBoolean();
                }

                if (root.TryGetProperty("LastSelectedPreset", out JsonElement presetProp) &&
                    presetProp.ValueKind == JsonValueKind.String)
                {
                    _lastSelectedPresetTitle = presetProp.GetString()?.Trim() ?? string.Empty;
                }

                if (root.TryGetProperty("NexusApiKey", out JsonElement keyProp) &&
                    keyProp.ValueKind == JsonValueKind.String)
                {
                    _nexusApiKey = keyProp.GetString()?.Trim() ?? string.Empty;
                }
            }
            catch
            {
                // Ignore corrupt settings file
            }
        }

        private void SaveAppPreferences()
        {
            try
            {
                if (_cmbScheme.SelectedItem is FlightPresets.FlightScheme currentScheme)
                    _lastSelectedPresetTitle = currentScheme.Title;

                var payload = new
                {
                    CheckUpdatesOnStart = _chkCheckUpdates?.Checked ?? true,
                    LastSelectedPreset = _lastSelectedPresetTitle,
                    NexusApiKey = _nexusApiKey
                };
                string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(AppSettingsFilePath, json);
            }
            catch
            {
                // Ignore write errors
            }
        }

        private async void ChkCheckUpdates_CheckedChanged(object? sender, EventArgs e)
        {
            if (_suppressPreviewUpdate) return;
            SaveAppPreferences();

            if (_chkCheckUpdates.Checked)
                await UpdateEngine.CheckForUpdatesAsync();
        }





        private void PadVisualizer_StickLayoutSelected(object? sender, FlightPresets.StickLayoutType stickLayout)
        {
            _cmbSticks.SelectedItem = stickLayout;
            ConsoleLog($"[STICKS] Switched Analog Stick Layout to {stickLayout}.");
        }

        private void PadVisualizer_RemapRequested(object? sender, ControllerRemapEventArgs e)
        {
            if (_cmbScheme.SelectedItem is not FlightPresets.FlightScheme scheme)
                return;

            scheme.EnsureBaselineTables();
            string domain = e.Domain.Equals("heli", StringComparison.OrdinalIgnoreCase) ? "helicopter" : e.Domain;
            string vehicleName = domain == "helicopter" ? "Helicopter" : "Jet";

            if (e.ResetToDefault)
            {
                FlightPresets.FlightScheme? defaultSource = null;
                if (scheme.BasePresetType is FlightPresets.FlightPresetType baseType &&
                    FlightPresets.FlightSchemeRegistry.Schemes.TryGetValue(baseType, out var foundBuiltIn))
                {
                    defaultSource = foundBuiltIn.Clone();
                }
                else if (!string.IsNullOrWhiteSpace(scheme.SourceFilePath) && File.Exists(scheme.SourceFilePath))
                {
                    defaultSource = ProfEngine.ImportPresetFromFile(scheme.SourceFilePath);
                }

                if (defaultSource is not null)
                {
                    scheme.RawVariables.Clear();
                    foreach (var (k, v) in defaultSource.RawVariables)
                        scheme.RawVariables[k] = v;
                    scheme.HasCustomHeliBinds = defaultSource.HasCustomHeliBinds;
                    scheme.HasCustomJetBinds = defaultSource.HasCustomJetBinds;
                    SyncUIFromProfile();
                    RefreshBindingPreview();
                    ConsoleLog($"[RESET] Restored '{scheme.Title}' button bindings to preset default.");
                }
                return;
            }

            if (e.CopyToOtherVehicle)
            {
                string targetDomain = domain == "helicopter" ? "jet" : "helicopter";
                foreach (string actionKey in EngineBindings.ActionSlots.Keys)
                {
                    if (actionKey is "Yaw" or "Pitch" or "Roll" or "ThrottleAxis") continue;
                    if (targetDomain == "jet" && actionKey == "SwitchSeat") continue;
                    if (targetDomain == "helicopter" && actionKey == "Afterburner") continue;

                    if (GetCurrentPadBindForAction(scheme.RawVariables, domain, actionKey) is EngineBindings.PadBind srcBind)
                        scheme.BindAction(targetDomain, actionKey, srcBind);
                }

                scheme.HasCustomHeliBinds = true;
                scheme.HasCustomJetBinds = true;
                scheme.RawVariables["GstInput.Scheme5Buttons"] = "4";
                scheme.RawVariables["GstInput.Scheme2Buttons"] = "4";
                scheme.RawVariables["GstInput.OptionHelicopterAssist"] = "0";
                scheme.RawVariables["GstInput.OptionPlaneAssist"] = "0";
                scheme.RawVariables["GstInput.OptionPlaneTerrainAvoidance"] = "0";
                RefreshBindingPreview();
                ConsoleLog($"[SYNC] Copied {vehicleName} button bindings to {(targetDomain == "helicopter" ? "Helicopter" : "Jet")}.");
                return;
            }

            if (!EngineBindings.PadInput.TryGetBindForSlot(e.SlotKey, out var targetPadBind))
                return;

            if (domain == "helicopter")
            {
                scheme.RawVariables["GstInput.Scheme5Buttons"] = "4";
                scheme.RawVariables["GstInput.OptionHelicopterAssist"] = "0";
                scheme.RawVariables["GstInput.OptionPlaneAssist"] = "0";
                scheme.RawVariables["GstInput.OptionPlaneTerrainAvoidance"] = "0";
                scheme.HasCustomHeliBinds = true;
            }
            else
            {
                scheme.RawVariables["GstInput.Scheme2Buttons"] = "4";
                scheme.RawVariables["GstInput.OptionHelicopterAssist"] = "0";
                scheme.RawVariables["GstInput.OptionPlaneAssist"] = "0";
                scheme.RawVariables["GstInput.OptionPlaneTerrainAvoidance"] = "0";
                scheme.HasCustomJetBinds = true;
            }

            if (!string.IsNullOrEmpty(e.SwapTargetSlotKey) &&
                EngineBindings.PadInput.TryGetBindForSlot(e.SwapTargetSlotKey, out var otherPadBind))
            {
                var slot1Actions = GetActionsBoundToPadBind(scheme.RawVariables, domain, targetPadBind);
                var slot2Actions = GetActionsBoundToPadBind(scheme.RawVariables, domain, otherPadBind);

                foreach (string a in slot1Actions)
                    scheme.BindAction(domain, a, otherPadBind);
                foreach (string a in slot2Actions)
                    scheme.BindAction(domain, a, targetPadBind);

                RefreshBindingPreview();
                ConsoleLog($"[SWAP] {vehicleName}: Swapped {_padVisualizer.GetFriendlySlotName(e.SlotKey)} <-> {_padVisualizer.GetFriendlySlotName(e.SwapTargetSlotKey)}.");
                return;
            }

            if (e.Concept == "UNBIND")
            {
                var actionsOnSlot = GetActionsBoundToPadBind(scheme.RawVariables, domain, targetPadBind);
                foreach (string a in actionsOnSlot)
                    scheme.BindAction(domain, a, EngineBindings.PadInput.Unbound);

                RefreshBindingPreview();
                ConsoleLog($"[UNBIND] {vehicleName}: Cleared {_padVisualizer.GetFriendlySlotName(e.SlotKey)}.");
                return;
            }

            if (!string.IsNullOrEmpty(e.Concept))
            {
                var existingActionsOnTarget = GetActionsBoundToPadBind(scheme.RawVariables, domain, targetPadBind);
                var previousPadBind = GetCurrentPadBindForAction(scheme.RawVariables, domain, e.Concept);

                if (e.StackAction)
                {
                    if (existingActionsOnTarget.Contains(e.Concept))
                        scheme.BindAction(domain, e.Concept, EngineBindings.PadInput.Unbound);
                    else
                        scheme.BindAction(domain, e.Concept, targetPadBind);
                }
                else
                {
                    foreach (string oldAction in existingActionsOnTarget)
                    {
                        if (oldAction.Equals(e.Concept, StringComparison.OrdinalIgnoreCase)) continue;

                        if (previousPadBind is EngineBindings.PadBind prevBind && !prevBind.IsUnbound && !prevBind.IsStickAxis)
                            scheme.BindAction(domain, oldAction, prevBind);
                        else
                            scheme.BindAction(domain, oldAction, EngineBindings.PadInput.Unbound);
                    }

                    scheme.BindAction(domain, e.Concept, targetPadBind);
                }

                RefreshBindingPreview();
                string actionName = FDControllerVisualizer.FormatActionName(e.Concept, domain);
                ConsoleLog($"[REMAP] {vehicleName}: Bound {_padVisualizer.GetFriendlySlotName(e.SlotKey)} -> {actionName} (GstKeyBinding.version 2000007).");
            }
        }

        private static List<string> GetActionsBoundToPadBind(IReadOnlyDictionary<string, string> vars, string domain, EngineBindings.PadBind target)
        {
            var matches = new List<string>();
            foreach (var (actionKey, slots) in EngineBindings.ActionSlots)
            {
                if (actionKey is "Yaw" or "Pitch" or "Roll" or "ThrottleAxis") continue;
                var primary = slots[0];
                string baseKey = $"GstKeyBinding.{domain}.{primary.Concept}.{primary.SlotIndex}";
                if (vars.TryGetValue($"{baseKey}.axis", out string? sAxis) &&
                    vars.TryGetValue($"{baseKey}.button", out string? sBtn) &&
                    int.TryParse(sAxis, out int axis) &&
                    int.TryParse(sBtn, out int btn) &&
                    axis == target.Axis && btn == target.Button)
                {
                    matches.Add(actionKey);
                }
            }
            return matches;
        }

        private static EngineBindings.PadBind? GetCurrentPadBindForAction(IReadOnlyDictionary<string, string> vars, string domain, string actionKey)
        {
            if (!EngineBindings.ActionSlots.TryGetValue(actionKey, out var slots))
                return null;

            var primary = slots[0];
            string baseKey = $"GstKeyBinding.{domain}.{primary.Concept}.{primary.SlotIndex}";
            if (vars.TryGetValue($"{baseKey}.axis", out string? sAxis) &&
                vars.TryGetValue($"{baseKey}.button", out string? sBtn) &&
                int.TryParse(sAxis, out int axis) &&
                int.TryParse(sBtn, out int btn))
            {
                int negate = vars.TryGetValue($"{baseKey}.negate", out string? sNeg) && int.TryParse(sNeg, out int n) ? n : 0;
                return new EngineBindings.PadBind(axis, btn, negate);
            }
            return null;
        }

        private void ReloadPresetDropdown(string? selectTitle = null)
        {
            _cmbScheme.Items.Clear();
            FlightPresets.FlightSchemeRegistry.CustomSchemes.Clear();

            foreach (var kvp in FlightPresets.FlightSchemeRegistry.Schemes)
            {
                var builtIn = kvp.Value.Clone();
                builtIn.IsCustom = false;
                _cmbScheme.Items.Add(builtIn);
            }

            if (Directory.Exists(CustomPresetsDirectory))
            {
                var files = Directory.EnumerateFiles(CustomPresetsDirectory)
                    .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".ini", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase);

                foreach (string file in files)
                {
                    try
                    {
                        var custom = ProfEngine.ImportPresetFromFile(file);
                        custom.IsCustom = true;
                        custom.SourceFilePath = file;
                        FlightPresets.FlightSchemeRegistry.CustomSchemes.Add(custom);
                        _cmbScheme.Items.Add(custom);
                    }
                    catch
                    {
                        // Ignore invalid preset files
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(selectTitle))
            {
                for (int i = 0; i < _cmbScheme.Items.Count; i++)
                {
                    if (_cmbScheme.Items[i] is FlightPresets.FlightScheme s &&
                        string.Equals(s.Title, selectTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        _cmbScheme.SelectedIndex = i;
                        return;
                    }
                }
            }

            if (_cmbScheme.Items.Count > 0 && _cmbScheme.SelectedIndex < 0)
                _cmbScheme.SelectedIndex = 0;
        }

        private void BtnClose_Click(object? sender, EventArgs e) => Close();

        private void BtnMin_Click(object? sender, EventArgs e) =>
            WindowState = FormWindowState.Minimized;

        private void BtnBrowse_Click(object? sender, EventArgs e) => BrowseProfile();

        private void CmbProfilePath_SelectedIndexChanged(object? sender, EventArgs e) =>
            SyncFromProfile();

        private void CmbScheme_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suppressPreviewUpdate) return;
            SyncUIFromProfile();
            RefreshBindingPreview();
            SaveAppPreferences();
        }

        private void CmbSticks_SelectedIndexChanged(object? sender, EventArgs e) =>
            RefreshBindingPreview();

        private void ChkInvertPitch_CheckedChanged(object? sender, EventArgs e) =>
            RefreshBindingPreview();

        private void ChkOverrideTuning_CheckedChanged(object? sender, EventArgs e)
        {
            bool enabled = _chkOverrideTuning.Checked;
            _sldHeliSens.Enabled = enabled;
            _numHeliSens.Enabled = enabled;
            _sldJetSens.Enabled = enabled;
            _numJetSens.Enabled = enabled;
            _sldDeadzone.Enabled = enabled;
            _numDeadzone.Enabled = enabled;
            RefreshBindingPreview();
        }

        private void SldHeliSens_ValueChanged(object? sender, EventArgs e)
        {
            if (_numHeliSens.Value != _sldHeliSens.Value)
                _numHeliSens.Value = _sldHeliSens.Value;
        }

        private void NumHeliSens_ValueChanged(object? sender, EventArgs e)
        {
            if (_sldHeliSens.Value != _numHeliSens.Value)
                _sldHeliSens.Value = _numHeliSens.Value;
            RefreshBindingPreview();
        }

        private void SldJetSens_ValueChanged(object? sender, EventArgs e)
        {
            if (_numJetSens.Value != _sldJetSens.Value)
                _numJetSens.Value = _sldJetSens.Value;
        }

        private void NumJetSens_ValueChanged(object? sender, EventArgs e)
        {
            if (_sldJetSens.Value != _numJetSens.Value)
                _sldJetSens.Value = _numJetSens.Value;
            RefreshBindingPreview();
        }

        private void SldDeadzone_ValueChanged(object? sender, EventArgs e)
        {
            if (_numDeadzone.Value != _sldDeadzone.Value)
                _numDeadzone.Value = _sldDeadzone.Value;
        }

        private void NumDeadzone_ValueChanged(object? sender, EventArgs e)
        {
            if (_sldDeadzone.Value != _numDeadzone.Value)
                _sldDeadzone.Value = _numDeadzone.Value;
            RefreshBindingPreview();
        }

        private void TuningNumeric_ValueChanged(object? sender, EventArgs e) =>
            RefreshBindingPreview();

        private void BtnImportPreset_Click(object? sender, EventArgs e) => ImportCustomPreset();

        private void BtnExportPreset_Click(object? sender, EventArgs e) => ExportCurrentPreset();

        private void BtnDeletePreset_Click(object? sender, EventArgs e) => DeleteSelectedCustomPreset();

        private void BtnApply_Click(object? sender, EventArgs e) => ApplySelectedScheme();

        private void BtnRestoreLatest_Click(object? sender, EventArgs e) => RestoreLatestBackupFile();

        private void BtnSelectBackup_Click(object? sender, EventArgs e) => ShowBackupSelectorMenu();

        private void BtnProfile_Click(object? sender, EventArgs e) => OpenProfileFolder();

        private void LnkWebsite_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) =>
            Process.Start(new ProcessStartInfo("https://voidtechstudios.us/") { UseShellExecute = true });

        private void LnkDonate_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) =>
            Process.Start(new ProcessStartInfo("https://buymeacoffee.com/rustforgedev") { UseShellExecute = true });

        private void BtnToggleVisual_Click(object? sender, EventArgs e)
        {
            if (_padVisualizer is null) return;

            bool showVisual = !_padVisualizer.Visible;
            _padVisualizer.Visible = showVisual;
            _lvBindings.Visible = !showVisual;

            if (showVisual)
            {
                _btnToggleVisual.Text = "Hide Visual Map (Table)";
                _btnToggleVisual.AccentColor = Color.FromArgb(255, 102, 36);
                _padVisualizer.BringToFront();
            }
            else
            {
                _btnToggleVisual.Text = "Show Visual Controller";
                _btnToggleVisual.AccentColor = Color.FromArgb(0, 180, 240);
                _lvBindings.BringToFront();
            }
        }

        private void LoadDetectedProfiles()
        {
            var paths = ProfEngine.FindProfilePaths();
            _cmbProfilePath.Items.Clear();
            foreach (string p in paths)
                _cmbProfilePath.Items.Add(p);

            if (_cmbProfilePath.Items.Count > 0)
            {
                _cmbProfilePath.SelectedIndex = 0;
                SyncFromProfile();
                ConsoleLog($"Auto-detected {paths.Count} PROFSAVEbf6mp_profile file(s).");
            }
            else
            {
                ConsoleLog("No PROFSAVEbf6mp_profile auto-detected. Click 'Browse...' to locate your Battlefield 6 settings folder.");
            }
        }

        private void SyncFromProfile()
        {
            string path = _cmbProfilePath.Text.Trim();
            if (!File.Exists(path)) return;

            bool prevSuppress = _suppressPreviewUpdate;
            _suppressPreviewUpdate = true;
            try
            {
                var snapshot = ProfEngine.ReadCurrentTuning(path);
                _chkInvertPitch.Checked = snapshot.InvertPitch;
                _cmbSticks.SelectedIndex = Math.Clamp((int)snapshot.StickLayout, 0, 3);

                if (_numHeliSens is not null) _numHeliSens.Value = snapshot.HeliSensitivity;
                if (_sldHeliSens is not null) _sldHeliSens.Value = snapshot.HeliSensitivity;

                if (_numJetSens is not null) _numJetSens.Value = snapshot.JetSensitivity;
                if (_sldJetSens is not null) _sldJetSens.Value = snapshot.JetSensitivity;

                if (_numDeadzone is not null) _numDeadzone.Value = snapshot.CenterDeadzone;
                if (_sldDeadzone is not null) _sldDeadzone.Value = snapshot.CenterDeadzone;

                _chkReadOnly.Checked = new FileInfo(path).IsReadOnly;

                if (_cmbScheme.SelectedItem is FlightPresets.FlightScheme activeScheme &&
                    ProfEngine.SyncLiveUserProfile(path, activeScheme))
                {
                    ConsoleLog("[PROFILE] Loaded your saved custom controller bindings from PROFSAVEbf6mp_profile.");
                }
            }
            catch
            {
                // Ignore locked files
            }
            finally
            {
                _suppressPreviewUpdate = prevSuppress;
            }

            RefreshBindingPreview();
        }

        private void SyncUIFromProfile()
        {
            if (_cmbScheme.SelectedItem is not FlightPresets.FlightScheme selected)
                return;

            bool prevSuppress = _suppressPreviewUpdate;
            _suppressPreviewUpdate = true;
            try
            {
                if (_btnDeletePreset is not null)
                    _btnDeletePreset.Enabled = selected.IsCustom;

                if (selected.RawVariables.TryGetValue("GstInput.Scheme5FlipY", out string? flip))
                    _chkInvertPitch.Checked = flip.Trim() == "1";

                if (selected.RawVariables.TryGetValue("GstInput.Scheme5Sticks", out string? sticks) &&
                    int.TryParse(sticks.Trim(), out int sIdx))
                {
                    _cmbSticks.SelectedIndex = Math.Clamp(sIdx, 0, 3);
                }

                if (selected.RawVariables.TryGetValue("GstInput.VehicleSensitivityHelicopterPilot", out string? hSens) &&
                    decimal.TryParse(hSens.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal pHeli))
                {
                    decimal clampedHeli = Math.Clamp(pHeli, 0.10m, 3.00m);
                    _numHeliSens.Value = clampedHeli;
                    if (_sldHeliSens is not null) _sldHeliSens.Value = clampedHeli;
                }

                if (selected.RawVariables.TryGetValue("GstInput.VehicleSensitivityPlanePilot", out string? jSens) &&
                    decimal.TryParse(jSens.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal pJet))
                {
                    decimal clampedJet = Math.Clamp(pJet, 0.10m, 3.00m);
                    _numJetSens.Value = clampedJet;
                    if (_sldJetSens is not null) _sldJetSens.Value = clampedJet;
                }

                if (selected.RawVariables.TryGetValue("GstInput.GamepadLeftStickDeadzoneCenter", out string? dz) &&
                    decimal.TryParse(dz.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal pDz))
                {
                    decimal clampedDz = Math.Clamp(pDz, 0.00m, 0.50m);
                    _numDeadzone.Value = clampedDz;
                    if (_sldDeadzone is not null) _sldDeadzone.Value = clampedDz;
                }
            }
            finally
            {
                _suppressPreviewUpdate = prevSuppress;
            }
        }

        private FlightPresets.FlightScheme BuildActiveConfiguredScheme()
        {
            var templateScheme = _cmbScheme.SelectedItem is FlightPresets.FlightScheme fs
                ? fs
                : FlightPresets.FlightSchemeRegistry.Schemes[FlightPresets.FlightPresetType.Bf4Veteran];

            var stickType = _cmbSticks.SelectedItem is FlightPresets.StickLayoutType st
                ? st
                : FlightPresets.StickLayoutType.Default;

            return FlightPresets.FlightSchemeRegistry.BuildConfiguredScheme(
                templateScheme,
                stickType,
                _chkInvertPitch.Checked,
                _chkOverrideTuning.Checked,
                _numHeliSens.Value,
                _numJetSens.Value,
                _numDeadzone.Value);
        }

        private void RefreshBindingPreview()
        {
            if (_suppressPreviewUpdate || _cmbScheme.SelectedItem is null) return;

            var scheme = BuildActiveConfiguredScheme();
            _grpPreset.SubTitle = scheme.Subtitle;

            _padVisualizer?.LoadScheme(scheme.RawVariables);

            _lvBindings.BeginUpdate();
            _lvBindings.Items.Clear();

            foreach (var (key, val) in scheme.RawVariables)
            {
                string category = key.StartsWith("GstKeyBinding.helicopter.", StringComparison.Ordinal) ? "Helicopter Bind"
                                : key.StartsWith("GstKeyBinding.jet.", StringComparison.Ordinal) ? "Jet Bind"
                                : key.Equals("GstKeyBinding.version", StringComparison.Ordinal) ? "KeyBind Schema Version"
                                : key.Contains("Sensitivity", StringComparison.Ordinal) || key.Contains("Deadzone", StringComparison.Ordinal)
                                    ? "Sensitivity / Deadzone"
                                : key.Contains("Scheme1", StringComparison.Ordinal) || key.Contains("Scheme3", StringComparison.Ordinal) || key.Contains("Scheme4", StringComparison.Ordinal)
                                    ? "Ground / Tank Safeguard"
                                : "Flight Physics / Curve";

                var item = new ListViewItem(new string[] { key, val, category });
                _lvBindings.Items.Add(item);
            }
            _lvBindings.EndUpdate();
        }

        private void ApplySelectedScheme()
        {
            string path = _cmbProfilePath.Text.Trim();
            if (_cmbScheme.SelectedItem is null) return;

            var scheme = BuildActiveConfiguredScheme();
            var result = ProfEngine.ApplyScheme(path, scheme, _chkReadOnly.Checked);

            if (!result.Success)
            {
                ConsoleLog($"[ERROR] {result.Message}");
                MessageBox.Show(result.Message, "Patch Aborted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveAppPreferences();

            ConsoleLog($"[SUCCESS] {result.Message}");
            ConsoleLog($"  -> Stick Layout: {_cmbSticks.SelectedItem} | Invert Pitch: {(_chkInvertPitch.Checked ? "ON" : "OFF")} | Custom Binds: {(scheme.HasCustomButtonBinds ? "YES (v2000007)" : "NO (Preset)")}");
            ConsoleLog($"  -> Updated {result.UpdatedCount} existing keys | Injected {result.InjectedCount} lines.");
            ConsoleLog($"  -> Backup created: {Path.GetFileName(result.BackupPath)}");
        }

        private void ImportCustomPreset()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Import FlightDeck Preset(s) (.json or .ini)",
                Filter = "FlightDeck Presets (*.json;*.ini)|*.json;*.ini|JSON Preset (*.json)|*.json|INI Preset (*.ini)|*.ini",
                Multiselect = true
            };

            if (ofd.ShowDialog() != DialogResult.OK || ofd.FileNames.Length == 0) return;

            Directory.CreateDirectory(CustomPresetsDirectory);
            string? lastImportedTitle = null;

            foreach (string srcFile in ofd.FileNames)
            {
                try
                {
                    var imported = ProfEngine.ImportPresetFromFile(srcFile);
                    string destPath = Path.Combine(CustomPresetsDirectory, Path.GetFileName(srcFile));

                    if (!string.Equals(Path.GetFullPath(srcFile), Path.GetFullPath(destPath), StringComparison.OrdinalIgnoreCase))
                        File.Copy(srcFile, destPath, overwrite: true);

                    lastImportedTitle = imported.Title;
                    ConsoleLog($"[IMPORTED] Added custom preset '{imported.Title}' ({Path.GetFileName(srcFile)}).");
                }
                catch (Exception ex)
                {
                    ConsoleLog($"[ERROR] Failed to import {Path.GetFileName(srcFile)}: {ex.Message}");
                    ExceptionHandler.LogError(ex);
                    MessageBox.Show($"Could not import '{Path.GetFileName(srcFile)}':\n{ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _suppressPreviewUpdate = true;
            ReloadPresetDropdown(lastImportedTitle);
            _suppressPreviewUpdate = false;

            SyncUIFromProfile();
            RefreshBindingPreview();
            SaveAppPreferences();
        }

        private void ExportCurrentPreset()
        {
            var scheme = BuildActiveConfiguredScheme();

            string defaultTitle = scheme.IsCustom
                ? scheme.Title
                : $"{scheme.Title} ({_cmbSticks.SelectedItem})";

            string defaultDesc = $"Sticks: {_cmbSticks.SelectedItem} | Invert Pitch: {(_chkInvertPitch.Checked ? "ON" : "OFF")} | Heli Sens: {_numHeliSens.Value:0.00} | Jet Sens: {_numJetSens.Value:0.00} | DZ: {_numDeadzone.Value:0.00}";

            if (!PromptExportMetadata(ref defaultTitle, ref defaultDesc))
                return;

            scheme.Title = defaultTitle;
            scheme.Subtitle = defaultDesc;
            scheme.IsCustom = true;

            Directory.CreateDirectory(CustomPresetsDirectory);

            string safeFileName = string.Join("_", scheme.Title.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
            if (string.IsNullOrWhiteSpace(safeFileName))
                safeFileName = "CustomFlightPreset";

            using var sfd = new SaveFileDialog
            {
                Title = "Export FlightDeck Preset (.json or .ini)",
                Filter = "JSON Preset (*.json)|*.json|INI Preset (*.ini)|*.ini",
                InitialDirectory = CustomPresetsDirectory,
                FileName = $"{safeFileName}.json"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                ProfEngine.ExportPresetToFile(scheme, sfd.FileName);

                string localCopy = Path.Combine(CustomPresetsDirectory, Path.GetFileName(sfd.FileName));
                if (!string.Equals(Path.GetFullPath(sfd.FileName), Path.GetFullPath(localCopy), StringComparison.OrdinalIgnoreCase))
                    File.Copy(sfd.FileName, localCopy, overwrite: true);

                _suppressPreviewUpdate = true;
                ReloadPresetDropdown(scheme.Title);
                _suppressPreviewUpdate = false;

                SyncUIFromProfile();
                RefreshBindingPreview();
                SaveAppPreferences();
                ConsoleLog($"[EXPORTED] Saved custom preset '{scheme.Title}' to {Path.GetFileName(sfd.FileName)}.");
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                ConsoleLog($"[ERROR] Failed to export preset: {ex.Message}");
                MessageBox.Show($"Could not export preset:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSelectedCustomPreset()
        {
            if (_cmbScheme.SelectedItem is not FlightPresets.FlightScheme selected || !selected.IsCustom)
                return;

            var confirm = MessageBox.Show(
                $"Delete custom preset '{selected.Title}' from Flight Deck?",
                "Delete Custom Preset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                if (!string.IsNullOrWhiteSpace(selected.SourceFilePath) && File.Exists(selected.SourceFilePath))
                    File.Delete(selected.SourceFilePath);

                ConsoleLog($"[DELETED] Removed custom preset '{selected.Title}'.");
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                ConsoleLog($"[ERROR] Could not delete preset file: {ex.Message}");
            }

            _suppressPreviewUpdate = true;
            ReloadPresetDropdown();
            _suppressPreviewUpdate = false;

            SyncUIFromProfile();
            RefreshBindingPreview();
            SaveAppPreferences();
        }

        private bool PromptExportMetadata(ref string presetTitle, ref string presetSubtitle)
        {
            using var dlg = new Form
            {
                Text = "Export Custom Flight Preset",
                Size = new Size(460, 235),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(20, 24, 31),
                ForeColor = Color.FromArgb(240, 244, 248),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            var lblName = new Label
            {
                Text = "Preset Name (shown in dropdown):",
                Location = new Point(16, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 180, 240)
            };

            var txtName = new TextBox
            {
                Text = presetTitle,
                Location = new Point(16, 38),
                Size = new Size(410, 26),
                BackColor = Color.FromArgb(14, 17, 22),
                ForeColor = Color.FromArgb(240, 244, 248),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblDesc = new Label
            {
                Text = "Preset Description / Notes:",
                Location = new Point(16, 76),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 160, 176)
            };

            var txtDesc = new TextBox
            {
                Text = presetSubtitle,
                Location = new Point(16, 98),
                Size = new Size(410, 26),
                BackColor = Color.FromArgb(14, 17, 22),
                ForeColor = Color.FromArgb(240, 244, 248),
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnSave = new Button
            {
                Text = "Continue to Save...",
                DialogResult = DialogResult.OK,
                Location = new Point(206, 142),
                Size = new Size(130, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 145, 205),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(344, 142),
                Size = new Size(82, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(34, 41, 52),
                ForeColor = Color.FromArgb(240, 244, 248)
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            dlg.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDesc, btnSave, btnCancel });
            dlg.AcceptButton = btnSave;
            dlg.CancelButton = btnCancel;

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return false;

            if (!string.IsNullOrWhiteSpace(txtName.Text))
                presetTitle = txtName.Text.Trim();

            if (!string.IsNullOrWhiteSpace(txtDesc.Text))
                presetSubtitle = txtDesc.Text.Trim();

            return true;
        }

        private void RestoreSpecificBackup(string profilePath, string backupPath)
        {
            var result = ProfEngine.RestoreFromSpecificBackup(profilePath, backupPath);
            if (!result.Success)
            {
                ConsoleLog($"[ERROR] {result.Message}");
                MessageBox.Show(result.Message, "Restore Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SyncFromProfile();
            ConsoleLog($"[RESTORED] {result.Message}");
        }

        private void BrowseAndRestoreBackup(string profilePath)
        {
            string? dir = Path.GetDirectoryName(profilePath);
            using var ofd = new OpenFileDialog
            {
                Title = "Select Backup File to Restore",
                Filter = "BF6 Profile Backups (*.bak_*)|*.bak_*|All Files (*.*)|*.*",
                InitialDirectory = (!string.IsNullOrEmpty(dir) && Directory.Exists(dir)) ? dir : string.Empty
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                RestoreSpecificBackup(profilePath, ofd.FileName);
            }
        }

        private void RestoreLatestBackupFile()
        {
            string path = _cmbProfilePath.Text.Trim();
            var result = ProfEngine.RestoreLatestBackup(path);

            if (!result.Success)
            {
                ConsoleLog($"[RESTORE] {result.Message}");
                MessageBox.Show(result.Message, "Restore Latest Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SyncFromProfile();
            ConsoleLog($"[RESTORED LATEST] {result.Message}");
        }

        private void ShowBackupSelectorMenu()
        {
            string profilePath = _cmbProfilePath.Text.Trim();
            var backups = ProfEngine.GetBackupFiles(profilePath);

            _cmsBackups.Items.Clear();

            if (backups.Count == 0)
            {
                var emptyItem = new ToolStripMenuItem("No .bak_ files found in folder") { Enabled = false };
                _cmsBackups.Items.Add(emptyItem);
            }
            else
            {
                for (int i = 0; i < backups.Count; i++)
                {
                    string backupPath = backups[i];
                    string fileName = Path.GetFileName(backupPath);
                    string displayLabel = FormatBackupDisplayName(fileName, i == 0, i == backups.Count - 1);

                    var item = new ToolStripMenuItem(displayLabel)
                    {
                        ForeColor = Color.FromArgb(240, 244, 248)
                    };
                    item.Click += (_, _) => RestoreSpecificBackup(profilePath, backupPath);
                    _cmsBackups.Items.Add(item);
                }
            }

            _cmsBackups.Items.Add(new ToolStripSeparator());

            var browseBackupItem = new ToolStripMenuItem("Browse for backup file...")
            {
                ForeColor = Color.FromArgb(0, 180, 240)
            };
            browseBackupItem.Click += (_, _) => BrowseAndRestoreBackup(profilePath);
            _cmsBackups.Items.Add(browseBackupItem);

            _cmsBackups.Show(_btnSelectBackup, new Point(0, _btnSelectBackup.Height + 2));
        }

        private static string FormatBackupDisplayName(string fileName, bool isNewest, bool isOldest)
        {
            int bakIdx = fileName.IndexOf(".bak_", StringComparison.OrdinalIgnoreCase);
            if (bakIdx >= 0 && fileName.Length >= bakIdx + 20)
            {
                string stamp = fileName[(bakIdx + 5)..];
                if (DateTime.TryParseExact(stamp, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    string tag = isNewest ? " (Latest)" : isOldest ? " (Original)" : string.Empty;
                    return $"{dt:yyyy-MM-dd  hh:mm:ss tt}  |  {fileName}{tag}";
                }
            }
            return fileName;
        }

        private void OpenProfileFolder()
        {
            string path = _cmbProfilePath.Text.Trim();
            string? dir = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"\"{dir}\"") { UseShellExecute = true });
            }
            else
            {
                ConsoleLog("[ERROR] Settings folder does not exist yet.");
            }
        }

        private void BrowseProfile()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select Battlefield 6 Profile (PROFSAVEbf6mp_profile)",
                Filter = "BF6 Profile Files (PROFSAVE*_profile*)|PROFSAVE*_profile*;PROFSAVE*|All Files (*.*)|*.*",
                CheckFileExists = true
            };

            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string bf6Dir = Path.Combine(docs, "Battlefield 6");
            if (Directory.Exists(bf6Dir))
                ofd.InitialDirectory = bf6Dir;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!_cmbProfilePath.Items.Contains(ofd.FileName))
                    _cmbProfilePath.Items.Add(ofd.FileName);
                _cmbProfilePath.SelectedItem = ofd.FileName;
            }
        }

        private void ConsoleLog(string text)
        {
            _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            _txtLog.ScrollToBottom();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
           _themeShell.Text = $"FLIGHT DECK v.{UpdateEngine.currentVersion}";
        }
    }
}