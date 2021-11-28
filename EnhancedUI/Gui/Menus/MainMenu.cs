using EnhancedUI.Gui.Browser;
using EnhancedUI.Gui.HtmlGuiControl;
using EnhancedUI.ViewModels.MainMenuViewModel;
using Sandbox.Graphics.GUI;
using VRageMath;

namespace EnhancedUI.Gui.Menus
{
    internal class MainMenu : MyGuiScreenBase
    {
        public override MyGuiControls Controls => base.Controls;

        private const string WebPageName = "MainMenu\\MainMenu";

        private static readonly WebContent Content = new();

        public MainMenu() : base(Vector2.Zero)
        {
            m_closeOnEsc = false;
        }

        public override string GetFriendlyName()
        {
            return "WebMenu";
        }

        public override void RecreateControls(bool constructor)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            ChromiumGuiControl control = new ChromiumGuiControl(Content, WebPageName, "MainMenuViewModel", MainMenuViewModel.Instance)
#pragma warning restore CS8604 // Possible null reference argument.
            {
                Position = new Vector2(0.50f, 0.50f),
                Size = new Vector2(1.331f, 1.0f)
            };


            // Adds the GUI elements to the screen
            Controls.Add(control.Wheel);
            Controls.Add(control);

        }

        public override bool RegisterClicks()
        {
            return true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }
    }
}
