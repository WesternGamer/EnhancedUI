using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using EnhancedUI.Gui;
using HarmonyLib;
using JetBrains.Annotations;
using VRageRender;

namespace EnhancedUI.Patches
{
    // Patch to allow loading HTML files using the video player
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [HarmonyPatch]
    internal static class VideoPlayPatch
    {
        public const string VIDEO_NAME = "CefFrame";

        private static readonly Type _factoryType = Type.GetType("VRageRender.MyVideoFactory, VRage.Render11", true);
        private static readonly Type _playerType = Type.GetType("VRageRender.MyVideoPlayer, VRage.Render11", true);

        private static readonly MethodBase _getByIdMethod = AccessTools.Method(_factoryType, "GetVideo");
        private static readonly MethodBase _initMethod = AccessTools.Method(_playerType, "Init");

        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(_factoryType, "Play");
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static bool Prefix(uint id, string videoFile)
        {
            if (videoFile != VIDEO_NAME)
                return true;

            var video = _getByIdMethod.Invoke(null, new object[] { id });
            if (video is null || ChromiumGuiControl.Player is null)
                return false;

            try
            {
                lock (video)
                {
                    _initMethod.Invoke(video, new object[] { videoFile, ChromiumGuiControl.Player });
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
