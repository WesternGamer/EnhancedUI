using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using VRageMath;

namespace EnhancedUI.Gui.LoadingMenu
{
    [HarmonyPatch(typeof(MyGuiScreenLoading), "RecreateControls")]
    internal class MyGuiScreenLoading_Patch
    {
        private const string Name = "LoadingMenu";
        private static readonly WebContent Content = new();

        [HarmonyPatch("RecreateControls")]
        [HarmonyPrefix]
        // ReSharper disable once UnusedMember.Local
        private static bool RecreateControlsPrefix(MyGuiScreenLoading __instance, ref MyGuiControlRotatingWheel ___m_wheel)
        {
            ___m_wheel = new MyGuiControlRotatingWheel(new Vector2(-0.1f, -0.1f));
            
            var control = new ChromiumGuiControl(Content, Name)
            {
                Position = new Vector2(0.50f, 0.50f),
                Size = new Vector2(1.331f, 1.0f)
            };

            // Adds the GUI elements to the screen
            __instance.Controls.Add(___m_wheel);
            __instance.Controls.Add(control);
            return false;
        }
    }
}
