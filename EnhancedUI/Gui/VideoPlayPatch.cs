using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using HarmonyLib;
using VRage.Utils;

namespace EnhancedUI.Gui
{
    /// <summary>
    /// Patch to allow loading HTML files using the video player
    /// </summary>
    [HarmonyPatch]
    internal static class VideoPlayPatch
    {
        private static readonly Type FactoryType = Type.GetType("VRageRender.MyVideoFactory, VRage.Render11", true);
        private static readonly Type PlayerType = Type.GetType("VRageRender.MyVideoPlayer, VRage.Render11", true);

        private static readonly MethodBase GetByIdMethod = AccessTools.Method(FactoryType, "GetVideo");
        private static readonly MethodBase InitMethod = AccessTools.Method(PlayerType, "Init");

        private static readonly Dictionary<string, BatchDataPlayer> Players = new Dictionary<string, BatchDataPlayer>();

        public const string VideoNamePrefix = "EnhancedUI_";

        public static void RegisterVideoPlayer(string name, BatchDataPlayer player)
        {
            name = VideoNamePrefix + name;

            if (Players.ContainsKey(name))
            {
                throw new Exception($"Video player already registered: {name}");
            }

            Players[name] = player;
        }

        public static void UnregisterVideoPlayer(string name)
        {
            name = VideoNamePrefix + name;

            if (!Players.ContainsKey(name))
            {
                return;
            }

            Players.Remove(name);
        }

        // ReSharper disable once UnusedMember.Local
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(FactoryType, "Play");
        }

        /// <summary>
        /// Patched version of MyVideoFactory.Play
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videoFile"></param>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        // ReSharper disable once UnusedMember.Local
        private static bool Prefix(uint id, string videoFile)
        {
            if (!Players.TryGetValue(videoFile, out var player))
            {
                return true;
            }

            var video = GetByIdMethod.Invoke(null, new object[] { id });
            if (video is null)
            {
                MyLog.Default.Error($"No EnhancedUI video found: videoFile={videoFile}, id={id}");
                return false;
            }

            try
            {
                lock (video)
                {
                    InitMethod.Invoke(video, new object[] { videoFile, player });
                }
            }
            catch (Exception e)
            {
                MyLog.Default.Error($"Failed to Init EnhancedUI video player: videoFile={videoFile}; id={id}");
                MyLog.Default.Error(e.ToString());
                return false;
            }

            MyLog.Default.Info($"Initialized EnhancedUI video player: videoFile={videoFile}; id={id}");

            return false;
        }
    }
}