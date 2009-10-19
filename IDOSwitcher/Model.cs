using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IDOSwitcher
{
    public static class Model
    {
        public static IDOSwitcher.HotKey hotkey = new IDOSwitcher.HotKey();
        public static List<window> WindowList = new List<window>();

        public static void Initialize() 
        {       
            hotkey.LoadSettings();          
        }
       
        public static void getwindows()
        {
            winapi.EnumWindowsProc callback = new winapi.EnumWindowsProc(enumwindows);
            winapi.EnumWindows(callback, 0);
        }

        private static bool enumwindows(IntPtr hWnd, int lParam)
        {
            // TODO: Make this a settable option
            string[] excludeList = { "Program Manager", "VirtuaWinMainClass" };

            if (!winapi.IsWindowVisible(hWnd))
                return true;

            StringBuilder title = new StringBuilder(256);
            winapi.GetWindowText(hWnd, title, 256);

            if (string.IsNullOrEmpty(title.ToString())) {
                return true;
            }

            //Exclude windows on the exclusion list
            if (excludeList.Contains(title.ToString())) {
                return true;
            }

            if (title.Length != 0 || (title.Length == 0 & hWnd != winapi.statusbar)) {
                WindowList.Add(new window(hWnd, title.ToString(), winapi.IsIconic(hWnd), winapi.IsZoomed(hWnd), winapi.GetAppIcon(hWnd)));
            }

            return true;
        }

        public static IEnumerable<IDOSwitcher.window> FilterList(string FilterText)
        {
            Regex filter = BuildPattern(FilterText);
            var filtered_windows = from w in WindowList
                                   where filter.Match(w.title).Success
                                   orderby !w.title.StartsWith(FilterText, StringComparison.OrdinalIgnoreCase)
                                   orderby (w.title.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) < 0)
                                   select w;

            return filtered_windows;           
        }

        static Regex BuildPattern(string input)
        {
            string newPattern = "";
            input = input.Trim();
            foreach (char c in input) {
                newPattern += ".*";
                // escape regex reserved characters
                if (@"[\^$.|?*+(){}".Contains(c)) {
                    newPattern += @"\";
                }
                newPattern += c;
            }
            return new Regex(newPattern, RegexOptions.IgnoreCase);
        }


    }
}
