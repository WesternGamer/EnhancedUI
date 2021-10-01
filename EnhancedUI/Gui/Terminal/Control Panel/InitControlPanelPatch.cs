using System;
using System.Reflection;
using HarmonyLib;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    [HarmonyPatch]
    // ReSharper disable once UnusedType.Global
    internal static class InitControlPanelPatch
    {
        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalControlPanel, Sandbox.Game", true), "Init");
        }

        // ReSharper disable once UnusedMember.Local
        private static bool Prefix() => false;
    }
}