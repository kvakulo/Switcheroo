/*
 * Switcheroo - The incremental-search task switcher for Windows.
 * http://bitbucket.org/jasulak/switcheroo/
 * Copyright 2009, 2010 James Sulak
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Switcheroo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<AppWindow> WindowList;
        private System.Windows.Forms.NotifyIcon m_notifyIcon;                
        private HotKey hotkey;

        public readonly static RoutedUICommand CloseWindowCommand = new RoutedUICommand();
        public readonly static RoutedUICommand SwitchToWindowCommand = new RoutedUICommand();
        public readonly static RoutedUICommand ScrollListDownCommand = new RoutedUICommand();
        public readonly static RoutedUICommand ScrollListUpCommand = new RoutedUICommand();

        public MainWindow()
        {           
            InitializeComponent();            
        
            // Handle notification icon stuff            
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.Text = "Switcheroo";           
            Bitmap bmp = Switcheroo.Properties.Resources.arrow_switch;
            m_notifyIcon.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());                                          
            m_notifyIcon.Visible = true;

            //Create right-click menu on notification icon
            m_notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Options", (s, e) => Options()),
                new System.Windows.Forms.MenuItem("About", (s, e) => About()),
                new System.Windows.Forms.MenuItem("Exit", (s, e) => Quit())               
            });

            Core.Initialize();
            hotkey = Core.HotKey;
            WindowList = Core.WindowList;
            hotkey.HotkeyPressed += new EventHandler(hotkey_HotkeyPressed);
            try {
                hotkey.Enabled = true;
            }
            catch (ManagedWinapi.HotkeyAlreadyInUseException) {
                System.Windows.MessageBox.Show("Could not register hotkey (already in use).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // TODO: HACK: This bit is just so the winow opens up at the proper size the first time.  
            // Otherwise, it's too small the very first time it is activated.
            LoadData();
            Show();
            Hide();
        }


        /// =================================
        #region Private Methods
        /// =================================

        /// <summary>
        /// Populates the window list with the current running windows.
        /// </summary>
        private void LoadData()
        {
            WindowList.Clear();
            Core.GetWindows();
            lb.DataContext = null;
            lb.DataContext = WindowList;
            lb.SelectedIndex = 0;
            tb.Clear();
            tb.Focus();
            Resize();
        }

        /// <summary>
        /// Resizes window to match width and height of list.
        /// </summary>
        private void Resize()
        {
            // These two lines size upon load, but don't whiplash resize during typing
            SizeToContent = SizeToContent.WidthAndHeight;
            SizeToContent = SizeToContent.Manual;
            Left = (SystemParameters.PrimaryScreenWidth / 2) - (ActualWidth / 2);
            Top = (SystemParameters.PrimaryScreenHeight / 2) - (ActualHeight / 2);
        }

        /// <summary>
        /// Switches the window associated with the selected item.
        /// </summary>
        private void Switch()
        {
            if (lb.Items.Count > 0)
            {
                AppWindow win = (AppWindow)lb.SelectedItem ?? (AppWindow)lb.Items[0];
                win.SwitchTo();
            }
            Hide();
        }

        #endregion


        /// =================================
        #region Right-click menu functions
        /// =================================

        /// <summary>
        /// Show Options dialog.
        /// </summary>
        private void Options()
        {            
            Window opts = new Switcheroo.OptionsWindow();            
            opts.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            opts.ShowDialog();
        }

        /// <summary>
        /// Show About dialog.
        /// </summary>
        private void About()
        {
            Window about = new Switcheroo.About();           
            //about.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            about.ShowDialog();
        }

        /// <summary>
        /// Quit Switcheroo
        /// </summary>
        private void Quit()
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
            hotkey.Dispose();
            Environment.Exit(0);  
        }

        #endregion


        /// =================================
        #region Event Handlers
        /// =================================
        
        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {           
            e.Cancel = true;
            Hide();
        }            
        
        void hotkey_HotkeyPressed(object sender, EventArgs e)
        {
            if (Visibility != System.Windows.Visibility.Visible)
            {
                LoadData();
                Show();
                Activate();
                Keyboard.Focus(tb);
            }
            else
            {
                Hide();
            }
        }

        private void PrintText(object sender, SelectionChangedEventArgs args)
        {
            ListBoxItem lbi = (sender as ListBox).SelectedItem as ListBoxItem;            
        }

        private void TextChanged(object sender, TextChangedEventArgs args)
        {            
            lb.DataContext = Core.FilterList(tb.Text);
            if (lb.Items.Count > 0) {
                lb.SelectedItem = lb.Items[0];
            }            
        }

        private void Hide(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
        }
     
        private void OnEnterPressed(object sender, ExecutedRoutedEventArgs e)
        {
            Switch();
            e.Handled = true;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Switch();
            e.Handled = true;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (lb.Items.Count > 0)
            {                
                Hide();
                AppWindow win = (AppWindow)lb.SelectedItem;
                win.PostClose();
                win.SwitchTo();               
            }
            else
            {
                Hide();
            }
            e.Handled = true;
        }

        private void ScrollListUp(object sender, ExecutedRoutedEventArgs e)
        {
            if (lb.SelectedIndex != 0)
            {
                lb.SelectedIndex--;
            }
            e.Handled = true;
        }

        private void ScrollListDown(object sender, ExecutedRoutedEventArgs e)
        {
            if (lb.SelectedIndex != lb.Items.Count - 1)
            {
                lb.SelectedIndex++;
            }
            e.Handled = true;
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            lb.SelectedItem = (sender as StackPanel).DataContext;
            if (!lb.IsFocused) {
                lb.Focus();
            }
        }

        private void MainWindow_OnLostFocus(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion
    }       
}
