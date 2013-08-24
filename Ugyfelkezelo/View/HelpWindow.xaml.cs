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
using System.Windows.Shapes;
using System.IO;

namespace Ugyfelkezelo.View
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        public void Navigate(String url)
        {
            url = System.IO.Path.Combine(Deployment.ApplicationPath, url);
            Uri uri = new Uri(url);            
            //webBrowser.Source = uri; 
            webBrowser.Navigate(uri);
        }
    }
}
