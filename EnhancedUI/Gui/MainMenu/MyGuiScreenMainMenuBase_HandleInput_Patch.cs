using HarmonyLib;
using Sandbox.Game.Screens;

namespace EnhancedUI.Gui.MainMenu
{
    [HarmonyPatch(typeof(MyGuiScreenMainMenuBase), "HandleInput")]
    internal class MyGuiScreenMainMenuBase_HandleInput_Patch
    {
        private static bool Prefix() => false;
    }
}
