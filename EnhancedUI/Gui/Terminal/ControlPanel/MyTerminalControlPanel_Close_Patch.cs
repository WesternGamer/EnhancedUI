using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    [HarmonyPatch]
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once InconsistentNaming
    internal static class MyTerminalControlPanel_Patch
    {
        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalControlPanel, Sandbox.Game", true), "Close");
        }

        // ReSharper disable once UnusedMember.Local
        private static bool Prefix()
        {
            return false;
        }
    }
}