using EnhancedUI.Gui.Menus;
using ParallelTasks;
using Sandbox;
using Sandbox.Engine.Networking;
using Sandbox.Game;
using Sandbox.Game.Gui;
using Sandbox.Game.Localization;
using Sandbox.Game.Screens;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Text;
using VRage;
using VRage.Game;
using VRage.GameServices;
using VRage.Input;
using VRage.Utils;

namespace EnhancedUI.ViewModels.MainMenuViewModel
{
    public class MainMenuViewModel : IWebPageViewModel
    {
        // Model is a singleton
        public static MainMenuViewModel? Instance;

        private bool m_parallelLoadIsRunning;

        public MainMenuViewModel()
        {
            if (Instance != null)
            {
                throw new Exception("Only one instance of MainMenuViewModel can be open at a time.");
            }

            Instance = this;
        }

        public void ContinueLastGame()
        {
            MyObjectBuilder_LastSession mySession = MyLocalCache.GetLastSession();
            if (mySession == null)
            {
                return;
            }
            if (mySession.IsOnline)
            {
                if (mySession.IsLobby)
                {
                    MyJoinGameHelper.JoinGame(ulong.Parse(mySession.ServerIP));
                    return;
                }
                MyGameService.Service.RequestPermissions(Permissions.Multiplayer, attemptResolution: true, delegate (PermissionResult granted)
                {
                    switch (granted)
                    {
                        case PermissionResult.Granted:
                            MyGameService.Service.RequestPermissions(Permissions.CrossMultiplayer, attemptResolution: true, delegate (PermissionResult crossGranted)
                            {
                                switch (crossGranted)
                                {
                                    case PermissionResult.Granted:
                                        MyGameService.Service.RequestPermissions(Permissions.UGC, attemptResolution: true, delegate (PermissionResult ugcGranted)
                                        {
                                            switch (ugcGranted)
                                            {
                                                case PermissionResult.Granted:
                                                    JoinServer(mySession);
                                                    break;
                                                case PermissionResult.Error:
                                                    MySandboxGame.Static.Invoke(delegate
                                                    {
                                                        MyGuiSandbox.Show(MyCommonTexts.XBoxPermission_MultiplayerError, default(MyStringId), MyMessageBoxStyleEnum.Info);
                                                    }, "New Game screen");
                                                    break;
                                            }
                                        });
                                        break;
                                    case PermissionResult.Error:
                                        MySandboxGame.Static.Invoke(delegate
                                        {
                                            MyGuiSandbox.Show(MyCommonTexts.XBoxPermission_MultiplayerError, default(MyStringId), MyMessageBoxStyleEnum.Info);
                                        }, "New Game screen");
                                        break;
                                }
                            });
                            break;
                        case PermissionResult.Error:
                            MySandboxGame.Static.Invoke(delegate
                            {
                                MyGuiSandbox.Show(MyCommonTexts.XBoxPermission_MultiplayerError, default(MyStringId), MyMessageBoxStyleEnum.Info);
                            }, "New Game screen");
                            break;
                    }
                });
            }
            else if (!m_parallelLoadIsRunning)
            {
                m_parallelLoadIsRunning = true;
                MyGuiScreenProgress progressScreen = new MyGuiScreenProgress(MyTexts.Get(MySpaceTexts.ProgressScreen_LoadingWorld));
                MyScreenManager.AddScreen(progressScreen);
                Parallel.StartBackground(delegate
                {
                    MySessionLoader.LoadLastSession();
                }, delegate
                {
                    progressScreen.CloseScreen();
                    m_parallelLoadIsRunning = false;
                });
            }
        }

        private void JoinServer(MyObjectBuilder_LastSession mySession)
        {
            try
            {
                MyGuiScreenProgress prog = new MyGuiScreenProgress(MyTexts.Get(MyCommonTexts.DialogTextCheckServerStatus));
                MyGuiSandbox.AddScreen(prog);
                MyGameService.OnPingServerResponded += OnPingSuccess;
                MyGameService.OnPingServerFailedToRespond += OnPingFailure;
                MyGameService.PingServer(mySession.GetConnectionString());
                void OnPingFailure(object sender, object data)
                {
                    MyGuiSandbox.RemoveScreen(prog);
                    MySandboxGame.Static.ServerFailedToRespond(sender, data);
                    MyGameService.OnPingServerResponded -= OnPingSuccess;
                    MyGameService.OnPingServerFailedToRespond -= OnPingFailure;
                }
                void OnPingSuccess(object sender, MyGameServerItem item)
                {
                    MyGuiSandbox.RemoveScreen(prog);
                    MySandboxGame.Static.ServerResponded(sender, item);
                    MyGameService.OnPingServerResponded -= OnPingSuccess;
                    MyGameService.OnPingServerFailedToRespond -= OnPingFailure;
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLine(ex);
                MyGuiSandbox.Show(MyTexts.Get(MyCommonTexts.MultiplayerJoinIPError), MyCommonTexts.MessageBoxCaptionError);
            }
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

        public void NewGame()
        {
            MyGuiSandbox.AddScreen(MyGuiSandbox.CreateScreen<MyGuiScreenNewGame>(Array.Empty<object>()));
        }

        public void LoadGame()
        {
            MyGuiSandbox.AddScreen(new MyGuiScreenLoadSandbox());
        }

        public void JoinGame()
        {
            if (MyGameService.IsOnline)
            {
                MyGameService.Service.RequestPermissions(Permissions.Multiplayer, attemptResolution: true, delegate (PermissionResult granted)
                {
                    switch (granted)
                    {
                        case PermissionResult.Granted:
                            MyGameService.Service.RequestPermissions(Permissions.UGC, attemptResolution: true, delegate (PermissionResult ugcGranted)
                            {
                                switch (ugcGranted)
                                {
                                    case PermissionResult.Granted:
                                        MyGameService.Service.RequestPermissions(Permissions.CrossMultiplayer, attemptResolution: true, delegate (PermissionResult crossGranted)
                                        {
                                            MyGuiScreenJoinGame myGuiScreenJoinGame = new MyGuiScreenJoinGame(crossGranted == PermissionResult.Granted);
                                            myGuiScreenJoinGame.Closed += joinGameScreen_Closed;
                                            MyGuiSandbox.AddScreen(myGuiScreenJoinGame);
                                        });
                                        break;
                                    case PermissionResult.Error:
                                        MySandboxGame.Static.Invoke(delegate
                                        {
                                            MyGuiSandbox.Show(MyCommonTexts.XBoxPermission_MultiplayerError, default(MyStringId), MyMessageBoxStyleEnum.Info);
                                        }, "New Game screen");
                                        break;
                                }
                            });
                            break;
                        case PermissionResult.Error:
                            MyGuiSandbox.Show(MyCommonTexts.XBoxPermission_MultiplayerError, default(MyStringId), MyMessageBoxStyleEnum.Info);
                            break;
                    }
                });
            }
            else
            {
                MyGuiSandbox.AddScreen(MyGuiSandbox.CreateMessageBox(MyMessageBoxStyleEnum.Error, MyMessageBoxButtonsType.OK, messageCaption: MyTexts.Get(MyCommonTexts.MessageBoxCaptionError), messageText: new StringBuilder().AppendFormat(MyTexts.GetString(MyGameService.IsActive ? MyCommonTexts.SteamIsOfflinePleaseRestart : MyCommonTexts.ErrorJoinSessionNoUser), MySession.GameServiceName)));
            }
        }

        private void joinGameScreen_Closed(MyGuiScreenBase source, bool isUnloading)
        {
            if (source.Cancelled)
            {
                //base.State = MyGuiScreenState.OPENING;
                source.Closed -= joinGameScreen_Closed;
            }
        }

        public void Options()
        {
            //bool flag = !MyPlatformGameSettings.LIMITED_MAIN_MENU;
            //MyGuiSandbox.AddScreen(MyGuiSandbox.CreateScreen<MyGuiScreenOptionsSpace>(new object[1] { !flag }));
        }

        public void Character()
        {
            if (MyGameService.IsActive)
            {
                if (MyGameService.Service.GetInstallStatus(out int _))
                {
                    if (MySession.Static == null)
                    {
                        MyGuiScreenLoadInventory inventory = MyGuiSandbox.CreateScreen<MyGuiScreenLoadInventory>(Array.Empty<object>());
                        MyGuiScreenLoading screen = new MyGuiScreenLoading(inventory, null);
                        MyGuiScreenLoadInventory myGuiScreenLoadInventory = inventory;
                        myGuiScreenLoadInventory.OnLoadingAction = (Action)Delegate.Combine(myGuiScreenLoadInventory.OnLoadingAction, (Action)delegate
                        {
                            MySessionLoader.LoadInventoryScene();
                            MySandboxGame.IsUpdateReady = true;
                            inventory.Initialize(inGame: false, null);
                        });
                        MyGuiSandbox.AddScreen(screen);
                    }
                    else
                    {
                        MyGuiSandbox.AddScreen(MyGuiSandbox.CreateScreen<MyGuiScreenLoadInventory>(new object[1] { false }));
                    }
                }
                else
                {
                    MyGuiSandbox.AddScreen(MyGuiSandbox.CreateMessageBox(MyMessageBoxStyleEnum.Error, MyMessageBoxButtonsType.OK, messageCaption: MyTexts.Get(MyCommonTexts.MessageBoxCaptionInfo), messageText: MyTexts.Get(MyCommonTexts.InventoryScreen_InstallInProgress)));
                }
            }
            else
            {
                MyGuiSandbox.AddScreen(MyGuiSandbox.CreateMessageBox(MyMessageBoxStyleEnum.Error, MyMessageBoxButtonsType.OK, messageCaption: MyTexts.Get(MyCommonTexts.MessageBoxCaptionError), messageText: MyTexts.Get(MyCommonTexts.SteamIsOfflinePleaseRestart)));
            }
        }

        public void Update()
        {

        }
    }
}