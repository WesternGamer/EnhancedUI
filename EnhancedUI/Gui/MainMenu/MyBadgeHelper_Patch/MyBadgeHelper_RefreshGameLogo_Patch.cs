using HarmonyLib;
using SpaceEngineers.Game.GUI;

namespace EnhancedUI.Gui.MainMenu.MyBadgeHelper_Patch
{
    [HarmonyPatch(typeof(MyBadgeHelper), "RefreshGameLogo")]
    internal class MyBadgeHelper_RefreshGameLogo_Patch
    {
        private static bool Prefix() => false;
    }
}
