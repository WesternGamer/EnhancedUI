using System.IO;
using System.Reflection;
using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.ViewModel;
using HarmonyLib;
using VRage.FileSystem;
using VRage.Plugins;

namespace EnhancedUI
{
    // ReSharper disable once UnusedType.Global
    /// <summary>
    /// Main entry point of the plugin.
    /// </summary>
    public class Main : IPlugin
    {
        /// <summary>
        /// Single instance of the view model, reused for all browser instances
        /// </summary>
        private readonly TerminalViewModel model = new();

        public void Dispose()
        {
            Cef.Shutdown();
        }

        public void Init(object gameInstance)
        {
            new Harmony("EnhancedUI").PatchAll(Assembly.GetExecutingAssembly());

            //Sets settings for Cef.
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
            //TerminalViewModel.Update
            model.Update();
        }
    }
}