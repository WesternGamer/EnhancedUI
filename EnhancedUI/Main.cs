using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.ViewModels;
using HarmonyLib;
using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using VRage;
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
            #region CEFSettings

            //Deletes the CEF log file so it's size does not pile up overtime.
            File.Delete(Path.Combine(MyFileSystem.UserDataPath, "Storage\\PluginData\\EnhancedUI\\EnhancedUI_CEF.log"));

            //Sets settings for Cef.
            CefSettings settings = new()
            {
                CommandLineArgsDisabled = true,
                CachePath = Path.Combine(MyFileSystem.CachePath, "PluginCache\\EnhancedUI"),
                UserDataPath = Path.Combine(MyFileSystem.UserDataPath, "Storage\\PluginData\\EnhancedUI\\UserData"),
                LogFile = Path.Combine(MyFileSystem.UserDataPath, "Storage\\PluginData\\EnhancedUI\\EnhancedUI_CEF.log"),
#if DEBUG
                LogSeverity = LogSeverity.Verbose,
#else
                LogSeverity = LogSeverity.Warning,
#endif
            };

            //Language selection.
            switch (MySandboxGame.Config.Language)
            {
                case MyLanguagesEnum.English:
                    settings.Locale = "en-US";
                    break;
                case MyLanguagesEnum.Czech:
                    settings.Locale = "cs";
                    break;
                case MyLanguagesEnum.Slovak:
                    settings.Locale = "sk";
                    break;
                case MyLanguagesEnum.German:
                    settings.Locale = "de";
                    break;
                case MyLanguagesEnum.Russian:
                    settings.Locale = "ru";
                    break;
                case MyLanguagesEnum.Spanish_Spain:
                    settings.Locale = "es";
                    break;
                case MyLanguagesEnum.French:
                    settings.Locale = "fr";
                    break;
                case MyLanguagesEnum.Italian:
                    settings.Locale = "it";
                    break;
                case MyLanguagesEnum.Danish:
                    settings.Locale = "da";
                    break;
                case MyLanguagesEnum.Dutch:
                    settings.Locale = "nl";
                    break;
                case MyLanguagesEnum.Icelandic:
                    settings.Locale = "en-US"; // Icelandic locale is not in CEF so en-US will be used instead.
                    break;
                case MyLanguagesEnum.Polish:
                    settings.Locale = "pl";
                    break;
                case MyLanguagesEnum.Finnish:
                    settings.Locale = "fi";
                    break;
                case MyLanguagesEnum.Hungarian:
                    settings.Locale = "hu";
                    break;
                case MyLanguagesEnum.Portuguese_Brazil:
                    settings.Locale = "pt_BR";
                    break;
                case MyLanguagesEnum.Estonian:
                    settings.Locale = "et";
                    break;
                case MyLanguagesEnum.Norwegian:
                    settings.Locale = "en-US"; // Norwegian locale is not in CEF so en-US will be used instead.
                    break;
                case MyLanguagesEnum.Spanish_HispanicAmerica:
                    settings.Locale = "es-419";
                    break;
                case MyLanguagesEnum.Swedish:
                    settings.Locale = "sv";
                    break;
                case MyLanguagesEnum.Catalan:
                    settings.Locale = "ca";
                    break;
                case MyLanguagesEnum.Croatian:
                    settings.Locale = "hr";
                    break;
                case MyLanguagesEnum.Romanian:
                    settings.Locale = "ro";
                    break;
                case MyLanguagesEnum.Ukrainian:
                    settings.Locale = "uk";
                    break;
                case MyLanguagesEnum.Turkish:
                    settings.Locale = "tr";
                    break;
                case MyLanguagesEnum.Latvian:
                    settings.Locale = "lv";
                    break;
                case MyLanguagesEnum.ChineseChina:
                    settings.Locale = "zh_CN";
                    break;
                case MyLanguagesEnum.Japanese:
                    settings.Locale = "ja";
                    break;
            }

            //TODO: Should we have this? Keeping Gpu Acceleration enabled did not impact performance and loaded the webpages a litte but faster.
            settings.DisableGpuAcceleration();

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            #endregion
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