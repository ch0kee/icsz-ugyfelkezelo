using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ugyfelkezelo.Controls
{
    /// <summary>
    /// Interaction logic for MonthsDiagramPopup.xaml
    /// </summary>
    public partial class MonthsDiagramControl : UserControl
    {
        public MonthsDiagramControl()
        {
            InitializeComponent();
        }

        public Int32 SelectedYear { set { _SelectedYear = value; } get { return _SelectedYear; } }
        public Int32 SelectedMonthIndex { set { _SelectedMonthIndex = value; } get { return _SelectedMonthIndex; } }
        public String SelectedDateString
        {
            get
            {
                return String.Format("{0}. {1}. hónap ({2})", SelectedYear,
                    (Months.Items[SelectedMonthIndex] as MonthDescriptor).Index + 1,
                    (Months.Items[SelectedMonthIndex] as MonthDescriptor).Name);
            }
        }

        Int32 _SelectedMonthIndex = 0;
        Int32 _SelectedYear = 2001;

        private void Button_PreviousYearClicked(object sender, MouseButtonEventArgs e)
        {
            --_SelectedYear;
            yearTextBlock.Text = _SelectedYear.ToString();
            e.Handled = true;            
        }

        private void Button_NextYearClicked(object sender, MouseButtonEventArgs e)
        {
            ++_SelectedYear;
            yearTextBlock.Text = _SelectedYear.ToString();
            e.Handled = true;
        }


        private void Button_MonthClicked(object sender, MouseButtonEventArgs e)
        {
            Button monthButton = sender as Button;
            int monthIndex = (Int32)monthButton.CommandParameter;
            (Months.Items[_SelectedMonthIndex] as MonthDescriptor).Enabled = false;
            _SelectedMonthIndex = monthIndex;
            (Months.Items[_SelectedMonthIndex] as MonthDescriptor).Enabled = true;

            e.Handled = false; //kilépünk
        }
        
    }
}
