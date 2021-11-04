using HarmonyLib;
using SpaceEngineers.Game.GUI;

namespace EnhancedUI.Gui.MainMenu
{
    [HarmonyPatch(typeof(MyGuiScreenMainMenu), "OnClickExitToWindows")]
    internal class MyGuiScreenMainMenu_OnClickExitToWindows_Patch
    {
        private static bool Prefix() => false;
    }
}
