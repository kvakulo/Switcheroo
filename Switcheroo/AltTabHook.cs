using System;
using System.Diagnostics;
using System.Windows.Forms;
using ManagedWinapi;
using ManagedWinapi.Hooks;

namespace Switcheroo
{
    public delegate void AltTabHookEventHandler(object sender, AltTabHookEventArgs args);

    public class AltTabHookEventArgs : EventArgs
    {
        public bool ShiftDown { get; set; }
        public bool Handled { get; set; }
    }

    public class AltTabHook : IDisposable
    {
        public event AltTabHookEventHandler Pressed;
        private const int AltDown = 32;
        private readonly KeyboardKey _shiftKey = new KeyboardKey(Keys.LShiftKey);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly LowLevelKeyboardHook _lowLevelKeyboardHook;

        public AltTabHook()
        {
            _lowLevelKeyboardHook = new LowLevelKeyboardHook();
            _lowLevelKeyboardHook.MessageIntercepted += OnMessageIntercepted;
            _lowLevelKeyboardHook.StartHook();
        }

        private void OnMessageIntercepted(LowLevelMessage lowLevelMessage, ref bool handled)
        {
            var keyboardMessage = lowLevelMessage as LowLevelKeyboardMessage;
            if (handled || keyboardMessage == null)
            {
                return;
            }

            if (!IsAltTabKeyCombination(keyboardMessage))
            {
                return;
            }

            var shiftKeyDown = (_shiftKey.AsyncState & 32768) != 0; // is held down
            Trace.WriteLine("Shiftkey: " + shiftKeyDown);

            var eventArgs = OnPressed(shiftKeyDown);

            handled = eventArgs.Handled;
        }

        private static bool IsAltTabKeyCombination(LowLevelKeyboardMessage keyboardMessage)
        {
            return keyboardMessage.VirtualKeyCode == (int)Keys.Tab
                   && keyboardMessage.Flags == AltDown;
        }

        private AltTabHookEventArgs OnPressed(bool shiftDown)
        {
            var altTabHookEventArgs = new AltTabHookEventArgs { ShiftDown = shiftDown };
            var handler = Pressed;
            if (handler != null)
            {
                handler(this, altTabHookEventArgs);
            }
            return altTabHookEventArgs;
        }

        public void Dispose()
        {
            if (_lowLevelKeyboardHook != null)
            {
                _lowLevelKeyboardHook.Dispose();
            }
        }
    }


}
