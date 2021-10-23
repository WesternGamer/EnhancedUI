using System.Windows.Forms;
using CefSharp;
using EnhancedUI.Utils;
using Sandbox.Graphics.GUI;
using VRage.Input;
using VRageMath;
using Winook;
using Control = System.Windows.Forms.Control;

namespace EnhancedUI.Gui
{
    public partial class ChromiumGuiControl
    {
        private const bool HasFocus2 = true;

        private readonly MouseHook mouseHook = new(ProcessInfo.Id);
        private readonly KeyboardHook keyboardHook = new(ProcessInfo.Id);

        public override MyGuiControlBase HandleInput()
        {
            if (!IsBrowserInitialized || !HasFocus2 || MyInput.Static.IsNewKeyPressed(MyKeys.Escape))
                return base.HandleInput();

            // F12 opens Chromium's Developer Tools in a new window
            if (MyInput.Static.IsNewKeyPressed(MyKeys.F12))
            {
                OpenWebDeveloperTools();
            }

            // Ctrl-R reloads the page (handled here to be independent of browser state and input focus)
            if (MyInput.Static.IsAnyCtrlKeyPressed() && MyInput.Static.IsNewKeyPressed(MyKeys.R))
            {
                ReloadPage();

                // Ctrl-Shift-R reloads the page and clears all cookies
                // FIXME: Do we need this?
                if (MyInput.Static.IsAnyShiftKeyPressed())
                    ClearCookies();
            }

            // Tell the caller that we handled the input
            return this;
        }

        private static CefEventFlags GetModifiers()
        {
            var input = MyInput.Static;
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

        private void RegisterInputEvents()
        {
            mouseHook.MessageReceived += MouseHookOnMessageReceived;
            keyboardHook.MessageReceived += KeyboardHookOnMessageReceived;

            mouseHook.InstallAsync();
            keyboardHook.InstallAsync();
        }

        private void UnregisterInputEvents()
        {
            mouseHook.MessageReceived -= MouseHookOnMessageReceived;
            keyboardHook.MessageReceived -= KeyboardHookOnMessageReceived;

            mouseHook.Uninstall();
            keyboardHook.Uninstall();
        }

        private void MouseHookOnMessageReceived(object sender, MouseMessageEventArgs e)
        {
            if (!IsMouseOver)
                return;

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
            if (!HasFocus2)
                return;

            switch (e.Direction)
            {
                case KeyDirection.Any:
                    break;
                case KeyDirection.Up:
                    BrowserHost?.SendKeyEvent((int)Wm.Keyup, e.KeyValue, 0);
                    break;
                case KeyDirection.Down:
                    BrowserHost?.SendKeyEvent((int)Wm.Keydown, e.KeyValue, 0);
                    foreach (var c in KeyConverter.KeyCodeToUnicode(e.KeyValue))
                    {
                        // TODO IME text input
                        BrowserHost?.SendKeyEvent((int)Wm.Char, c, 0);
                    }

                    break;
            }
        }

        private Vector2I GetMousePos()
        {
            var mousePosition = MyInput.Static.GetMousePosition();

            // Correct for left-top corner (position)
            var vr = GetVideoScreenRectangle();
            mousePosition.X -= vr.Left;
            mousePosition.Y -= vr.Top;

            // Browser alignment is pixel perfect, the scale is always 1:1
            return new(mousePosition + new Vector2(0.5f, 0.5f));
        }

        private MouseEvent GetMouseEvent()
        {
            var pos = GetMousePos();
            return new MouseEvent(pos.X, pos.Y, GetModifiers());
        }
    }
}