using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Chat
{
    [HarmonyPatch]
    internal static class MyTerminalFactionController_Init_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalChatController, Sandbox.Game", true), "Init");
        }

        private static bool Prefix()
        {
            ChatState.Instance?.Init();
            return false;
        }
    }
}
