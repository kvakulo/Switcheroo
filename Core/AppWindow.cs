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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;
using ManagedWinapi.Windows;

namespace Switcheroo.Core
{

    /// <summary>
    /// This class is a wrapper around the Win32 api window handles
    /// </summary>
    public class AppWindow : ManagedWinapi.Windows.SystemWindow
    {
        public string FormattedTitle { get; set; }

        public string ProcessTitle
        {
            get
            {
                var key = "ProcessTitle-" + HWnd;
                var processTitle = MemoryCache.Default.Get(key) as string;
                if (processTitle == null)
                {
                    processTitle = Process.ProcessName;
                    MemoryCache.Default.Add(key, processTitle, DateTimeOffset.Now.AddHours(1));
                }
                return processTitle;
            }
        }

        public string FormattedProcessTitle { get; set; }

        public BitmapImage IconImage
        {
            get
            {
                var key = "IconImage-" + HWnd;
                var iconImage = MemoryCache.Default.Get(key) as BitmapImage;
                if (iconImage == null)
                {
                    iconImage = ExtractIcon() ?? new BitmapImage();
                    MemoryCache.Default.Add(key, iconImage, DateTimeOffset.Now.AddHours(1));
                }
                return iconImage;
            }
        }

        private BitmapImage ExtractIcon()
        {
            Icon extractAssociatedIcon = null;
            try
            {
                extractAssociatedIcon = Icon.ExtractAssociatedIcon(GetExecutablePath(Process));
            }
            catch (Win32Exception)
            {
                // Could not extract icon
            }
            if (extractAssociatedIcon == null)
            {
                return null;
            }

            using (var memory = new MemoryStream())
            {
                var bitmap = extractAssociatedIcon.ToBitmap();
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private static string GetExecutablePath(Process process)
        {
            // If Vista or later
            if (Environment.OSVersion.Version.Major >= 6)
            {
                return GetExecutablePathAboveVista(process.Id);
            }

            return process.MainModule.FileName;
        }

        private static string GetExecutablePathAboveVista(int processId)
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

        public AppWindow(IntPtr HWnd) : base(HWnd) { }

        /// <summary>
        /// Sets the focus to this window and brings it to the foreground.
        /// </summary>
        public void SwitchTo()
        {
            // This function is deprecated, so should probably be replaced.
            WinApi.SwitchToThisWindow(HWnd, true);                                    
        }

        public bool IsAltTabWindow()
        {
            if (IsAppWindow()) return true;
            if (IsToolWindow() || IsControlParent()) return false;

            // Start at the root owner
            var handleWalk = WinApi.GetAncestor(HWnd, WinApi.GetAncestorFlags.GetRootOwner);

            // See if we are the last active visible popup
            IntPtr handleTry;
            while ((handleTry = WinApi.GetLastActivePopup(handleWalk)) != handleTry)
            {
                if (WinApi.IsWindowVisible(handleTry)) break;
                handleWalk = handleTry;
            }
            return handleWalk == HWnd;
        }

        private bool IsToolWindow()
        {
            return (ExtendedStyle & WindowExStyleFlags.TOOLWINDOW) == WindowExStyleFlags.TOOLWINDOW;
        }

        private bool IsAppWindow()
        {
            return (ExtendedStyle & WindowExStyleFlags.APPWINDOW) == WindowExStyleFlags.APPWINDOW;
        }

        private bool IsControlParent()
        {
            return (ExtendedStyle & WindowExStyleFlags.CONTROLPARENT) == WindowExStyleFlags.CONTROLPARENT;
        }
    }
}
