using EnhancedUI.Gui.Browser;
using EnhancedUI.Gui.HtmlGuiControl;
using EnhancedUI.ViewModels.NewGameMenuViewModel;
using Sandbox;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace EnhancedUI.Gui.Menus
{
    internal class NewGameMenu : MyGuiScreenBase
    {
        public override MyGuiControls Controls => base.Controls;
#if !DEBUG
        private const string WebPageName = "NewGameMenu\\NewGameMenu";
#else
        private const string WebPageName = "Debug\\NewGameMenu\\NewGameMenu";
#endif
        private static readonly WebContent Content = new();

        public NewGameMenu() : base(Vector2.Zero)
        {
            base.EnabledBackgroundFade = true;
            RecreateControls(true);
        }

        public override string GetFriendlyName()
        {
            return "NewGameMenu";
        }

        public override void RecreateControls(bool constructor)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            ChromiumGuiControl control = new ChromiumGuiControl(Content, WebPageName, "NewGameMenuViewModel", NewGameMenuViewModel.Instance)
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
    }
}
