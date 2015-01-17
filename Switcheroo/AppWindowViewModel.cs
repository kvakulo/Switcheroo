using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Switcheroo.Core;

namespace Switcheroo
{
	public class AppWindowViewModel: PropertyChangedBase
	{
		public AppWindowViewModel( AppWindow appWindow )
		{
			AppWindow = appWindow;
		}

		public AppWindow AppWindow { get; private set; }

		#region Bindable properties

		public IntPtr HWnd
		{
			get { return AppWindow.HWnd; }
		}

		public string FormattedTitle
		{
			get { return AppWindow.FormattedTitle; }
			set { AppWindow.FormattedTitle = value; NotifyOfPropertyChange( () => FormattedTitle ); }
		}

		public string FormattedProcessTitle
		{
			get { return AppWindow.FormattedProcessTitle; }
			set { AppWindow.FormattedProcessTitle = value; NotifyOfPropertyChange( () => FormattedProcessTitle ); }
		}

		#endregion
	}
}