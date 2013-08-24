using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;

namespace Ugyfelkezelo.ViewModel.Converters
{
    class BankszamlaszamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";

            //8asával betesszük a kötőjeleket
            string strValue = (string)value;
            int length = strValue.Length;
            string dirtyValue = strValue.Substring(0, Math.Min(length, 8));
            if (strValue.Length >= 8)
            {
                dirtyValue += '-';
                strValue = new String(strValue.Skip(8).ToArray());
                length = strValue.Length;
                dirtyValue += strValue.Substring(0, Math.Min(length, 8));
                if (strValue.Length >= 8)
                {
                    dirtyValue += '-';
                    strValue = new String(strValue.Skip(8).ToArray());
                    length = strValue.Length;
                    dirtyValue += strValue.Substring(0, Math.Min(length, 8));
                }
            }
            return dirtyValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";
            String strValue = (String)value;
            //kivesszuk a kotojeleket
            string cleanValue = "";
            for (int i = 0; i < strValue.Length; ++i)
                if (strValue[i] != '-')
                    cleanValue += strValue[i];

            return cleanValue;
        }
    }
}
