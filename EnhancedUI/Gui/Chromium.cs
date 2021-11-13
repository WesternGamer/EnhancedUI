using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.ViewModel;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using VRageMath;

namespace EnhancedUI.Gui
{
    public class Chromium : IDisposable
    {
        private readonly byte[] videoData;

        public event Action? Ready;

        public readonly ChromiumWebBrowser Browser;

        public Chromium(Vector2I size)
        {
            videoData = new byte[size.X * size.Y * 4];

            Browser = new ChromiumWebBrowser
            {
                Size = new Size(size.X, size.Y),
                LifeSpanHandler = new LifespanHandler()
            };

            Browser.MenuHandler = new CustomMenuHandler();

            Browser.Paint += BrowserOnPaint;
            Browser.BrowserInitialized += BrowserOnBrowserInitialized;

            //C# to Js implementation
            Browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            {
                IJavascriptObjectRepository? repo = e.ObjectRepository;
                if (e.ObjectName == "WebPageViewModel")
                {
                    // No CamelCase of Javascript Names
                    repo.NameConverter = null;
                    repo.Register("WebPageViewModel", WebPageViewModel.Instance, isAsync: true, options: BindingOptions.DefaultBinder);
                }
            };
        }

        public byte[] GetVideoData()
        {
            return videoData;
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
            {
                Browser.GetBrowserHost().Invalidate(PaintElementType.View);
            }
        }

        private void BrowserOnPaint(object sender, OnPaintEventArgs e)
        {
            Marshal.Copy(e.BufferHandle, videoData, 0, e.Width * e.Height * 4);
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