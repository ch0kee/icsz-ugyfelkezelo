using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Ugyfelkezelo.ViewModel;

namespace Ugyfelkezelo.Controls
{
    /*
    public class MonthPickerModel : ViewModel.ViewModelBase
    {
        public ObservableCollection<MonthDescriptor> Months { get; private set; }

        Int32 _SelectedMonthIndex = 0;
        Int32 _SelectedYear = 2001;
        public Int32 SelectedMonthIndex { get { return _SelectedMonthIndex; } set
        {
            Months[_SelectedMonthIndex].Enabled = false;
            _SelectedMonthIndex = value;
            Months[_SelectedMonthIndex].Enabled = true;
            OnPropertyChanged("SelectedMonthIndex"); OnPropertyChanged("SelectedDate"); } }
        public Int32 SelectedYear { get { return _SelectedYear; } set { _SelectedYear = value; OnPropertyChanged("SelectedYear"); OnPropertyChanged("SelectedDate"); } }


        public MonthPickerModel()
        {
            Months = new ObservableCollection<MonthDescriptor>();
            Months.Add(new MonthDescriptor(0, "Január"));
            Months.Add(new MonthDescriptor(1, "Február"));
            Months.Add(new MonthDescriptor(2, "Március"));
            Months.Add(new MonthDescriptor(3, "Április"));
            Months.Add(new MonthDescriptor(4, "Május"));
            Months.Add(new MonthDescriptor(5, "Június"));
            Months.Add(new MonthDescriptor(6, "Július"));
            Months.Add(new MonthDescriptor(7, "Augusztus"));
            Months.Add(new MonthDescriptor(8, "Szeptember"));
            Months.Add(new MonthDescriptor(9, "Október"));
            Months.Add(new MonthDescriptor(10, "November"));
            Months.Add(new MonthDescriptor(11, "December"));

            foreach (MonthDescriptor md in Months)
            {
                md.SelectMonth = new ViewModel.DelegateCommand(m => SelectMonthExecuted(m));
            }

            PreviousYearCommand = new DelegateCommand(x => SelectedYear = (SelectedYear - 1));
            NextYearCommand = new DelegateCommand(x => SelectedYear = (SelectedYear + 1));

            SelectedMonthIndex = DateTime.Today.Month - 1;
            SelectedYear = DateTime.Today.Year;
        }

        public string SelectedDate
        {
            get
            {
                return String.Format("{0}. {1}. hónap ({2})", SelectedYear, Months[SelectedMonthIndex].Index+1, Months[SelectedMonthIndex].Name);
            }
            set
            {
            }
        }


        public DelegateCommand PreviousYearCommand { get; set; }
        public DelegateCommand NextYearCommand { get; set; }



        private void SelectMonthExecuted(object o)
        {
            if (o == null || !(o is Int32))
                return;
            Int32 i = (Int32) o;
            SelectedMonthIndex = i;

            if (CloseWindow != null)
                CloseWindow(this, EventArgs.Empty);

        }

        public event EventHandler CloseWindow;
    }
        */
}
