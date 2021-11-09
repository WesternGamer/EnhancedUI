using HarmonyLib;
using Sandbox.Game.Gui;

namespace EnhancedUI.Gui.LoadingMenu
{
    [HarmonyPatch(typeof(MyGuiScreenLoading), "OnClosed")]
    internal class MyGuiScreenLoading_OnClosed_Patch
    {
        private static void Postfix()
        {
            MyGuiScreenLoading_Patch.audioPlayer.FadeOut();
        }
    }
}
