using System;

namespace EnhancedUI.ViewModels
{
    internal interface IWebPageViewModel : IDisposable
    {
        // TODO: Any way to call Update() in all classes that implements this interface?
        void Update();
    }
}
