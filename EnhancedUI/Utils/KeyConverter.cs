using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace EnhancedUI.Utils
{
    public static class KeyConverter
    {
        private static readonly StringBuilder _result = new ();

        public static unsafe string KeyCodeToUnicode(uint virtualKeyCode)
        {
            var keyboardState = stackalloc byte[255];

            var keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return string.Empty;
            }

            var scanCode = MapVirtualKey(virtualKeyCode, 0);
            var inputLocaleIdentifier = GetKeyboardLayout(0);

            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, _result, 5, 0, inputLocaleIdentifier);

            try
            {
                return _result.ToString();
            }
            finally
            {
                _result.Clear();
            }
        }

#pragma warning disable SA1305 // Due to native parameters names
        [DllImport("user32.dll")]
        private static extern unsafe bool GetKeyboardState(byte* lpKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        private static extern unsafe int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte* lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
#pragma warning restore SA1305
    }
}
