using System;
using CefSharp;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using VRage.Utils;
using VRageMath;
using VRageRender;
using VRageRender.Messages;
using Rectangle = VRageMath.Rectangle;

namespace EnhancedUI.Gui
{
    public partial class ChromiumGuiControl : MyGuiControlBase
    {
        private Chromium? chromium;
        private BatchDataPlayer? player;
        private uint videoId;

        //Returns false if the browser is not initialized else it returns true.
        private bool IsBrowserInitialized => chromium?.Browser.IsBrowserInitialized ?? false;

        private IBrowserHost? BrowserHost => chromium?.Browser.GetBrowser().GetHost();

        public readonly MyGuiControlRotatingWheel Wheel = new(Vector2.Zero)
        {
            Visible = false
        };

        private readonly WebContent content;
        private readonly string name;

        private readonly IPanelState state;

        public ChromiumGuiControl(WebContent content, string name, IPanelState state)
        {
            this.content = content;
            this.name = name;
            this.state = state;

            // FIXME: Do we need this?
            CanHaveFocus = true;

            MyLog.Default.Info($"{name} browser created");
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
                chromium = new Chromium(new Vector2I(rect.Width, rect.Height), state);

            MyLog.Default.Info($"{name} browser size: {rect.Width} * {rect.Height} px");

            chromium.Ready += OnChromiumReady;
            chromium.Browser.LoadingStateChanged += OnBrowserLoadingStateChanged;

            RegisterInputEvents();

            player = new BatchDataPlayer(new Vector2I(rect.Width, rect.Height), chromium.GetVideoData);
            VideoPlayPatch.RegisterVideoPlayer(name, player);

            MyLog.Default.Info($"{name} browser video player created");
        }

        public override void OnRemoving()
        {
            base.OnRemoving();

            if (chromium == null)
            {
                return;
            }

            UnregisterInputEvents();

            state.SetBrowser(null);

            chromium.Ready -= OnChromiumReady;
            chromium.Browser.LoadingStateChanged -= OnBrowserLoadingStateChanged;

            chromium.Dispose();
            MyRenderProxy.CloseVideo(videoId);

            VideoPlayPatch.UnregisterVideoPlayer(name);

            player = null;
            chromium = null;

            MyLog.Default.Info($"{name} browser removed");
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

            Navigate();

            videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VideoNamePrefix + name, 0);
        }

        private void Navigate()
        {
            var url = content.FormatIndexUrl(name);
            MyLog.Default.Info($"{name} browser navigation: {url}");
            state.SetBrowser(chromium?.Browser);
            chromium?.Navigate(url);
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
            Navigate();
            MyLog.Default.Info($"{name} browser is reloading");
        }

        private void OpenWebDeveloperTools()
        {
            chromium?.Browser.ShowDevTools();
            MyLog.Default.Info($"{name} browser Developer Tools opened");
        }

        // Clears the cookies from the CEF browser
        private void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }

        // FIXME: This event is not reliable. Why?
        public override void OnFocusChanged(bool focus)
        {
            // BrowserHost?.SetFocus(focus);

            MyLog.Default.Info($"{name} browser focus {focus}");

            base.OnFocusChanged(focus);
        }

        private void DebugDraw()
        {
            MyGuiManager.DrawBorders(GetPositionAbsoluteTopLeft(), Size, Color.White, 1);
        }
    }
}