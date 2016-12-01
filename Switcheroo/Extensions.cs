using System;
using System.Collections.Generic;
using System.Linq;

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
	}
}