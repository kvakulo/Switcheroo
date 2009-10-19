//
// Window.cs
//
// Authors:
//  James Sulak <jsulak@gmail.com>
//
// Copyright (C) 2009 James SUlak


using System;
using System.Drawing;

namespace IDOSwitcher
{

    /// <summary>
    /// This class is a wrapper around the Win32 api window handles
    /// </summary>
    public class AppWindow
    {
        public IntPtr handle { get; set; }
        public string title { get; set; }
        public bool isminimzed;
        public bool ismaximized;
        public Icon icon;
        ////public ImageSource img {get; set;}
               
        public AppWindow()
        {
            handle = IntPtr.Zero;
            title = "";
            isminimzed = false;
            ismaximized = false;
        }
        
        public AppWindow(IntPtr handle, string title, bool isminimized, bool ismaximized, Icon icon)
        {
                                    
            this.handle = handle;
            this.title = title;
            this.isminimzed = isminimized;
            this.ismaximized = ismaximized;
            this.icon = icon;

            ////Bitmap bitmap = icon.ToBitmap();
            ////IntPtr hBitmap = bitmap.GetHbitmap();
            ////ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ////img = wpfBitmap;            
        }

        public override string ToString()
        {
            return this.title;
        }
    }
}
