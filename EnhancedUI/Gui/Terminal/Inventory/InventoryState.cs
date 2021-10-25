using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.Gui.Terminal.Inventory
{
    public class InventoryState: PanelState
    {
        public static InventoryState? Instance;

        public InventoryState()
        {
            Instance = this;
        }

        public void Init(MyTerminalBlock block)
        {
            // TODO
        }
    }
}