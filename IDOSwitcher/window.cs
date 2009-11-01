/*
 * Switcheroo - The progressive-search task switcher for Windows.
 * http://bitbucket.org/jasulak/switcheroo/
 * Copyright 2009 James Sulak
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
using System.Drawing;

namespace Switcheroo
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
