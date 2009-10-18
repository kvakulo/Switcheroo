using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDOSwitcher
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<window> windows = new List<window>();
        private System.Windows.Forms.NotifyIcon m_notifyIcon;                
        public static IDOSwitcher.HotKey hotkey {get; set;}

        public MainWindow()
        {           
            InitializeComponent();
            Hide();  
        
            // Handle notification icon stuff            
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.Text = "FantastSwitch";           
            Bitmap bmp = IDOSwitcher.Properties.Resources.arrow_switch;
            m_notifyIcon.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());                                          
            m_notifyIcon.Visible = true;

            //Create right-click menu on notification icon
            m_notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Quit", (s, e) => Quit()),
                new System.Windows.Forms.MenuItem("Options", (s, e) => Options())
            });
                     
            // Setup hotkey
            hotkey = new IDOSwitcher.HotKey();        
            hotkey.LoadSettings();
            hotkey.HotkeyPressed += new EventHandler(hotkey_HotkeyPressed);
            try
            {
                hotkey.Enabled = true;
            }
            catch (ManagedWinapi.HotkeyAlreadyInUseException)
            {
                System.Windows.MessageBox.Show("Could not register hotkey (already in use).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Options()
        {            
            Window opts = new IDOSwitcher.options();            
            opts.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            opts.ShowDialog();
        }
        

        private void Quit()
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
            hotkey.Dispose();
            Environment.Exit(0);  
        }       

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {           
            e.Cancel = true;
            Hide();
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
            string[] excludeList = { "Program Manager", "VirtuaWinMainClass" };

            if (!winapi.IsWindowVisible(hWnd))
                return true;

            StringBuilder title = new StringBuilder(256);
            winapi.GetWindowText(hWnd, title, 256);            

            if (string.IsNullOrEmpty(title.ToString())) {
                return true;
            }

            //Exclude windows on the exclusion list
            if (excludeList.Contains(title.ToString())) {
                return true;
            }

            if (title.Length != 0 || (title.Length == 0 & hWnd != winapi.statusbar)) {
                windows.Add(new window(hWnd, title.ToString(), winapi.IsIconic(hWnd), winapi.IsZoomed(hWnd), winapi.GetAppIcon(hWnd)));
            }
            
            return true;
        }

        void hotkey_HotkeyPressed(object sender, EventArgs e)
        {
            LoadData();            
            Show();
            Activate();
            //WindowState = m_storedWindowState;
            Keyboard.Focus(tb);
        }

        public void LoadData()
        {
            windows.Clear();
            getwindows();
            windows.Sort((x, y) => string.Compare(x.title, y.title));
            lb.DataContext = null;
            lb.DataContext = windows;
            tb.Clear();
            tb.Focus();          
            //These two lines size upon load, but don't whiplash resize during typing
            SizeToContent = SizeToContent.Width;
            SizeToContent = SizeToContent.Manual;
            this.Left = (SystemParameters.PrimaryScreenWidth / 2) - (this.ActualWidth / 2);
            this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.ActualHeight / 2);            
        }

        void FilterList()
        {
            Regex filter = BuildPattern(tb.Text);
            var filtered_windows =  from w in windows
                                    where filter.Match(w.title).Success                                                                                         
                                    orderby !w.title.StartsWith(tb.Text, StringComparison.OrdinalIgnoreCase)
                                    orderby (w.title.IndexOf(tb.Text, StringComparison.OrdinalIgnoreCase) < 0)
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
            FilterList();
            if (lb.Items.Count > 0) {
                lb.SelectedItem = lb.Items[0];
            }            
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {            
            switch (e.Key)
            {
                case Key.Enter:
                    if (lb.Items.Count > 0) {
                        SwitchToThisWindow(((window)lb.SelectedItem).handle);
                        //m_notifyIcon.Icon = ((window)lb.SelectedItem).icon;
                    }
                    Hide();
                    break;
                case Key.Down:
                    if (lb.SelectedIndex != lb.Items.Count - 1) {
                        lb.SelectedIndex++;
                    }
                    break;
                case Key.Up:
                    if (lb.SelectedIndex != 0) {
                        lb.SelectedIndex--;
                    }
                    break;
                case Key.Escape:
                    Hide();
                    break;                
                default:
                    break;
            }
        } 
    }
}
