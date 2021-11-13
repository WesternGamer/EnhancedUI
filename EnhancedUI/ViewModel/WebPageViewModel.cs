
using Sandbox;
using Sandbox.Graphics.GUI;
using System;
using VRage.Input;

namespace EnhancedUI.ViewModel
{
    public class WebPageViewModel : IWebPageViewModel, IDisposable
    {
        // Model is a singleton
        public static WebPageViewModel? Instance;

        public WebPageViewModel()
        {
            if (Instance != null)
            {
                throw new Exception("This is a singleton");
            }

            Instance = this;
        }

        public void Dispose()
        {
            
        }

        public void Exit()
        {
            MySandboxGame.Config.ControllerDefaultOnStart = MyInput.Static.IsJoystickLastUsed;
            MySandboxGame.Config.Save();
            MyScreenManager.CloseAllScreensNowExcept(null);
            MySandboxGame.ExitThreadSafe();
        }
    }
}