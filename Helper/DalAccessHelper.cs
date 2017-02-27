using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;

namespace Helper
{
    public class DalAccessHelper
    {
        private static string _strCon = ConfigurationSettings.AppSettings.Get("dbConnStr");
        private static OleDbConnection _dbConnect = null;
        private static readonly object obj = new object();

        private static OleDbConnection GetCon()
        {
            if (_dbConnect != null) return _dbConnect;
            lock (obj)
            {
                if (_dbConnect != null) return _dbConnect;
                _dbConnect = new OleDbConnection(_strCon);
            }
            return _dbConnect;
        }
        public static DataSet ExecuteDataSet(string sql, List<OleDbParameter> parameters)
        {
            DataSet ds = new DataSet();
            if (_dbConnect == null)
            {
                GetCon();
            }
            _dbConnect.Open();
            using (OleDbTransaction transaction = _dbConnect.BeginTransaction())
            {
                
                OleDbCommand sqlCommand = new OleDbCommand(sql);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = _dbConnect;
                sqlCommand.Transaction = transaction;
                if (parameters!=null && parameters.Count>0)
                {
                    foreach (OleDbParameter oleDbParameter in parameters)
                    {
                        sqlCommand.Parameters.Add(oleDbParameter);
                    }
                }

                OleDbDataAdapter sqlDataAdapter = new OleDbDataAdapter(sqlCommand);
                try
                {
                    sqlDataAdapter.Fill(ds);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                _dbConnect.Close();
            }

            return ds;
        }

        public static int ExecuteNonQuery(string sql, List<OleDbParameter> parameters)
        {
            int result = 0;
            if (_dbConnect == null)
            {
                GetCon();
            }
            _dbConnect.Open();
            using (OleDbTransaction transaction = _dbConnect.BeginTransaction())
            {

                OleDbCommand sqlCommand = new OleDbCommand(sql);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = _dbConnect;
                sqlCommand.Transaction = transaction;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (OleDbParameter oleDbParameter in parameters)
                    {
                        sqlCommand.Parameters.Add(oleDbParameter);
                    }
                }

                try
                {
                    result=sqlCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                _dbConnect.Close();
            }

            return result;
        }
    }
}
