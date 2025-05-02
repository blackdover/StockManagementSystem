using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StockManagementSystem.Data
{
    // 数据库访问和初始化工具类
    public class SqlHelper
    {
        // 数据库连接字符串
        private static string connectionString = "Data Source=.;Initial Catalog=StockManagementDB;Integrated Security=True;MultipleActiveResultSets=True";

        // 数据库创建脚本
        private static readonly string[] createDatabaseScripts = new string[]
        {
            // 创建数据库脚本
            @"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'StockManagementDB')
            BEGIN
                CREATE DATABASE StockManagementDB;
            END",

            // 使用数据库脚本
            @"USE StockManagementDB;",

            // 创建股票表脚本
            @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stocks')
            BEGIN
                CREATE TABLE Stocks (
                    StockId INT PRIMARY KEY IDENTITY(1,1),
                    Code NVARCHAR(10) NOT NULL,
                    Name NVARCHAR(50) NOT NULL,
                    Type NVARCHAR(20) NULL,
                    Industry NVARCHAR(50) NULL,
                    ListingDate DATETIME NOT NULL,
                    Description NVARCHAR(MAX) NULL,
                    CreateTime DATETIME NOT NULL,
                    UpdateTime DATETIME NOT NULL
                );
            END",

            // 创建股票价格表脚本
            @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StockPrices')
            BEGIN
                CREATE TABLE StockPrices (
                    PriceId INT PRIMARY KEY IDENTITY(1,1),
                    StockId INT NOT NULL,
                    TradeDate DATETIME NOT NULL,
                    OpenPrice DECIMAL(18,4) NOT NULL,
                    ClosePrice DECIMAL(18,4) NOT NULL,
                    HighPrice DECIMAL(18,4) NOT NULL,
                    LowPrice DECIMAL(18,4) NOT NULL,
                    Volume BIGINT NOT NULL,
                    Amount DECIMAL(18,4) NOT NULL,
                    ChangePercent DECIMAL(18,4) NOT NULL,
                    CreateTime DATETIME NOT NULL,
                    CONSTRAINT FK_StockPrices_Stocks FOREIGN KEY (StockId) REFERENCES Stocks(StockId)
                );
            END",

            // 创建股票代码唯一索引
            @"IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Stocks_Code' AND object_id = OBJECT_ID('Stocks'))
            BEGIN
                CREATE UNIQUE INDEX IX_Stocks_Code ON Stocks(Code);
            END",

            // 创建股票价格复合索引
            @"IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StockPrices_StockId_TradeDate' AND object_id = OBJECT_ID('StockPrices'))
            BEGIN
                CREATE INDEX IX_StockPrices_StockId_TradeDate ON StockPrices(StockId, TradeDate);
            END"
        };

        // 初始化连接字符串
        public static void InitConnString(string connStr)
        {
            connectionString = connStr;
        }

        // 获取当前连接字符串
        public static string GetConnectionString()
        {
            return connectionString;
        }

        // 初始化数据库
        public static void InitializeDatabase()
        {
            try
            {
                // 使用master数据库连接
                string masterConnectionString = connectionString.Replace("Initial Catalog=StockManagementDB;", "Initial Catalog=master;");

                // 在master中执行数据库创建语句
                using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
                {
                    try
                    {
                        masterConnection.Open();
                        using (SqlCommand createDbCommand = new SqlCommand(createDatabaseScripts[0], masterConnection))
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
                        // 从第二个脚本开始执行，因为第一个脚本是创建数据库
                        for (int i = 1; i < createDatabaseScripts.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(createDatabaseScripts[i]))
                            {
                                using (SqlCommand tableCommand = new SqlCommand(createDatabaseScripts[i], dbConnection))
                                {
                                    try
                                    {
                                        tableCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"执行第{i + 1}个脚本时发生错误: " + ex.Message);
                                    }
                                }
                            }
                        }
                        Console.WriteLine("数据库初始化成功！");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"连接到StockManagementDB数据库时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据库初始化过程中发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 执行SQL语句，返回影响的行数
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

        // 执行SQL查询，返回DataTable
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

        // 执行查询，返回第一行第一列的值
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

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns>数据库连接字符串</returns>
        public static string GetConnectionString()
        {
            return connectionString;
        }
    }
}