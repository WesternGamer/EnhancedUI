using System;
using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    public class BlockState
    {
        public readonly string Name;

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public BlockState(MyTerminalBlock block)
        {
            Name = block.CustomName.ToString();
            if (Name == "")
            {
                Name = block.DisplayNameText ?? block.DisplayName;
            }
        }
    }
}