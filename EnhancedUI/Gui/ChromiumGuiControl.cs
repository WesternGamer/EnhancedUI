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
            _browserHost.Navigate("file:///C:/redmrp/resources/redemrp_clothing/ui/index.html");
            _videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VIDEO_NAME, 0);
        }

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

        private Rectangle GetScreenSize()
        {
            var pos = (Vector2I)MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(GetPositionAbsoluteTopLeft());

            var size = (Vector2I)MyGuiManager.GetScreenSizeFromNormalizedSize(Size);

            return new (pos.X, pos.Y, size.X, size.Y);
        }

        public override void Draw(float transitionAlpha, float backgroundTransitionAlpha)
        {
            if (!MyRenderProxy.IsVideoValid(_videoId))
                return;

            _browserHost.Draw();
            MyRenderProxy.UpdateVideo(_videoId);
            MyRenderProxy.DrawVideo(_videoId, GetScreenSize(), new (Vector4.One),
                MyVideoRectangleFitMode.AutoFit, false);
        }

        public void ReloadPage()
        {
            _browserHost.Browser.Reload();
        }
    }
}