using CefSharp.OffScreen;

namespace EnhancedUI.Gui
{
    public interface IPanelState
    {
        // Access to the browser instance is required to invoke JavaScript
        void SetBrowser(ChromiumWebBrowser? browser);

        // Return True if the page has been loaded
        // ReSharper disable once UnusedMemberInSuper.Global
        bool HasBound();

        // Marks the page as loaded
        // Invoked from JavaScript
        // ReSharper disable once UnusedMember.Global
        void NotifyBound();
    }
}