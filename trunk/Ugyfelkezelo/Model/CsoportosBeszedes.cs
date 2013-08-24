using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Ugyfelkezelo.ViewModel.Modules;
using Ugyfelkezelo.ViewModel;

namespace Ugyfelkezelo.Model
{ 
    public class CsoportosBeszedes
    {
        /*
         * Itt ossze is szedjuk megfelelo formaban az adatokat
         */
        public void CollectData(IList<Ugyfel> ugyfelek, IList<CSBESZEDMegbizas> Megbizasok)
        {
            //eloszor az aktualis honap,
            //a tobbibe a tartozasok
            Dictionary<Ugyfel, Queue<CSBESZEDUgyfel>> megbizasok = new Dictionary<Ugyfel, Queue<CSBESZEDUgyfel>>();
            foreach (Ugyfel u in ugyfelek)
            {
                //ha csoportos
                if (u.DijbefizetesModjaEnum == Ugyfel.EDijbefizetesModja.Csoportos)
                {
                    Payments allpayments = new Payments();
                    foreach (Elofizetes e in u.Elofizetes)
                    {
                        Payments pms = e.FizetendoHonapok;
                        allpayments.Merge(pms);
                    }

                    //a legfrissebb van legelol, utana a tartozasok
                    allpayments.OrderByDescending();

                    int numOfMonthsToPay = allpayments.Fees.Count;
                    if (numOfMonthsToPay > 0) //van fizetendo honap?
                    {
                        megbizasok.Add(u, new Queue<CSBESZEDUgyfel>());
                        for (int i = 0; i < numOfMonthsToPay; ++i)
                        {
                            var csbeszed = new CSBESZEDUgyfel();
                            csbeszed.Ugyfel = u;
                            csbeszed.Kod = u.CsBeszedKod;

                            MonthlyFee mf = allpayments.Fees[i];
                            csbeszed.Csomagok = CsomagBitSetToCsomagNevek(mf.CsomagokBitSet);
                            csbeszed.Ar = (Int32)mf.Price;

                            megbizasok[u].Enqueue(csbeszed);
                        }
                    }

                }
            }
            //////////////////////
            //addig megyunk amig minden sor ki nem urul
            int counter = 1;
            while (megbizasok.Count > 0)
            {
                List<CSBESZEDUgyfel> beszedesek = new List<CSBESZEDUgyfel>();
                foreach (Ugyfel u in ugyfelek)
                {
                    if (megbizasok.ContainsKey(u) && megbizasok[u].Count > 0)
                    {
                        var queue = megbizasok[u];
                        CSBESZEDUgyfel csbu = queue.Dequeue();
                        beszedesek.Add(csbu);
                        if (queue.Count == 0)
                        {
                            megbizasok.Remove(u);
                        }
                    }
                }
                if (beszedesek.Count != 0)
                {
                    var ujmegbizas = new CSBESZEDMegbizas
                    {
                        Fejlec = counter.ToString().PadLeft(4, '0'),
                        Ugyfelek = new ObservableCollection<CSBESZEDUgyfel>(beszedesek)
                    };
                    ujmegbizas.RawData = GenerateRawData(ujmegbizas);
                    Megbizasok.Add(ujmegbizas);
                    ++counter;
                }
            }

        }


        private class InfoProvider
        {
            public static string Kozlemeny { get { return "KTV CSBESZED"; } }

            private static bool IsWeekend(DateTime d)
            {
                return d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday;
            }

            public static DateTime MaiNap { get { return DateTime.Today; } }
            public static DateTime ErtesitesiHatarido
            {
                get
                {
                    //hetvegeket atlepdeljuk
                    DateTime d = MaiNap;
                    int weekdaysBetween = 0;
                    while (weekdaysBetween != 10)
                    {
                        if (!IsWeekend(d))
                        {
                            ++weekdaysBetween;
                        }
                        d = d.AddDays(1);
                    }
                    return d;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        delegate bool StringValidatorFunction(string str);
        delegate bool CharacterValidatorFunction (char c);

        abstract class Token
        {
            StringValidatorFunction _Validator = s => true;

            public virtual bool isValid() { return _Validator( this.ToString() ); }

            //c conjunctive validators
            public Token andStringIs(StringValidatorFunction svf) { _Validator = (s) => _Validator(s) && svf(s); return this;  }
            public Token andCharsAre(CharacterValidatorFunction cvf)
            {
                return andStringIs((string s) => s.All(c => cvf(c)));
            }

            public static Token operator + (Token t1, Token t2) {
                return t1._appendToken(t2);
            }

            protected abstract Token _appendToken(Token t);

            public new abstract string ToString();

        }

 

        class CompositeToken : Token
        {
            public override bool isValid()
            {
                return _Parts.All(p => p.isValid());
            }

            private List<Token> _Parts;
            protected override Token _appendToken(Token t)
            {
                _Parts.Add(t);
                return this;
            }
            public override string  ToString()
            {
                return _Parts.Aggregate("", (p1, p2)  => p1 + p2); 
            }

            public CompositeToken(List<Token> tokens)
            {
                _Parts = tokens;
            }

        }



        class StringToken : Token
        {
            protected string _Value;
            protected override Token _appendToken(Token t)
            {
                return new CompositeToken( new List<Token> { this, t });
            }

            public StringToken(string value)
            {
                _Value = value;
            }

            public override string ToString()
            {
                return _Value;
            }

        }

        class FixedWidthToken : StringToken
        {
            protected int _Width;
            public FixedWidthToken(string value, int width)
                : base(value)
            {
                _Width = width;
                andStringIs((s) => s.Length == width);
            }
        }

        class PaddedToken : FixedWidthToken
        {
            public PaddedToken(string value, int width)
                : base(value, width)
            { }

            public override string ToString()
            {
                return base.ToString().PadRight(_Width, ' ');
            }

        }

        class NumberToken : FixedWidthToken
        {
            public NumberToken(int value, int width)
                : base(value.ToString(), width)
            {
                andCharsAre(char.IsDigit);
            }

            public override string ToString()
            {
                return base.ToString().PadLeft(_Width, '0');
            }
        }

        

        Token adoazonosito(string a)
        {//F213
            return szoveg(a, 13).andStringIs(s => s[0] == 'A');
        }

        Token uzenet_azonosito(string f)
        { //F214
            return new FixedWidthToken(f, 4).andCharsAre(char.IsDigit);
        }

        Token bankszamlaszam(string bsz)
        {
            return new FixedWidthToken(bsz, 8*3).andCharsAre(char.IsDigit);
        }

        Token datum(DateTime date)
        {
            return new FixedWidthToken(date.ToString("yyyyMMdd"), 8).andCharsAre(char.IsDigit);
        }


        Token kozlemeny(string k)
        {
            return szoveg(k, 70);
        }


        Token token_csoport(List<Token> tokens)
        {
            return new CompositeToken(tokens) + sorzaro();
        }

        Token szam(int number, int width)
        {
            return new NumberToken(number, width);
        }

        Token szoveg(string a, int w)
        {
            return new PaddedToken(a, w);
        }

        Token szoveg(string s)
        {
            return new FixedWidthToken(s, 9);
        }

        Token sorzaro()
        {
            return new FixedWidthToken("\r\n", 2);
        }


        private string GenerateRawData(CSBESZEDMegbizas megbizas)
        {
            string sajatAdoazonosito = UKModel.Beallitasok.Adoazonosito;
            string sajatBankszamlaszam = UKModel.Beallitasok.Bankszamlaszam;
            string sajatCegnev = UKModel.Beallitasok.Cegnev;

            //FEJ
            var csbeszed = token_csoport(new List<Token> {
                szoveg("01BESZED0",9),
                adoazonosito(sajatAdoazonosito),
                datum(DateTime.Today),
                uzenet_azonosito(megbizas.Fejlec),
                bankszamlaszam(sajatBankszamlaszam),
                datum(InfoProvider.ErtesitesiHatarido),
                szoveg(sajatCegnev, 35),
                kozlemeny(InfoProvider.Kozlemeny)
            });

            //TÉTELEK
            for (int i = 0; i < megbizas.Ugyfelek.Count; ++i) {
                CSBESZEDUgyfel uf = megbizas.Ugyfelek[i];

                csbeszed += token_csoport(new List<Token> {
                    szam(i + 1, 6),//tetel_sorszam 211
                    datum(InfoProvider.ErtesitesiHatarido), //esedékesség 212
                    szam(uf.Ar, 10), //összeg 213
                    bankszamlaszam(uf.Ugyfel.Bankszamlaszam), //214
                    szoveg(uf.Kod, 24), //azonosító 215
                    szoveg(uf.Ugyfel.Nev, 35),//név 216
                    szoveg(uf.Ugyfel.SzamlazasiCim2, 35),//cím 217
                    szoveg(uf.Ugyfel.Nev, 35),//sztul 218
                    kozlemeny(InfoProvider.Kozlemeny)//közlemény 219
                });
            }

            //LÁBLÉC
            csbeszed += token_csoport(new List<Token> {
                szam(megbizas.Ugyfelek.Count, 6), //211 tételek száma
                szam( megbizas.Ugyfelek.Sum(uf => uf.Ar), 16) //211 tételek összértéke
            });

            // // //
            //adatcsomag összeállítása
            string data = csbeszed.ToString();
            return data;
        }


        private string GenerateRawData2(CSBESZEDMegbizas megbizas)
        {

            string sajatAdoazonosito = UKModel.Beallitasok.Adoazonosito;
            adoazonosito(sajatAdoazonosito);
            string sajatBankszamlaszam = UKModel.Beallitasok.Bankszamlaszam;
            string sajatCegnev = UKModel.Beallitasok.Cegnev;

            //adóazonosító
            string F213 = sajatAdoazonosito.PadRight(13, ' ');
            if (F213.Length != 13 || F213[0] != 'A')
                throw new Exception("Rossz adóazonosító:" + F213);
            //üzenetazonosító
            string code = megbizas.Fejlec;
            if (code.Length != 4 || !code.All(c => Char.IsDigit(c)))
                throw new Exception("Hibás fejlec:" + code);
            string F214 = DateTime.Today.ToString("yyyyMMdd") + code; //mai nap
            if (F214.Length != 12)
                throw new Exception("Hibás mező:" + F214);
            string F215 = sajatBankszamlaszam;
            if (F215.Length != 24 || !F215.All(c => char.IsDigit(c)))
                throw new Exception("Hibás bankszámlaszám:" + F215);
            string F216 = InfoProvider.ErtesitesiHatarido.ToString("yyyyMMdd"); //értesítési határidő
            string F218 = sajatCegnev.PadRight(35, ' ');
            if (F218.Length != 35)
                throw new Exception("Túl hosszú cégnév:" + F218);

            string F219 = InfoProvider.Kozlemeny.PadRight(70, ' ');
            if (F219.Length != 70)
                throw new Exception("Túl hosszú közlemény:" + F219);

            string FEJ = "01BESZED0" + F213 + F214 + F215 + F216 + F218 + F219;
            //csbeszedes.CSBESZEDES_FEJ.AddRow(F213, F214, F215, F216, F218, F219);

            int összérték = 0;
            List<string> rows = new List<string>();
            for (int i = 0; i < megbizas.Ugyfelek.Count; ++i)
            {
                CSBESZEDUgyfel uf = megbizas.Ugyfelek[i];

                //tételsorszám
                string T211 = (i + 1).ToString().PadLeft(6, '0');
                //esedékességi dátum
                string T212 = InfoProvider.ErtesitesiHatarido.ToString("yyyyMMdd"); //ugyanaz, mint az ertesitesi hatairod
                //összeg
                string T213 = uf.Ar.ToString().PadLeft(10, '0');
                összérték += uf.Ar;
                //számlaszám
                string T214 = uf.Ugyfel.Bankszamlaszam;
                if (T214.Length != 24 || !T214.All(c => char.IsDigit(c)))
                    throw new Exception("Hibás bankszámlaszám:" + T214);
                //azonosító
                string T215 = uf.Kod.PadRight(24, ' ');
                if (T215.Length != 24)
                    throw new Exception("Túl hosszú azonosító:" + T215);
                //név
                string T216 = uf.Ugyfel.Nev.PadRight(35, ' ');
                if (T216.Length != 35)
                    throw new Exception("Túl hosszú:" + T216);
                //cím
                string T217 = uf.Ugyfel.SzamlazasiCim2.PadRight(35, ' ');
                if (T217.Length != 35)
                    throw new Exception("Túl hosszú:" + T217);
                //számlatulajdonos
                string T218 = uf.Ugyfel.Nev.PadRight(35, ' ');
                if (T218.Length != 35)
                    throw new Exception("Túl hosszú:" + T218);
                //közlemény
                string T219 = InfoProvider.Kozlemeny.PadRight(70, ' ');
                rows.Add(T211 + T212 + T213 + T214 + T215 + T216 + T217 + T218 + T219);
            }
            //tételek száma
            string Z211 = rows.Count.ToString().PadLeft(6, '0');
            //tételek összértéke
            string Z212 = összérték.ToString().PadLeft(16, '0'); ;
            string LAB = Z211 + Z212;

            /////////////////////////
            //adatcsomag összeállítása
            string data = "";
            {
                data += FEJ + "\r\n";
                for (int i = 0; i < rows.Count; ++i)
                {
                    data += rows[i] + "\r\n";
                }
                data += LAB + "\r\n";
            }
            return data;
        }

        private static string CsomagBitSetToCsomagNevek(Int32 csomagbitset)
        {
            if (csomagbitset == 0)
                return "-";

            String pmcs = "|";
            if ((csomagbitset & 1) == 1)
            {
                pmcs += "Basic|";
            }
            if ((csomagbitset & 2) == 2)
            {
                pmcs += "EBS|";
            }
            if ((csomagbitset & 4) == 4)
            {
                pmcs += "HBO1éves|";
            }
            if ((csomagbitset & 8) == 8)
            {
                pmcs += "HBOhnélkül|";
            }
            return pmcs;
        }
    }
}