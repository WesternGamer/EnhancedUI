using CefSharp;
using EnhancedUI.Patches;
using EnhancedUI.Utils;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using VRage.Input;
using VRageMath;
using VRageRender;
using VRageRender.Messages;
using Rectangle = VRageMath.Rectangle;

namespace EnhancedUI.Gui
{
    public partial class ChromiumGuiControl : MyGuiControlBase
    {
        private readonly BrowserHost _browserHost;
        private uint _videoId;
        private string? _url;

        public ChromiumGuiControl()
        {
            var rect = GetVideoScreenRectangle();
            _browserHost = new (new (rect.Width, rect.Height));

            _browserHost.Ready += BrowserHostOnReady;
            _browserHost.Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;

            Player = new (new (rect.Width, rect.Height), _browserHost.GetVideoData);
        }

        public static BatchDataPlayer? Player { get; private set; }

        // Returns false if the browser is not initialized else it returns true.
        public bool IsBrowserInitialized => _browserHost.Browser.IsBrowserInitialized;

        public MyGuiControlRotatingWheel Wheel { get; } = new (Vector2.Zero)
        {
            Visible = false,
        };

        public string? Url
        {
            get => _url;
            set
            {
                _url = value;
                if (!IsBrowserInitialized || string.IsNullOrEmpty(_url))
                    return;
                _browserHost.Navigate(_url!);
            }
        }

        // Removes the browser instance when ChromiumGuiControl is no longer needed.
        public override void OnRemoving()
        {
            base.OnRemoving();
            if (IsBrowserInitialized)
            {
                MyRenderProxy.CloseVideo(_videoId);
                UnregisterEvents();
            }

            _browserHost.Ready -= BrowserHostOnReady;
            _browserHost.Browser.LoadingStateChanged -= BrowserOnLoadingStateChanged;

            _browserHost.Dispose();
        }

        // Renders the HTML document on the screen using the video player
        public override void Draw(float transitionAlpha, float backgroundTransitionAlpha)
        {
            if (!MyRenderProxy.IsVideoValid(_videoId))
                return;

            _browserHost.Draw();
            MyRenderProxy.UpdateVideo(_videoId);
            MyRenderProxy.DrawVideo(_videoId, GetVideoScreenRectangle(), new (Vector4.One), MyVideoRectangleFitMode.AutoFit, false);
        }

        // Reloads the HTML document
        public void ReloadPage()
        {
            _browserHost.Browser.Reload();
        }

        // Clears the cookies from the CEF browser
        public void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies(string.Empty, string.Empty);
        }

        public void RegisterJsType(string name, object typeObject)
        {
            _browserHost.Browser.JavascriptObjectRepository.Register(name, typeObject, true, BindingOptions.DefaultBinder);
        }

        public override MyGuiControlBase HandleInput()
        {
            if (!IsBrowserInitialized || !CheckMouseOver())
                return base.HandleInput();

            var input = MyInput.Static;

            if (!input.IsAnyCtrlKeyPressed() || !input.IsNewKeyPressed(MyKeys.R))
                return base.HandleInput();

            ReloadPage();
            if (MyInput.Static.IsAnyShiftKeyPressed())
                ClearCookies();

            return base.HandleInput();
        }

        private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Wheel.Visible = e.IsLoading;
        }

        private void BrowserHostOnReady()
        {
            if (!string.IsNullOrEmpty(_url))
                _browserHost.Navigate(_url!);

            _videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VIDEO_NAME, 0);
            RegisterEvents();
        }

        // Returns the on-screen rectangle of the video player (browser) in pixels
        private Rectangle GetVideoScreenRectangle()
        {
            var pos = (Vector2I)MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(GetPositionAbsoluteTopLeft());

            var size = (Vector2I)MyGuiManager.GetScreenSizeFromNormalizedSize(Size);

            return new (pos.X, pos.Y, size.X, size.Y);
        }
    }
}
