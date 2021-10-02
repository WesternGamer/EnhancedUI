using CefSharp.OffScreen;

namespace EnhancedUI.Gui
{
    public interface IBrowserViewModel
    {
        // Assigns the current Chromium browser to the view model, required to invoke JavaScript
        void SetBrowser(ChromiumWebBrowser? browser);

        // Return True if the page has been loaded
        bool HasLoaded();

        // Marks the page as loaded
        void MarkLoaded();

        // Reloads the page
        void Reload();
    }
}