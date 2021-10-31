using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EnhancedUI.Utils;
using Sandbox.Game.Entities.Cube;
using VRage.Utils;

namespace EnhancedUI.ViewModel
{
    public class TerminalViewModel : ITerminalViewModel, IDisposable
    {
        // Model is a singleton
        public static TerminalViewModel? Instance;

        // Logical clock, model state version number for browser synchronization
        private long latestVersion;

        // Event triggered on new game state versions
        public delegate void OnGameStateChangedHandler(long version);

        public event OnGameStateChangedHandler? OnGameStateChanged;

        // View model of reachable blocks by ID
        private readonly Dictionary<long, BlockViewModel> blocks = new();

        // Set of IDs of blocks modified by the game
        private readonly Tracker<long> blocksModifiedByGame = new();

        // Set of IDs of blocks modified by the user
        private readonly Tracker<long> blocksModifiedByUser = new();

        // Terminal block the player interacts with
        // Set to null if the player is not connected to any grids
        private MyTerminalBlock? interactedBlock;
        private long? interactedBlockId;

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
            if (interactedBlock == block)
                return;

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
                interactedBlockId = null;
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
                    var blockViewModel = new BlockViewModel(this, block, version);
                    blocks[blockViewModel.Id] = blockViewModel;

                    if (block == interactedBlock)
                        interactedBlockId = blockViewModel.Id;
                }
            }
        }

        private long GetNextVersion()
        {
            return Interlocked.Increment(ref latestVersion);
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

            bool changed;
            lock (blocks)
            {
                var version = GetNextVersion();
                changed = UpdateGameModifiedBlocks(version);
                changed = ApplyUserModifications(version) || changed;
            }

            if (changed)
            {
                MyLog.Default.Debug($"EnhancedUI: OnGameStateChanged({latestVersion})");
                OnGameStateChanged?.Invoke(latestVersion);
            }
        }

        private bool ApplyUserModifications(long version)
        {
            var changed = false;

            using var context = blocksModifiedByUser.Process();
            foreach (var blockId in context.Items)
            {
                if (!blocks.TryGetValue(blockId, out var block))
                    continue;

                changed = block.Apply(version) || changed;
            }

            return changed;
        }

        private bool UpdateGameModifiedBlocks(long version)
        {
            using var context = blocksModifiedByGame.Process();
            var changed = false;
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
            return interactedBlockId;
        }

        public List<long> GetBlockIds()
        {
            lock (blocks)
            {
                return blocks.Values.Select(b => b.Id).ToList();
            }
        }

        public List<long> GetModifiedBlockIds(long sinceVersion)
        {
            lock (blocks)
            {
                var blockIds =blocks.Values.Where(b => b.Version > sinceVersion).Select(b => b.Id).ToList();
                MyLog.Default.Debug($"EnhancedUI: GetModifiedBlockIds({sinceVersion}) => {blockIds.Count} blocks");
                return blockIds;
            }
        }

        public BlockViewModel? GetBlockState(long blockId)
        {
            lock (blocks)
            {
                var blockViewModel = blocks.GetValueOrDefault(blockId);
                MyLog.Default.Debug(
                    blockViewModel == null
                        ? $"EnhancedUI: GetBlockState({blockId}) => NOT FOUND"
                        : $"EnhancedUI: GetBlockState({blockId}) => {blockViewModel}");
                return blockViewModel;
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

        public void SetBlockProperty(long blockId, string propertyId, object? value)
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