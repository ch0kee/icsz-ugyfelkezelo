using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data.Objects;
using System.Collections.ObjectModel;
using Ugyfelkezelo.Model;
using System.Data.Entity;

namespace Ugyfelkezelo.ViewModel.Modules
{
    public class LepcsohazViewModel : EntityViewModelWithEditWindow<Model.Lepcsohaz, View.EditLepcsohazWindow>
    {
        public UgyfelViewModel UgyfelViewModel { get; private set; }
        public LepcsohazViewModel(DbSet<Lepcsohaz> lepcsohazak, UgyfelViewModel ugyfelek)
            : base(lepcsohazak)
        {
            UgyfelViewModel = ugyfelek;

        }

        protected override void PrepareNewItem(Model.Lepcsohaz newItem)
        {
            newItem.Iranyitoszam = "2030";
            newItem.Varos = "Érd";
        }

        protected override FailureVerifier AttemptToDeleteItem(Model.Lepcsohaz i)
        {
            bool cant_be_deleted = SuppressConfirmDeleteQuestion ? false : MessageBox.Show(String.Format("Biztosan törölni akarod a következő lépcsőházat ?\n{0}", i.TeljesCim), "Ügyfélkezelő", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.No;
            FailureVerifier fv = new FailureVerifier(cant_be_deleted);
            fv.AddFailureCondition(i.Ugyfel.Count > 0,"A lépcsőház nem törölhető, mert ügyfelekhez tartozik!");
            return fv;
        }

        protected override FailureVerifier AttemptToSaveItem(Model.Lepcsohaz i)
        {
            FailureVerifier fv = new FailureVerifier();
            int irszam;
            fv.AddFailureCondition(!Int32.TryParse(i.Iranyitoszam, out irszam) ||
                (irszam < 1000 || irszam > 9999),"Érvénytelen irányítószám");
            fv.AddFailureCondition(String.IsNullOrEmpty(i.Szam),"Érvénytelen lakásszám");
            fv.AddFailureCondition(String.IsNullOrEmpty(i.Utca),"Érvénytelen utca");
            fv.AddFailureCondition(String.IsNullOrEmpty(i.Varos), "Érvénytelen város");
            return fv;
        }
        /*
        public override void RefreshItem(Model.Lepcsohaz l)
        {
            base.RefreshItem(l);
        }*/

        public override int GetItemIdentifier(Lepcsohaz o) { return o.ID; }
        //public override void SetItemIdentifier(Lepcsohaz o, int id) { o.ID = id; }
    }
}
