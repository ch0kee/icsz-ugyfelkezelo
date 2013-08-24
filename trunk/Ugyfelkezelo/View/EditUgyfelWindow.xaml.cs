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

namespace Ugyfelkezelo.View
{
    /// <summary>
    /// Interaction logic for EditUgyfelWindow.xaml
    /// </summary>
    public partial class EditUgyfelWindow : Window
    {
        public EditUgyfelWindow()
        {
            InitializeComponent();
            SuppressTextChangedEvent = false;
        }

        public List<String> UgyfelNevAutoCompleteList
        {
            set;
            private get;
        }

        //auto complete
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SuppressTextChangedEvent)
            {
                e.Handled = true;
                return;
            }

            if ((sender == null) || !(sender is TextBox) || UgyfelNevAutoCompleteList == null)
                return;
            TextBox tb = sender as TextBox;
            string str = tb.Text;

            TextChange f = e.Changes.First();
            if (f.RemovedLength > 0 && f.AddedLength == 0)
            {
                e.Handled = true;
                return;                    
            }

            var possibleChoices = UgyfelNevAutoCompleteList.Where(u => u.ToLower().StartsWith(str.ToLower()));
            if (possibleChoices.Count() == 0)
                return; //nincs egyezes, kiraly
            string firstMatch = possibleChoices.First();
            string postfix = firstMatch.Substring(str.Length);
            SuppressTextChangedEvent = true;
            tb.Text += postfix;
            SuppressTextChangedEvent = false;
            tb.Select(str.Length, postfix.Length);
            e.Handled = true;
        }

        private bool SuppressTextChangedEvent { get; set; }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender == null) || !(sender is TextBox))
                return;
            TextBox tb = sender as TextBox;
            if (tb.SelectionLength > 0)
            {
                string str = tb.Text;
                SuppressTextChangedEvent = true;
                tb.Text = str.Remove(tb.SelectionStart, tb.SelectionLength);
                SuppressTextChangedEvent = false;
            }            
        }


    }
}
