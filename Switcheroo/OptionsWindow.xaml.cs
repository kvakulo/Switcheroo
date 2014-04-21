using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Switcheroo
{
    /// <summary>
    /// Interaction logic for options.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private readonly HotKey _hotkey;
        private readonly List<string> _exceptions;

        public OptionsWindow()
        {
            InitializeComponent();

            Regex filter = new Regex("^([A-Z]|F([1-9]|1[0-2])|Space)$");
            var keyList = Enum.GetValues(typeof(Keys))
                              .Cast<Keys>()
                              .Where(x => filter.Match(x.ToString()).Success);  
            Keys.DataContext = keyList;
            
            // Highlight what's already selected     
            _hotkey = (HotKey) Application.Current.Properties["hotkey"];
            _hotkey.LoadSettings();
            Keys.SelectedItem = _hotkey.KeyCode;
            Alt.IsChecked = _hotkey.Alt;
            Ctrl.IsChecked = _hotkey.Ctrl;
            WindowsKey.IsChecked = _hotkey.WindowsKey;
            Shift.IsChecked = _hotkey.Shift;

            // Populate text box
            _exceptions = (List<string>) Application.Current.Properties["exceptions"];
            ExceptionList.Text = String.Join(Environment.NewLine, _exceptions);

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Change the active hotkey
            _hotkey.Alt = (bool)Alt.IsChecked;
            _hotkey.Shift = (bool)Shift.IsChecked;
            _hotkey.Ctrl = (bool)Ctrl.IsChecked;
            _hotkey.WindowsKey = (bool)WindowsKey.IsChecked;
            _hotkey.KeyCode = (Keys)Keys.SelectedItem;
            _hotkey.SaveSettings();

            // Save edited text list to app config
            var tempExclusionList = ExceptionList.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            _exceptions.Clear();
            _exceptions.AddRange(tempExclusionList);
            Properties.Settings.Default.Exceptions.Clear();
            Properties.Settings.Default.Exceptions.AddRange(tempExclusionList);
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
