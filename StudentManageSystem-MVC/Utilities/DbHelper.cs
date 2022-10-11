using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace StudentManageSystem_MVC.Utilities
{
    public static class DbHelper
    {
        public static SqlConnection GetConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["mydb"].ConnectionString;
            var connection = new SqlConnection(connStr);
            connection.Open();
            return connection;
        }

        public static int DoExecuteNonQuery(string sql, params SqlParameter[] sqlParams)
        {
            using (var connection = GetConnection())
            {
                var command = new SqlCommand { CommandText = sql, Connection = connection };
                if (sqlParams != null)
                {
                    command.Parameters.AddRange(sqlParams);
                }

                return command.ExecuteNonQuery();
            }
        }

        public static object DoExecuteScalar(string sql, params SqlParameter[] sqlParams)
        {
            using (var connection = GetConnection())
            {
                var command = new SqlCommand { CommandText = sql, Connection = connection };
                if (sqlParams != null)
                {
                    command.Parameters.AddRange(sqlParams);
                }

                return command.ExecuteScalar();
            }
        }

        public static DataTable DoExecuteQuery(string sql, params SqlParameter[] sqlParams)
        {
            using (var connection = GetConnection())
            {
                var adapter = new SqlDataAdapter(sql, connection);
                if (sqlParams != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(sqlParams);
                }

                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        // 辅助方法
        public static void Show(DataTable table, Action<DataRow> act)
        {
            foreach (DataRow row in table.Rows) act(row);
        }

        public static List<T> ToModelList<T>(this DataTable dt)
        {
            return
                dt.Rows.Cast<DataRow>()
                .Select(r =>
                {
                    T t = (T)Activator.CreateInstance(typeof(T));
                    foreach (PropertyInfo p in typeof(T).GetProperties())
                    {
                        p.SetValue(t, r[p.Name.ToLower()]);
                    }
                    return t;
                })
                .ToList();
        }
    }
}
