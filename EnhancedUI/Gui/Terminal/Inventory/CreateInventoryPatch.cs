using System.Collections.Generic;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.Inventory
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal), "CreateInventoryPageControls")]
    internal static class CreateInventoryPatch
    {
        private const string Name = "Terminal";
        private static readonly WebContent Content = new();

        // ReSharper disable once UnusedMember.Local
        private static bool Prefix(
            MyGuiControlTabPage page,
            // ReSharper disable once InconsistentNaming
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard)
        {
            page.Name = "PageInventory";
            page.TextEnum = MySpaceTexts.Inventory;
            page.TextScale = 0.7005405f;

            var control = new ChromiumGuiControl(Content, Name)
            {
                Position = new Vector2(0f, 0.005f),
                Size = new Vector2(0.9f, 0.7f)
            };

            // Adds the GUI elements to the screen
            page.Controls.Add(control);
            page.Controls.Add(control.Wheel);

            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.Inventory] = control;
            return false;
        }
    }
}