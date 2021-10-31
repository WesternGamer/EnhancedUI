using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.Utils;
using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.ViewModel
{
    public class TerminalViewModel : ITerminalViewModel, IDisposable
    {
        // Model is a singleton
        public static TerminalViewModel? Instance;

        // Logical clock, model state versions for browser synchronization
        private readonly object latestVersionLock = new();
        private long latestVersion;

        // Event triggered on new game state versions
        public delegate void OnNewGameStateVersionHandler(long version);

        public event OnNewGameStateVersionHandler? OnNewGameStateVersion;

        // View model of reachable blocks by EntityId
        private readonly Dictionary<long, BlockViewModel> blocks = new();

        // Set of EntityIds of blocks modified by the game
        private readonly Tracker<long> blocksModifiedByGame = new();

        // Set of EntityIds of blocks modified by the user
        private readonly Tracker<long> blocksModifiedByUser = new();

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

            // TODO: Listen on grid modifications (add/remove blocks) and splits!
            // TODO: Collect named groups!
        }

        // Called to clear the current view model on closing the terminal
        private void Clear()
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

                var version = GetNextVersion();
                foreach (var block in interactedBlock.CubeGrid.GridSystems.TerminalSystem.Blocks)
                {
                    blocks[block.EntityId] = new BlockViewModel(this, block, version);
                }
            }
        }

        private long GetNextVersion()
        {
            lock (latestVersionLock)
            {
                return ++latestVersion;
            }
        }

        internal void NotifyGameModifiedBlock(long blockId)
        {
            if (blocks.ContainsKey(blockId))
                blocksModifiedByGame.Add(blockId);
        }

        internal void NotifyUserModifiedBlock(long blockId)
        {
            if (blocks.ContainsKey(blockId))
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

            var version = GetNextVersion();

            bool changed;
            lock (blocks)
            {
                changed = ApplyUserModifications();
                changed = UpdateGameModifiedBlocks(version) || changed;
            }

            if (changed)
                OnNewGameStateVersion?.Invoke(version);
        }

        private bool ApplyUserModifications()
        {
            var changed = false;

            using var context = blocksModifiedByUser.Process();
            foreach (var blockId in context.Items)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    continue;

                changed = block.Apply() || changed;
            }

            return changed;
        }

        private bool UpdateGameModifiedBlocks(long version)
        {
            var changed = false;

            using var context = blocksModifiedByGame.Process();
            foreach (var blockId in context.Items)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    continue;

                changed = block.Update(version) || changed;
            }

            return changed;
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

        public List<long> GetModifiedBlockIds(long sinceVersion)
        {
            lock (blocks)
            {
                return blocks.Values.Where(b => b.Version >= sinceVersion).Select(b => b.EntityId).ToList();
            }
        }

        public BlockViewModel? GetBlockState(long blockId)
        {
            lock (blocks)
            {
                return blocks.GetValueOrDefault(blockId);
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