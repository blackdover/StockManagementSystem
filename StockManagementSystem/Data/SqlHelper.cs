using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace StockManagementSystem.Data
{
    /// <summary>
    /// 数据库访问工具类
    /// </summary>
    public class SqlHelper
    {
        // 数据库连接字符串
        private static string connectionString = string.Empty;

        /// <summary>
        /// 初始化连接字符串
        /// </summary>
        public static void InitConnString(string connStr)
        {
            connectionString = connStr;
        }

        /// <summary>
        /// 执行SQL语句，返回影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>影响行数</returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    try
                    {
                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // 记录日志
                        System.Diagnostics.Debug.WriteLine("执行SQL错误：" + ex.Message);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL查询，返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>DataTable结果集</returns>
        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    DataTable dt = new DataTable();
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dt);
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        // 记录日志
                        System.Diagnostics.Debug.WriteLine("查询SQL错误：" + ex.Message);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询，返回第一行第一列的值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>结果的第一行第一列的值</returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    try
                    {
                        connection.Open();
                        return command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        // 记录日志
                        System.Diagnostics.Debug.WriteLine("执行SQL错误：" + ex.Message);
                        throw;
                    }
                }
            }
        }
    }
}