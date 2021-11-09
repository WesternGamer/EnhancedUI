using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.ViewModel;
using HarmonyLib;
using Sandbox;
using System.IO;
using System.Reflection;
using VRage;
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
            CefSettings settings = new()
            {
                CachePath = Path.Combine(MyFileSystem.CachePath, "CefCache"),
                CommandLineArgsDisabled = true
            };

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