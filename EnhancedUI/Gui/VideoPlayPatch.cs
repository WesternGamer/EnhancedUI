using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using HarmonyLib;
using VRageRender;

namespace EnhancedUI.Gui
{
    // Patch to allow loading HTML files using the video player
    [HarmonyPatch]
    internal static class VideoPlayPatch
    {
        private static readonly Type FactoryType = Type.GetType("VRageRender.MyVideoFactory, VRage.Render11", true);
        private static readonly Type PlayerType = Type.GetType("VRageRender.MyVideoPlayer, VRage.Render11", true);

        private static readonly MethodBase GetByIdMethod = AccessTools.Method(FactoryType, "GetVideo");
        private static readonly MethodBase InitMethod = AccessTools.Method(PlayerType, "Init");

        public const string VideoName = "CefFrame";

        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(FactoryType, "Play");
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        // ReSharper disable once UnusedMember.Local
        private static bool Prefix(uint id, string videoFile)
        {
            if (videoFile != VideoName)
                return true;

            var video = GetByIdMethod.Invoke(null, new object[] { id });
            if (video is null || ChromiumGuiControl.Player is null)
                return false;

            try
            {
                lock (video)
                {
                    InitMethod.Invoke(video, new object[] { videoFile, ChromiumGuiControl.Player });
                }
            }
            catch (Exception e)
            {
                MyRenderProxy.Log.WriteLine(e);
            }

            return false;
        }
    }
}