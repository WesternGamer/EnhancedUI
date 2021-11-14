using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.ViewModels;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using VRage.FileSystem;
using VRage.Plugins;

namespace EnhancedUI
{
    // ReSharper disable once UnusedType.Global
    public class Main : IPlugin
    {
        private readonly List<IWebPageViewModel> instances = new();

        /// <summary>
        /// Called earlier than Init.
        /// </summary>
        public Main()
        {
            //This is the earliest point that Harmony can initialize.
            new Harmony("EnhancedUI").PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Dispose()
        {
            Cef.Shutdown();
        }

        public void Init(object gameInstance)
        {
            CefSettings? settings = new CefSettings
            {
                CachePath = Path.Combine(MyFileSystem.CachePath, "CefCache"),
                CommandLineArgsDisabled = true
            };
            settings.DisableGpuAcceleration();

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            Cef.Initialize(settings, true, browserProcessHandler: null);

            InitViewModels();
        }

        private void InitViewModels()
        {
            foreach (Type? type in GetAllTypesThatImplementInterface<IWebPageViewModel>())
            {
                IWebPageViewModel? instance = (IWebPageViewModel)Activator.CreateInstance(type);

                instances.Add(instance);
            }
        }

        private IEnumerable<Type> GetAllTypesThatImplementInterface<T>()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
        }

        public void Update()
        {
            if (instances != null)
            {
                foreach (IWebPageViewModel type in instances)
                {
                    type.Update();
                }
            }

        }
    }
}