using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirebirdSql.Data.FirebirdClient;
using Ugyfelkezelo.Model;

namespace Ugyfelkezelo.ViewModel.Modules
{
    class InfoCentrumViewModel : ViewModelBase
    {

        public InfoCentrumViewModel()
        {
            Szamlatombok = new ObservableCollection<ICSzamlatomb>();
            UKUgyfelek = new ObservableCollection<UKUgyfel>();
            ICUgyfelek = new ObservableCollection<ICUgyfel>();

            ReleaseBindingCommand = new DelegateCommand(x => ExecuteReleaseBinding(x));
            BindCommand = new DelegateCommand(x => ExecuteBind());

            SaveCommand = new DelegateCommand(x => ExecuteSave());
            DiscardAndCloseCommand = new CommandWithEvent();
        }


        private void ExecuteSave()
        {
            //save invoicebooks
            foreach (var szt in Szamlatombok)
            {
                int hasznaltValue = szt.Used ? 1 : 0;
                int icid = szt.ICSzamlatombID;
                InfoCentrumSzamlatomb icsz = UKModel.InfoCentrumSzamlatombok.FirstOrDefault(c => c.ICSzamlatombID == icid);
                if (icsz != null)
                {
                    icsz.Hasznalt = hasznaltValue;
                }
            }

            //save bindingds
            foreach (var uf in UKUgyfelek)
            {
                if (uf.BoundICUgyfel != null)
                {
                    uf.Ugyfel.ICUgyfelID = uf.BoundICUgyfel.ICID;
                }                
            }
            UKModel.SaveChanges();

            DiscardAndCloseCommand.Execute(null);
        }



        private void ExecuteBind()
        {
            ICUgyfel icu = SelectedICUgyfel;
            UKUgyfel uku = SelectedUKUgyfel;

            if (icu == null || uku == null)
            {
                MessageBox.Show("Mindkét listában kell kiválasztás!", "Ügyfélkezelő");
                return;
            }

            uku.BoundICUgyfel = icu;
        }

        private void ExecuteReleaseBinding(object o)
        {
            if (o == null || !(o is UKUgyfel))
                return;
            UKUgyfel uf = o as UKUgyfel;
            uf.BoundICUgyfel = null;                       
        }

        ICUgyfel _SelectedICUgyfel;
        public ICUgyfel SelectedICUgyfel
        {
            get { return _SelectedICUgyfel; }
            set
            {
                _SelectedICUgyfel = value;
            }
        }

        UKUgyfel _SelectedUKUgyfel;
        public UKUgyfel SelectedUKUgyfel
        {
            get { return _SelectedUKUgyfel; }
            set
            {
                _SelectedUKUgyfel = value;
                if (_SelectedUKUgyfel.BoundICUgyfel != null)
                {
                    SelectedICUgyfel = _SelectedUKUgyfel.BoundICUgyfel;
                    OnPropertyChanged("SelectedICUgyfel");
                }
                
            }
        }

        public ObservableCollection<ICSzamlatomb> Szamlatombok { get; private set; }

        public void LoadData()
        {
            SynchronizeINVOICEBOOK();
            FillUgyfelek();
        }

        public ObservableCollection<UKUgyfel> UKUgyfelek { get; private set; }
        public ObservableCollection<ICUgyfel> ICUgyfelek { get; private set; }

        public DelegateCommand ReleaseBindingCommand { get; private set; }
        public DelegateCommand BindCommand { get; private set; }

        public DelegateCommand SaveCommand { get; private set; }
        public CommandWithEvent DiscardAndCloseCommand { get; private set; }

        private void FillUgyfelek()
        {
            //connect
            FirebirdDbReader fbdbr = Model.UKModel.ICSzamlaReader;
            if (fbdbr == null)
                return; //nincs beallitva

            //collect IC data
            List<ICUgyfel> icugyfelek = new List<ICUgyfel>();
            using (FbDataReader fdr = fbdbr.SelectReader("CUSTOMER"))
            {
                while (fdr.Read())
                {
                    if ((short)fdr["ACTIVEITEM"] != 1)
                        continue;

                    ICUgyfel uf = new ICUgyfel();
                    uf.ICID = (int)fdr["ID"];
                    uf.Name = (string)fdr["NAME"];
                    uf.Address = String.Format("{0} {1} {2}", fdr["ZIP"], fdr["CITY"], fdr["ADDRESS"]);
                    if (fdr["ACCOUNTNR"].GetType() == typeof(String))
                        uf.AccountNr = (string)fdr["ACCOUNTNR"];
                    icugyfelek.Add(uf);
                }
            }
            var icugyfelek2 = icugyfelek.OrderBy(icu => icu.Name);
            foreach (var icuf in icugyfelek2)
                ICUgyfelek.Add(icuf);


            //collect UK data
            List<UKUgyfel> ukugyfelek = new List<UKUgyfel>();
            foreach (var ugyfel in UKModel.Ugyfelek)
            {
                UKUgyfel uf = new UKUgyfel
                {
                    Name = ugyfel.Nev,
                    Address = ugyfel.Cim,
                    UKID = ugyfel.ID,
                    AccountNr = ugyfel.Bankszamlaszam,
                    Ugyfel = ugyfel
                };

                ukugyfelek.Add(uf);
            }

            var ukugyfelek2 = ukugyfelek.OrderBy(uku => uku.Name);
            foreach (var ukuf in ukugyfelek2)
                UKUgyfelek.Add(ukuf);

            //load bindings
            foreach(var uf in UKModel.Ugyfelek)
            {
                if (!uf.ICUgyfelID.HasValue)
                    continue;

                int ukid = uf.ID;
                int icid = uf.ICUgyfelID.Value;
                UKUgyfel uku = UKUgyfelek.FirstOrDefault(u => u.UKID == ukid);
                ICUgyfel icu = ICUgyfelek.FirstOrDefault(u => u.ICID == icid);
                if (uku != null && icu != null)
                {
                    uku.BoundICUgyfel = icu;
                }
            }
        }


        private void SynchronizeINVOICEBOOK()
        {

            //connect
            FirebirdDbReader fbdbr = Model.UKModel.ICSzamlaReader;
            if (fbdbr == null)
                return; //nincs beallitva

            //synchronize invoicebooks
            using (FbDataReader fdr = fbdbr.SelectReader("INVOICEBOOKS"))
            {
                List<int> usedICids = new List<int>();
                while (fdr.Read())
                {
                    if ((short)fdr["ISACTIVE"] != 1)
                        continue;
                    int icid = (int)fdr["ID"];
                    usedICids.Add(icid);
                    string icprefix = (string)fdr["PREFIX"];
                    string icpostfix = fdr["SUFFIX"].ToString();
                    string icname = (string)fdr["NAME"];

                    InfoCentrumSzamlatomb icsz = UKModel.InfoCentrumSzamlatombok.FirstOrDefault(ic => ic.ICSzamlatombID == icid);
                    if (icsz == null)
                    {
                        icsz = new InfoCentrumSzamlatomb();
                        icsz.ICSzamlatombID = icid;
                        icsz.Hasznalt = 0;
                        UKModel.InfoCentrumSzamlatombok.Add(icsz);
                    }
                    icsz.Nev = icname;
                    icsz.Elotag = icprefix;
                    icsz.Utotag = icpostfix;
                    UKModel.SaveChanges();
                }

                foreach (InfoCentrumSzamlatomb icsz in UKModel.InfoCentrumSzamlatombok)
                {
                    if (!usedICids.Contains(icsz.ICSzamlatombID))
                    {
                        UKModel.InfoCentrumSzamlatombok.Remove(icsz);
                    }
                }
                UKModel.SaveChanges();
            }

            foreach (var icsz in UKModel.InfoCentrumSzamlatombok)
            {
                Szamlatombok.Add(new ICSzamlatomb { Name = icsz.Nev, Used = (icsz.Hasznalt == 1), ICSzamlatombID = icsz.ICSzamlatombID });
            }

        }

    }

    public class ICSzamlatomb : ViewModelBase
    {
        public Int32 ICSzamlatombID { get; set; }
        public String Name { get; set; }
        public Boolean Used { get; set; }
    }

    public class ICUgyfel : ViewModelBase
    {
        public Int32 ICID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String AccountNr { get; set; }
    }

    public class UKUgyfel : ViewModelBase
    {
        public Ugyfel Ugyfel { get; set; }
        public Int32 UKID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String AccountNr { get; set; }
        public ICUgyfel BoundICUgyfel { get { return _BoundICUgyfel; } set { _BoundICUgyfel = value; OnPropertyChanged("BoundICUgyfel"); } }
        private ICUgyfel _BoundICUgyfel;
    }
}
