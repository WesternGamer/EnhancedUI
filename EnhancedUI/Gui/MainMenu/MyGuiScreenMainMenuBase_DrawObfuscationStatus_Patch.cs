using HarmonyLib;
using Sandbox.Game.Screens;

namespace EnhancedUI.Gui.MainMenu
{
    [HarmonyPatch(typeof(MyGuiScreenMainMenuBase), "DrawObfuscationStatus")]
    internal class MyGuiScreenMainMenuBase_DrawObfuscationStatus_Patch
    {
        private static bool Prefix() => false;
    }
}
