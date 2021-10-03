using System.Collections.Generic;
using CefSharp;
using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    public class ControlPanelState : PanelState
    {
        public static ControlPanelState? Instance;

        // Link to SE's data model
        private MyTerminalBlock? interactedBlock;

        // State variables filled from SE's data
        private readonly List<BlockInfo> blocks = new();

        // Getters, because CefSharp relays only method calls
        public List<BlockInfo> GetBlocks() => blocks;

        public ControlPanelState()
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
            interactedBlock = null;

            blocks.Clear();

            if (HasBound())
            {
                Browser.ExecuteScriptAsync("stateUpdated();");
            }
        }

        public void Update(MyTerminalBlock block)
        {
            if (!HasBound())
            {
                return;
            }

            if (interactedBlock?.EntityId == block.EntityId)
            {
                return;
            }

            interactedBlock = block;

            UpdateBlocks();

            Browser.ExecuteScriptAsync("stateUpdated();");
        }

        // ReSharper disable once UnusedMember.Global
        private void UpdateBlocks()
        {
            blocks.Clear();
            if (interactedBlock == null)
            {
                return;
            }

            foreach (var functionalBlock in interactedBlock.CubeGrid.GetFatBlocks<MyFunctionalBlock>())
            {
                blocks.Add(new BlockInfo(functionalBlock));
            }
        }
    }
}