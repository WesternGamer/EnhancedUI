using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Info
{
    [HarmonyPatch]
    internal static class MyTerminalInfoController_UpdateBeforeDraw_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalInfoController, Sandbox.Game", true), "UpdateBeforeDraw");
        }

        private static bool Prefix() => false;
    }
}
