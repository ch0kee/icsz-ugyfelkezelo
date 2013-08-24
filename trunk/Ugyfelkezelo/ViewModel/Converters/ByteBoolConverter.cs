using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Ugyfelkezelo.ViewModel.Converters
{
    class ByteBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int32 param = Byte.Parse(parameter.ToString());
            Int32 val = (Int32)value;
            if (val == param)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean chk = (Boolean)value;
            if (!chk)
                return Binding.DoNothing;
            Int32 param = Int32.Parse(parameter.ToString());
            return param;
        }
    }
}
