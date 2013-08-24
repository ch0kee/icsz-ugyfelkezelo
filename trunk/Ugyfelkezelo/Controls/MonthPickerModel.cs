using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Ugyfelkezelo.ViewModel;

namespace Ugyfelkezelo.Controls
{
    public class MonthPickerModel : ViewModel.ViewModelBase
    {
        public ObservableCollection<MonthDescriptor> Months { get; private set; }

        public Int32 SelectedMonthIndex { get; set; }
        public Int32 SelectedYear { get; set; }


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

            PreviousYearCommand = new DelegateCommand(x => SetCurrentYear(SelectedYear - 1));
            NextYearCommand = new DelegateCommand(x => SetCurrentYear(SelectedYear + 1));

            PreviousMonthCommand = new DelegateCommand(x => {
                if (SelectedMonthIndex == 0)
                    SetCurrentYear(SelectedYear - 1);
                SetCurrentMonth((SelectedMonthIndex - 1) % 12);
            });
            NextMonthCommand = new DelegateCommand(x => {
                SetCurrentMonth((SelectedMonthIndex + 1) % 12);
                if (SelectedMonthIndex == 0)
                    SetCurrentYear(SelectedYear + 1);
            });


            SetCurrentMonth(DateTime.Today.Month - 1);
            SetCurrentYear(DateTime.Today.Year);
        }



        public DelegateCommand PreviousYearCommand { get; set; }
        public DelegateCommand NextYearCommand { get; set; }

        public DelegateCommand PreviousMonthCommand { get; set; }
        public DelegateCommand NextMonthCommand { get; set; }

        private void SetCurrentYear(int yy)
        {
            SelectedYear = yy;
            OnPropertyChanged("SelectedYear");
        }

        private void SetCurrentMonth(int mm)
        {
            Months[SelectedMonthIndex].Enabled = false;
            SelectedMonthIndex = mm;
            Months[SelectedMonthIndex].Enabled = true;
            OnPropertyChanged("SelectedMonth");
        }

        private void SelectMonthExecuted(object o)
        {
            if (o == null || !(o is Int32))
                return;
            Int32 i = (Int32) o;
            SetCurrentMonth(i);

            if (CloseWindow != null)
                CloseWindow(this, EventArgs.Empty);

        }

        public event EventHandler CloseWindow;
    }
        
}
