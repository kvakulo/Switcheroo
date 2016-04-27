using System;
using System.Drawing;
using System.Windows.Forms;

namespace Switcheroo
{
	class WindowPositionCalculator : ICalculateWindowPosition
	{
		public WindowPositionCalculator()
		{
			
		}
		public WindowPosition CalculateWindowPosition(Screen screen, double windowWidth, double windowHeight)
		{
			return new WindowPosition
			{
				Left = Math.Round(screen.Bounds.X + (((double) screen.Bounds.Width/2) - (windowWidth/2))),
				Top = Math.Round(screen.Bounds.Y + (((double) screen.Bounds.Height/2) - (windowHeight/2)))
			};
		}
	}
}