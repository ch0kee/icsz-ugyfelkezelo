using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Ugyfelkezelo.Model;
using Ugyfelkezelo.ViewModel;
using Ugyfelkezelo.ViewModel.Modules;

namespace Ugyfelkezelo.View
{
    /// <summary>
    /// Interaction logic for ManageElofizetesekWindow.xaml
    /// </summary>
    public partial class ManageElofizetesekWindow : Window
    {
        MergedElofizetesekViewModel _MergedElofizetesekViewModel;
        public ManageElofizetesekWindow()
        {
            InitializeComponent();
            DataContext = _MergedElofizetesekViewModel = new MergedElofizetesekViewModel();
            //ugyfelekDataGrid.DataContext = UgyfelkezeloViewModel.Instance;
            //UgyfelViewModel = UgyfelkezeloViewModel.Instance.UgyfelViewModel;
            EnableElofizetesEditing(false);
        }

        //c ügyfelek
        private void _UgyfelekDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayUgyfel(SelectedUgyfel);
        }

        private void DisplayUgyfel(Ugyfel ugyfel)
        {
            _ElofizetesekDataGrid.Items.Clear();

            if (ugyfel != null)
            {
                foreach (Elofizetes elof in ugyfel.Elofizetes)
                    _ElofizetesekDataGrid.Items.Add(elof);
            }
            ClearFields();
            EnableElofizetesEditing(false);
        }


        //c
        private void _HatarozottLastMonthCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _LastMonthPicker.IsEnabled = true;
        }

        //c
        private void _HatarozottLastMonthCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _LastMonthPicker.IsEnabled = false;
        }

        private void _KonstrukcioDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Konstrukcio k = _KonstrukcioDataGrid.SelectedItem as Konstrukcio;
            if (k != null)
            {

            }
        }


        //c
        private void _ElofizetesekDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayElofizetes(SelectedElofizetes, false);
        }

        //c
        private void DisplayElofizetes(Elofizetes elofizetes, bool enableEditing)
        {
            if (elofizetes != null)
            {
                _KonstrukcioDataGrid.SelectedItem = elofizetes.Konstrukcio;
                // EnableElofizetesEditing(true);
                _FirstMonthPicker.Date = elofizetes.KezdoDatum;
                bool hatarozottIdejuElofizetes = elofizetes.Meddig.HasValue;
                _HatarozottLastMonthCheckBox.IsChecked = hatarozottIdejuElofizetes;
                _LastMonthPicker.Date = elofizetes.ZaroDatum;
                EnableElofizetesEditing(enableEditing);
            }
        }

        private void ClearFields()
        {
            _KonstrukcioDataGrid.SelectedItem = null;
            _FirstMonthPicker.Clear();
            _LastMonthPicker.Clear();
            _HatarozottLastMonthCheckBox.IsChecked = null;
        }

        private void EnableElofizetesEditing(bool enable)
        {
            _KonstrukcioDataGrid.IsEnabled = enable;
            _FirstMonthPicker.IsEnabled = enable;
            _HatarozottLastMonthCheckBox.IsEnabled = enable;
            _LastMonthPicker.IsEnabled = enable && _HatarozottLastMonthCheckBox.IsChecked.HasValue && _HatarozottLastMonthCheckBox.IsChecked.Value;
            _ElofizetesMenteseButton.IsEnabled = enable;
            _ModositasokElvetese.IsEnabled = enable;
        }

        private void _ElofizetesModositasaButton_Click(object sender, RoutedEventArgs e)
        {
            _EditedElofizetes = SelectedElofizetes;
            DisplayElofizetes(_EditedElofizetes, true);
            StartEditingElofizetes();
        }


        private Elofizetes _EditedElofizetes = null;

        private void _UjElofizetesButton_Click(object sender, RoutedEventArgs e)
        {
            _EditedElofizetes = new Elofizetes();
            _EditedElofizetes.Ugyfel = SelectedUgyfel;
            _EditedElofizetes.Konstrukcio = null;

            if (SelectedElofizetes != null)
            { //c egy meglévőt klónozunk
                _EditedElofizetes.Mettol = SelectedElofizetes.KezdoDatum; //c legalabb az ujak menjenek ezen
                _EditedElofizetes.Meddig = SelectedElofizetes.Meddig;
            }
            else
            {
                _EditedElofizetes.Mettol = SelectedUgyfel.SzolgaltatasKezdoIdopontja;
                _EditedElofizetes.Meddig = null;
            }


            DisplayElofizetes(_EditedElofizetes, true);

            StartEditingElofizetes();
        }


        private void StartEditingElofizetes()
        {
            _UgyfelekDataGrid.IsEnabled = false;
            //c ez letiltja az új előfizetés és előfizetés és módosítás gombokat is
            _ElofizetesekDataGrid.SelectedItem = null;
            _ElofizetesekDataGrid.IsEnabled = false;
            //c

        }

        private void EndEditingElofizetes()
        {
            _UgyfelekDataGrid.IsEnabled = true;
            _ElofizetesekDataGrid.IsEnabled = true;
            _EditedElofizetes = null;
        }

        private void _ElofizetesTorleseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Biztos vagy benne?", "Előfizetés törlése", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                UKModel.Elofizetesek.Remove(SelectedElofizetes);
                DisplayUgyfel(SelectedUgyfel);
            }
        }

        private void _ElofizetesMenteseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Mented a módosításokat?", "Módosítások mentése", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (SelectedKonstrukcio == null)
                {
                    MessageBox.Show("Nem adtál meg konstrukciót!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                _EditedElofizetes.Konstrukcio = SelectedKonstrukcio;
                _EditedElofizetes.Ugyfel = SelectedUgyfel;
                _EditedElofizetes.Mettol = _FirstMonthPicker.Date;
                if (_HatarozottLastMonthCheckBox.IsChecked.HasValue && _HatarozottLastMonthCheckBox.IsChecked.Value)
                {
                    _EditedElofizetes.Meddig = _LastMonthPicker.Date;
                }
                else
                {
                    _EditedElofizetes.Meddig = null;

                }

                if (_EditedElofizetes.ID == 0)
                {
                    //új elem
                    UKModel.Elofizetesek.Add(_EditedElofizetes);
                }
            }
            
            EndEditingElofizetes();
            DisplayUgyfel(SelectedUgyfel);
        }


        private Elofizetes SelectedElofizetes { get { return _ElofizetesekDataGrid.SelectedItem as Elofizetes; } }
        private Ugyfel SelectedUgyfel { get { return _UgyfelekDataGrid.SelectedItem as Ugyfel; } }
        private Konstrukcio SelectedKonstrukcio { get { return _KonstrukcioDataGrid.SelectedItem as Konstrukcio; } }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UKModel.SaveChanges();
            UgyfelkezeloViewModel.Instance.RefreshModel();
        }

        private void _BezarasButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _ModositasokElvetese_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Elveted a módosításokat?", "Módosítások elvetése", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                EndEditingElofizetes();
                DisplayUgyfel(SelectedUgyfel);
            }
        }

    }

    public class MergedElofizetesekViewModel : ViewModelBase
    {
        bool _HasKTV1 = false;
        bool _HasKTV2 = false;
        bool _HasHBO1EV = false;
        bool _HasHBO = false;
        int _ArKTV1 = -1;
        int _ArKTV2 = -1;
        int _ArHBO1EV = -1;
        int _ArHBO = -1;
        public bool HasKTV1 { get { return _HasKTV1; } set  { }  }
        public bool HasKTV2 { get { return _HasKTV2; } set { } }
        public bool HasHBO1EV { get { return _HasHBO1EV; } set { } }
        public bool HasHBO { get { return _HasHBO; } set { } }

        public string ArKTV1 { get { return _ArKTV1.ToString(); } set { } }
        public string ArKTV2 { get { return _ArKTV2.ToString(); } set { } }
        public string ArHBO1EV { get { return _ArHBO1EV.ToString(); } set { } }
        public string ArHBO { get { return _ArHBO.ToString(); } set { } }

        public Int32 StartYear { get { return 2002; } set { int i = value; } }
        public Int32 StartMonth { get { return 11; } set {} }

        List<Ugyfel> _Ugyfelek = new List<Ugyfel>();
        public MergedElofizetesekViewModel()
        {
        }

        public void AddUgyfel(Ugyfel u)
        {
            _HasKTV1 = u.Elofizetes.Any( e => e.Konstrukcio.IntKod == 1);
            _HasKTV2 = u.Elofizetes.Any(e => e.Konstrukcio.IntKod == 2);
            _HasHBO1EV = u.Elofizetes.Any(e => e.Konstrukcio.IntKod == 4);
            _HasHBO = u.Elofizetes.Any(e => e.Konstrukcio.IntKod == 8);
            //
            if (_HasKTV1)
                _ArKTV1 = u.Elofizetes.First(e => e.Konstrukcio.IntKod == 1).Konstrukcio.Ar;
            if (_HasKTV2)
                _ArKTV2 = u.Elofizetes.First(e => e.Konstrukcio.IntKod == 2).Konstrukcio.Ar;
            if (_HasHBO1EV)
                _ArHBO1EV = u.Elofizetes.First(e => e.Konstrukcio.IntKod == 4).Konstrukcio.Ar;
            if (_HasHBO)
                _ArHBO = u.Elofizetes.First(e => e.Konstrukcio.IntKod == 8).Konstrukcio.Ar;

            OnPropertyChanged("HasKTV1");
            OnPropertyChanged("HasKTV2");
            OnPropertyChanged("HasHBO1EV");
            OnPropertyChanged("HasHBO");
            OnPropertyChanged("ArKTV1");
            OnPropertyChanged("ArKTV2");
            OnPropertyChanged("ArHBO1EV");
            OnPropertyChanged("ArHBO");
        }

        public void Clear()
        {
            _Ugyfelek.Clear();
        }

    }
}
