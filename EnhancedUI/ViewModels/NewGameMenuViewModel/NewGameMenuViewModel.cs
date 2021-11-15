using CefSharp;
using EnhancedUI.Gui.Browser;
using EnhancedUI.Gui.HtmlGuiControl;
using Sandbox;
using Sandbox.Graphics.GUI;
using System;
using System.Text;

namespace EnhancedUI.ViewModels.NewGameMenuViewModel
{
    internal class NewGameMenuViewModel : IWebPageViewModel
    {
        // Model is a singleton
        public static NewGameMenuViewModel? Instance;

        public NewGameMenuViewModel()
        {
            if (Instance != null)
            {
                throw new Exception("Only one instance of MainMenuViewModel can be open at a time.");
            }

            Instance = this;
        }

        public void Dispose()
        {
            
        }

        public void Update()
        {
            
        }
    }
}
