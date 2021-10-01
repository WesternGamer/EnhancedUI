using System;
using System.Reflection;
using HarmonyLib;

namespace EnhancedUI.Gui.Terminal.Inventory
{
    [HarmonyPatch]
    // ReSharper disable once UnusedType.Global
    internal static class InputInventoryPatch
    {
        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalInventoryController, Sandbox.Game", true), "HandleInput");
        }

        // ReSharper disable once UnusedMember.Local
        private static bool Prefix() => false;
    }
}