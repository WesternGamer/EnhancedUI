using CefSharp;
using CefSharp.OffScreen;

namespace EnhancedUI.Gui
{
    public class BrowserViewModel: IBrowserViewModel
    {
        public ChromiumWebBrowser? Browser;
        private bool loaded;

        public void SetBrowser(ChromiumWebBrowser? browser)
        {
            Browser = browser;
        }

        public bool HasLoaded()
        {
            return loaded && Browser?.IsBrowserInitialized == true;
        }

        public virtual void MarkLoaded()
        {
            loaded = true;
        }

        public virtual void Reload()
        {
            loaded = false;
            Browser?.Reload();
        }
    }
}