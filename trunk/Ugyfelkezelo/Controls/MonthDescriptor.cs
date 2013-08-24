using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ugyfelkezelo.ViewModel;
using System.Windows.Media;

namespace Ugyfelkezelo.Controls
{
    public class MonthDescriptor : ViewModelBase
    {
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
