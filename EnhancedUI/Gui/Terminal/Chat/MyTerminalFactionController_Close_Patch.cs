using HarmonyLib;
using System;
using System.Reflection;

namespace EnhancedUI.Gui.Terminal.Chat
{
    [HarmonyPatch]
    internal static class MyTerminalChatController_Close_Patch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(Type.GetType("Sandbox.Game.Gui.MyTerminalChatController, Sandbox.Game", true), "Close");
        }

        private static bool Prefix() => false;
    }
}
