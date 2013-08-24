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
using Ugyfelkezelo.ViewModel;

namespace Ugyfelkezelo.Controls
{
    /// <summary>
    /// Interaction logic for PathPicker.xaml
    /// </summary>
    public partial class PathPicker : UserControl
    {
        //PathPickerViewModel _ViewModel = new PathPickerViewModel();
        public PathPicker()
        {
            InitializeComponent();
            DefaultExtension = "*";
            FilterString = "All files";
            //DataContext = _ViewModel;
        }

        /*
        public int MyProperty
        {
            get { return (int)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(int), typeof(ownerclass), new PropertyMetadata(0));
        */
        
        public String DefaultExtension { get; set; }
        public String FilterString { get; set; }

        public static readonly DependencyProperty PickedPathProperty = DependencyProperty.Register(
            "PickedPath",
            typeof(String),
            typeof(PathPicker));

        public String GetPickedPath(FrameworkElement fe)
        {
            return (String)fe.GetValue(PickedPathProperty);
        }

        public void SetPickedPath(FrameworkElement fe, String val)
        {
            fe.SetValue(PickedPathProperty, val);
        }
        
        public String PickedPath
        {
            get
            {
                return GetPickedPath(this);
            }
            set
            {
                SetPickedPath(this, value);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();          
 
            // Set filter for file extension and default file extension
            dlg.DefaultExt = "."+DefaultExtension;
            dlg.Filter = String.Format("{0} ({1})|*.{1}", FilterString, DefaultExtension);
 
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
 
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                //string oldpath = Path;
                string filename = dlg.FileName;
                PickedPath = filename;
                //OnPropertyChanged( new DependencyPropertyChangedEventArgs(PathProperty, oldpath,filename));
                //_ViewModel.Path = filename;
            }
        }
    }
    /*
    class PathPickerViewModel : ViewModelBase
    {
        private String _Path = "";
        public string Path { get { return _Path; }
            set
            {
                _Path = value;
                OnPropertyChanged("Path");
            }
        }
    }*/
}
