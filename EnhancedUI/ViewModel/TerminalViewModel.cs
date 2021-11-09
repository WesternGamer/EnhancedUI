using EnhancedUI.Utils;
using Sandbox.Game.Entities.Cube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VRage.Utils;

namespace EnhancedUI.ViewModel
{
    public class TerminalViewModel : ITerminalViewModel, IDisposable
    {
        /// <summary>
        /// Model is a singleton.
        /// </summary>
        public static TerminalViewModel? Instance;

        /// <summary>
        /// Event triggered on new game state versions.
        /// </summary>
        /// <param name="version"></param>
        public delegate void OnGameStateChangedHandler(long version);

        public event OnGameStateChangedHandler? OnGameStateChanged;

        /// <summary>
        /// View model of reachable blocks by ID.
        /// </summary>
        private readonly Dictionary<long, BlockViewModel> blocks = new();

        /// <summary>
        /// Set of IDs of blocks modified by the game.
        /// </summary>
        private readonly Tracker<long> blocksModifiedByGame = new();

        /// <summary>
        /// Set of IDs of blocks modified by the user.
        /// </summary>
        private readonly Tracker<long> blocksModifiedByUser = new();

        /// <summary>
        /// Terminal block the player interacts with. Set to null if the player is not connected to any grids.
        /// </summary>
        private MyTerminalBlock? interactedBlock;

        /// <summary>
        /// The ID of the block that the player interacts with.
        /// </summary>
        private long? interactedBlockId;

        /// <summary>
        /// Returns true if the player is connected to a terminal system.
        /// </summary>
        private bool IsConnected => interactedBlock?.IsFunctional == true;

        /// <summary>
        /// Logical clock, model state version number for browser synchronization.
        /// </summary>
        private long latestVersion;

        public long GetNextVersion()
        {
            return Interlocked.Increment(ref latestVersion);
        }

        public TerminalViewModel()
        {
            if (Instance != null)
            {
                throw new Exception("This is a singleton");
            }

            Instance = this;
        }

        public void Dispose()
        {
            Clear();
            Instance = null;
        }

        /// <summary>
        /// Called when the user connects to a terminal block
        /// </summary>
        /// <param name="block">The block that the player interacts with.</param>
        public void Connect(MyTerminalBlock block)
        {
            if (interactedBlock == block)
            {
                return;
            }

            Clear();

            interactedBlock = block;

            CreateBlockViewModels();

            // TODO: Listen on grid modifications (add/remove blocks) and splits!
            // TODO: Collect named groups!
        }

        /// <summary>
        /// Called to clear the current view model on closing the terminal.
        /// </summary>
        private void Clear()
        {
            if (interactedBlock == null)
            {
                return;
            }

            lock (blocks)
            {
                foreach (BlockViewModel? block in blocks.Values)
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
                {
                    return;
                }

                long version = GetNextVersion();
                foreach (MyTerminalBlock? block in interactedBlock.CubeGrid.GridSystems.TerminalSystem.Blocks)
                {
                    BlockViewModel? blockViewModel = new BlockViewModel(this, block, version);
                    blocks[blockViewModel.Id] = blockViewModel;

                    if (block == interactedBlock)
                    {
                        interactedBlockId = blockViewModel.Id;
                    }
                }
            }
        }

        /// <summary>
        /// Adds to the list of blocks modified by the game.
        /// </summary>
        /// <param name="blockId">The blockID of the block that is modified by the game.</param>
        internal void NotifyGameModifiedBlock(long blockId)
        {
            if (blocks.ContainsKey(blockId))
            {
                blocksModifiedByGame.Add(blockId);
            }
        }

        /// <summary>
        /// Adds to the list of blocks modified by the player.
        /// </summary>
        /// <param name="blockId">The blockID of the block that is modified by the player.</param>
        internal void NotifyUserModifiedBlock(long blockId)
        {
            if (blocks.ContainsKey(blockId))
            {
                blocksModifiedByUser.Add(blockId);
            }
        }

        /// <summary>
        /// Called on game updates.
        /// </summary>
        internal void Update()
        {
            if (!IsConnected)
            {
                Clear();
                return;
            }

            ApplyUserModifications();

            long versionBefore = latestVersion;

            lock (blocks)
            {
                UpdateGameModifiedBlocks();
            }

            if (latestVersion != versionBefore)
            {
                MyLog.Default.Debug($"EnhancedUI: OnGameStateChanged({latestVersion})");
                OnGameStateChanged?.Invoke(latestVersion);
            }
        }

        /// <summary>
        /// Applies the properties that the player modified.
        /// </summary>
        private void ApplyUserModifications()
        {
            using Tracker<long>.Context? context = blocksModifiedByUser.Process();
            foreach (long blockId in context.Items)
            {
                if (!blocks.TryGetValue(blockId, out BlockViewModel? block))
                {
                    continue;
                }

                block.Apply();
            }
        }

        private void UpdateGameModifiedBlocks()
        {
            using Tracker<long>.Context? context = blocksModifiedByGame.Process();
            foreach (long blockId in context.Items)
            {
                if (!blocks.TryGetValue(blockId, out BlockViewModel? block))
                {
                    continue;
                }

                block.Update();
            }
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
                List<long>? blockIds = blocks.Values.Where(b => b.Version > sinceVersion).Select(b => b.Id).ToList();
                MyLog.Default.Debug($"EnhancedUI: GetModifiedBlockIds({sinceVersion}) => {blockIds.Count} blocks");
                return blockIds;
            }
        }

        public BlockViewModel? GetBlockState(long blockId)
        {
            lock (blocks)
            {
                BlockViewModel? blockViewModel = blocks.GetValueOrDefault(blockId);
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
                if (!blocks.TryGetValue(blockId, out BlockViewModel? block))
                {
                    return;
                }

                block.SetName(name);
            }
        }

        public void SetBlockCustomData(long blockId, string customData)
        {
            lock (blocks)
            {
                if (!blocks.TryGetValue(blockId, out BlockViewModel? block))
                {
                    return;
                }

                block.SetCustomData(customData);
            }
        }

        public void SetBlockProperty(long blockId, string propertyId, object? value)
        {
            lock (blocks)
            {
                if (!blocks.TryGetValue(blockId, out BlockViewModel? block))
                {
                    return;
                }

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