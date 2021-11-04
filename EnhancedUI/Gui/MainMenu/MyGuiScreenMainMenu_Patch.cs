using HarmonyLib;
using SpaceEngineers.Game.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace EnhancedUI.Gui.MainMenu
{
    [HarmonyPatch(typeof(MyGuiScreenMainMenu))]
    internal class MyGuiScreenMainMenu_Patch
    {
        private const string Name = "MainMenu";
        private static readonly WebContent Content = new();

        [HarmonyPatch("RecreateControls")]
        [HarmonyPrefix]
        // ReSharper disable once UnusedMember.Local
        private static bool RecreateControlsPrefix(MyGuiScreenMainMenu __instance
            //MyGuiControlTabPage page,
            // ReSharper disable once InconsistentNaming
            //Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            // ReSharper disable once InconsistentNaming
            //Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad
            )
        {
            

            var control = new ChromiumGuiControl(Content, Name)
            {
                Position = new Vector2(0.50f, 0.50f),
                Size = new Vector2(1.33f, 1.0f)
            };

            // Adds the GUI elements to the screen
            __instance.Controls.Add(control);
            __instance.Controls.Add(control.Wheel);

            // Focus the browser "control" by default when the tab is selected (see the original function)
            //___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.ControlPanel] = control;

            // FIXME: Looks like this is not needed, since it does not present in the original code either.
            //___m_defaultFocusedControlGamepad[MyTerminalPageEnum.ControlPanel] = control;

            return false;
        }
    }
}
