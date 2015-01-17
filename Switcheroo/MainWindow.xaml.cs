/*
 * Switcheroo - The incremental-search task switcher for Windows.
 * http://www.switcheroo.io/
 * Copyright 2009, 2010 James Sulak
 * Copyright 2014 Regin Larsen
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using ManagedWinapi;
using ManagedWinapi.Windows;
using Switcheroo.Core;
using Switcheroo.Core.Matchers;
using Switcheroo.Properties;
using Application = System.Windows.Application;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace Switcheroo
{
    public partial class MainWindow : Window
    {
		private WindowCloser _windowCloser;
        private ObservableCollection<AppWindowViewModel> _windowList;
        private NotifyIcon _notifyIcon;                
        private HotKey _hotkey;

        public readonly static RoutedUICommand CloseWindowCommand = new RoutedUICommand();
        public readonly static RoutedUICommand SwitchToWindowCommand = new RoutedUICommand();
        public readonly static RoutedUICommand ScrollListDownCommand = new RoutedUICommand();
        public readonly static RoutedUICommand ScrollListUpCommand = new RoutedUICommand();
        private OptionsWindow _optionsWindow;
        private AboutWindow _aboutWindow;
        private AltTabHook _altTabHook;

        public MainWindow()
        {           
            InitializeComponent();            
        
            SetUpNotifyIcon();

            SetUpHotKey();

            SetUpAltTabHook();

            CheckForUpdates();

            Opacity = 0;
        }

        /// =================================
        #region Private Methods
        /// =================================
        private void SetUpHotKey()
        {
            _hotkey = new HotKey();
            _hotkey.LoadSettings();

            Application.Current.Properties["hotkey"] = _hotkey;

            _hotkey.HotkeyPressed += hotkey_HotkeyPressed;
            try
            {
                _hotkey.Enabled = true;
            }
            catch (HotkeyAlreadyInUseException)
            {
                var boxText = "The current hotkey for activating Switcheroo is in use by another program." +
                                     Environment.NewLine +
                                     Environment.NewLine +
                                     "You can change the hotkey by right-clicking the Switcheroo icon in the system tray and choosing 'Options'.";
                MessageBox.Show(boxText, "Hotkey already in use", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetUpAltTabHook()
        {
            _altTabHook = new AltTabHook();
            _altTabHook.Pressed += AltTabPressed;
        }

        private void SetUpNotifyIcon()
        {
            var icon = Properties.Resources.icon;

            var runOnStartupMenuItem = new MenuItem("Run on Startup", (s, e) => RunOnStartup(s as MenuItem))
            {
                Checked = new AutoStart().IsEnabled
            };

            _notifyIcon = new NotifyIcon
            {
                Text = "Switcheroo",
                Icon = icon,
                Visible = true,
                ContextMenu = new System.Windows.Forms.ContextMenu(new[]
                {
                    new MenuItem("Options", (s, e) => Options()),
                    runOnStartupMenuItem,
                    new MenuItem("About", (s, e) => About()),
                    new MenuItem("Exit", (s, e) => Quit())
                })
            };
        }

        private static void RunOnStartup(MenuItem menuItem)
        {
            try
            {
                var autoStart = new AutoStart
                {
                    IsEnabled = !menuItem.Checked
                };
                menuItem.Checked = autoStart.IsEnabled;
            }
            catch (AutoStartException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static void CheckForUpdates()
        {
            var timer = new DispatcherTimer();

            timer.Tick += async (sender, args) =>
            {
                timer.Stop();
                var latestVersion = await GetLatestVersion();
                var currentVersion = Assembly.GetEntryAssembly().GetName().Version;
                if (latestVersion != null && latestVersion > currentVersion)
                {
                    var result = MessageBox.Show(
                        string.Format("Switcheroo v{0} is available (you have v{1}).\r\n\r\nDo you want to download it?",
                           latestVersion, currentVersion),
                        "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start("https://github.com/kvakulo/Switcheroo/releases/latest");
                    }
                }
                else
                {
                    timer.Interval = new TimeSpan(24, 0, 0);
                    timer.Start();
                }
            };

            timer.Interval = new TimeSpan(0, 5, 0);
            timer.Start();
        }

        private static async Task<Version> GetLatestVersion()
        {
            try
            {
                var versionAsString = await new WebClient().DownloadStringTaskAsync("https://raw.github.com/kvakulo/Switcheroo/update/version.txt");
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
			_windowList = new ObservableCollection<AppWindowViewModel>( new WindowFinder().GetWindows().Select( window => new AppWindowViewModel( window ) ) );
			_windowCloser = new WindowCloser();

            foreach (var window in _windowList)
            {
                window.FormattedTitle = new XamlHighlighter().Highlight(new[] { new StringPart(window.AppWindow.Title) });
                window.FormattedProcessTitle = new XamlHighlighter().Highlight(new[] { new StringPart(window.AppWindow.ProcessTitle) });
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
                var win = (AppWindowViewModel) (lb.SelectedItem ?? lb.Items[0]);
                win.AppWindow.SwitchTo();
            }
            HideWindow();
        }

        private void HideWindow()
        {
			_windowCloser.Dispose();
            Opacity = 0;

            // Avoid flicker by delaying the "Hide" a bit. This makes sure
            // that "Opacity = 0" is taking effect before the window is hidden.
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
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
        
        private void hotkey_HotkeyPressed(object sender, EventArgs e)
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

        private void AltTabPressed(object sender, AltTabHookEventArgs e)
        {
            if (!Settings.Default.AltTabHook)
            {
                // Ignore Alt+Tab presses if the hook is not activated by the user
                return;
            }

            e.Handled = true;

            if (Visibility != Visibility.Visible)
            {
                ActivateAndFocusMainWindow();

                Keyboard.Focus(tb);
                LoadData();

                if (e.ShiftDown)
                {
                    lb.SelectedIndex = lb.Items.Count - 1;
                }
                else
                {
                    lb.SelectedIndex = 1;
                }

                Opacity = 1;

            }
            else
            {
                if (e.ShiftDown)
                {
                    PreviousItem();
                }
                else
                {
                    NextItem();
                }
            }
        }

        private void ActivateAndFocusMainWindow()
        {
            // What happens below looks a bit weird, but for Switcheroo to get focus when using the Alt+Tab hook,
            // it is needed to simulate an Alt keypress will bring Switcheroo to the foreground. Otherwise Switcheroo
            // will become the foreground window, but the previous window will retain focus, and receive keep getting
            // the keyboard input.
            // http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo

            var thisWindowHandle = new WindowInteropHelper(this).Handle;
            var thisWindow = new AppWindow(thisWindowHandle);

            var altKey = new KeyboardKey(Keys.Alt);
            var altKeyPressed = false;

            // Press the Alt key if it is not already being pressed
            if ((altKey.AsyncState & 0x8000) == 0)
            {
                altKey.Press();
                altKeyPressed = true;
            }

            // Bring the Switcheroo window to the foreground
            Show();
            SystemWindow.ForegroundWindow = thisWindow;
            Activate();

            // Release the Alt key if it was pressed above
            if (altKeyPressed)
            {
                altKey.Release();
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs args)
        {
            var text = tb.Text;

			var filterResults = new WindowFilterer().Filter( _windowList, text ).ToList();

            foreach (var filterResult in filterResults)
            {
                filterResult.AppWindow.FormattedTitle = GetFormattedTitleFromBestResult(filterResult.WindowTitleMatchResults);
                filterResult.AppWindow.FormattedProcessTitle = GetFormattedTitleFromBestResult(filterResult.ProcessTitleMatchResults);
            }

			lb.DataContext = new ObservableCollection<AppWindowViewModel>( filterResults.Select( r => r.AppWindow ) );
            if (lb.Items.Count > 0)
            {
                lb.SelectedItem = lb.Items[0];
            }
        }

        private static string GetFormattedTitleFromBestResult(IList<MatchResult> matchResults)
        {
            var bestResult = matchResults.FirstOrDefault(r => r.Matched) ?? matchResults.First();
            return new XamlHighlighter().Highlight(bestResult.StringParts);
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

        private async void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (lb.Items.Count > 0)
            {
                var win = (AppWindowViewModel)lb.SelectedItem;
                if (win != null)
                {
					bool isClosed = await _windowCloser.TryCloseAsync( win );
					if ( isClosed )
						RemoveWindow( win );
                }
            }
            else
            {
                HideWindow();
            }
            e.Handled = true;
        }

		private void RemoveWindow( AppWindowViewModel window )
		{
			int index = _windowList.IndexOf( window );
			if ( index < 0 )
				return;

			if ( lb.SelectedIndex == index )
			{
				if ( _windowList.Count > index + 1 )
					lb.SelectedIndex++;
				else
				{
					if ( index > 0 )
						lb.SelectedIndex--;
				}
			}

			_windowList.Remove( window );
			SizeToContent = SizeToContent.WidthAndHeight;
			SizeToContent = SizeToContent.Manual;
		}

        private void ScrollListUp(object sender, ExecutedRoutedEventArgs e)
        {
            PreviousItem();
            e.Handled = true;
        }

        private void PreviousItem()
        {
            if (lb.Items.Count > 0)
            {
                if (lb.SelectedIndex != 0)
                {
                    lb.SelectedIndex--;
                }
                else
                {
                    lb.SelectedIndex = lb.Items.Count - 1;
                }
            }
        }

        private void ScrollListDown(object sender, ExecutedRoutedEventArgs e)
        {
            NextItem();
            e.Handled = true;
        }

        private void NextItem()
        {
            if (lb.Items.Count > 0)
            {
                if (lb.SelectedIndex != lb.Items.Count - 1)
                {
                    lb.SelectedIndex++;
                }
                else
                {
                    lb.SelectedIndex = 0;
                }
            }
        }

        private void MainWindow_OnLostFocus(object sender, EventArgs e)
        {
            HideWindow();
        }

        #endregion
    }       
}
