using System.Collections.Generic;

namespace EnhancedUI.ViewModel
{
    // JavaScript API of the ViewModel
    public interface ITerminalViewModel
    {
        // TODO: Grids

        // Returns list of IDs of blocks the player have access to via the interacted block.
        // Returns empty list if the player is not connected to a terminal port.
        List<long> GetBlockIds();

        // Returns list of IDs of blocks have got any modifications since the given version
        List<long> GetModifiedBlockIds(long sinceVersion);

        // Retrieves the view model of the specific block.
        // Returns null if the player is not connected to a terminal port.
        BlockViewModel? GetBlockState(long blockId);

        // Modifies a block's Name, actual modification will happen on the next game update
        void SetBlockName(long blockId, string name);

        // Modifies a block's CustomData, actual modification will happen on the next game update
        void SetBlockCustomData(long blockId, string customData);

        // Modified a property value, actual modification will happen on the next game update
        void SetProperty(long blockId, string propertyId, object? value);

        // Returns the named groups of blocks the player have access to via the interacted block.
        // Returns an empty dictionary if the player is not connected to a terminal port.
        Dictionary<string, List<long>> GetGroups();

        // Returns the list of group names a specific block is member of
        List<string> GetBlockGroups();

        // Enqueues adding a block to a named group
        void AddBlockToGroup(long blockId, string groupName);

        // Enqueues removing a block from a named group
        void RemoveBlockFromGroup(long blockId, string groupName);
    }
}