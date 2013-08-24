using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ugyfelkezelo.Common
{
    public class MonthIterator
    {
        public MonthIterator(DateTime thisMonth, DateTime end)
        {
            _ThisMonth = thisMonth.Date.AddMonths(-1); //egyel visszabbrol indulunk, mert nexttel kezdunk  
            _End = end;
        }

        public bool Next()
        {
            _ThisMonth = _ThisMonth.AddMonths(1);
            return _ThisMonth.Date <= _End.Date;
        }

        public bool IsInside(DateSpan ds)
        {
            if (ds.Start <= _ThisMonth && _ThisMonth <= ds.End)
            {
                return true;
            }
            else
                return false;
        }

        public Int32 Month { get { return _ThisMonth.Month; } }
        public Int32 Year { get { return _ThisMonth.Year; } }

        DateTime _ThisMonth;
        DateTime _End;
    }

    public class DateSpan
    {
        public DateSpan(DateTime d1, DateTime d2)
        {
            Start = d1;
            End = d2;
        }

        public Int32 Months
        {
            get
            {
                return MonthsBetween(Start, End);
            }
        }

        


        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public void Exclude(DateSpan ds)
        {

        }

        public void Include(DateSpan ds)
        {

        }

        private static int MonthsBetween(DateTime a, DateTime b)
        {
            int monthsBetween = 0;
            for (int y = a.Year; y <= b.Year; ++y)
            {
                bool thisYear = y == b.Year;
                for (int m = 1; m <= (thisYear ? b.Month : 12); ++m)
                {
                    ++monthsBetween;
                }
            }
            return monthsBetween;
        }
    }
}
