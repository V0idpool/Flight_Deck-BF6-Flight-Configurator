#pragma warning disable WFO1000

using FlightDeck_BF6_Flight_Configurator.CORE;
using FlightDeck_BF6_Flight_Configurator.GUI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace FlightDeck_BF6_Flight_Configurator
{
    public partial class MainForm : Form
    {
        private bool _suppressPreviewUpdate;

        public MainForm()
        {
            InitializeComponent();
            EnsureCustomControls();
            _btnBrowse.SendToBack();
            _cmbProfilePath.BringToFront();
            _cmsBackups.Renderer = new ToolStripProfessionalRenderer(new ColorTable());
            _cmsBackups.ForeColor = Color.FromArgb(240, 244, 248);
            _cmsBackups.Font = new Font("Segoe UI", 9.25f, FontStyle.Regular);

            _suppressPreviewUpdate = true;

            foreach (var preset in Enum.GetValues<FlightPresets.FlightPresetType>())
            {
                if (preset != FlightPresets.FlightPresetType.CustomImported)
                    _cmbScheme.Items.Add(preset);
            }

            foreach (var stickLayout in Enum.GetValues<FlightPresets.StickLayoutType>())
                _cmbSticks.Items.Add(stickLayout);

            if (_cmbSticks.Items.Count > 0)
                _cmbSticks.SelectedIndex = 0;

            LoadDetectedProfiles();

            _suppressPreviewUpdate = false;

            if (_cmbScheme.Items.Count > 0)
                _cmbScheme.SelectedIndex = 0;
        }

        private void EnsureCustomControls()
        {
            _padVisualizer ??= new FDControllerVisualizer();
            _padVisualizer.Dock = DockStyle.Fill;
            _padVisualizer.Visible = false;
            if (!_pnlInspectorBody.Controls.Contains(_padVisualizer))
                _pnlInspectorBody.Controls.Add(_padVisualizer);

            if (_sldHeliSens is null)
            {
                _sldHeliSens = new FDSlider
                {
                    Name = "_sldHeliSens",
                    AccentColor = Color.FromArgb(0, 180, 240),
                    Minimum = 0.10m,
                    Maximum = 3.00m,
                    Value = 1.00m,
                    Location = new Point(206, 40),
                    Size = new Size(92, 23)
                };
                _sldHeliSens.ValueChanged += SldHeliSens_ValueChanged;
            }
            if (!_pnlPresetContent.Controls.Contains(_sldHeliSens))
                _pnlPresetContent.Controls.Add(_sldHeliSens);

            if (_sldJetSens is null)
            {
                _sldJetSens = new FDSlider
                {
                    Name = "_sldJetSens",
                    AccentColor = Color.FromArgb(0, 180, 240),
                    Minimum = 0.10m,
                    Maximum = 3.00m,
                    Value = 1.00m,
                    Location = new Point(402, 40),
                    Size = new Size(92, 23)
                };
                _sldJetSens.ValueChanged += SldJetSens_ValueChanged;
            }
            if (!_pnlPresetContent.Controls.Contains(_sldJetSens))
                _pnlPresetContent.Controls.Add(_sldJetSens);

            if (_sldDeadzone is null)
            {
                _sldDeadzone = new FDSlider
                {
                    Name = "_sldDeadzone",
                    AccentColor = Color.FromArgb(255, 102, 36),
                    Minimum = 0.00m,
                    Maximum = 0.50m,
                    Value = 0.06m,
                    Location = new Point(636, 40),
                    Size = new Size(92, 23)
                };
                _sldDeadzone.ValueChanged += SldDeadzone_ValueChanged;
            }
            if (!_pnlPresetContent.Controls.Contains(_sldDeadzone))
                _pnlPresetContent.Controls.Add(_sldDeadzone);

            _chkOverrideTuning.Location = new Point(0, 40);
            _chkOverrideTuning.Size = new Size(165, 24);
            _chkOverrideTuning.Text = "Tune Sens & Deadzone:";

            _lblHeliSens.Location = new Point(170, 44);
            _lblHeliSens.Text = "Heli:";
            _sldHeliSens.Location = new Point(206, 40);
            _numHeliSens.Location = new Point(302, 40);
            _numHeliSens.Size = new Size(58, 23);

            _lblJetSens.Location = new Point(372, 44);
            _lblJetSens.Text = "Jet:";
            _sldJetSens.Location = new Point(402, 40);
            _numJetSens.Location = new Point(498, 40);
            _numJetSens.Size = new Size(58, 23);

            _lblDeadzone.Location = new Point(568, 44);
            _lblDeadzone.Text = "Deadzone:";
            _sldDeadzone.Location = new Point(636, 40);
            _numDeadzone.Location = new Point(732, 40);
            _numDeadzone.Size = new Size(58, 23);

            bool tuningEnabled = _chkOverrideTuning.Checked;
            _sldHeliSens.Enabled = tuningEnabled;
            _numHeliSens.Enabled = tuningEnabled;
            _sldJetSens.Enabled = tuningEnabled;
            _numJetSens.Enabled = tuningEnabled;
            _sldDeadzone.Enabled = tuningEnabled;
            _numDeadzone.Enabled = tuningEnabled;
        }

        private void BtnClose_Click(object? sender, EventArgs e) => Close();

        private void BtnMin_Click(object? sender, EventArgs e) =>
            WindowState = FormWindowState.Minimized;

        private void BtnBrowse_Click(object? sender, EventArgs e) => BrowseProfile();

        private void CmbProfilePath_SelectedIndexChanged(object? sender, EventArgs e) =>
            SyncTuningFromActiveProfile();

        private void CmbScheme_SelectedIndexChanged(object? sender, EventArgs e) =>
            RefreshBindingPreview();

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
                SyncTuningFromActiveProfile();
                Log($"Auto-detected {paths.Count} PROFSAVEbf6mp_profile file(s).");
            }
            else
            {
                Log("No PROFSAVEbf6mp_profile auto-detected. Click 'Browse...' to locate your Battlefield 6 settings folder.");
            }
        }

        private void SyncTuningFromActiveProfile()
        {
            string path = _cmbProfilePath.Text.Trim();
            if (!File.Exists(path)) return;

            bool prevSuppress = _suppressPreviewUpdate;
            _suppressPreviewUpdate = true;
            try
            {
                var snapshot = ProfEngine.ReadCurrentTuning(path);
                if (_numHeliSens is not null) _numHeliSens.Value = snapshot.HeliSensitivity;
                if (_sldHeliSens is not null) _sldHeliSens.Value = snapshot.HeliSensitivity;

                if (_numJetSens is not null) _numJetSens.Value = snapshot.JetSensitivity;
                if (_sldJetSens is not null) _sldJetSens.Value = snapshot.JetSensitivity;

                if (_numDeadzone is not null) _numDeadzone.Value = snapshot.CenterDeadzone;
                if (_sldDeadzone is not null) _sldDeadzone.Value = snapshot.CenterDeadzone;
            }
            catch
            {
                // Ignore locked files, Do nothing.
            }
            finally
            {
                _suppressPreviewUpdate = prevSuppress;
            }
        }

        private FlightPresets.FlightScheme BuildActiveConfiguredScheme()
        {
            var presetType = _cmbScheme.SelectedItem is FlightPresets.FlightPresetType pt
                ? pt
                : FlightPresets.FlightPresetType.Bf4Veteran;

            var stickType = _cmbSticks.SelectedItem is FlightPresets.StickLayoutType st
                ? st
                : FlightPresets.StickLayoutType.Default;

            return FlightPresets.FlightSchemeRegistry.BuildConfiguredScheme(
                presetType,
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
                string category = key.StartsWith("GstKeyBinding.heli.", StringComparison.Ordinal) ? "Helicopter Bind"
                                : key.StartsWith("GstKeyBinding.jet.", StringComparison.Ordinal) ? "Jet Bind"
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
                Log($"[ERROR] {result.Message}");
                MessageBox.Show(result.Message, "Patch Aborted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Log($"[SUCCESS] {result.Message}");
            Log($"  -> Stick Layout: {_cmbSticks.SelectedItem} | Invert Pitch: {(_chkInvertPitch.Checked ? "ON" : "OFF")}");
            Log($"  -> Updated {result.UpdatedCount} existing keys | Injected {result.InjectedCount} GstKeyBinding lines.");
            Log($"  -> Backup created: {Path.GetFileName(result.BackupPath)}");
        }

        private void ImportCustomPreset()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Import FlightDeck Preset (.json or .ini)",
                Filter = "FlightDeck Presets (*.json;*.ini)|*.json;*.ini|JSON Preset (*.json)|*.json|INI Preset (*.ini)|*.ini"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var imported = ProfEngine.ImportPresetFromFile(ofd.FileName);
                FlightPresets.FlightSchemeRegistry.CustomLoadedScheme = imported;

                if (!_cmbScheme.Items.Contains(FlightPresets.FlightPresetType.CustomImported))
                    _cmbScheme.Items.Add(FlightPresets.FlightPresetType.CustomImported);

                _cmbScheme.SelectedItem = FlightPresets.FlightPresetType.CustomImported;
                RefreshBindingPreview();
                Log($"[IMPORTED] Loaded preset '{imported.Title}' from {Path.GetFileName(ofd.FileName)}.");
            }
            catch (Exception ex)
            {
                Log($"[ERROR] Failed to import preset: {ex.Message}");
                MessageBox.Show($"Could not import preset:\n{ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportCurrentPreset()
        {
            var scheme = BuildActiveConfiguredScheme();
            using var sfd = new SaveFileDialog
            {
                Title = "Export FlightDeck Preset (.json or .ini)",
                Filter = "JSON Preset (*.json)|*.json|INI Preset (*.ini)|*.ini",
                FileName = $"FlightDeck_{_cmbScheme.SelectedItem}_{_cmbSticks.SelectedItem}.json"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                ProfEngine.ExportPresetToFile(scheme, sfd.FileName);
                Log($"[EXPORTED] Saved preset to {Path.GetFileName(sfd.FileName)}.");
            }
            catch (Exception ex)
            {
                Log($"[ERROR] Failed to export preset: {ex.Message}");
                MessageBox.Show($"Could not export preset:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestoreSpecificBackup(string profilePath, string backupPath)
        {
            var result = ProfEngine.RestoreFromSpecificBackup(profilePath, backupPath);
            if (!result.Success)
            {
                Log($"[ERROR] {result.Message}");
                MessageBox.Show(result.Message, "Restore Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SyncTuningFromActiveProfile();
            Log($"[RESTORED] {result.Message}");
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
                Log($"[RESTORE] {result.Message}");
                MessageBox.Show(result.Message, "Restore Latest Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SyncTuningFromActiveProfile();
            Log($"[RESTORED LATEST] {result.Message}");
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
                    return $"{dt:yyyy-MM-dd  hh:mm:ss tt}  —  {fileName}{tag}";
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
                Log("[ERROR] Settings folder does not exist yet.");
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

        private void Log(string text)
        {
            _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            _txtLog.ScrollToBottom();
        }
    }
}