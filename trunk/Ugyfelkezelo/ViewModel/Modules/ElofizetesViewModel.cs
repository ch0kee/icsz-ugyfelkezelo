using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Ugyfelkezelo.Model;
using System.Data.Objects;
using System.Data.Entity;
using Ugyfelkezelo.View;

namespace Ugyfelkezelo.ViewModel.Modules
{
    public class ElofizetesViewModel : EntityViewModelWithDelete<Model.Elofizetes>, IDisposable
    {
        public DelegateCommand ManageElofizetesekCommand { get; private set; }
        public ElofizetesViewModel(DbSet<Elofizetes> elofizetesek)
            : base(elofizetesek)
        {
            ManageElofizetesekCommand = new DelegateCommand((o) => ManageElofizetesekExecuted(o));
        }

        protected void ManageElofizetesekExecuted(object o)
        {
            ManageElofizetesekWindow window = new ManageElofizetesekWindow();
            window.DataContext = this;
            window.ShowDialog();
        }




        public void AddElofizetesek(IList<Elofizetes> elofizetesek)
        {
            try
            {
                ExecuteSafeDatabaseCommand(() =>
                    {
                        //Queue<Int32> ids = GetNextDatabaseIDs(elofizetesek.Count);
                        foreach (Elofizetes e in elofizetesek)
                        {
                            //if (e.ID == 0)
                            //    e.ID = ids.Dequeue();
                            _EntitySet.Add(e);
                        }
                    });
                UKModel.SaveChanges();
                //_Entities.SaveChanges();
                UgyfelkezeloViewModel.Instance.RefreshModel();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Hiba a művelet végrehajtása közben:\n"+ex.Message , "Ügyfélkezelő", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
        }

        public void PopulateByContract(IList<Ugyfel> ugyfelek, ref ObservableCollection<Elofizetes> elofizetesek)
        {
            elofizetesek.Clear();

            Dictionary<Byte, Konstrukcio> konstrukciok = new Dictionary<byte, Konstrukcio>();
            //konstrukciók kigyűjtése
            foreach (Konstrukcio k in UgyfelkezeloViewModel.Instance.KonstrukcioViewModel.Items)
            {
                konstrukciok.Add(k.Kod, k);
            }

            //előfizetések indítása
            foreach (Ugyfel u in ugyfelek)
            {

                foreach (var k in konstrukciok)
                {
                    if (u.KonstrukciotIgenyelt(k.Key))
                    {


                        Elofizetes e = new Elofizetes();
                        e.Ugyfel = u;
                        e.Konstrukcio = k.Value;

                        e.Mettol = null;
                        e.Meddig = null;
                        elofizetesek.Add(e);
                    }
                }
            }
            UKModel.SaveChanges();
            UgyfelkezeloViewModel.Instance.RefreshModel();
        }

        public override int GetItemIdentifier(Elofizetes o) { return o.ID; }
        //public override void SetItemIdentifier(Elofizetes o, int id) { o.ID = id; }

        //varazslohoz

        protected override FailureVerifier AttemptToDeleteItem(Elofizetes i)
        {
            bool cant_be_deleted = SuppressConfirmDeleteQuestion ? false : MessageBox.Show(String.Format("Biztosan törölni akarod a következő előfizetést ?\n{0} / {1} / {2}",
                i.Ugyfel.Nev, i.Konstrukcio.Nev, i.Konstrukcio.Ar), "Ügyfélkezelő", MessageBoxButton.YesNo, MessageBoxImage.Question)
    == MessageBoxResult.No;
            FailureVerifier fv = new FailureVerifier(cant_be_deleted);
            return fv;
        }
    }
}
