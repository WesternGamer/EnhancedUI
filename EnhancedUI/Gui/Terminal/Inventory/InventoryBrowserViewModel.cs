namespace EnhancedUI.Gui.Terminal.Inventory
{
    public class InventoryBrowserViewModel: BrowserViewModel
    {
        public static InventoryBrowserViewModel? Instance;

        public InventoryBrowserViewModel()
        {
            Instance = this;
        }

        public int TestAdd(int a, int b)
        {
            return a + b;
        }
    }
}