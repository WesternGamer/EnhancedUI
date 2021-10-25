using System;
using System.Reflection;
using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Gui;

namespace EnhancedUI.Gui.Terminal.Inventory
{
    [HarmonyPatch]
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once InconsistentNaming
    internal static class MyTerminalInventoryController_Init_Patch
    {
        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalInventoryController, Sandbox.Game", true), "Init");
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private static bool Prefix()
        {
            if (MyGuiScreenTerminal.InteractedEntity is MyTerminalBlock block)
            {
                InventoryState.Instance?.Init(block);
            }
            return false;
        }
    }
}