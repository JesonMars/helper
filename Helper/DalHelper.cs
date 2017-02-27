using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Helper
{
    public class DalHelper
    {
        private static string _strCon = ConfigurationSettings.AppSettings.Get("dbConnStr");
        private static SqlConnection _dbConnect = null;
        private static readonly object obj=new object();

        private static DbConnection GetCon()
        {
            if (_dbConnect != null) return _dbConnect;
            lock (obj)
            {
                if (_dbConnect != null) return _dbConnect;
                _dbConnect = new SqlConnection(_strCon);
            }
            return _dbConnect;
        }
        public static DataSet ExecuteDataSet(string sql,SqlParameterCollection parameterCollection)
        {
            DataSet ds=new DataSet();
            if (_dbConnect == null)
            {
                DalHelper.GetCon();
            }
            _dbConnect.Open();
            using (SqlTransaction transaction=_dbConnect.BeginTransaction())
            {
                SqlCommand sqlCommand = new SqlCommand(sql);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = _dbConnect;
                sqlCommand.Transaction = transaction;
                sqlCommand.Parameters.Add(parameterCollection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
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

        public static int ExecuteNonQuery(string sql, SqlParameterCollection parameterCollection)
        {
            int result= 0;
            if (_dbConnect == null)
            {
                DalHelper.GetCon();
            }
            _dbConnect.Open();
            using (SqlTransaction transaction = _dbConnect.BeginTransaction())
            {
                SqlCommand sqlCommand = new SqlCommand(sql);
                
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = _dbConnect;
                sqlCommand.Transaction = transaction;
                sqlCommand.Parameters.Add(parameterCollection);
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
