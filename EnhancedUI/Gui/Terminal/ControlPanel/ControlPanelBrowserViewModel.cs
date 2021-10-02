using System.Collections.Generic;
using CefSharp;
using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    public class ControlPanelBrowserViewModel: BrowserViewModel
    {
        public static ControlPanelBrowserViewModel? Instance;
        public MyTerminalBlock? InteractedBlock;

        public ControlPanelBrowserViewModel()
        {
            Instance = this;
        }

        public override void Reload()
        {
            base.Reload();
            Clear();
        }

        public void Clear()
        {
            if (HasLoaded())
            {
                Browser.ExecuteScriptAsync("clearContent();");
            }

            InteractedBlock = null;
        }

        public void Load(MyTerminalBlock block)
        {
            if (!HasLoaded())
            {
                InteractedBlock = null;
                return;
            }

            if (InteractedBlock?.EntityId == block.EntityId)
                return;

            Browser.ExecuteScriptAsync("updateContent();");

            InteractedBlock = block;
        }

        // ReSharper disable once UnusedMember.Global
        public List<BlockInfo> GetBlocks()
        {
            var blockList = new List<BlockInfo>();
            if (InteractedBlock == null)
            {
                return blockList;
            }

            foreach (var functionalBlock in InteractedBlock.CubeGrid.GetFatBlocks<MyFunctionalBlock>())
            {
                blockList.Add(new BlockInfo(functionalBlock));
            }

            return blockList;
        }
    }
}