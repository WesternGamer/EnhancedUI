using HarmonyLib;
using Sandbox.Game.Screens;

namespace EnhancedUI.Gui.MainMenu
{
    [HarmonyPatch(typeof(MyGuiScreenMainMenuBase), "DrawAppVersion")]
    internal class MyGuiScreenMainMenuBase_DrawAppVersion_Patch
    {
        private static bool Prefix() => false;
    }
}
