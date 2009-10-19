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
        public static List<AppWindow> WindowList = new List<AppWindow>();

        public static void Initialize() 
        {       
            hotkey.LoadSettings();          
        }
       
        public static void GetWindows()
        {
            WinAPI.EnumWindowsProc callback = new WinAPI.EnumWindowsProc(EnumWindows);
            WinAPI.EnumWindows(callback, 0);
        }

        public static IEnumerable<IDOSwitcher.AppWindow> FilterList(string filterText)
        {
            Regex filter = BuildPattern(filterText);
            var filtered_windows = from w in WindowList
                                   where filter.Match(w.title).Success
                                   orderby !w.title.StartsWith(filterText, StringComparison.OrdinalIgnoreCase)
                                   orderby (w.title.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) < 0)
                                   select w;

            return filtered_windows;
        }

        private static bool EnumWindows(IntPtr hWnd, int lParam)
        {
            // TODO: Make this a settable option
            string[] excludeList = { "Program Manager", "VirtuaWinMainClass" };

            if (!WinAPI.IsWindowVisible(hWnd))
                return true;

            StringBuilder title = new StringBuilder(256);
            WinAPI.GetWindowText(hWnd, title, 256);

            if (string.IsNullOrEmpty(title.ToString())) {
                return true;
            }

            //Exclude windows on the exclusion list
            if (excludeList.Contains(title.ToString())) {
                return true;
            }

            if (title.Length != 0 || (title.Length == 0 & hWnd != WinAPI.statusbar)) {
                WindowList.Add(new AppWindow(hWnd, title.ToString(), WinAPI.IsIconic(hWnd), WinAPI.IsZoomed(hWnd), WinAPI.GetAppIcon(hWnd)));
            }

            return true;
        }
      
        private static Regex BuildPattern(string input)
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
