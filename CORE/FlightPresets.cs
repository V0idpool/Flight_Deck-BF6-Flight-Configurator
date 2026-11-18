using System;
using System.Collections.Generic;
using System.Globalization;
using static FlightDeck_BF6_Flight_Configurator.CORE.EngineBindings;

namespace FlightDeck_BF6_Flight_Configurator.CORE
{
    public class FlightPresets
    {
        public enum FlightPresetType
        {
            Bf4Veteran,
            Bf3Classic,
            Bf2042Alternate,
            Bf6StockReset
        }

        public enum StickLayoutType
        {
            Default = 0,
            Legacy = 1,
            Southpaw = 2,
            LegacySouthpaw = 3
        }

        public sealed class FlightScheme
        {
            public string Title { get; set; } = string.Empty;
            public string Subtitle { get; set; } = string.Empty;
            public string? SourceFilePath { get; set; }
            public bool IsCustom { get; set; }
            public bool HasCustomHeliBinds { get; set; }
            public bool HasCustomJetBinds { get; set; }

            public bool HasCustomButtonBinds
            {
                get => HasCustomHeliBinds || HasCustomJetBinds;
                set
                {
                    HasCustomHeliBinds = value;
                    HasCustomJetBinds = value;
                }
            }

            public FlightPresetType? BasePresetType { get; set; }
            public Dictionary<string, string> RawVariables { get; set; } = new(StringComparer.Ordinal);

            public override string ToString() => Title;

            public FlightScheme AddGstInput(string key, string value)
            {
                RawVariables[key] = value;
                return this;
            }

            public FlightScheme EnsureBaselineTables()
            {
                foreach (var (k, v) in GetDefaultKeyBindingTable("helicopter"))
                {
                    if (!RawVariables.ContainsKey(k))
                        RawVariables[k] = v;
                }
                foreach (var (k, v) in GetDefaultKeyBindingTable("jet"))
                {
                    if (!RawVariables.ContainsKey(k))
                        RawVariables[k] = v;
                }
                RawVariables["GstKeyBinding.version"] = "2000007";
                return this;
            }

            public FlightScheme BindAction(string domain, string actionKey, PadBind bind)
            {
                string normDomain = domain.Equals("heli", StringComparison.OrdinalIgnoreCase) ? "helicopter" : domain;
                if (!ActionSlots.TryGetValue(actionKey, out var slots))
                    return this;

                foreach (var slot in slots)
                {
                    if (normDomain == "jet" && slot.Concept == "ConceptNextPosition")
                        continue;
                    if (normDomain == "helicopter" && slot.Concept == "ConceptSprint")
                        continue;

                    int neg = (slot.Concept == "ConceptBrake" || slot.Concept == "ConceptPitch") && !bind.IsStickAxis
                        ? slot.DefaultNegate
                        : bind.Negate;

                    var actualBind = new PadBind(bind.Axis, bind.Button, neg, DeviceType.Controller);
                    foreach (var kvp in actualBind.ToProfSaveLines(normDomain, slot.Concept, slot.SlotIndex))
                        RawVariables[kvp.Key] = kvp.Value;
                }
                return this;
            }

            public FlightScheme BindHeliAction(string actionKey, PadBind bind) =>
                BindAction("helicopter", actionKey, bind);

            public FlightScheme BindJetAction(string actionKey, PadBind bind) =>
                BindAction("jet", actionKey, bind);

            public FlightScheme Clone()
            {
                var copy = new FlightScheme
                {
                    Title = Title,
                    Subtitle = Subtitle,
                    SourceFilePath = SourceFilePath,
                    IsCustom = IsCustom,
                    HasCustomHeliBinds = HasCustomHeliBinds,
                    HasCustomJetBinds = HasCustomJetBinds,
                    BasePresetType = BasePresetType
                };
                foreach (var (k, v) in RawVariables)
                    copy.RawVariables[k] = v;
                return copy;
            }
        }

        public static class FlightSchemeRegistry
        {
            public static readonly List<FlightScheme> CustomSchemes = new();

            public static readonly IReadOnlyDictionary<FlightPresetType, FlightScheme> Schemes =
                new Dictionary<FlightPresetType, FlightScheme>
                {
                    [FlightPresetType.Bf4Veteran] = BuildBf4Veteran(),
                    [FlightPresetType.Bf3Classic] = BuildBf3Classic(),
                    [FlightPresetType.Bf2042Alternate] = BuildBf2042Alternate(),
                    [FlightPresetType.Bf6StockReset] = BuildBf6Stock()
                };

            public static FlightScheme BuildConfiguredScheme(
                FlightScheme templateScheme,
                StickLayoutType stickLayout,
                bool invertPitch,
                bool applyTuningOverrides,
                decimal heliSens,
                decimal jetSens,
                decimal centerDeadzone)
            {
                FlightScheme baseScheme = templateScheme.Clone();
                baseScheme.EnsureBaselineTables();

                string flipVal = invertPitch ? "1" : "0";
                baseScheme.AddGstInput("GstInput.Scheme2FlipY", flipVal)
                          .AddGstInput("GstInput.Scheme5FlipY", flipVal)
                          .AddGstInput("GstInput.Scheme1FlipY", "0")
                          .AddGstInput("GstInput.Scheme3FlipY", "0")
                          .AddGstInput("GstInput.Scheme4FlipY", "0");

                string stickVal = ((int)stickLayout).ToString(CultureInfo.InvariantCulture);
                baseScheme.AddGstInput("GstInput.Scheme2Sticks", stickVal)
                          .AddGstInput("GstInput.Scheme5Sticks", stickVal);

                bool triggersAreThrottle = !baseScheme.RawVariables.TryGetValue("GstInput.Scheme5Buttons", out string? btnMode) ||
                                           btnMode.Trim() != "0";

                ApplyStickAxes(baseScheme, stickLayout, triggersAreThrottle, invertPitch);

                if (applyTuningOverrides)
                {
                    string hSens = heliSens.ToString("0.000000", CultureInfo.InvariantCulture);
                    string jSens = jetSens.ToString("0.000000", CultureInfo.InvariantCulture);
                    string dzVal = centerDeadzone.ToString("0.000000", CultureInfo.InvariantCulture);

                    baseScheme.AddGstInput("GstInput.VehicleSensitivityHelicopterPilot", hSens)
                              .AddGstInput("GstInput.VehicleSensitivityPlanePilot", jSens)
                              .AddGstInput("GstInput.Scheme5Sensitivity", hSens)
                              .AddGstInput("GstInput.Scheme2Sensitivity", jSens)
                              .AddGstInput("GstInput.GamepadLeftStickDeadzoneCenter", dzVal)
                              .AddGstInput("GstInput.GamepadRightStickDeadzoneCenter", dzVal);
                }

                return baseScheme;
            }

            public static void ApplyStickAxes(FlightScheme scheme, StickLayoutType stickLayout, bool triggersAreThrottle, bool invertPitch = true)
            {
                int pitchNeg = invertPitch ? 1 : 0;
                PadBind lsPitch = new(1, 60, pitchNeg);
                PadBind rsPitch = new(10, 60, pitchNeg);

                (PadBind yaw, PadBind pitch, PadBind roll, PadBind throttleAxis) = stickLayout switch
                {
                    StickLayoutType.Legacy => (
                        yaw: PadInput.RightStickX_Pos,
                        pitch: rsPitch,
                        roll: PadInput.LeftStickX_Pos,
                        throttleAxis: PadInput.LeftStickY_Pos
                    ),
                    StickLayoutType.Southpaw => (
                        yaw: PadInput.RightStickX_Pos,
                        pitch: lsPitch,
                        roll: PadInput.LeftStickX_Pos,
                        throttleAxis: PadInput.RightStickY_Pos
                    ),
                    StickLayoutType.LegacySouthpaw => (
                        yaw: PadInput.LeftStickX_Pos,
                        pitch: lsPitch,
                        roll: PadInput.RightStickX_Pos,
                        throttleAxis: PadInput.RightStickY_Pos
                    ),
                    _ => (
                        yaw: PadInput.LeftStickX_Pos,
                        pitch: rsPitch,
                        roll: PadInput.RightStickX_Pos,
                        throttleAxis: PadInput.LeftStickY_Pos
                    )
                };

                scheme.BindHeliAction("Yaw", yaw)
                      .BindHeliAction("Pitch", pitch)
                      .BindHeliAction("Roll", roll)
                      .BindJetAction("Yaw", yaw)
                      .BindJetAction("Pitch", pitch)
                      .BindJetAction("Roll", roll);

                if (!scheme.HasCustomHeliBinds)
                {
                    if (triggersAreThrottle)
                    {
                        scheme.BindHeliAction("Ascend", PadInput.RightTrigger_RT)
                              .BindHeliAction("Descend", PadInput.LeftTrigger_LT)
                              .BindHeliAction("ThrottleAxis", PadInput.Unbound);
                    }
                    else
                    {
                        scheme.BindHeliAction("ThrottleAxis", throttleAxis)
                              .BindHeliAction("Ascend", PadInput.RightBumper_RB)
                              .BindHeliAction("Descend", PadInput.LeftBumper_LB);
                    }
                }
                else if (IsThrottleBoundToStick(scheme.RawVariables, "helicopter"))
                {
                    scheme.BindHeliAction("ThrottleAxis", throttleAxis);
                }

                if (!scheme.HasCustomJetBinds)
                {
                    if (triggersAreThrottle)
                    {
                        scheme.BindJetAction("Ascend", PadInput.RightTrigger_RT)
                              .BindJetAction("Descend", PadInput.LeftTrigger_LT)
                              .BindJetAction("ThrottleAxis", PadInput.Unbound);
                    }
                    else
                    {
                        scheme.BindJetAction("ThrottleAxis", throttleAxis)
                              .BindJetAction("Ascend", PadInput.RightBumper_RB)
                              .BindJetAction("Descend", PadInput.LeftBumper_LB);
                    }
                }
                else if (IsThrottleBoundToStick(scheme.RawVariables, "jet"))
                {
                    scheme.BindJetAction("ThrottleAxis", throttleAxis);
                }
            }

            private static bool IsThrottleBoundToStick(IReadOnlyDictionary<string, string> vars, string domain)
            {
                string axisKey = $"GstKeyBinding.{domain}.ConceptMoveFB.2.axis";
                string btnKey = $"GstKeyBinding.{domain}.ConceptMoveFB.2.button";
                return vars.TryGetValue(axisKey, out string? sAxis) &&
                       vars.TryGetValue(btnKey, out string? sBtn) &&
                       int.TryParse(sAxis, out int axis) &&
                       int.TryParse(sBtn, out int btn) &&
                       axis != 56 && btn == 60;
            }

            private static FlightScheme BuildBf4Veteran()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 4 | Veteran Flight",
                    Subtitle = "RT/LT Collective & Throttle | LS Yaw | RS Pitch/Roll | LB Fire | RB Countermeasures | Zero Flight Assist",
                    BasePresetType = FlightPresetType.Bf4Veteran
                }.EnsureBaselineTables();

                scheme.AddGstInput("GstInput.OptionHelicopterAssist", "0")
                      .AddGstInput("GstInput.OptionPlaneAssist", "0")
                      .AddGstInput("GstInput.OptionPlaneTerrainAvoidance", "0")
                      .AddGstInput("GstInput.OptionAirplanePilotControl", "1")
                      .AddGstInput("GstInput.PlaneFreelookDecouple", "1")
                      .AddGstInput("GstInput.VehicleAimRelativeControl", "0")
                      .AddGstInput("GstInput.VehicleDecoupleAimingController", "1")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicle", "2")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicleZoom", "2")
                      .AddGstInput("GstInput.ControllerAccelerationPresetsVehicle", "2")
                      .AddGstInput("GstInput.StickAimingAccelerationAmountVehicle", "0.000000")
                      .AddGstInput("GstInput.Scheme2Buttons", "1")
                      .AddGstInput("GstInput.Scheme2Sticks", "0")
                      .AddGstInput("GstInput.Scheme2FlipY", "1")
                      .AddGstInput("GstInput.Scheme2FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme5Buttons", "1")
                      .AddGstInput("GstInput.Scheme5Sticks", "0")
                      .AddGstInput("GstInput.Scheme5FlipY", "1")
                      .AddGstInput("GstInput.Scheme1Buttons", "0")
                      .AddGstInput("GstInput.Scheme1Sticks", "0")
                      .AddGstInput("GstInput.Scheme1FlipY", "0")
                      .AddGstInput("GstInput.Scheme1FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme3Buttons", "0")
                      .AddGstInput("GstInput.Scheme3Sticks", "0")
                      .AddGstInput("GstInput.Scheme3FlipY", "0")
                      .AddGstInput("GstInput.Scheme4Buttons", "0")
                      .AddGstInput("GstInput.Scheme4Sticks", "0")
                      .AddGstInput("GstInput.Scheme4FlipY", "0");

                scheme.BindHeliAction("Ascend", PadInput.RightTrigger_RT)
                      .BindHeliAction("Descend", PadInput.LeftTrigger_LT)
                      .BindHeliAction("ThrottleAxis", PadInput.Unbound)
                      .BindHeliAction("Fire", PadInput.LeftBumper_LB)
                      .BindHeliAction("Countermeasures", PadInput.RightBumper_RB)
                      .BindHeliAction("Zoom", PadInput.RightStickClick)
                      .BindHeliAction("ChaseCam", PadInput.LeftStickClick)
                      .BindHeliAction("Freelook", PadInput.DPadUp)
                      .BindHeliAction("Ping", PadInput.DPadDown)
                      .BindHeliAction("RepairGadget2", PadInput.DPadRight)
                      .BindHeliAction("SwapWeapon", PadInput.ButtonY_Tri)
                      .BindHeliAction("ReloadExit", PadInput.ButtonX_Square)
                      .BindHeliAction("SwitchSeat", PadInput.ButtonA_Cross);

                scheme.BindJetAction("Ascend", PadInput.RightTrigger_RT)
                      .BindJetAction("Descend", PadInput.LeftTrigger_LT)
                      .BindJetAction("ThrottleAxis", PadInput.Unbound)
                      .BindJetAction("Afterburner", PadInput.LeftStickClick)
                      .BindJetAction("Fire", PadInput.LeftBumper_LB)
                      .BindJetAction("Countermeasures", PadInput.RightBumper_RB)
                      .BindJetAction("Zoom", PadInput.RightStickClick)
                      .BindJetAction("Freelook", PadInput.DPadDown)
                      .BindJetAction("ChaseCam", PadInput.DPadUp)
                      .BindJetAction("RepairGadget2", PadInput.DPadRight)
                      .BindJetAction("SwapWeapon", PadInput.ButtonY_Tri)
                      .BindJetAction("ReloadExit", PadInput.ButtonX_Square);

                return scheme;
            }

            private static FlightScheme BuildBf3Classic()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 3 | Classic Alternate",
                    Subtitle = "RT/LT Throttle | LB Fire | RB Countermeasures | Linear 1:1 BF3 Stick Response | Zero Assist",
                    BasePresetType = FlightPresetType.Bf3Classic
                }.EnsureBaselineTables();

                scheme.AddGstInput("GstInput.OptionHelicopterAssist", "0")
                      .AddGstInput("GstInput.OptionPlaneAssist", "0")
                      .AddGstInput("GstInput.OptionPlaneTerrainAvoidance", "0")
                      .AddGstInput("GstInput.OptionAirplanePilotControl", "1")
                      .AddGstInput("GstInput.PlaneFreelookDecouple", "1")
                      .AddGstInput("GstInput.VehicleAimRelativeControl", "0")
                      .AddGstInput("GstInput.VehicleDecoupleAimingController", "1")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicle", "1")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicleZoom", "1")
                      .AddGstInput("GstInput.ControllerAccelerationPresetsVehicle", "1")
                      .AddGstInput("GstInput.StickAimingAccelerationAmountVehicle", "0.000000")
                      .AddGstInput("GstInput.Scheme2Buttons", "1")
                      .AddGstInput("GstInput.Scheme2Sticks", "0")
                      .AddGstInput("GstInput.Scheme2FlipY", "1")
                      .AddGstInput("GstInput.Scheme2FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme5Buttons", "1")
                      .AddGstInput("GstInput.Scheme5Sticks", "0")
                      .AddGstInput("GstInput.Scheme5FlipY", "1")
                      .AddGstInput("GstInput.Scheme1Buttons", "0")
                      .AddGstInput("GstInput.Scheme1Sticks", "0")
                      .AddGstInput("GstInput.Scheme1FlipY", "0")
                      .AddGstInput("GstInput.Scheme1FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme3Buttons", "0")
                      .AddGstInput("GstInput.Scheme3Sticks", "0")
                      .AddGstInput("GstInput.Scheme3FlipY", "0")
                      .AddGstInput("GstInput.Scheme4Buttons", "0")
                      .AddGstInput("GstInput.Scheme4Sticks", "0")
                      .AddGstInput("GstInput.Scheme4FlipY", "0");

                scheme.BindHeliAction("Ascend", PadInput.RightTrigger_RT)
                      .BindHeliAction("Descend", PadInput.LeftTrigger_LT)
                      .BindHeliAction("ThrottleAxis", PadInput.Unbound)
                      .BindHeliAction("Fire", PadInput.LeftBumper_LB)
                      .BindHeliAction("Countermeasures", PadInput.RightBumper_RB)
                      .BindHeliAction("Zoom", PadInput.DPadUp)
                      .BindHeliAction("Freelook", PadInput.DPadDown)
                      .BindHeliAction("ChaseCam", PadInput.LeftStickClick)
                      .BindHeliAction("SwapWeapon", PadInput.ButtonY_Tri)
                      .BindHeliAction("ReloadExit", PadInput.ButtonX_Square)
                      .BindHeliAction("SwitchSeat", PadInput.ButtonA_Cross);

                scheme.BindJetAction("Ascend", PadInput.RightTrigger_RT)
                      .BindJetAction("Descend", PadInput.LeftTrigger_LT)
                      .BindJetAction("ThrottleAxis", PadInput.Unbound)
                      .BindJetAction("Afterburner", PadInput.LeftStickClick)
                      .BindJetAction("Fire", PadInput.LeftBumper_LB)
                      .BindJetAction("Countermeasures", PadInput.RightBumper_RB)
                      .BindJetAction("Zoom", PadInput.RightStickClick)
                      .BindJetAction("Freelook", PadInput.DPadDown)
                      .BindJetAction("ChaseCam", PadInput.DPadUp)
                      .BindJetAction("SwapWeapon", PadInput.ButtonY_Tri)
                      .BindJetAction("ReloadExit", PadInput.ButtonX_Square);

                return scheme;
            }

            private static FlightScheme BuildBf2042Alternate()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 2042 | Alternate",
                    Subtitle = "RT/LT Ascend & Descend | LB Fire | RB Zoom/Spot | D-Pad Left Flares | 2042 Curves",
                    BasePresetType = FlightPresetType.Bf2042Alternate
                }.EnsureBaselineTables();

                scheme.AddGstInput("GstInput.OptionHelicopterAssist", "0")
                      .AddGstInput("GstInput.OptionPlaneAssist", "0")
                      .AddGstInput("GstInput.OptionPlaneTerrainAvoidance", "0")
                      .AddGstInput("GstInput.OptionAirplanePilotControl", "1")
                      .AddGstInput("GstInput.PlaneFreelookDecouple", "1")
                      .AddGstInput("GstInput.VehicleAimRelativeControl", "0")
                      .AddGstInput("GstInput.VehicleDecoupleAimingController", "0")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicle", "3")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicleZoom", "3")
                      .AddGstInput("GstInput.ControllerAccelerationPresetsVehicle", "3")
                      .AddGstInput("GstInput.StickAimingAccelerationAmountVehicle", "0.500000")
                      .AddGstInput("GstInput.Scheme2Buttons", "1")
                      .AddGstInput("GstInput.Scheme2Sticks", "0")
                      .AddGstInput("GstInput.Scheme2FlipY", "1")
                      .AddGstInput("GstInput.Scheme2FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme5Buttons", "1")
                      .AddGstInput("GstInput.Scheme5Sticks", "0")
                      .AddGstInput("GstInput.Scheme5FlipY", "1")
                      .AddGstInput("GstInput.Scheme1Buttons", "0")
                      .AddGstInput("GstInput.Scheme1Sticks", "0")
                      .AddGstInput("GstInput.Scheme1FlipY", "0")
                      .AddGstInput("GstInput.Scheme1FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme3Buttons", "0")
                      .AddGstInput("GstInput.Scheme3Sticks", "0")
                      .AddGstInput("GstInput.Scheme3FlipY", "0")
                      .AddGstInput("GstInput.Scheme4Buttons", "0")
                      .AddGstInput("GstInput.Scheme4Sticks", "0")
                      .AddGstInput("GstInput.Scheme4FlipY", "0");

                scheme.BindHeliAction("Ascend", PadInput.RightTrigger_RT)
                      .BindHeliAction("Descend", PadInput.LeftTrigger_LT)
                      .BindHeliAction("ThrottleAxis", PadInput.Unbound)
                      .BindHeliAction("Fire", PadInput.LeftBumper_LB)
                      .BindHeliAction("Zoom", PadInput.RightBumper_RB)
                      .BindHeliAction("Countermeasures", PadInput.DPadLeft)
                      .BindHeliAction("Freelook", PadInput.DPadUp)
                      .BindHeliAction("ChaseCam", PadInput.RightStickClick)
                      .BindHeliAction("SwapWeapon", PadInput.ButtonY_Tri)
                      .BindHeliAction("ReloadExit", PadInput.ButtonX_Square)
                      .BindHeliAction("SwitchSeat", PadInput.ButtonA_Cross);

                scheme.BindJetAction("Ascend", PadInput.RightTrigger_RT)
                      .BindJetAction("Descend", PadInput.LeftTrigger_LT)
                      .BindJetAction("ThrottleAxis", PadInput.Unbound)
                      .BindJetAction("Afterburner", PadInput.LeftStickClick)
                      .BindJetAction("Fire", PadInput.LeftBumper_LB)
                      .BindJetAction("Zoom", PadInput.RightBumper_RB)
                      .BindJetAction("Countermeasures", PadInput.DPadLeft)
                      .BindJetAction("Freelook", PadInput.DPadDown)
                      .BindJetAction("ChaseCam", PadInput.RightStickClick)
                      .BindJetAction("SwapWeapon", PadInput.ButtonY_Tri)
                      .BindJetAction("ReloadExit", PadInput.ButtonX_Square);

                return scheme;
            }

            private static FlightScheme BuildBf6Stock()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 6 - Stock Default",
                    Subtitle = "Restores stock Battlefield 6 flight settings and default controls",
                    BasePresetType = FlightPresetType.Bf6StockReset
                }.EnsureBaselineTables();

                scheme.AddGstInput("GstInput.OptionHelicopterAssist", "1")
                      .AddGstInput("GstInput.OptionPlaneAssist", "0")
                      .AddGstInput("GstInput.OptionPlaneTerrainAvoidance", "1")
                      .AddGstInput("GstInput.OptionAirplanePilotControl", "1")
                      .AddGstInput("GstInput.PlaneFreelookDecouple", "0")
                      .AddGstInput("GstInput.VehicleAimRelativeControl", "0")
                      .AddGstInput("GstInput.VehicleDecoupleAimingController", "0")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicle", "0")
                      .AddGstInput("GstInput.ControllerAimingCurveVehicleZoom", "0")
                      .AddGstInput("GstInput.ControllerAccelerationPresetsVehicle", "0")
                      .AddGstInput("GstInput.StickAimingAccelerationAmountVehicle", "0.700000")
                      .AddGstInput("GstInput.Scheme1Buttons", "0")
                      .AddGstInput("GstInput.Scheme1Sticks", "0")
                      .AddGstInput("GstInput.Scheme1FlipY", "0")
                      .AddGstInput("GstInput.Scheme1FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme2Buttons", "0")
                      .AddGstInput("GstInput.Scheme2Sticks", "0")
                      .AddGstInput("GstInput.Scheme2FlipY", "1")
                      .AddGstInput("GstInput.Scheme2FlipCameraY", "0")
                      .AddGstInput("GstInput.Scheme3Buttons", "0")
                      .AddGstInput("GstInput.Scheme3Sticks", "0")
                      .AddGstInput("GstInput.Scheme3FlipY", "0")
                      .AddGstInput("GstInput.Scheme4Buttons", "0")
                      .AddGstInput("GstInput.Scheme4Sticks", "0")
                      .AddGstInput("GstInput.Scheme4FlipY", "0")
                      .AddGstInput("GstInput.Scheme5Buttons", "0")
                      .AddGstInput("GstInput.Scheme5Sticks", "0")
                      .AddGstInput("GstInput.Scheme5FlipY", "1");

                return scheme;
            }
        }
    }
}