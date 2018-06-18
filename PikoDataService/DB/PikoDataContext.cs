using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikoDataService.DB
{
    public class PikoDataContext : IDisposable
    {
        public string PikoDatabase
        {
            get;
            private set;
        }

        public OleDbConnection Connection
        {
            get;
            private set;
        }

        private string _connectionString = "";


        public PikoDataContext(string dbFilePath, string dbFileName)
        {
            if (dbFilePath.Last() != '\\')
                dbFilePath += @"\";
            this._connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}{1}.mdb;Persist Security Info=True", dbFilePath,dbFileName); // Microsoft.Jet.OLEDB.4.0
            //Microsoft.ACE.OLEDB.12.0
            this.Connection = new OleDbConnection(this._connectionString);
            this.Connection.Open();
        }

        public OleDbDataReader Select(string SqlQuery)
        {
            OleDbDataReader dataReader = null;
            using (OleDbCommand Query = this.Connection.CreateCommand())
            {
                Query.Connection = this.Connection;
                Query.CommandText = SqlQuery;
                dataReader = Query.ExecuteReader();
            }
            return dataReader;
        }

        public int UpdateData(string SqlUpdateQuery)
        {
            return this.ExecuteQuery(SqlUpdateQuery);
        }

        public int InsertData(string SqlInsertQuery)
        {
            return this.ExecuteQuery(SqlInsertQuery);
        }

        public void Execute(string SqlQueryToExecute)
        {
            this.ExecuteQuery(SqlQueryToExecute);
        }

        public bool CheckSchema(string ColumnName,string TableName)
        {
            var schema = this.Connection.GetSchema("COLUMNS");
            var col = schema.Select("TABLE_NAME='" + TableName + "' AND COLUMN_NAME='" + ColumnName + "'");
            return col.Length > 0;
        }

        private int ExecuteQuery(string SqlQuery)
        {
            int result = 0;
            using (OleDbCommand Query = this.Connection.CreateCommand())
            {
                Query.Connection = this.Connection;
                Query.CommandText = SqlQuery;
                result = Query.ExecuteNonQuery();
            }
            return result;
        }

        public void Dispose()
        {
            this.Connection.Close();
            this.Connection.Dispose();
        }
    }
}
