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
        MonthPickerModel _Model;
        public MonthPicker()
        {
            InitializeComponent();
            _Model = new MonthPickerModel();
            this.DataContext = _Model;
            StoredYear = StoredMonth = 0;
        }

        public String SelectedMonth { get; set; }

        public Int32 StoredYear { get; private set; }
        public Int32 StoredMonth { get; private set; }

        public bool IsMonthPicked { get { return StoredYear != 0 && StoredMonth != 0; } }

        
        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //hónapválasztó megnyitása
            MonthsDiagramWindow mdw = new MonthsDiagramWindow();
            mdw.DataContext = _Model;
            _Model.CloseWindow += (s, ev) => {
                StoredYear = _Model.SelectedYear;
                StoredMonth = _Model.SelectedMonthIndex;
                mdw.Close();             
            };
            Point parentAbsPos = this.PointToScreen(new Point(0.0, 0.0));
            mdw.Top = parentAbsPos.Y - mdw.Height;
            mdw.Left = parentAbsPos.X;
            mdw.Show();
        }

    }


    /*
                Ugyfelkezelo.UserControls.MonthsDiagramWindow mdw = new UserControls.MonthsDiagramWindow();
                mdw.DataContext = new Ugyfelkezelo.UserControls.MonthPickerModel();
                mdw.ShowDialog();
     */
}
