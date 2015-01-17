using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switcheroo.Core
{
	public class WindowCloser
	{
		private static readonly TimeSpan _checkInterval = TimeSpan.FromMilliseconds( 125 );
		private static readonly TimeSpan _closeWaitInterval = TimeSpan.FromSeconds( 1 );

		public async Task<bool> TryCloseAsync( AppWindowViewModel window )
		{
			window.IsBeingClosed = true;
			window.AppWindow.PostClose();

			int attempts = (int) ( _closeWaitInterval.TotalMilliseconds / _checkInterval.TotalMilliseconds );
			for ( int i = 0; i < attempts && !window.AppWindow.IsClosed; i++ )
				await Task.Delay( _checkInterval ).ConfigureAwait( false );

			return window.AppWindow.IsClosed;
		}
	}
}
