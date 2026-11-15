#pragma warning disable WFO1000

using FlightDeck_BF6_Flight_Configurator.GUI;
using System.Drawing;
using System.Windows.Forms;

namespace FlightDeck_BF6_Flight_Configurator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;

        private FDThemeContainer _themeShell = null!;
        private FDModernButton _btnClose = null!;
        private FDModernButton _btnMin = null!;
        private TableLayoutPanel _root = null!;

        private FDGroupBox _grpProfile = null!;
        private Panel _pnlProfileInner = null!;
        private FDComboBox _cmbProfilePath = null!;
        private FDModernButton _btnBrowse = null!;
        private FDGroupBox _grpPreset = null!;
        private Panel _pnlPresetContent = null!;
        private Label _lblPresetTag = null!;
        private FDComboBox _cmbScheme = null!;
        private Label _lblStickTag = null!;
        private FDComboBox _cmbSticks = null!;
        private FDCheckBox _chkInvertPitch = null!;
        private FDModernButton _btnImportPreset = null!;
        private FDModernButton _btnExportPreset = null!;

        private FDCheckBox _chkOverrideTuning = null!;
        private Label _lblHeliSens = null!;
        private FDSlider _sldHeliSens = null!;
        private NumericUpDown _numHeliSens = null!;
        private Label _lblJetSens = null!;
        private FDSlider _sldJetSens = null!;
        private NumericUpDown _numJetSens = null!;
        private Label _lblDeadzone = null!;
        private FDSlider _sldDeadzone = null!;
        private NumericUpDown _numDeadzone = null!;
        private FDGroupBox _grpInspector = null!;
        private FDModernButton _btnToggleVisual = null!;
        private Panel _pnlInspectorBody = null!;
        private ListView _lvBindings = null!;
        private ColumnHeader _colKey = null!;
        private ColumnHeader _colVal = null!;
        private ColumnHeader _colCat = null!;
        private FDControllerVisualizer _padVisualizer = null!;
        private FDGroupBox _grpBackups = null!;
        private FlowLayoutPanel _pnlBackupActions = null!;
        private FDModernButton _btnRestoreLatest = null!;
        private FDModernButton _btnSelectBackup = null!;
        private FDModernButton _btnOpenFolder = null!;
        private FDCheckBox _chkReadOnly = null!;
        private Panel _pnlApplySection = null!;
        private FDLaunchButton _btnApply = null!;

        private ContextMenuStrip _cmsBackups = null!;
        private FDTextBox _txtLog = null!;

        private Panel _pnlFooter = null!;
        private FDLabel _lblStudioBrand = null!;
        private FlowLayoutPanel _pnlFooterLinks = null!;
        private LinkLabel _lnkWebsite = null!;
        private LinkLabel _lnkDonate = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            _themeShell = new FDThemeContainer();
            _btnClose = new FDModernButton();
            _btnMin = new FDModernButton();
            _root = new TableLayoutPanel();
            _grpProfile = new FDGroupBox();
            _pnlProfileInner = new Panel();
            _cmbProfilePath = new FDComboBox();
            _btnBrowse = new FDModernButton();
            _grpPreset = new FDGroupBox();
            _pnlPresetContent = new Panel();
            _lblPresetTag = new Label();
            _cmbScheme = new FDComboBox();
            _lblStickTag = new Label();
            _cmbSticks = new FDComboBox();
            _chkInvertPitch = new FDCheckBox();
            _btnImportPreset = new FDModernButton();
            _btnExportPreset = new FDModernButton();
            _chkOverrideTuning = new FDCheckBox();
            _lblHeliSens = new Label();
            _numHeliSens = new NumericUpDown();
            _lblJetSens = new Label();
            _numJetSens = new NumericUpDown();
            _lblDeadzone = new Label();
            _numDeadzone = new NumericUpDown();
            _grpInspector = new FDGroupBox();
            _btnToggleVisual = new FDModernButton();
            _pnlInspectorBody = new Panel();
            _lvBindings = new ListView();
            _colKey = new ColumnHeader();
            _colVal = new ColumnHeader();
            _colCat = new ColumnHeader();
            _grpBackups = new FDGroupBox();
            _pnlBackupActions = new FlowLayoutPanel();
            _btnRestoreLatest = new FDModernButton();
            _btnSelectBackup = new FDModernButton();
            _btnOpenFolder = new FDModernButton();
            _chkReadOnly = new FDCheckBox();
            _pnlApplySection = new Panel();
            _btnApply = new FDLaunchButton();
            _txtLog = new FDTextBox();
            _pnlFooter = new Panel();
            _lblStudioBrand = new FDLabel();
            _pnlFooterLinks = new FlowLayoutPanel();
            _lnkDonate = new LinkLabel();
            _lnkWebsite = new LinkLabel();
            _cmsBackups = new ContextMenuStrip(components);
            _themeShell.SuspendLayout();
            _root.SuspendLayout();
            _grpProfile.SuspendLayout();
            _pnlProfileInner.SuspendLayout();
            _grpPreset.SuspendLayout();
            _pnlPresetContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_numHeliSens).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numJetSens).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numDeadzone).BeginInit();
            _grpInspector.SuspendLayout();
            _pnlInspectorBody.SuspendLayout();
            _grpBackups.SuspendLayout();
            _pnlBackupActions.SuspendLayout();
            _pnlApplySection.SuspendLayout();
            _pnlFooter.SuspendLayout();
            _pnlFooterLinks.SuspendLayout();
            SuspendLayout();
            // 
            // _themeShell
            // 
            _themeShell.BackColor = Color.FromArgb(14, 17, 22);
            _themeShell.BorderStyle = FormBorderStyle.None;
            _themeShell.Controls.Add(_btnClose);
            _themeShell.Controls.Add(_btnMin);
            _themeShell.Controls.Add(_root);
            _themeShell.Customization = "FhEO/yQbFv8bFBD/RDYt//C0AP/49PD/";
            _themeShell.Dock = DockStyle.Fill;
            _themeShell.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _themeShell.Image = null;
            _themeShell.Location = new Point(0, 0);
            _themeShell.Movable = true;
            _themeShell.Name = "_themeShell";
            _themeShell.NoRounding = false;
            _themeShell.Padding = new Padding(12, 46, 12, 12);
            _themeShell.Sizable = true;
            _themeShell.Size = new Size(920, 810);
            _themeShell.SmartBounds = true;
            _themeShell.StartPosition = FormStartPosition.CenterScreen;
            _themeShell.SubHeader = "|   BF6 Flight Configurator";
            _themeShell.TabIndex = 0;
            _themeShell.Text = "FLIGHT DECK";
            _themeShell.TransparencyKey = Color.Empty;
            _themeShell.Transparent = false;
            // 
            // _btnClose
            // 
            _btnClose.AccentColor = Color.FromArgb(255, 102, 36);
            _btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnClose.BackColor = Color.Transparent;
            _btnClose.CenterText = true;
            _btnClose.CornerRadius = 6;
            _btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _btnClose.ForeColor = Color.FromArgb(240, 244, 248);
            _btnClose.Icon = null;
            _btnClose.IsPrimaryAction = false;
            _btnClose.Location = new Point(874, 6);
            _btnClose.Name = "_btnClose";
            _btnClose.ShowAccentBar = false;
            _btnClose.ShowIconBox = true;
            _btnClose.Size = new Size(34, 26);
            _btnClose.TabIndex = 0;
            _btnClose.Text = "✕";
            _btnClose.Click += BtnClose_Click;
            // 
            // _btnMin
            // 
            _btnMin.AccentColor = Color.FromArgb(0, 180, 240);
            _btnMin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnMin.BackColor = Color.Transparent;
            _btnMin.CenterText = true;
            _btnMin.CornerRadius = 6;
            _btnMin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _btnMin.ForeColor = Color.FromArgb(240, 244, 248);
            _btnMin.Icon = null;
            _btnMin.IsPrimaryAction = false;
            _btnMin.Location = new Point(836, 6);
            _btnMin.Name = "_btnMin";
            _btnMin.ShowAccentBar = false;
            _btnMin.ShowIconBox = true;
            _btnMin.Size = new Size(34, 26);
            _btnMin.TabIndex = 1;
            _btnMin.Text = "—";
            _btnMin.Click += BtnMin_Click;
            // 
            // _root
            // 
            _root.BackColor = Color.FromArgb(14, 17, 22);
            _root.ColumnCount = 1;
            _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _root.Controls.Add(_grpProfile, 0, 0);
            _root.Controls.Add(_grpPreset, 0, 1);
            _root.Controls.Add(_grpInspector, 0, 2);
            _root.Controls.Add(_grpBackups, 0, 3);
            _root.Controls.Add(_pnlApplySection, 0, 4);
            _root.Controls.Add(_txtLog, 0, 5);
            _root.Controls.Add(_pnlFooter, 0, 6);
            _root.Dock = DockStyle.Fill;
            _root.Location = new Point(12, 46);
            _root.Name = "_root";
            _root.Padding = new Padding(4);
            _root.RowCount = 7;
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 134F));
            _root.RowStyles.Add(new RowStyle(SizeType.Percent, 68F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            _root.RowStyles.Add(new RowStyle(SizeType.Percent, 32F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            _root.Size = new Size(896, 752);
            _root.TabIndex = 2;
            // 
            // _grpProfile
            // 
            _grpProfile.BackColor = Color.Transparent;
            _grpProfile.Controls.Add(_pnlProfileInner);
            _grpProfile.Dock = DockStyle.Fill;
            _grpProfile.Font = new Font("Segoe UI", 10F);
            _grpProfile.Location = new Point(7, 7);
            _grpProfile.Name = "_grpProfile";
            _grpProfile.Padding = new Padding(12, 50, 12, 12);
            _grpProfile.Size = new Size(882, 86);
            _grpProfile.SubTitle = "Select your PROFSAVEbf6mp_profile configuration file path";
            _grpProfile.TabIndex = 0;
            _grpProfile.Title = "1. BF6 Config Path";
            // 
            // _pnlProfileInner
            // 
            _pnlProfileInner.Controls.Add(_cmbProfilePath);
            _pnlProfileInner.Controls.Add(_btnBrowse);
            _pnlProfileInner.Dock = DockStyle.Fill;
            _pnlProfileInner.Location = new Point(12, 50);
            _pnlProfileInner.Name = "_pnlProfileInner";
            _pnlProfileInner.Size = new Size(858, 24);
            _pnlProfileInner.TabIndex = 0;
            // 
            // _cmbProfilePath
            // 
            _cmbProfilePath.BackColor = Color.FromArgb(25, 30, 38);
            _cmbProfilePath.Dock = DockStyle.Fill;
            _cmbProfilePath.DrawMode = DrawMode.OwnerDrawFixed;
            _cmbProfilePath.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbProfilePath.Font = new Font("Segoe UI", 9.75F);
            _cmbProfilePath.ForeColor = Color.FromArgb(240, 244, 248);
            _cmbProfilePath.ItemHeight = 24;
            _cmbProfilePath.Location = new Point(0, 0);
            _cmbProfilePath.Name = "_cmbProfilePath";
            _cmbProfilePath.Size = new Size(748, 30);
            _cmbProfilePath.TabIndex = 0;
            _cmbProfilePath.SelectedIndexChanged += CmbProfilePath_SelectedIndexChanged;
            // 
            // _btnBrowse
            // 
            _btnBrowse.AccentColor = Color.FromArgb(0, 180, 240);
            _btnBrowse.BackColor = Color.Transparent;
            _btnBrowse.CenterText = true;
            _btnBrowse.CornerRadius = 6;
            _btnBrowse.Dock = DockStyle.Right;
            _btnBrowse.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnBrowse.ForeColor = Color.FromArgb(240, 244, 248);
            _btnBrowse.Icon = null;
            _btnBrowse.IsPrimaryAction = false;
            _btnBrowse.Location = new Point(748, 0);
            _btnBrowse.Name = "_btnBrowse";
            _btnBrowse.ShowAccentBar = true;
            _btnBrowse.ShowIconBox = true;
            _btnBrowse.Size = new Size(110, 24);
            _btnBrowse.TabIndex = 1;
            _btnBrowse.Text = "Browse...";
            _btnBrowse.Click += BtnBrowse_Click;
            // 
            // _grpPreset
            // 
            _grpPreset.BackColor = Color.Transparent;
            _grpPreset.Controls.Add(_pnlPresetContent);
            _grpPreset.Dock = DockStyle.Fill;
            _grpPreset.Font = new Font("Segoe UI", 10F);
            _grpPreset.Location = new Point(7, 99);
            _grpPreset.Name = "_grpPreset";
            _grpPreset.Padding = new Padding(12, 48, 12, 10);
            _grpPreset.Size = new Size(882, 128);
            _grpPreset.SubTitle = "Mix & match button presets, stick layouts, pitch inversion, and sensitivity/deadzone overrides";
            _grpPreset.TabIndex = 1;
            _grpPreset.Title = "2. Flight Control Scheme & Stick Tuning";
            // 
            // _pnlPresetContent
            // 
            _pnlPresetContent.BackColor = Color.Transparent;
            _pnlPresetContent.Controls.Add(_lblPresetTag);
            _pnlPresetContent.Controls.Add(_cmbScheme);
            _pnlPresetContent.Controls.Add(_lblStickTag);
            _pnlPresetContent.Controls.Add(_cmbSticks);
            _pnlPresetContent.Controls.Add(_chkInvertPitch);
            _pnlPresetContent.Controls.Add(_btnImportPreset);
            _pnlPresetContent.Controls.Add(_btnExportPreset);
            _pnlPresetContent.Controls.Add(_chkOverrideTuning);
            _pnlPresetContent.Controls.Add(_lblHeliSens);
            _pnlPresetContent.Controls.Add(_numHeliSens);
            _pnlPresetContent.Controls.Add(_lblJetSens);
            _pnlPresetContent.Controls.Add(_numJetSens);
            _pnlPresetContent.Controls.Add(_lblDeadzone);
            _pnlPresetContent.Controls.Add(_numDeadzone);
            _pnlPresetContent.Dock = DockStyle.Fill;
            _pnlPresetContent.Location = new Point(12, 48);
            _pnlPresetContent.Name = "_pnlPresetContent";
            _pnlPresetContent.Size = new Size(858, 70);
            _pnlPresetContent.TabIndex = 0;
            // 
            // _lblPresetTag
            // 
            _lblPresetTag.AutoSize = true;
            _lblPresetTag.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            _lblPresetTag.ForeColor = Color.FromArgb(148, 160, 176);
            _lblPresetTag.Location = new Point(0, 7);
            _lblPresetTag.Name = "_lblPresetTag";
            _lblPresetTag.Size = new Size(54, 15);
            _lblPresetTag.TabIndex = 0;
            _lblPresetTag.Text = "Buttons:";
            // 
            // _cmbScheme
            // 
            _cmbScheme.BackColor = Color.FromArgb(25, 30, 38);
            _cmbScheme.DrawMode = DrawMode.OwnerDrawFixed;
            _cmbScheme.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbScheme.Font = new Font("Segoe UI", 9.5F);
            _cmbScheme.ForeColor = Color.FromArgb(240, 244, 248);
            _cmbScheme.ItemHeight = 22;
            _cmbScheme.Location = new Point(56, 1);
            _cmbScheme.Name = "_cmbScheme";
            _cmbScheme.Size = new Size(185, 28);
            _cmbScheme.TabIndex = 1;
            _cmbScheme.SelectedIndexChanged += CmbScheme_SelectedIndexChanged;
            // 
            // _lblStickTag
            // 
            _lblStickTag.AutoSize = true;
            _lblStickTag.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            _lblStickTag.ForeColor = Color.FromArgb(148, 160, 176);
            _lblStickTag.Location = new Point(252, 7);
            _lblStickTag.Name = "_lblStickTag";
            _lblStickTag.Size = new Size(43, 15);
            _lblStickTag.TabIndex = 2;
            _lblStickTag.Text = "Sticks:";
            // 
            // _cmbSticks
            // 
            _cmbSticks.BackColor = Color.FromArgb(25, 30, 38);
            _cmbSticks.DrawMode = DrawMode.OwnerDrawFixed;
            _cmbSticks.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbSticks.Font = new Font("Segoe UI", 9.5F);
            _cmbSticks.ForeColor = Color.FromArgb(240, 244, 248);
            _cmbSticks.ItemHeight = 22;
            _cmbSticks.Location = new Point(298, 1);
            _cmbSticks.Name = "_cmbSticks";
            _cmbSticks.Size = new Size(165, 28);
            _cmbSticks.TabIndex = 3;
            _cmbSticks.SelectedIndexChanged += CmbSticks_SelectedIndexChanged;
            // 
            // _chkInvertPitch
            // 
            _chkInvertPitch.BackColor = Color.Transparent;
            _chkInvertPitch.Checked = true;
            _chkInvertPitch.Font = new Font("Segoe UI", 9.25F);
            _chkInvertPitch.Location = new Point(476, 3);
            _chkInvertPitch.Name = "_chkInvertPitch";
            _chkInvertPitch.Size = new Size(118, 24);
            _chkInvertPitch.TabIndex = 4;
            _chkInvertPitch.Text = "Invert Pitch";
            _chkInvertPitch.CheckedChanged += ChkInvertPitch_CheckedChanged;
            // 
            // _btnImportPreset
            // 
            _btnImportPreset.AccentColor = Color.FromArgb(0, 180, 240);
            _btnImportPreset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnImportPreset.BackColor = Color.Transparent;
            _btnImportPreset.CenterText = true;
            _btnImportPreset.CornerRadius = 6;
            _btnImportPreset.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _btnImportPreset.ForeColor = Color.FromArgb(240, 244, 248);
            _btnImportPreset.Icon = null;
            _btnImportPreset.IsPrimaryAction = false;
            _btnImportPreset.Location = new Point(610, 1);
            _btnImportPreset.Name = "_btnImportPreset";
            _btnImportPreset.ShowAccentBar = true;
            _btnImportPreset.ShowIconBox = true;
            _btnImportPreset.Size = new Size(120, 28);
            _btnImportPreset.TabIndex = 5;
            _btnImportPreset.Text = "Import Preset";
            _btnImportPreset.Click += BtnImportPreset_Click;
            // 
            // _btnExportPreset
            // 
            _btnExportPreset.AccentColor = Color.FromArgb(0, 180, 240);
            _btnExportPreset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnExportPreset.BackColor = Color.Transparent;
            _btnExportPreset.CenterText = true;
            _btnExportPreset.CornerRadius = 6;
            _btnExportPreset.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _btnExportPreset.ForeColor = Color.FromArgb(240, 244, 248);
            _btnExportPreset.Icon = null;
            _btnExportPreset.IsPrimaryAction = false;
            _btnExportPreset.Location = new Point(738, 1);
            _btnExportPreset.Name = "_btnExportPreset";
            _btnExportPreset.ShowAccentBar = true;
            _btnExportPreset.ShowIconBox = true;
            _btnExportPreset.Size = new Size(120, 28);
            _btnExportPreset.TabIndex = 6;
            _btnExportPreset.Text = "Export Preset";
            _btnExportPreset.Click += BtnExportPreset_Click;
            // 
            // _chkOverrideTuning
            // 
            _chkOverrideTuning.BackColor = Color.Transparent;
            _chkOverrideTuning.Checked = true;
            _chkOverrideTuning.Font = new Font("Segoe UI", 9F);
            _chkOverrideTuning.Location = new Point(0, 40);
            _chkOverrideTuning.Name = "_chkOverrideTuning";
            _chkOverrideTuning.Size = new Size(165, 24);
            _chkOverrideTuning.TabIndex = 7;
            _chkOverrideTuning.Text = "Tune Sens & Deadzone:";
            _chkOverrideTuning.CheckedChanged += ChkOverrideTuning_CheckedChanged;
            // 
            // _lblHeliSens
            // 
            _lblHeliSens.AutoSize = true;
            _lblHeliSens.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _lblHeliSens.ForeColor = Color.FromArgb(0, 180, 240);
            _lblHeliSens.Location = new Point(170, 44);
            _lblHeliSens.Name = "_lblHeliSens";
            _lblHeliSens.Size = new Size(32, 15);
            _lblHeliSens.TabIndex = 8;
            _lblHeliSens.Text = "Heli:";
            // 
            // _numHeliSens
            // 
            _numHeliSens.BackColor = Color.FromArgb(18, 22, 28);
            _numHeliSens.BorderStyle = BorderStyle.FixedSingle;
            _numHeliSens.DecimalPlaces = 2;
            _numHeliSens.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _numHeliSens.ForeColor = Color.FromArgb(240, 244, 248);
            _numHeliSens.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            _numHeliSens.Location = new Point(302, 40);
            _numHeliSens.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            _numHeliSens.Minimum = new decimal(new int[] { 10, 0, 0, 131072 });
            _numHeliSens.Name = "_numHeliSens";
            _numHeliSens.Size = new Size(58, 23);
            _numHeliSens.TabIndex = 10;
            _numHeliSens.Value = new decimal(new int[] { 100, 0, 0, 131072 });
            _numHeliSens.ValueChanged += NumHeliSens_ValueChanged;
            // 
            // _lblJetSens
            // 
            _lblJetSens.AutoSize = true;
            _lblJetSens.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _lblJetSens.ForeColor = Color.FromArgb(0, 180, 240);
            _lblJetSens.Location = new Point(372, 44);
            _lblJetSens.Name = "_lblJetSens";
            _lblJetSens.Size = new Size(27, 15);
            _lblJetSens.TabIndex = 11;
            _lblJetSens.Text = "Jet:";
            // 
            // _numJetSens
            // 
            _numJetSens.BackColor = Color.FromArgb(18, 22, 28);
            _numJetSens.BorderStyle = BorderStyle.FixedSingle;
            _numJetSens.DecimalPlaces = 2;
            _numJetSens.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _numJetSens.ForeColor = Color.FromArgb(240, 244, 248);
            _numJetSens.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            _numJetSens.Location = new Point(498, 40);
            _numJetSens.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            _numJetSens.Minimum = new decimal(new int[] { 10, 0, 0, 131072 });
            _numJetSens.Name = "_numJetSens";
            _numJetSens.Size = new Size(58, 23);
            _numJetSens.TabIndex = 13;
            _numJetSens.Value = new decimal(new int[] { 100, 0, 0, 131072 });
            _numJetSens.ValueChanged += NumJetSens_ValueChanged;
            // 
            // _lblDeadzone
            // 
            _lblDeadzone.AutoSize = true;
            _lblDeadzone.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            _lblDeadzone.ForeColor = Color.FromArgb(255, 102, 36);
            _lblDeadzone.Location = new Point(568, 44);
            _lblDeadzone.Name = "_lblDeadzone";
            _lblDeadzone.Size = new Size(66, 15);
            _lblDeadzone.TabIndex = 14;
            _lblDeadzone.Text = "Deadzone:";
            // 
            // _numDeadzone
            // 
            _numDeadzone.BackColor = Color.FromArgb(18, 22, 28);
            _numDeadzone.BorderStyle = BorderStyle.FixedSingle;
            _numDeadzone.DecimalPlaces = 2;
            _numDeadzone.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _numDeadzone.ForeColor = Color.FromArgb(240, 244, 248);
            _numDeadzone.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            _numDeadzone.Location = new Point(732, 40);
            _numDeadzone.Maximum = new decimal(new int[] { 50, 0, 0, 131072 });
            _numDeadzone.Name = "_numDeadzone";
            _numDeadzone.Size = new Size(58, 23);
            _numDeadzone.TabIndex = 16;
            _numDeadzone.Value = new decimal(new int[] { 6, 0, 0, 131072 });
            _numDeadzone.ValueChanged += NumDeadzone_ValueChanged;
            // 
            // _grpInspector
            // 
            _grpInspector.BackColor = Color.Transparent;
            _grpInspector.Controls.Add(_btnToggleVisual);
            _grpInspector.Controls.Add(_pnlInspectorBody);
            _grpInspector.Dock = DockStyle.Fill;
            _grpInspector.Font = new Font("Segoe UI", 10F);
            _grpInspector.Location = new Point(7, 233);
            _grpInspector.Name = "_grpInspector";
            _grpInspector.Padding = new Padding(12, 50, 12, 12);
            _grpInspector.Size = new Size(882, 218);
            _grpInspector.SubTitle = "Variables that will be updated or injected into PROFSAVEbf6mp_profile";
            _grpInspector.TabIndex = 2;
            _grpInspector.Title = "3. Keybind & Stick Axis Inspector";
            // 
            // _btnToggleVisual
            // 
            _btnToggleVisual.AccentColor = Color.FromArgb(0, 180, 240);
            _btnToggleVisual.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnToggleVisual.BackColor = Color.Transparent;
            _btnToggleVisual.CenterText = true;
            _btnToggleVisual.CornerRadius = 6;
            _btnToggleVisual.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            _btnToggleVisual.ForeColor = Color.FromArgb(240, 244, 248);
            _btnToggleVisual.Icon = null;
            _btnToggleVisual.IsPrimaryAction = false;
            _btnToggleVisual.Location = new Point(684, 11);
            _btnToggleVisual.Name = "_btnToggleVisual";
            _btnToggleVisual.ShowAccentBar = true;
            _btnToggleVisual.ShowIconBox = true;
            _btnToggleVisual.Size = new Size(186, 28);
            _btnToggleVisual.TabIndex = 0;
            _btnToggleVisual.Text = "Show Visual Controller";
            _btnToggleVisual.Click += BtnToggleVisual_Click;
            // 
            // _pnlInspectorBody
            // 
            _pnlInspectorBody.Controls.Add(_lvBindings);
            _pnlInspectorBody.Dock = DockStyle.Fill;
            _pnlInspectorBody.Location = new Point(12, 50);
            _pnlInspectorBody.Name = "_pnlInspectorBody";
            _pnlInspectorBody.Size = new Size(858, 156);
            _pnlInspectorBody.TabIndex = 1;
            // 
            // _lvBindings
            // 
            _lvBindings.BackColor = Color.FromArgb(18, 22, 28);
            _lvBindings.BorderStyle = BorderStyle.None;
            _lvBindings.Columns.AddRange(new ColumnHeader[] { _colKey, _colVal, _colCat });
            _lvBindings.Dock = DockStyle.Fill;
            _lvBindings.ForeColor = Color.FromArgb(240, 244, 248);
            _lvBindings.FullRowSelect = true;
            _lvBindings.GridLines = true;
            _lvBindings.Location = new Point(0, 0);
            _lvBindings.Name = "_lvBindings";
            _lvBindings.Size = new Size(858, 156);
            _lvBindings.TabIndex = 0;
            _lvBindings.UseCompatibleStateImageBehavior = false;
            _lvBindings.View = View.Details;
            // 
            // _colKey
            // 
            _colKey.Text = "Key";
            _colKey.Width = 460;
            // 
            // _colVal
            // 
            _colVal.Text = "Target Value";
            _colVal.Width = 130;
            // 
            // _colCat
            // 
            _colCat.Text = "Category";
            _colCat.Width = 180;
            // 
            // _grpBackups
            // 
            _grpBackups.BackColor = Color.Transparent;
            _grpBackups.Controls.Add(_pnlBackupActions);
            _grpBackups.Dock = DockStyle.Fill;
            _grpBackups.Font = new Font("Segoe UI", 10F);
            _grpBackups.Location = new Point(7, 457);
            _grpBackups.Name = "_grpBackups";
            _grpBackups.Padding = new Padding(12, 48, 12, 10);
            _grpBackups.Size = new Size(882, 90);
            _grpBackups.SubTitle = "Roll back to a previous snapshot, open the settings folder, or lock the file Read-Only";
            _grpBackups.TabIndex = 3;
            _grpBackups.Title = "4. Profile Backups & File Management";
            // 
            // _pnlBackupActions
            // 
            _pnlBackupActions.BackColor = Color.Transparent;
            _pnlBackupActions.Controls.Add(_btnRestoreLatest);
            _pnlBackupActions.Controls.Add(_btnSelectBackup);
            _pnlBackupActions.Controls.Add(_btnOpenFolder);
            _pnlBackupActions.Controls.Add(_chkReadOnly);
            _pnlBackupActions.Dock = DockStyle.Fill;
            _pnlBackupActions.Location = new Point(12, 48);
            _pnlBackupActions.Name = "_pnlBackupActions";
            _pnlBackupActions.Size = new Size(858, 32);
            _pnlBackupActions.TabIndex = 0;
            _pnlBackupActions.WrapContents = false;
            // 
            // _btnRestoreLatest
            // 
            _btnRestoreLatest.AccentColor = Color.FromArgb(255, 102, 36);
            _btnRestoreLatest.BackColor = Color.Transparent;
            _btnRestoreLatest.CenterText = true;
            _btnRestoreLatest.CornerRadius = 6;
            _btnRestoreLatest.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            _btnRestoreLatest.ForeColor = Color.FromArgb(240, 244, 248);
            _btnRestoreLatest.Icon = null;
            _btnRestoreLatest.IsPrimaryAction = false;
            _btnRestoreLatest.Location = new Point(0, 0);
            _btnRestoreLatest.Margin = new Padding(0, 0, 10, 0);
            _btnRestoreLatest.Name = "_btnRestoreLatest";
            _btnRestoreLatest.ShowAccentBar = true;
            _btnRestoreLatest.ShowIconBox = true;
            _btnRestoreLatest.Size = new Size(170, 32);
            _btnRestoreLatest.TabIndex = 0;
            _btnRestoreLatest.Text = "Restore Latest";
            _btnRestoreLatest.Click += BtnRestoreLatest_Click;
            // 
            // _btnSelectBackup
            // 
            _btnSelectBackup.AccentColor = Color.FromArgb(0, 180, 240);
            _btnSelectBackup.BackColor = Color.Transparent;
            _btnSelectBackup.CenterText = true;
            _btnSelectBackup.CornerRadius = 6;
            _btnSelectBackup.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            _btnSelectBackup.ForeColor = Color.FromArgb(240, 244, 248);
            _btnSelectBackup.Icon = null;
            _btnSelectBackup.IsPrimaryAction = false;
            _btnSelectBackup.Location = new Point(180, 0);
            _btnSelectBackup.Margin = new Padding(0, 0, 10, 0);
            _btnSelectBackup.Name = "_btnSelectBackup";
            _btnSelectBackup.ShowAccentBar = true;
            _btnSelectBackup.ShowIconBox = true;
            _btnSelectBackup.Size = new Size(175, 32);
            _btnSelectBackup.TabIndex = 1;
            _btnSelectBackup.Text = "Select Backup ▾";
            _btnSelectBackup.Click += BtnSelectBackup_Click;
            // 
            // _btnOpenFolder
            // 
            _btnOpenFolder.AccentColor = Color.FromArgb(0, 180, 240);
            _btnOpenFolder.BackColor = Color.Transparent;
            _btnOpenFolder.CenterText = true;
            _btnOpenFolder.CornerRadius = 6;
            _btnOpenFolder.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            _btnOpenFolder.ForeColor = Color.FromArgb(240, 244, 248);
            _btnOpenFolder.Icon = null;
            _btnOpenFolder.IsPrimaryAction = false;
            _btnOpenFolder.Location = new Point(365, 0);
            _btnOpenFolder.Margin = new Padding(0, 0, 16, 0);
            _btnOpenFolder.Name = "_btnOpenFolder";
            _btnOpenFolder.ShowAccentBar = false;
            _btnOpenFolder.ShowIconBox = true;
            _btnOpenFolder.Size = new Size(150, 32);
            _btnOpenFolder.TabIndex = 2;
            _btnOpenFolder.Text = "View Files ↗";
            _btnOpenFolder.Click += BtnProfile_Click;
            // 
            // _chkReadOnly
            // 
            _chkReadOnly.BackColor = Color.Transparent;
            _chkReadOnly.Checked = false;
            _chkReadOnly.Font = new Font("Segoe UI", 9.75F);
            _chkReadOnly.Location = new Point(531, 4);
            _chkReadOnly.Margin = new Padding(0, 4, 0, 0);
            _chkReadOnly.Name = "_chkReadOnly";
            _chkReadOnly.Size = new Size(160, 26);
            _chkReadOnly.TabIndex = 3;
            _chkReadOnly.Text = "Lock Read-Only";
            // 
            // _pnlApplySection
            // 
            _pnlApplySection.BackColor = Color.Transparent;
            _pnlApplySection.Controls.Add(_btnApply);
            _pnlApplySection.Dock = DockStyle.Fill;
            _pnlApplySection.Location = new Point(7, 553);
            _pnlApplySection.Name = "_pnlApplySection";
            _pnlApplySection.Padding = new Padding(0, 4, 0, 4);
            _pnlApplySection.Size = new Size(882, 58);
            _pnlApplySection.TabIndex = 4;
            // 
            // _btnApply
            // 
            _btnApply.AccentColor = Color.FromArgb(0, 180, 240);
            _btnApply.BackColor = Color.Transparent;
            _btnApply.CornerRadius = 8;
            _btnApply.Dock = DockStyle.Fill;
            _btnApply.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            _btnApply.ForeColor = Color.FromArgb(240, 244, 248);
            _btnApply.Icon = null;
            _btnApply.Location = new Point(0, 4);
            _btnApply.Name = "_btnApply";
            _btnApply.Size = new Size(882, 50);
            _btnApply.SubText = "WRITE TO PROFSAVEbf6mp_profile";
            _btnApply.TabIndex = 0;
            _btnApply.Text = "APPLY FLIGHT DECK PROFILE";
            _btnApply.Click += BtnApply_Click;
            // 
            // _txtLog
            // 
            _txtLog.BackColor = Color.Transparent;
            _txtLog.Dock = DockStyle.Fill;
            _txtLog.Font = new Font("Consolas", 9F);
            _txtLog.Location = new Point(7, 617);
            _txtLog.MaxLength = 32767;
            _txtLog.Multiline = true;
            _txtLog.Name = "_txtLog";
            _txtLog.ReadOnly = true;
            _txtLog.Size = new Size(882, 99);
            _txtLog.TabIndex = 5;
            _txtLog.TextAlign = HorizontalAlignment.Left;
            _txtLog.UseSystemPasswordChar = false;
            // 
            // _pnlFooter
            // 
            _pnlFooter.BackColor = Color.Transparent;
            _pnlFooter.Controls.Add(_lblStudioBrand);
            _pnlFooter.Controls.Add(_pnlFooterLinks);
            _pnlFooter.Dock = DockStyle.Fill;
            _pnlFooter.Location = new Point(7, 722);
            _pnlFooter.Name = "_pnlFooter";
            _pnlFooter.Padding = new Padding(2, 0, 2, 0);
            _pnlFooter.Size = new Size(882, 23);
            _pnlFooter.TabIndex = 6;
            // 
            // _lblStudioBrand
            // 
            _lblStudioBrand.BackColor = Color.Transparent;
            _lblStudioBrand.Dock = DockStyle.Left;
            _lblStudioBrand.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _lblStudioBrand.Location = new Point(2, 0);
            _lblStudioBrand.Name = "_lblStudioBrand";
            _lblStudioBrand.Size = new Size(185, 23);
            _lblStudioBrand.TabIndex = 0;
            _lblStudioBrand.Value1 = "VOID TECH";
            _lblStudioBrand.Value2 = " STUDIOS";
            // 
            // _pnlFooterLinks
            // 
            _pnlFooterLinks.BackColor = Color.Transparent;
            _pnlFooterLinks.Controls.Add(_lnkDonate);
            _pnlFooterLinks.Controls.Add(_lnkWebsite);
            _pnlFooterLinks.Dock = DockStyle.Right;
            _pnlFooterLinks.FlowDirection = FlowDirection.RightToLeft;
            _pnlFooterLinks.Location = new Point(480, 0);
            _pnlFooterLinks.Name = "_pnlFooterLinks";
            _pnlFooterLinks.Size = new Size(400, 23);
            _pnlFooterLinks.TabIndex = 1;
            _pnlFooterLinks.WrapContents = false;
            // 
            // _lnkDonate
            // 
            _lnkDonate.ActiveLinkColor = Color.White;
            _lnkDonate.AutoSize = true;
            _lnkDonate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lnkDonate.LinkBehavior = LinkBehavior.HoverUnderline;
            _lnkDonate.LinkColor = Color.FromArgb(255, 102, 36);
            _lnkDonate.Location = new Point(283, 3);
            _lnkDonate.Margin = new Padding(0, 3, 2, 0);
            _lnkDonate.Name = "_lnkDonate";
            _lnkDonate.Size = new Size(115, 15);
            _lnkDonate.TabIndex = 1;
            _lnkDonate.TabStop = true;
            _lnkDonate.Text = "Buy Me a Coffee ☕";
            _lnkDonate.VisitedLinkColor = Color.FromArgb(255, 102, 36);
            _lnkDonate.LinkClicked += LnkDonate_LinkClicked;
            // 
            // _lnkWebsite
            // 
            _lnkWebsite.ActiveLinkColor = Color.White;
            _lnkWebsite.AutoSize = true;
            _lnkWebsite.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lnkWebsite.LinkBehavior = LinkBehavior.HoverUnderline;
            _lnkWebsite.LinkColor = Color.FromArgb(0, 180, 240);
            _lnkWebsite.Location = new Point(142, 3);
            _lnkWebsite.Margin = new Padding(0, 3, 18, 0);
            _lnkWebsite.Name = "_lnkWebsite";
            _lnkWebsite.Size = new Size(123, 15);
            _lnkWebsite.TabIndex = 0;
            _lnkWebsite.TabStop = true;
            _lnkWebsite.Text = "voidtechstudios.us ↗";
            _lnkWebsite.VisitedLinkColor = Color.FromArgb(0, 180, 240);
            _lnkWebsite.LinkClicked += LnkWebsite_LinkClicked;
            // 
            // _cmsBackups
            // 
            _cmsBackups.Name = "_cmsBackups";
            _cmsBackups.Size = new Size(61, 4);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(14, 17, 22);
            ClientSize = new Size(920, 810);
            Controls.Add(_themeShell);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(860, 740);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FLIGHT DECK | BF6 FLIGHT CONFIGURATOR";
            _themeShell.ResumeLayout(false);
            _root.ResumeLayout(false);
            _grpProfile.ResumeLayout(false);
            _pnlProfileInner.ResumeLayout(false);
            _grpPreset.ResumeLayout(false);
            _pnlPresetContent.ResumeLayout(false);
            _pnlPresetContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_numHeliSens).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numJetSens).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numDeadzone).EndInit();
            _grpInspector.ResumeLayout(false);
            _pnlInspectorBody.ResumeLayout(false);
            _grpBackups.ResumeLayout(false);
            _pnlBackupActions.ResumeLayout(false);
            _pnlApplySection.ResumeLayout(false);
            _pnlFooter.ResumeLayout(false);
            _pnlFooterLinks.ResumeLayout(false);
            _pnlFooterLinks.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}