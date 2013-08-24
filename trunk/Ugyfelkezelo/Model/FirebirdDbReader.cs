using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace Ugyfelkezelo.Model
{
    public class FirebirdDbReader
    {
        public String DatabasePath { get; set; }

        FbConnection _Connection;

        public FirebirdDbReader()
            : this("")
        { }

        public FirebirdDbReader(string dbpath)
        {
            DatabasePath = dbpath;
        }

        public void Connect()
        {            
            string connectionString = String.Format(connectionStringTemplate, DatabasePath);
            _Connection = new FirebirdSql.Data.FirebirdClient.FbConnection(connectionString);
            _Connection.Open();
        }

        public FbDataReader ExecuteCommand(string commandText)
        {
            FbCommand cmd = new FbCommand(commandText, _Connection);
            FbDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public FbDataReader SelectFieldsWhere(string fields, string table, string where)
        {
            string sql = "SELECT " + fields + " FROM " + table + " WHERE " + where;
            FbCommand cmd = new FbCommand(sql, _Connection);
            FbDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public FbDataReader SelectReader(string table)
        {
            string sql = "SELECT * FROM "+table;
            FbCommand cmd = new FbCommand(sql, _Connection);
            FbDataReader reader = cmd.ExecuteReader();
            return reader;
        }
  /*  {

                //int affectedRows = cmd.ExecuteNonQuery();


                List<object[]> rows = new List<object[]>();
                while (reader.Read())
                {
                    object[] columns = new object[6];
                    reader.GetValues(columns);
                    rows.Add(columns);
                }


                con.Close();
            string connectionString;*/
        

        public void Disconnect()
        {
            _Connection.Close();
        }

        private static readonly string connectionStringTemplate =
"User=SYSDBA;" +
"Password=masterkey;" +
"Database={0};" +
"DataSource=localhost;" +
"Port=3050;" +
"Dialect=3;" +
"Charset=NONE;" +
"Role=;" +
"Connection lifetime=15;" +
"Pooling=true;" +
"MinPoolSize=0;" +
"MaxPoolSize=50;" +
"Packet Size=8192;" +
"ServerType=0";
    }
}
