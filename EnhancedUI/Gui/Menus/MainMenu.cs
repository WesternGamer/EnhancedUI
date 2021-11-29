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
#if !DEBUG
        private const string WebPageName = "MainMenu\\MainMenu";
#else
        private const string WebPageName = "Debug\\MainMenu\\MainMenu";
#endif
        private static readonly WebContent Content = new();

        private ChromiumGuiControl? Control;

        public MainMenu() : base(Vector2.Zero)
        {
            m_closeOnEsc = false;
            CanBeHidden = true;
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
            Control = control;


            // Adds the GUI elements to the screen
            Controls.Add(control.Wheel);
            Controls.Add(control);
            control.CanAutoFocusOnInputHandling = true;
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

        public override void OnRemoved()
        {
            Control.OnRemoving();
            base.OnRemoved();
        }
    }
}
