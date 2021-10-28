using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Factions
{
    [HarmonyPatch]
    internal static class MyTerminalFactionController_Close_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalFactionController, Sandbox.Game", true), "Close");
        }

        private static bool Prefix() => false;
    }
}
