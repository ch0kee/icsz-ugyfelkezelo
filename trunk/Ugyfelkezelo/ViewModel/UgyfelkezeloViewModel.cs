using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Ugyfelkezelo.Model;
using System.Windows;
using System.Data.Objects;
using Ugyfelkezelo.ViewModel.Modules;
using System.Windows.Controls;
using System.Diagnostics;
using FirebirdSql.Data.FirebirdClient;

namespace Ugyfelkezelo.ViewModel
{
    public class UgyfelkezeloViewModel : ViewModelBase, IDisposable
    {
        List<IModule> _Modules = new List<IModule>();

        public DelegateCommand DeleteKeyCommand { get; private set; }

        public String StatusBarText { get; private set; }

        public DelegateCommand ElofizetesKezdodatumCommand { get; private set; }
        
        public DelegateCommand AutoBindContracts { get; private set; }

        public DelegateCommand OpenHelpWindow { get; private set; }

        public CommandWithEvent InfoCentrumSettingsCommand { get; private set; }

        public class OpenHelpWindowEventArgs : EventArgs
        {
            public string Url { get; set; }
        }
        public event EventHandler<OpenHelpWindowEventArgs> OpenHelpWindowEvent;

        public event EventHandler CloseEditWindowEvent {
            add {
                foreach (IModule m in _Modules)
                    if (m is IHasWindow)
                        (m as IHasWindow).CloseEditWindowEvent += value;                   
            }
            remove {
                foreach (IModule m in _Modules)
                    if (m is IHasWindow)
                        (m as IHasWindow).CloseEditWindowEvent -= value;
            }
        }

        public event EventHandler OpenEditWindowEvent {
            add {
                foreach (IModule m in _Modules)
                    if (m is IHasWindow)
                        (m as IHasWindow).OpenEditWindowEvent += value;
            }
            remove {
                foreach (IModule m in _Modules)
                    if (m is IHasWindow)
                        (m as IHasWindow).OpenEditWindowEvent -= value;
            }
        }

        public Int32 ProgressBarValue { get; private set; }

        public static UgyfelkezeloViewModel Instance { get; private set; }

        public void RefreshModel()
        {
            foreach (IModule m in _Modules)
                m.ForceRefreshAll();
        }
        
        //public Beallitasok Beallitasok { get; private set; }

        public LepcsohazViewModel LepcsohazViewModel { get; private set; }
        public UgyfelViewModel UgyfelViewModel { get; private set; }
        public KonstrukcioViewModel KonstrukcioViewModel { get; private set; }
        public ElofizetesViewModel ElofizetesViewModel { get; private set; }
        public CsoportosBeszedesiMegbizasViewModel CsoportosBeszedesiMegbizasViewModel { get; private set; }
        public UgyfelkezeloViewModel()
        {
            Instance = this;
            try
            {
                StatusBarText = "Adatbázis betöltése...";

                LepcsohazViewModel = new LepcsohazViewModel(UKModel.Lepcsohazak,/*_Entities,*/ UgyfelViewModel);
                UgyfelViewModel = new UgyfelViewModel(UKModel.Ugyfelek,/*_Entities,*/ LepcsohazViewModel);
                KonstrukcioViewModel = new KonstrukcioViewModel(UKModel.Konstrukciok/*_Entities*/);
                ElofizetesViewModel = new ElofizetesViewModel(UKModel.Elofizetesek/*_Entities*/);
                CsoportosBeszedesiMegbizasViewModel = new Modules.CsoportosBeszedesiMegbizasViewModel();

                _Modules.Add(LepcsohazViewModel);
                _Modules.Add(UgyfelViewModel);
                _Modules.Add(KonstrukcioViewModel);
                _Modules.Add(ElofizetesViewModel);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                App.Current.Shutdown();
                return;
            }
            StatusBarText = String.Format("Programverzió: {0}", Deployment.CurrentVersion);


            AutoBindContracts = new DelegateCommand(u => AutoBindContractsExecuted());
            OpenHelpWindow = new DelegateCommand(o => OpenHelpWindowExecuted(o));
            DeleteKeyCommand = new DelegateCommand(d => DeleteKeyExecuted());

            SettingsCommand = new DelegateCommand(x => SettingsExecuted());
            OpenCSBESZEDWindow = new DelegateCommand(x => OpenCSBESZEDWindowExecuted());

            InfoCentrumSettingsCommand = new CommandWithEvent();

            ElofizetesKezdodatumCommand = new DelegateCommand(x => ElofizetesKezdodatumExecuted());
        }


        private void AutoBindContractsExecuted()
        {
            //
            Dictionary<Byte, Konstrukcio> konstrukciok = new Dictionary<byte, Konstrukcio>();
            //konstrukciók kigyűjtése
            foreach (Konstrukcio k in UgyfelkezeloViewModel.Instance.KonstrukcioViewModel.Items)
            {
                if (konstrukciok.ContainsKey(k.Kod))
                {
                    if (k.Ar > konstrukciok[k.Kod].Ar)
                        konstrukciok[k.Kod] = k;
                }
                else
                    konstrukciok.Add(k.Kod, k);
            }

            foreach (Ugyfel u in UgyfelViewModel.SelectedItems)
            {

                //előfizetések indítása
                foreach (var k in konstrukciok)
                {
                    if (u.KonstrukciotIgenyelt(k.Key))
                    {
                        Elofizetes e = new Elofizetes();
                        e.Ugyfel = u;
                        e.Konstrukcio = k.Value;

                        e.Mettol = null;
                        e.Meddig = null;
                        UKModel.Elofizetesek.Add(e);
                    }
                }
            }
            UKModel.SaveChanges();
            RefreshModel();
        }

        private void ElofizetesKezdodatumExecuted()
        {
            DatePicker datePicker = new DatePicker();
            datePicker.Width = 100;
            datePicker.Height = 30;
            var getter = new GetUserInput<DateTime>(datePicker, DatePicker.SelectedDateProperty, DateTime.Now,
                "Add meg az előfizetések új kezdődátumát!");
            bool ret = getter.Show();
            if (!ret)
                return;
            //
            foreach (Elofizetes e in ElofizetesViewModel.SelectedItems)
            {
                e.Mettol = getter.Value;
            }
            UKModel.SaveChanges();
            RefreshModel();                
        }

        public void SyncInfocentrumBefizetesek()
        {
            FirebirdDbReader reader = UKModel.ICSzamlaReader;
            if (reader == null)
            {
                Debug.Assert(false, "nincs beállítva az infocentrum adatbázis");
                return;
            }

            if (0 == UKModel.InfoCentrumSzamlatombok.Count( icsz => icsz.Hasznalt.HasValue && icsz.Hasznalt.Value == 1))
            {
                Debug.Assert(false, "nincs használt számlatömb");
                return;
            }


            string invoiceBookIds = "";
            foreach(InfoCentrumSzamlatomb szt in UKModel.InfoCentrumSzamlatombok.Where(icsz => icsz.Hasznalt == 1))
            {
                invoiceBookIds += (invoiceBookIds.Length == 0 ? "" : ",")
                    + szt.ICSzamlatombID;
            }
            invoiceBookIds = "("+invoiceBookIds+")";

            foreach (Ugyfel u in UKModel.Ugyfelek)
            {
                u.Befizetve = 0;
                if (u.ICUgyfelID.HasValue)
                {
                    string selectSzlakif = "SELECT ID FROM SZLAKIF WHERE " +
                           "VEVOAZO=" + u.ICUgyfelID.Value
                         + " AND "
                         + "INVOICEBOOKID IN " + invoiceBookIds
                         + " AND "
                         + "ACTIVE_INVOICE=1"
                         + " AND "
                         + "FIZETVE=1"
                         ;
                    string selectSzlakit = "SELECT SZAMLAAZO, EGYSEGAR, MENNYISEG, AFA FROM SZLAKIT";
                    /*string join = "SELECT EGYSEGAR, AFA FROM (" + selectSzlakif
                        + ") INNER JOIN (" + selectSzlakit
                        + ") ON SZLAKIF.ID=SZLAKIT.SZAMLAAZO";*/



                    using (FbDataReader rszkf = reader.ExecuteCommand(selectSzlakif))
                    {
                        while (rszkf.Read())
                        {
                            int szamlaazo = (int)rszkf["ID"];

                            using (FbDataReader rszkt = reader.ExecuteCommand(selectSzlakit
                                + " WHERE SZAMLAAZO=" + szamlaazo))
                            {
                                while (rszkt.Read())
                                {
                                    double afa = (double)(decimal)rszkt["AFA"];
                                    double mennyiseg = (double)rszkt["MENNYISEG"];
                                    double egysegar = (double)rszkt["EGYSEGAR"];

                                    u.Befizetve += (int)Math.Round(
                                        (1.0 + (afa / 100.0)) * (mennyiseg * egysegar));
                                }
                            }
                        }
                    }

                }
            }
            UKModel.SaveChanges();
        }

        private void SettingsExecuted()
        {
            FireEvent(OpenSettingsWindow);
        }

        public DelegateCommand SaveSettingsCommand { get; private set; }
        public event EventHandler OpenSettingsWindow;

        public DelegateCommand SettingsCommand { get; private set; }

        public event EventHandler OpenCSBESZEDWindowEvent;

        private void OpenCSBESZEDWindowExecuted()
        {
            FireEvent(OpenCSBESZEDWindowEvent);
        }

        public DelegateCommand OpenCSBESZEDWindow { get; private set; }

        TabItem _CurrentTab;
        public TabItem CurrentTab { get { return _CurrentTab; }
            set
            {
                _CurrentTab = value;
            }
        }

        IEntityViewModelWithDelete StringToViewModel(string str)
        {
            IEntityViewModelWithDelete viewModel = null;
            if (str == "Ügyfelek")
            {
                viewModel = UgyfelViewModel;
            }
            else if (str == "Előfizetések")
            {
                viewModel = ElofizetesViewModel;
            }
            else if (str == "Lépcsőházak")
            {
                viewModel = LepcsohazViewModel;
            }
            else if (str == "Konstrukciók")
            {
                viewModel = KonstrukcioViewModel;
            }
            return viewModel;
        }

        private void DeleteKeyExecuted()
        {
            String header = CurrentTab.Header.ToString();
            //aktualis tab
            MessageBoxResult res  = MessageBox.Show("Kérjek megerősítést külön minden sor törlése előtt ?", "Ügyfélkezelő"
                , MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel);
            bool askBeforeEveryRow = true;
            switch (res)
            {
                case MessageBoxResult.Yes:
                    break;
                case MessageBoxResult.No:
                    askBeforeEveryRow = false;
                    break;
                default:
                    return;
            }

            IEntityViewModelWithDelete deleteInViewModel = StringToViewModel(header);
            if (deleteInViewModel == null)
            {
                MessageBox.Show("Hiba! Kérjük szépen jelenteni!", "Ügyfélkezelő", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
                return;
            }
            deleteInViewModel.DeleteSelectedItems(askBeforeEveryRow);

        }



        private void OpenHelpWindowExecuted(object o)
        {
            if (o == null || !(o is String))
                return;
            String url = o as String;
            if (OpenHelpWindowEvent != null)
            OpenHelpWindowEvent(this, new OpenHelpWindowEventArgs { Url = url });
        }



        private void FireEvent(EventHandler e)
        {
            FireEvent(e, EventArgs.Empty);
        }

        private void FireEvent(EventHandler e, EventArgs args)
        {
            if (e != null)
                e(this, EventArgs.Empty);
        }

        private bool ExecuteSafeDatabaseCommand(Action a)
        {
            try
            {
                a();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Adatbázis hiba\n"+e.InnerException.ToString(), "Ügyfélkezelő", MessageBoxButton.OK, MessageBoxImage.Error);
                throw e;
                //return false;
            }
        }


        public void Dispose()
        {
            foreach (IModule m in _Modules)
            {
                if (m is IDisposable)
                {
                    IDisposable id = m as IDisposable;
                    id.Dispose();
                }
            }
        }
    }


}


