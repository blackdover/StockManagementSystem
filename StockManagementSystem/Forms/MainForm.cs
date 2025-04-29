using System;
using System.Data;
using System.Drawing;
using System.Linq;
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

            // 绑定工具栏按钮事件
            toolStripButtonAddStock.Click += btnAddStock_Click;
            toolStripButtonEditStock.Click += btnEditStock_Click;
            toolStripButtonDeleteStock.Click += btnDeleteStock_Click;
            toolStripButtonViewPrice.Click += btnViewStockPrice_Click;
            toolStripButtonFilter.Click += btnStockFilter_Click;
            toolStripButtonDataIO.Click += btnDataIO_Click;

            // 绑定ListView选择事件
            listViewStocks.SelectedIndexChanged += listViewStocks_SelectedIndexChanged;
            
            // 绑定窗口大小改变事件，自动调整列宽
            this.SizeChanged += MainForm_SizeChanged;
            this.Resize += MainForm_Resize;

            // 初始化股票图表
            InitializeStockChart();
        }

        /// <summary>
        /// 窗口大小改变时自动调整列宽
        /// </summary>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            AdjustColumnWidths();
        }

        /// <summary>
        /// 窗口大小改变时自动调整列宽
        /// </summary>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            AdjustColumnWidths();
        }

        /// <summary>
        /// 调整列表视图列宽
        /// </summary>
        private void AdjustColumnWidths()
        {
            if (listViewStocks.Columns.Count == 0) return;

            // 获取ListView的可见宽度（减去滚动条宽度）
            int totalWidth = listViewStocks.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            
            // 定义各列的相对宽度比例
            int[] columnRatios = new int[] { 5, 10, 15, 10, 15, 45 }; // ID, 代码, 名称, 类型, 行业, 描述
            int totalRatio = columnRatios.Sum();
            
            // 根据比例计算并设置各列宽度
            for (int i = 0; i < listViewStocks.Columns.Count; i++)
            {
                if (i < columnRatios.Length)
                {
                    listViewStocks.Columns[i].Width = (int)(totalWidth * columnRatios[i] / totalRatio);
                }
            }
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

            // 自动选择第一个股票
            if (listViewStocks.Items.Count > 0)
            {
                listViewStocks.Items[0].Selected = true;
                // 确保选中项可见
                listViewStocks.EnsureVisible(0);
            }
            
            // 调整列宽以填满窗口
            AdjustColumnWidths();
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

                // 创建成交量柱形图（使用第二个Y轴）
                var volumeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 成交量");
                volumeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                volumeSeries.Color = Color.LightGray;
                volumeSeries.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
                // 设置柱状图宽度，避免太宽遮挡
                volumeSeries["PointWidth"] = "0.6";

                // 创建收盘价线图
                var closeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 收盘价");
                closeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                closeSeries.Color = Color.Blue;
                closeSeries.BorderWidth = 2;
                // 设置线图显示在柱状图前面
                closeSeries["DrawingStyle"] = "Cylinder";

                // 添加数据点
                for (int i = 0; i < prices.Count; i++)
                {
                    var price = prices[i];
                    var date = price.TradeDate.ToShortDateString();
                    volumeSeries.Points.AddXY(date, price.Volume);
                    closeSeries.Points.AddXY(date, price.ClosePrice);
                }

                // 添加数据系列到图表 - 先添加成交量系列，后添加收盘价系列，确保收盘价线图显示在最上层
                stockChart.Series.Add(volumeSeries);
                stockChart.Series.Add(closeSeries);

                // 配置图表区域
                var chartArea = stockChart.ChartAreas[0];

                // 设置X轴
                chartArea.AxisX.Title = "日期";
                chartArea.AxisX.LabelStyle.Angle = -30; // 斜角显示日期，节省空间
                chartArea.AxisX.LabelStyle.Font = new Font("Microsoft YaHei UI", 8);
                chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
                chartArea.AxisX.Interval = Math.Max(1, prices.Count / 10); // 动态调整X轴标签间隔

                // 设置主Y轴（收盘价）
                chartArea.AxisY.Title = "价格";
                chartArea.AxisY.LabelStyle.Format = "C2";
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

                // 获取收盘价的最大最小值，计算合适的Y轴范围
                decimal minPrice = prices.Min(p => p.LowPrice);
                decimal maxPrice = prices.Max(p => p.HighPrice);
                decimal priceRange = maxPrice - minPrice;
                // 设置Y轴范围，留有10%的余量
                chartArea.AxisY.Minimum = (double)Math.Max(0, minPrice - priceRange * 0.1m);
                chartArea.AxisY.Maximum = (double)(maxPrice + priceRange * 0.1m);

                // 配置第二个Y轴（成交量）
                chartArea.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
                chartArea.AxisY2.Title = "成交量";
                chartArea.AxisY2.LabelStyle.Format = "N0";
                chartArea.AxisY2.MajorGrid.Enabled = false; // 禁用第二Y轴的网格线，避免混淆

                // 获取成交量的最大值，计算合适的Y2轴范围
                long maxVolume = prices.Max(p => p.Volume);
                // 设置Y2轴范围，留有20%的余量
                chartArea.AxisY2.Minimum = 0;
                chartArea.AxisY2.Maximum = (double)(maxVolume * 1.2);

                // 优化图表外观
                chartArea.BackColor = Color.White;
                chartArea.BorderColor = Color.LightGray;
                chartArea.BorderWidth = 1;

                // 设置内边距，确保所有元素都在视图内
                chartArea.InnerPlotPosition.Auto = false;
                chartArea.InnerPlotPosition.X = 10;
                chartArea.InnerPlotPosition.Y = 5;
                chartArea.InnerPlotPosition.Width = 85;
                chartArea.InnerPlotPosition.Height = 85;

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

                    // 清空图表
                    stockChart.Series.Clear();

                    // 如果列表中还有股票，则选中第一个
                    if (listViewStocks.Items.Count > 0)
                    {
                        listViewStocks.Items[0].Selected = true;
                        listViewStocks.EnsureVisible(0);
                    }
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
        /// 启动股票筛选
        /// </summary>
        private void btnStockFilter_Click(object sender, EventArgs e)
        {
            using (var filterForm = new StockFilterForm())
            {
                if (filterForm.ShowDialog() == DialogResult.OK)
                {
                    // 如果用户在筛选表单中选择了股票，更新当前选中股票
                    if (filterForm.SelectedStock != null)
                    {
                        // 查找并选中对应的股票
                        foreach (ListViewItem item in listViewStocks.Items)
                        {
                            if (item.Tag is Stock stock && stock.StockId == filterForm.SelectedStock.StockId)
                            {
                                item.Selected = true;
                                item.EnsureVisible();
                                break;
                            }
                        }
                    }
                }

                // 检查是否有删除操作，如果有则刷新数据
                if (filterForm.HasDeletedRecords)
                {
                    LoadStockData();
                }
            }
        }

        /// <summary>
        /// 启动数据导入导出功能
        /// </summary>
        private void btnDataIO_Click(object sender, EventArgs e)
        {
            using (var dataIOForm = new DataIOForm())
            {
                if (dataIOForm.ShowDialog() == DialogResult.OK)
                {
                    // 刷新股票数据
                    LoadStockData();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 释放资源
            _stockService.Dispose();
            _stockPriceService.Dispose();
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {

        }
    }
}
