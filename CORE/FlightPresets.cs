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
            Bf6StockReset,
            CustomImported
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
            public Dictionary<string, string> RawVariables { get; set; } = new(StringComparer.Ordinal);

            public FlightScheme AddGstInput(string key, string value)
            {
                RawVariables[key] = value;
                return this;
            }

            public FlightScheme BindHeli(string concept, PadBind bind, int controllerSlot = 0)
            {
                foreach (var kvp in bind.ToProfSaveLines("heli", concept, controllerSlot))
                    RawVariables[kvp.Key] = kvp.Value;
                return this;
            }

            public FlightScheme BindJet(string concept, PadBind bind, int controllerSlot = 0)
            {
                foreach (var kvp in bind.ToProfSaveLines("jet", concept, controllerSlot))
                    RawVariables[kvp.Key] = kvp.Value;
                return this;
            }

            public FlightScheme Clone()
            {
                var copy = new FlightScheme
                {
                    Title = Title,
                    Subtitle = Subtitle
                };
                foreach (var (k, v) in RawVariables)
                    copy.RawVariables[k] = v;
                return copy;
            }
        }

        public static class FlightSchemeRegistry
        {
            public static FlightScheme? CustomLoadedScheme { get; set; }

            public static readonly IReadOnlyDictionary<FlightPresetType, FlightScheme> Schemes =
                new Dictionary<FlightPresetType, FlightScheme>
                {
                    [FlightPresetType.Bf4Veteran] = BuildBf4Veteran(),
                    [FlightPresetType.Bf3Classic] = BuildBf3Classic(),
                    [FlightPresetType.Bf2042Alternate] = BuildBf2042Alternate(),
                    [FlightPresetType.Bf6StockReset] = BuildBf6Stock()
                };

            public static FlightScheme BuildConfiguredScheme(
                FlightPresetType presetType,
                StickLayoutType stickLayout,
                bool invertPitch,
                bool applyTuningOverrides,
                decimal heliSens,
                decimal jetSens,
                decimal centerDeadzone)
            {
                FlightScheme baseScheme = (presetType == FlightPresetType.CustomImported && CustomLoadedScheme is not null)
                    ? CustomLoadedScheme.Clone()
                    : Schemes.TryGetValue(presetType, out var found)
                        ? found.Clone()
                        : Schemes[FlightPresetType.Bf4Veteran].Clone();

                string flipVal = invertPitch ? "1" : "0";
                baseScheme.AddGstInput("GstInput.Scheme2FlipY", flipVal)
                          .AddGstInput("GstInput.Scheme5FlipY", flipVal)
                          .AddGstInput("GstInput.Scheme1FlipY", "0")
                          .AddGstInput("GstInput.Scheme3FlipY", "0")
                          .AddGstInput("GstInput.Scheme4FlipY", "0");

                string stickVal = ((int)stickLayout).ToString(CultureInfo.InvariantCulture);
                baseScheme.AddGstInput("GstInput.Scheme2Sticks", stickVal)
                          .AddGstInput("GstInput.Scheme5Sticks", stickVal);

                bool triggersAreThrottle = presetType != FlightPresetType.Bf6StockReset;
                ApplyStickAxes(baseScheme, stickLayout, triggersAreThrottle);

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

            private static void ApplyStickAxes(FlightScheme scheme, StickLayoutType stickLayout, bool triggersAreThrottle)
            {
                (PadBind yaw, PadBind pitch, PadBind roll, PadBind throttleUp, PadBind throttleDown) = stickLayout switch
                {
                    StickLayoutType.Legacy => (
                        yaw: PadInput.RightStickX_Pos,
                        pitch: PadInput.RightStickY_Pos,
                        roll: PadInput.LeftStickX_Pos,
                        throttleUp: PadInput.LeftStickY_Pos,
                        throttleDown: PadInput.LeftStickY_Neg
                    ),
                    StickLayoutType.Southpaw => (
                        yaw: PadInput.RightStickX_Pos,
                        pitch: PadInput.LeftStickY_Pos,
                        roll: PadInput.LeftStickX_Pos,
                        throttleUp: PadInput.RightStickY_Pos,
                        throttleDown: PadInput.RightStickY_Neg
                    ),
                    StickLayoutType.LegacySouthpaw => (
                        yaw: PadInput.LeftStickX_Pos,
                        pitch: PadInput.LeftStickY_Pos,
                        roll: PadInput.RightStickX_Pos,
                        throttleUp: PadInput.RightStickY_Pos,
                        throttleDown: PadInput.RightStickY_Neg
                    ),
                    _ => (
                        yaw: PadInput.LeftStickX_Pos,
                        pitch: PadInput.RightStickY_Pos,
                        roll: PadInput.RightStickX_Pos,
                        throttleUp: PadInput.LeftStickY_Pos,
                        throttleDown: PadInput.LeftStickY_Neg
                    )
                };

                scheme.BindHeli("ConceptYaw", yaw)
                      .BindHeli("ConceptPitch", pitch)
                      .BindHeli("ConceptRoll", roll)
                      .BindJet("ConceptYaw", yaw)
                      .BindJet("ConceptPitch", pitch)
                      .BindJet("ConceptRoll", roll);

                if (!triggersAreThrottle)
                {
                    scheme.BindHeli("ConceptMoveFB", throttleUp)
                          .BindHeli("ConceptBrake", throttleDown)
                          .BindJet("ConceptMoveFB", throttleUp)
                          .BindJet("ConceptBrake", throttleDown);
                }
            }

            private static FlightScheme BuildBf4Veteran()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 4 — Veteran Flight",
                    Subtitle = "RT/LT Collective & Throttle | LS Yaw | RS Pitch/Roll | RB Fire | LB Zoom/Flares | Zero Flight Assist"
                };

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

                scheme.BindHeli("ConceptMoveFB", PadInput.RightTrigger_RT)
                      .BindHeli("ConceptBrake", PadInput.LeftTrigger_LT)
                      .BindHeli("ConceptCrawl", PadInput.LeftTrigger_LT)
                      .BindHeli("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindHeli("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindHeli("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindHeli("ConceptFire", PadInput.RightBumper_RB)
                      .BindHeli("ConceptActivateCountermeasures", PadInput.LeftBumper_LB)
                      .BindHeli("ConceptZoom", PadInput.RightStickClick)
                      .BindHeli("ConceptFreeCamera", PadInput.DPadUp)
                      .BindHeli("ConceptCameraSwitch", PadInput.LeftStickClick)
                      .BindHeli("ConceptSelectInventoryItem1", PadInput.ButtonY_Tri)
                      .BindHeli("ConceptReload", PadInput.ButtonX_Square)
                      .BindHeli("ConceptExit", PadInput.ButtonX_Square)
                      .BindHeli("ConceptSwitchToPrimaryWeapon", PadInput.ButtonA_Cross);

                scheme.BindJet("ConceptMoveFB", PadInput.RightTrigger_RT)
                      .BindJet("ConceptBrake", PadInput.LeftTrigger_LT)
                      .BindJet("ConceptSprint", PadInput.LeftStickClick)
                      .BindJet("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindJet("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindJet("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindJet("ConceptFire", PadInput.RightBumper_RB)
                      .BindJet("ConceptActivateCountermeasures", PadInput.LeftBumper_LB)
                      .BindJet("ConceptZoom", PadInput.RightStickClick)
                      .BindJet("ConceptFreeCamera", PadInput.DPadDown)
                      .BindJet("ConceptCameraSwitch", PadInput.DPadUp)
                      .BindJet("ConceptSelectInventoryItem1", PadInput.ButtonY_Tri);

                return scheme;
            }

            private static FlightScheme BuildBf3Classic()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 3 — Classic Alternate",
                    Subtitle = "RT/LT Throttle | Bumpers Fire/Zoom | Linear 1:1 BF3 Stick Response | Zero Assist"
                };

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

                scheme.BindHeli("ConceptMoveFB", PadInput.RightTrigger_RT)
                      .BindHeli("ConceptBrake", PadInput.LeftTrigger_LT)
                      .BindHeli("ConceptCrawl", PadInput.LeftTrigger_LT)
                      .BindHeli("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindHeli("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindHeli("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindHeli("ConceptFire", PadInput.LeftBumper_LB)
                      .BindHeli("ConceptActivateCountermeasures", PadInput.RightStickClick)
                      .BindHeli("ConceptZoom", PadInput.DPadUp)
                      .BindHeli("ConceptCameraSwitch", PadInput.LeftStickClick)
                      .BindHeli("ConceptSelectInventoryItem1", PadInput.ButtonY_Tri);

                scheme.BindJet("ConceptMoveFB", PadInput.RightTrigger_RT)
                      .BindJet("ConceptBrake", PadInput.LeftTrigger_LT)
                      .BindJet("ConceptSprint", PadInput.LeftStickClick)
                      .BindJet("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindJet("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindJet("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindJet("ConceptFire", PadInput.LeftBumper_LB)
                      .BindJet("ConceptActivateCountermeasures", PadInput.RightStickClick)
                      .BindJet("ConceptCameraSwitch", PadInput.DPadUp)
                      .BindJet("ConceptSelectInventoryItem1", PadInput.ButtonY_Tri);

                return scheme;
            }

            private static FlightScheme BuildBf2042Alternate()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 2042 - Alternate",
                    Subtitle = "RT/LT Ascend & Descend | RB Fire | LB Zoom | D-Pad Left Flares | 2042 Curves"
                };

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

                scheme.BindHeli("ConceptMoveFB", PadInput.RightTrigger_RT)
                      .BindHeli("ConceptBrake", PadInput.LeftTrigger_LT)
                      .BindHeli("ConceptCrawl", PadInput.LeftTrigger_LT)
                      .BindHeli("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindHeli("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindHeli("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindHeli("ConceptFire", PadInput.RightBumper_RB)
                      .BindHeli("ConceptZoom", PadInput.LeftBumper_LB)
                      .BindHeli("ConceptActivateCountermeasures", PadInput.DPadLeft)
                      .BindHeli("ConceptCameraSwitch", PadInput.RightStickClick);

                scheme.BindJet("ConceptMoveFB", PadInput.RightTrigger_RT)
                      .BindJet("ConceptBrake", PadInput.LeftTrigger_LT)
                      .BindJet("ConceptSprint", PadInput.LeftStickClick)
                      .BindJet("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindJet("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindJet("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindJet("ConceptFire", PadInput.RightBumper_RB)
                      .BindJet("ConceptZoom", PadInput.LeftBumper_LB)
                      .BindJet("ConceptActivateCountermeasures", PadInput.DPadLeft)
                      .BindJet("ConceptCameraSwitch", PadInput.RightStickClick);

                return scheme;
            }

            private static FlightScheme BuildBf6Stock()
            {
                var scheme = new FlightScheme
                {
                    Title = "Battlefield 6 — Stock Default",
                    Subtitle = "Restores stock Battlefield 6 flight settings and default controls"
                };

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

                scheme.BindHeli("ConceptMoveFB", PadInput.LeftStickY_Pos)
                      .BindHeli("ConceptBrake", PadInput.LeftStickY_Neg)
                      .BindHeli("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindHeli("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindHeli("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindHeli("ConceptFire", PadInput.RightTrigger_RT)
                      .BindHeli("ConceptZoom", PadInput.LeftTrigger_LT)
                      .BindHeli("ConceptActivateCountermeasures", PadInput.DPadLeft);

                scheme.BindJet("ConceptMoveFB", PadInput.LeftStickY_Pos)
                      .BindJet("ConceptBrake", PadInput.LeftStickY_Neg)
                      .BindJet("ConceptYaw", PadInput.LeftStickX_Pos)
                      .BindJet("ConceptPitch", PadInput.RightStickY_Pos)
                      .BindJet("ConceptRoll", PadInput.RightStickX_Pos)
                      .BindJet("ConceptFire", PadInput.RightTrigger_RT)
                      .BindJet("ConceptZoom", PadInput.LeftTrigger_LT)
                      .BindJet("ConceptActivateCountermeasures", PadInput.DPadLeft);

                return scheme;
            }
        }
    }
}