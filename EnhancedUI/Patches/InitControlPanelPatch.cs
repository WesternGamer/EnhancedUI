using System;
using System.Reflection;
using HarmonyLib;

namespace EnhancedUI.Gui
{
    [HarmonyPatch]
    internal static class InitControlPanelPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalControlPanel, Sandbox.Game", true), "Init");
        }

        private static bool Prefix() => false;
    }
}