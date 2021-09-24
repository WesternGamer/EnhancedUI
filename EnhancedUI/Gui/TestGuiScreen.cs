using Sandbox;
using Sandbox.Graphics.GUI;
using VRage.Utils;
using VRageMath;

namespace EnhancedUI.Gui
{
    public sealed class TestGuiScreen : MyGuiScreenBase
    {
        public override string GetFriendlyName() => nameof(TestGuiScreen);

        public TestGuiScreen() : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR,
            new Vector2(1.0157f, 0.9172f), true, null, MySandboxGame.Config.UIBkOpacity,
            MySandboxGame.Config.UIOpacity)
        {
            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);
            Controls.Add(new ChromiumGuiControl
            {
                Position = GetPositionAbsoluteTopLeft(),
                Size = m_size!.Value,
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP
            });
        }
    }
}