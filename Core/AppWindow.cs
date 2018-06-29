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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ManagedWinapi.Windows;

namespace Switcheroo.Core
{
    /// <summary>
    /// This class is a wrapper around the Win32 api window handles
    /// </summary>
    public class AppWindow : SystemWindow
    {
        public string ProcessTitle
        {
            get
            {
                var key = "ProcessTitle-" + HWnd;
                var processTitle = MemoryCache.Default.Get(key) as string;
                if (processTitle == null)
                {
                    if (IsApplicationFrameWindow())
                    {
                        processTitle = "UWP";

                        var underlyingProcess = AllChildWindows.Where(w => w.Process.Id != Process.Id)
                            .Select(w => w.Process)
                            .FirstOrDefault();

                        if (underlyingProcess != null && underlyingProcess.ProcessName != "")
                        {
                            processTitle = underlyingProcess.ProcessName;
                        }
                    }
                    else
                    {
                        processTitle = getWindowProcessName(HWnd);

                    }
                    MemoryCache.Default.Add(key, processTitle, DateTimeOffset.Now.AddHours(1));
                }
                return processTitle;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        private enum ProcessRights
        {
            PROCESS_TERMINATE = 0x1,
            PROCESS_CREATE_THREAD = 0x2,
            PROCESS_SET_SESSIONID = 0x4,
            PROCESS_VM_OPERATION = 0x8,
            PROCESS_VM_READ = 0x10,
            PROCESS_VM_WRITE = 0x20,
            PROCESS_DUP_HANDLE = 0x40,
            PROCESS_CREATE_PROCESS = 0x80,
            PROCESS_SET_QUOTA = 0x100,
            PROCESS_SET_INFORMATION = 0x200,
            PROCESS_QUERY_INFORMATION = 0x400,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(ProcessRights dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, out int lpdwSize);

        private static Regex processNameRegex = new Regex("([^\\\\]+?)(?=(\\.exe)?$)");

        private static string getWindowProcessName(IntPtr hwnd)
        {
            int pid;
            GetWindowThreadProcessId(hwnd, out pid);
            IntPtr handle = OpenProcess(ProcessRights.PROCESS_QUERY_INFORMATION | ProcessRights.PROCESS_VM_READ, false, pid);
            int exeBufferSize = 512;
            StringBuilder exeBuffer = new StringBuilder(exeBufferSize);
            QueryFullProcessImageName(handle, 0, exeBuffer, out exeBufferSize);
            return processNameRegex.Match(exeBuffer.ToString()).Value;
        }

        public Icon LargeWindowIcon
        {
            get { return new WindowIconFinder().Find(this, WindowIconSize.Large); }
        }

        public Icon SmallWindowIcon
        {
            get { return new WindowIconFinder().Find(this, WindowIconSize.Small); }
        }

        public string ExecutablePath
        {
            get { return GetExecutablePath(Process.Id); }
        }

        public AppWindow(IntPtr HWnd) : base(HWnd)
        {
        }

        /// <summary>
        /// Sets the focus to this window and brings it to the foreground.
        /// </summary>
        public void SwitchTo()
        {
            // This function is deprecated, so should probably be replaced.
            WinApi.SwitchToThisWindow(HWnd, true);
        }

        public void SwitchToLastVisibleActivePopup()
        {
            var lastActiveVisiblePopup = GetLastActiveVisiblePopup();
            WinApi.SwitchToThisWindow(lastActiveVisiblePopup, true);
        }

        public AppWindow Owner
        {
            get
            {
                var ownerHandle = WinApi.GetWindow(HWnd, WinApi.GetWindowCmd.GW_OWNER);
                if (ownerHandle == IntPtr.Zero) return null;
                return new AppWindow(ownerHandle);
            }
        }

        public new static IEnumerable<AppWindow> AllToplevelWindows
        {
            get
            {
                return SystemWindow.AllToplevelWindows
                    .Select(w => new AppWindow(w.HWnd));
            }
        }

        public bool IsAltTabWindow()
        {
            if (!Visible) return false;
            if (!HasWindowTitle()) return false;
            if (IsAppWindow()) return true;
            if (IsToolWindow()) return false;
            if (IsNoActivate()) return false;
            if (!IsOwnerOrOwnerNotVisible()) return false;
            if (HasITaskListDeletedProperty()) return false;
            if (IsCoreWindow()) return false;
            if (IsApplicationFrameWindow() && !HasAppropriateApplicationViewCloakType()) return false;

            return true;
        }

        private bool HasWindowTitle()
        {
            return !string.IsNullOrEmpty(Title);
        }

        private bool IsToolWindow()
        {
            return (ExtendedStyle & WindowExStyleFlags.TOOLWINDOW) == WindowExStyleFlags.TOOLWINDOW
                    || (Style & WindowStyleFlags.TOOLWINDOW) == WindowStyleFlags.TOOLWINDOW;
        }

        private bool IsAppWindow()
        {
            return (ExtendedStyle & WindowExStyleFlags.APPWINDOW) == WindowExStyleFlags.APPWINDOW;
        }

        private bool IsNoActivate()
        {
            return (ExtendedStyle & WindowExStyleFlags.NOACTIVATE) == WindowExStyleFlags.NOACTIVATE;
        }

        private IntPtr GetLastActiveVisiblePopup()
        {
            // Which windows appear in the Alt+Tab list? -Raymond Chen
            // http://blogs.msdn.com/b/oldnewthing/archive/2007/10/08/5351207.aspx

            // Start at the root owner
            var hwndWalk = WinApi.GetAncestor(HWnd, WinApi.GetAncestorFlags.GetRootOwner);

            // See if we are the last active visible popup
            var hwndTry = IntPtr.Zero;
            while (hwndWalk != hwndTry)
            {
                hwndTry = hwndWalk;
                hwndWalk = WinApi.GetLastActivePopup(hwndTry);
                if (WinApi.IsWindowVisible(hwndWalk))
                {
                    return hwndWalk;
                }
            }
            return hwndWalk;
        }

        private bool IsOwnerOrOwnerNotVisible()
        {
            return Owner == null || !Owner.Visible;
        }

        private bool HasITaskListDeletedProperty()
        {
            return WinApi.GetProp(HWnd, "ITaskList_Deleted") != IntPtr.Zero;
        }

        private bool IsCoreWindow()
        {
            // Avoids double entries for Windows Store Apps on Windows 10
            return ClassName == "Windows.UI.Core.CoreWindow";
        }

        private bool IsApplicationFrameWindow()
        {
            // Is a UWP application
            return ClassName == "ApplicationFrameWindow";
        }

        private bool HasAppropriateApplicationViewCloakType()
        {
            // The ApplicationFrameWindows that host Windows Store Apps like to
            // hang around in Windows 10 even after the underlying program has been
            // closed. A way to figure out if the ApplicationFrameWindow is
            // currently hosting an application is to check if it has a property called
            // "ApplicationViewCloakType", and that the value != 1.
            //
            // I've stumbled upon these values of "ApplicationViewCloakType":
            //    0 = Program is running on current virtual desktop
            //    1 = Program is not running
            //    2 = Program is running on a different virtual desktop

            var hasAppropriateApplicationViewCloakType = false;
            WinApi.EnumPropsEx(HWnd, (hwnd, lpszString, data, dwData) =>  
            {
                var propName = Marshal.PtrToStringAnsi(lpszString);
                if (propName == "ApplicationViewCloakType")
                {
                    hasAppropriateApplicationViewCloakType = data != 1;
                    return 0;
                }
                return 1;
            }, IntPtr.Zero);

            return hasAppropriateApplicationViewCloakType;
        }

        // This method only works on Windows >= Windows Vista
        private static string GetExecutablePath(int processId)
        {
            var buffer = new StringBuilder(1024);
            var hprocess = WinApi.OpenProcess(WinApi.ProcessAccess.QueryLimitedInformation, false, processId);
            if (hprocess == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                // ReSharper disable once RedundantAssignment
                var size = buffer.Capacity;
                if (WinApi.QueryFullProcessImageName(hprocess, 0, buffer, out size))
                {
                    return buffer.ToString();
                }
            }
            finally
            {
                WinApi.CloseHandle(hprocess);
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}