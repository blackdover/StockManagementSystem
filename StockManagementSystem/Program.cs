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
            // 数据库连接字符串
            SqlHelper.InitConnString("Data Source=.;Initial Catalog=StockManagementDB;Integrated Security=True;MultipleActiveResultSets=True");

            // 初始化数据库
            DatabaseInitializer.InitializeDatabase();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
