using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using CefSharp;
using EnhancedUI.Utils;
using VRage.Input;
using VRageMath;
using Winook;

namespace EnhancedUI.Gui
{
    public partial class ChromiumGuiControl
    {
        private readonly MouseHook _mouseHook = new (Process.GetCurrentProcess().Id);
        private readonly KeyboardHook _keyboardHook = new (Process.GetCurrentProcess().Id);

#region Events

        private void RegisterEvents()
        {
            _mouseHook.MessageReceived += MouseHookOnMessageReceived;
            _keyboardHook.MessageReceived += KeyboardHookOnMessageReceived;

            _mouseHook.InstallAsync();
            _keyboardHook.InstallAsync();
        }

        private void UnregisterEvents()
        {
            _mouseHook.MessageReceived -= MouseHookOnMessageReceived;
            _keyboardHook.MessageReceived -= KeyboardHookOnMessageReceived;

            _mouseHook.Uninstall();
            _keyboardHook.Uninstall();
        }

#endregion

#region Mouse

        private void MouseHookOnMessageReceived(object sender, MouseMessageEventArgs e)
        {
            switch ((MouseMessageCode)e.MessageCode)
            {
                case MouseMessageCode.MouseMove:
                    _browserHost.Host.SendMouseMoveEvent(GetMouseEvent(), false);
                    break;
                case MouseMessageCode.LeftButtonDown:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Left, false, 1);
                    break;
                case MouseMessageCode.LeftButtonUp:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Left, true, 1);
                    break;
                case MouseMessageCode.LeftButtonDblClk:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Left, true, 2);
                    break;
                case MouseMessageCode.RightButtonDown:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Right, false, 1);
                    break;
                case MouseMessageCode.RightButtonUp:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Right, true, 1);
                    break;
                case MouseMessageCode.RightButtonDblClk:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Right, true, 2);
                    break;
                case MouseMessageCode.MiddleButtonDown:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Middle, false, 1);
                    break;
                case MouseMessageCode.MiddleButtonUp:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Middle, true, 1);
                    break;
                case MouseMessageCode.MiddleButtonDblClk:
                    _browserHost.Host.SendMouseClickEvent(GetMouseEvent(), MouseButtonType.Middle, true, 2);
                    break;
                case MouseMessageCode.MouseWheel:
                    _browserHost.Host.SendMouseWheelEvent(GetMouseEvent(), 0, e.Delta);
                    break;
                case MouseMessageCode.MouseLeave:
                    _browserHost.Host.SendMouseMoveEvent(GetMouseEvent(), true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#endregion

#region Keyboard

        private void KeyboardHookOnMessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            switch (e.Direction)
            {
                case KeyDirection.Any:
                    break;
                case KeyDirection.Up:
                    _browserHost.Host.SendKeyEvent((int)WM.KEYUP, e.KeyValue, 0);
                    break;
                case KeyDirection.Down:
                    _browserHost.Host.SendKeyEvent((int)WM.KEYDOWN, e.KeyValue, 0);
                    // TODO IME text input
                    _browserHost.Host.SendKeyEvent((int)WM.CHAR, e.KeyValue, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#endregion

#region Utils

        private Vector2I GetMousePos()
        {
            var mousePosition = MyInput.Static.GetMousePosition();
            var vr = GetVideoScreenRectangle();
            mousePosition.X -= vr.Left;
            mousePosition.Y -= vr.Top;

            // Correct for aspect ratio and scale (if any)
            mousePosition /= Rectangle.Size;

            return new (mousePosition + new Vector2(0.5f, 0.5f));
        }

        private MouseEvent GetMouseEvent()
        {
            var pos = GetMousePos();
            return new (pos.X, pos.Y, GetModifiers());
        }

        private static CefEventFlags GetModifiers()
        {
            var input = MyInput.Static;
            return (
                (Control.IsKeyLocked(Keys.CapsLock) ? CefEventFlags.CapsLockOn : 0) |
                (Control.IsKeyLocked(Keys.NumLock) ? CefEventFlags.NumLockOn : 0) |
                (input.IsAnyShiftKeyPressed() ? CefEventFlags.ShiftDown : 0) |
                (input.IsAnyCtrlKeyPressed() ? CefEventFlags.ControlDown : 0) |
                (input.IsAnyAltKeyPressed() ? CefEventFlags.AltDown : 0) |
                (input.IsLeftMousePressed() ? CefEventFlags.LeftMouseButton : 0) |
                (input.IsMiddleMousePressed() ? CefEventFlags.MiddleMouseButton : 0) |
                (input.IsRightMousePressed() ? CefEventFlags.RightMouseButton : 0));
        }

#endregion
    }
}