using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.Gui.Terminal.Production
{
    public class ProductionState : PanelState
    {
        public static ProductionState? Instance;

        public ProductionState()
        {
            Instance = this;
        }

        public void Init(MyTerminalBlock block)
        {
            // TODO
        }
    }
}
