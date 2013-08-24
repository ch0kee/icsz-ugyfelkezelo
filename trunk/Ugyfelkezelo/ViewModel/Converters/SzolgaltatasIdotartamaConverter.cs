using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Ugyfelkezelo.ViewModel.Converters
{
    class SzolgaltatasIdotartamaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int32 param = Int32.Parse(parameter.ToString());
            if (param == 0)
            {
                //hatarozatlan
                return value == null;
            }
            else
            {
                //hatarozott
                return value != null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean chk = (Boolean)value;
            if (!chk)
                return Binding.DoNothing;
            Int32 param = Int32.Parse(parameter.ToString());
            if (param == 0)
            {
                //hatarozatlan
                return null;
            }
            return value;
        }
    }
}
