using System;
using System.Threading.Tasks;

namespace Switcheroo
{
    public class WindowCloser : IDisposable
    {
        private bool _isDisposed;

        private static readonly TimeSpan CheckInterval = TimeSpan.FromMilliseconds(125);

        public async Task<bool> TryCloseAsync(AppWindowViewModel window)
        {
            window.IsBeingClosed = true;
            window.AppWindow.Close();

            while (!_isDisposed && !window.AppWindow.IsClosedOrHidden)
                await Task.Delay(CheckInterval).ConfigureAwait(false);

            return window.AppWindow.IsClosedOrHidden;
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}