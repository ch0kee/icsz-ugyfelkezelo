﻿using System;
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

namespace Ugyfelkezelo.Controls
{
    /// <summary>
    /// Interaction logic for MonthsDiagramWindow.xaml
    /// </summary>
    public partial class MonthsDiagramWindow : Window
    {
        public MonthsDiagramWindow()
        {
            InitializeComponent();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!_isclosing)
                Close();
        }

        bool _isclosing = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isclosing = true;
        }
    }
}
