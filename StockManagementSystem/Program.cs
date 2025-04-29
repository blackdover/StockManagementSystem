using System;
using System.Windows.Forms;
using StockManagementSystem.Data;

namespace StockManagementSystem
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 初始化数据库连接字符串
            SqlHelper.InitConnString("StockDbConnection");

            // 初始化数据库（如果需要）
            DatabaseInitializer.InitializeDatabase();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
