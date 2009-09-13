/*
 * Class that demonstrates how to use a low level system hook to detect 
 * when any key is pressed. Converts the low level keyboard data into
 * .NET friendly info and raises two events: KeyUp and KeyDown.
 * 
 * Sample usage:
 * 
 *    private KeyboardHook _hook;
 * 
 *    _hook = new KeyboardHook();
 *    _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);
 *
 *    void OnHookKeyDown(object sender, HookEventArgs e)
 *    {
 *       // process the key that was pressed
 *    }
 *
 * Make sure do disable the Visual Studio 'Enable the Visual Studio 
 * hosting process' option in the project debug options.
 * 
 * From http://blogs.vertigo.com/personal/ralph/Blog/archive/2007/02/12/wpf-low-level-keyboard-hook-sample.aspx
*/

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Forms;

namespace IDOSwitcher
{
    // argument sent in event handler
    public class HookEventArgs : EventArgs
    {
        // using Windows.Forms.Keys instead of Input.Key since the Forms.Keys maps
        // to the Win32 KBDLLHOOKSTRUCT virtual key member, where Input.Key does not
        public Keys Key;
        public bool Alt;
        public bool Control;
        public bool Shift;

        public HookEventArgs(UInt32 keyCode)
        {
            // detect what modifier keys are pressed, using 
            // Windows.Forms.Control.ModifierKeys instead of Keyboard.Modifiers
            // since Keyboard.Modifiers does not correctly get the state of the 
            // modifier keys when the application does not have focus
            this.Key = (Keys)keyCode;
            this.Alt = (System.Windows.Forms.Control.ModifierKeys & Keys.Alt) != 0;
            this.Control = (System.Windows.Forms.Control.ModifierKeys & Keys.Control) != 0;
            this.Shift = (System.Windows.Forms.Control.ModifierKeys & Keys.Shift) != 0;
        }
    }

    // hooks system keyboard
    public class KeyboardHook
    {
        #region pinvoke details

        private enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct KBDLLHOOKSTRUCT
        {
            public UInt32 vkCode;
            public UInt32 scanCode;
            public UInt32 flags;
            public UInt32 time;
            public IntPtr extraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(
            HookType code, HookProc func, IntPtr instance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(
            IntPtr hook, int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        #endregion

        HookType _hookType = HookType.WH_KEYBOARD_LL;
        IntPtr _hookHandle = IntPtr.Zero;
        HookProc _hookFunction = null;

        // hook method called by system
        private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        // events
        public delegate void HookEventHandler(object sender, HookEventArgs e);
        public event HookEventHandler KeyDown;
        public event HookEventHandler KeyUp;

        public KeyboardHook()
        {
            _hookFunction = new HookProc(HookCallback);
            Install();
        }

        ~KeyboardHook()
        {
            Uninstall();
        }

        // hook function called by system
        private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (code < 0)
                return CallNextHookEx(_hookHandle, code, wParam, ref lParam);

            // KeyUp event
            if ((lParam.flags & 0x80) != 0 && this.KeyUp != null)
                this.KeyUp(this, new HookEventArgs(lParam.vkCode));

            // KeyDown event
            if ((lParam.flags & 0x80) == 0 && this.KeyDown != null)
                this.KeyDown(this, new HookEventArgs(lParam.vkCode));

            return CallNextHookEx(_hookHandle, code, wParam, ref lParam);
        }

        private void Install()
        {
            // make sure not already installed
            if (_hookHandle != IntPtr.Zero)
                return;

            // need instance handle to module to create a system-wide hook
            Module[] list = System.Reflection.Assembly.GetExecutingAssembly().GetModules();
            System.Diagnostics.Debug.Assert(list != null && list.Length > 0);

            // install system-wide hook
            _hookHandle = SetWindowsHookEx(_hookType,
                _hookFunction, Marshal.GetHINSTANCE(list[0]), 0);
        }

        private void Uninstall()
        {
            if (_hookHandle != IntPtr.Zero) {
                // uninstall system-wide hook
                UnhookWindowsHookEx(_hookHandle);
                _hookHandle = IntPtr.Zero;
            }
        }
    }
}
