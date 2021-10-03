using CefSharp;
using CefSharp.OffScreen;

namespace EnhancedUI.Gui
{
    public class PanelState : IPanelState
    {
        // Stores a reference to the browser, required to invoke JavaScript code (send events from C# to JS)
        protected ChromiumWebBrowser? Browser;

        // True value indicates that the state has been bound to a JS accessible global variable already
        private bool bound;

        public void SetBrowser(ChromiumWebBrowser? browser)
        {
            Browser = browser;
        }

        // Checks whether the state has been bound to a JS accessible global variable already
        public bool HasBound()
        {
            return bound && Browser?.IsBrowserInitialized == true;
        }

        // Invoked by JS code after the state is bound to a global variable
        public virtual void NotifyBound()
        {
            bound = true;
        }

        // Reloads the page, which requires re-binding the state to a JS global variable
        public virtual void Reload()
        {
            bound = false;
            Browser?.Reload();
        }
    }
}