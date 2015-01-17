using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switcheroo.Core
{
	public class WindowCloser: IDisposable
	{
		private bool _isDisposed = false;

		private static readonly TimeSpan _checkInterval = TimeSpan.FromMilliseconds( 125 );

		public async Task<bool> TryCloseAsync( AppWindowViewModel window )
		{
			window.IsBeingClosed = true;
			window.AppWindow.PostClose();

			while ( !_isDisposed && !window.AppWindow.IsClosedOrHidden )
				await Task.Delay( _checkInterval ).ConfigureAwait( false );

			return window.AppWindow.IsClosedOrHidden;
		}

		#region IDisposable Members

		public void Dispose()
		{
			_isDisposed = true;
		}

		#endregion
	}
}
