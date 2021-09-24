using System.IO;
using System.Reflection;
using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.Gui;
using HarmonyLib;
using Sandbox.Graphics.GUI;
using VRage.FileSystem;
using VRage.Input;
using VRage.Plugins;

namespace EnhancedUI
{
    public class Main : IPlugin
    {
        public void Dispose()
        {
            Cef.Shutdown();
        }

        public void Init(object gameInstance)
        {
            new Harmony("EnhancedUI").PatchAll(Assembly.GetExecutingAssembly());

            var settings = new CefSettings
            {
                CachePath = Path.Combine(MyFileSystem.CachePath, "CefCache"),
                CommandLineArgsDisabled = true,
            };
            settings.DisableGpuAcceleration();

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            Cef.Initialize(settings, true, browserProcessHandler: null);
        }

        public void Update()
        {
        }
    }
}