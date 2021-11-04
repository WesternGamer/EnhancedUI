using HarmonyLib;
using SpaceEngineers.Game.GUI;

namespace EnhancedUI.Gui.MainMenu.MyBadgeHelper_Patch
{
    [HarmonyPatch(typeof(MyBadgeHelper), "DrawGameLogo")]
    internal class MyBadgeHelper_DrawGameLogo_Patch
    {
        private static bool Prefix() => false;
    }
}
