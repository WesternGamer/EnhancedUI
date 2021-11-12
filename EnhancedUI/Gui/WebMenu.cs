using Sandbox.Graphics.GUI;
using VRageMath;

namespace EnhancedUI.Gui
{
    internal class WebMenu : MyGuiScreenBase
    {
        public override MyGuiControls Controls => base.Controls;

        private const string WebPageName = "MainMenu";

        private static readonly WebContent Content = new();

        public WebMenu() : base(Vector2.Zero)
        {
            m_isTopMostScreen = true;
            m_closeOnEsc = false;
            m_drawEvenWithoutFocus = false;
        }

        public override string GetFriendlyName()
        {
            return "WebMenu";
        }

        public override void RecreateControls(bool constructor)
        {
            ChromiumGuiControl control = new ChromiumGuiControl(Content, WebPageName)
            {
                Position = new Vector2(0.50f, 0.50f),
                Size = new Vector2(1.331f, 1.0f)
            };


            // Adds the GUI elements to the screen
            Controls.Add(control);
            Controls.Add(control.Wheel);
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
