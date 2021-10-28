using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using VRage.Game.ModAPI;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.Chat
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal))]
    internal static class MyGuiScreenTerminal_Patch
    {
        private const string Name = "Comms";
        private static readonly WebContent Content = new();

        [HarmonyPatch("CreateChatPageControls")] 
        [HarmonyPrefix]
        private static bool CreateChatPageControlsPrefix(
            MyGuiControlTabPage chatPage,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard,
            Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlGamepad)
        {
            chatPage.Name = "PageComms";
            chatPage.TextEnum = MySpaceTexts.TerminalTab_Chat;
            chatPage.TextScale = 0.7005405f;

            var state = new ChatState();
            var control = new ChromiumGuiControl(Content, Name, state)
            {
                Position = new Vector2(0f, 0.005f),
                Size = new Vector2(0.9f, 0.7f)
            };

            // Adds the GUI elements to the screen
            chatPage.Controls.Add(control);
            chatPage.Controls.Add(control.Wheel);

            // Focus the browser "control" by default when the tab is selected (see the original function)
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.Comms] = control;
            ___m_defaultFocusedControlGamepad[MyTerminalPageEnum.Comms] = control;

            return false;
        }
    }
}
