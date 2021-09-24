using System;
using System.Collections.Generic;
using HarmonyLib;
using Sandbox;
using Sandbox.Game;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using VRage.Game.ModAPI;
using VRage.Input;
using VRage.Utils;
using VRageMath;

namespace EnhancedUI.Gui
{
    [HarmonyPatch(typeof(MyGuiScreenTerminal), "CreateControlPanelPageControls")]
    internal static class CreateControlPanelPatch
    {
        public static ChromiumGuiControl control;

        private static bool Prefix(MyGuiControlTabPage page, Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard)
        {
            //MyGuiControlButton refreshButton = new MyGuiControlButton(new Vector2(0, 0.0f), VRage.Game.MyGuiControlButtonStyleEnum.Default, null, null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, "Reload HTML page.", new System.Text.StringBuilder("Reload Page"), onButtonClick: new Action<MyGuiControlButton>(ReloadAction));
            //page.Controls.Add(refreshButton);


            page.Name = "PageControlPanel";
            page.TextEnum = MySpaceTexts.ControlPanel;
            page.TextScale = 0.7005405f;

            control = new ChromiumGuiControl
            {
                Position = new (0f, 0.005f),
                Size = new (0.9f, 0.7f)
            };
            page.Controls.Add(control);
            page.Controls.Add(control.Wheel);

            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.ControlPanel] = control;

            

            return false;
        }

        private static void ReloadAction(MyGuiControlButton myGuiControlButton)
        {
            control.ReloadPage();
        }
    }

    //Patch to allow reloading of HTML page.
    [HarmonyPatch(typeof(MyGuiScreenTerminal), "HandleUnhandledInput")]
    internal static class HandleUnhandledInputPatch
    {
        private static void Postfix()
        {
            //Checks if Ctrl+R is pressed.
            if(MyInput.Static.IsAnyCtrlKeyPressed() && MyInput.Static.IsNewKeyPressed(MyKeys.R))
            {
                CreateControlPanelPatch.control.ReloadPage();
            }
        }
    }
}