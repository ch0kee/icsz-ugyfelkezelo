using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ugyfelkezelo.Model
{
    //havidij
    public class MonthlyFee
    {
        public MonthlyFee(decimal price, Int32 yy, Int32 mm, int csomagbit)
        {
            Price = price;
            _Date = new DateTime(yy,mm,1);
            CsomagokBitSet = csomagbit;
        }

        public Int32 CsomagokBitSet { get; set; }

        private readonly DateTime _Date;

        public Int32 Month { get { return _Date.Month; } }
        public Int32 Year { get { return _Date.Year; } }

        public DateTime Date { get { return _Date; } } 

        public decimal Price { get; set; }
    }
}
