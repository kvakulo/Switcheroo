using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Drawing;

namespace IDOSwitcher
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<window> windows = new List<window>();
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private WindowState m_storedWindowState = WindowState.Normal;
        // system-wide keyboard hook
        private KeyboardHook _hook;
                
        public MainWindow()
        {           
            InitializeComponent();
            LoadData();
            tb.Focus();

            // Handle notification icon stuff
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "Fantastic is active.";
            m_notifyIcon.BalloonTipTitle = "Fantastic";
            m_notifyIcon.Icon = new System.Drawing.Icon(GetType(), @"notifyicon.ico");
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);            
            m_notifyIcon.Visible = true;
            m_notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Quit", (s, e) => Quit())
            });
            Hide();
            
            // Set keyboard hooks
            _hook = new KeyboardHook();
            _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);

        }

        private void Quit()
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;            
            Environment.Exit(0);  
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {           
            e.Cancel = true;
            Hide();
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized) {
                Hide();
                if (m_notifyIcon != null) {
                    m_notifyIcon.ShowBalloonTip(2000);
                }
            }
            else {
                m_storedWindowState = WindowState;
                LoadData();
            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            //LoadData();
            //Show();
            //WindowState = m_storedWindowState;
        }

        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null) {
                m_notifyIcon.Visible = show;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SwitchToThisWindow(IntPtr hWnd);


        private void getwindows()
        {
            winapi.EnumWindowsProc callback = new winapi.EnumWindowsProc(enumwindows);
            winapi.EnumWindows(callback, 0);
        }

        private bool enumwindows(IntPtr hWnd, int lParam)
        {
            if (!winapi.IsWindowVisible(hWnd))
                return true;

            StringBuilder title = new StringBuilder(256);
            winapi.GetWindowText(hWnd, title, 256);

            if (string.IsNullOrEmpty(title.ToString())) {
                return true;
            }

            if (title.Length != 0 || (title.Length == 0 & hWnd != winapi.statusbar)) {
                windows.Add(new window(hWnd, title.ToString(), winapi.IsIconic(hWnd), winapi.IsZoomed(hWnd), winapi.GetAppIcon(hWnd)));
            }

            return true;
        }

        void LoadData()
        {
            windows.Clear();
            getwindows();            
            lb.DataContext = windows;
            tb.Clear();
            tb.Focus();
            Focus();
            //These two lines size upon load, but don't whiplash resize during typing
            SizeToContent = SizeToContent.Width;
            SizeToContent = SizeToContent.Manual;
        }


        // TODO: Change so that any line that matches a .contains() exactly is first and highlighted
        void FilterList(Regex filter)
        {
            var filtered_windows =  from w in windows
                                    where filter.Match(w.title).Success
                                    select w;
            lb.DataContext = filtered_windows;
        }

        static Regex BuildPattern(string input)
        {
            string newPattern = "";
            input = input.Trim();
            foreach (char c in input) {
                newPattern += ".*";
                // escape regex reserved characters
                if (@"[\^$.|?*+(){}".Contains(c)) {
                    newPattern += @"\";
                }                
                newPattern += c;
            }
            return new Regex(newPattern, RegexOptions.IgnoreCase);
        }

        void PrintText(object sender, SelectionChangedEventArgs args)
        {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);            
        }

        void TextChanged(object sender, TextChangedEventArgs args)
        {
            // This compensates for text added as part of the hotkey event.
            // May need to be made smarter later.
            if (tb.Text == " ") {
                tb.Text = "";
                return;
            }
            Regex pattern = BuildPattern(tb.Text);
            FilterList(pattern);
            if (lb.Items.Count > 0) {
                lb.SelectedItem = lb.Items[0];
            }            
        }

        private void tb_KeyUp(object sender, KeyEventArgs e)
        {            
            if (e.Key == System.Windows.Input.Key.Enter) {
                if (lb.Items.Count > 0) {
                    SwitchToThisWindow(((window)lb.SelectedItem).handle);                                      
                }
                Hide();
            }
            else if (lb.SelectedIndex != lb.Items.Count - 1  && e.Key == System.Windows.Input.Key.Down) {
                lb.SelectedIndex = lb.SelectedIndex + 1;
            }
            else if (lb.SelectedIndex != 0 && e.Key == System.Windows.Input.Key.Up) {
                lb.SelectedIndex = lb.SelectedIndex - 1;
            }
            else if (e.Key == System.Windows.Input.Key.Escape) {
                Hide();
            }

        }


        // keyboard hook handler
        void OnHookKeyDown(object sender, HookEventArgs e)
        {
            if (e.Control && e.Key == System.Windows.Forms.Keys.Space) {
                LoadData();
                Show();  
                Activate();
                WindowState = m_storedWindowState;
                Keyboard.Focus(tb);
                //tb.Clear();
            }
        }
      
 

    }
}
