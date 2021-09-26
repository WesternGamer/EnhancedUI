using System;
using System.IO;
using System.Reflection;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.Gui;
using EnhancedUI.Utils;
using HarmonyLib;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Internal;
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