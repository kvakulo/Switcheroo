using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace IDOSwitcher
{
    public class window
    {
        public IntPtr handle;
        public string title { get; set; }
        public bool isminimzed;
        public bool ismaximized;
        public Icon icon;
        //public Image img;

        public window()
        {
            handle = IntPtr.Zero;
            title = "";
            isminimzed = false;
            ismaximized = false;
        }

        public window(IntPtr handle, string title, bool isminimized, bool ismaximized, Icon icon)
        {
            this.handle = handle;
            this.title = title;
            this.isminimzed = isminimized;
            this.ismaximized = ismaximized;
            this.icon = icon;
            //this.img = icon.ToBitmap();
        }

        public override string ToString()
        {
            return this.title;
        }
    }
}
