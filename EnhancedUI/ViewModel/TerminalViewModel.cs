using System;
using System.Collections.Generic;
using System.Linq;
using CefSharp;
using CefSharp.OffScreen;
using EnhancedUI.Utils;
using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.ViewModel
{
    public class TerminalViewModel : ITerminalViewModel, IDisposable
    {
        // Model is a singleton
        public static TerminalViewModel? Instance;

        // View model of reachable blocks by EntityId
        private readonly Dictionary<long, BlockViewModel> blocks = new();

        // Set of EntityIds of blocks modified by the game
        private readonly Tracker<long> blocksModifiedByGame = new();

        // Set of EntityIds of blocks modified by the user
        private readonly Tracker<long> blocksModifiedByUser = new();

        // Set of EntityIds of blocks need to be refreshed in the browser
        private readonly Tracker<long> blocksToRefresh = new();

        // Terminal block the player interacts with
        // Set to null if the player is not connected to any grids
        private MyTerminalBlock? interactedBlock;

        // True if the player is connected to a terminal system
        private bool IsConnected => interactedBlock?.IsFunctional == true;

        public TerminalViewModel()
        {
            if (Instance != null)
                throw new Exception("This is a singleton");

            Instance = this;
        }

        public void Dispose()
        {
            Clear();
            Instance = null;
        }

        // Called when the user connects to a terminal block
        public void Connect(MyTerminalBlock block)
        {
            if (interactedBlock?.EntityId == block.EntityId)
            {
                return;
            }

            Clear();

            interactedBlock = block;

            CreateBlockViewModels();

            // TODO: Listen on grid modification (add/remove blocks) and split events!
            // TODO: Collect named groups!
        }

        // Called to clear the current view model on closing the terminal
        public void Clear()
        {
            if (interactedBlock == null)
                return;

            lock (blocks)
            {
                foreach (var block in blocks.Values)
                {
                    block.Dispose();
                }

                blocks.Clear();

                interactedBlock = null;
            }
        }

        private void CreateBlockViewModels()
        {
            lock (blocks)
            {
                blocks.Clear();

                if (interactedBlock == null || !IsConnected)
                    return;

                foreach (var block in interactedBlock.CubeGrid.GridSystems.TerminalSystem.Blocks)
                {
                    blocks[block.EntityId] = new BlockViewModel(this, block);
                }
            }
        }

        internal void NotifyGameModifiedBlock(long blockId)
        {
            blocksModifiedByGame.Add(blockId);
        }

        internal void NotifyUserModifiedBlock(long blockId)
        {
            blocksModifiedByUser.Add(blockId);
        }

        // Called on game updates
        internal void Update()
        {
            if (!IsConnected)
            {
                Clear();
                return;
            }

            lock (blocks)
            {
                UpdateGameModifiedBlocks();
                ApplyUserModifications();
            }
        }

        internal void NotifyBrowser(ChromiumWebBrowser browser)
        {
            if (blocksToRefresh.Count > 0)
            {
                browser.ExecuteScriptAsync("NotifyBrowserBlocksToRefresh()");
            }
        }

        private void UpdateGameModifiedBlocks()
        {
            using var context = blocksModifiedByGame.Process();
            foreach (var blockId in context.Items)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    continue;

                if (block.Update())
                    blocksToRefresh.Add(blockId);
            }
        }

        private void ApplyUserModifications()
        {
            using var context = blocksModifiedByUser.Process();
            foreach (var blockId in context.Items)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    continue;

                if (block.Apply())
                    blocksToRefresh.Add(blockId);
            }
        }

        #region JavaScript API

        public long? GetInteractedBlockId()
        {
            lock (blocks)
            {
                return interactedBlock?.EntityId;
            }
        }

        public List<long> GetBlockIds()
        {
            lock (blocks)
            {
                return blocks.Values.Select(b => b.EntityId).ToList();
            }
        }

        public BlockViewModel? GetBlock(long blockId)
        {
            lock (blocks)
            {
                return blocks.GetValueOrDefault(blockId);
            }
        }

        public List<long> GetDirtyBlockIds()
        {
            lock (blocks)
            {
                using var context = blocksToRefresh.Process();
                return context.Items.ToList();
            }
        }

        public void SetBlockName(long blockId, string name)
        {
            lock (blocks)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    return;

                block.SetName(name);
            }
        }

        public void SetBlockCustomData(long blockId, string customData)
        {
            lock (blocks)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    return;

                block.SetCustomData(customData);
            }
        }

        public void SetProperty(long blockId, string propertyId, object? value)
        {
            lock (blocks)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    return;

                block.SetProperty(propertyId, value);
            }
        }

        public Dictionary<string, List<long>> GetGroups()
        {
            throw new NotImplementedException();
        }

        public List<string> GetBlockGroups()
        {
            throw new NotImplementedException();
        }

        public void AddBlockToGroup(long blockId, string groupName)
        {
            throw new NotImplementedException();
        }

        public void RemoveBlockFromGroup(long blockId, string groupName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}