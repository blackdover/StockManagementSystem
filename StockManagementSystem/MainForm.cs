using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockManagementSystem.Forms;
using StockManagementSystem.Models;
using StockManagementSystem.Services;

namespace StockManagementSystem
{
    public partial class MainForm : Form
    {
        private readonly StockService _stockService;
        private readonly StockPriceService _stockPriceService;

        public MainForm()
        {
            InitializeComponent();
            _stockService = new StockService();
            _stockPriceService = new StockPriceService();

            // 绑定菜单和工具栏按钮事件
            添加股票ToolStripMenuItem.Click += btnAddStock_Click;
            编辑股票ToolStripMenuItem.Click += btnEditStock_Click;
            删除股票ToolStripMenuItem.Click += btnDeleteStock_Click;
            查看行情ToolStripMenuItem.Click += btnViewStockPrice_Click;
            添加行情ToolStripMenuItem.Click += btnAddStockPrice_Click;
            行情分析ToolStripMenuItem.Click += btnAnalyzeStockPrice_Click;
            退出ToolStripMenuItem.Click += (s, e) => Close();

            toolStripButtonAddStock.Click += btnAddStock_Click;
            toolStripButtonEditStock.Click += btnEditStock_Click;
            toolStripButtonDeleteStock.Click += btnDeleteStock_Click;
            toolStripButtonViewPrice.Click += btnViewStockPrice_Click;
            toolStripButtonAddPrice.Click += btnAddStockPrice_Click;
            toolStripButtonAnalyze.Click += btnAnalyzeStockPrice_Click;
            toolStripButtonPrint.Click += btnPrintReport_Click;

            // 绑定ListView选择事件
            listViewStocks.SelectedIndexChanged += listViewStocks_SelectedIndexChanged;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 初始化界面
            LoadStockData();
        }

        /// <summary>
        /// 加载股票数据
        /// </summary>
        private void LoadStockData()
        {
            try
            {
                listViewStocks.Items.Clear();
                var stocks = _stockService.GetAllStocks();
                
                foreach (var stock in stocks)
                {
                    var item = new ListViewItem(stock.StockId.ToString());
                    item.SubItems.Add(stock.Code);
                    item.SubItems.Add(stock.Name);
                    item.SubItems.Add(stock.Type);
                    item.SubItems.Add(stock.Industry);
                    item.Tag = stock;
                    
                    listViewStocks.Items.Add(item);
                }

                toolStripStatusLabel1.Text = $"共加载 {stocks.Count} 支股票";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载股票数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        private void listViewStocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count > 0)
            {
                var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
                if (selectedStock != null)
                {
                    LoadStockPrices(selectedStock.StockId);
                }
            }
        }

        /// <summary>
        /// 加载股票行情数据
        /// </summary>
        private void LoadStockPrices(int stockId)
        {
            try
            {
                var prices = _stockPriceService.GetStockPricesByStockId(stockId);
                
                // 创建DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("行情ID", typeof(int));
                dt.Columns.Add("交易日期", typeof(string));
                dt.Columns.Add("开盘价", typeof(decimal));
                dt.Columns.Add("收盘价", typeof(decimal));
                dt.Columns.Add("最高价", typeof(decimal));
                dt.Columns.Add("最低价", typeof(decimal));
                dt.Columns.Add("成交量", typeof(long));
                dt.Columns.Add("成交额", typeof(decimal));
                dt.Columns.Add("涨跌幅", typeof(string));

                foreach (var price in prices)
                {
                    dt.Rows.Add(
                        price.PriceId,
                        price.TradeDate.ToString("yyyy-MM-dd"),
                        price.OpenPrice,
                        price.ClosePrice,
                        price.HighPrice,
                        price.LowPrice,
                        price.Volume,
                        price.Amount,
                        price.ChangePercent + "%"
                    );
                }

                dataGridViewPrices.DataSource = dt;
                dataGridViewPrices.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载行情数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 添加股票信息
        /// </summary>
        private void btnAddStock_Click(object sender, EventArgs e)
        {
            using (var form = new StockEditForm(_stockService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStockData();
                }
            }
        }

        /// <summary>
        /// 编辑股票信息
        /// </summary>
        private void btnEditStock_Click(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要编辑的股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
            using (var form = new StockEditForm(_stockService, selectedStock))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStockData();
                }
            }
        }

        /// <summary>
        /// 删除股票信息
        /// </summary>
        private void btnDeleteStock_Click(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要删除的股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
            var result = MessageBox.Show($"确定要删除股票 {selectedStock.Name} 吗？此操作将同时删除该股票的所有行情数据！", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                bool success = _stockService.DeleteStock(selectedStock.StockId);
                if (success)
                {
                    MessageBox.Show("删除成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStockData();
                    dataGridViewPrices.DataSource = null;
                }
                else
                {
                    MessageBox.Show("删除失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 查看股票行情
        /// </summary>
        private void btnViewStockPrice_Click(object sender, EventArgs e)
        {
            using (var form = new StockPriceQueryForm(_stockService, _stockPriceService))
            {
                form.ShowDialog();
            }
        }

        /// <summary>
        /// 添加股票行情
        /// </summary>
        private void btnAddStockPrice_Click(object sender, EventArgs e)
        {
            int? selectedStockId = null;
            if (listViewStocks.SelectedItems.Count > 0)
            {
                var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
                selectedStockId = selectedStock.StockId;
            }

            using (var form = new StockPriceEditForm(_stockService, _stockPriceService, null, selectedStockId))
            {
                if (form.ShowDialog() == DialogResult.OK && selectedStockId.HasValue)
                {
                    LoadStockPrices(selectedStockId.Value);
                }
            }
        }

        /// <summary>
        /// 统计股票行情
        /// </summary>
        private void btnAnalyzeStockPrice_Click(object sender, EventArgs e)
        {
            using (var form = new StockPriceQueryForm(_stockService, _stockPriceService))
            {
                form.ShowDialog();
            }
        }

        /// <summary>
        /// 打印行情报表
        /// </summary>
        private void btnPrintReport_Click(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要打印报表的股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
            
            // 打印预览对话框
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // 此处应实现实际的打印功能
                // 在实际应用中，可使用第三方库如iTextSharp或ReportViewer
                MessageBox.Show($"正在打印 {selectedStock.Name} 的行情报表...", "打印", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 释放资源
            _stockService.Dispose();
            _stockPriceService.Dispose();
        }
    }
}
