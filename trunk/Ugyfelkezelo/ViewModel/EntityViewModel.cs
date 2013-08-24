using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Collections.ObjectModel;
using System.Windows;
using Ugyfelkezelo.Model;
using System.Data.Entity;

namespace Ugyfelkezelo.ViewModel
{
    public interface IItemHasID<T>
        where T : class
    {

        int GetItemIdentifier(T o);
        //void SetItemIdentifier(T o, int id);
    }

    public interface IModule
    {
        void ForceRefreshAll();
    }


    public abstract class EntityViewModel<TItem> : ViewModelBase, IItemHasID<TItem>, IModule, IDisposable
        where TItem : class, new ()
    {
        public List<TItem> SelectedItems { get; set; }

        //protected Model.ukdbEntities3 _Entities;
        protected DbSet<TItem> _EntitySet;
        public ObservableCollection<TItem> Items { get; private set; }
       
        public EntityViewModel(/* Model.ukdbEntities3 entities, */DbSet<TItem> objectSet)
        {
            //_Entities = entities;
            _EntitySet = objectSet;
            Items = new ObservableCollection<TItem>(objectSet);
            SelectedItems = new List<TItem>();
        }

        //public EntityViewModelFeature Feat

        protected bool ExecuteSafeDatabaseCommand(Action a)
        {
            try
            {
                a();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Adatbázis hiba\n" + (e.InnerException != null ? e.InnerException.ToString() : e.Message), "Ügyfélkezelő", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
                return false;
            }
        }



        public void ForceRefreshAll()
        {
            while (Items.Count != 0)
                Items.RemoveAt(0);
            foreach (TItem i in _EntitySet)
            {
                UKModel.ReloadObjectFromDatabase(i);
                //_Entities.Refresh(RefreshMode.StoreWins, i);
                Items.Add(i);
            }
        }

        public abstract int GetItemIdentifier(TItem o);

        public void Dispose()
        {
            
        }
    }
}
