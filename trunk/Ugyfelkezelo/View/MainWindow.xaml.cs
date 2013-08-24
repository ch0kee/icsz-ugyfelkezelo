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
using Ugyfelkezelo.ViewModel;

namespace Ugyfelkezelo.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ugyfelekDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedItems(ViewModel.UgyfelkezeloViewModel.Instance.UgyfelViewModel, ugyfelekDataGrid);
        }

        private void lepcsohazakDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedItems(ViewModel.UgyfelkezeloViewModel.Instance.LepcsohazViewModel, lepcsohazakDataGrid);
        }

        private void konstrukciokDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedItems(ViewModel.UgyfelkezeloViewModel.Instance.KonstrukcioViewModel, konstrukciokDataGrid);
        }

        private void elofizetesekDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedItems(ViewModel.UgyfelkezeloViewModel.Instance.ElofizetesViewModel, elofizetesekDataGrid);
        }

        private void UpdateSelectedItems<TItem>(EntityViewModel<TItem> viewmodel, DataGrid grid)
            where TItem : class, new()
        {
            System.Collections.IList selectedObjs = grid.SelectedItems;
            viewmodel.SelectedItems = selectedObjs.Cast<TItem>().ToList();
        }
    }
}
