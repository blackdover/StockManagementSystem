using System;
using System.IO;  // 用于文件操作，如读取SQL脚本文件
using System.Windows.Forms;  // 用于获取应用程序路径（Application.StartupPath）
using System.Data.SqlClient;  // 用于SQL Server数据库连接和操作

namespace StockManagementSystem.Data
{
    // 数据库初始化类
    public class DatabaseInitializer
    {
        // 数据库连接字符串
        private static readonly string connectionString = 
        "Data Source=.;Initial Catalog=StockManagementDB;Integrated Security=True;MultipleActiveResultSets=True";
        
        // 初始化数据库
        public static void InitializeDatabase()
        {
            string scriptPath = Path.Combine(Application.StartupPath, "Data", "CreateDatabase.sql");
            if (File.Exists(scriptPath))
            {
                string script = File.ReadAllText(scriptPath);
                string[] batches = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                
                // 使用master数据库连接
                string masterConnectionString = connectionString.Replace("Initial Catalog=StockManagementDB;", "Initial Catalog=master;");

                // 在master中执行数据库创建语句
                using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
                {
                    try
                    {
                        masterConnection.Open();
                        using (SqlCommand createDbCommand = new SqlCommand(batches[0], masterConnection))
                        {
                            createDbCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        // 如果数据库已存在，会抛出异常，但可以忽略这个异常
                        Console.WriteLine("创建数据库时出现消息: " + ex.Message);
                    }
                }

                // 执行后续的表创建脚本（使用StockManagementDB连接）
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        dbConnection.Open();
                        // 从第二个批次开始执行，因为第一个批次是创建数据库
                        for (int i = 1; i < batches.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(batches[i]))
                            {
                                using (SqlCommand tableCommand = new SqlCommand(batches[i], dbConnection))
                                {
                                    try
                                    {
                                        tableCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"执行第{i + 1}个批次脚本时发生错误: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"连接到StockManagementDB数据库时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("找不到数据库初始化脚本文件。");
            }
        }
    }
}