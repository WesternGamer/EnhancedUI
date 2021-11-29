using CefSharp;
using EnhancedUI.Gui.Browser;
using EnhancedUI.Gui.HtmlGuiControl;
using Sandbox;
using Sandbox.Engine.Networking;
using Sandbox.Game;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Game.ObjectBuilders.Campaign;
using VRage.GameServices;

namespace EnhancedUI.ViewModels.NewGameMenuViewModel
{
    internal class NewGameMenuViewModel : IWebPageViewModel
    {
		private List<MyObjectBuilder_Campaign>? VanillaWorlds;
		private List<MyObjectBuilder_Campaign>? LocalCustomWorlds;
		private List<MyObjectBuilder_Campaign>? CustomWorlds;
		private List<MyObjectBuilder_Campaign>? DebugWorlds;

		// Model is a singleton
		public static NewGameMenuViewModel? Instance;

        public NewGameMenuViewModel()
        {
            if (Instance != null)
            {
                throw new Exception("Only one instance of MainMenuViewModel can be open at a time.");
            }

            Instance = this;
        }

        public void Dispose()
        {
            
        }

        public void Update()
        {
			RefreshCampaignList();
		}

		private void RefreshCampaignList()
		{
			List<MyObjectBuilder_Campaign> campaigns = Enumerable.ToList<MyObjectBuilder_Campaign>(MyCampaignManager.Static.Campaigns);
			VanillaWorlds = new List<MyObjectBuilder_Campaign>();
			LocalCustomWorlds = new List<MyObjectBuilder_Campaign>();
			CustomWorlds = new List<MyObjectBuilder_Campaign>();
			DebugWorlds = new List<MyObjectBuilder_Campaign>();

			foreach (MyObjectBuilder_Campaign campaign in Enumerable.ToList(((IEnumerable<MyObjectBuilder_Campaign>)campaigns).OrderBy((MyObjectBuilder_Campaign x) => x.Order)))
			{
				if (campaign.IsVanilla && !campaign.IsDebug)
				{
					VanillaWorlds.Add(campaign);
				}
				else if (campaign.IsLocalMod)
				{
					LocalCustomWorlds.Add(campaign);
				}
				else if (campaign.IsVanilla && campaign.IsDebug)
				{
					DebugWorlds.Add(campaign);
				}
				else
				{
					CustomWorlds.Add(campaign);
				}
			}
		}

		public List<MyObjectBuilder_Campaign>? GetVanillaWorlds()
        {
			if (VanillaWorlds != null)
            {
				return VanillaWorlds;
			}
			return null;
        }
		public List<MyObjectBuilder_Campaign>? GetLocalCustomWorlds()
		{
			if (LocalCustomWorlds != null)
			{
				return LocalCustomWorlds;
			}
			return null;
		}
		public List<MyObjectBuilder_Campaign>? GetCustomWorlds()
		{
			if (CustomWorlds != null)
			{
				return CustomWorlds;
			}
			return null;
		}
		public List<MyObjectBuilder_Campaign>? GetDebugWorlds()
		{
			if (DebugWorlds != null)
			{
				return DebugWorlds;
			}
			return null;
		}
	}
}
