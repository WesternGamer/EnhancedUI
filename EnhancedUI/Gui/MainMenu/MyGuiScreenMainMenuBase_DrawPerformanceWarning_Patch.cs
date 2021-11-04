using HarmonyLib;
using Sandbox.Game.Screens;

namespace EnhancedUI.Gui.MainMenu
{
    [HarmonyPatch(typeof(MyGuiScreenMainMenuBase), "DrawPerformanceWarning")]
    internal class MyGuiScreenMainMenuBase_DrawPerformanceWarning_Patch
    {
        private static bool Prefix() => false;
    }
}
