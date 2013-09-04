using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for MonthPicker.xaml
    /// </summary>
    public partial class MonthPicker : UserControl
    {

        //public MonthPickerModel _Model;

        public MonthPicker()
        {
            InitializeComponent();
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month-1;
        }

        public DateTime Date { get { return new DateTime(Year, Month + 1, 1); } set { Year = value.Year; Month = value.Month - 1; } }

        public Int32 Year { get { return _MonthsDiagramControl.SelectedYear; } set { _MonthsDiagramControl.SelectedYear = value; SetSelectedDateString();  } }
        public Int32 Month { get { return _MonthsDiagramControl.SelectedMonthIndex; } set { _MonthsDiagramControl.SelectedMonthIndex = value; SetSelectedDateString(); } }

        //public bool IsMonthPicked { get { return Year != 0 && Month != 0; } }
        
        public void SetSelectedDateString()
        {
            _SelectedDateTextBox.Text = _MonthsDiagramControl.SelectedDateString;
        }

        public void Clear()
        {
            _SelectedDateTextBox.Text = "";
        }
        

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MonthsDiagramPopup.IsOpen = true;
            e.Handled = false;
         //   MonthsDiagramPopup.Focus();
            /*
            //hónapválasztó megnyitása
            MonthsDiagramPopup mdw = new MonthsDiagramWindow();
            mdw.DataContext = _Model;

            Point parentAbsPos = this.PointToScreen(new Point(0.0, 0.0));
            mdw.Top = parentAbsPos.Y - mdw.Height;
            mdw.Left = parentAbsPos.X;
            mdw.Show();*/
        }

        private void TextBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Year = _MonthsDiagramControl.SelectedYear;
            SetSelectedDateString();
            //Month = _Model.SelectedMonthIndex;
            MonthsDiagramPopup.IsOpen = false;
        }





        public bool IsReadOnly { get { return _SelectedDateTextBox.IsReadOnly; } set { _SelectedDateTextBox.IsReadOnly = value; } }
    }


    /*
                Ugyfelkezelo.UserControls.MonthsDiagramWindow mdw = new UserControls.MonthsDiagramWindow();
                mdw.DataContext = new Ugyfelkezelo.UserControls.MonthPickerModel();
                mdw.ShowDialog();
     */
}
