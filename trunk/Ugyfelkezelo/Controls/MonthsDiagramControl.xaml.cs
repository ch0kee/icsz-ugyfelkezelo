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

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //c hogy ne veszitsük el a focust évváltáskor
            if (sender is Button && (sender as Button).Command != null)
            {
                (sender as Button).Command.Execute(null);
                e.Handled = true;
            }
        }
    }
}
