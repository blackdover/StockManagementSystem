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
        private System.Windows.Forms.DataVisualization.Charting.Chart stockChart;

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
            退出ToolStripMenuItem.Click += (s, e) => Close();

            toolStripButtonAddStock.Click += btnAddStock_Click;
            toolStripButtonEditStock.Click += btnEditStock_Click;
            toolStripButtonDeleteStock.Click += btnDeleteStock_Click;
            toolStripButtonViewPrice.Click += btnViewStockPrice_Click;
            toolStripButtonAddPrice.Click += btnAddStockPrice_Click;

            // 绑定ListView选择事件
            listViewStocks.SelectedIndexChanged += listViewStocks_SelectedIndexChanged;

            // 初始化股票图表
            InitializeStockChart();
        }

        /// <summary>
        /// 初始化股票图表
        /// </summary>
        private void InitializeStockChart()
        {
            // 创建图表控件
            stockChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            stockChart.Dock = DockStyle.Fill;

            // 创建图表区域
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("股票价格");
            stockChart.ChartAreas.Add(chartArea);

            // 添加图例
            stockChart.Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend("Legend"));

            // 配置坐标轴
            chartArea.AxisX.Title = "日期";
            chartArea.AxisY.Title = "价格";
            chartArea.AxisY.LabelStyle.Format = "C2";

            // 替换下方数据面板为图表
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(stockChart);
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
                    item.SubItems.Add(stock.Description);
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
                    DrawStockPriceChart(selectedStock.StockId);
                }
            }
        }

        /// <summary>
        /// 绘制股票价格图表
        /// </summary>
        private void DrawStockPriceChart(int stockId)
        {
            try
            {
                var stock = _stockService.GetStockById(stockId);
                var prices = _stockPriceService.GetStockPricesByStockId(stockId);

                if (prices.Count == 0)
                {
                    // 完全清空图表
                    stockChart.Series.Clear();
                    stockChart.Titles.Clear();

                    // 重置坐标轴设置
                    stockChart.ChartAreas[0].AxisX.Title = "";
                    stockChart.ChartAreas[0].AxisY.Title = "";
                    stockChart.ChartAreas[0].AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;

                    // 添加提示文本
                    var noDataTitle = new System.Windows.Forms.DataVisualization.Charting.Title("暂无该股票的行情数据");
                    noDataTitle.Font = new Font("Microsoft YaHei UI", 12, FontStyle.Regular);
                    noDataTitle.ForeColor = Color.Gray;
                    stockChart.Titles.Add(noDataTitle);

                    // 刷新图表
                    stockChart.Invalidate();
                    return;
                }

                // 排序价格数据（按日期升序）
                prices = prices.OrderBy(p => p.TradeDate).ToList();

                // 清除现有数据
                stockChart.Series.Clear();
                stockChart.Titles.Clear();

                // 创建收盘价线图
                var closeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 收盘价");
                closeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                closeSeries.Color = Color.Blue;
                closeSeries.BorderWidth = 2;

                // 创建成交量柱形图（使用第二个Y轴）
                var volumeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 成交量");
                volumeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                volumeSeries.Color = Color.LightGray;
                volumeSeries.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;

                // 添加数据点
                for (int i = 0; i < prices.Count; i++)
                {
                    var price = prices[i];
                    var date = price.TradeDate.ToShortDateString();
                    closeSeries.Points.AddXY(date, price.ClosePrice);
                    volumeSeries.Points.AddXY(date, price.Volume);
                }

                // 添加数据系列到图表
                stockChart.Series.Add(closeSeries);
                stockChart.Series.Add(volumeSeries);

                // 配置第二个Y轴
                stockChart.ChartAreas[0].AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
                stockChart.ChartAreas[0].AxisY2.Title = "成交量";
                stockChart.ChartAreas[0].AxisY2.LabelStyle.Format = "N0";

                // 设置标题
                var title = new System.Windows.Forms.DataVisualization.Charting.Title($"{stock.Name}({stock.Code}) 股票价格走势");
                title.Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold);
                stockChart.Titles.Add(title);

                // 刷新图表
                stockChart.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"绘制股票图表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    stockChart.Series.Clear();
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
            // 获取当前选中的股票ID
            int? selectedStockId = null;
            if (listViewStocks.SelectedItems.Count > 0)
            {
                var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
                selectedStockId = selectedStock.StockId;
            }

            using (var form = new StockPriceQueryForm(_stockService, _stockPriceService))
            {
                // 如果有选中股票，预先设置到查询表单
                if (selectedStockId.HasValue)
                {
                    form.PresetStockId = selectedStockId.Value;
                }
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
                    DrawStockPriceChart(selectedStockId.Value);
                }
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
