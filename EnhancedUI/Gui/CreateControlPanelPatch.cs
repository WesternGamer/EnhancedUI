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
    //Replaces the controls on the Control Panel section of the terminal.
    [HarmonyPatch(typeof(MyGuiScreenTerminal), "CreateControlPanelPageControls")]
    internal static class CreateControlPanelPatch
    {
        public static ChromiumGuiControl control;

        private static bool Prefix(MyGuiControlTabPage page, Dictionary<MyTerminalPageEnum, MyGuiControlBase> ___m_defaultFocusedControlKeyboard)
        {
            //Code for a reload button
            //MyGuiControlButton refreshButton = new MyGuiControlButton(new Vector2(0, 0.0f), VRage.Game.MyGuiControlButtonStyleEnum.Default, null, null, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, "Reload HTML page.", new System.Text.StringBuilder("Reload Page"), onButtonClick: new Action<MyGuiControlButton>(ReloadAction));
            //page.Controls.Add(refreshButton);

            page.Name = "PageControlPanel";
            page.TextEnum = MySpaceTexts.ControlPanel;
            page.TextScale = 0.7005405f;

            control = new ChromiumGuiControl
            {
                //Arguements
                Position = new (0f, 0.005f),
                Size = new (0.9f, 0.7f)
            };

            //Adds the gui elements to the screen
            page.Controls.Add(control);
            page.Controls.Add(control.Wheel);

            //Sets m_defaultFocusedControlKeyboard to control
            ___m_defaultFocusedControlKeyboard[MyTerminalPageEnum.ControlPanel] = control;

            //Tells Harmony to replace the original method with code in this method.
            return false;
        }

        //Code for reloading the page with the reload button
        /*
        private static void ReloadAction(MyGuiControlButton myGuiControlButton)
        {
            control.ReloadPage();
        }*/
    }

    //Patch to allow reloading of HTML page.
    [HarmonyPatch(typeof(MyGuiScreenTerminal), "HandleUnhandledInput")]
    internal static class HandleUnhandledInputPatch
    {
        private static void Postfix()
        {
            //Checks if Ctrl+R is pressed and that the embedded browser is initialized.
            if(MyInput.Static.IsAnyCtrlKeyPressed() && MyInput.Static.IsNewKeyPressed(MyKeys.R) && CreateControlPanelPatch.control.IsBrowserInitialized)
            {
                //Reloads the HTML page.
                CreateControlPanelPatch.control.ReloadPage();
            }
        }
    }
}