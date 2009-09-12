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
        private IEnumerable<string> windows;
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
                
        public MainWindow()
        {           
            InitializeComponent();
            LoadData();
            tb.Focus();

            // Handle notification icon stuff
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "Fantastic is active.";
            m_notifyIcon.BalloonTipTitle = "Fantastic";
            m_notifyIcon.Icon = new System.Drawing.Icon(@"B:\workspace\idoswitcher\IDOSwitcher\IDOSwitcher\notifyicon.ico");
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);

        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //m_notifyIcon.Dispose();
            //m_notifyIcon = null;
            Hide();
        }

        private WindowState m_storedWindowState = WindowState.Normal;

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
            Show();
            WindowState = m_storedWindowState;
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

        void LoadData()
        {
            windows = from p in Process.GetProcesses()
                      where p.MainWindowTitle != "" 
                      orderby p.MainWindowTitle
                      select p.MainWindowTitle;

            lb.DataContext = windows;
        }

        void FilterList(Regex filter)
        {
            var filtered_windows =  from w in windows
                                    where filter.Match(w).Success
                                    select w;
            lb.DataContext = filtered_windows;
        }

        static Regex BuildPattern(string input)
        {
            string newPattern = "";
            foreach (char c in input) {
                newPattern += ".{0,5}";
                newPattern += c;
            }
            return new Regex(newPattern, RegexOptions.IgnoreCase);
        }

        void PrintText(object sender, SelectionChangedEventArgs args)
        {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
            //tb.Text = "   You selected " + lbi.Content.ToString() + ".";
        }

        void TextChanged(object sender, TextChangedEventArgs args)
        {                        
            Regex pattern = BuildPattern(tb.Text);
            FilterList(pattern);
            if (lb.Items.Count > 0) {
                lb.SelectedItem = lb.Items[0];
            }            
        }

        private void tb_KeyUp(object sender, KeyEventArgs e)
        {            
            if (e.Key == System.Windows.Input.Key.Enter) {
                //MessageBox.Show("Enter!");

                var w = from p in Process.GetProcesses()
                        where p.MainWindowTitle == lb.SelectedValue.ToString()
                        select p;
                foreach (Process p in w) {
                    SwitchToThisWindow(p.MainWindowHandle);
                }

                Environment.Exit(0);
            }
            else if (lb.SelectedIndex != lb.Items.Count - 1  && e.Key == System.Windows.Input.Key.Down) {
                lb.SelectedIndex = lb.SelectedIndex + 1;
            }
            else if (lb.SelectedIndex != 0 && e.Key == System.Windows.Input.Key.Up) {
                lb.SelectedIndex = lb.SelectedIndex - 1;
            }

        }



      

    }
}
