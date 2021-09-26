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
    public partial class ChromiumGuiControl : MyGuiControlBase
    {
        private readonly BrowserHost _browserHost;
        public static BatchDataPlayer? Player;

        private uint _videoId;

        //Returns false if the browser is not initialized else it returns true.
        public bool IsBrowserInitialized => _browserHost.Browser.IsBrowserInitialized;

        public readonly MyGuiControlRotatingWheel Wheel = new (Vector2.Zero)
        {
            Visible = false
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
        private string? _url;

        public ChromiumGuiControl()
        {
            var rect = GetScreenSize();
            _browserHost = new (new (rect.Width, rect.Height));
            _browserHost.Ready += BrowserHostOnReady;
            _browserHost.Browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
            Player = new (new (rect.Width, rect.Height), DataGetter);
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

        //Removes the browser instance when ChromiumGuiControl is no longer needed.
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

        private byte[] DataGetter()
        {
            return _browserHost.VideoData;
        }

        //Returns the screen size as a VRageMath.Rectangle.
        private Rectangle GetScreenSize()
        {
            var pos = (Vector2I)MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(GetPositionAbsoluteTopLeft());

            var size = (Vector2I)MyGuiManager.GetScreenSizeFromNormalizedSize(Size);

            return new (pos.X, pos.Y, size.X, size.Y);
        }

        //Draws the HTML file on the screen using the video player.
        public override void Draw(float transitionAlpha, float backgroundTransitionAlpha)
        {
            if (!MyRenderProxy.IsVideoValid(_videoId))
                return;

            _browserHost.Draw();
            MyRenderProxy.UpdateVideo(_videoId);
            MyRenderProxy.DrawVideo(_videoId, GetScreenSize(), new (Vector4.One),
                MyVideoRectangleFitMode.AutoFit, false);
        }

        //Reloads the HTML page.
        public void ReloadPage()
        {
            _browserHost.Browser.Reload();
        }

        //Clears the cookies from the CEF browser.
        public void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }

        public override MyGuiControlBase HandleInput()
        {
            if (!IsBrowserInitialized)
                return null!;

            var input = MyInput.Static;

            if (!input.IsAnyCtrlKeyPressed() || !input.IsNewKeyPressed(MyKeys.R))
                return base.HandleInput();

            ReloadPage();
            if (MyInput.Static.IsAnyShiftKeyPressed())
                ClearCookies();

            return base.HandleInput();

        }
    }
}