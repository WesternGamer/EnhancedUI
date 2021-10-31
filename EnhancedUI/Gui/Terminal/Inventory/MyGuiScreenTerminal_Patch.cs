using System.Collections.Generic;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.Inventory
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal))]
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once InconsistentNaming
    internal static class MyGuiScreenTerminal_Patch
    {
        private const string Name = "Inventory";
        private static readonly WebContent Content = new();

        [HarmonyPatch("CreateInventoryPageControls")]
        [HarmonyPrefix]
        // ReSharper disable once UnusedMember.Local
        private static bool CreateInventoryPageControlsPrefix(
            MyGuiControlTabPage page,
            // ReSharper disable once InconsistentNaming
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            // ReSharper disable once InconsistentNaming
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad)
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

            // Focus the browser "control" by default when the tab is selected (see the original function)
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.Inventory] = control;
            ___m_defaultFocusedControlGamepad[MyTerminalPageEnum.Inventory] = control;

            return false;
        }
    }
}