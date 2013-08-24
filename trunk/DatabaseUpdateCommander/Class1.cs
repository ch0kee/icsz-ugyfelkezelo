using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Deployment.Application;
using System.Diagnostics;
using System.Data.SqlServerCe;

using Microsoft.SqlServer.Server;

using PatchInfo = System.Tuple<System.IO.FileInfo, long>;

namespace DatabaseUpdateCommander
{
    public class DatabaseUpdateCommander
    {
        private const long LATEST_DATABASE_VERSION = 14;
        long _Version;

        public DatabaseUpdateCommander()
        {
        }

        public void QueryDatabaseVersion()
        {
            SqlCeCommand getVersionCmd = _Connection.CreateCommand();
            getVersionCmd.CommandText = "SELECT MAX(Version) FROM [SchemaInfo];";
            _Version = (long)getVersionCmd.ExecuteScalar();
        }

        public Boolean LocalDatabaseIsOutOfDate
        {
            get
            {
                return _Version < LATEST_DATABASE_VERSION;
            }
        }

        class FileInfoVersionComparer : IComparer<Tuple<FileInfo, long>>
        {
            public int Compare(Tuple<FileInfo, long> x, Tuple<FileInfo, long> y)
            {
                if (x.Item2 > y.Item2)
                    return 1;
                if (x.Item2 < y.Item2)
                    return -1;
                else
                    return 0;
            }
        }

        List<PatchInfo> _Patches;


        public void DownloadPatches()
        {
            string dlpath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Patches");
            //delete all previous patch files
            foreach (FileInfo f in new DirectoryInfo(dlpath).GetFiles("v*.sqlce"))
            {
                f.Delete();
            }

            //download patch files
            ApplicationDeployment.CurrentDeployment.DownloadFileGroup("databasepatch");

            _Patches = new List<PatchInfo>();
            foreach (FileInfo f in new DirectoryInfo(dlpath).GetFiles("v*.sqlce"))
            {
                string strVersion = new String(f.Name
                    .Skip(1)
                    .Reverse()
                    .Skip(6)
                    .Reverse()
                    .ToArray()
                    );
                long version = long.Parse(strVersion);
                if (version <= _Version)
                {
                    f.Delete();
                }
                else
                {
                    _Patches.Add(new PatchInfo(f, version));
                }
            }
            _Patches.Sort(new FileInfoVersionComparer());
        }

        SqlCeConnection _Connection;
        public void DisconnectFromDatabase()
        {
            _Connection.Close();
            _Connection.Dispose();
        }

        public void ConnectToDatabase()
        {
            _Connection = new SqlCeConnection(ConnectionString);
            _Connection.Open();
        }


        public void ApplyPatches()
        {
            foreach(PatchInfo pi in _Patches)
            {
                FileInfo file = pi.Item1;

                StreamReader sr = file.OpenText();
                string script = sr.ReadToEnd();

                string[] commands = script.Split(new string[] {"GO\r\n"},  StringSplitOptions.RemoveEmptyEntries);
                foreach (string command in commands)
                {
                    SqlCeCommand cmd = _Connection.CreateCommand();
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }               

                sr.Dispose();
            }           
        }
        
        public string ConnectionString { get; set; }
    }
}
