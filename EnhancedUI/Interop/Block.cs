using System.Threading.Tasks;
using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.Interop
{
    public class Block
    {
        private readonly MyTerminalBlock _terminalBlock;

        public Block(MyTerminalBlock terminalBlock)
        {
            _terminalBlock = terminalBlock;
        }

        public string GetDisplayName()
        {
            return _terminalBlock.DisplayName;
        }

        public void SetDisplayName(string value)
        {
            _terminalBlock.SetCustomName(value);
        }
    }
}
