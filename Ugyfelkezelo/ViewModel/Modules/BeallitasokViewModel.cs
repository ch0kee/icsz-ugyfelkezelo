using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ugyfelkezelo.Model;

namespace Ugyfelkezelo.ViewModel.Modules
{
    public class BeallitasokViewModel : ViewModelBase
    {
        public BeallitasokViewModel()
        {
            SaveCommand = new DelegateCommand(x => SaveSettingsExecuted());
        }

        public string Cegnev { get { return UKModel.Beallitasok.Cegnev; } set { UKModel.Beallitasok.Cegnev = value; } }
        public string Adoazonosito { get { return UKModel.Beallitasok.Adoazonosito; } set { UKModel.Beallitasok.Adoazonosito = value; } }
        public string Bankszamlaszam { get { return UKModel.Beallitasok.Bankszamlaszam; } set { UKModel.Beallitasok.Bankszamlaszam = value; } }
        public string IcSzamlaUtvonal { get { return UKModel.Beallitasok.IcSzamlaUtvonal; } set { UKModel.Beallitasok.IcSzamlaUtvonal = value; } }

        public DelegateCommand SaveCommand { get; private set; }

        private void SaveSettingsExecuted()
        {
            UKModel.SaveChanges();
        }
    }
}
