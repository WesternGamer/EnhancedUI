using CefSharp;
using EnhancedUI.Utils;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using System.Windows.Forms;
using VRage.Input;
using VRageMath;
using Winook;
using Control = System.Windows.Forms.Control;

namespace EnhancedUI.Gui.HtmlGuiControl
{
    public partial class ChromiumGuiControl
    {
        private static readonly MouseHook MouseHook = new(ProcessInfo.Id);
        private static readonly KeyboardHook KeyboardHook = new(ProcessInfo.Id);

        // FIXME: For debugging only
        private static readonly Dictionary<string, ChromiumGuiControl> BrowserControls = new();

        public override MyGuiControlBase HandleInput()
        {
            if (!IsActive || !IsBrowserInitialized || !CheckMouseOver() || MyInput.Static.IsNewKeyPressed(MyKeys.Escape))
            {
                return base.HandleInput();
            }

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
                {
                    ClearCookies();
                }
            }

            return base.HandleInputElements();
        }

        private static CefEventFlags GetModifiers()
        {
            IMyInput? input = MyInput.Static;
            return
                (Control.IsKeyLocked(Keys.CapsLock) ? CefEventFlags.CapsLockOn : 0) |
                (Control.IsKeyLocked(Keys.NumLock) ? CefEventFlags.NumLockOn : 0) |
                (input.IsAnyShiftKeyPressed() ? CefEventFlags.ShiftDown : 0) |
                (input.IsAnyCtrlKeyPressed() ? CefEventFlags.ControlDown : 0) |
                (input.IsAnyAltKeyPressed() ? CefEventFlags.AltDown : 0) |
                (input.IsLeftMousePressed() ? CefEventFlags.LeftMouseButton : 0) |
                (input.IsMiddleMousePressed() ? CefEventFlags.MiddleMouseButton : 0) |
                (input.IsRightMousePressed() ? CefEventFlags.RightMouseButton : 0);
        }

        private void InstallHooks()
        {
            MouseHook.InstallAsync();
            KeyboardHook.InstallAsync();
        }

        private void UninstallHooks()
        {
            MouseHook.Uninstall();
            KeyboardHook.Uninstall();
        }

        private void RegisterInputEvents()
        {
            MouseHook.MessageReceived += MouseHookOnMessageReceived;
            KeyboardHook.MessageReceived += KeyboardHookOnMessageReceived;
        }

        private void UnregisterInputEvents()
        {
            MouseHook.MessageReceived -= MouseHookOnMessageReceived;
            KeyboardHook.MessageReceived -= KeyboardHookOnMessageReceived;
        }

        private void MouseHookOnMessageReceived(object sender, MouseMessageEventArgs e)
        {
            if (!IsActive)
            {
                return;
            }

            switch ((MouseMessageCode)e.MessageCode)
            {
                case MouseMessageCode.MouseMove:
                    BrowserHost?.SendMouseMoveEvent(GetMouseEvent(), false);
                    break;
                case MouseMessageCode.LeftButtonDown:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Left, false, 1);
                    break;
                case MouseMessageCode.LeftButtonUp:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Left, true, 1);
                    break;
                case MouseMessageCode.LeftButtonDblClk:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Left, true, 2);
                    break;
                case MouseMessageCode.RightButtonDown:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Right, false, 1);
                    break;
                case MouseMessageCode.RightButtonUp:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Right, true, 1);
                    break;
                case MouseMessageCode.RightButtonDblClk:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Right, true, 2);
                    break;
                case MouseMessageCode.MiddleButtonDown:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Middle, false, 1);
                    break;
                case MouseMessageCode.MiddleButtonUp:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Middle, true, 1);
                    break;
                case MouseMessageCode.MiddleButtonDblClk:
                    BrowserHost?.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Middle, true, 2);
                    break;
                case MouseMessageCode.MouseWheel:
                    BrowserHost?.SendMouseWheelEvent(GetMouseEvent(), 0, e.Delta);
                    break;
                case MouseMessageCode.MouseLeave:
                    BrowserHost?.SendMouseMoveEvent(GetMouseEvent(), true);
                    break;
                    // FIXME: MouseMessageCode.NCMouseMove can arrive!
                    // default:
                    //     throw new ArgumentOutOfRangeException();
            }
        }

        private void KeyboardHookOnMessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            if (!IsActive)
            {
                return;
            }

            switch (e.Direction)
            {
                case KeyDirection.Any:
                    break;
                case KeyDirection.Up:
                    BrowserHost?.SendKeyEvent((int)Wm.Keyup, e.KeyValue, 0);
                    break;
                case KeyDirection.Down:
                    BrowserHost?.SendKeyEvent((int)Wm.Keydown, e.KeyValue, 0);
                    foreach (char c in KeyConverter.KeyCodeToUnicode(e.KeyValue))
                    {
                        // TODO IME text input
                        BrowserHost?.SendKeyEvent((int)Wm.Char, c, 0);
                    }

                    break;
            }
        }

        private Vector2I GetMousePos()
        {
            Vector2 mousePosition = MyInput.Static.GetMousePosition();

            // Correct for left-top corner (position)
            Rectangle vr = GetVideoScreenRectangle();
            mousePosition.X -= vr.Left;
            mousePosition.Y -= vr.Top;

            // Browser alignment is pixel perfect, the scale is always 1:1
            return new(mousePosition + new Vector2(0.5f, 0.5f));
        }

        private MouseEvent GetMouseEvent()
        {
            Vector2I pos = GetMousePos();
            return new MouseEvent(pos.X, pos.Y, GetModifiers());
        }
    }
}