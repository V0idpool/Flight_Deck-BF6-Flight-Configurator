# FLIGHT DECK | BF6 Flight Configurator

<p align="center">
  <img width="512" height="512" alt="FDLogo" src="https://github.com/user-attachments/assets/669bc6c4-acb5-4d0b-b27f-9533613b28a2" />
</p>

**Helicopter & Jet Controller Config Utility for Battlefield 6**  
*Developed by [Void Tech Studios](https://voidtechstudios.us/)*

---

## Overview

Tired of fighting Battlefield 6's default helicopter auto-leveling assist, sluggish stick acceleration, or left-stick throttle?  
**Flight Deck** is a configuration utility designed for veteran Battlefield pilots, and those that HATE the default controller schemes.

Flight Deck configures your `PROFSAVEbf6mp_profile` file to restore classic **Battlefield 4 Veteran**, **Battlefield 3 Classic**, or **Battlefield 2042 Alternate** flight controls—or lets you build your own **fully custom button layout** directly on an interactive visual controller map, putting collective/throttle back on **RT / LT**, disabling horrible flight assists, applying true 1:1 or BF4 stick response curves, and protecting your ground vehicle/tank pitch settings from getting inverted (unless you're into that kinda thing).
---

## Key Features

* **Interactive Visual Controller Remapper (Xbox & PlayStation):**
  * Click any button callout or physical gamepad button on the **Visual Controller Map** to remap Helicopter or Jet controls on the fly.
  * **Smart 1-Click Assign & Auto-Swap:** Assign any flight action (**Ascend/Throttle**, **Descend/Airbrake**, **Afterburner**, **Fire Weapon**, **Flares / ECM**, **Zoom Optics**, **Chase Cam**, **Freelook**, **Ping / Spot**, **Equipment / Repair**, **Swap Weapon**, **Reload / Exit**, **Switch Seat**) to any controller button, if that button was already in use, Flight Deck automatically swaps the displaced action so nothing gets lost.
  * **Stack / Combine Actions:** Bind multiple actions onto a single button (such as **Reload + Spot** on **X / Square**).
  * **Swap, Mirror & Reset:** Swap any two buttons directly, copy your entire Helicopter button layout over to Jet (or vice versa) in one click, unbind individual buttons, or click **RESET BINDS** to restore preset defaults.
  * Automatically sets BF6 to Custom Button Mode (`Scheme5Buttons 4` / `Scheme2Buttons 4`), disables flight/terrain assists, and writes BF6-validated `GstKeyBinding.version 2000007` multi-slot tables so the game never wipes your custom binds on startup.
* **4 Built-In Flight Control Schemes:**
  * **Battlefield 4 | Veteran Flight:** Sets Helicopter (`Scheme5`) & Jet (`Scheme2`) buttons to Trigger Throttle (**RT** Ascend / **LT** Descend), disables Helicopter/Plane Assist & Terrain Avoidance, decouples pilot freelook, sets BF4 aiming curves (`Curve = 2`), and eliminates stick aiming acceleration (`0.000000`).
  * **Battlefield 3 | Classic Alternate:** **RT / LT** Throttle paired with pure 1:1 linear BF3 stick response (`Curve = 1`, zero acceleration) and zero flight assist.
  * **Battlefield 2042 | Alternate:** **RT / LT** Ascend & Descend with BF2042 vehicle curves and acceleration presets.
  * **Battlefield 6 - Stock Default:** Restores all stock BF6 flight physics and default button/stick mappings.
* **Mix & Match Stick Layouts:** Pair any button preset with **Default**, **Legacy**, **Southpaw**, or **Legacy Southpaw** analog stick layouts.
* **Tank & Ground Turret Pitch Safeguard:** Toggling **Invert Pitch** only inverts Helicopter & Jet pitch while explicitly locking Ground/Tank schemes (`Scheme1`, `Scheme3`, `Scheme4`) to normal so your tank aiming is never inverted.
* **Sensitivity & Deadzone Tuning:** Dial in exact Helicopter Sensitivity (`0.10` - `3.00`), Jet Sensitivity (`0.10` - `3.00`), and Center Stick Deadzones (`0.00` - `0.50`).
* **Live Variable Inspector:** Toggle between the interactive **Visual Controller Map** and the raw `GstInput` / `GstKeyBinding` table to inspect every single variable before applying a Flight Deck Profile.
* **Import, Export & Manage Custom Presets (`.json` / `.ini`):** Save your personalized flight configuration and custom button mappings with a custom title and description, import community profiles directly into the preset dropdown (stored in `CustomPresets\`), or delete custom presets anytime.
* **Automatic Timestamped Backups & Read-Only Lock:** Every time you click Apply, Flight Deck creates a timestamped `.bak_YYYYMMDD_HHMMSS` backup in your settings folder. Roll back to your latest or original backup anytime via the **Select Backup** menu.

---

## Installation & How to Use

1. Download from NexusMods: https://www.nexusmods.com/battlefield6/mods/25 and extract `Flight Deck.exe` anywhere on your PC (no installation required).
2. Close Battlefield 6 before applying changes so the game does not overwrite your profile on exit.
3. Run `Flight Deck.exe`. It will automatically detect your `PROFSAVEbf6mp_profile` file inside your `Documents\Battlefield 6\settings` (or OneDrive) folder.  
   *(Note: If browsing manually, always select `PROFSAVEbf6mp_profile`, not the binary `PROFSAVEbf6mp` file.)*
4. Select your preferred **Buttons** preset, **Sticks** layout, **Invert Pitch** option, and **Sensitivity / Deadzone** values. Optionally, bind the buttons manually in the Controller Visualizer to suit your preferences.
5. Click **APPLY FLIGHT DECK PROFILE** and launch Battlefield 6!

---

## 100% Safe, Open-Source & Anti-Cheat Friendly

* **Zero Memory Or Game Binary Modification:** Flight Deck does not hook into the Battlefield 6 process, does not run in the background while you play, and does not touch binary `FBCHUNKS` files.
* **Standard Config Text Editing:** Flight Deck only edits standard `GstInput.*` lines inside your local `PROFSAVEbf6mp_profile` settings file, the exact same changes you can make manually in Notepad, or digging through the in-game menus.
* **Open Source:** Full C# (.NET 10) source code is publicly available on GitHub for complete transparency.

---

## Building from Source & Editing the UI Layout

### Prerequisites
- **Visual Studio 2026** (or the **.NET 10 SDK**) with the **.NET Desktop Development** (Windows Forms) workload installed.
- Zero external NuGet UI packages are required; all custom dark-theme controls (`FDThemeContainer`, `FDGroupBox`, `FDModernButton`, `FDSlider`, `FDControllerVisualizer`, etc.) are built directly into the project using native GDI+. (I'm old)

### Publishing as a Standalone Single-File Executable (`PublishSingleFile`)
To compile and bundle Flight Deck into a single, self-contained `.exe` with all native and managed dependencies embedded, run the following command in the project directory:

```powershell
dotnet publish "FlightDeck_BF6 Flight Configurator.csproj" -c Release -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --output ".\Publish"
```

### Editing Control Placement in Visual Studio Designer (Unlocking & Re-Locking Panels)
To keep the layout responsive when resizing the window, the main sections use nested layout containers (`TableLayoutPanel`, `FlowLayoutPanel`, and `Dock`/`Anchor` rules):

1. **Resizing Main Sections (`_root` TableLayoutPanel):**
   * The 7 horizontal sections of the app (`_grpProfile`, `_grpPreset`, `_grpInspector`, `_grpBackups`, `_pnlApplySection`, `_txtLog`, and `_pnlFooter`) are docked inside `_root` (`TableLayoutPanel`) with `Dock = DockStyle.Fill`.
   * **To resize a section's height:** Select `_root` in the Visual Studio Designer (or via the **Document Outline** window: `Ctrl + Alt + T`), click the small arrow `[>]` in the top-right corner of `_root`, and select **Edit Rows and Columns...** to adjust row heights.
   * **To freely drag a docked control inside a panel:** Select the control, open the **Properties** window (`F4`), and change `Dock` from `Fill` (or `Right`/`Left`) to **`None`**. If a control's `Locked` property is set to `True`, set `Locked = False`.
2. **Editing Section 2 Controls (Presets, Sliders & Buttons):**
   * All dropdowns, checkboxes, `FDSlider` controls, `NumericUpDown` inputs, and action buttons (`Import Preset`, `Export Preset`, `Delete`) inside `_pnlPresetContent` use standard `Location (X, Y)` coordinates and can be dragged and resized directly on the Designer canvas.
   * Note that `_btnImportPreset`, `_btnExportPreset`, and `_btnDeletePreset` use `Anchor = Top, Right` so they stay pinned to the right edge when the window is widened.
3. **Editing Inspector Views (`_padVisualizer` & `_lvBindings`):**
   * Both `_padVisualizer` (Visual Controller Map) and `_lvBindings` share `_pnlInspectorBody` with `Dock = DockStyle.Fill`. To edit, remove the DockStyle.Fill, and after editing, set it back to DockStyle.Fill.
4. **Editing Backup & Footer Buttons (`FlowLayoutPanel`):**
   * Controls inside `_pnlBackupActions` and `_pnlFooterLinks` are arranged automatically by a `FlowLayoutPanel`.
   * To reorder them, drag them within the Designer or reorder them in the **Document Outline** (`Ctrl + Alt + T`). To adjust spacing between buttons, edit each control's `Margin` property.
5. **Re-Locking Controls After Editing:**
   * Once you finish customizing the layout, restore `Dock = DockStyle.Fill` on container panels (or right-click and choose **Lock Controls**) so the window maintains proper alignment and scaling.

---

## Links & Support

If Flight Deck helped you get back in the air with proper controls, please consider starring the repo / endorsing the mod, and consider donating!

* **Void Tech Studios Website:** [https://voidtechstudios.us/](https://voidtechstudios.us/)
* Download from NexusMods: https://www.nexusmods.com/battlefield6/mods/25
* **Support Development (Donate):** [Buy Me a Coffee ☕ (buymeacoffee.com/rustforgedev)](https://buymeacoffee.com/rustforgedev)
