using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Helper
{
    public class DalMysqlHelper
    {
        private static string _strCon = ConfigurationSettings.AppSettings.Get("dbConnStr");
        private static MySqlConnection _dbConnect = null;
        private static readonly object obj=new object();

        private static MySqlConnection GetCon()
        {
            if (_dbConnect != null) return _dbConnect;
            lock (obj)
            {
                if (_dbConnect != null) return _dbConnect;
                _dbConnect = new MySqlConnection(_strCon);
            }
            return _dbConnect;
        }
        public static DataSet ExecuteDataSet(string sql, List<MySqlParameter> parameterCollection)
        {
            DataSet ds=new DataSet();
            if (_dbConnect == null)
            {
                DalMysqlHelper.GetCon();
            }
            if (_dbConnect.State != ConnectionState.Open)
            {
                _dbConnect.Open();
            }
            using (MySqlTransaction transaction=_dbConnect.BeginTransaction())
            {
                MySqlCommand sqlCommand = new MySqlCommand(sql);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = _dbConnect;
                sqlCommand.Transaction = transaction;
                parameterCollection = parameterCollection ?? new List<MySqlParameter>();
                foreach (var mySqlParameter in parameterCollection)
                {
                    sqlCommand.Parameters.Add(mySqlParameter);    
                }
                
                MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
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

        public static int ExecuteNonQuery(string sql, List<MySqlParameter> parameterCollection)
        {
            int result= 0;
            if (_dbConnect == null)
            {
                DalMysqlHelper.GetCon();
            }
            if (_dbConnect.State != ConnectionState.Open)
            {
                _dbConnect.Open();
            }
            using (MySqlTransaction transaction = _dbConnect.BeginTransaction())
            {
                MySqlCommand sqlCommand = new MySqlCommand(sql);
                
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = _dbConnect;
                sqlCommand.Transaction = transaction;
                parameterCollection = parameterCollection ?? new List<MySqlParameter>();
                foreach (var mySqlParameter in parameterCollection)
                {
                    sqlCommand.Parameters.Add(mySqlParameter);
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
