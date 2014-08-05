using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Switcheroo.Core
{
    // Convert a keycode to the relevant display character
    // http://stackoverflow.com/a/375047/198065
    public class KeyboardHelper
    {
        public static string CodeToString(uint virtualKey)
        {
            uint procId;
            var thread = WinApi.GetWindowThreadProcessId(Process.GetCurrentProcess().MainWindowHandle, out procId);
            var hkl = WinApi.GetKeyboardLayout(thread);

            if (hkl == IntPtr.Zero)
            {
                return string.Empty;
            }

            var keyStates = new Keys[256];
            if (!WinApi.GetKeyboardState(keyStates))
            {
                return string.Empty;
            }

            var scanCode = WinApi.MapVirtualKeyEx(virtualKey, WinApi.MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR, hkl);

            var sb = new StringBuilder(10);
            var rc = WinApi.ToUnicodeEx(virtualKey, scanCode, new Keys[0], sb, sb.Capacity, 0, hkl);
            return sb.ToString();
        }
    }
}