using System;
using System.Linq;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Gui;

namespace EnhancedUI.Interop
{
    public class Terminal
    {
        private readonly MyGuiScreenTerminal _screenTerminal;

        public Terminal(MyGuiScreenTerminal screenTerminal)
        {
            _screenTerminal = screenTerminal;
        }
        public Block[] GetBlocks()
        {
            if (MyGuiScreenTerminal.InteractedEntity is MyCubeGrid grid)
                return grid.GetFatBlocks().OfType<MyTerminalBlock>().Select(b => new Block(b)).ToArray();

            return Array.Empty<Block>();
        }
    }
}