using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Interop;

namespace IDOSwitcher
{
    public class window
    {
        public IntPtr handle;
        public string title { get; set; }
        public bool isminimzed;
        public bool ismaximized;
        public Icon icon;
        //public ImageSource img {get; set;}
               
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

            //Bitmap bitmap = icon.ToBitmap();
            //IntPtr hBitmap = bitmap.GetHbitmap();
            //ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //img = wpfBitmap;
            
        }

        public override string ToString()
        {
            return this.title;
        }
    }
}
