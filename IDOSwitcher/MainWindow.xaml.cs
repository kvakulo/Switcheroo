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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ManagedWinapi;

namespace Switcheroo
{
    public partial class MainWindow : Window
    {
        private List<AppWindow> _windowList;
        private System.Windows.Forms.NotifyIcon _notifyIcon;                
        private HotKey _hotkey;

        public readonly static RoutedUICommand CloseWindowCommand = new RoutedUICommand();
        public readonly static RoutedUICommand SwitchToWindowCommand = new RoutedUICommand();
        public readonly static RoutedUICommand ScrollListDownCommand = new RoutedUICommand();
        public readonly static RoutedUICommand ScrollListUpCommand = new RoutedUICommand();
        private OptionsWindow _optionsWindow;
        private AboutWindow _aboutWindow;

        public MainWindow()
        {           
            InitializeComponent();            
        
            SetUpNotifyIcon();

            SetUpHotKey();

            CheckForUpdates();

            Opacity = 0;
        }

        /// =================================
        #region Private Methods
        /// =================================
        private void SetUpHotKey()
        {
            Core.Initialize();
            _hotkey = Core.HotKey;
            _windowList = Core.WindowList;
            _hotkey.HotkeyPressed += hotkey_HotkeyPressed;
            try
            {
                _hotkey.Enabled = true;
            }
            catch (HotkeyAlreadyInUseException)
            {
                MessageBox.Show("Could not register hotkey (already in use).", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SetUpNotifyIcon()
        {
            var bmp = Properties.Resources.arrow_switch;
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = "Switcheroo",
                Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon()),
                Visible = true,
                ContextMenu = new System.Windows.Forms.ContextMenu(new[]
                {
                    new System.Windows.Forms.MenuItem("Options", (s, e) => Options()),
                    new System.Windows.Forms.MenuItem("About", (s, e) => About()),
                    new System.Windows.Forms.MenuItem("Exit", (s, e) => Quit())
                })
            };
        }

        private void CheckForUpdates()
        {
            var timer = new DispatcherTimer();

            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                Task.Factory.StartNew(() =>
                {
                    var latestVersion = GetLatestVersion();
                    var currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                    if (latestVersion > currentVersion)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var result = MessageBox.Show(
                                string.Format("Switcheroo v{0} is available (you have v{1}).\r\n\r\nDo you want to download it?",
                                   latestVersion, currentVersion),
                                "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (result == MessageBoxResult.Yes)
                            {
                                Process.Start("https://github.com/kvakulo/Switcheroo/releases/latest");
                            }
                        }));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            timer.Interval = new TimeSpan(24, 0, 0);
                            timer.Start();
                        }));
                    }
                });
            };

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private static Version GetLatestVersion()
        {
            try
            {
                var versionAsString = new WebClient().DownloadString("https://raw.github.com/kvakulo/Switcheroo/update/version.txt");
                Version newVersion;
                if (Version.TryParse(versionAsString, out newVersion))
                {
                    return newVersion;
                }
            }
            catch (WebException)
            {
            }
            return null;
        }

        /// <summary>
        /// Populates the window list with the current running windows.
        /// </summary>
        private void LoadData()
        {
            _windowList.Clear();
            Core.GetWindows();

            foreach (var window in _windowList)
            {
                window.FormattedTitle = HighlightMatchingLetters("", window.Title);
                window.FormattedProcessTitle = HighlightMatchingLetters("", window.ProcessTitle);
            }

            lb.DataContext = null;
            lb.DataContext = _windowList;
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
            HideWindow();
        }

        private void HideWindow()
        {
            Opacity = 0;

            // Avoid flicker by delaying the "Hide" a bit. This makes sure
            // that "Opacity = 0" is taking effekt before the window is hidden.
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (sender, args) =>
            {
                Hide();
                timer.Stop();
            };
            timer.Start();
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
            if(_optionsWindow == null)
            {
                _optionsWindow = new OptionsWindow
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                _optionsWindow.Closed += (sender, args) => _optionsWindow = null;
                _optionsWindow.ShowDialog();
            }
            else
            {
                _optionsWindow.Activate();
            }
        }

        /// <summary>
        /// Show About dialog.
        /// </summary>
        private void About()
        {
            if (_aboutWindow == null)
            {
                _aboutWindow = new AboutWindow
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                _aboutWindow.Closed += (sender, args) => _aboutWindow = null;
                _aboutWindow.ShowDialog();
                
            }
            else
            {
                _aboutWindow.Activate();
            }
        }

        /// <summary>
        /// Quit Switcheroo
        /// </summary>
        private void Quit()
        {
            _notifyIcon.Dispose();
            _notifyIcon = null;
            _hotkey.Dispose();
            Application.Current.Shutdown();
        }

        #endregion


        /// =================================
        #region Event Handlers
        /// =================================
        
        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {           
            e.Cancel = true;
            HideWindow();
        }            
        
        void hotkey_HotkeyPressed(object sender, EventArgs e)
        {
            if (Visibility != Visibility.Visible)
            {
                Show();
                Activate();
                Keyboard.Focus(tb);
                LoadData();
                Opacity = 1;
            }
            else
            {
                HideWindow();
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs args)
        {
            var text = tb.Text;

            Task.Factory.StartNew(() =>
            {
                var appWindows = Core.FilterList(text);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (text != tb.Text) return;

                    foreach (var appWindow in appWindows)
                    {
                        appWindow.FormattedTitle = HighlightMatchingLetters(text, appWindow.Title);
                        appWindow.FormattedProcessTitle = HighlightMatchingLetters(text, appWindow.ProcessTitle);
                    }

                    lb.DataContext = appWindows;
                    if (lb.Items.Count > 0)
                    {
                        lb.SelectedItem = lb.Items[0];
                    }
                }));
            });
        }

        private static string HighlightMatchingLetters(string filterText, string title)
        {
            if (string.IsNullOrEmpty(filterText)) return title;
            var filterChars = filterText.ToCharArray().Select(c => c + "").ToList();
            var titleChars = title.ToCharArray().Select(c => c + "").ToList();

            var lastTitleIndex = 0;

            for (var filterIndex = 0; filterIndex < filterChars.Count; filterIndex++)
            {
                for (var titleIndex = lastTitleIndex; titleIndex < titleChars.Count; titleIndex++)
                {
                    lastTitleIndex = titleIndex + 1;
                    if (filterChars[filterIndex].Equals(titleChars[titleIndex], StringComparison.OrdinalIgnoreCase))
                    {
                        titleChars[titleIndex] = "<Bold><![CDATA[" + titleChars[titleIndex] + "]]></Bold>";
                        break;
                    }
                    else
                    {
                        titleChars[titleIndex] = "<![CDATA[" + titleChars[titleIndex] + "]]>";
                    }
                }
            }
            return string.Join("", titleChars.ToArray());
        }

        private void Hide(object sender, ExecutedRoutedEventArgs e)
        {
            HideWindow();
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
                HideWindow();
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

        private void MainWindow_OnLostFocus(object sender, EventArgs e)
        {
            HideWindow();
        }

        #endregion
    }       
}
