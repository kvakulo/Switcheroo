/*
 * Switcheroo - The incremental-search task switcher for Windows.
 * http://bitbucket.org/jasulak/switcheroo/
 * Copyright 2009, 2010 James Sulak
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
using System.Drawing;
using System.Runtime.InteropServices;

namespace Switcheroo
{

    /// <summary>
    /// This class is a wrapper around the Win32 api window handles
    /// </summary>
    public class AppWindow : ManagedWinapi.Windows.SystemWindow
    {
        private const int MAX_TITLE_LENGTH = 100;
        private const UInt32 WM_CLOSE = 0x0010;
        
        // Returns a short version of the title
        public string TruncatedTitle 
        { 
            get 
            {
                if (Title.Length > MAX_TITLE_LENGTH) {
                    return Title.Substring(0, MAX_TITLE_LENGTH) + "...";
                }
                else {
                    return Title;
                }
            } 
        }

        public string ProcessTitle
        {
            get { return Process.ProcessName; }
        }

        public AppWindow(IntPtr HWnd) : base(HWnd) { }

        /// <summary>
        /// Sets the focus to this window and brings it to the foreground.
        /// </summary>
        public void SwitchTo()
        {
            // This function is deprecated, so should probably be replaced.
            SwitchToThisWindow(this.HWnd, true);                                    
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);          
    }
}
