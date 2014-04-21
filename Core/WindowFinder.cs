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
using System.Collections.Generic;
using System.Text;

namespace Switcheroo.Core
{
    public class WindowFinder
    {
        private readonly List<AppWindow> _windowList = new List<AppWindow>();

        public List<AppWindow> GetWindows()
        {
            _windowList.Clear();
            WinApi.EnumWindowsProc callback =  EnumWindows;
            WinApi.EnumWindows(callback, 0);
            return _windowList;
        }

        private bool EnumWindows(IntPtr hWnd, int lParam)
        {
            if (!WinApi.IsWindowVisible(hWnd))
            {
                return true;
            }

            if (hWnd == WinApi.Statusbar)
            {
                return true;
            }

            var title = new StringBuilder(256);
            WinApi.GetWindowText(hWnd, title, 256);

            if (string.IsNullOrEmpty(title.ToString())) {
                return true;
            }

            var appWindow = new AppWindow(hWnd);
            if (appWindow.IsAltTabWindow())
            {
                _windowList.Add(appWindow);
            }

            return true;
        }
    }
}
