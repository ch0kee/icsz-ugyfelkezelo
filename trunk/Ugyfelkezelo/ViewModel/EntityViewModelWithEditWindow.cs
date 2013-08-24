using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Windows;
using Ugyfelkezelo.Model;
using System.Data.Entity;

namespace Ugyfelkezelo.ViewModel
{
    public class FailureVerifier
    {
        public FailureVerifier()
            : this(false)
        {
        }

        public FailureVerifier(bool failed)
        {
            Accepted = !failed;
        }

        public void AddFailureCondition(bool failed)
        {
            AddFailureCondition(failed, "");
        }

        public void AddFailureCondition(bool failed, string whatsMissing)
        {
            if (!Accepted)
                return;

            if (failed)
            {                
                Accepted = false;
                ErrorMessage = whatsMissing;
            }
        }

        public string ErrorMessage { get; private set; }

        public bool Accepted { get; private set; }
        public bool OmitErrorMessage
        {
            get { return String.IsNullOrEmpty(ErrorMessage); }
        }
    }




    public interface IHasWindow
    {
        event EventHandler CloseEditWindowEvent;
        event EventHandler OpenEditWindowEvent;
        void ShowWindow();
        void CloseWindow();
    }

    //T = Ugyfel , Lepcsohaz, stb.
    public abstract class EntityViewModelWithEditWindow<TItem, TWindow> : EntityViewModelWithDelete<TItem>, IHasWindow
        where TItem : class, new ()
        where TWindow : Window, new ()
    {
        public DelegateCommand NewCommand { get; private set; }        
        public DelegateCommand ModifyCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand SaveAndNewCommand { get; private set; }

        public event EventHandler CloseEditWindowEvent;
        public event EventHandler OpenEditWindowEvent;

        public TWindow Window { get; private set; }
        
        public TItem EditedItem { get; set; }

        public EntityViewModelWithEditWindow( /*Model.ukdbEntities3 entities,*/ DbSet<TItem> objectSet)
            : base(/*entities, */objectSet)
        {
            //parancsok
            NewCommand = new DelegateCommand(x => NewExecuted());
            ModifyCommand = new DelegateCommand(u => ModifyExecuted(u));
            SaveCommand = new DelegateCommand(x => SaveExecuted());
            CancelCommand = new DelegateCommand(x => CancelExecuted());

            SaveAndNewCommand = new DelegateCommand(x => SaveAndNewExecuted());

            CreateWindow();
        }


        protected void CancelExecuted()
        {
            FireEvent(CloseEditWindowEvent);
        }

        abstract protected void PrepareNewItem(TItem newItem);

        virtual protected void NewExecuted()
        {
            EditedItem = new TItem();
            //SetItemIdentifier(EditedItem, 0); //innen tudjuk, hogy ujat kell neki adni
            
            PrepareNewItem(EditedItem);

            FireEvent(OpenEditWindowEvent);
        }

        virtual protected void ModifyExecuted(object o)
        {
            if (o == null || !(o is TItem))
                return;
            TItem u = o as TItem;
            EditedItem = u;
            FireEvent(OpenEditWindowEvent);
        }

        private void SaveAndNewExecuted()
        {
            if (SaveExecuted())
                NewExecuted();
        }


        abstract protected FailureVerifier AttemptToSaveItem(TItem i);

        protected bool SaveExecuted()
        {
            FailureVerifier fv = AttemptToSaveItem(EditedItem);
            if (!fv.Accepted)
            {
                if (!fv.OmitErrorMessage)
                {
                    MessageBox.Show(fv.ErrorMessage, "Ügyfélkezelő", MessageBoxButton.OK);
                }
                return false;
            }

            bool newItem = true;
            foreach (TItem x in _EntitySet)
            {
                if (GetItemIdentifier(x) == GetItemIdentifier(EditedItem))
                {
                    newItem = false;
                    break;
                }
            }
            bool done = ExecuteSafeDatabaseCommand(() =>
            {
                if (newItem)
                {
                    //SetItemIdentifier(EditedItem, GetNextDatabaseID());
                    _EntitySet.Add(EditedItem);
                }
                UKModel.SaveChanges();
                //_Entities.SaveChanges();
                if (newItem)
                    Items.Add(EditedItem);
                /*else
                    RefreshItem(EditedItem);*/
            });

            if (done)
            {
                //UgyfelkezeloViewModel.Instance.RefreshModel();
                CancelExecuted();
            }
            return done;
        }

 
        void IHasWindow.ShowWindow()
        {
            Window.DataContext = this;
            Window.Top = 0;
            Window.Left = 0;
            Window.ShowDialog();
        }

        void IHasWindow.CloseWindow()
        {
            //minden nem mentett modositast eldobunk
            if (GetItemIdentifier(EditedItem) > 0)
            {
                UKModel.ReloadObjectFromDatabase(EditedItem);
                //_Entities.Refresh(RefreshMode.StoreWins, EditedItem);
            }
            UgyfelkezeloViewModel.Instance.RefreshModel();
            if (Window != null)
                Window.Close();
        }


        private void CreateWindow()
        {
            Window = new TWindow();
            Window.Closed += new EventHandler(Window_Closed);
        }

        void Window_Closed(object sender, EventArgs e)
        {
            CreateWindow();          
        }
    }
}
