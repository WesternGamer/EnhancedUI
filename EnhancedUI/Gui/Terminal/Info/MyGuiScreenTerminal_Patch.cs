using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.Info
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal))]
    internal static class MyGuiScreenTerminal_Patch
    {
        private const string Name = "Info";
        private static readonly WebContent Content = new();

        [HarmonyPatch("CreateInfoPageControls")] 
        [HarmonyPrefix]
        private static bool CreateInfoPageControlsPrefix(
            MyGuiControlTabPage infoPage,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad)
        {
            infoPage.Name = "PageInfo";
            infoPage.TextEnum = MySpaceTexts.TerminalTab_Info;
            infoPage.TextScale = 0.7005405f;

            var state = new InfoState();
            var control = new ChromiumGuiControl(Content, Name, state)
            {
                Position = new Vector2(0f, 0.005f),
                Size = new Vector2(0.9f, 0.7f)
            };

            // Adds the GUI elements to the screen
            infoPage.Controls.Add(control);
            infoPage.Controls.Add(control.Wheel);

            // Focus the browser "control" by default when the tab is selected (see the original function)
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.Info] = control;
            ___m_defaultFocusedControlGamepad[MyTerminalPageEnum.Info] = control;

            return false;
        }
    }
}
