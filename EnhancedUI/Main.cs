using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using VRage.FileSystem;
using VRage.Plugins;

[assembly: SuppressMessage("Apeira.StyleCop", "SA1600", Justification = "Documentation is not nessesary for plugins, because they cannot have dependecies")]
[assembly: SuppressMessage("Apeira.StyleCop", "SA1601", Justification = "Documentation is not nessesary for plugins, because they cannot have dependecies")]

namespace EnhancedUI
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
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

#if !DEBUG
            new Thread(BuildWebServer)
            {
                Name = "EnchancedUI WebServer",
                IsBackground = true
            }.Start();
#endif
        }

        public void Update()
        {
        }

        private static void BuildWebServer()
        {
            WebHost.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                .Configure(config => config.UseDefaultFiles().UseStaticFiles())
                .UseUrls("http://localhost:3000/")
                .UseWebRoot(Path.Combine(FileSystem.GetPluginsDir(), "Content")).Build().Run();
        }
    }
}
