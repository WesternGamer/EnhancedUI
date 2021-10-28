using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.Gps
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal))]
    internal static class MyGuiScreenTerminal_Patch
    {
        private const string Name = "Gps";
        private static readonly WebContent Content = new();

        [HarmonyPatch("CreateGpsPageControls")] 
        [HarmonyPrefix]
        private static bool CreateChatPageControlsPrefix(
            MyGuiControlTabPage gpsPage,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad)
        {
            gpsPage.Name = "PageIns";
            gpsPage.TextEnum = MySpaceTexts.TerminalTab_GPS;
            gpsPage.TextScale = 0.7005405f;

            var state = new GpsState();
            var control = new ChromiumGuiControl(Content, Name, state)
            {
                Position = new Vector2(0f, 0.005f),
                Size = new Vector2(0.9f, 0.7f)
            };

            // Adds the GUI elements to the screen
            gpsPage.Controls.Add(control);
            gpsPage.Controls.Add(control.Wheel);

            // Focus the browser "control" by default when the tab is selected (see the original function)
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.Gps] = control;
            ___m_defaultFocusedControlGamepad[MyTerminalPageEnum.Gps] = control;

            return false;
        }
    }
}
