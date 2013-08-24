using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Deployment.Application;
using System.IO;
using System.Reflection;

using Ugyfelkezelo.View;
using Ugyfelkezelo.ViewModel;
using System.Threading;
using System.Diagnostics;
using Ugyfelkezelo.Model;
using Ugyfelkezelo.ViewModel.Modules;

namespace Ugyfelkezelo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow _MainWindow;
        UgyfelkezeloViewModel _MainViewModel;
        HelpWindow _HelpWindow;

        public App() { }


        protected override void OnStartup(StartupEventArgs ev)
        {
            base.OnStartup(ev);
            Deployment.InitializeDeployment();

            if (!Deployment.EnsureDatabaseIsUpToDate())
                this.Shutdown();

            Deployment.DisplayChangeLog();

            if (!UKModel.Initialize())
                this.Shutdown();

            try {
                _MainViewModel = new ViewModel.UgyfelkezeloViewModel();
                _MainViewModel.CloseEditWindowEvent += new EventHandler(CloseEditWindowEvent);
                _MainViewModel.OpenEditWindowEvent += new EventHandler(OpenEditWindowEvent);

                _CsoportosBeszedesiMegbizasVMWV = new ViewModelWithView<CsoportosBeszedesiMegbizasWindow, CsoportosBeszedesiMegbizasViewModel>
                (true,
                 (v, vm, vmwv) => { vm.LoadData(); }
                );
                _MainViewModel.OpenCSBESZEDWindowEvent += _CsoportosBeszedesiMegbizasVMWV.OpenWindowEventHandler;

                _MainViewModel.OpenHelpWindowEvent += new EventHandler<UgyfelkezeloViewModel.OpenHelpWindowEventArgs>(_MainViewModel_OpenHelpWindowEvent);

                _BeallitasokVMWV = new ViewModelWithView<BeallitasokWindow, BeallitasokViewModel>();
                _MainViewModel.OpenSettingsWindow += _BeallitasokVMWV.OpenWindowEventHandler;// new EventHandler(_MainViewModel_OpenSettingsWindow);

                _InfoCentrumVMWV =
                    new ViewModelWithView<InfocentrumBeallitasokWindow, InfoCentrumViewModel>
                    (true,
                    (v, vm, vmwv) =>
                    {
                        vm.DiscardAndCloseCommand.Event += vmwv.CloseWindowEventHandler;
                        vm.LoadData();
                    });
                _MainViewModel.InfoCentrumSettingsCommand.Event += _InfoCentrumVMWV.OpenWindowEventHandler;


                _MainWindow = new View.MainWindow();
                _MainWindow.DataContext = _MainViewModel;

                _MainWindow.Closed += new EventHandler(_MainWindow_Closed);
                _MainWindow.Show();

                _MainViewModel.SyncInfocentrumBefizetesek();

            }
            catch(Exception e)
            {
                MessageBox.Show("Gond van, az adatok megmaradtak, de ne használd a progit, szólj!\n" + e.Message);
                MessageBox.Show(e.StackTrace);
                App.Current.Shutdown();
            }

            
        }

        ViewModelWithView<BeallitasokWindow, BeallitasokViewModel> _BeallitasokVMWV;
        ViewModelWithView<CsoportosBeszedesiMegbizasWindow, CsoportosBeszedesiMegbizasViewModel> _CsoportosBeszedesiMegbizasVMWV;
        ViewModelWithView<InfocentrumBeallitasokWindow, InfoCentrumViewModel> _InfoCentrumVMWV;

        
        void _MainWindow_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown(0);
            Process.GetCurrentProcess().Kill();
        }

        void _MainViewModel_OpenHelpWindowEvent(object sender, UgyfelkezeloViewModel.OpenHelpWindowEventArgs e)
        {
            if (_HelpWindow != null)
                _HelpWindow.Close();

            _HelpWindow = new HelpWindow();
            _HelpWindow.Navigate(e.Url);

            _HelpWindow.Show();
        }

        void CloseEditWindowEvent(object sender, EventArgs e)
        {
            if (sender == null || !(sender is IHasWindow))
                return;
            IHasWindow ihw = sender as IHasWindow;
            try
            {
                ihw.CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                App.Current.Shutdown();
            }
        }

        void OpenEditWindowEvent(object sender, EventArgs e)
        {
            if (sender == null || !(sender is IHasWindow))
                return;
            IHasWindow ihw = sender as IHasWindow;
            try
            {
                ihw.ShowWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                App.Current.Shutdown();
            }
        }

    }
}
