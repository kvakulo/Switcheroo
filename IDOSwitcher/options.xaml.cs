using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IDOSwitcher
{
    /// <summary>
    /// Interaction logic for options.xaml
    /// </summary>
    public partial class options : Window
    {
        private HotKey hotkey;

        public options()
        {
            InitializeComponent();

            Regex filter = new Regex("^([A-Z]|F([1-9]|1[0-2])|Space)$");
            var keyList = Enum.GetValues(typeof(Keys))
                              .Cast<Keys>()
                              .Where(x => filter.Match(x.ToString()).Success);  
            Keys.DataContext = keyList;
            
            // Highlight what's already selected     
            hotkey = Model.hotkey;
            Keys.SelectedItem = hotkey.KeyCode;
            Alt.IsChecked = hotkey.Alt;
            Ctrl.IsChecked = hotkey.Ctrl;
            WindowsKey.IsChecked = hotkey.WindowsKey;
            Shift.IsChecked = hotkey.Shift;

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Change the active hotkey
            hotkey.Alt = (bool)Alt.IsChecked;
            hotkey.Shift = (bool)Shift.IsChecked;
            hotkey.Ctrl = (bool)Ctrl.IsChecked;
            hotkey.WindowsKey = (bool)WindowsKey.IsChecked;
            hotkey.KeyCode = (Keys)Keys.SelectedItem;
            hotkey.SaveSettings();
            Close();
        }
    }
}
