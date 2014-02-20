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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Switcheroo.Core.Matchers;

namespace Switcheroo
{
    /// <summary>
    /// This class contains the main logic for the program.
    /// </summary>
    public static class CoreStuff
    {
        public static HotKey HotKey = new HotKey();
        public static List<AppWindow> WindowList = new List<AppWindow>();
        public static List<string> ExceptionList;

        public static void Initialize() 
        {       
            HotKey.LoadSettings();
            LoadSettings();
        }
       
        public static void GetWindows()
        {
            WinApi.EnumWindowsProc callback = EnumWindows;
            WinApi.EnumWindows(callback, 0);
        }

        public static IEnumerable<FilterResult> FilterList(string filterText)
        {
            return WindowList
                .Select(w => new { Window = w, ResultsTitle = Score(w.Title, filterText), ResultsProcessTitle = Score(w.ProcessTitle, filterText) })
                .Where(r => r.ResultsTitle.Any(wt => wt.Matched) || r.ResultsProcessTitle.Any(pt => pt.Matched))
                .OrderByDescending(r => r.ResultsTitle.Sum(wt => wt.Score) + r.ResultsProcessTitle.Sum(pt => pt.Score))
                .Select(r => new FilterResult { AppWindow = r.Window, WindowTitleMatchResults = r.ResultsTitle, ProcessTitleMatchResults = r.ResultsProcessTitle });
        }

        private static List<MatchResult> Score(string title, string filterText)
        {
            var startsWithMatcher = new StartsWithMatcher();
            var containsMatcher = new ContainsMatcher();
            var significantCharactersMatcher = new SignificantCharactersMatcher();
            var individualCharactersMatcher = new IndividualCharactersMatcher();

            var results = new List<MatchResult>
            {
                startsWithMatcher.Evaluate(title, filterText),
                significantCharactersMatcher.Evaluate(title, filterText),
                containsMatcher.Evaluate(title, filterText),
                individualCharactersMatcher.Evaluate(title, filterText)
            };

            return results;
        }

        private static void LoadSettings()
        {
            ExceptionList = Properties.Settings.Default.Exceptions.Cast<string>().ToList();
        }
        
        private static bool EnumWindows(IntPtr hWnd, int lParam)
        {           
            if (!WinApi.IsWindowVisible(hWnd))
                return true;

            var title = new StringBuilder(256);
            WinApi.GetWindowText(hWnd, title, 256);

            if (string.IsNullOrEmpty(title.ToString())) {
                return true;
            }

            //Exclude windows on the exclusion list
            if (ExceptionList.Contains(title.ToString())) {
                return true;
            }

            if (title.Length != 0 || (title.Length == 0 & hWnd != WinApi.statusbar))
            {
                var appWindow = new AppWindow(hWnd);
                if (appWindow.IsAltTabWindow())
                {
                    WindowList.Add(appWindow);
                }
            }

            return true;
        }
      
        /// <summary>
        /// Builds a regex to filter the titles of open windows.
        /// </summary>
        /// <param name="input">The user-created string to create the regex from</param>
        /// <returns>A filter regex</returns>
        private static Regex BuildPattern(string input)
        {
            var newPattern = "";
            input = input.Trim();
            foreach (var c in input) {
                newPattern += ".*";
                
                // escape regex reserved characters
                newPattern += Regex.Escape(c + "");
            }
            return new Regex(newPattern, RegexOptions.IgnoreCase);
        }
    }
}
