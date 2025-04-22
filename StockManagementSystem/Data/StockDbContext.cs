using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace StockManagementSystem.Data
{
    /// <summary>
    /// 数据库初始化类
    /// </summary>
    public class DatabaseInitializer
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["StockDbConnection"].ConnectionString;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                // 检查数据库是否存在，不存在则创建
                string scriptPath = Path.Combine(Application.StartupPath, "Data", "CreateDatabase.sql");
                if (File.Exists(scriptPath))
                {
                    string script = File.ReadAllText(scriptPath);
                    string[] commands = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                    // 使用master数据库连接
                    string masterConnectionString = connectionString.Replace("Initial Catalog=StockManagementDB;", "Initial Catalog=master;");

                    foreach (string command in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(command))
                        {
                            using (SqlConnection connection = new SqlConnection(masterConnectionString))
                            {
                                using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                                {
                                    try
                                    {
                                        connection.Open();
                                        sqlCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        // 记录日志
                                        Console.WriteLine("执行脚本时发生错误: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("找不到数据库初始化脚本文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化数据库时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}