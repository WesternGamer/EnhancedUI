using System;
using CefSharp;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
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

        //Returns false if the browser is not initalized else it returns true.
        public bool IsBrowserInitialized => _browserHost.Browser.IsBrowserInitialized;

        public readonly MyGuiControlRotatingWheel Wheel = new (Vector2.Zero)
        {
            Visible = false
        };

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
            _browserHost.Navigate("file:///C:/redmrp/resources/redemrp_identity/html/ui.html");
            _videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VIDEO_NAME, 0);
        }

        //Removes the browser instance when ChromiumGuiControl is no longer needed.
        public override void OnRemoving()
        {
            base.OnRemoving();
            _browserHost.Ready -= BrowserHostOnReady;
            _browserHost.Browser.LoadingStateChanged -= BrowserOnLoadingStateChanged;
            _browserHost.Dispose();
            MyRenderProxy.CloseVideo(_videoId);
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

        public void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }
    }
}