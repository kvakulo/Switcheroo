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
using System.ComponentModel;
using System.Drawing;

namespace Switcheroo.Core
{
    public enum WindowIconSize
    {
        Small,
        Large
    }

    public class WindowIconFinder
    {
        public Icon Find(AppWindow window, WindowIconSize size)
        {
            Icon icon = null;
            try
            {
                // http://msdn.microsoft.com/en-us/library/windows/desktop/ms632625(v=vs.85).aspx
                IntPtr response;
                var outvalue = WinApi.SendMessageTimeout(window.HWnd, 0x007F, size == WindowIconSize.Small ? new IntPtr(2) : new IntPtr(1),
                    IntPtr.Zero, WinApi.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 100, out response);

                if (outvalue == IntPtr.Zero || response == IntPtr.Zero)
                {
                    response = WinApi.GetClassLongPtr(window.HWnd,
                        size == WindowIconSize.Small ? WinApi.ClassLongFlags.GCLP_HICONSM : WinApi.ClassLongFlags.GCLP_HICON);
                }

                if (response != IntPtr.Zero)
                {
                    icon = Icon.FromHandle(response);
                }
                else
                {
                    var executablePath = window.ExecutablePath;
                    icon = Icon.ExtractAssociatedIcon(executablePath);
                }
            }
            catch (Win32Exception)
            {
                // Could not extract icon
            }
            return icon;
        }
    }
}