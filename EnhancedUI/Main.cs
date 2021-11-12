using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.ViewModel;
using HarmonyLib;
using System.IO;
using System.Reflection;
using VRage.FileSystem;
using VRage.Plugins;

namespace EnhancedUI
{
    // ReSharper disable once UnusedType.Global
    public class Main : IPlugin
    {
        // Single instance of the view model, reused for all browser instances
        private readonly TerminalViewModel model = new();

        /// <summary>
        /// Called earlier than Init.
        /// </summary>
        public Main()
        {
            //This is the earliest point that Harmony can initialize.
            new Harmony("EnhancedUI").PatchAll(Assembly.GetExecutingAssembly());

            CefSettings? settings = new CefSettings
            {
                CachePath = Path.Combine(MyFileSystem.CachePath, "CefCache"),
                CommandLineArgsDisabled = true
            };
            settings.DisableGpuAcceleration();

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            Cef.Initialize(settings, true, browserProcessHandler: null);
        }

        public void Dispose()
        {
            Cef.Shutdown();
        }

        public void Init(object gameInstance)
        {

        }

        public void Update()
        {
            model.Update();
        }
    }
}