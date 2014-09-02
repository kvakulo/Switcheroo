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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Switcheroo
{
    // Create a shortcut file in the current users start up folder
    // Based on this answer on Stackoverflow:
    // http://stackoverflow.com/a/19914018/198065
    public class AutoStart
    {
        public bool IsEnabled
        {
            get { return HasShortcut(); }

            set
            {
                var appLink = GetAppLinkPath();

                if (value)
                {
                    CreateShortcut(appLink);
                }
                else if (IsEnabled)
                {
                    DeleteShortcut(appLink);
                }
            }
        }

        private static bool HasShortcut()
        {
            try
            {
                return File.Exists(GetAppLinkPath());
            }
            catch
            {
                return false;
            }
        }

        private static string GetAppLinkPath()
        {
            var appDataStart =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Microsoft\Windows\Start Menu\Programs\Startup");
            var appLink = Path.Combine(appDataStart, "Switcheroo.lnk");
            return appLink;
        }

        private static void DeleteShortcut(string appLink)
        {
            try
            {
                File.Delete(appLink);
            }
            catch
            {
                throw new AutoStartException("It was not possible to delete the shortcut to Switcheroo in the startup folder");
            }
        }

        private static void CreateShortcut(string appLink)
        {
            try
            {
                var exeLocation = Assembly.GetEntryAssembly().Location;

                //Windows Script Host Shell Object
                var t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
                dynamic shell = Activator.CreateInstance(t);
                try
                {
                    var lnk = shell.CreateShortcut(appLink);
                    try
                    {
                        lnk.TargetPath = exeLocation;
                        lnk.Save();
                    }
                    finally
                    {
                        Marshal.FinalReleaseComObject(lnk);
                    }
                }
                finally
                {
                    Marshal.FinalReleaseComObject(shell);
                }
            }
            catch
            {
                throw new AutoStartException("It was not possible to create a shortcut to Switcheroo in the startup folder");
            }
        }
    }

    public class AutoStartException : Exception
    {
        public AutoStartException(string message)
            : base(message)
        {

        }
    }
}
