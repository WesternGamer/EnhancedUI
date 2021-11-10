using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal))]
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once InconsistentNaming
    internal static class MyGuiScreenTerminal_Patch
    {
        private const string Name = "ControlPanel";
        private static readonly WebContent Content = new();

        [HarmonyPatch("CreateControlPanelPageControls")]
        [HarmonyPrefix]
        // ReSharper disable once UnusedMember.Local
        private static bool CreateControlPanelPageControlsPrefix(
            MyGuiControlTabPage page,
            // ReSharper disable once InconsistentNaming
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            // ReSharper disable once InconsistentNaming
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad)
        {
            page.Name = "PageControlPanel";
            page.TextEnum = MySpaceTexts.ControlPanel;
            page.TextScale = 0.7005405f;

            ChromiumGuiControl? control = new ChromiumGuiControl(Content, Name)
            {
                Position = new Vector2(0f, 0.005f),
                Size = new Vector2(0.9f, 0.7f)
            };

            // Adds the GUI elements to the screen
            page.Controls.Add(control);
            page.Controls.Add(control.Wheel);

            // Focus the browser "control" by default when the tab is selected (see the original function)
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.ControlPanel] = control;

            // FIXME: Looks like this is not needed, since it does not present in the original code either.
            //___m_defaultFocusedControlGamepad[MyTerminalPageEnum.ControlPanel] = control;

            return false;
        }

        [HarmonyPatch("AttachGroups")]
        [HarmonyPrefix]
        private static bool AttachGroupsPatch()
        {
            return false;
        }

        [HarmonyPatch("DetachGroups")]
        [HarmonyPrefix]
        private static bool DetachGroupsPatch()
        {
            return false;
        }
    }
}