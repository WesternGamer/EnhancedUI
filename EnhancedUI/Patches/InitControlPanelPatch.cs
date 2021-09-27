using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace EnhancedUI.Patches
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    internal static class InitControlPanelPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalControlPanel, Sandbox.Game", true), "Init");
        }

        private static bool Prefix() => false;
    }
}
