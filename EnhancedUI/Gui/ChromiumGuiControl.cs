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
        public static BatchDataPlayer? Player;

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
        private MyKeys lastKey;
        private int delay;

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

            Player = new BatchDataPlayer(new Vector2I(rect.Width, rect.Height), chromium.GetVideoData);
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

            Player = null;
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
            videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VideoName, 0);
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

            var input = MyInput.Static;

            if (input.IsAnyCtrlKeyPressed() && input.IsNewKeyPressed(MyKeys.R))
            {
                ReloadPage();
                if (MyInput.Static.IsAnyShiftKeyPressed())
                {
                    ClearCookies();
                }

                return base.HandleInput();
            }

            if (chromium == null)
            {
                throw new Exception("This should not happen");
            }

            var browser = chromium.Browser;
            var browserHost = browser.GetBrowser().GetHost();

            if (input.IsKeyPress(MyKeys.CapsLock))
            {
                capsLock = !capsLock;
            }

            var modifiers = GetModifiers();

            var pressedKeys = new List<MyKeys>();
            input.GetPressedKeys(pressedKeys);

            if (pressedKeys.Count == 0)
            {
                lastKey = MyKeys.None;
            }

            foreach (var key in pressedKeys)
            {
                if (key == MyKeys.Escape)
                {
                    continue;
                }

                if (key == lastKey)
                {
                    if (delay > 0)
                    {
                        delay--;
                        continue;
                    }

                    delay = 5;
                }
                else
                {
                    lastKey = key;
                    delay = 20;
                }

                var keyChar = (char)key;

                browserHost.SendKeyEvent(new KeyEvent
                {
                    WindowsKeyCode = keyChar, // Space
                    FocusOnEditableField = true,
                    IsSystemKey = false,
                    Type = KeyEventType.KeyDown,
                    Modifiers = modifiers
                });

                browserHost.SendKeyEvent(new KeyEvent
                {
                    WindowsKeyCode = keyChar, // Space
                    FocusOnEditableField = true,
                    IsSystemKey = false,
                    Type = KeyEventType.Char,
                    Modifiers = modifiers
                });

                browserHost.SendKeyEvent(new KeyEvent
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
            {
                return base.HandleInput();
            }

            // Correct for left-top corner (position)
            var vr = GetVideoScreenRectangle();
            mousePosition.X -= vr.Left;
            mousePosition.Y -= vr.Top;

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
                browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Left, false, 1, modifiers);
            }

            if (input.IsMiddleMousePressed())
            {
                browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Middle, false, 1, modifiers);
            }

            if (input.IsRightMousePressed())
            {
                browserHost.SendMouseClickEvent(intMousePosition.X, intMousePosition.Y, MouseButtonType.Right, false, 1, modifiers);
            }

            // TODO: Double-click, drag&drop, context menu

            return base.HandleInput();
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
    }
}