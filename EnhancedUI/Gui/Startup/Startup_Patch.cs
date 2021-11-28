using EnhancedUI.Gui.Menus;
using HarmonyLib;
using Sandbox;
using Sandbox.Engine.Networking;
using Sandbox.Game;
using Sandbox.Game.GUI;
using Sandbox.Game.Localization;
using Sandbox.Graphics.GUI;
using VRage;
using VRage.Audio;
using VRage.Input;

namespace EnhancedUI.Gui.Startup
{
    [HarmonyPatch(typeof(MySandboxGame), "InitQuickLaunch")]
    internal class Startup_Patch
    {
        private static bool Prefix(MySandboxGame __instance, ref bool __result)
        {
            if (MyVRage.Platform.Windows.Window != null)
            {
                MyVRage.Platform.Windows.Window.ShowAndFocus();
            }
            new MyWorkshop.CancelToken();
            if (MyPlatformGameSettings.GAME_SAVES_TO_CLOUD)
            {
                MySandboxGame.Config.LoadFromCloud(true, delegate
                {
                    __instance.UpdateUIScale();
                    MyAudio.Static.EnableDoppler = MySandboxGame.Config.EnableDoppler;
                    MyAudio.Static.VolumeMusic = MySandboxGame.Config.MusicVolume;
                    MyAudio.Static.VolumeGame = MySandboxGame.Config.GameVolume;
                    MyAudio.Static.VolumeHud = MySandboxGame.Config.GameVolume;
                    MyAudio.Static.VolumeVoiceChat = MySandboxGame.Config.VoiceChatVolume;
                    MyAudio.Static.EnableVoiceChat = MySandboxGame.Config.EnableVoiceChat;
                    MyGuiAudio.HudWarnings = MySandboxGame.Config.HudWarnings;
                    MyVRage.Platform.Input.MouseCapture = MySandboxGame.Config.CaptureMouse && MySandboxGame.Config.WindowMode != MyWindowModeEnum.Fullscreen;
                    MyLanguage.CurrentLanguage = MySandboxGame.Config.Language;
                    MyInput.Static.LoadControls(MySandboxGame.Config.ControlsGeneral, MySandboxGame.Config.ControlsButtons);
                    MyLocalCache.UpdateLastSessionFromCloud();
                });
            }
            MyGuiSandbox.AddScreen(new MainMenu());
            __result = true;
            return false;
        }
    }
}
