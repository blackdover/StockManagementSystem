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

            // 初始化数据库
            DatabaseInitializer.InitializeDatabase();

            // 连接数据库
            SqlHelper.InitConnString("Data Source=.;Initial Catalog=StockManagementDB;Integrated Security=True;MultipleActiveResultSets=True");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
