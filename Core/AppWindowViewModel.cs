using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Switcheroo.Core
{
	public class AppWindowViewModel: INotifyPropertyChanged
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

		private bool _isBeingClosed = false;
		public bool IsBeingClosed
		{
			get { return _isBeingClosed; }
			set { _isBeingClosed = value; NotifyOfPropertyChange( () => IsBeingClosed ); }
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyOfPropertyChange<T>( Expression<Func<T>> property )
		{
			var handler = PropertyChanged;
			if ( handler != null )
				handler( this, new PropertyChangedEventArgs( GetPropertyName( property ) ) );
		}

		private string GetPropertyName<T>( Expression<Func<T>> property )
		{
			var lambda = (LambdaExpression) property;

			MemberExpression memberExpression;
			if ( lambda.Body is UnaryExpression )
			{
				var unaryExpression = (UnaryExpression) lambda.Body;
				memberExpression = (MemberExpression) unaryExpression.Operand;
			}
			else
			{
				memberExpression = (MemberExpression) lambda.Body;
			}

			return memberExpression.Member.Name;
		}

		#endregion
	}
}