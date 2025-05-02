using System;
using System.Windows.Forms;
using StockManagementSystem.Data;

namespace StockManagementSystem
{
    static class Program
    {
        // 应用程序的主入口点
        [STAThread]
        static void Main()
        {
            // 初始化数据库 - 使用内置SQL脚本创建数据库和表
            SqlHelper.InitializeDatabase();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
