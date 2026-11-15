using System;
using System.Collections.Generic;
using System.Text;

namespace FlightDeck_BF6_Flight_Configurator.CORE
{
    public class EngineBindings
    {
        public enum DeviceType
        {
            Mouse = 0,
            Controller = 1,
            Keyboard = 2
        }

        public static class PadInput
        {
            public const int AnalogAxisButtonId = 60;
            public static readonly PadBind LeftStickY_Pos = new(Axis: 1, Button: 60, Negate: 0);
            public static readonly PadBind LeftStickY_Neg = new(Axis: 1, Button: 60, Negate: 1);
            public static readonly PadBind LeftStickX_Pos = new(Axis: 0, Button: 60, Negate: 0);
            public static readonly PadBind LeftStickX_Neg = new(Axis: 0, Button: 60, Negate: 1);

            public static readonly PadBind RightStickY_Pos = new(Axis: 3, Button: 60, Negate: 0);
            public static readonly PadBind RightStickY_Neg = new(Axis: 3, Button: 60, Negate: 1);
            public static readonly PadBind RightStickX_Pos = new(Axis: 2, Button: 60, Negate: 0);
            public static readonly PadBind RightStickX_Neg = new(Axis: 2, Button: 60, Negate: 1);
            public static readonly PadBind DPadUp = new(Axis: 0, Button: 0, Negate: 0);
            public static readonly PadBind DPadDown = new(Axis: 0, Button: 1, Negate: 0);
            public static readonly PadBind DPadLeft = new(Axis: 0, Button: 2, Negate: 0);
            public static readonly PadBind DPadRight = new(Axis: 0, Button: 3, Negate: 0);

            public static readonly PadBind ButtonA_Cross = new(Axis: 0, Button: 4, Negate: 0);
            public static readonly PadBind ButtonB_Circle = new(Axis: 0, Button: 5, Negate: 0);
            public static readonly PadBind ButtonX_Square = new(Axis: 0, Button: 6, Negate: 0);
            public static readonly PadBind ButtonY_Tri = new(Axis: 0, Button: 7, Negate: 0);
            public static readonly PadBind LeftBumper_LB = new(Axis: 0, Button: 8, Negate: 0);
            public static readonly PadBind RightBumper_RB = new(Axis: 0, Button: 9, Negate: 0);
            public static readonly PadBind LeftStickClick = new(Axis: 0, Button: 10, Negate: 0);
            public static readonly PadBind RightStickClick = new(Axis: 0, Button: 11, Negate: 0);
            public static readonly PadBind LeftTrigger_LT = new(Axis: 4, Button: 12, Negate: 0);
            public static readonly PadBind RightTrigger_RT = new(Axis: 5, Button: 13, Negate: 0);
            public static readonly PadBind Unbound = new(Axis: 0, Button: 60, Negate: 0);
        }

        public readonly record struct PadBind(
            int Axis,
            int Button,
            int Negate = 0,
            DeviceType Type = DeviceType.Controller
        )
        {
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
    }
}
