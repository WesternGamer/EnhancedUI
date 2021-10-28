using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.Production
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal))]
    internal static class MyGuiScreenTerminal_Patch
    {
        private const string Name = "Production";
        private static readonly WebContent Content = new();

        [HarmonyPatch("CreateProductionPageControls")] 
        [HarmonyPrefix]
        private static bool CreateProductionPageControlsPrefix(
            MyGuiControlTabPage productionPage,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad)
        {
            productionPage.Name = "PageProduction";
            productionPage.TextEnum = MySpaceTexts.TerminalTab_Production;
            productionPage.TextScale = 0.7005405f;

            var state = new ProductionState();
            var control = new ChromiumGuiControl(Content, Name, state)
            {
                Position = new Vector2(0f, 0.005f),
                Size = new Vector2(0.9f, 0.7f)
            };

            // Adds the GUI elements to the screen
            productionPage.Controls.Add(control);
            productionPage.Controls.Add(control.Wheel);

            // Focus the browser "control" by default when the tab is selected (see the original function)
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.Production] = control;
            ___m_defaultFocusedControlGamepad[MyTerminalPageEnum.Production] = control;

            return false;
        }
    }
}
