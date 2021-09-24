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
    public class Main : IHandleInputPlugin
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

        private bool _isOpen;
        public void HandleInput()
        {
            if (!MyInput.Static.IsAnyCtrlKeyPressed() || !MyInput.Static.IsKeyPress(MyKeys.C) || _isOpen)
                return;

            var screen = new TestGuiScreen();
            screen.Closed += ScreenOnClosed;
            MyScreenManager.AddScreen(screen);
            _isOpen = true;
        }

        private void ScreenOnClosed(MyGuiScreenBase source, bool isUnloading)
        {
            source.Closed -= ScreenOnClosed;
            _isOpen = false;
        }
    }
}