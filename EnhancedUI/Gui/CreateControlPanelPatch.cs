using System.Collections.Generic;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using VRage.Game.ModAPI;
using VRage.Utils;

namespace EnhancedUI.Gui
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal), "CreateControlPanelPageControls")]
    internal static class CreateControlPanelPatch
    {
        private static bool Prefix(MyGuiControlTabPage page, Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard)
        {
            page.Name = "PageControlPanel";
            page.TextEnum = MySpaceTexts.ControlPanel;
            page.TextScale = 0.7005405f;

            var control = new ChromiumGuiControl
            {
                Position = new (0f, 0.005f),
                Size = new (0.9f, 0.7f)
            };
            page.Controls.Add(control);
            page.Controls.Add(control.Wheel);

            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.ControlPanel] = control;
            return false;
        }
    }
}