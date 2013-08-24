using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Ugyfelkezelo.Model;
using System.Data.Objects;
using System.Data.Entity;

namespace Ugyfelkezelo.ViewModel.Modules
{
    public class UgyfelViewModel : EntityViewModelWithEditWindow<Model.Ugyfel, View.EditUgyfelWindow>
    {
        // kell a combobox miatt
        public LepcsohazViewModel LepcsohazViewModel { get; private set; }



        public UgyfelViewModel(DbSet<Ugyfel> ugyfelek, LepcsohazViewModel lepcsohazak)
            : base(ugyfelek)
        {
            LepcsohazViewModel = lepcsohazak;
        }


        public bool EditedItemIsEgyeniElofizeto
        {
            get
            {
                return (EditedItem != null && EditedItem.ElofizetoEgyeni == 1);
            }
            set
            {
                if (EditedItem != null)
                    EditedItem.ElofizetoEgyeni = value ? 1 : 0;
                OnPropertyChanged("EditedItemIsEgyeniElofizeto");
            }
        }

        public bool EditedItemIsCsoportosBeszedes
        {
            get
            {
                return (EditedItem != null && EditedItem.DijbefizetesModjaEnum == Ugyfel.EDijbefizetesModja.Csoportos);
            }
        }



        public Int32 EditedItemDijbefizetesModja
        {
            set
            {
                if (EditedItem != null)
                {
                    EditedItem.DijbefizetesModja = value;
                    OnPropertyChanged("EditedItemIsCsoportosBeszedes");
                }
            }
            get
            {
                return EditedItem.DijbefizetesModja;
            }
        }


        protected override void PrepareNewItem(Model.Ugyfel newItem)
        {
            Window.UgyfelNevAutoCompleteList = Items.Select(u => u.Nev).ToList();
            newItem.ElofizetoEgyeni = 1;
            newItem.SzolgaltatasKezdoIdopontja = DateTime.Today;
        }

        protected override FailureVerifier AttemptToDeleteItem(Model.Ugyfel i)
        {
            bool cant_be_deleted = SuppressConfirmDeleteQuestion ? false : MessageBox.Show(String.Format("Biztosan törölni akarod a következő ügyfelet ?\n{0}", i.Nev),
                "Ügyfélkezelő", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No ;
            FailureVerifier fv = new FailureVerifier(cant_be_deleted);
            fv.AddFailureCondition(i.Elofizetes.Count > 0, "Az ügyfél nem törölhető, mert előfizetés tartozik hozzá!");
            return new FailureVerifier(cant_be_deleted);
        }

        bool IsValidDate(DateTime d)
        {
            return d.Year > 2000 && d.Year < (DateTime.Today.Year + 1 );
        }


        protected override FailureVerifier AttemptToSaveItem(Model.Ugyfel i)
        {
            FailureVerifier fv = new FailureVerifier();
            fv.AddFailureCondition(String.IsNullOrEmpty(i.Nev), "Az ügyfél nevét meg kell adni!");
            fv.AddFailureCondition(i.Lepcsohaz == null, "A lépcsőházat meg kell adni!");
            fv.AddFailureCondition(!IsValidDate(i.SzolgaltatasKezdoIdopontja), "A szolgáltatás kezdő időpont évszámnak 2000 és " + (DateTime.Today.Year+1) + " között kell lenni.");

            if (i.DijbefizetesModjaEnum == Ugyfel.EDijbefizetesModja.Csoportos)
            {
                fv.AddFailureCondition(String.IsNullOrEmpty(i.CsBeszedKod), "Csoportos beszedés esetén meg kell adni a Csoportos beszedés kódját!");
                fv.AddFailureCondition(String.IsNullOrEmpty(i.Bankszamlaszam) || i.Bankszamlaszam.Length != 24
                    || i.Bankszamlaszam.Any(c => !Char.IsDigit(c)), "Csoportos beszedés esetén érvényes (24 jegyű) bankszámlaszámot kell megadni!");
            }
            return fv;
        }

        public override int GetItemIdentifier(Ugyfel o) { return o.ID; }
        //public override void SetItemIdentifier(Ugyfel o, int id) { o.ID = id; }
    }
}
