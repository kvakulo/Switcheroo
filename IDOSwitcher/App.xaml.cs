using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace IDOSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private ManagedWinapi.Hotkey hotkey;

        //protected override void OnStartup(StartupEventArgs e)
        //{           
        //    hotkey = new ManagedWinapi.Hotkey();
        //    //hotkey.WindowsKey = true;
        //    hotkey.Ctrl = true;
        //    hotkey.KeyCode = System.Windows.Forms.Keys.Space;
        //    hotkey.HotkeyPressed += new EventHandler(hotkey_HotkeyPressed);
        //    try
        //    {
        //        hotkey.Enabled = true;
        //    }
        //    catch (ManagedWinapi.HotkeyAlreadyInUseException)
        //    {
        //        System.Windows.MessageBox.Show("Could not register hotkey (already in use).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }

        //    System.Windows.MessageBox.Show("Hello!");

        //    base.OnStartup(e);
        //}

        //void hotkey_HotkeyPressed(object sender, EventArgs e)
        //{
        //    //System.Windows.MessageBox.Show("Hello!");
        //    MainWindow.Show();           
          
        //    //        Show();  
        //    //        Activate();
        //    //        WindowState = m_storedWindowState;
        //    //        Keyboard.Focus(tb);
        //}

        //protected override void OnExit(ExitEventArgs e)
        //{
        //    hotkey.Dispose();
        //    base.OnExit(e);
        //}
    
    }
}
