using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Production
{
    [HarmonyPatch]
    internal static class MyTerminalProductionController_Close_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalProductionController, Sandbox.Game", true), "Close");
        }

        private static bool Prefix() => false;
    }
}
