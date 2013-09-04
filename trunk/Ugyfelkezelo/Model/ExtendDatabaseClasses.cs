using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ugyfelkezelo.Common;

namespace Ugyfelkezelo.Model
{
    public partial class Elofizetes
    {
        //az elofizetes aktiv, ha az idointervallumba, amire vonatkozik, beleesik a mai nap
        public bool Aktiv
        {
            get
            {
                if (this.Mettol.HasValue)
                {
                    if (this.Mettol.Value > DateTime.Today)
                        return false;
                }
                if (this.Meddig.HasValue)
                {
                    if (this.Meddig.Value < DateTime.Today)
                        return false;
                }
                return true;
            }
        }

        public string KonstrukcioNev
        {
            get
            {
                return Konstrukcio.Nev;
            }
        }

        public int KonstrukcioAr
        {
            get
            {
                return Konstrukcio.Ar;
            }
        }

        public DateTime KezdoDatum
        {
            get
            {
                DateTime from = Ugyfel.SzolgaltatasKezdoIdopontja;
                if (this.Mettol.HasValue && this.Mettol.Value > from)
                    from = this.Mettol.Value;
                return from;
            }
        }

        public DateTime ZaroDatum
        {
            get
            {
                DateTime to = DateTime.Today;
                if (this.Meddig.HasValue)
                    to = this.Meddig.Value;
                return to;
            }
        }

        public string ElsoHonap
        {
            get
            {
                DateTime from = KezdoDatum;
                //if (!this.Mettol.HasValue)
                //    return "Szerződéskötés napjától";

                return from.ToString("yyyy' - 'MM");
            }
        }

        public string UtolsoHonap
        {
            get
            {
                DateTime to = ZaroDatum;
                return to.ToString("yyyy' - 'MM");
            }
        }

        public Tuple<DateTime, DateTime> AktivIdoszak
        {
            get
            {
                return new Tuple<DateTime, DateTime>(
                    KezdoDatum,
                    (this.Meddig.HasValue && this.Meddig.Value < DateTime.Today) ? this.Meddig.Value : DateTime.Today);
            }
        }

        public Tuple<DateTime, DateTime> FizetendoIdoszak
        {
            get
            {
                var t = AktivIdoszak;
                if (AreInTheSameMonth(t.Item1, t.Item2))
                    return null;

                if (AreInTheSameMonth(t.Item2, DateTime.Today))
                {
                    return new Tuple<DateTime, DateTime>(t.Item1, t.Item2.AddMonths(-1));
                }
                else
                    return t;
            }
        }


        public Payments FizetendoHonapok
        {
            get
            {
                Payments pms = new Payments();
                var fi = FizetendoIdoszak;
                Int32 csomagbit = this.Konstrukcio.Kod;
                var mi = new MonthIterator(fi.Item1, fi.Item2);
                while (mi.Next())
                {
                    //TODO:megnezni, hogy nincs-e a fizetett idoszakok kozott, egyebkent
                    pms.AddMonthlyFee(this.Konstrukcio.Ar, mi.Year, mi.Month, csomagbit);
                }
                return pms;
            }
        }


        private static String DateTimeToYYYYMM(DateTime d)
        {
            return d.ToString("yyyy' - 'MM");
        }
        private static bool AreInTheSameMonth(DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month;
        }

        //itt majd kivonjuk az elszamolt honapokat
        public string ElsoFizetendoHonap
        {
            get
            {
                Tuple<DateTime, DateTime> from_to = AktivIdoszak;
                if (AreInTheSameMonth(from_to.Item1, from_to.Item2))
                    return "Nincs";
                else
                    return DateTimeToYYYYMM(from_to.Item1);
            }

        }
        /*
        public Int32 FizetendoHonapokSzama
        {
            get
            {
                new DateSpan(
            }
        }
        */
        //itt is ki kell vonni, amit ki kell vonni
        public string UtolsoFizetendoHonap
        {
            get
            {
                Tuple<DateTime, DateTime> from_to = AktivIdoszak;
                if (AreInTheSameMonth(from_to.Item1, from_to.Item2))
                    return "Nincs";

                //ezt a honapot nem szamoljuk
                //ha 'to' erteke == a jelen honappal, akkor ezt még nem kell kifizetnie
                DateTime endDate = from_to.Item2;
                if (AreInTheSameMonth(endDate, DateTime.Today))
                {
                    endDate = endDate.AddMonths(-1);
                }

                return DateTimeToYYYYMM(endDate);
            }
        }
    }
    public partial class Konstrukcio
    {
        public Int32 IntKod
        {
            get
            {
                return Kod;
            }
            set
            {
                Kod = (byte)value;
            }
        }

    }
    public partial class Lepcsohaz
    {
        public String TeljesCim
        {
            get
            {
                return Iranyitoszam.ToString() + " " + Varos + ", " + Utca + " " + Szam;
            }
        }

    }
    public partial class Ugyfel
    {
        public bool KonstrukciotIgenyelt(int konstrukcioKod)
        {
            return ((MegrendeltCsomagok & konstrukcioKod) == konstrukcioKod);
        }

        public bool VanAktivElofizetese
        {
            get
            {
                return Elofizetes.Any(e => e.Aktiv);
            }
        }

        public decimal Tartozas
        {
            get
            {
                decimal tartozas = -Befizetve;
                foreach (Elofizetes e in this.Elofizetes)
                {
                    Konstrukcio k = e.Konstrukcio;
                    //////////////////
                    //alap intervallum
                    var fizIdoszak = e.FizetendoIdoszak;
                    if (fizIdoszak == null)
                        return tartozas;
                    DateSpan dateSpan = new DateSpan(fizIdoszak.Item1, fizIdoszak.Item2);

                    //fizetesi idoszakok
                    /*List<DateSpan> payedSpans = new List<DateSpan>();
                    foreach(FizetettIdoszak fi in e.FizetettIdoszak)
                    {
                        DateTime fifrom = SzolgaltatasKezdoIdopontja.Date;
                        if (fi.Mettol.HasValue)
                            fifrom = fi.Mettol.Value.Date;
                        DateTime fito = DateTime.Today.Date;
                        if (fi.Meddig.HasValue)
                            fito = fi.Meddig.Value.Date;
                        payedSpans.Add(new DateSpan(fifrom, fito));
                    }*/

                    //akkor szamoljunk
                    int monthsToPay = 0;
                    MonthIterator mi = new MonthIterator(dateSpan.Start, dateSpan.End);
                    while (mi.Next())
                    {
                        /*bool inside = false;
                        for(int i = 0; !inside && i < payedSpans.Count; ++i)
                        {
                            inside = mi.IsInside(payedSpans[i]); 
                        }
                        if (!inside) //nincs a fizetett honapok kozt, fizessen*/
                        ++monthsToPay;
                    }
                    tartozas += (decimal)monthsToPay * k.Ar;
                }
                return tartozas;
            }
        }

        public Int32 CsomagBitSet
        {
            set
            {
                if (value > 0)
                {
                    MegrendeltCsomagok |= value;
                }
                else if (value < 0)
                {
                    MegrendeltCsomagok &= ~(-value);
                }
                else
                {
                    MegrendeltCsomagok = 0;
                }

            }
            get
            {
                return MegrendeltCsomagok;
            }
        }

        public String Cim
        {
            get
            {
                return Lepcsohaz.TeljesCim + " " + (String.IsNullOrEmpty(LakohelyEmelet) ? LakohelyLakas : LakohelyEmelet + "/" + LakohelyLakas);
            }
        }

        public String SzamlazasiCim2
        {
            get
            {
                if (String.IsNullOrEmpty(SzamlazasiCim))
                    return Cim;
                else
                    return SzamlazasiCim;
            }


        }

        public String PrettyMegrendeltCsomagok
        {
            get
            {
                if (MegrendeltCsomagok == 0)
                    return "-";

                String pmcs = "|";
                if ((MegrendeltCsomagok & 1) == 1)
                {
                    pmcs += "Basic|";
                }
                if ((MegrendeltCsomagok & 2) == 2)
                {
                    pmcs += "EBS|";
                }
                if ((MegrendeltCsomagok & 4) == 4)
                {
                    pmcs += "HBO1éves|";
                }
                if ((MegrendeltCsomagok & 8) == 8)
                {
                    pmcs += "HBOhnélkül|";
                }
                return pmcs;
            }
        }

        public String PrettyDijbefizetesModja
        {
            get
            {
                switch (DijbefizetesModja)
                {
                    case 0: return "nyugta";
                    case 1: return "csoportos";
                    case 2: return "utalás";
                }
                return "HIBA";
            }
        }

        public String PrettyDijbefizetesUtemezese
        {
            get
            {
                switch (DifbefizetesUtemezese)
                {
                    case 0: return "havi";
                    case 1: return "negyedéves";
                    case 2: return "féléves";
                    case 3: return "éves";
                }
                return "HIBA";
            }
        }

        public enum EDijbefizetesModja { Csoportos, Nyugta, Utalas }


        public EDijbefizetesModja DijbefizetesModjaEnum
        {
            get
            {
                switch (DijbefizetesModja)
                {
                    case 1: return Ugyfel.EDijbefizetesModja.Csoportos;
                    case 2: return Ugyfel.EDijbefizetesModja.Utalas;
                    case 0:
                    default: return Ugyfel.EDijbefizetesModja.Nyugta;
                }
            }

        }

    }
}
