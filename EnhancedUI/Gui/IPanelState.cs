using CefSharp.OffScreen;

namespace EnhancedUI.Gui
{
    public interface IPanelState
    {
        // Access to the browser instance is required to invoke JavaScript
        void SetBrowser(ChromiumWebBrowser? browser);

        // Return True if the page has been loaded
        bool HasBound();

        // Marks the page as loaded
        void NotifyBound();

        // Reloads the page
        void Reload();
    }
}