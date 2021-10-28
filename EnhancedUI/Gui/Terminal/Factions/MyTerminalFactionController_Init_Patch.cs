using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Factions
{
    [HarmonyPatch]
    internal static class MyTerminalFactionController_Init_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalFactionController, Sandbox.Game", true), "Init");
        }

        private static bool Prefix()
        {
            FactionsState.Instance?.Init();
            return false;
        }
    }
}
