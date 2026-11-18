#pragma warning disable WFO1000

using FlightDeck_BF6_Flight_Configurator.CORE;
using FlightDeck_BF6_Flight_Configurator.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FlightDeck_BF6_Flight_Configurator.GUI
{
    // IMPORTANT:
    // Please leave these comments in place as they help protect intellectual rights and allow
    // developers to determine the version of the theme they are using.

    // Name: Cyanogen Theme
    // Created: 11/14/2026
    // Version: 1.0 (.NET 10 / VS2026)
    // Site: https://voidtechstudios.us/
    // Copyright © 2026 VoidTech Studios

    public static class FDPalette
    {
        public static readonly Color BgDeep = Color.FromArgb(14, 17, 22);
        public static readonly Color SurfaceDark = Color.FromArgb(20, 24, 31);
        public static readonly Color SurfaceCard = Color.FromArgb(25, 30, 38);
        public static readonly Color SurfaceHover = Color.FromArgb(34, 41, 52);
        public static readonly Color SurfaceInput = Color.FromArgb(18, 22, 28);
        public static readonly Color BorderSubtle = Color.FromArgb(45, 54, 68);
        public static readonly Color BorderFocus = Color.FromArgb(0, 174, 239);
        public static readonly Color AccentCyan = Color.FromArgb(0, 180, 240);
        public static readonly Color AccentOrange = Color.FromArgb(255, 102, 36);
        public static readonly Color HudGreen = Color.FromArgb(46, 204, 113);

        public static readonly Color TextPrimary = Color.FromArgb(240, 244, 248);
        public static readonly Color TextSecondary = Color.FromArgb(148, 160, 176);
        public static readonly Color TextMuted = Color.FromArgb(100, 110, 124);
    }

    public abstract class ThemeModule : ContainerControl
    {
        internal static Graphics? G;
        public static readonly Bitmap TextBitmap = new(1, 1);
        public static readonly Graphics TextGraphics = Graphics.FromImage(TextBitmap);

        public static GraphicsPath GetResponsiveRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new();
            if (rect.Width <= 0 || rect.Height <= 0)
                return path;

            int safeRadius = Math.Max(1, Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2)));
            int curveSize = safeRadius * 2;

            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180f, 90f);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270f, 90f);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0f, 90f);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90f, 90f);
            path.CloseFigure();

            return path;
        }

        internal static SizeF MeasureString(string text, Font font) =>
            TextGraphics.MeasureString(text, font);

        internal static SizeF MeasureString(string text, Font font, int width) =>
            TextGraphics.MeasureString(text, font, width, StringFormat.GenericTypographic);

        internal static GraphicsPath CreateRound(int x, int y, int width, int height, int slope) =>
            CreateRound(new Rectangle(x, y, width, height), slope);

        internal static GraphicsPath CreateRound(Rectangle r, int slope)
        {
            GraphicsPath path = new(FillMode.Winding);
            if (r.Width <= 0 || r.Height <= 0) return path;

            int safeSlope = Math.Max(1, Math.Min(slope, Math.Min(r.Width, r.Height)));
            path.AddArc(r.X, r.Y, safeSlope, safeSlope, 180.0f, 90.0f);
            path.AddArc(r.Right - safeSlope, r.Y, safeSlope, safeSlope, 270.0f, 90.0f);
            path.AddArc(r.Right - safeSlope, r.Bottom - safeSlope, safeSlope, safeSlope, 0.0f, 90.0f);
            path.AddArc(r.X, r.Bottom - safeSlope, safeSlope, safeSlope, 90.0f, 90.0f);
            path.CloseFigure();
            return path;
        }
    }

    public class ColorTable : ProfessionalColorTable
    {
        private readonly Color _backColor = FDPalette.SurfaceCard;
        private readonly Color _hoverColor = FDPalette.SurfaceHover;
        private readonly Color _borderColor = FDPalette.BorderSubtle;

        public override Color ToolStripDropDownBackground => _backColor;
        public override Color ImageMarginGradientBegin => _backColor;
        public override Color ImageMarginGradientMiddle => _backColor;
        public override Color ImageMarginGradientEnd => _backColor;
        public override Color MenuItemSelected => _hoverColor;
        public override Color MenuItemBorder => FDPalette.AccentCyan;
        public override Color MenuItemPressedGradientBegin => _backColor;
        public override Color MenuItemPressedGradientEnd => _backColor;
        public override Color MenuItemPressedGradientMiddle => _backColor;
        public override Color MenuBorder => _borderColor;
        public override Color SeparatorDark => _borderColor;
        public override Color SeparatorLight => _backColor;
    }

    [DefaultEvent("Click")]
    public class FDActionCard : Control
    {
        private string _title = "BF4 Veteran Preset";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        private string _subtitle = "Triggers for throttle, decoupled aim, and classic BF4 response curves.";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; Invalidate(); }
        }

        private Image? _icon;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        private Color _accentColor = FDPalette.AccentCyan;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        private bool _isSelected;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected
        {
            get => _isSelected;
            set { _isSelected = value; Invalidate(); }
        }

        private bool _isHovered;

        public FDActionCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(260, 84);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            int cornerRadius = Math.Max(6, (int)(Math.Min(Width, Height) * 0.14f));

            Color surfaceColor = _isSelected
                ? Color.FromArgb(28, 38, 50)
                : (_isHovered ? FDPalette.SurfaceHover : FDPalette.SurfaceCard);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, cornerRadius))
            using (SolidBrush brush = new(surfaceColor))
            {
                g.FillPath(brush, path);

                if (_isSelected || _isHovered)
                {
                    Color glow = _isSelected ? _accentColor : Color.FromArgb(140, _accentColor);
                    using Pen glowPen = new(glow, _isSelected ? 2f : 1.5f);
                    g.DrawPath(glowPen, path);
                }
                else
                {
                    using Pen pen = new(FDPalette.BorderSubtle, 1f);
                    g.DrawPath(pen, path);
                }
            }

            int iconBoxSize = (int)(Height * 0.68f);
            int padding = (Height - iconBoxSize) / 2;
            Rectangle iconRect = new(padding, padding, iconBoxSize, iconBoxSize);

            using (GraphicsPath iconBg = ThemeModule.GetResponsiveRoundedPath(iconRect, (int)(iconBoxSize * 0.2f)))
            using (LinearGradientBrush iconBrush = new(iconRect, Color.FromArgb(32, 39, 50), FDPalette.BgDeep, 45f))
            {
                g.FillPath(iconBrush, iconBg);
                using Pen rimPen = new(Color.FromArgb(40, _accentColor), 1f);
                g.DrawPath(rimPen, iconBg);
            }

            if (_icon != null)
            {
                int imgSize = (int)(iconBoxSize * 0.76f);
                int imgPadding = (iconBoxSize - imgSize) / 2;
                Rectangle imgRect = new(iconRect.X + imgPadding, iconRect.Y + imgPadding, imgSize, imgSize);
                g.DrawImage(_icon, imgRect);
            }
            else
            {
                using Font glyphFont = new("Segoe UI Symbol", iconBoxSize * 0.36f, FontStyle.Bold);
                TextRenderer.DrawText(g, "✈", glyphFont, iconRect, _accentColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            int textX = iconRect.Right + padding;
            int maxTextWidth = Width - textX - padding;
            if (maxTextWidth <= 0) return;

            float titleSize = Math.Clamp(Height * 0.13f, 9f, 12.5f);
            float subSize = Math.Clamp(Height * 0.105f, 8.25f, 9.75f);

            Font titleFont = new("Segoe UI", titleSize, FontStyle.Bold);
            while (TextRenderer.MeasureText(g, _title, titleFont).Width > maxTextWidth && titleSize > 8f)
            {
                titleSize -= 0.5f;
                titleFont.Dispose();
                titleFont = new("Segoe UI", titleSize, FontStyle.Bold);
            }

            int titleHeight = TextRenderer.MeasureText(g, _title, titleFont).Height;
            int titleY = padding;

            TextRenderer.DrawText(g, _title, titleFont, new Point(textX, titleY), FDPalette.TextPrimary);
            titleFont.Dispose();

            int subY = titleY + titleHeight;
            int remainingHeight = Height - subY - padding;

            if (remainingHeight > 0)
            {
                Font subFont = new("Segoe UI", subSize, FontStyle.Regular);
                TextFormatFlags subFlags = TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.Top;
                Size wrapConstraints = new(maxTextWidth, int.MaxValue);

                while (TextRenderer.MeasureText(g, _subtitle, subFont, wrapConstraints, subFlags).Height > remainingHeight && subSize > 7.5f)
                {
                    subSize -= 0.5f;
                    subFont.Dispose();
                    subFont = new("Segoe UI", subSize, FontStyle.Regular);
                }

                subFlags |= TextFormatFlags.EndEllipsis;
                Rectangle subRect = new(textX, subY, maxTextWidth, remainingHeight);

                TextRenderer.DrawText(g, _subtitle, subFont, subRect, FDPalette.TextSecondary, subFlags);
                subFont.Dispose();
            }
        }
    }

    [DefaultEvent("ValueChanged")]
    public class FDSlider : Control
    {
        public event EventHandler? ValueChanged;

        private decimal _minimum = 0.00m;
        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum) _value = _minimum;
                Invalidate();
            }
        }

        private decimal _maximum = 3.00m;
        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(_minimum + 0.01m, value);
                if (_value > _maximum) _value = _maximum;
                Invalidate();
            }
        }

        private decimal _value = 1.00m;
        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Value
        {
            get => _value;
            set
            {
                decimal clamped = Math.Clamp(Math.Round(value, 2), _minimum, _maximum);
                if (_value == clamped) return;
                _value = clamped;
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        private Color _accentColor = FDPalette.AccentCyan;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        private bool _isDragging;
        private bool _isHovered;

        public FDSlider()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(95, 23);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && Enabled)
            {
                _isDragging = true;
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging && Enabled)
                UpdateValueFromMouse(e.X);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
            Invalidate();
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            int pad = 7;
            int trackWidth = Math.Max(1, Width - pad * 2);
            double ratio = Math.Clamp((mouseX - pad) / (double)trackWidth, 0.0, 1.0);
            decimal newVal = _minimum + (decimal)ratio * (_maximum - _minimum);
            Value = newVal;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int pad = 7;
            int trackY = Height / 2 - 3;
            int trackWidth = Math.Max(4, Width - pad * 2);
            Rectangle trackRect = new(pad, trackY, trackWidth, 6);

            using (GraphicsPath trackPath = ThemeModule.GetResponsiveRoundedPath(trackRect, 3))
            using (SolidBrush trackBrush = new(FDPalette.SurfaceInput))
            using (Pen borderPen = new(FDPalette.BorderSubtle, 1f))
            {
                g.FillPath(trackBrush, trackPath);
                g.DrawPath(borderPen, trackPath);
            }

            double range = (double)Math.Max(0.01m, _maximum - _minimum);
            double ratio = Math.Clamp((double)(_value - _minimum) / range, 0.0, 1.0);
            int fillW = (int)Math.Round(ratio * trackWidth);

            Color activeCol = Enabled ? _accentColor : FDPalette.TextMuted;

            if (fillW > 2)
            {
                Rectangle fillRect = new(pad, trackY, fillW, 6);
                using GraphicsPath fillPath = ThemeModule.GetResponsiveRoundedPath(fillRect, 3);
                using SolidBrush fillBrush = new(activeCol);
                g.FillPath(fillBrush, fillPath);
            }

            int thumbX = pad + fillW;
            int thumbY = Height / 2;
            int radius = (_isDragging || _isHovered) && Enabled ? 6 : 5;
            Rectangle thumbRect = new(thumbX - radius, thumbY - radius, radius * 2, radius * 2);

            using (SolidBrush thumbBrush = new(Enabled ? Color.White : FDPalette.TextMuted))
            using (Pen thumbRing = new(activeCol, 2f))
            {
                g.FillEllipse(thumbBrush, thumbRect);
                g.DrawEllipse(thumbRing, thumbRect);
            }
        }
    }

    public sealed class ControllerRemapEventArgs : EventArgs
    {
        public string Domain { get; }
        public string SlotKey { get; }
        public string Concept { get; }
        public bool StackAction { get; }
        public string? SwapTargetSlotKey { get; }
        public bool CopyToOtherVehicle { get; }
        public bool ResetToDefault { get; }

        public ControllerRemapEventArgs(
            string domain,
            string slotKey,
            string concept = "",
            bool stackAction = false,
            string? swapTargetSlotKey = null,
            bool copyToOtherVehicle = false,
            bool resetToDefault = false)
        {
            Domain = domain;
            SlotKey = slotKey;
            Concept = concept;
            StackAction = stackAction;
            SwapTargetSlotKey = swapTargetSlotKey;
            CopyToOtherVehicle = copyToOtherVehicle;
            ResetToDefault = resetToDefault;
        }
    }

    public class FDControllerVisualizer : Control
    {
        public enum PadStyle { Xbox, PlayStation }
        public enum VehicleMode { Helicopter, Jet }

        public event EventHandler<ControllerRemapEventArgs>? RemapRequested;
        public event EventHandler<FlightPresets.StickLayoutType>? StickLayoutSelected;

        private PadStyle _style = PadStyle.Xbox;
        [Category("Appearance Cyanogen")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PadStyle ControllerStyle
        {
            get => _style;
            set { _style = value; Invalidate(); }
        }

        private VehicleMode _vehicle = VehicleMode.Helicopter;
        [Category("Appearance Cyanogen")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public VehicleMode ActiveVehicle
        {
            get => _vehicle;
            set { _vehicle = value; Invalidate(); }
        }

        private readonly Dictionary<string, string> _heliBinds = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _jetBinds = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _heliSlotActions = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _jetSlotActions = new(StringComparer.OrdinalIgnoreCase);

        private readonly record struct SlotHitZone(string SlotKey, string BadgeLabel, Rectangle BoxRect, Rectangle PadHotspot);
        private readonly List<SlotHitZone> _hitZones = new();

        private Rectangle _rectHeliTab;
        private Rectangle _rectJetTab;
        private Rectangle _rectResetTab;
        private Rectangle _rectXboxTab;
        private Rectangle _rectPsTab;
        private string? _hoveredSlotKey;
        private bool _hoverResetTab;
        private readonly ContextMenuStrip _cmsRemap;

        private static readonly (string ActionKey, string HeliLabel, string JetLabel, bool HeliOnly, bool JetOnly)[] AvailableActions =
        [
            ("Ascend", "Ascend (Up)", "Throttle Up", false, false),
            ("Descend", "Descend", "Airbrake", false, false),
            ("Afterburner", "Afterburner", "Afterburner", false, true),
            ("Fire", "Fire Weapon", "Fire Weapon", false, false),
            ("Countermeasures", "Flares / ECM", "Flares / ECM", false, false),
            ("Zoom", "Zoom Optics", "Zoom Optics", false, false),
            ("ChaseCam", "Chase Cam", "Chase Cam", false, false),
            ("Freelook", "Freelook", "Freelook", false, false),
            ("Ping", "Ping / Spot", "Ping / Spot", false, false),
            ("RepairGadget2", "Equipment / Repair", "Equipment 2", false, false),
            ("SwapWeapon", "Swap Wpn", "Swap Wpn", false, false),
            ("ReloadExit", "Reload / Exit", "Reload / Exit", false, false),
            ("SwitchSeat", "Switch Seat", "Switch Seat", true, false)
        ];

        private static readonly string[] AllButtonSlots =
        [
            "LT", "LB", "LS_CLICK", "DPAD_UP", "DPAD_DOWN", "DPAD_LEFT", "DPAD_RIGHT",
            "RT", "RB", "RS_CLICK", "BTN_NORTH", "BTN_WEST", "BTN_SOUTH", "BTN_EAST"
        ];

        public FDControllerVisualizer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(858, 195);
            Cursor = Cursors.Default;

            _cmsRemap = new ContextMenuStrip
            {
                Renderer = new ToolStripProfessionalRenderer(new ColorTable()),
                ForeColor = FDPalette.TextPrimary,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ShowImageMargin = true
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _cmsRemap.Dispose();
            base.Dispose(disposing);
        }

        public void LoadScheme(IReadOnlyDictionary<string, string> rawVariables)
        {
            _heliBinds.Clear();
            _jetBinds.Clear();
            _heliSlotActions.Clear();
            _jetSlotActions.Clear();

            ParseDomainActions(rawVariables, "helicopter", _heliBinds, _heliSlotActions);
            ParseDomainActions(rawVariables, "jet", _jetBinds, _jetSlotActions);

            Invalidate();
        }

        private static void ParseDomainActions(
            IReadOnlyDictionary<string, string> vars,
            string domain,
            Dictionary<string, string> targetDisplay,
            Dictionary<string, List<string>> targetActions)
        {
            foreach (var (actionKey, slots) in EngineBindings.ActionSlots)
            {
                if (domain == "jet" && actionKey == "SwitchSeat") continue;
                if (domain == "helicopter" && actionKey == "Afterburner") continue;

                var primary = slots[0];
                string baseKey = $"GstKeyBinding.{domain}.{primary.Concept}.{primary.SlotIndex}";
                if (!vars.TryGetValue($"{baseKey}.axis", out string? sAxis) ||
                    !vars.TryGetValue($"{baseKey}.button", out string? sBtn))
                    continue;

                int.TryParse(sAxis, out int axis);
                int.TryParse(sBtn, out int button);

                string slotKey = MapFrostbiteToSlot(axis, button);
                if (string.IsNullOrEmpty(slotKey)) continue;

                if (!targetActions.TryGetValue(slotKey, out var list))
                {
                    list = new List<string>();
                    targetActions[slotKey] = list;
                }
                if (!list.Contains(actionKey))
                    list.Add(actionKey);

                string friendly = FormatActionName(actionKey, domain);
                if (targetDisplay.TryGetValue(slotKey, out string? existing))
                {
                    if (!existing.Contains(friendly, StringComparison.OrdinalIgnoreCase))
                        targetDisplay[slotKey] = $"{existing}/{friendly}";
                }
                else
                {
                    targetDisplay[slotKey] = friendly;
                }
            }
        }

        private static string MapFrostbiteToSlot(int axis, int button) => (axis, button) switch
        {
            (56, 60) => string.Empty,
            (1, 60) => "LS_Y",
            (0, 60) => "LS_X",
            (10, 60) or (3, 60) => "RS_Y",
            (9, 60) or (2, 60) => "RS_X",
            (56, 0) => "DPAD_UP",
            (56, 1) => "DPAD_DOWN",
            (56, 2) => "DPAD_LEFT",
            (56, 3) => "DPAD_RIGHT",
            (56, 4) => "BTN_NORTH",
            (56, 5) => "BTN_SOUTH",
            (56, 6) => "BTN_WEST",
            (56, 7) => "BTN_EAST",
            (56, 10) => "LS_CLICK",
            (56, 11) => "RS_CLICK",
            (56, 14) => "LT",
            (56, 15) => "RT",
            (56, 16) => "LB",
            (56, 17) => "RB",
            _ => string.Empty
        };

        public static string FormatActionName(string actionKey, string domain) => actionKey switch
        {
            "Ascend" => domain == "jet" ? "Throttle Up" : "Ascend (Up)",
            "Descend" => domain == "jet" ? "Airbrake" : "Descend",
            "ThrottleAxis" => "Throttle U/D",
            "Afterburner" => "Afterburner",
            "Fire" => "Fire Weapon",
            "Countermeasures" => "Flares / ECM",
            "RepairGadget2" => "Equip / Repair",
            "Zoom" => "Zoom Optics",
            "ChaseCam" => "Chase Cam",
            "Freelook" => "Freelook",
            "Ping" => "Ping / Spot",
            "SwapWeapon" => "Swap Wpn",
            "ReloadExit" => "Reload / Exit",
            "SwitchSeat" => "Switch Seat",
            "Yaw" => "Yaw",
            "Pitch" => "Pitch",
            "Roll" => "Roll",
            _ => actionKey
        };

        public string GetFriendlySlotName(string slotKey)
        {
            bool isPs = _style == PadStyle.PlayStation;
            return slotKey switch
            {
                "LT" => isPs ? "L2 Trigger" : "LT Trigger",
                "RT" => isPs ? "R2 Trigger" : "RT Trigger",
                "LB" => isPs ? "L1 Bumper" : "LB Bumper",
                "RB" => isPs ? "R1 Bumper" : "RB Bumper",
                "LS_CLICK" => isPs ? "L3 (Left Stick Click)" : "LS Click (Left Stick)",
                "RS_CLICK" => isPs ? "R3 (Right Stick Click)" : "RS Click (Right Stick)",
                "DPAD_UP" => "D-Pad Up",
                "DPAD_DOWN" => "D-Pad Down",
                "DPAD_LEFT" => "D-Pad Left",
                "DPAD_RIGHT" => "D-Pad Right",
                "BTN_NORTH" => isPs ? "△ Triangle" : "Y Button",
                "BTN_WEST" => isPs ? "□ Square" : "X Button",
                "BTN_SOUTH" => isPs ? "✕ Cross" : "A Button",
                "BTN_EAST" => isPs ? "○ Circle" : "B Button",
                "LS_AXES" => "Left Analog Stick",
                "RS_AXES" => "Right Analog Stick",
                _ => slotKey
            };
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredSlotKey is not null || _hoverResetTab)
            {
                _hoveredSlotKey = null;
                _hoverResetTab = false;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool overTopTab = _rectHeliTab.Contains(e.Location) || _rectJetTab.Contains(e.Location) ||
                              _rectXboxTab.Contains(e.Location) || _rectPsTab.Contains(e.Location);
            bool overReset = _rectResetTab.Contains(e.Location);

            string? hitSlot = null;
            foreach (var zone in _hitZones)
            {
                if (zone.BoxRect.Contains(e.Location) || zone.PadHotspot.Contains(e.Location))
                {
                    hitSlot = zone.SlotKey;
                    break;
                }
            }

            if (hitSlot != _hoveredSlotKey || overReset != _hoverResetTab)
            {
                _hoveredSlotKey = hitSlot;
                _hoverResetTab = overReset;
                Invalidate();
            }

            Cursor = (overTopTab || overReset || hitSlot is not null) ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;

            if (_rectHeliTab.Contains(e.Location)) { ActiveVehicle = VehicleMode.Helicopter; return; }
            if (_rectJetTab.Contains(e.Location)) { ActiveVehicle = VehicleMode.Jet; return; }
            if (_rectXboxTab.Contains(e.Location)) { ControllerStyle = PadStyle.Xbox; return; }
            if (_rectPsTab.Contains(e.Location)) { ControllerStyle = PadStyle.PlayStation; return; }

            if (_rectResetTab.Contains(e.Location))
            {
                string domain = _vehicle == VehicleMode.Helicopter ? "helicopter" : "jet";
                RemapRequested?.Invoke(this, new ControllerRemapEventArgs(domain, string.Empty, resetToDefault: true));
                return;
            }

            foreach (var zone in _hitZones)
            {
                if (zone.BoxRect.Contains(e.Location) || zone.PadHotspot.Contains(e.Location))
                {
                    ShowRemapMenuForSlot(zone.SlotKey, new Point(zone.BoxRect.Left, zone.BoxRect.Bottom + 2));
                    return;
                }
            }
        }

        private void ShowRemapMenuForSlot(string slotKey, Point menuLocation)
        {
            _cmsRemap.Items.Clear();
            string domain = _vehicle == VehicleMode.Helicopter ? "helicopter" : "jet";
            string vehicleTitle = _vehicle == VehicleMode.Helicopter ? "Helicopter" : "Jet";
            var activeBinds = _vehicle == VehicleMode.Helicopter ? _heliBinds : _jetBinds;
            var activeActionsMap = _vehicle == VehicleMode.Helicopter ? _heliSlotActions : _jetSlotActions;

            if (slotKey is "LS_AXES" or "RS_AXES")
            {
                var header = new ToolStripMenuItem($"Analog Stick Layout ({vehicleTitle} & Air)")
                {
                    Enabled = false,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = FDPalette.AccentCyan
                };
                _cmsRemap.Items.Add(header);
                _cmsRemap.Items.Add(new ToolStripSeparator());

                (FlightPresets.StickLayoutType Type, string Label)[] layouts =
                [
                    (FlightPresets.StickLayoutType.Default, "Default (LS: Yaw  |  RS: Pitch & Roll)"),
                    (FlightPresets.StickLayoutType.Legacy, "Legacy (LS: Roll  |  RS: Pitch & Yaw)"),
                    (FlightPresets.StickLayoutType.Southpaw, "Southpaw (LS: Pitch & Roll  |  RS: Yaw)"),
                    (FlightPresets.StickLayoutType.LegacySouthpaw, "Legacy Southpaw (LS: Pitch & Yaw  |  RS: Roll)")
                ];

                foreach (var (sType, sLabel) in layouts)
                {
                    var item = new ToolStripMenuItem(sLabel) { ForeColor = FDPalette.TextPrimary };
                    item.Click += (_, _) => StickLayoutSelected?.Invoke(this, sType);
                    _cmsRemap.Items.Add(item);
                }

                _cmsRemap.Show(this, menuLocation);
                return;
            }

            string friendlySlot = GetFriendlySlotName(slotKey);
            string currentAction = GetBind(activeBinds, slotKey);
            activeActionsMap.TryGetValue(slotKey, out var currentSlotActions);
            currentSlotActions ??= [];

            var titleItem = new ToolStripMenuItem($"Remap {friendlySlot}  —  {vehicleTitle}")
            {
                Enabled = false,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = FDPalette.AccentCyan
            };
            _cmsRemap.Items.Add(titleItem);

            var currentSubItem = new ToolStripMenuItem($"Current: {currentAction}")
            {
                Enabled = false,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Italic),
                ForeColor = FDPalette.TextSecondary
            };
            _cmsRemap.Items.Add(currentSubItem);
            _cmsRemap.Items.Add(new ToolStripSeparator());

            foreach (var act in AvailableActions)
            {
                if (domain == "helicopter" && act.JetOnly) continue;
                if (domain == "jet" && act.HeliOnly) continue;

                string label = domain == "helicopter" ? act.HeliLabel : act.JetLabel;
                string ownerSlot = FindSlotForAction(activeActionsMap, act.ActionKey);
                string ownerSuffix = !string.IsNullOrEmpty(ownerSlot) && !ownerSlot.Equals(slotKey, StringComparison.OrdinalIgnoreCase)
                    ? $"   [{GetShortBadge(ownerSlot)}]"
                    : string.Empty;

                bool isChecked = currentSlotActions.Contains(act.ActionKey);
                var actionItem = new ToolStripMenuItem($"{label}{ownerSuffix}")
                {
                    Checked = isChecked,
                    ForeColor = isChecked ? FDPalette.AccentCyan : FDPalette.TextPrimary,
                    Font = new Font("Segoe UI", 9f, isChecked ? FontStyle.Bold : FontStyle.Regular)
                };
                actionItem.Click += (_, _) =>
                    RemapRequested?.Invoke(this, new ControllerRemapEventArgs(domain, slotKey, act.ActionKey, stackAction: false));
                _cmsRemap.Items.Add(actionItem);
            }

            _cmsRemap.Items.Add(new ToolStripSeparator());

            var stackMenu = new ToolStripMenuItem("Stack / Combine Action on This Button")
            {
                ForeColor = FDPalette.TextPrimary
            };
            foreach (var act in AvailableActions)
            {
                if (domain == "helicopter" && act.JetOnly) continue;
                if (domain == "jet" && act.HeliOnly) continue;

                string label = domain == "helicopter" ? act.HeliLabel : act.JetLabel;
                bool isChecked = currentSlotActions.Contains(act.ActionKey);
                var subItem = new ToolStripMenuItem($"Toggle + {label}")
                {
                    Checked = isChecked,
                    ForeColor = FDPalette.TextPrimary
                };
                subItem.Click += (_, _) =>
                    RemapRequested?.Invoke(this, new ControllerRemapEventArgs(domain, slotKey, act.ActionKey, stackAction: true));
                stackMenu.DropDownItems.Add(subItem);
            }
            _cmsRemap.Items.Add(stackMenu);

            var swapMenu = new ToolStripMenuItem("Swap Button With...")
            {
                ForeColor = FDPalette.TextPrimary
            };
            foreach (string otherSlot in AllButtonSlots)
            {
                if (otherSlot.Equals(slotKey, StringComparison.OrdinalIgnoreCase)) continue;
                string otherAct = GetBind(activeBinds, otherSlot);
                var swapItem = new ToolStripMenuItem($"{GetShortBadge(otherSlot)}  ({otherAct})")
                {
                    ForeColor = FDPalette.TextPrimary
                };
                swapItem.Click += (_, _) =>
                    RemapRequested?.Invoke(this, new ControllerRemapEventArgs(domain, slotKey, swapTargetSlotKey: otherSlot));
                swapMenu.DropDownItems.Add(swapItem);
            }
            _cmsRemap.Items.Add(swapMenu);

            string otherVehicleName = _vehicle == VehicleMode.Helicopter ? "Jet" : "Helicopter";
            var copyToOtherItem = new ToolStripMenuItem($"Copy All {vehicleTitle} Button Binds to {otherVehicleName}")
            {
                ForeColor = FDPalette.HudGreen
            };
            copyToOtherItem.Click += (_, _) =>
                RemapRequested?.Invoke(this, new ControllerRemapEventArgs(domain, slotKey, copyToOtherVehicle: true));
            _cmsRemap.Items.Add(copyToOtherItem);

            _cmsRemap.Items.Add(new ToolStripSeparator());

            var clearItem = new ToolStripMenuItem($"Unbind / Clear {GetShortBadge(slotKey)}")
            {
                ForeColor = FDPalette.AccentOrange
            };
            clearItem.Click += (_, _) =>
                RemapRequested?.Invoke(this, new ControllerRemapEventArgs(domain, slotKey, concept: "UNBIND"));
            _cmsRemap.Items.Add(clearItem);

            var resetItem = new ToolStripMenuItem("Reset All Binds to Preset Default")
            {
                ForeColor = FDPalette.TextSecondary
            };
            resetItem.Click += (_, _) =>
                RemapRequested?.Invoke(this, new ControllerRemapEventArgs(domain, slotKey, resetToDefault: true));
            _cmsRemap.Items.Add(resetItem);

            _cmsRemap.Show(this, menuLocation);
        }

        private static string FindSlotForAction(Dictionary<string, List<string>> slotActions, string actionKey)
        {
            foreach (var (slot, list) in slotActions)
            {
                if (list.Contains(actionKey))
                    return slot;
            }
            return string.Empty;
        }

        private string GetShortBadge(string slotKey)
        {
            bool isPs = _style == PadStyle.PlayStation;
            return slotKey switch
            {
                "LT" => isPs ? "L2" : "LT",
                "RT" => isPs ? "R2" : "RT",
                "LB" => isPs ? "L1" : "LB",
                "RB" => isPs ? "R1" : "RB",
                "LS_CLICK" => isPs ? "L3" : "LS",
                "RS_CLICK" => isPs ? "R3" : "RS",
                "DPAD_UP" => "D-UP",
                "DPAD_DOWN" => "D-DN",
                "DPAD_LEFT" => "D-LF",
                "DPAD_RIGHT" => "D-RT",
                "BTN_NORTH" => isPs ? "△" : "Y",
                "BTN_WEST" => isPs ? "□" : "X",
                "BTN_SOUTH" => isPs ? "✕" : "A",
                "BTN_EAST" => isPs ? "○" : "B",
                _ => slotKey
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 20 || Height <= 20) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle bgRect = new(0, 0, Width - 1, Height - 1);
            using (GraphicsPath bgPath = ThemeModule.GetResponsiveRoundedPath(bgRect, 6))
            using (SolidBrush bgBrush = new(FDPalette.SurfaceInput))
            using (Pen borderPen = new(FDPalette.BorderSubtle, 1f))
            {
                g.FillPath(bgBrush, bgPath);
                g.DrawPath(borderPen, bgPath);
            }

            DrawTopToggles(g);

            var activeMap = _vehicle == VehicleMode.Helicopter ? _heliBinds : _jetBinds;
            bool isPs = _style == PadStyle.PlayStation;

            int cx = Width / 2;
            int cy = (Height / 2) + 14;

            DrawControllerBody(g, cx, cy, isPs);

            _hitZones.Clear();

            int boxW = Math.Clamp((Width - 220) / 2, 220, 320);
            int leftBoxX = 12;
            int rightBoxX = Width - boxW - 12;

            string lsAxes = CombineAxes(activeMap, "LS_Y", "LS_X");
            string rsAxes = CombineAxes(activeMap, "RS_Y", "RS_X");

            int startY = 33;
            int stepY = Math.Max(22, (Height - startY - 5) / 5);

            Point lsTarget = isPs ? new Point(cx - 26, cy + 12) : new Point(cx - 42, cy - 10);
            Point dpadCenter = isPs ? new Point(cx - 42, cy - 10) : new Point(cx - 26, cy + 16);
            Point rsTarget = new(cx + 26, cy + 12);
            Point fbCenter = new(cx + 44, cy - 10);

            int gap = 5;
            int halfW = (boxW - gap) / 2;
            int stickW = (int)((boxW - gap) * 0.54f);
            int clickW = boxW - gap - stickW;

            DrawInteractiveRow(g, "LT", leftBoxX, startY + stepY * 0, boxW, isPs ? "L2" : "LT", GetBind(activeMap, "LT"),
                new Point(cx - 52, cy - 42), new Rectangle(cx - 66, cy - 54, 32, 16), true, true);
            DrawInteractiveRow(g, "LB", leftBoxX, startY + stepY * 1, boxW, isPs ? "L1" : "LB", GetBind(activeMap, "LB"),
                new Point(cx - 56, cy - 30), new Rectangle(cx - 72, cy - 38, 32, 14), true, true);
            DrawInteractiveRow(g, "LS_AXES", leftBoxX, startY + stepY * 2, stickW, "LS", lsAxes,
                lsTarget, new Rectangle(lsTarget.X - 13, lsTarget.Y - 13, 26, 26), true, false);
            DrawInteractiveRow(g, "LS_CLICK", leftBoxX + stickW + gap, startY + stepY * 2, clickW, isPs ? "L3" : "LSC", GetBind(activeMap, "LS_CLICK"),
                lsTarget, Rectangle.Empty, true, true);
            DrawInteractiveRow(g, "DPAD_UP", leftBoxX, startY + stepY * 3, halfW, "D-UP", GetBind(activeMap, "DPAD_UP"),
                new Point(dpadCenter.X, dpadCenter.Y - 8), new Rectangle(dpadCenter.X - 6, dpadCenter.Y - 13, 12, 9), true, false);
            DrawInteractiveRow(g, "DPAD_DOWN", leftBoxX + halfW + gap, startY + stepY * 3, boxW - halfW - gap, "D-DN", GetBind(activeMap, "DPAD_DOWN"),
                new Point(dpadCenter.X, dpadCenter.Y + 8), new Rectangle(dpadCenter.X - 6, dpadCenter.Y + 4, 12, 9), true, true);
            DrawInteractiveRow(g, "DPAD_LEFT", leftBoxX, startY + stepY * 4, halfW, "D-LF", GetBind(activeMap, "DPAD_LEFT"),
                new Point(dpadCenter.X - 8, dpadCenter.Y), new Rectangle(dpadCenter.X - 13, dpadCenter.Y - 6, 9, 12), true, false);
            DrawInteractiveRow(g, "DPAD_RIGHT", leftBoxX + halfW + gap, startY + stepY * 4, boxW - halfW - gap, "D-RT", GetBind(activeMap, "DPAD_RIGHT"),
                new Point(dpadCenter.X + 8, dpadCenter.Y), new Rectangle(dpadCenter.X + 4, dpadCenter.Y - 6, 9, 12), true, true);
            DrawInteractiveRow(g, "RT", rightBoxX, startY + stepY * 0, boxW, isPs ? "R2" : "RT", GetBind(activeMap, "RT"),
                new Point(cx + 52, cy - 42), new Rectangle(cx + 34, cy - 54, 32, 16), false, true);
            DrawInteractiveRow(g, "RB", rightBoxX, startY + stepY * 1, boxW, isPs ? "R1" : "RB", GetBind(activeMap, "RB"),
                new Point(cx + 56, cy - 30), new Rectangle(cx + 40, cy - 38, 32, 14), false, true);
            DrawInteractiveRow(g, "RS_AXES", rightBoxX, startY + stepY * 2, stickW, "RS", rsAxes,
                rsTarget, new Rectangle(rsTarget.X - 13, rsTarget.Y - 13, 26, 26), false, true);
            DrawInteractiveRow(g, "RS_CLICK", rightBoxX + stickW + gap, startY + stepY * 2, clickW, isPs ? "R3" : "RSC", GetBind(activeMap, "RS_CLICK"),
                rsTarget, Rectangle.Empty, false, false);
            DrawInteractiveRow(g, "BTN_NORTH", rightBoxX, startY + stepY * 3, halfW, isPs ? "△" : "Y", GetBind(activeMap, "BTN_NORTH"),
                new Point(fbCenter.X, fbCenter.Y - 9), new Rectangle(fbCenter.X - 5, fbCenter.Y - 14, 10, 10), false, true);
            DrawInteractiveRow(g, "BTN_WEST", rightBoxX + halfW + gap, startY + stepY * 3, boxW - halfW - gap, isPs ? "□" : "X", GetBind(activeMap, "BTN_WEST"),
                new Point(fbCenter.X - 9, fbCenter.Y), new Rectangle(fbCenter.X - 14, fbCenter.Y - 5, 10, 10), false, false);
            DrawInteractiveRow(g, "BTN_SOUTH", rightBoxX, startY + stepY * 4, halfW, isPs ? "✕" : "A", GetBind(activeMap, "BTN_SOUTH"),
                new Point(fbCenter.X, fbCenter.Y + 9), new Rectangle(fbCenter.X - 5, fbCenter.Y + 4, 10, 10), false, true);
            DrawInteractiveRow(g, "BTN_EAST", rightBoxX + halfW + gap, startY + stepY * 4, boxW - halfW - gap, isPs ? "○" : "B", GetBind(activeMap, "BTN_EAST"),
                new Point(fbCenter.X + 9, fbCenter.Y), new Rectangle(fbCenter.X + 4, fbCenter.Y - 5, 10, 10), false, false);
        }

        private void DrawTopToggles(Graphics g)
        {
            _rectHeliTab = new Rectangle(12, 6, 94, 21);
            _rectJetTab = new Rectangle(110, 6, 68, 21);
            _rectResetTab = new Rectangle(184, 6, 94, 21);

            _rectPsTab = new Rectangle(Width - 110, 6, 98, 21);
            _rectXboxTab = new Rectangle(Width - 190, 6, 76, 21);

            DrawPillTab(g, _rectHeliTab, "HELICOPTER", _vehicle == VehicleMode.Helicopter, FDPalette.AccentCyan);
            DrawPillTab(g, _rectJetTab, "JET", _vehicle == VehicleMode.Jet, FDPalette.AccentCyan);
            DrawPillTab(g, _rectResetTab, "RESET BINDS", _hoverResetTab, FDPalette.AccentOrange);

            DrawPillTab(g, _rectXboxTab, "XBOX", _style == PadStyle.Xbox, FDPalette.HudGreen);
            DrawPillTab(g, _rectPsTab, "PLAYSTATION", _style == PadStyle.PlayStation, Color.FromArgb(80, 150, 255));

            int hintLeft = _rectResetTab.Right + 8;
            int hintWidth = _rectXboxTab.Left - hintLeft - 8;
            if (hintWidth > 120)
            {
                Rectangle hintRect = new(hintLeft, 6, hintWidth, 21);
                using Font hintFont = new("Segoe UI", 7.75f, FontStyle.Italic);
                TextRenderer.DrawText(g, "Click any button or callout to remap", hintFont, hintRect, FDPalette.TextSecondary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private static void DrawPillTab(Graphics g, Rectangle rect, string text, bool active, Color activeColor)
        {
            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 4);
            using SolidBrush bg = new(active ? Color.FromArgb(45, activeColor) : Color.FromArgb(24, 29, 38));
            using Pen border = new(active ? activeColor : FDPalette.BorderSubtle, 1f);
            g.FillPath(bg, path);
            g.DrawPath(border, path);

            using Font f = new("Segoe UI", 7.5f, FontStyle.Bold);
            Color fg = active ? Color.White : FDPalette.TextSecondary;
            TextRenderer.DrawText(g, text, f, rect, fg, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void DrawControllerBody(Graphics g, int cx, int cy, bool isPs)
        {
            using GraphicsPath padPath = new();
            padPath.AddArc(cx - 78, cy - 42, 45, 45, 180, 90);
            padPath.AddLine(cx - 55, cy - 42, cx + 55, cy - 42);
            padPath.AddArc(cx + 33, cy - 42, 45, 45, 270, 90);
            padPath.AddBezier(new Point(cx + 78, cy - 18), new Point(cx + 92, cy + 28), new Point(cx + 72, cy + 56), new Point(cx + 52, cy + 52));
            padPath.AddBezier(new Point(cx + 52, cy + 52), new Point(cx + 38, cy + 48), new Point(cx + 32, cy + 24), new Point(cx + 18, cy + 24));
            padPath.AddLine(cx + 18, cy + 24, cx - 18, cy + 24);
            padPath.AddBezier(new Point(cx - 18, cy + 24), new Point(cx - 32, cy + 24), new Point(cx - 38, cy + 48), new Point(cx - 52, cy + 52));
            padPath.AddBezier(new Point(cx - 52, cy + 52), new Point(cx - 72, cy + 56), new Point(cx - 92, cy + 28), new Point(cx - 78, cy - 18));
            padPath.CloseFigure();

            using (LinearGradientBrush bodyBrush = new(new Rectangle(cx - 95, cy - 50, 190, 110),
                Color.FromArgb(34, 41, 54), Color.FromArgb(20, 25, 34), 90f))
            using (Pen bodyPen = new(FDPalette.AccentCyan, 1.4f))
            {
                g.FillPath(bodyBrush, padPath);
                g.DrawPath(bodyPen, padPath);
            }

            bool ltHover = _hoveredSlotKey is "LT" or "LB";
            bool rtHover = _hoveredSlotKey is "RT" or "RB";
            using (Pen leftShoulderPen = new(ltHover ? FDPalette.AccentOrange : FDPalette.BorderFocus, ltHover ? 2.2f : 1.2f))
            using (Pen rightShoulderPen = new(rtHover ? FDPalette.AccentOrange : FDPalette.BorderFocus, rtHover ? 2.2f : 1.2f))
            {
                g.DrawArc(leftShoulderPen, cx - 64, cy - 52, 30, 16, 180, 180);
                g.DrawArc(rightShoulderPen, cx + 34, cy - 52, 30, 16, 180, 180);
            }

            Point lsCenter = isPs ? new Point(cx - 26, cy + 12) : new Point(cx - 42, cy - 10);
            Point dpadCenter = isPs ? new Point(cx - 42, cy - 10) : new Point(cx - 26, cy + 16);
            Point rsCenter = new(cx + 26, cy + 12);

            bool lsHover = _hoveredSlotKey is "LS_AXES" or "LS_CLICK";
            bool rsHover = _hoveredSlotKey is "RS_AXES" or "RS_CLICK";

            using (SolidBrush lsBrush = new(lsHover ? Color.FromArgb(45, FDPalette.AccentOrange) : Color.FromArgb(16, 20, 26)))
            using (Pen lsPen = new(lsHover ? FDPalette.AccentOrange : FDPalette.AccentCyan, lsHover ? 2.2f : 1.5f))
            {
                g.FillEllipse(lsBrush, lsCenter.X - 12, lsCenter.Y - 12, 24, 24);
                g.DrawEllipse(lsPen, lsCenter.X - 12, lsCenter.Y - 12, 24, 24);
            }

            using (SolidBrush rsBrush = new(rsHover ? Color.FromArgb(45, FDPalette.AccentOrange) : Color.FromArgb(16, 20, 26)))
            using (Pen rsPen = new(rsHover ? FDPalette.AccentOrange : FDPalette.AccentCyan, rsHover ? 2.2f : 1.5f))
            {
                g.FillEllipse(rsBrush, rsCenter.X - 12, rsCenter.Y - 12, 24, 24);
                g.DrawEllipse(rsPen, rsCenter.X - 12, rsCenter.Y - 12, 24, 24);
            }

            bool dpadHover = _hoveredSlotKey is "DPAD_UP" or "DPAD_DOWN" or "DPAD_LEFT" or "DPAD_RIGHT";
            using (Pen dpadPen = new(dpadHover ? FDPalette.AccentOrange : FDPalette.TextSecondary, dpadHover ? 1.8f : 1.2f))
            {
                g.DrawRectangle(dpadPen, dpadCenter.X - 4, dpadCenter.Y - 11, 8, 22);
                g.DrawRectangle(dpadPen, dpadCenter.X - 11, dpadCenter.Y - 4, 22, 8);
            }

            int fbX = cx + 44;
            int fbY = cy - 10;
            DrawFaceButton(g, new Rectangle(fbX - 4, fbY - 13, 8, 8), _hoveredSlotKey == "BTN_NORTH");
            DrawFaceButton(g, new Rectangle(fbX - 4, fbY + 5, 8, 8), _hoveredSlotKey == "BTN_SOUTH");
            DrawFaceButton(g, new Rectangle(fbX - 13, fbY - 4, 8, 8), _hoveredSlotKey == "BTN_WEST");
            DrawFaceButton(g, new Rectangle(fbX + 5, fbY - 4, 8, 8), _hoveredSlotKey == "BTN_EAST");
        }

        private static void DrawFaceButton(Graphics g, Rectangle rect, bool hovered)
        {
            if (hovered)
            {
                using SolidBrush fill = new(FDPalette.AccentCyan);
                g.FillEllipse(fill, rect);
            }
            using Pen pen = new(hovered ? Color.White : FDPalette.AccentOrange, hovered ? 1.8f : 1.2f);
            g.DrawEllipse(pen, rect);
        }

        private void DrawInteractiveRow(
            Graphics g,
            string slotKey,
            int x,
            int y,
            int width,
            string badge,
            string actionText,
            Point targetPt,
            Rectangle padHotspot,
            bool isLeft,
            bool drawLeaderLine)
        {
            bool isBound = !string.IsNullOrEmpty(actionText) && actionText != "Unbound";
            bool isHovered = string.Equals(_hoveredSlotKey, slotKey, StringComparison.OrdinalIgnoreCase);
            Rectangle rowRect = new(x, y, width, 22);

            _hitZones.Add(new SlotHitZone(slotKey, badge, rowRect, padHotspot));

            if (drawLeaderLine || isHovered)
            {
                Point lineStart = isLeft ? new Point(rowRect.Right, y + 11) : new Point(rowRect.Left, y + 11);
                Point elbow = isLeft ? new Point(lineStart.X + 10, y + 11) : new Point(lineStart.X - 10, y + 11);

                Color lineColor = isHovered
                    ? FDPalette.AccentOrange
                    : (isBound ? Color.FromArgb(105, FDPalette.AccentCyan) : Color.FromArgb(40, 70, 80, 95));

                using Pen leaderPen = new(lineColor, isHovered ? 1.6f : 1f);
                g.DrawLine(leaderPen, lineStart, elbow);
                g.DrawLine(leaderPen, elbow, targetPt);
            }

            Color boxBg = isHovered ? FDPalette.SurfaceHover : Color.FromArgb(24, 30, 39);
            Color boxBorder = isHovered
                ? FDPalette.AccentOrange
                : (isBound ? Color.FromArgb(95, FDPalette.AccentCyan) : FDPalette.BorderSubtle);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rowRect, 4))
            using (SolidBrush bg = new(boxBg))
            using (Pen border = new(boxBorder, isHovered ? 1.5f : 1f))
            {
                g.FillPath(bg, path);
                g.DrawPath(border, path);
            }

            int badgeW = badge.Length <= 2 ? 28 : 36;
            Rectangle badgeRect = new(x + 3, y + 2, badgeW, 18);
            Color badgeBg = isHovered
                ? FDPalette.AccentOrange
                : (isBound ? FDPalette.AccentCyan : Color.FromArgb(40, 48, 60));

            using (GraphicsPath bPath = ThemeModule.GetResponsiveRoundedPath(badgeRect, 3))
            using (SolidBrush bBrush = new(badgeBg))
            {
                g.FillPath(bBrush, bPath);
            }

            using Font badgeFont = new("Segoe UI", 7.25f, FontStyle.Bold);
            TextRenderer.DrawText(g, badge, badgeFont, badgeRect,
                (isBound || isHovered) ? Color.FromArgb(12, 16, 22) : FDPalette.TextSecondary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            string displayStr = isBound ? actionText : "Unbound";
            int textX = x + badgeW + 6;
            int maxTextW = Math.Max(10, width - badgeW - 8);
            Rectangle textRect = new(textX, y, maxTextW, 22);

            float fontSize = 8.0f;
            FontStyle style = isBound ? FontStyle.Bold : FontStyle.Regular;
            Font actionFont = new("Segoe UI", fontSize, style);

            while (TextRenderer.MeasureText(g, displayStr, actionFont).Width > maxTextW && fontSize > 6.25f)
            {
                fontSize -= 0.25f;
                actionFont.Dispose();
                actionFont = new("Segoe UI", fontSize, style);
            }

            Color textCol = isHovered
                ? Color.White
                : (isBound ? FDPalette.TextPrimary : FDPalette.TextMuted);

            TextRenderer.DrawText(g, displayStr, actionFont, textRect, textCol,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);

            actionFont.Dispose();
        }

        private static string GetBind(Dictionary<string, string> map, string key) =>
            map.TryGetValue(key, out string? val) ? val : "Unbound";

        private static string CombineAxes(Dictionary<string, string> map, params string[] keys)
        {
            var parts = new List<string>();
            foreach (string k in keys)
            {
                if (map.TryGetValue(k, out string? v) && !parts.Contains(v))
                    parts.Add(v);
            }
            return parts.Count > 0 ? string.Join(" + ", parts) : "Unbound";
        }
    }

    [DefaultEvent("Click")]
    public class FDModernButton : Control
    {
        private Image? _icon;
        [Category("Appearance FlightDeck"), Description("Optional icon displayed inside a dark rounded well on the left.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        private int _cornerRadius = 6;
        [Category("Appearance FlightDeck"), Description("Corner radius of the button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(1, value); Invalidate(); }
        }

        private Color _accentColor = FDPalette.AccentCyan;
        [Category("Appearance FlightDeck"), Description("Accent and hover border color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        private bool _showAccentBar = true;
        [Category("Appearance FlightDeck"), Description("Draws a sleek vertical accent bar on the left edge.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowAccentBar
        {
            get => _showAccentBar;
            set { _showAccentBar = value; Invalidate(); }
        }

        private bool _isPrimaryAction;
        [Category("Appearance FlightDeck"), Description("Renders with a high-visibility FlightDeck gradient fill for primary actions.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsPrimaryAction
        {
            get => _isPrimaryAction;
            set { _isPrimaryAction = value; Invalidate(); }
        }

        private bool _showIconBox = true;
        [Category("Appearance FlightDeck"), Description("Draws the dark inset box behind the icon when an icon is assigned.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowIconBox
        {
            get => _showIconBox;
            set { _showIconBox = value; Invalidate(); }
        }

        private bool _centerText = true;
        [Category("Appearance FlightDeck"), Description("If true, centers the text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CenterText
        {
            get => _centerText;
            set { _centerText = value; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get => base.Text;
            set { base.Text = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; Invalidate(); }
        }

        private bool _isHovered;
        private bool _isMouseDown;

        public FDModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);

            Size = new Size(150, 36);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            ForeColor = FDPalette.TextPrimary;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _isHovered = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _isHovered = false; _isMouseDown = false; Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) { _isMouseDown = true; Invalidate(); }
        }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); _isMouseDown = false; Invalidate(); }
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle outerRect = new(0, 0, Width - 1, Height - 1);
            Rectangle innerRect = new(1, 1, Width - 3, Height - 3);

            Color gradTop, gradBottom, outerBorder, innerBevel, textColor;

            if (!Enabled)
            {
                gradTop = Color.FromArgb(32, 36, 44);
                gradBottom = Color.FromArgb(26, 30, 36);
                outerBorder = Color.FromArgb(20, 22, 28);
                innerBevel = Color.FromArgb(42, 48, 58);
                textColor = FDPalette.TextMuted;
            }
            else if (_isPrimaryAction)
            {
                if (_isMouseDown)
                {
                    gradTop = Color.FromArgb(0, 125, 180);
                    gradBottom = Color.FromArgb(0, 145, 205);
                    outerBorder = Color.FromArgb(14, 18, 24);
                    innerBevel = _accentColor;
                }
                else if (_isHovered)
                {
                    gradTop = Color.FromArgb(30, 200, 255);
                    gradBottom = Color.FromArgb(0, 150, 215);
                    outerBorder = Color.FromArgb(140, 230, 255);
                    innerBevel = Color.FromArgb(180, 240, 255);
                }
                else
                {
                    gradTop = FDPalette.AccentCyan;
                    gradBottom = Color.FromArgb(0, 130, 190);
                    outerBorder = Color.FromArgb(14, 18, 24);
                    innerBevel = Color.FromArgb(100, 215, 255);
                }
                textColor = Color.White;
            }
            else
            {
                if (_isMouseDown)
                {
                    gradTop = Color.FromArgb(26, 31, 40);
                    gradBottom = Color.FromArgb(34, 40, 52);
                    outerBorder = _accentColor;
                    innerBevel = Color.FromArgb(22, 26, 34);
                }
                else if (_isHovered)
                {
                    gradTop = Color.FromArgb(46, 55, 70);
                    gradBottom = Color.FromArgb(34, 41, 52);
                    outerBorder = _accentColor;
                    innerBevel = Color.FromArgb(120, _accentColor);
                }
                else
                {
                    gradTop = Color.FromArgb(38, 45, 58);
                    gradBottom = Color.FromArgb(28, 33, 43);
                    outerBorder = Color.FromArgb(16, 19, 25);
                    innerBevel = Color.FromArgb(62, 74, 92);
                }
                textColor = ForeColor;
            }

            using (GraphicsPath outerPath = ThemeModule.GetResponsiveRoundedPath(outerRect, _cornerRadius))
            using (GraphicsPath innerPath = ThemeModule.GetResponsiveRoundedPath(innerRect, Math.Max(1, _cornerRadius - 1)))
            using (LinearGradientBrush surfaceBrush = new(outerRect, gradTop, gradBottom, LinearGradientMode.Vertical))
            {
                g.FillPath(surfaceBrush, outerPath);

                using (LinearGradientBrush bevelBrush = new(innerRect, innerBevel, Color.FromArgb(20, innerBevel), LinearGradientMode.Vertical))
                using (Pen bevelPen = new(bevelBrush, 1f))
                {
                    g.DrawPath(bevelPen, innerPath);
                }

                using (Pen borderPen = new(outerBorder, (_isHovered || _isMouseDown) && Enabled ? 1.5f : 1f))
                {
                    g.DrawPath(borderPen, outerPath);
                }
            }

            int pressOffset = (_isMouseDown && Enabled) ? 1 : 0;
            int leftContentOffset = 6;

            if (_showAccentBar && !_isPrimaryAction)
            {
                int barHeight = Math.Max(8, (int)(Height * 0.48f));
                int barY = (Height - barHeight) / 2 + pressOffset;
                Rectangle barRect = new(5, barY, 3, barHeight);

                Color barColor = !Enabled
                    ? Color.FromArgb(65, 70, 80)
                    : (_isHovered ? Color.FromArgb(100, 225, 255) : _accentColor);

                using (GraphicsPath barPath = ThemeModule.GetResponsiveRoundedPath(barRect, 2))
                using (SolidBrush barBrush = new(barColor))
                {
                    g.FillPath(barBrush, barPath);
                }

                leftContentOffset = 12;
            }

            Rectangle textBounds = new(pressOffset, pressOffset, Width, Height);

            if (_icon != null)
            {
                int iconBox = (int)(Height * 0.68f);
                int padY = (Height - iconBox) / 2;
                int padX = Math.Max(padY, leftContentOffset);
                Rectangle iconRect = new(padX + pressOffset, padY + pressOffset, iconBox, iconBox);

                if (_showIconBox)
                {
                    using GraphicsPath iconBg = ThemeModule.GetResponsiveRoundedPath(iconRect, 5);
                    using SolidBrush iconBrush = new(FDPalette.BgDeep);
                    using Pen iconRim = new(Color.FromArgb(40, 255, 255, 255), 1f);
                    g.FillPath(iconBrush, iconBg);
                    g.DrawPath(iconRim, iconBg);
                }

                int imgSize = _showIconBox ? (int)(iconBox * 0.72f) : iconBox;
                int offset = (iconBox - imgSize) / 2;
                g.DrawImage(_icon, new Rectangle(iconRect.X + offset, iconRect.Y + offset, imgSize, imgSize));

                int leftTextStart = iconRect.Right + 8;
                textBounds = new(leftTextStart, pressOffset, Width - leftTextStart - 6, Height);
            }
            else if (_showAccentBar && !_isPrimaryAction && !_centerText)
            {
                textBounds = new(leftContentOffset + pressOffset, pressOffset, Width - leftContentOffset - 6, Height);
            }

            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis;
            flags |= _centerText ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;

            if (Enabled)
            {
                Rectangle shadowBounds = new(textBounds.X + 1, textBounds.Y + 1, textBounds.Width, textBounds.Height);
                TextRenderer.DrawText(g, Text, Font, shadowBounds, Color.FromArgb(180, 0, 0, 0), flags);
            }

            TextRenderer.DrawText(g, Text, Font, textBounds, textColor, flags);
        }
    }

    [DefaultEvent("Click")]
    public class FDLaunchButton : Control
    {
        private bool _isHovered;
        private bool _isPressed;

        private string _subText = "WRITE TO PROFSAVE_PROFILE";
        [Category("Appearance Cyanogen"), Description("Secondary tactical status line displayed below the main title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SubText
        {
            get => _subText;
            set { _subText = value; Invalidate(); }
        }

        private Image? _icon;
        [Category("Appearance Cyanogen"), Description("Optional custom icon inside the left inset well.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        private Color _accentColor = FDPalette.AccentCyan;
        [Category("Appearance Cyanogen"), Description("Primary glow and accent color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        private int _cornerRadius = 8;
        [Category("Appearance Cyanogen"), Description("Corner radius of the launch button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(2, value); Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; Invalidate(); }
        }

        public FDLaunchButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);

            Size = new Size(265, 58);
            BackColor = Color.Transparent;
            ForeColor = FDPalette.TextPrimary;
            Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            Text = "APPLY FLIGHT DECK";
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle outerRect = new(0, 0, Width - 1, Height - 1);
            Rectangle innerRect = new(1, 1, Width - 3, Height - 3);

            Color gradTop, gradBottom, outerBorder, innerBevel, titleColor, subColor;

            if (!Enabled)
            {
                gradTop = Color.FromArgb(26, 30, 38);
                gradBottom = Color.FromArgb(20, 24, 30);
                outerBorder = Color.FromArgb(35, 42, 52);
                innerBevel = Color.FromArgb(40, 46, 56);
                titleColor = FDPalette.TextMuted;
                subColor = Color.FromArgb(75, 85, 98);
            }
            else if (_isPressed)
            {
                gradTop = Color.FromArgb(0, 95, 140);
                gradBottom = Color.FromArgb(0, 125, 180);
                outerBorder = _accentColor;
                innerBevel = Color.FromArgb(60, 200, 255);
                titleColor = Color.White;
                subColor = Color.FromArgb(195, 235, 250);
            }
            else if (_isHovered)
            {
                gradTop = Color.FromArgb(20, 175, 235);
                gradBottom = Color.FromArgb(0, 115, 170);
                outerBorder = Color.FromArgb(150, 235, 255);
                innerBevel = Color.FromArgb(170, 240, 255);
                titleColor = Color.White;
                subColor = Color.FromArgb(215, 245, 255);
            }
            else
            {
                gradTop = Color.FromArgb(0, 145, 205);
                gradBottom = Color.FromArgb(0, 92, 142);
                outerBorder = Color.FromArgb(16, 22, 30);
                innerBevel = Color.FromArgb(110, 215, 255);
                titleColor = Color.White;
                subColor = Color.FromArgb(175, 225, 245);
            }

            using (GraphicsPath outerPath = ThemeModule.GetResponsiveRoundedPath(outerRect, _cornerRadius))
            using (GraphicsPath innerPath = ThemeModule.GetResponsiveRoundedPath(innerRect, Math.Max(1, _cornerRadius - 1)))
            using (LinearGradientBrush surfaceBrush = new(outerRect, gradTop, gradBottom, LinearGradientMode.Vertical))
            {
                g.FillPath(surfaceBrush, outerPath);

                using (LinearGradientBrush bevelBrush = new(innerRect, innerBevel, Color.FromArgb(15, innerBevel), LinearGradientMode.Vertical))
                using (Pen bevelPen = new(bevelBrush, 1f))
                {
                    g.DrawPath(bevelPen, innerPath);
                }

                using (Pen borderPen = new(outerBorder, (_isHovered || _isPressed) && Enabled ? 1.5f : 1f))
                {
                    g.DrawPath(borderPen, outerPath);
                }
            }

            int pressOffset = (_isPressed && Enabled) ? 1 : 0;

            int barHeight = Math.Max(12, (int)(Height * 0.52f));
            int barY = (Height - barHeight) / 2 + pressOffset;
            Rectangle barRect = new(6, barY, 3, barHeight);
            using (GraphicsPath barPath = ThemeModule.GetResponsiveRoundedPath(barRect, 2))
            using (SolidBrush barBrush = new(Enabled ? Color.FromArgb(220, 255, 255, 255) : FDPalette.TextMuted))
            {
                g.FillPath(barBrush, barPath);
            }

            int wellSize = (int)(Height * 0.66f);
            int wellY = (Height - wellSize) / 2 + pressOffset;
            int wellX = 15 + pressOffset;
            Rectangle wellRect = new(wellX, wellY, wellSize, wellSize);

            using (GraphicsPath wellPath = ThemeModule.GetResponsiveRoundedPath(wellRect, 6))
            using (LinearGradientBrush wellBrush = new(wellRect, Color.FromArgb(16, 22, 30), Color.FromArgb(10, 14, 20), 45f))
            using (Pen wellRim = new(Color.FromArgb(80, _accentColor), 1f))
            {
                g.FillPath(wellBrush, wellPath);
                g.DrawPath(wellRim, wellPath);
            }

            if (_icon != null)
            {
                int imgSize = (int)(wellSize * 0.70f);
                int imgOffset = (wellSize - imgSize) / 2;
                g.DrawImage(_icon, new Rectangle(wellRect.X + imgOffset, wellRect.Y + imgOffset, imgSize, imgSize));
            }
            else
            {
                int triRadius = Math.Max(4, (int)(wellSize * 0.22f));
                int cx = wellRect.X + wellRect.Width / 2 + 1;
                int cy = wellRect.Y + wellRect.Height / 2;
                Point[] playPoints =
                [
                    new Point(cx - triRadius + 1, cy - triRadius),
                    new Point(cx - triRadius + 1, cy + triRadius),
                    new Point(cx + triRadius + 1, cy)
                ];

                Color glyphCol = Enabled
                    ? (_isHovered ? Color.White : _accentColor)
                    : FDPalette.TextMuted;

                using SolidBrush glyphBrush = new(glyphCol);
                g.FillPolygon(glyphBrush, playPoints);
            }

            int textLeft = wellRect.Right + 12;
            int textWidth = Width - textLeft - 10;
            if (textWidth <= 10) return;

            string mainTitle = string.IsNullOrWhiteSpace(Text) ? "APPLY FLIGHT DECK" : Text;
            float mainFontSize = Math.Clamp(Height * 0.20f, 9.5f, 12.5f);
            using Font titleFont = new("Segoe UI", mainFontSize, FontStyle.Bold);

            if (!string.IsNullOrWhiteSpace(_subText))
            {
                float subFontSize = Math.Clamp(Height * 0.135f, 7.5f, 9f);
                using Font subFont = new("Segoe UI", subFontSize, FontStyle.Bold);

                int titleH = TextRenderer.MeasureText(g, mainTitle, titleFont).Height;
                int subH = TextRenderer.MeasureText(g, _subText, subFont).Height;
                int totalTextH = titleH + subH - 2;
                int startY = (Height - totalTextH) / 2 + pressOffset;

                Rectangle titleBounds = new(textLeft, startY, textWidth, titleH);
                Rectangle subBounds = new(textLeft, startY + titleH - 2, textWidth, subH);

                TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

                if (Enabled)
                {
                    Rectangle shadowRect = new(titleBounds.X + 1, titleBounds.Y + 1, titleBounds.Width, titleBounds.Height);
                    TextRenderer.DrawText(g, mainTitle, titleFont, shadowRect, Color.FromArgb(160, 0, 0, 0), flags);
                }

                TextRenderer.DrawText(g, mainTitle, titleFont, titleBounds, titleColor, flags);
                TextRenderer.DrawText(g, _subText, subFont, subBounds, subColor, flags);
            }
            else
            {
                Rectangle titleBounds = new(textLeft, pressOffset, textWidth, Height);
                TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

                if (Enabled)
                {
                    Rectangle shadowRect = new(titleBounds.X + 1, titleBounds.Y + 1, titleBounds.Width, titleBounds.Height);
                    TextRenderer.DrawText(g, mainTitle, titleFont, shadowRect, Color.FromArgb(160, 0, 0, 0), flags);
                }

                TextRenderer.DrawText(g, mainTitle, titleFont, titleBounds, titleColor, flags);
            }
        }
    }

    [DefaultEvent("CheckedChanged")]
    public class FDToggle : Control
    {
        private bool _checked;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                CheckedChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        public event EventHandler? CheckedChanged;

        public FDToggle()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(46, 22);
            Cursor = Cursors.Hand;
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            Color trackColor = _checked ? FDPalette.AccentCyan : Color.FromArgb(45, 52, 64);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, Height / 2))
            using (SolidBrush brush = new(trackColor))
            {
                g.FillPath(brush, path);
                using Pen borderPen = new(_checked ? FDPalette.AccentCyan : FDPalette.BorderSubtle, 1f);
                g.DrawPath(borderPen, path);
            }

            int padding = Math.Max(2, (int)(Height * 0.12f));
            int thumbSize = Height - (padding * 2) - 1;
            int thumbX = _checked ? (Width - thumbSize - padding - 1) : padding;

            Rectangle thumbRect = new(thumbX, padding, thumbSize, thumbSize);

            using GraphicsPath thumbPath = ThemeModule.GetResponsiveRoundedPath(thumbRect, thumbSize / 2);
            using SolidBrush thumbBrush = new(Color.White);
            g.FillPath(thumbBrush, thumbPath);
        }
    }

    [DefaultEvent("Click")]
    public class FDSidebarItem : Control
    {
        private bool _isHovered;
        private bool _isSelected;

        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Control? LinkedPanel { get; set; }

        private Image? _icon;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected
        {
            get => _isSelected;
            set { _isSelected = value; Invalidate(); }
        }

        public FDSidebarItem()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(200, 48);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnClick(EventArgs e)
        {
            Selected = true;

            if (Parent != null)
            {
                foreach (Control ctrl in Parent.Controls)
                {
                    if (ctrl is FDSidebarItem otherItem && otherItem != this)
                    {
                        otherItem.Selected = false;
                        if (otherItem.LinkedPanel != null)
                            otherItem.LinkedPanel.Visible = false;
                    }
                }
            }

            if (LinkedPanel != null)
            {
                LinkedPanel.Visible = true;
                LinkedPanel.BringToFront();
            }

            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle rect = new(0, 0, Width, Height);

            Color bg = _isSelected
                ? Color.FromArgb(28, 36, 48)
                : (_isHovered ? Color.FromArgb(22, 27, 36) : FDPalette.BgDeep);

            using (SolidBrush bgBrush = new(bg))
            {
                g.FillRectangle(bgBrush, rect);
            }

            if (_isSelected)
            {
                int lineHt = (int)(Height * 0.58f);
                int lineY = (Height - lineHt) / 2;
                using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(new Rectangle(3, lineY, 4, lineHt), 2);
                using SolidBrush accent = new(FDPalette.AccentCyan);
                g.FillPath(accent, path);
            }

            int textX = (int)(Width * 0.14f);

            if (_icon != null)
            {
                int iconSize = (int)(Height * 0.44f);
                int iconY = (Height - iconSize) / 2;
                int iconX = 16;

                g.DrawImage(_icon, new Rectangle(iconX, iconY, iconSize, iconSize));
                textX = iconX + iconSize + 10;
            }

            float fontSize = Math.Clamp(Height * 0.21f, 9f, 11f);
            Color textColor = _isSelected ? FDPalette.TextPrimary : FDPalette.TextSecondary;

            using Font font = new("Segoe UI", fontSize, _isSelected ? FontStyle.Bold : FontStyle.Regular);
            TextRenderer.DrawText(g, Text, font, new Point(textX, (Height - font.Height) / 2), textColor);
        }
    }

    public class FDStatPill : Control
    {
        private string _prefix = "PFDFILE:";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Prefix
        {
            get => _prefix;
            set { _prefix = value; Invalidate(); }
        }

        public FDStatPill()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(520, 30);
            Text = "Steam PFDFSAVE_profile Detected";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, Height / 2))
            using (SolidBrush brush = new(FDPalette.SurfaceCard))
            using (Pen borderPen = new(FDPalette.BorderSubtle, 1f))
            {
                g.FillPath(brush, path);
                g.DrawPath(borderPen, path);
            }

            float fontSize = Math.Clamp(Height * 0.32f, 8f, 10f);
            using Font prefixFont = new("Segoe UI", fontSize, FontStyle.Bold);
            using Font valFont = new("Segoe UI", fontSize, FontStyle.Regular);

            Point prefixPos = new((int)(Height * 0.42f), (Height - prefixFont.Height) / 2);
            TextRenderer.DrawText(g, _prefix, prefixFont, prefixPos, FDPalette.AccentCyan);

            int prefixWidth = TextRenderer.MeasureText(_prefix, prefixFont).Width;
            Point valPos = new(prefixPos.X + prefixWidth + 4, prefixPos.Y);
            TextRenderer.DrawText(g, Text, valFont, valPos, FDPalette.TextPrimary);
        }
    }

    public class AboutLinkTile : Control
    {
        private string _title = "GitHub Repository";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Url { get; set; } = "";

        private Image? _icon;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        private bool _isHovered;

        public AboutLinkTile()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(220, 52);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!string.IsNullOrWhiteSpace(Url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                    MessageBox.Show($"Could not open link: {ex.Message}", "FlightDeck", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            Color surfaceColor = _isHovered ? FDPalette.SurfaceHover : FDPalette.SurfaceCard;
            Color borderColor = _isHovered ? FDPalette.AccentCyan : FDPalette.BorderSubtle;

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 8))
            using (SolidBrush brush = new(surfaceColor))
            using (Pen pen = new(borderColor, 1f))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            int iconBox = (int)(Height * 0.65f);
            int pad = (Height - iconBox) / 2;
            Rectangle iconRect = new(pad, pad, iconBox, iconBox);

            using (GraphicsPath iconBg = ThemeModule.GetResponsiveRoundedPath(iconRect, 6))
            using (SolidBrush iconBrush = new(FDPalette.BgDeep))
            {
                g.FillPath(iconBrush, iconBg);
            }

            if (_icon != null)
            {
                int imgSize = (int)(iconBox * 0.7f);
                int offset = (iconBox - imgSize) / 2;
                g.DrawImage(_icon, new Rectangle(iconRect.X + offset, iconRect.Y + offset, imgSize, imgSize));
            }

            int textX = iconRect.Right + 10;
            using (Font font = new("Segoe UI", 9.5f, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, _title, font, new Point(textX, (Height - font.Height) / 2), FDPalette.TextPrimary);
            }

            using (Font arrowFont = new("Segoe UI", 10f, FontStyle.Regular))
            {
                Color arrowColor = _isHovered ? FDPalette.AccentCyan : FDPalette.TextSecondary;
                TextRenderer.DrawText(g, "↗", arrowFont, new Point(Width - 24, (Height - arrowFont.Height) / 2), arrowColor);
            }
        }
    }

    public class FDPatchNotesBox : Control
    {
        private string _title = "PATCH NOTES / CHANGELOG";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        private Color _accentColor = FDPalette.AccentCyan;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                _scrollOffset = 0;
                Invalidate();
            }
        }

        private int _scrollOffset;
        private int _maxScroll;
        private bool _isDraggingThumb;
        private int _dragStartY;
        private int _dragStartScroll;
        private Rectangle _thumbRect;
        private Rectangle _trackRect;
        private bool _hoverThumb;

        public FDPatchNotesBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, true);

            BackColor = Color.Transparent;
            ForeColor = FDPalette.TextPrimary;
            Font = new Font("Segoe UI", 9.25f, FontStyle.Regular);
            Size = new Size(448, 120);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_maxScroll <= 0) return;

            int delta = e.Delta > 0 ? -24 : 24;
            _scrollOffset = Math.Clamp(_scrollOffset + delta, 0, _maxScroll);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            if (e.Button == MouseButtons.Left && _maxScroll > 0)
            {
                if (_thumbRect.Contains(e.Location))
                {
                    _isDraggingThumb = true;
                    _dragStartY = e.Y;
                    _dragStartScroll = _scrollOffset;
                    Invalidate();
                }
                else if (_trackRect.Contains(e.Location))
                {
                    float ratio = (e.Y - _trackRect.Y) / (float)Math.Max(1, _trackRect.Height);
                    _scrollOffset = Math.Clamp((int)(ratio * _maxScroll), 0, _maxScroll);
                    Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDraggingThumb && _maxScroll > 0)
            {
                int trackScrollable = Math.Max(1, _trackRect.Height - _thumbRect.Height);
                int dy = e.Y - _dragStartY;
                int scrollDelta = (int)Math.Round((dy / (double)trackScrollable) * _maxScroll);
                _scrollOffset = Math.Clamp(_dragStartScroll + scrollDelta, 0, _maxScroll);
                Invalidate();
                return;
            }

            bool overThumb = _maxScroll > 0 && _thumbRect.Contains(e.Location);
            if (overThumb != _hoverThumb)
            {
                _hoverThumb = overThumb;
                Cursor = overThumb ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isDraggingThumb)
            {
                _isDraggingThumb = false;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 10 || Height <= 10) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle outerRect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath outerPath = ThemeModule.GetResponsiveRoundedPath(outerRect, 6);
            using (SolidBrush bgBrush = new(FDPalette.SurfaceInput))
            {
                g.FillPath(bgBrush, outerPath);
            }

            int headerH = string.IsNullOrWhiteSpace(_title) ? 0 : 25;
            Rectangle viewport = new(2, headerH + 2, Width - 16, Height - headerH - 4);
            if (viewport.Width <= 10 || viewport.Height <= 10) return;

            string rawText = string.IsNullOrWhiteSpace(Text)
                ? "- A new version of Flight Deck is ready to download on NexusMods."
                : Text;

            string[] lines = rawText.Replace("\r\n", "\n").Split('\n');

            using Font regularFont = new(Font.FontFamily, Font.Size, FontStyle.Regular);
            using Font boldFont = new(Font.FontFamily, Font.Size, FontStyle.Bold);
            using StringFormat sf = new(StringFormatFlags.LineLimit)
            {
                Trimming = StringTrimming.Word
            };

            int padX = 10;
            int textMaxW = Math.Max(20, Width - 28);
            int totalContentHeight = 6;

            var parsedLines = new List<(string Text, bool IsBullet, bool IsHeader, int Height, int AvailW)>(lines.Length);

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    parsedLines.Add((string.Empty, false, false, 8, textMaxW));
                    totalContentHeight += 8;
                    continue;
                }

                bool isBullet = line.StartsWith("- ") || line.StartsWith("* ") || line.StartsWith("• ");
                bool isHeader = !isBullet && (line.StartsWith('#') || line.EndsWith(':') || (line.StartsWith('[') && line.EndsWith(']')));

                string cleanText = isBullet ? line[2..].Trim() : line.TrimStart('#', ' ');
                int availW = isBullet ? Math.Max(20, textMaxW - 16) : textMaxW;
                Font measureFont = isHeader ? boldFont : regularFont;

                SizeF measured = g.MeasureString(cleanText, measureFont, availW);
                int rowH = Math.Max(18, (int)Math.Ceiling(measured.Height) + 2);

                parsedLines.Add((cleanText, isBullet, isHeader, rowH, availW));
                totalContentHeight += rowH + 6;
            }

            totalContentHeight += 6;
            _maxScroll = Math.Max(0, totalContentHeight - viewport.Height);
            _scrollOffset = Math.Clamp(_scrollOffset, 0, _maxScroll);

            GraphicsState state = g.Save();
            g.SetClip(viewport);

            using SolidBrush primaryBrush = new(FDPalette.TextPrimary);
            using SolidBrush secondaryBrush = new(FDPalette.TextSecondary);
            using SolidBrush accentBrush = new(_accentColor);

            int curY = viewport.Y + 6 - _scrollOffset;

            foreach (var item in parsedLines)
            {
                if (string.IsNullOrEmpty(item.Text))
                {
                    curY += item.Height;
                    continue;
                }

                if (curY + item.Height >= viewport.Top && curY <= viewport.Bottom)
                {
                    if (item.IsBullet)
                    {
                        g.FillEllipse(accentBrush, new Rectangle(padX + 2, curY + 6, 6, 6));
                        RectangleF textBounds = new(padX + 16, curY, item.AvailW, item.Height + 4);
                        g.DrawString(item.Text, regularFont, primaryBrush, textBounds);
                    }
                    else if (item.IsHeader)
                    {
                        RectangleF textBounds = new(padX, curY, item.AvailW, item.Height + 4);
                        g.DrawString(item.Text, boldFont, accentBrush, textBounds);
                    }
                    else
                    {
                        RectangleF textBounds = new(padX, curY, item.AvailW, item.Height + 4);
                        g.DrawString(item.Text, regularFont, secondaryBrush, textBounds);
                    }
                }

                curY += item.Height + 6;
            }

            g.Restore(state);

            if (headerH > 0)
            {
                GraphicsState hdrState = g.Save();
                g.SetClip(outerPath);

                Rectangle headerRect = new(0, 0, Width, headerH);
                using (SolidBrush headerBg = new(Color.FromArgb(22, 27, 35)))
                using (Pen linePen = new(FDPalette.BorderSubtle, 1f))
                {
                    g.FillRectangle(headerBg, headerRect);
                    g.DrawLine(linePen, 1, headerH, Width - 2, headerH);
                }

                g.FillRectangle(accentBrush, new Rectangle(10, 6, 3, 13));

                using Font headerFont = new("Segoe UI", 7.75f, FontStyle.Bold);
                using StringFormat hdrSf = new() { LineAlignment = StringAlignment.Center };
                g.DrawString(_title, headerFont, secondaryBrush, new RectangleF(18, 1, Width - 24, headerH), hdrSf);

                g.Restore(hdrState);
            }

            using (Pen borderPen = new(FDPalette.BorderSubtle, 1f))
            {
                g.DrawPath(borderPen, outerPath);
            }

            if (_maxScroll > 0)
            {
                _trackRect = new Rectangle(Width - 10, headerH + 6, 5, Height - headerH - 12);
                using (GraphicsPath trackPath = ThemeModule.GetResponsiveRoundedPath(_trackRect, 2))
                using (SolidBrush trackBrush = new(Color.FromArgb(26, 32, 42)))
                {
                    g.FillPath(trackBrush, trackPath);
                }

                float viewRatio = (float)viewport.Height / Math.Max(1, totalContentHeight);
                int thumbH = Math.Max(22, (int)(_trackRect.Height * viewRatio));
                int maxThumbTravel = Math.Max(1, _trackRect.Height - thumbH);
                int thumbY = _trackRect.Y + (int)((_scrollOffset / (float)_maxScroll) * maxThumbTravel);

                _thumbRect = new Rectangle(_trackRect.X, thumbY, _trackRect.Width, thumbH);
                Color thumbCol = (_isDraggingThumb || _hoverThumb) ? _accentColor : FDPalette.BorderSubtle;

                using GraphicsPath thumbPath = ThemeModule.GetResponsiveRoundedPath(_thumbRect, 2);
                using SolidBrush thumbBrush = new(thumbCol);
                g.FillPath(thumbBrush, thumbPath);
            }
            else
            {
                _thumbRect = Rectangle.Empty;
                _trackRect = Rectangle.Empty;
            }
        }
    }

    [DefaultEvent("TextChanged")]
    public class FDTextBox : Control
    {
        public readonly TextBox Base;
        private readonly int _borderRadius = 5;

        [DllImport("user32.dll")]
        private static extern nint SendMessage(nint hWnd, int wMsg, nint wParam, nint lParam);
        private const int WM_VSCFDLL = 0x115;
        private const int SB_BOTTOM = 7;

        protected override Size DefaultSize => new(200, 32);

        public FDTextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;

            Base = new TextBox
            {
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                ForeColor = FDPalette.TextPrimary,
                BackColor = FDPalette.SurfaceInput,
                Location = new Point(7, 7)
            };

            Controls.Add(Base);

            Base.TextChanged += (_, _) => { base.Text = Base.Text; };
            Base.KeyDown += OnBaseKeyDown;
            Base.Enter += (_, _) => Invalidate();
            Base.Leave += (_, _) => Invalidate();

            UpdateBaseSize();
        }

        public void ScrollToBottom()
        {
            if (Base.IsHandleCreated)
                SendMessage(Base.Handle, WM_VSCFDLL, SB_BOTTOM, nint.Zero);
        }

        public void AppendText(string text) => Base.AppendText(text);
        public void Clear() => Base.Clear();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HorizontalAlignment TextAlign
        {
            get => Base.TextAlign;
            set => Base.TextAlign = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxLength
        {
            get => Base.MaxLength;
            set => Base.MaxLength = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ReadOnly
        {
            get => Base.ReadOnly;
            set => Base.ReadOnly = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseSystemPasswordChar
        {
            get => Base.UseSystemPasswordChar;
            set => Base.UseSystemPasswordChar = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Multiline
        {
            get => Base.Multiline;
            set
            {
                Base.Multiline = value;
                Base.WordWrap = value;
                Base.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
                UpdateBaseSize();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; Base.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font
        {
            get => base.Font;
            set { base.Font = value; Base.Font = value; UpdateBaseSize(); }
        }

        private void OnBaseKeyDown(object? s, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                Base.SelectAll();
                e.SuppressKeyPress = true;
            }
        }

        private void UpdateBaseSize()
        {
            if (Base == null) return;
            Base.Width = Math.Max(10, Width - 14);

            if (!Multiline)
                Height = Base.Height + 14;
            else
                Base.Height = Math.Max(10, Height - 14);
        }

        protected override void OnResize(EventArgs e)
        {
            UpdateBaseSize();
            base.OnResize(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Base.Focus();
            base.OnMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle mainRect = new(0, 0, Width - 1, Height - 1);

            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(mainRect, _borderRadius);
            using (SolidBrush bgBrush = new(Base.BackColor))
            {
                e.Graphics.FillPath(bgBrush, path);
            }

            Color borderColor = Base.Focused ? FDPalette.BorderFocus : FDPalette.BorderSubtle;
            using (Pen borderPen = new(borderColor, Base.Focused ? 1.5f : 1f))
            {
                e.Graphics.DrawPath(borderPen, path);
            }
        }
    }

    public class FDButton : Control
    {
        private readonly int _borderRadius = 5;
        private bool _isHovered;
        private bool _isPressed;

        protected override Size DefaultSize => new(125, 35);

        public FDButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle mainRect = new(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(mainRect, _borderRadius))
            {
                Color topColor = Color.FromArgb(40, 48, 62);
                Color bottomColor = Color.FromArgb(30, 36, 46);

                if (!Enabled)
                {
                    topColor = Color.FromArgb(26, 30, 38);
                    bottomColor = Color.FromArgb(22, 26, 32);
                }
                else if (_isHovered && !_isPressed)
                {
                    topColor = Color.FromArgb(52, 62, 80);
                    bottomColor = Color.FromArgb(38, 46, 60);
                }
                else if (_isPressed)
                {
                    topColor = Color.FromArgb(24, 29, 38);
                    bottomColor = Color.FromArgb(28, 34, 44);
                }

                using (LinearGradientBrush brush = new(mainRect, topColor, bottomColor, 90F))
                {
                    e.Graphics.FillPath(brush, path);
                }

                if (Enabled && !_isPressed)
                {
                    Rectangle innerRect = new(1, 1, Width - 3, Height - 3);
                    using GraphicsPath innerPath = ThemeModule.GetResponsiveRoundedPath(innerRect, _borderRadius - 1);
                    using Pen innerPen = new(Color.FromArgb(22, 255, 255, 255), 1);
                    e.Graphics.DrawPath(innerPen, innerPath);
                }

                Color borderCol = (_isHovered && Enabled) ? FDPalette.AccentCyan : FDPalette.BorderSubtle;
                using (Pen borderPen = new(borderCol, 1))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }

            Color textColor = Enabled ? FDPalette.TextPrimary : FDPalette.TextMuted;
            TextRenderer.DrawText(e.Graphics, Text, Font, mainRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public class FDGroupBox : ContainerControl
    {
        private string _title = "Flight Control Group";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        private string _subTitle = "Configure Battlefield 6 PFDFSAVE_profile axis & button mappings";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SubTitle
        {
            get => _subTitle;
            set { _subTitle = value; Invalidate(); }
        }

        protected override Size DefaultSize => new(280, 140);

        public FDGroupBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            Padding = new Padding(12, 50, 12, 12);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle mainRect = new(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(mainRect, 7))
            using (SolidBrush brush = new(FDPalette.SurfaceDark))
            using (Pen borderPen = new(FDPalette.BorderSubtle, 1f))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            using (SolidBrush accentBar = new(FDPalette.AccentCyan))
            {
                e.Graphics.FillRectangle(accentBar, new Rectangle(12, 13, 3, 30));
            }

            using (Font titleFont = new("Segoe UI", 11.5f, FontStyle.Bold))
            using (SolidBrush titleBrush = new(FDPalette.TextPrimary))
            {
                e.Graphics.DrawString(_title, titleFont, titleBrush, new PointF(20, 10));
            }

            using (Font subFont = new("Segoe UI", 8.5f, FontStyle.Regular))
            using (SolidBrush subBrush = new(FDPalette.TextSecondary))
            {
                e.Graphics.DrawString(_subTitle, subFont, subBrush, new PointF(21, 30));
            }
        }
    }

    public class FDMenuContainer : ContainerControl
    {
        private string _title = "FlightDeck Module";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        private Image? _icon;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        protected override Size DefaultSize => new(260, 130);

        public FDMenuContainer()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            Padding = new Padding(12, 42, 12, 12);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle mainRect = new(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(mainRect, 7))
            using (SolidBrush brush = new(FDPalette.SurfaceDark))
            using (Pen borderPen = new(FDPalette.BorderSubtle, 1f))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            int textX = 12;
            int headerY = 10;
            int iconSize = 32;

            if (_icon != null)
            {
                e.Graphics.DrawImage(_icon, new Rectangle(textX, headerY, iconSize, iconSize));
                textX += iconSize + 8;
            }

            using Font titleFont = new("Segoe UI", 11.5f, FontStyle.Bold);
            using SolidBrush titleBrush = new(FDPalette.TextPrimary);
            float textY = _icon != null ? headerY + ((iconSize - titleFont.Height) / 2f) : headerY;
            e.Graphics.DrawString(_title, titleFont, titleBrush, new PointF(textX, textY));
        }
    }

    public class FDComboBox : ComboBox
    {
        public FDComboBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            BackColor = FDPalette.SurfaceCard;
            ForeColor = FDPalette.TextPrimary;
            Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            ItemHeight = 24;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle mainRect = new(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(mainRect, 5))
            using (SolidBrush bgBrush = new(FDPalette.SurfaceInput))
            using (Pen borderPen = new(Focused ? FDPalette.AccentCyan : FDPalette.BorderSubtle, 1f))
            {
                e.Graphics.FillPath(bgBrush, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            if (SelectedIndex != -1)
            {
                using SolidBrush textBrush = new(FDPalette.TextPrimary);
                e.Graphics.DrawString(Text, Font, textBrush, new PointF(8, (Height - Font.Height) / 2f));
            }

            using (Pen arrowPen = new(FDPalette.AccentCyan, 2f))
            {
                int arrowX = Width - 18;
                int arrowY = Height / 2 - 2;
                e.Graphics.DrawLine(arrowPen, arrowX, arrowY, arrowX + 4, arrowY + 4);
                e.Graphics.DrawLine(arrowPen, arrowX + 4, arrowY + 4, arrowX + 8, arrowY);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color bgColor = isSelected ? FDPalette.SurfaceHover : FDPalette.SurfaceCard;

            using (SolidBrush bgBrush = new(bgColor))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            if (isSelected)
            {
                using SolidBrush barBrush = new(FDPalette.AccentCyan);
                e.Graphics.FillRectangle(barBrush, new Rectangle(e.Bounds.X, e.Bounds.Y, 3, e.Bounds.Height));
            }

            using (SolidBrush textBrush = new(FDPalette.TextPrimary))
            {
                string itemText = Items[e.Index]?.ToString() ?? string.Empty;
                e.Graphics.DrawString(itemText, Font, textBrush, new PointF(e.Bounds.X + 8, e.Bounds.Y + 3));
            }
        }
    }

    [DefaultEvent("CheckedChanged")]
    public class FDCheckBox : Control
    {
        public event EventHandler? CheckedChanged;
        private bool _checked;

        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                CheckedChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        public FDCheckBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            Size = new Size(180, 24);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Checked = !Checked;
            base.OnMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle boxRect = new(0, (Height - 18) / 2, 18, 18);

            using (GraphicsPath boxPath = ThemeModule.GetResponsiveRoundedPath(boxRect, 4))
            using (SolidBrush bgBrush = new(_checked ? FDPalette.AccentCyan : FDPalette.SurfaceInput))
            using (Pen borderPen = new(_checked ? FDPalette.AccentCyan : FDPalette.BorderSubtle, 1f))
            {
                e.Graphics.FillPath(bgBrush, boxPath);
                e.Graphics.DrawPath(borderPen, boxPath);
            }

            if (_checked)
            {
                using Pen checkPen = new(Color.FromArgb(14, 18, 24), 2.2f);
                e.Graphics.DrawLine(checkPen, boxRect.X + 4, boxRect.Y + 9, boxRect.X + 8, boxRect.Bottom - 5);
                e.Graphics.DrawLine(checkPen, boxRect.X + 8, boxRect.Bottom - 5, boxRect.Right - 4, boxRect.Y + 5);
            }

            using SolidBrush textBrush = new(FDPalette.TextPrimary);
            float textY = (Height - e.Graphics.MeasureString(Text, Font).Height) / 2f;
            e.Graphics.DrawString(Text, Font, textBrush, new PointF(26, textY));
        }
    }

    [DefaultEvent("Click")]
    public class LaunchButton : Control
    {
        private readonly int _borderRadius = 6;
        private bool _isHovered;
        private bool _isPressed;

        public LaunchButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            DoubleBuffered = true;
            Size = new Size(240, 56);
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            Text = "APPLY FLIGHT SCHEME";
            BackColor = Color.Transparent;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 2 || Height <= 2) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle mainRect = new(0, 0, Width - 1, Height - 1);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(mainRect, _borderRadius))
            {
                Color topColor = Color.FromArgb(0, 165, 225);
                Color bottomColor = Color.FromArgb(0, 120, 175);

                if (_isHovered && !_isPressed)
                {
                    topColor = Color.FromArgb(25, 190, 250);
                    bottomColor = Color.FromArgb(0, 145, 205);
                }
                else if (_isPressed)
                {
                    topColor = Color.FromArgb(0, 110, 160);
                    bottomColor = Color.FromArgb(0, 95, 140);
                }

                using (LinearGradientBrush brush = new(mainRect, topColor, bottomColor, 90F))
                {
                    e.Graphics.FillPath(brush, path);
                }

                Rectangle innerRect = new(1, 1, Width - 3, Height - 3);
                using (GraphicsPath innerPath = ThemeModule.GetResponsiveRoundedPath(innerRect, _borderRadius - 1))
                using (Pen innerPen = new(Color.FromArgb(45, 255, 255, 255), 1))
                {
                    e.Graphics.DrawPath(innerPen, innerPath);
                }

                using (Pen borderPen = new(Color.FromArgb(14, 18, 24), 1))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, mainRect, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public class FDProgressBar : Control
    {
        private int _minimum;
        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Minimum
        {
            get => _minimum;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _minimum = value;
                if (value > _value) _value = value;
                if (value > _maximum) _maximum = value;
                Invalidate();
            }
        }

        private int _maximum = 100;
        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _maximum = value;
                if (value < _value) _value = value;
                if (value < _minimum) _minimum = value;
                Invalidate();
            }
        }

        private int _value;
        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            get => _value;
            set
            {
                _value = Math.Clamp(value, _minimum, _maximum);
                Invalidate();
            }
        }

        public void Increment(int amount) => Value += amount;

        public FDProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
            Size = new Size(200, 18);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle outer = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath gpOuter = ThemeModule.GetResponsiveRoundedPath(outer, 5);
            using (SolidBrush trackBrush = new(FDPalette.SurfaceInput))
            {
                g.FillPath(trackBrush, gpOuter);
            }

            double range = Math.Max(1, _maximum - _minimum);
            int fillWidth = (int)Math.Round((_value - _minimum) / range * (Width - 3));

            if (fillWidth > 2)
            {
                Rectangle fillRect = new(1, 1, fillWidth, Height - 3);
                using GraphicsPath gpFill = ThemeModule.GetResponsiveRoundedPath(fillRect, 4);
                using LinearGradientBrush fillBrush = new(fillRect, FDPalette.AccentCyan, Color.FromArgb(0, 125, 190), 90f);
                g.FillPath(fillBrush, gpFill);
            }

            using Pen borderPen = new(FDPalette.BorderSubtle, 1f);
            g.DrawPath(borderPen, gpOuter);
        }
    }

    public class FDLabel : Control
    {
        private string _value1 = "FLIGHT";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Value1
        {
            get => _value1;
            set { _value1 = value; Invalidate(); }
        }

        private string _value2 = "DECK";
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Value2
        {
            get => _value2;
            set { _value2 = value; Invalidate(); }
        }

        public FDLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);

            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            Size = new Size(150, 28);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            SizeF sz1 = g.MeasureString(_value1, Font, Width, StringFormat.GenericTypographic);
            PointF pt1 = new(0f, Height / 2f - sz1.Height / 2f);
            PointF pt2 = new(sz1.Width + 3f, Height / 2f - sz1.Height / 2f);

            g.DrawString(_value1, Font, Brushes.Black, pt1.X + 1f, pt1.Y + 1f);
            using (SolidBrush primaryBrush = new(FDPalette.TextPrimary))
                g.DrawString(_value1, Font, primaryBrush, pt1);

            g.DrawString(_value2, Font, Brushes.Black, pt2.X + 1f, pt2.Y + 1f);
            using (SolidBrush accentBrush = new(FDPalette.AccentCyan))
                g.DrawString(_value2, Font, accentBrush, pt2);
        }
    }

    public class FDTabControl : TabControl
    {
        public FDTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);

            SizeMode = TabSizeMode.Fixed;
            Alignment = TabAlignment.Left;
            ItemSize = new Size(34, 135);
            DrawMode = TabDrawMode.OwnerDrawFixed;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control is TabPage page)
            {
                page.BackColor = FDPalette.SurfaceDark;
                page.ForeColor = FDPalette.TextPrimary;
            }
            base.OnControlAdded(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(FDPalette.BgDeep);

            int navWidth = ItemSize.Height + 4;
            Rectangle navRect = new(0, 0, navWidth, Height - 1);

            using (GraphicsPath navPath = ThemeModule.GetResponsiveRoundedPath(navRect, 6))
            using (SolidBrush navBrush = new(FDPalette.SurfaceDark))
            using (Pen navBorder = new(FDPalette.BorderSubtle, 1f))
            {
                g.FillPath(navBrush, navPath);
                g.DrawPath(navBorder, navPath);
            }

            using StringFormat sf = new() { LineAlignment = StringAlignment.Center };

            for (int i = 0; i < TabCount; i++)
            {
                Rectangle tabRect = GetTabRect(i);
                tabRect.Y += 2;
                tabRect.Height -= 4;
                tabRect.Width -= 2;
                tabRect.X += 2;

                TabPage page = TabPages[i];
                bool selected = SelectedIndex == i;

                if (selected)
                {
                    using GraphicsPath selPath = ThemeModule.GetResponsiveRoundedPath(tabRect, 5);
                    using SolidBrush selBrush = new(FDPalette.SurfaceHover);
                    g.FillPath(selBrush, selPath);

                    using SolidBrush barBrush = new(FDPalette.AccentCyan);
                    g.FillRectangle(barBrush, new Rectangle(tabRect.X + 3, tabRect.Y + 6, 3, tabRect.Height - 12));
                }

                Rectangle textRect = new(tabRect.X + 12, tabRect.Y, tabRect.Width - 14, tabRect.Height);
                using SolidBrush textBrush = new(selected ? FDPalette.TextPrimary : FDPalette.TextSecondary);
                g.DrawString(page.Text, Font, textBrush, textRect, sf);
            }

            Rectangle contentBorder = new(navWidth + 2, 0, Width - navWidth - 3, Height - 1);
            using GraphicsPath contentPath = ThemeModule.GetResponsiveRoundedPath(contentBorder, 6);
            using Pen borderPen = new(FDPalette.BorderSubtle, 1f);
            g.DrawPath(borderPen, contentPath);
        }
    }

    [DefaultEvent("CheckedChanged")]
    public class FDOnOffBox : Control
    {
        public delegate void CheckedChangedEventHandler(object sender);
        public event CheckedChangedEventHandler? CheckedChanged;

        private bool _checked;
        [Category("Appearance FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                CheckedChanged?.Invoke(this);
                Invalidate();
            }
        }

        public FDOnOffBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);

            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            Size = new Size(76, 28);
            MinimumSize = Size;
            MaximumSize = Size;
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Checked = !Checked;
            base.OnMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle outer = new(0, 0, Width - 1, Height - 1);
            using (GraphicsPath gpOuter = ThemeModule.GetResponsiveRoundedPath(outer, 6))
            using (SolidBrush bgBrush = new(FDPalette.SurfaceInput))
            using (Pen borderPen = new(_checked ? FDPalette.AccentCyan : FDPalette.BorderSubtle, 1f))
            {
                g.FillPath(bgBrush, gpOuter);
                g.DrawPath(borderPen, gpOuter);
            }

            using StringFormat sfLeft = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near };
            using StringFormat sfRight = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far };

            Rectangle labelRect = new(8, 0, Width - 16, Height);
            if (_checked)
            {
                using SolidBrush onBrush = new(FDPalette.AccentCyan);
                g.DrawString("ON", Font, onBrush, labelRect, sfLeft);
            }
            else
            {
                using SolidBrush offBrush = new(FDPalette.TextMuted);
                g.DrawString("OFF", Font, offBrush, labelRect, sfRight);
            }

            int knobWidth = Width / 2 - 4;
            int knobX = _checked ? (Width - knobWidth - 3) : 3;
            Rectangle knobRect = new(knobX, 3, knobWidth, Height - 6);

            using GraphicsPath gpKnob = ThemeModule.GetResponsiveRoundedPath(knobRect, 4);
            using SolidBrush knobBrush = new(_checked ? FDPalette.AccentCyan : FDPalette.SurfaceHover);
            g.FillPath(knobBrush, gpKnob);
        }
    }

    public class FDSeperator : Control
    {
        public FDSeperator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            BackColor = Color.Transparent;
            Height = 10;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using Pen p1 = new(Color.FromArgb(14, 17, 22));
            using Pen p2 = new(FDPalette.BorderSubtle);
            e.Graphics.DrawLine(p1, 0, 5, Width, 5);
            e.Graphics.DrawLine(p2, 0, 6, Width, 6);
        }
    }



    public class FDKeyboard : Control
    {
        private readonly Bitmap _textBitmap = new(1, 1);
        private readonly Graphics _textGraphics;

        public const string LowerKeys = @"1234567890-=qwertyuiop[]asdfghjkl\;'zxcvbnm,./`";
        public const string UpperKeys = "!@#$%^&*()_+QWERTYUIOP{}ASDFGHJKL|:\"ZXCVBNM<>?~";

        private Control? _target;
        [Category("Behavior FlightDeck")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Control? Target
        {
            get => _target;
            set => _target = value;
        }

        public bool Shift;
        private int _pressed = -1;
        private Rectangle[] _buttons = [];
        private readonly char[] _lower;
        private readonly char[] _upper;
        private readonly string[] _other = ["Shift", "Space", "Back"];
        private PointF[] _upperCache = [];
        private PointF[] _lowerCache = [];

        public FDKeyboard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);

            Font = new Font("Segoe UI", 8.25f, FontStyle.Bold);
            _textGraphics = Graphics.FromImage(_textBitmap);

            MinimumSize = new Size(386, 162);
            MaximumSize = new Size(386, 162);

            _lower = LowerKeys.ToCharArray();
            _upper = UpperKeys.ToCharArray();

            PrepareCache();
        }

        private void PrepareCache()
        {
            _buttons = new Rectangle[51];
            _upperCache = new PointF[_upper.Length];
            _lowerCache = new PointF[_lower.Length];

            for (int y = 0; y <= 3; y++)
            {
                for (int x = 0; x <= 11; x++)
                {
                    int i = y * 12 + x;
                    Rectangle r = new(x * 32, y * 32, 32, 32);
                    _buttons[i] = r;

                    if (i != 47 && !char.IsLetter(_upper[i]))
                    {
                        SizeF sUpper = _textGraphics.MeasureString(_upper[i].ToString(), Font);
                        _upperCache[i] = new PointF(r.X + (r.Width / 2f - sUpper.Width / 2f), r.Y + r.Height - sUpper.Height - 2f);

                        SizeF sLower = _textGraphics.MeasureString(_lower[i].ToString(), Font);
                        _lowerCache[i] = new PointF(r.X + (r.Width / 2f - sLower.Width / 2f), r.Y + r.Height - sLower.Height - 2f);
                    }
                }
            }

            _buttons[48] = new Rectangle(0, 4 * 32, 2 * 32, 32);
            _buttons[49] = new Rectangle(_buttons[48].Right, 4 * 32, 8 * 32, 32);
            _buttons[50] = new Rectangle(_buttons[49].Right, 4 * 32, 2 * 32, 32);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(FDPalette.SurfaceDark);

            using Pen pBorder = new(FDPalette.BorderSubtle);
            using Pen pInner = new(FDPalette.SurfaceHover);
            using SolidBrush subCharBrush = new(FDPalette.TextMuted);
            using SolidBrush mainCharBrush = new(FDPalette.TextPrimary);

            g.DrawRectangle(pBorder, 0, 0, 12 * 32 + 1, 5 * 32 + 1);

            for (int i = 0; i < _buttons.Length; i++)
            {
                Rectangle r = _buttons[i];
                int offset = (i == _pressed) ? 1 : 0;

                using (LinearGradientBrush gb = new(r,
                    i == _pressed ? FDPalette.SurfaceInput : FDPalette.SurfaceCard,
                    FDPalette.SurfaceDark, 90f))
                {
                    g.FillRectangle(gb, r);
                }

                switch (i)
                {
                    case 48:
                    case 49:
                    case 50:
                        string label = _other[i - 48];
                        SizeF sz = g.MeasureString(label, Font);
                        g.DrawString(label, Font, mainCharBrush,
                            r.X + (r.Width / 2f - sz.Width / 2f) + offset,
                            r.Y + (r.Height / 2f - sz.Height / 2f) + offset);
                        break;

                    case 47:
                        DrawArrow(g, FDPalette.AccentCyan, r.X + offset, r.Y + offset);
                        break;

                    default:
                        char primary = Shift ? _upper[i] : _lower[i];
                        char secondary = Shift ? _lower[i] : _upper[i];
                        PointF cachePt = Shift ? _lowerCache[i] : _upperCache[i];

                        g.DrawString(primary.ToString(), Font, mainCharBrush, r.X + 3 + offset, r.Y + 2 + offset);
                        if (!char.IsLetter(secondary))
                            g.DrawString(secondary.ToString(), Font, subCharBrush, cachePt.X + offset, cachePt.Y + offset);
                        break;
                }

                g.DrawRectangle(pInner, r.X + 1 + offset, r.Y + 1 + offset, r.Width - 2, r.Height - 2);
                g.DrawRectangle(pBorder, r.X + offset, r.Y + offset, r.Width, r.Height);
            }
        }

        private static void DrawArrow(Graphics g, Color color, int rx, int ry)
        {
            Rectangle r = new(rx + 8, ry + 8, 16, 16);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using Pen p = new(color, 1.5f);
            using AdjustableArrowCap cap = new(3f, 2f);
            p.CustomEndCap = cap;
            g.DrawArc(p, r, 0f, 290f);
            g.SmoothingMode = SmoothingMode.None;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int index = e.Y / 32 * 12 + e.X / 32;

            if (index > 47)
            {
                for (int i = 48; i < _buttons.Length; i++)
                {
                    if (_buttons[i].Contains(e.X, e.Y))
                    {
                        _pressed = i;
                        break;
                    }
                }
            }
            else if (index >= 0 && index < _buttons.Length)
            {
                _pressed = index;
            }

            HandleKey();
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _pressed = -1;
            Invalidate();
            base.OnMouseUp(e);
        }

        private void HandleKey()
        {
            if (_target is null || _pressed == -1) return;

            switch (_pressed)
            {
                case 47:
                    _target.Text = string.Empty;
                    break;
                case 48:
                    Shift = !Shift;
                    break;
                case 49:
                    _target.Text += " ";
                    break;
                case 50:
                    if (_target.Text.Length > 0)
                        _target.Text = _target.Text[..^1];
                    break;
                default:
                    char c = Shift ? _upper[_pressed] : _lower[_pressed];
                    _target.Text += c.ToString();
                    break;
            }
        }
    }
}