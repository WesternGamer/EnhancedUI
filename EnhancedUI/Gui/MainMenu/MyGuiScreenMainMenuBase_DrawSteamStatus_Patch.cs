using HarmonyLib;
using Sandbox.Game.Screens;

namespace EnhancedUI.Gui.MainMenu
{
    [HarmonyPatch(typeof(MyGuiScreenMainMenuBase), "DrawSteamStatus")]
    internal class MyGuiScreenMainMenuBase_DrawSteamStatus_Patch
    {
        private static bool Prefix() => false;
    }
}
