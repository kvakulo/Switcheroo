using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Switcheroo
{
    public class BoolConverter<T> : IValueConverter
    {
        public T IfTrue { get; set; }
        public T IfFalse { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool) value) ? IfTrue : IfFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class BoolToDoubleConverter : BoolConverter<double>
    {
    }

    public class BoolToColorConverter : BoolConverter<Color>
    {
    }
}