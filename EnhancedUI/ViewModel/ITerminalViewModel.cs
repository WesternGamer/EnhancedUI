using System.Collections.Generic;

namespace EnhancedUI.ViewModel
{
    /// <summary>
    /// JavaScript API of the ViewModel.
    /// </summary>
    public interface ITerminalViewModel
    {
        // TODO: Grid API

        /// <summary>
        /// Returns the ID of the interacted block the player is directly connected to. Returns null if the player is not connected to a terminal port.
        /// </summary>
        long? GetInteractedBlockId();

        /// <summary>
        /// Returns list of IDs of blocks the player have access to via the interacted block. Returns empty list if the player is not connected to a terminal port.
        /// </summary>
        List<long> GetBlockIds();

        /// <summary>
        /// Returns list of IDs of blocks have got any modifications since the given version.
        /// </summary>
        /// <param name="sinceVersion"></param>
        List<long> GetModifiedBlockIds(long sinceVersion);

        /// <summary>
        /// Retrieves the view model of the specific block. Returns null if the player is not connected to a terminal port.
        /// </summary>
        /// <param name="blockId">Block to retrieve the view model from.</param>
        BlockViewModel? GetBlockState(long blockId);

        /// <summary>
        /// Modifies a block's Name, actual modification will happen on the next game update.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <param name="name">The name that will be set.</param>
        void SetBlockName(long blockId, string name);

        /// <summary>
        /// Modifies a block's CustomData, actual modification will happen on the next game update.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <param name="customData">The CustomData that will be set.</param>
        void SetBlockCustomData(long blockId, string customData);

        /// <summary>
        /// Modifies a block property's value, actual modification will happen on the next game update.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <param name="propertyId">The ID of the property.</param>
        /// <param name="value">The data to set the property to.</param>
        void SetBlockProperty(long blockId, string propertyId, object? value);

        /// <summary>
        /// Returns the named groups of blocks the player have access to via the interacted block. Returns an empty dictionary if the player is not connected to a terminal port.
        /// </summary>
        Dictionary<string, List<long>> GetGroups();

        /// <summary>
        /// Returns the list of group names a specific block is member of.
        /// </summary>
        List<string> GetBlockGroups();

        /// <summary>
        /// Enqueues adding a block to a named group.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <param name="groupName">The name of the block group.</param>
        void AddBlockToGroup(long blockId, string groupName);

        /// <summary>
        /// Enqueues removing a block from a named group.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <param name="groupName">The name of the block group.</param>
        void RemoveBlockFromGroup(long blockId, string groupName);
    }
}