using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Gps
{
    [HarmonyPatch]
    internal static class MyTerminalGpsController_Close_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalGpsController, Sandbox.Game", true), "Close");
        }

        private static bool Prefix() => false;
    }
}
