using HarmonyLib;
using SpaceEngineers.Game.GUI;

namespace EnhancedUI.Gui.MainMenu
{
    /// <summary>
    /// Disables the background video on the Main Menu
    /// </summary>
    [HarmonyPatch(typeof(MyGuiScreenMainMenu), "AddIntroScreen")]
    internal class MyGuiScreenMainMenu_AddIntroScreen_Patch
    {
        private static bool Prefix() => false;
    }
}
