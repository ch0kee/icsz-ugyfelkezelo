using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ugyfelkezelo.ViewModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;

namespace Ugyfelkezelo.Controls
{
    public class MonthDescriptionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] parts = ((string)value).Split(new char[] { ' ' });
                return new MonthDescriptor(Int32.Parse(parts[0]), parts[1]);
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((MonthDescriptor)value).Index + " " + ((MonthDescriptor)value).Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(MonthDescriptionConverter))]
    public class MonthDescriptor : ViewModelBase
    {
        //c hogy xaml-ben lehessen megadni
        public MonthDescriptor(string desc)
        {

        }

        public MonthDescriptor(int index, string name)
        {
            Name = name;
            Index = index;
            _Enabled = false;
        }

        public String Name { get; private set; }
        public Int32 Index { get; private set; }

        private Boolean _Enabled;

        public Boolean Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; OnPropertyChanged("Color"); }
        }


        public Color Color
        {
            get
            {
                if (_Enabled)
                    return Colors.White;
                else
                    return Colors.Yellow;
            }
        }

        public Int32 X { get; set; }
        public Int32 Y { get; set; }

        public Int32 Number { get { return X * 9 + Y; } }

        public DelegateCommand SelectMonth { get; set; }
    }
}
