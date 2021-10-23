using System;
using static System.Diagnostics.Process;

namespace EnhancedUI.Utils
{
    public static class ProcessInfo
    {
        public static int Id = GetCurrentProcess().Id;
        public static IntPtr MainWindowHandle => GetCurrentProcess().MainWindowHandle;
    }
}