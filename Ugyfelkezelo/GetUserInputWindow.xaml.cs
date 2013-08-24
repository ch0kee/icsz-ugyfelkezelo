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
using System.Windows.Shapes;

namespace Ugyfelkezelo
{
    /// <summary>
    /// Interaction logic for GetUserInputWindow.xaml
    /// </summary>
    public partial class GetUserInputWindow : Window
    {
        readonly Control _InputControl;
        readonly String _Text;
        public GetUserInputWindow(Control inputCtrl, DependencyProperty bindingProperty, string text)
        {
            InitializeComponent();
            
            _InputControl = inputCtrl;
            MainGrid.Children.Add(_InputControl);
            _InputControl.SetValue(Grid.RowProperty, 1);

            //Binding b = new Binding("Value");

            _InputControl.SetBinding(bindingProperty, "Value");
            
            
            _Text = text;
            HeaderText.Text = _Text;
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
