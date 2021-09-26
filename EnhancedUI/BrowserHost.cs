using System;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.OffScreen;
using VRageMath;
using VRageRender;

namespace EnhancedUI
{
    public class BrowserHost : IDisposable
    {
        public byte[] VideoData { get; private set; }

        public event Action? Ready;

        public readonly ChromiumWebBrowser Browser;

        public IBrowserHost Host => Browser.GetBrowserHost();

        public BrowserHost(Vector2I size)
        {
            VideoData = new byte[size.X * size.Y * 4];
            Browser = new ()
            {
                Size = new (size.X, size.Y)
            };
            Browser.Paint += BrowserOnPaint;
            Browser.BrowserInitialized += BrowserOnBrowserInitialized;
            Browser.LifeSpanHandler = new LifespanHandler();
        }

        public byte[] GetVideoData()
        {
            return VideoData;
        }

        private void BrowserOnBrowserInitialized(object sender, EventArgs e)
        {
            Ready.InvokeIfNotNull();
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

        private void BrowserOnPaint(object sender, OnPaintEventArgs e)
        {
            var videoData = VideoData;
            Marshal.Copy(e.BufferHandle, videoData, 0, e.Width * e.Height * 4);
            VideoData = videoData;

            e.Handled = true;
        }

        public void Dispose()
        {
            Browser.Paint -= BrowserOnPaint;
            Browser.BrowserInitialized -= BrowserOnBrowserInitialized;
            Browser.Dispose();
        }

        private class LifespanHandler : ILifeSpanHandler
        {
            bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
            {
                newBrowser = null!;
                return true;
            }

            bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
            { return false; }

            void ILifeSpanHandler.OnBeforeClose(IWebBrowser browserControl, IBrowser browser) { }

            void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser) { }
        }
    }
}