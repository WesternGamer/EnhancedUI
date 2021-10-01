using System;
using System.Collections.Generic;
using CefSharp;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using VRage.Input;
using VRageMath;
using VRageRender;
using VRageRender.Messages;
using Rectangle = VRageMath.Rectangle;

namespace EnhancedUI.Gui
{
    public class ChromiumGuiControl : MyGuiControlBase
    {
        private Chromium? chromium;
        private BatchDataPlayer? player;
        private static readonly MyKeyThrottler KeyThrottler = new();

        private uint videoId;

        //Returns false if the browser is not initialized else it returns true.
        private bool IsBrowserInitialized => chromium?.Browser.IsBrowserInitialized ?? false;

        public readonly MyGuiControlRotatingWheel Wheel = new(Vector2.Zero)
        {
            Visible = false
        };

        private readonly WebContent content;
        private readonly string name;

        private bool capsLock;
        private Vector2I lastValidMousePosition = -Vector2I.One;
        private List<MyKeys> lastPressedKeys = new();

        public ChromiumGuiControl(WebContent content, string name)
        {
            this.content = content;
            this.name = name;
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            // Create the player only when the exact size of the control is already known
            // FIXME: Verify whether we need to support control re-sizing
            CreatePlayerIfNeeded();
        }

        private void CreatePlayerIfNeeded()
        {
            if (chromium != null)
            {
                return;
            }

            var rect = GetVideoScreenRectangle();
            chromium = new Chromium(new Vector2I(rect.Width, rect.Height));

            chromium.Ready += OnChromiumReady;
            chromium.Browser.LoadingStateChanged += OnBrowserLoadingStateChanged;

            player = new BatchDataPlayer(new Vector2I(rect.Width, rect.Height), chromium.GetVideoData);
            VideoPlayPatch.RegisterVideoPlayer(name, player);
        }

        public override void OnRemoving()
        {
            base.OnRemoving();

            if (chromium == null)
            {
                return;
            }

            chromium.Ready -= OnChromiumReady;
            chromium.Browser.LoadingStateChanged -= OnBrowserLoadingStateChanged;

            chromium.Dispose();
            MyRenderProxy.CloseVideo(videoId);

            VideoPlayPatch.UnregisterVideoPlayer(name);

            player = null;
            chromium = null;
        }

        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Wheel.Visible = e.IsLoading;
        }

        private void OnChromiumReady()
        {
            if (chromium == null)
            {
                throw new Exception("This should not happen");
            }

            var url = content.FormatIndexUrl(name);
            chromium.Navigate(url);
            videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VideoNamePrefix + name, 0);
        }

        // Removes the browser instance when ChromiumGuiControl is no longer needed.

        // Returns the on-screen rectangle of the video player (browser) in pixels

        private Rectangle GetVideoScreenRectangle()
        {
            var pos = (Vector2I)MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(GetPositionAbsoluteTopLeft());

            var size = (Vector2I)MyGuiManager.GetScreenSizeFromNormalizedSize(Size);

            return new Rectangle(pos.X, pos.Y, size.X, size.Y);
        }

        // Renders the HTML document on the screen using the video player
        public override void Draw(float transitionAlpha, float backgroundTransitionAlpha)
        {
            if (!MyRenderProxy.IsVideoValid(videoId))
            {
                return;
            }

            if (chromium == null)
            {
                throw new Exception("This should not happen");
            }

            chromium.Draw();
            MyRenderProxy.UpdateVideo(videoId);
            MyRenderProxy.DrawVideo(videoId, GetVideoScreenRectangle(), new Color(Vector4.One),
                MyVideoRectangleFitMode.AutoFit, false);
        }

        // Reloads the HTML document
        private void ReloadPage()
        {
            if (chromium == null)
            {
                throw new Exception("This should not happen");
            }

            chromium.Browser.Reload();
        }

        // Clears the cookies from the CEF browser
        private void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }

        public override MyGuiControlBase HandleInput()
        {
            if (!IsBrowserInitialized)
            {
                return null!;
            }

            if (chromium == null)
            {
                throw new Exception("This should not happen");
            }

            var ret = base.HandleInput();
            if (ret != null)
                return ret;

            var input = MyInput.Static;

            // Do not handle the ESC key, so the player can exit the menu anytime
            if (input.IsNewKeyPressed(MyKeys.Escape))
                return null!;

            // Reload the page with Ctrl-R, also clears cookies with Ctrl-Shift-R
            if (input.IsAnyCtrlKeyPressed() && input.IsNewKeyPressed(MyKeys.R))
            {
                ReloadPage();
                if (MyInput.Static.IsAnyShiftKeyPressed())
                {
                    ClearCookies();
                }
                return this;
            }

            var handled = false;

            var browser = chromium.Browser;
            var browserHost = browser.GetBrowser().GetHost();

            if (input.IsNewKeyPressed(MyKeys.CapsLock))
            {
                capsLock = !capsLock;
                handled = true;
            }

            var modifiers = GetModifiers();

            var pressedKeys = new List<MyKeys>();
            input.GetPressedKeys(pressedKeys);

            if (pressedKeys.Count > 0)
                handled = true;

            foreach (var key in pressedKeys)
            {
                if (KeyThrottler.GetKeyStatus(key) == ThrottledKeyStatus.PRESSED_AND_READY)
                {
                    var windowsKeyCode = ToWindowsKeyCode(key);
                    browserHost.SendKeyEvent(new KeyEvent
                    {
                        WindowsKeyCode = windowsKeyCode,
                        FocusOnEditableField = true,
                        IsSystemKey = false,
                        Type = KeyEventType.KeyDown,
                        Modifiers = modifiers
                    });
                }
            }

            foreach (var ch in input.TextInput)
            {
                if (char.IsControl(ch) && ch != '\r' && ch != '\t')
                    continue;

                browserHost.SendKeyEvent(new KeyEvent
                {
                    WindowsKeyCode = ch,
                    FocusOnEditableField = true,
                    IsSystemKey = false,
                    Type = KeyEventType.Char,
                    Modifiers = modifiers
                });
            }

            foreach (var lastPressedKey in lastPressedKeys)
            {
                if (KeyThrottler.GetKeyStatus(lastPressedKey) == ThrottledKeyStatus.UNPRESSED)
                {
                    browserHost.SendKeyEvent(new KeyEvent
                    {
                        WindowsKeyCode = ToWindowsKeyCode(lastPressedKey),
                        FocusOnEditableField = true,
                        IsSystemKey = false,
                        Type = KeyEventType.KeyUp,
                        Modifiers = modifiers
                    });
                }
            }

            lastPressedKeys = pressedKeys;

            if (IsMouseOver)
            {
                var mousePosition = input.GetMousePosition();

                // Correct for left-top corner (position)
                var vr = GetVideoScreenRectangle();
                mousePosition.X -= vr.Left;
                mousePosition.Y -= vr.Top;

                var intMousePosition = new Vector2I(mousePosition + new Vector2(0.5f, 0.5f));
                lastValidMousePosition = intMousePosition;

                var wheelDelta = MyInput.Static.DeltaMouseScrollWheelValue();
                if (wheelDelta != 0)
                {
                    browser.SendMouseWheelEvent(intMousePosition.X, intMousePosition.Y, 0, wheelDelta, modifiers);
                }
                else
                {
                    browserHost.SendMouseMoveEvent(intMousePosition.X, intMousePosition.Y, false, modifiers);
                }

                if (input.IsNewLeftMousePressed())
                {
                    browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Left, false, 1, modifiers);
                }

                if (input.IsNewMiddleMousePressed())
                {
                    browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Middle, false, 1, modifiers);
                }

                if (input.IsNewRightMousePressed())
                {
                    browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Right, false, 1, modifiers);
                }
            }
            else
            {
                if (lastValidMousePosition.X >= 0)
                {
                    browserHost.SendMouseMoveEvent(lastValidMousePosition.X, lastValidMousePosition.Y, true, modifiers);
                }
                lastValidMousePosition = -Vector2I.One;
            }

            return handled ? this : null!;
        }

        private static int ToWindowsKeyCode(MyKeys key)
        {
            return (int)key;
        }

        private CefEventFlags GetModifiers()
        {
            var input = MyInput.Static;
            return (capsLock ? CefEventFlags.CapsLockOn : 0) |
                   (input.IsAnyShiftKeyPressed() ? CefEventFlags.ShiftDown : 0) |
                   (input.IsAnyCtrlKeyPressed() ? CefEventFlags.ControlDown : 0) |
                   (input.IsAnyAltKeyPressed() ? CefEventFlags.AltDown : 0) |
                   (input.IsLeftMousePressed() ? CefEventFlags.LeftMouseButton : 0) |
                   (input.IsMiddleMousePressed() ? CefEventFlags.MiddleMouseButton : 0) |
                   (input.IsRightMousePressed() ? CefEventFlags.RightMouseButton : 0);
        }

        public void FocusEnded()
        {
            OnFocusChanged(false);
        }

        public override void OnFocusChanged(bool focus)
        {
            chromium?.Browser.GetBrowser().GetHost().SetFocus(focus);

            base.OnFocusChanged(focus);
        }

        private void DebugDraw()
        {
            MyGuiManager.DrawBorders(GetPositionAbsoluteTopLeft(), Size, Color.White, 1);
        }
   }
}