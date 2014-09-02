/*
 * Switcheroo - The incremental-search task switcher for Windows.
 * http://www.switcheroo.io/
 * Copyright 2009, 2010 James Sulak
 * Copyright 2014 Regin Larsen
 * 
 * Switcheroo is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Switcheroo is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Switcheroo.  If not, see <http://www.gnu.org/licenses/>.
 */

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