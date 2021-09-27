namespace EnhancedUI.Utils
{
    /// <summary>
    /// Windows Message Enums
    /// Gratiosly based on http://www.pinvoke.net/default.aspx/Enums/WindowsMessages.html
    /// </summary>
    public enum Wm : uint
    {
        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a non-system key is pressed. A non-system
        /// key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        Keydown = 0x0100,

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a non-system key is released. A non-system
        /// key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the
        /// keyboard focus.
        /// </summary>
        Keyup = 0x0101,

        /// <summary>
        /// The WM_CHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the
        /// TranslateMessage function. The WM_CHAR message contains the character code of the key that was pressed.
        /// </summary>
        Char = 0x0102,

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user presses the F10 key (which
        /// activates the menu bar) or holds down the ALT key and then presses another key. It also occurs when no window
        /// currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window. The window
        /// that receives the message can distinguish between these two contexts by checking the context code in the lParam
        /// parameter.
        /// </summary>
        SysKeydown = 0x0104,

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed
        /// while the ALT key was held down. It also occurs when no window currently has the keyboard focus; in this case, the
        /// WM_SYSKEYUP message is sent to the active window. The window that receives the message can distinguish between these
        /// two contexts by checking the context code in the lParam parameter.
        /// </summary>
        SysKeyup = 0x0105,

        /// <summary>
        /// The WM_SYSCHAR message is posted to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by
        /// the TranslateMessage function. It specifies the character code of a system character key — that is, a character key
        /// that is pressed while the ALT key is down.
        /// </summary>
        SysChar = 0x0106,

        /// <summary>
        /// Sent to an application when the IME gets a character of the conversion result. A window receives this message through
        /// its WindowProc function.
        /// </summary>
        ImeChar = 0x0286,

        /// <summary>
        /// Sent immediately before the IME generates the composition string as a result of a keystroke. A window receives this
        /// message through its WindowProc function.
        /// </summary>
        ImeStartComposition = 0x10D,

        /// <summary>
        /// Sent to an application when the IME ends composition. A window receives this message through its WindowProc function.
        /// </summary>
        ImeEndComposition = 0x10E,

        /// <summary>
        /// Sent to an application when the IME changes composition status as a result of a keystroke. A window receives this
        /// message through its WindowProc function.
        /// </summary>
        ImeComposition = 0x10F,

        /// <summary>
        /// Sent to an application when a window is activated. A window receives this message through its WindowProc function.
        /// </summary>
        ImeSetContext = 0x281,
    }
}