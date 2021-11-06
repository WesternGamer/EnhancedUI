using System.Collections.Generic;
using Sandbox.Graphics.GUI;
using VRage.Input;

namespace EnhancedUI.Gui
{
    public partial class ChromiumGuiControl
    {
        // FIXME: For debugging only
        private static readonly Dictionary<string, ChromiumGuiControl> BrowserControls = new();

        public override MyGuiControlBase HandleInput()
        {
            if (!IsActive || !IsBrowserInitialized || !CheckMouseOver() || MyInput.Static.IsNewKeyPressed(MyKeys.Escape))
                return base.HandleInput();

            // F12 opens Chromium's Developer Tools in a new window
            if (MyInput.Static.IsNewKeyPressed(MyKeys.F12) && MyInput.Static.IsAnyCtrlKeyPressed())
            {
                OpenWebDeveloperTools();
            }

            if (MyInput.Static.IsAnyCtrlKeyPressed() && MyInput.Static.IsNewKeyPressed(MyKeys.R))
            {
                ReloadPage();

                // FIXME: Do we need this?
                // Ctrl-Shift-R reloads the page and clears all cookies
                if (MyInput.Static.IsAnyShiftKeyPressed())
                    ClearCookies();
            }

            return base.HandleInputElements();
        }
    }
}