using EnhancedUI.ViewModel;
using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Gui;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    [HarmonyPatch]
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once InconsistentNaming
    internal static class MyTerminalControlPanel_Init_Patch
    {
        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalControlPanel, Sandbox.Game", true), "Init");
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private static bool Prefix()
        {
            if (MyGuiScreenTerminal.InteractedEntity is MyTerminalBlock block)
            {
                TerminalViewModel.Instance?.Connect(block);
            }
            return false;
        }
    }
}