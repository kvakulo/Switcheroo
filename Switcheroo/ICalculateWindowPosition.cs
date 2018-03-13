using System.Windows.Forms;

namespace Switcheroo
{
    public interface ICalculateWindowPosition
    {
        WindowPosition CalculateWindowPosition(Screen screen, double windowWidth, double windowHeight);
    }
}