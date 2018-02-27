using System;
using System.Collections.Generic;
using System.Linq;
using ManagedWinapi.Windows;

namespace Switcheroo
{
	public static class Extensions
	{
		public static bool IsPinnedToBottom(this AppWindowViewModel appWindow, IEnumerable<IntPtr> pinnedList)
		{
			return pinnedList.Any(ptr => (long)ptr == (long)appWindow.HWnd);
		}

		public static void PinToBottom(this AppWindowViewModel appWindow, List<IntPtr> pinnedList)
		{
			if (appWindow.IsPinnedToBottom(pinnedList))
				return;

			pinnedList.Add(appWindow.HWnd);
		}

		public static void UnpinFromBottom(this AppWindowViewModel appWindow, List<IntPtr> pinnedList)
		{
			if (!appWindow.IsPinnedToBottom(pinnedList))
				return;

			pinnedList.RemoveAll(ptr => (long) ptr == (long) appWindow.HWnd);
		}

		public static List<AppWindowViewModel> Sort(this IEnumerable<AppWindowViewModel> windowList,
			SystemWindow foregroundWindow, IEnumerable<IntPtr> pinnedList)
		{
			var result = new List<AppWindowViewModel>();

			var windowArray = windowList as AppWindowViewModel[] ?? windowList.ToArray();
			result.AddRange(windowArray.Where(w => !w.IsPinnedToBottom(pinnedList)));
			result.AddRange(windowArray.Where(w => w.IsPinnedToBottom(pinnedList)));

			var firstWindow = result.FirstOrDefault();

			if (firstWindow != null && firstWindow.AppWindow.IsRelatedTo(foregroundWindow))
			{
				result.RemoveAt(0);
				result.Add(firstWindow);
			}

			return result;
		}

		public static bool IsRelatedTo(this SystemWindow window1, SystemWindow window2)
		{
			return window1.HasTheSameHandleAs(window2) || window1.HasTheSameProcessIdAs(window2);
		}

		public static bool HasTheSameHandleAs(this SystemWindow window1, SystemWindow window2)
		{
			
            return window1.HWnd == window2.HWnd;
		}

		public static bool HasTheSameProcessIdAs(this SystemWindow window1, SystemWindow window2)
		{
			return window1.Process.Id == window2.Process.Id;
		}
	}
}