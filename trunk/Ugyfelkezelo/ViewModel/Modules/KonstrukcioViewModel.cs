using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ugyfelkezelo.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data.Objects;
using System.Data.Entity;

namespace Ugyfelkezelo.ViewModel.Modules
{
    public class KonstrukcioViewModel : EntityViewModelWithEditWindow<Konstrukcio, View.EditKonstrukcioWindow>
    {
        public KonstrukcioViewModel(DbSet<Konstrukcio> konstrukciok)
            : base(/*entities, */konstrukciok)
        {
            List<KonstrukcioKod> kodok = new List<KonstrukcioKod>();
            kodok.Add(new KonstrukcioKod("Basic", 1));
            kodok.Add(new KonstrukcioKod("EBS", 2));
            kodok.Add(new KonstrukcioKod("HBO 1 éves", 4));
            kodok.Add(new KonstrukcioKod("HBO hűség nélkül", 8));

            Kodok = new ObservableCollection<KonstrukcioKod>(kodok);

        }

        public ObservableCollection<KonstrukcioKod> Kodok { get; private set; }        



        protected override FailureVerifier AttemptToDeleteItem(Model.Konstrukcio i)
        {
            bool cant_be_deleted = SuppressConfirmDeleteQuestion ? false :
            MessageBox.Show(String.Format("Biztosan törölni akarod a következő konstrukciót ?\n{0}", i.Nev), "Ügyfélkezelő", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.No;
            FailureVerifier fv = new FailureVerifier(cant_be_deleted);
            fv.AddFailureCondition(i.Elofizetes.Count > 0,"A konstrukció nem törölhető, mert előfizetésekhez tartozik!");
            return fv;
        }

        protected override FailureVerifier AttemptToSaveItem(Model.Konstrukcio i)
        {
            FailureVerifier fv = new FailureVerifier();
            fv.AddFailureCondition(String.IsNullOrEmpty(i.Nev),"Érvénytelen név");
            fv.AddFailureCondition(i.Ar < 0,"Érvénytelen ár");
            fv.AddFailureCondition(!(Kodok.Any(kk => kk.Kod == i.Kod)),"Érvénytelen kód");
            return fv;
        }

        protected override void PrepareNewItem(Konstrukcio newItem)
        {
            
        }
        public override int GetItemIdentifier(Konstrukcio o) { return o.ID; }
        //public override void SetItemIdentifier(Konstrukcio o, int id) { o.ID = id; }
    }
    public class KonstrukcioKod : ViewModelBase
    {
        public KonstrukcioKod(String nev, Int32 kod)
        {
            Kod = kod;
            OnPropertyChanged("Kod");
            Nev = nev;
            OnPropertyChanged("Nev");
        }

        public Int32 Kod { get; set; }
        public String Nev { get; set; }
    }
}
