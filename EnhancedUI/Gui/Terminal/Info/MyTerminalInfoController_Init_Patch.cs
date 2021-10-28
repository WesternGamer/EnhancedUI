using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Gui;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Production
{
    [HarmonyPatch]
    internal static class MyTerminalInfoController_Init_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalInfoController, Sandbox.Game", true), "Init");
        }

        private static bool Prefix()
        {
            if (MyGuiScreenTerminal.InteractedEntity is MyTerminalBlock block)
            {
                ProductionState.Instance?.Init(block);
            }
            return false;
        }
    }
}
