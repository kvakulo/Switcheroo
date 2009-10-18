using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDOSwitcher
{
    public class HotKey : ManagedWinapi.Hotkey
    {
        public void LoadSettings()
        {            
            this.KeyCode = (System.Windows.Forms.Keys)Properties.Settings.Default.HotKey;
            this.WindowsKey = Properties.Settings.Default.WindowsKey;
            this.Alt = Properties.Settings.Default.Alt;
            this.Ctrl = Properties.Settings.Default.Ctrl;
            this.Shift = Properties.Settings.Default.Shift;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.HotKey = (int)this.KeyCode;
            Properties.Settings.Default.WindowsKey = this.WindowsKey;
            Properties.Settings.Default.Alt = this.Alt;
            Properties.Settings.Default.Ctrl = this.Ctrl;
            Properties.Settings.Default.Shift = this.Shift;
            Properties.Settings.Default.Save();
        }
    }
}
