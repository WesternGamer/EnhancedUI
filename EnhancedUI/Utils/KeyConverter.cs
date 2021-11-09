using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EnhancedUI.Utils
{
    /// <summary>
    /// Class to convert key codes to unicode.
    /// </summary>
    public static class KeyConverter
    {
        private static readonly StringBuilder Result = new();

        /// <summary>
        /// Converts the native Windows key codes to unicode.
        /// </summary>
        /// <param name="virtualKeyCode"></param>
        /// <returns> Unicode of key.</returns>
        public static unsafe string KeyCodeToUnicode(uint virtualKeyCode)
        {
            byte* keyboardState = stackalloc byte[255];

            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return string.Empty;
            }

            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            uint windowThreadProcessId = GetWindowThreadProcessId(ProcessInfo.MainWindowHandle, IntPtr.Zero);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(windowThreadProcessId);

            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, Result, 5, 0, inputLocaleIdentifier);

            try
            {
                return Result.ToString();
            }
            finally
            {
                Result.Clear();
            }
        }

#pragma warning disable SA1305 
        // Due to native parameters names
        [DllImport("user32.dll")]
        private static extern unsafe bool GetKeyboardState(byte* lpKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        private static extern unsafe int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte* lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);
#pragma warning restore SA1305
    }
}