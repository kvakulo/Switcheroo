using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switcheroo.Core
{
	public interface IWindowText
	{
		string WindowTitle { get; }
		string ProcessTitle { get; }
	}
}
