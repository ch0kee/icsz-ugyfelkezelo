using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Windows;
using Ugyfelkezelo.Model;
using System.Data.Entity;
using System.Diagnostics;

namespace Ugyfelkezelo.ViewModel
{
    public interface IEntityViewModelWithDelete
    {
        void DeleteSelectedItems(bool askBeforeEveryRow);
    }

    public abstract class EntityViewModelWithDelete<TItem> : EntityViewModel<TItem>, IEntityViewModelWithDelete
        where TItem : class, new ()
    {
        public DelegateCommand DeleteCommand { get; private set; }

        public EntityViewModelWithDelete(/*Model.ukdbEntities3 entities, */DbSet<TItem> objectSet)
            : base(/*entities, */objectSet)
        {
            DeleteCommand = new DelegateCommand(u => DeleteExecuted(u));
        }

        protected void FireEvent(EventHandler e)
        {
            if (e != null)
                e(this, EventArgs.Empty);
        } 



        abstract protected FailureVerifier AttemptToDeleteItem(TItem i);

        virtual protected void DeleteExecuted(object o)
        {
            if (o == null || !(o is TItem))
                return;

            TItem u = o as TItem;

            FailureVerifier fv = AttemptToDeleteItem(u);
            if (!fv.Accepted)
            {
                if (!fv.OmitErrorMessage)
                {
                    MessageBox.Show(fv.ErrorMessage, "Ügyfélkezelő", MessageBoxButton.OK);
                }
                return;
            }


            ExecuteSafeDatabaseCommand(() =>
            {
                UKModel.DeleteObjectFromDatabase(u);
                Items.Remove(u);
            });

            if (_RefreshModelAfterDelete)
            {
                UKModel.SaveChanges();
                UgyfelkezeloViewModel.Instance.RefreshModel();
            }
        }

        private bool _RefreshModelAfterDelete = true;

        private bool _SuppressConfirmDeleteQuestion = false;
        protected bool SuppressConfirmDeleteQuestion { get { return _SuppressConfirmDeleteQuestion; } }

        public void DeleteSelectedItems(bool askBeforeEveryRow)
        {
            Debug.Assert(!_SuppressConfirmDeleteQuestion);
            if (!askBeforeEveryRow)
                _SuppressConfirmDeleteQuestion = true;

            _RefreshModelAfterDelete = false;
            foreach (TItem i in SelectedItems)
            {
                this.DeleteExecuted(i);
            }
            _RefreshModelAfterDelete = true;
            _SuppressConfirmDeleteQuestion = false;
            UKModel.SaveChanges();
            UgyfelkezeloViewModel.Instance.RefreshModel();


        }
    }
}
