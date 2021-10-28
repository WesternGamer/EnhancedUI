using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.Factions
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal))]
    internal static class MyGuiScreenTerminal_Patch
    {
        private const string Name = "Factions";
        private static readonly WebContent Content = new();

        [HarmonyPatch("CreateFactionsPageControls")] 
        [HarmonyPrefix]
        private static bool CreateFactionsPageControlsPrefix(
            MyGuiControlTabPage page,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad)
        {
            page.Name = "PageFactions";
            page.TextEnum = MySpaceTexts.TerminalTab_Factions;
            page.TextScale = 0.7005405f;

            var state = new FactionsState();
            var control = new ChromiumGuiControl(Content, Name, state)
            {
                Position = new Vector2(0f, 0.005f),
                Size = new Vector2(0.9f, 0.7f)
            };

            // Adds the GUI elements to the screen
            page.Controls.Add(control);
            page.Controls.Add(control.Wheel);

            // Focus the browser "control" by default when the tab is selected (see the original function)
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.Factions] = control;
            ___m_defaultFocusedControlGamepad[MyTerminalPageEnum.Factions] = control;

            return false;
        }
    }
}
