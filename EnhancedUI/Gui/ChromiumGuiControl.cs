using CefSharp;
using EnhancedUI.ViewModel;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using System;
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

        private IBrowserHost? BrowserHost => chromium?.Browser.GetBrowser().GetHost();

        /// <summary>
        /// Returns true if the browser has been initialized already.
        /// </summary>
        private bool IsBrowserInitialized => chromium?.Browser.IsBrowserInitialized ?? false;

        /// <summary>
        /// True if this browser is visible and should get the input focus. OnFocusChanged and HasFocus are not reliable, use OnVisibleChanged and Visible of the tab page instead
        /// </summary>
        private bool IsActive => Visible && (Owner as MyGuiControlTabPage)?.Visible == true;

        public readonly MyGuiControlRotatingWheel Wheel = new(Vector2.Zero)
        {
            Visible = false
        };

        private readonly WebContent content;
        private readonly string name;

        private static bool hooksInstalled;

        public ChromiumGuiControl(WebContent content, string name)
        {
            this.content = content;
            this.name = name;

            CanHaveFocus = true;

            MyLog.Default.Info($"{name} browser created");

            if (!hooksInstalled)
            {
                InstallHooks();
                hooksInstalled = true;
            }

            if (TerminalViewModel.Instance != null)
            {
                TerminalViewModel.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }

        ~ChromiumGuiControl()
        {
            if (TerminalViewModel.Instance != null)
            {
                TerminalViewModel.Instance.OnGameStateChanged -= OnGameStateChanged;
            }

            if (hooksInstalled)
            {
                UninstallHooks();
                hooksInstalled = false;
            }
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

            Rectangle rect = GetVideoScreenRectangle();
            chromium = new Chromium(new Vector2I(rect.Width, rect.Height));

            BrowserControls[name] = this;

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

            BrowserControls.Remove(name);

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

        private void OnGameStateChanged(long version)
        {
            if (!IsBrowserInitialized)
            {
                return;
            }

            chromium?.Browser.ExecuteScriptAsync($"if (typeof OnGameStateChange != typeof undefined) OnGameStateChange({version})");
        }

        private void Navigate()
        {
            string? url = content.FormatIndexUrl(name);
            MyLog.Default.Info($"{name} browser navigation: {url}");
            chromium?.Navigate(url);
        }

        /// <summary>
        /// Returns the on-screen rectangle of the video player (browser) in pixels
        /// </summary>
        private Rectangle GetVideoScreenRectangle()
        {
            Vector2I pos = (Vector2I)MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(GetPositionAbsoluteTopLeft());

            Vector2I size = (Vector2I)MyGuiManager.GetScreenSizeFromNormalizedSize(Size);

            return new Rectangle(pos.X, pos.Y, size.X, size.Y);
        }

        /// <summary>
        /// Renders the HTML document on the screen using the video player
        /// </summary>
        /// <param name="transitionAlpha">Unused.</param>
        /// <param name="backgroundTransitionAlpha">Unused.</param>
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

        /// <summary>
        /// Reloads the HTML document.
        /// </summary>
        private void ReloadPage()
        {
            Navigate();
            MyLog.Default.Info($"{name} browser is reloading");
        }

        /// <summary>
        /// Opens Dev tools.
        /// </summary>
        private void OpenWebDeveloperTools()
        {
            chromium?.Browser.ShowDevTools();
            MyLog.Default.Info($"{name} browser Developer Tools opened");
        }

        /// <summary>
        /// Clears the cookies from the CEF browser
        /// </summary>
        private void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }

        /// <summary>
        /// Debugging use.
        /// </summary>
        private void DebugDraw()
        {
            MyGuiManager.DrawBorders(GetPositionAbsoluteTopLeft(), Size, Color.White, 1);
        }
    }
}