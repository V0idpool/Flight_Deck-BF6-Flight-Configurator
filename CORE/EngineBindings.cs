using System;
using System.Collections.Generic;

namespace FlightDeck_BF6_Flight_Configurator.CORE
{
    public class EngineBindings
    {
        public enum DeviceType
        {
            Mouse = 0,
            Controller = 1,
            Keyboard = 2,
            Joystick = 3
        }

        public static class PadInput
        {
            public const int PadButtonAxis = 56;
            public const int AnalogOrUnboundButtonId = 60;
            public static readonly PadBind LeftStickX_Pos = new(Axis: 0, Button: 60, Negate: 0);
            public static readonly PadBind LeftStickX_Neg = new(Axis: 0, Button: 60, Negate: 1);
            public static readonly PadBind LeftStickY_Pos = new(Axis: 1, Button: 60, Negate: 0);
            public static readonly PadBind LeftStickY_Neg = new(Axis: 1, Button: 60, Negate: 1);

            public static readonly PadBind RightStickX_Pos = new(Axis: 9, Button: 60, Negate: 0);
            public static readonly PadBind RightStickX_Neg = new(Axis: 9, Button: 60, Negate: 1);
            public static readonly PadBind RightStickY_Pos = new(Axis: 10, Button: 60, Negate: 0);
            public static readonly PadBind RightStickY_Neg = new(Axis: 10, Button: 60, Negate: 1);
            public static readonly PadBind DPadUp = new(Axis: 56, Button: 0, Negate: 0);
            public static readonly PadBind DPadDown = new(Axis: 56, Button: 1, Negate: 0);
            public static readonly PadBind DPadLeft = new(Axis: 56, Button: 2, Negate: 0);
            public static readonly PadBind DPadRight = new(Axis: 56, Button: 3, Negate: 0);

            public static readonly PadBind ButtonY_Tri = new(Axis: 56, Button: 4, Negate: 0);
            public static readonly PadBind ButtonA_Cross = new(Axis: 56, Button: 5, Negate: 0);
            public static readonly PadBind ButtonX_Square = new(Axis: 56, Button: 6, Negate: 0);
            public static readonly PadBind ButtonB_Circle = new(Axis: 56, Button: 7, Negate: 0);

            public static readonly PadBind LeftStickClick = new(Axis: 56, Button: 10, Negate: 0);
            public static readonly PadBind RightStickClick = new(Axis: 56, Button: 11, Negate: 0);
            public static readonly PadBind MenuStartButton = new(Axis: 56, Button: 12, Negate: 0);
            public static readonly PadBind ViewSelectButton = new(Axis: 56, Button: 13, Negate: 0);

            public static readonly PadBind LeftTrigger_LT = new(Axis: 56, Button: 14, Negate: 0);
            public static readonly PadBind RightTrigger_RT = new(Axis: 56, Button: 15, Negate: 0);
            public static readonly PadBind LeftBumper_LB = new(Axis: 56, Button: 16, Negate: 0);
            public static readonly PadBind RightBumper_RB = new(Axis: 56, Button: 17, Negate: 0);
            public static readonly PadBind Unbound = new(Axis: 56, Button: 60, Negate: 0);

            public static bool TryGetBindForSlot(string slotKey, out PadBind bind)
            {
                bind = slotKey switch
                {
                    "LT" => LeftTrigger_LT,
                    "RT" => RightTrigger_RT,
                    "LB" => LeftBumper_LB,
                    "RB" => RightBumper_RB,
                    "LS_CLICK" => LeftStickClick,
                    "RS_CLICK" => RightStickClick,
                    "DPAD_UP" => DPadUp,
                    "DPAD_DOWN" => DPadDown,
                    "DPAD_LEFT" => DPadLeft,
                    "DPAD_RIGHT" => DPadRight,
                    "BTN_NORTH" => ButtonY_Tri,
                    "BTN_SOUTH" => ButtonA_Cross,
                    "BTN_WEST" => ButtonX_Square,
                    "BTN_EAST" => ButtonB_Circle,
                    _ => Unbound
                };
                return !bind.IsUnbound;
            }
        }

        public readonly record struct ConceptSlot(string Concept, int SlotIndex, int DefaultNegate = 0);

        public static readonly IReadOnlyDictionary<string, ConceptSlot[]> ActionSlots =
            new Dictionary<string, ConceptSlot[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["Ascend"] = [new("ConceptBrake", 3, 0)],
                ["Descend"] = [new("ConceptBrake", 2, 1)],
                ["ThrottleAxis"] = [new("ConceptMoveFB", 2, 0)],
                ["Afterburner"] = [new("ConceptSprint", 1, 0)],
                ["Fire"] = [new("ConceptFire", 1, 0)],
                ["Zoom"] = [new("ConceptZoom", 2, 0)],
                ["Countermeasures"] = [new("ConceptSwitchToDynamicGadget1", 1, 0)],
                ["RepairGadget2"] = [new("ConceptSwitchToDynamicGadget2", 1, 0)],
                ["SwapWeapon"] = [new("ConceptSelectInventoryItem1", 1, 0), new("ConceptSwitchInventoryItem", 2, 0)],
                ["ChaseCam"] = [new("ConceptToggleCamera", 1, 0)],
                ["Freelook"] = [new("ConceptBreathControl", 2, 0)],
                ["Ping"] = [new("ConceptPingInput", 1, 0)],
                ["SwitchSeat"] = [new("ConceptNextPosition", 0, 0)],
                ["ReloadExit"] = [new("ConceptAbilityChoice3", 0, 0)],
                ["Yaw"] = [new("ConceptYaw", 2, 0)],
                ["Pitch"] = [new("ConceptPitch", 2, 1)],
                ["Roll"] = [new("ConceptRoll", 2, 0)]
            };

        public readonly record struct PadBind(
            int Axis,
            int Button,
            int Negate = 0,
            DeviceType Type = DeviceType.Controller
        )
        {
            public bool IsUnbound => Axis == 56 && Button == 60;
            public bool IsStickAxis => Axis != 56 && Button == 60;

            public Dictionary<string, string> ToProfSaveLines(string domain, string concept, int slotIndex = 0)
            {
                string prefix = $"GstKeyBinding.{domain}.{concept}.{slotIndex}";
                return new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [$"{prefix}.axis"] = Axis.ToString(),
                    [$"{prefix}.button"] = Button.ToString(),
                    [$"{prefix}.negate"] = Negate.ToString(),
                    [$"{prefix}.type"] = ((int)Type).ToString()
                };
            }
        }

        public static Dictionary<string, string> GetDefaultKeyBindingTable(string domain)
        {
            var d = new Dictionary<string, string>(StringComparer.Ordinal);
            void Add(string concept, int slot, int axis, int button, int negate, int type)
            {
                string p = $"GstKeyBinding.{domain}.{concept}.{slot}";
                d[$"{p}.axis"] = axis.ToString();
                d[$"{p}.button"] = button.ToString();
                d[$"{p}.negate"] = negate.ToString();
                d[$"{p}.type"] = type.ToString();
            }

            Add("ConceptAbilityChoice3", 0, 56, 6, 0, 1);

            Add("ConceptBrake", 0, 0, 57, 0, 2);
            Add("ConceptBrake", 1, 0, 29, 1, 2);
            Add("ConceptBrake", 2, 56, 16, 1, 1);
            Add("ConceptBrake", 3, 56, 17, 0, 1);
            Add("ConceptBrake", 4, 56, 60, 1, 3);

            Add("ConceptBreathControl", 0, 56, 8, 0, 0);
            Add("ConceptBreathControl", 1, 0, 47, 0, 2);
            Add("ConceptBreathControl", 2, 56, 1, 0, 1);

            Add("ConceptCameraPitch", 0, 10, 60, 0, 1);
            Add("ConceptCameraPitch", 1, 0, 208, 1, 2);
            Add("ConceptCameraPitch", 2, 0, 200, 0, 2);
            Add("ConceptCameraPitch", 3, 56, 60, 0, 3);
            Add("ConceptCameraPitch", 4, 1, 8, 0, 0);

            Add("ConceptCameraYaw", 0, 9, 60, 0, 1);
            Add("ConceptCameraYaw", 1, 0, 203, 1, 2);
            Add("ConceptCameraYaw", 2, 0, 205, 0, 2);
            Add("ConceptCameraYaw", 3, 56, 60, 0, 3);
            Add("ConceptCameraYaw", 4, 0, 8, 0, 0);

            Add("ConceptChangeVehicle", 0, 0, 18, 0, 2);
            Add("ConceptChangeVehicle", 1, 56, 60, 0, 1);

            Add("ConceptFire", 0, 0, 157, 0, 2);
            Add("ConceptFire", 1, 56, 15, 0, 1);
            Add("ConceptFire", 2, 56, 0, 0, 0);

            Add("ConceptFlickLookForward", 0, 56, 60, 0, 1);
            Add("ConceptFlickLookForward", 1, 0, 255, 0, 2);
            Add("ConceptFlickLookForward", 2, 0, 255, 1, 2);

            Add("ConceptFlickLookSide", 0, 56, 60, 0, 1);
            Add("ConceptFlickLookSide", 1, 0, 255, 0, 2);
            Add("ConceptFlickLookSide", 2, 0, 255, 1, 2);

            Add("ConceptGraniteScoreboardMenu", 0, 0, 15, 0, 2);
            Add("ConceptGyroButton", 0, 56, 60, 0, 1);
            Add("ConceptMapSize", 0, 0, 50, 0, 2);

            Add("ConceptMoveFB", 0, 0, 17, 0, 2);
            Add("ConceptMoveFB", 1, 0, 31, 1, 2);
            Add("ConceptMoveFB", 2, 1, 60, 0, 1);
            Add("ConceptMoveFB", 3, 56, 60, 0, 3);
            Add("ConceptMoveFB", 4, 56, 60, 1, 3);

            Add("ConceptNextPosition", 0, 56, 5, 0, 1);
            Add("ConceptNextPosition", 1, 0, 255, 0, 2);

            Add("ConceptPingClear", 0, 0, 255, 0, 2);
            Add("ConceptPingClear", 1, 56, 60, 0, 1);

            Add("ConceptPingInput", 0, 0, 16, 0, 2);
            Add("ConceptPingInput", 1, 56, 0, 0, 1);
            Add("ConceptPingInput", 2, 56, 2, 0, 0);

            Add("ConceptPitch", 0, 0, 208, 0, 2);
            Add("ConceptPitch", 1, 0, 200, 1, 2);
            Add("ConceptPitch", 2, 10, 60, 1, 1);
            Add("ConceptPitch", 3, 1, 8, 1, 0);
            Add("ConceptPitch", 4, 56, 60, 0, 3);
            Add("ConceptPitch", 5, 56, 60, 1, 3);

            Add("ConceptQuickLoadout", 0, 0, 21, 0, 2);

            Add("ConceptRoll", 0, 0, 203, 1, 2);
            Add("ConceptRoll", 1, 0, 205, 0, 2);
            Add("ConceptRoll", 2, 9, 60, 0, 1);
            Add("ConceptRoll", 3, 56, 60, 0, 3);
            Add("ConceptRoll", 4, 0, 8, 0, 0);
            Add("ConceptRoll", 5, 56, 60, 1, 3);

            Add("ConceptScoreboard", 0, 0, 255, 0, 2);

            Add("ConceptSelectInventoryItem1", 0, 0, 2, 0, 2);
            Add("ConceptSelectInventoryItem1", 1, 56, 4, 0, 1);

            Add("ConceptSelectInventoryItem2", 0, 0, 3, 0, 2);
            Add("ConceptSelectInventoryItem2", 1, 56, 60, 0, 1);

            Add("ConceptSelectPosition2", 0, 0, 60, 0, 2);
            Add("ConceptSelectPosition3", 0, 0, 61, 0, 2);
            Add("ConceptSelectPosition4", 0, 0, 62, 0, 2);
            Add("ConceptSelectPosition5", 0, 0, 63, 0, 2);
            Add("ConceptSelectPosition6", 0, 0, 64, 0, 2);

            Add("ConceptSplitMapQuickLoadout", 0, 56, 13, 0, 1);

            if (string.Equals(domain, "jet", StringComparison.OrdinalIgnoreCase))
            {
                Add("ConceptSprint", 0, 0, 42, 0, 2);
                Add("ConceptSprint", 1, 56, 10, 0, 1);
                Add("ConceptSprint", 2, 56, 60, 0, 3);
            }

            Add("ConceptSwitchInventoryItem", 0, 0, 33, 0, 2);
            Add("ConceptSwitchInventoryItem", 1, 16, 8, 0, 0);
            Add("ConceptSwitchInventoryItem", 2, 56, 4, 0, 1);

            Add("ConceptSwitchToDynamicGadget1", 0, 0, 4, 0, 2);
            Add("ConceptSwitchToDynamicGadget1", 1, 56, 2, 0, 1);
            Add("ConceptSwitchToDynamicGadget1", 2, 56, 4, 0, 0);

            Add("ConceptSwitchToDynamicGadget2", 0, 0, 5, 0, 2);
            Add("ConceptSwitchToDynamicGadget2", 1, 56, 3, 0, 1);
            Add("ConceptSwitchToDynamicGadget2", 2, 56, 3, 0, 0);

            Add("ConceptToggleCamera", 0, 0, 46, 0, 2);
            Add("ConceptToggleCamera", 1, 56, 11, 0, 1);

            Add("ConceptYaw", 0, 0, 30, 1, 2);
            Add("ConceptYaw", 1, 0, 32, 0, 2);
            Add("ConceptYaw", 2, 0, 60, 0, 1);
            Add("ConceptYaw", 3, 56, 60, 0, 3);
            Add("ConceptYaw", 4, 56, 8, 0, 0);
            Add("ConceptYaw", 5, 56, 60, 1, 3);

            Add("ConceptZoom", 0, 56, 1, 0, 0);
            Add("ConceptZoom", 1, 0, 255, 0, 2);
            Add("ConceptZoom", 2, 56, 14, 0, 1);

            return d;
        }
    }
}