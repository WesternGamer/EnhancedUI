using HarmonyLib;
using Sandbox.Game.Gui;

namespace EnhancedUI.Gui.LoadingMenu
{
    [HarmonyPatch(typeof(MyGuiScreenLoading), "DrawInternal")]
    internal class MyGuiScreenLoading_DrawInternal_Patch
    {
        private static bool Prefix() => false;
    }
}
