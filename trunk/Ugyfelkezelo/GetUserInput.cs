using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ugyfelkezelo.ViewModel;

namespace Ugyfelkezelo
{
    class GetUserInput <TValue >: ViewModelBase
    {
        Control _Control;
        GetUserInputWindow _Window;
        String _HeaderText;
        DependencyProperty _Property;

        TValue _ValueProperty;
        public TValue Value
        {
            get
            {
                return _ValueProperty;
            }
            set
            {
                _ValueProperty = value;
                OnPropertyChanged("Value");
            }
        }


        public GetUserInput (Control control, DependencyProperty property, TValue initialValue, String headerText )
        {
            _Control = control;
            _Property = property;
            _ValueProperty = initialValue;
            _HeaderText = headerText;

            _Window = new GetUserInputWindow(_Control, _Property, _HeaderText);
            _Window.DataContext = this;


        }

        /// <summary>
        /// megjelenítés
        /// </summary>
        /// <returns>okéra nyomott-e</returns>
        public bool Show()
        {
            bool? ret = _Window.ShowDialog();
            Debug.Assert(ret.HasValue);
            return ret.HasValue ? ret.Value : false;           
        }
    }
}
