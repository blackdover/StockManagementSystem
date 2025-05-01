using System;
using System.IO;  // 用于文件操作，如读取SQL脚本文件
using System.Windows.Forms;  // 用于获取应用程序路径（Application.StartupPath）
using System.Data.SqlClient;  // 用于SQL Server数据库连接和操作

namespace StockManagementSystem.Data
{
    /// <summary>
    /// 数据库初始化类
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        /// <remarks>
        /// 使用static关键字是因为这是一个类级别的常量，所有实例共享同一个连接字符串。
        /// 使用readonly关键字是为了确保这个字符串在初始化后不能被修改，类似于C++中的const，
        /// 但有一点区别：readonly只能在声明时或构造函数中赋值，之后不能修改；
        /// 而const必须在声明时初始化，且是编译时常量。
        /// readonly提供了运行时的不可变性，保护连接字符串不被意外修改，增强了安全性。
        /// </remarks>
        private static readonly string connectionString = "Data Source=.;Initial Catalog=StockManagementDB;Integrated Security=True;MultipleActiveResultSets=True";
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <summary>
        /// 使用static关键字是因为这是一个工具方法，不需要创建DatabaseInitializer的实例就可以调用。
        /// 这样可以简化数据库初始化过程，允许在应用程序启动时直接通过类名调用此方法：
        /// DatabaseInitializer.InitializeDatabase()，而不需要先实例化对象。
        /// 这种模式适用于只执行操作而不维护状态的工具方法。
        /// </summary>
        public static void InitializeDatabase()
        {
            string scriptPath = Path.Combine(Application.StartupPath, "Data", "CreateDatabase.sql");//获取执行路径下data文件夹中的sql文件
            if (File.Exists(scriptPath))
            {
                string script = File.ReadAllText(scriptPath);//读取sql文件
                // 方式1：使用string[]声明字符串数组
                string[] batches = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);//将sql文件按GO分割成多个批次，go参数是分割符，StringSplitOptions.RemoveEmptyEntries表示去除空字符串
                
                // 其他声明数组的例子：
                // 方式2：显式声明数组类型
                // string[] batches = script.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                
                // 方式3：使用var关键字（编译器自动推断类型）
                // var batches = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                
                // 方式4：直接初始化数组
                // string[] keywords = new string[] { "SELECT", "FROM", "WHERE" };
                
                // 方式5：简化的数组初始化
                // string[] keywords = new[] { "SELECT", "FROM", "WHERE" };
                
                // 方式6：最简形式的数组初始化
                // string[] keywords = { "SELECT", "FROM", "WHERE" };
                
                // 方式7：指定长度的数组声明
                // int[] numbers = new int[5]; // 创建一个包含5个元素的整数数组


                // 使用master数据库连接
                string masterConnectionString = connectionString.Replace("Initial Catalog=StockManagementDB;", "Initial Catalog=master;");

                // 在master中执行数据库创建语句
                // using语句是C#中的一种资源管理模式，它实现了IDisposable接口的对象自动调用Dispose方法
                // 当代码块执行完毕后，无论是正常执行完成还是发生异常，都会自动调用对象的Dispose方法释放资源
                // 这里使用using确保SqlConnection对象在使用完毕后被正确关闭和释放，避免资源泄漏
                using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
                // SqlConnection是ADO.NET提供的一个类，用于建立和SQL Server数据库的连接
                // 这里创建了一个连接到master数据库的连接对象，用于执行创建数据库的SQL语句
                // masterConnectionString包含了连接到SQL Server所需的信息，如服务器地址、认证方式等
                {
                    try
                    {
                        masterConnection.Open();
                        // 这里的嵌套using同样确保SqlCommand对象在使用后被释放
                        // 相当于在finally块中调用createDbCommand.Dispose()
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
                    // 不需要手动调用masterConnection.Close()，using块结束时会自动调用
                }
                //简单来说就是using会自动释放链接

                // 执行后续的表创建脚本（使用StockManagementDB连接）
                //第一个using是创建数据库，第二个using是创建表，确保创建好数据库后切换连接到该数据库，然后开始创建表
                // 创建一个连接到StockManagementDB数据库的连接对象
                // 使用using语句确保连接在使用完毕后自动关闭和释放资源
                using (SqlConnection dbConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        // 打开数据库连接
                        dbConnection.Open();
                        // 从第二个批次开始执行，因为第一个批次是创建数据库
                        for (int i = 1; i < batches.Length; i++)
                        {
                            // 检查当前批次是否有有效内容
                            if (!string.IsNullOrWhiteSpace(batches[i]))
                            {
                                // 为每个SQL批次创建一个命令对象
                                using (SqlCommand tableCommand = new SqlCommand(batches[i], dbConnection))
                                {
                                    try
                                    {
                                        // 执行非查询SQL命令（如CREATE TABLE, CREATE INDEX等）
                                        tableCommand.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        // 捕获并记录每个批次执行时可能发生的错误
                                        // 继续执行下一个批次，不中断整个过程
                                        Console.WriteLine($"执行第{i + 1}个批次脚本时发生错误: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 如果连接数据库失败，显示错误消息
                        // MessageBox是Windows Forms中的消息框组件
                        // 在Qt中，等效的实现是：
                        // QMessageBox::critical(nullptr, "错误", 
                        //     QString("连接到StockManagementDB数据库时发生错误: %1").arg(ex.Message),
                        //     QMessageBox::Ok);
                        MessageBox.Show($"连接到StockManagementDB数据库时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // 不需要手动关闭连接，using块结束时会自动调用Close()和Dispose()
                }
            }
            else
            {
                // 如果找不到SQL脚本文件，显示错误消息
                MessageBox.Show("找不到数据库初始化脚本文件。");
            }
        }
    }
}