using System.IO;
using System.Reflection;
using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.Gui;
using HarmonyLib;
using VRage.FileSystem;
using VRage.Plugins;

namespace EnhancedUI
{
    // ReSharper disable once UnusedType.Global
    public class Main : IPlugin
    {
        public void Dispose()
        {
            ChromiumGuiControl.DisposeBrowsers();
            Cef.Shutdown();
        }

        public void Init(object gameInstance)
        {
            new Harmony("EnhancedUI").PatchAll(Assembly.GetExecutingAssembly());

            var settings = new CefSettings
            {
                CachePath = Path.Combine(MyFileSystem.CachePath, "CefCache"),
                CommandLineArgsDisabled = true
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