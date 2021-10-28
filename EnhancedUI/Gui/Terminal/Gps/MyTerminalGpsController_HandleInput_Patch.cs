using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Chat
{
    [HarmonyPatch]
    internal static class MyTerminalGpsController_HandleInput_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalGpsController, Sandbox.Game", true), "HandleInput");
        }

        private static bool Prefix() => false;
    }
}
