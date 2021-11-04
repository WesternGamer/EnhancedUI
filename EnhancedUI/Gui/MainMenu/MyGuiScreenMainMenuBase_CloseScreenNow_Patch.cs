using HarmonyLib;
using SpaceEngineers.Game.GUI;

namespace EnhancedUI.Gui.MainMenu
{
    /// <summary>
    /// This patch is to prevent exceptions at runtime when MyGuiScreenMainMenu.AddIntroScreen is patched by MyGuiScreenMainMenu_AddIntroScreen_Patch.
    /// </summary>
    [HarmonyPatch(typeof(MyGuiScreenMainMenu), "CloseScreenNow")]
    internal class MyGuiScreenMainMenuBase_CloseScreenNow_Patch
    {
        private static bool Prefix() => false;
    }
}
