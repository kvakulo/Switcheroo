using System;
using System.Collections.Generic;
using System.Text;

namespace IDOSwitcher
{
    public class window
    {
        public IntPtr handle;
        public string title;
        public bool isminimzed;
        public bool ismaximized;

        public window()
        {
            handle = IntPtr.Zero;
            title = "";
            isminimzed = false;
            ismaximized = false;
        }

        public window(IntPtr handle, string title, bool isminimized, bool ismaximized)
        {
            this.handle = handle;
            this.title = title;
            this.isminimzed = isminimized;
            this.ismaximized = ismaximized;
        }

        public override string ToString()
        {
            return this.title;
        }
    }
}
