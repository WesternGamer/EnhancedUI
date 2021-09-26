using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CefSharp;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using VRage.Collections;
using VRage.Input;
using VRageMath;
using VRageRender;
using VRageRender.Messages;
using Rectangle = VRageMath.Rectangle;

namespace EnhancedUI.Gui
{
    public class ChromiumGuiControl : MyGuiControlBase
    {
        private readonly BrowserHost _browserHost;
        public static BatchDataPlayer? Player;

        private uint _videoId;

        //Returns false if the browser is not initialized else it returns true.
        public bool IsBrowserInitialized => _browserHost.Browser.IsBrowserInitialized;

        public readonly MyGuiControlRotatingWheel Wheel = new(Vector2.Zero)
        {
            Visible = false
        };

        private readonly WebContent _content;
        private readonly string _name;

        private bool _capsLock;
        private MyKeys _lastKey;
        private int _delay;

        public ChromiumGuiControl(WebContent content, string name)
        {
            _content = content;
            _name = name;

            var rect = GetVideoScreenRectangle();
            _browserHost = new(new(rect.Width, rect.Height));

            _browserHost.Ready += BrowserHostOnReady;
            _browserHost.Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;

            Player = new(new(rect.Width, rect.Height), _browserHost.GetVideoData);
        }

        private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Wheel.Visible = e.IsLoading;
        }

        private void BrowserHostOnReady()
        {
            var url = _content.FormatIndexUrl(_name);
            _browserHost.Navigate(url);
            _videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VIDEO_NAME, 0);
        }

        // Removes the browser instance when ChromiumGuiControl is no longer needed.
        public override void OnRemoving()
        {
            base.OnRemoving();

            _browserHost.Ready -= BrowserHostOnReady;
            _browserHost.Browser.LoadingStateChanged -= BrowserOnLoadingStateChanged;

            _browserHost.Dispose();
            MyRenderProxy.CloseVideo(_videoId);
        }

        // Returns the on-screen rectangle of the video player (browser) in pixels
        private Rectangle GetVideoScreenRectangle()
        {
            var pos = (Vector2I)MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(GetPositionAbsoluteTopLeft());

            var size = (Vector2I)MyGuiManager.GetScreenSizeFromNormalizedSize(Size);

            return new(pos.X, pos.Y, size.X, size.Y);
        }

        // Renders the HTML document on the screen using the video player
        public override void Draw(float transitionAlpha, float backgroundTransitionAlpha)
        {
            if (!MyRenderProxy.IsVideoValid(_videoId))
                return;

            _browserHost.Draw();
            MyRenderProxy.UpdateVideo(_videoId);
            MyRenderProxy.DrawVideo(_videoId, GetVideoScreenRectangle(), new(Vector4.One),
                MyVideoRectangleFitMode.AutoFit, false);
        }

        // Reloads the HTML document
        public void ReloadPage()
        {
            _browserHost.Browser.Reload();
        }

        // Clears the cookies from the CEF browser
        public void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }

        public override MyGuiControlBase HandleInput()
        {
            if (!IsBrowserInitialized)
                return null!;

            var input = MyInput.Static;

            if (input.IsAnyCtrlKeyPressed() && input.IsNewKeyPressed(MyKeys.R))
            {
                ReloadPage();
                if (MyInput.Static.IsAnyShiftKeyPressed())
                    ClearCookies();

                return base.HandleInput();
            }

            var browser = _browserHost.Browser;
            var browserHost = browser.GetBrowser().GetHost();

            if (input.IsKeyPress(MyKeys.CapsLock))
                _capsLock = !_capsLock;

            var modifiers = GetModifiers();

            var pressedKeys = new List<MyKeys>();
            input.GetPressedKeys(pressedKeys);

            if (pressedKeys.Count == 0)
            {
                _lastKey = MyKeys.None;
            }

            foreach (var key in pressedKeys)
            {
                if (key == MyKeys.Escape)
                    continue;

                if (key == _lastKey)
                {
                    if (_delay > 0)
                    {
                        _delay--;
                        continue;
                    }
                    _delay = 5;
                }
                else
                {
                    _lastKey = key;
                    _delay = 20;
                }

                var keyChar = (char)key;

                browserHost.SendKeyEvent(new()
                {
                    WindowsKeyCode = keyChar, // Space
                    FocusOnEditableField = true,
                    IsSystemKey = false,
                    Type = KeyEventType.KeyDown,
                    Modifiers = modifiers
                });

                browserHost.SendKeyEvent(new()
                {
                    WindowsKeyCode = keyChar, // Space
                    FocusOnEditableField = true,
                    IsSystemKey = false,
                    Type = KeyEventType.Char,
                    Modifiers = modifiers
                });

                browserHost.SendKeyEvent(new()
                {
                    WindowsKeyCode = keyChar, // Space
                    FocusOnEditableField = true,
                    IsSystemKey = false,
                    Type = KeyEventType.KeyUp,
                    Modifiers = modifiers
                });
            }

            var mousePosition = input.GetMousePosition();
            var hasValidMousePosition = mousePosition.X >= 0 && mousePosition.Y >= 0;

            if (!hasValidMousePosition)
                return base.HandleInput();

            // Correct for left-top corner (position)
            var vr = GetVideoScreenRectangle();
            mousePosition.X -= vr.Left;
            mousePosition.Y -= vr.Top;

            // Correct for aspect ratio and scale (if any)
            mousePosition /= Rectangle.Size;

            var intMousePosition = new Vector2I(mousePosition + new Vector2(0.5f, 0.5f));

            var wheelDelta = MyInput.Static.DeltaMouseScrollWheelValue();

            if (wheelDelta != 0)
            {
                browser.SendMouseWheelEvent(intMousePosition.X, intMousePosition.Y, 0, wheelDelta, modifiers);
            }
            else
            {
                browserHost.SendMouseMoveEvent(intMousePosition.X, intMousePosition.Y, false, modifiers);
            }

            if (input.IsLeftMousePressed())
            {
                browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Left, false,
                    1, modifiers);
            }

            if (input.IsMiddleMousePressed())
            {
                browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Middle,
                    false,
                    1, modifiers);
            }

            if (input.IsRightMousePressed())
            {
                browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Right,
                    false,
                    1, modifiers);
            }

            // TODO: Double-click, drag&drop, context menu

            return base.HandleInput();
        }

        private CefEventFlags GetModifiers()
        {
            var input = MyInput.Static;
            return (
                (_capsLock ? CefEventFlags.CapsLockOn : 0) |
                (input.IsAnyShiftKeyPressed() ? CefEventFlags.ShiftDown : 0) |
                (input.IsAnyCtrlKeyPressed() ? CefEventFlags.ControlDown : 0) |
                (input.IsAnyAltKeyPressed() ? CefEventFlags.AltDown : 0) |
                (input.IsLeftMousePressed() ? CefEventFlags.LeftMouseButton : 0) |
                (input.IsMiddleMousePressed() ? CefEventFlags.MiddleMouseButton : 0) |
                (input.IsRightMousePressed() ? CefEventFlags.RightMouseButton : 0));
        }
    }
}