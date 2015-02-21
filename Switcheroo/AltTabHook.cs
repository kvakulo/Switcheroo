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
            return keyboardMessage.VirtualKeyCode == (int) Keys.Tab
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