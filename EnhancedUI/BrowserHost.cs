using System;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.OffScreen;
using VRageMath;

namespace EnhancedUI
{
    public class BrowserHost : IDisposable
    {
        public BrowserHost(Vector2I size)
        {
            VideoData = new byte[size.X * size.Y * 4];
            Browser = new ()
            {
                Size = new (size.X, size.Y),
            };
            Browser.Paint += BrowserOnPaint;
            Browser.BrowserInitialized += BrowserOnBrowserInitialized;
            Browser.LifeSpanHandler = new LifespanHandler();
        }

        public event Action? Ready;

        public ChromiumWebBrowser Browser { get; }

        public byte[] VideoData { get; private set; }

        public IBrowserHost Host => Browser.GetBrowserHost();

        public byte[] GetVideoData()
        {
            return VideoData;
        }

        public void Navigate(string url)
        {
            Browser.Load(url);
        }

        public void Draw()
        {
            if (Browser.IsBrowserInitialized)
                Browser.GetBrowserHost().Invalidate(PaintElementType.View);
        }

        public void Dispose()
        {
            Browser.Paint -= BrowserOnPaint;
            Browser.BrowserInitialized -= BrowserOnBrowserInitialized;
            Browser.Dispose();
        }

        private void BrowserOnPaint(object sender, OnPaintEventArgs e)
        {
            var videoData = VideoData;
            Marshal.Copy(e.BufferHandle, videoData, 0, e.Width * e.Height * 4);
            VideoData = videoData;

            e.Handled = true;
        }

        private void BrowserOnBrowserInitialized(object sender, EventArgs e)
        {
            Ready.InvokeIfNotNull();
        }

        private class LifespanHandler : ILifeSpanHandler
        {
            bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
            {
                newBrowser = null!;
                return true;
            }

            bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
            {
                return false;
            }

            void ILifeSpanHandler.OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
            {
            }

            void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
            {
            }
        }
    }
}
