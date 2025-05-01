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

            // 启用视觉样式，使应用程序使用当前Windows主题的视觉样式
            Application.EnableVisualStyles();
            // 设置兼容的文本渲染默认值为false，确保文本渲染的一致性
            Application.SetCompatibleTextRenderingDefault(false);
            // 创建并运行应用程序的主窗体，程序将在此处阻塞直到主窗体关闭
            Application.Run(new MainForm());
        }
    }
}
