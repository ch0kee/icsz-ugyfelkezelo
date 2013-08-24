using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ugyfelkezelo.Model
{
    public class Payments
    {
        public Payments()
        {
            Fees = new List<MonthlyFee>();
        }

        public void AddMonthlyFee(decimal price, int yy, int mm, int csomagbit)
        {
            foreach (MonthlyFee mf in this.Fees)
            {
                if (mf.Year == yy && mf.Month == mm)
                {
                    mf.Price += price;
                    mf.CsomagokBitSet |= csomagbit;
                    return;
                }
            }
            MonthlyFee newMf = new MonthlyFee(price, yy, mm, csomagbit);
            Fees.Add(newMf);
        }

        //legutolso honaptol kezdve
        public void OrderByDescending()
        {
            Fees = Fees.OrderByDescending(mf => mf.Date).ToList();
        }

        //az azonos idoszakokat beolvasztjuk
        public void Merge(Payments pms)
        {            
            foreach (MonthlyFee pms_mf in pms.Fees)
            {
                bool merged = false;
                foreach(MonthlyFee this_mf in this.Fees)
                {
                    if (this_mf.Year == pms_mf.Year && this_mf.Month == pms_mf.Month)
                    {
                        this_mf.Price += pms_mf.Price;
                        this_mf.CsomagokBitSet |= pms_mf.CsomagokBitSet;
                        merged = true;
                        break;
                    }
                }
                if (!merged)
                    this.Fees.Add(pms_mf);
            }
        }


        public List<MonthlyFee> Fees { get; private set; }
    }
}
