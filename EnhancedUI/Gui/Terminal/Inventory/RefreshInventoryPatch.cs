using System;
using System.Reflection;
using HarmonyLib;

namespace EnhancedUI.Gui.Terminal.Inventory
{
    [HarmonyPatch]
    // ReSharper disable once UnusedType.Global
    internal static class RefreshInventoryPatch
    {
        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalInventoryController, Sandbox.Game", true), "Refresh");
        }

        // ReSharper disable once UnusedMember.Local
        private static bool Prefix() => false;
    }
}
