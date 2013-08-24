using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace Ugyfelkezelo.Model
{
    public static class UKModel
    {
        static ukdbEntities6 _Entities = null;

        public static void Shutdown()
        {
            if (_Entities != null)
                _Entities.Dispose();
        }

        public static bool Initialize()
        {
            if (_Entities != null)
            {
                //already initialized
                return true;
            }

            _Entities = new ukdbEntities6();


            if (_Entities.Beallitasok.Count() == 0)
            {
                Beallitasok b = new Beallitasok();
                //b.ID = 1;
                b.Cegnev = "cegnev";
                b.Adoazonosito = "adoszam";
                b.Bankszamlaszam = "Bankszamlaszam";
                _Entities.Beallitasok.Add(b);
                _Entities.SaveChanges();
            }
            
            return true;
        }

        public static void ReloadObjectFromDatabase(object e)
        {
            _Entities.Entry(e).Reload();
            //_Entities.Refresh(RefreshMode.StoreWins, e);
        }

        public static void DeleteObjectFromDatabase(object e)
        {
            _Entities.Set(e.GetType()).Remove(e);
        }

        public static void AddObjectToDatabase<TItem>(TItem i)        
        {
            _Entities.Set(i.GetType()).Add(i);
            //_Entities.AddObject(i.GetType().ToString(), i);
        }


        public static DbSet<Ugyfel> Ugyfelek { get { return _Entities.Ugyfel; } }
        public static DbSet<Lepcsohaz> Lepcsohazak { get { return _Entities.Lepcsohaz; } }
        public static DbSet<Konstrukcio> Konstrukciok { get { return _Entities.Konstrukcio; } }
        public static DbSet<Elofizetes> Elofizetesek { get { return _Entities.Elofizetes; } }

        public static Beallitasok Beallitasok { get { return _Entities.Beallitasok.First(); } }
        public static DbSet<InfoCentrumSzamlatomb> InfoCentrumSzamlatombok { get { return _Entities.InfoCentrumSzamlatomb; } }

        public static void SaveChanges() { _Entities.SaveChanges(); }

        public static FirebirdDbReader ICSzamlaReader
        {
            get
            {
                if (_ICSzamlaReader == null)
                {
                    string origpath = Beallitasok.IcSzamlaUtvonal;
                    if (!File.Exists(origpath))
                    {
                        return null;
                    }

                    //steal database
                    string dbpath = Path.Combine(Deployment.ProductPath, "icszamla.fdb");
                    File.Delete(dbpath);
                    File.Copy(origpath, dbpath);

                    //connect
                    FirebirdDbReader fbdbr = new FirebirdDbReader(dbpath);
                    _ICSzamlaReader = fbdbr;
                    try
                    {
                        fbdbr.Connect();
                    }
                    catch
                    {
                        throw new Exception("Hiba a Firebird kapcsolat megnyitása során.");
                    }
                    return fbdbr;
                }
                else
                    return _ICSzamlaReader;
            }        
        }
        static FirebirdDbReader _ICSzamlaReader;

    }
}
