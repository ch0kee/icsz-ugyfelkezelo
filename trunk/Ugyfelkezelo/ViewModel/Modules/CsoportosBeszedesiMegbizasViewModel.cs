using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Ugyfelkezelo.Model;
using System.Windows;

namespace Ugyfelkezelo.ViewModel.Modules
{
    public class CSBESZEDUgyfel : ViewModelBase
    {
        public Int32 Ar { get; set; }
        //public string Nev { get; set; }
        public Ugyfel Ugyfel { get; set; }
        public string Kod { get; set; } //csoportos kod
        public string Csomagok { get; set; }
    }

    public class CSBESZEDMegbizas : ViewModelBase
    {
        public string Fejlec { get; set; }
        public ObservableCollection<CSBESZEDUgyfel> Ugyfelek { get; set; }
        public String RawData { get; set; }
    }

    public class CsoportosBeszedesiMegbizasViewModel : ViewModelBase
    {
        CsoportosBeszedes _CsoportosBeszedes;

        public void LoadData()
        {
            _CsoportosBeszedes.CollectData(UgyfelkezeloViewModel.Instance.UgyfelViewModel.Items, Megbizasok); 
        }


        public CsoportosBeszedesiMegbizasViewModel()
        {
            Megbizasok = new ObservableCollection<CSBESZEDMegbizas>();
            _CsoportosBeszedes = new CsoportosBeszedes();
        }

        public ObservableCollection<CSBESZEDMegbizas> Megbizasok { get; private set; }







    }

}
