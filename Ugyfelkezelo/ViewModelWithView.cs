using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ugyfelkezelo.ViewModel;

namespace Ugyfelkezelo
{
    public class ViewModelWithView <TView, TViewModel>
        where TView : Window, new ()
        where TViewModel : ViewModelBase, new ()
    {
        bool _AlwaysRecreateModel;
        public ViewModelWithView(bool alwaysRecreateModel, Action<TView, TViewModel, ViewModelWithView<TView, TViewModel> > doBeforeShow)
        {
            _AlwaysRecreateModel = alwaysRecreateModel;
            _DoBeforeShowAction = doBeforeShow;
        }

        public ViewModelWithView()
            : this(true, null)
        {
        }


        Action<TView, TViewModel, ViewModelWithView<TView, TViewModel> > _DoBeforeShowAction;

        TView _View;
        TViewModel _ViewModel;

        public TViewModel ViewModel { get { return _ViewModel; } }

        public void OpenWindowEventHandler(object sender, EventArgs e)
        {
            if (_View != null)
                _View.Close();
            if (_ViewModel == null || _AlwaysRecreateModel)
                _ViewModel = new TViewModel();
            _View = new TView();
            _View.DataContext = _ViewModel;
            if (_DoBeforeShowAction != null)
                _DoBeforeShowAction(_View, _ViewModel, this);
            _View.ShowDialog();
        }

        public void CloseWindowEventHandler(object sender, EventArgs e)
        {
            if (_View != null)
                _View.Close();
        }

    }
}
