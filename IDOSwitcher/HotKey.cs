
namespace Switcheroo
{
    public class HotKey : ManagedWinapi.Hotkey
    {
        public void LoadSettings()
        {            
            KeyCode = (System.Windows.Forms.Keys)Properties.Settings.Default.HotKey;
            WindowsKey = Properties.Settings.Default.WindowsKey;
            Alt = Properties.Settings.Default.Alt;
            Ctrl = Properties.Settings.Default.Ctrl;
            Shift = Properties.Settings.Default.Shift;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.HotKey = (int)KeyCode;
            Properties.Settings.Default.WindowsKey = WindowsKey;
            Properties.Settings.Default.Alt = Alt;
            Properties.Settings.Default.Ctrl = Ctrl;
            Properties.Settings.Default.Shift = Shift;
            Properties.Settings.Default.Save();
        }
    }
}
