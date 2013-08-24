using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Deployment.Application;
using System.Diagnostics;
using System.Windows;
using System.Configuration;
using System.Data.SqlServerCe;


using DatabaseInfo = System.Tuple<long, string>;

namespace Ugyfelkezelo
{
    
    //környezet beállítása
    public static class Deployment
    {
        private static String _CompanyName = "ch0kee";
        private static String _ProductName = "LakotelepUgyfelkezelo";
        private static String _DatabaseFile = "ukdb.sdf";

        public static String ProductPath { get { return _ProductPath; } }

        private readonly static String _ProductPath;

        public static String ApplicationPath { get { return System.AppDomain.CurrentDomain.BaseDirectory;}}

        static Deployment()
        {
            string localAppData = Environment.GetFolderPath(
    Environment.SpecialFolder.LocalApplicationData);
            string companyPath = Path.Combine(localAppData, _CompanyName);
            _ProductPath = Path.Combine(companyPath, _ProductName);
        }

        public static void InitializeDeployment()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", _ProductPath);           

        }
        /// <summary>
        /// Verziokulonbsegek elsimitasa
        /// </summary>
        public static bool EnsureDatabaseIsUpToDate()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
                return true;

            if (!ApplicationDeployment.CurrentDeployment.IsFirstRun)
                return true;

            try
            {
                DatabaseUpdateCommander.DatabaseUpdateCommander duc = new DatabaseUpdateCommander.DatabaseUpdateCommander();
                duc.ConnectionString = @"Data Source=|DataDirectory|\ukdb.sdf;";
                duc.ConnectToDatabase();
                duc.QueryDatabaseVersion();
                if (duc.LocalDatabaseIsOutOfDate)
                {
                    BackupDatabaseFile();
                    duc.DownloadPatches();
                    duc.ApplyPatches();
                }
                duc.DisconnectFromDatabase();
            }
            catch(Exception e)
            {
                MessageBox.Show("Gond van, az adatok megmaradtak, de ne használd a progit, szólj!\n" + e.Message);
                MessageBox.Show(e.StackTrace);
                return false;
            }
            return true;
        }


        private static DatabaseInfo GetDatabaseInfo()
        {
            long version;
            string connectstr;
            using (Model.ukdbEntities6 entities = new Model.ukdbEntities6())
            {
                version = entities.SchemaInfo.Max(si => si.Version);
                connectstr = entities.Database.Connection.ConnectionString;
            }
            return new DatabaseInfo(version, connectstr);
        }

        


        /// <summary>
        /// Biztonsagi mentes, ha esetleg valami balul sülne el
        /// </summary>
        private static void BackupDatabaseFile()
        {
            string dbfilePath = Path.Combine(_ProductPath, _DatabaseFile);
            if (!File.Exists(dbfilePath))
                return;
            string backupfname = DateTime.Now.ToString("yyMMddHHmmss") + ".sdf";
            string bkpfilePath = Path.Combine(_ProductPath, backupfname);
            File.Copy(dbfilePath, bkpfilePath);
        }



        public static string CurrentVersion { get {
            if (ApplicationDeployment.IsNetworkDeployed)
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);
            else
                return ("developer alpha");

        
        } }

        public static void DisplayChangeLog()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (ApplicationDeployment.CurrentDeployment.IsFirstRun)
                {

                    String changeLog = "Frissítési információ\nVerziószám ";
                    changeLog += ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4)
                    + "\n--- --- --- --- --- --- --- --- --- ---\n";
                    changeLog += global::Ugyfelkezelo.Properties.Resources.changelog;
                    changeLog = new String(changeLog.TakeWhile(c => c != '#').ToArray());
                    //az első hashmarkig levagjuk
                    MessageBox.Show(changeLog, "Verziófrissítés", MessageBoxButton.OK, MessageBoxImage.Information);
                    //Show the page if it exists
                }
            }
        }
    }
}
