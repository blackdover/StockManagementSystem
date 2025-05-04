using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StockManagementSystem.Forms;
using StockManagementSystem.Models;
using StockManagementSystem.Services;
using System.Collections.Generic;
using System.Threading;

namespace StockManagementSystem
{
    public partial class MainForm : Form
    {
        // 股票和价格服务类实例
        private readonly StockService _stockService;
        private readonly StockPriceService _stockPriceService;
        // 股票图表控件
        private System.Windows.Forms.DataVisualization.Charting.Chart stockChart;

        // 股票价格数据缓存，提高性能
        private Dictionary<int, List<StockPrice>> _priceCache = new Dictionary<int, List<StockPrice>>();
        // 当前选中的股票ID
        private int _currentStockId = -1;
        // 异步操作的取消令牌源
        private System.Threading.CancellationTokenSource _cancellationTokenSource;

        public MainForm()
        {
            InitializeComponent();
            // 初始化服务实例
            _stockService = new StockService();
            _stockPriceService = new StockPriceService();

            // 注册窗口大小变化事件
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
            // 检查是否有列需要调整
            if (listViewStocks.Columns.Count == 0) return;

            // 计算可用总宽度，减去垂直滚动条宽度
            int totalWidth = listViewStocks.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;

            // 设置各列的宽度比例
            int[] columnRatios = new int[] { 5, 10, 15, 10, 15, 45 };
            int totalRatio = columnRatios.Sum();
            for (int i = 0; i < listViewStocks.Columns.Count; i++)
            {
                if (i < columnRatios.Length)
                {
                    // 根据比例分配列宽
                    listViewStocks.Columns[i].Width = (int)(totalWidth * columnRatios[i] / totalRatio);
                }
            }
        }

        /// <summary>
        /// 初始化股票图表
        /// </summary>
        private void InitializeStockChart()
        {
            // 创建图表控件实例
            stockChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            stockChart.Dock = DockStyle.Fill;
            // 创建图表区域
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("股票价格");
            stockChart.ChartAreas.Add(chartArea);
            // 设置坐标轴标题和格式
            chartArea.AxisX.Title = "日期";
            chartArea.AxisY.Title = "价格";
            chartArea.AxisY.LabelStyle.Format = "C2";
            // 清除并添加图表到分割面板的第二个区域
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(stockChart);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 窗体加载时读取股票数据
            LoadStockData();

            // 如果有数据，自动选中第一项
            if (listViewStocks.Items.Count > 0)
            {
                listViewStocks.Items[0].Selected = true;
                listViewStocks.EnsureVisible(0);
            }

            // 调整列宽以适应窗口大小
            AdjustColumnWidths();
        }

        /// <summary>
        /// 加载股票数据
        /// </summary>
        private void LoadStockData()
        {
            try
            {
                // 清除价格缓存以重新加载最新数据
                ClearPriceCache();
                // 清空股票列表并获取所有股票数据
                listViewStocks.Items.Clear();
                var stocks = _stockService.GetAllStocks();
                foreach (var stock in stocks)
                {
                    // 创建列表项并添加股票信息
                    var item = new ListViewItem(stock.StockId.ToString());
                    item.SubItems.Add(stock.Code);
                    item.SubItems.Add(stock.Name);
                    item.SubItems.Add(stock.Type);
                    item.SubItems.Add(stock.Industry);
                    item.SubItems.Add(stock.Description);
                    // 使用Tag存储完整的股票对象以便后续使用
                    item.Tag = stock;
                    listViewStocks.Items.Add(item);
                }
                // 更新状态栏显示数据加载情况
                toolStripStatusLabel1.Text = $"共加载 {stocks.Count} 支股票";
            }
            catch (Exception ex)
            {
                // 捕获并显示加载过程中的错误
                MessageBox.Show($"加载股票数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        private void listViewStocks_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count > 0)
            {
                // 从选中项的Tag属性获取完整的股票对象
                var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
                if (selectedStock != null)
                {
                    // 异步加载所选股票的价格数据
                    LoadStockPriceDataAsync(selectedStock.StockId);
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
                List<StockPrice> prices;
                // 获取股票基本信息
                var stock = _stockService.GetStockById(stockId);
                // 检查缓存中是否已有该股票的价格数据
                if (_currentStockId == stockId && _priceCache.ContainsKey(stockId))
                {
                    prices = _priceCache[stockId];
                }
                else
                {
                    // 从数据库加载价格数据
                    prices = _stockPriceService.GetStockPricesByStockId(stockId);
                    // 将结果存入缓存并更新当前股票ID
                    _priceCache[stockId] = prices;
                    _currentStockId = stockId;
                }
                // 如果没有价格数据，显示无数据提示
                if (prices.Count == 0)
                {
                    // 清除现有图表元素
                    stockChart.Series.Clear();
                    stockChart.Titles.Clear();
                    // 重置坐标轴设置
                    stockChart.ChartAreas[0].AxisX.Title = "";
                    stockChart.ChartAreas[0].AxisY.Title = "";
                    stockChart.ChartAreas[0].AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
                    // 添加无数据提示标题
                    var noDataTitle = new System.Windows.Forms.DataVisualization.Charting.Title("暂无该股票的行情数据");
                    noDataTitle.Font = new Font("Microsoft YaHei UI", 12, FontStyle.Regular);
                    noDataTitle.ForeColor = Color.Gray;
                    stockChart.Titles.Add(noDataTitle);
                    // 刷新图表
                    stockChart.Invalidate();
                    return;
                }
                // 对价格数据按日期排序
                prices = prices.OrderBy(p => p.TradeDate).ToList();
                // 数据采样以提高图表性能
                int sampleRate = Math.Max(1, prices.Count / 200);
                List<StockPrice> sampledPrices = new List<StockPrice>();
                // 按采样率抽取数据点
                for (int i = 0; i < prices.Count; i += sampleRate)
                {
                    sampledPrices.Add(prices[i]);
                }
                prices = sampledPrices;
                // 判断是否需要重建图表系列
                bool needRebuildChart = stockChart.Series.Count == 0 ||
                                       (stockChart.Series.Count > 0 &&
                                        stockChart.Series[0].Name.IndexOf(stock.Name) < 0);
                // 如需重建，则清除现有图表并构建新的图表元素
                if (needRebuildChart)
                {
                    // 清空现有系列和标题
                    stockChart.Series.Clear();
                    stockChart.Titles.Clear();

                    // 创建成交量数据系列
                    var volumeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 成交量");
                    volumeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    volumeSeries.Color = Color.LightGray;
                    volumeSeries.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
                    volumeSeries["PointWidth"] = "0.6";

                    // 创建价格数据系列
                    var closeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 收盘价");
                    closeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    closeSeries.Color = Color.Blue;
                    closeSeries.BorderWidth = 2;
                    closeSeries["DrawingStyle"] = "Cylinder";

                    // 添加系列到图表
                    stockChart.Series.Add(volumeSeries);
                    stockChart.Series.Add(closeSeries);

                    // 获取图表区域引用
                    var chartArea = stockChart.ChartAreas[0];

                    // 配置X轴样式
                    chartArea.AxisX.LabelStyle.Font = new Font("Microsoft YaHei UI", 7);
                    chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;

                    // 配置主Y轴（价格）样式
                    chartArea.AxisY.Title = "价格";
                    chartArea.AxisY.LabelStyle.Format = "C2";
                    chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

                    // 配置次Y轴（成交量）样式
                    chartArea.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
                    chartArea.AxisY2.Title = "成交量";
                    chartArea.AxisY2.LabelStyle.Format = "N0";
                    chartArea.AxisY2.MajorGrid.Enabled = false;

                    // 设置图表区域背景和边框
                    chartArea.BackColor = Color.White;
                    chartArea.BorderColor = Color.LightGray;
                    chartArea.BorderWidth = 1;

                    // 调整图表内部绘图区域位置
                    chartArea.InnerPlotPosition.Auto = false;
                    chartArea.InnerPlotPosition.X = 10;
                    chartArea.InnerPlotPosition.Y = 5;
                    chartArea.InnerPlotPosition.Width = 85;
                    chartArea.InnerPlotPosition.Height = 85;

                    // 添加图表标题
                    var title = new System.Windows.Forms.DataVisualization.Charting.Title($"{stock.Name}({stock.Code}) 股票价格走势");
                    title.Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold);
                    stockChart.Titles.Add(title);
                }
                else
                {
                    // 如果不需要重建，只清除现有数据点
                    stockChart.Series[0].Points.Clear();
                    stockChart.Series[1].Points.Clear();
                }

                // 添加数据点到图表系列
                for (int i = 0; i < prices.Count; i++)
                {
                    var price = prices[i];
                    var date = price.TradeDate.ToShortDateString();
                    // 添加成交量数据
                    stockChart.Series[0].Points.AddXY(date, price.Volume);
                    // 添加收盘价数据
                    stockChart.Series[1].Points.AddXY(date, price.ClosePrice);
                }

                // 调整X轴刻度间隔，控制标签密度
                stockChart.ChartAreas[0].AxisX.Interval = Math.Max(1, prices.Count / 10);

                // 计算价格Y轴范围，并留出上下边距
                decimal minPrice = prices.Min(p => p.LowPrice);
                decimal maxPrice = prices.Max(p => p.HighPrice);
                decimal priceRange = maxPrice - minPrice;
                stockChart.ChartAreas[0].AxisY.Minimum = (double)Math.Max(0, minPrice - priceRange * 0.1m);
                stockChart.ChartAreas[0].AxisY.Maximum = (double)(maxPrice + priceRange * 0.1m);

                // 计算成交量Y轴范围
                long maxVolume = prices.Max(p => p.Volume);
                stockChart.ChartAreas[0].AxisY2.Minimum = 0;
                stockChart.ChartAreas[0].AxisY2.Maximum = (double)(maxVolume * 1.2);

                // 刷新图表显示
                stockChart.Invalidate();
            }
            catch (Exception ex)
            {
                // 显示图表绘制过程中的错误
                MessageBox.Show($"绘制股票图表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 添加股票信息
        /// </summary>
        private void toolStripButtonAddStock_Click(object sender, EventArgs e)
        {
            // 创建股票编辑表单实例
            using (var form = new StockEditForm(_stockService))
            {
                // 显示对话框，如果用户点击确定则重新加载数据
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStockData();
                }
            }
        }

        /// <summary>
        /// 编辑股票信息
        /// </summary>
        private void toolStripButtonEditStock_Click(object sender, EventArgs e)
        {
            // 检查是否有选中的股票
            if (listViewStocks.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要编辑的股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 获取选中的股票对象
            var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
            // 创建编辑表单并传入选中的股票信息
            using (var form = new StockEditForm(_stockService, selectedStock))
            {
                // 如果用户保存了更改，则重新加载数据
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStockData();
                }
            }
        }

        /// <summary>
        /// 删除股票信息
        /// </summary>
        private void toolStripButtonDeleteStock_Click(object sender, EventArgs e)
        {
            // 检查是否有选中的股票
            if (listViewStocks.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要删除的股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 获取选中的股票并显示确认对话框
            var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
            var result = MessageBox.Show($"确定要删除股票 {selectedStock.Name} 吗？此操作将同时删除该股票的所有行情数据！", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            // 如果用户确认删除
            if (result == DialogResult.Yes)
            {
                // 调用服务删除股票数据
                bool success = _stockService.DeleteStock(selectedStock.StockId);
                if (success)
                {
                    // 删除成功后的操作
                    MessageBox.Show("删除成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStockData();
                    // 清除图表内容
                    stockChart.Series.Clear();
                    // 如果列表中还有数据，自动选中第一项
                    if (listViewStocks.Items.Count > 0)
                    {
                        listViewStocks.Items[0].Selected = true;
                        listViewStocks.EnsureVisible(0);
                    }
                }
                else
                {
                    // 显示删除失败提示
                    MessageBox.Show("删除失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 查看股票行情
        /// </summary>
        private void toolStripButtonViewPrice_Click(object sender, EventArgs e)
        {
            // 初始化选中的股票ID为空
            int? selectedStockId = null;
            // 如果有选中的股票，获取其ID
            if (listViewStocks.SelectedItems.Count > 0)
            {
                var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
                selectedStockId = selectedStock.StockId;
            }

            // 创建股票价格查询表单
            using (var form = new StockPriceQueryForm(_stockService, _stockPriceService))
            {
                // 如果有预选的股票ID，传给表单
                if (selectedStockId.HasValue)
                {
                    form.PresetStockId = selectedStockId.Value;
                }
                // 显示查询表单
                form.ShowDialog();
            }
        }

        /// <summary>
        /// 启动股票筛选
        /// </summary>
        private void toolStripButtonFilter_Click(object sender, EventArgs e)
        {
            // 创建股票筛选表单
            using (var filterForm = new StockFilterForm())
            {
                // 显示筛选表单，如果用户确认了筛选条件
                if (filterForm.ShowDialog() == DialogResult.OK)
                {
                    // 如果筛选返回了特定股票
                    if (filterForm.SelectedStock != null)
                    {
                        // 清除所有选中状态
                        foreach (ListViewItem item in listViewStocks.Items)
                        {
                            item.Selected = false;
                        }

                        // 查找并选中筛选出的股票
                        bool found = false;
                        foreach (ListViewItem item in listViewStocks.Items)
                        {
                            if (item.Tag is Stock stock && stock.StockId == filterForm.SelectedStock.StockId)
                            {
                                // 选中匹配的项并确保可见
                                item.Selected = true;
                                item.EnsureVisible();
                                found = true;

                                // 绘制该股票的价格图表
                                int stockId = filterForm.SelectedStock.StockId;
                                DrawStockPriceChart(stockId);

                                // 更新状态栏显示
                                toolStripStatusLabel1.Text = $"已选择: {stock.Name}({stock.Code})";

                                break;
                            }
                        }

                        // 如果在当前列表中未找到筛选出的股票
                        if (!found && filterForm.SelectedStock != null)
                        {
                            // 仍然绘制该股票的价格图表
                            int stockId = filterForm.SelectedStock.StockId;
                            DrawStockPriceChart(stockId);

                            // 更新状态栏，提示股票不在当前显示列表中
                            toolStripStatusLabel1.Text = $"已选择ID为{stockId}的股票(未在当前列表中显示)";
                        }
                    }
                }

                // 如果筛选过程中删除了记录，需要重新加载数据
                if (filterForm.HasDeletedRecords)
                {
                    LoadStockData();
                }
            }
        }

        /// <summary>
        /// 启动数据导入导出功能
        /// </summary>
        private void toolStripButtonDataIO_Click(object sender, EventArgs e)
        {
            // 创建数据导入导出表单
            using (var dataIOForm = new DataIOForm())
            {
                // 显示表单，如果用户进行了导入导出操作
                if (dataIOForm.ShowDialog() == DialogResult.OK)
                {
                    // 重新加载数据以反映变化
                    LoadStockData();
                }
            }
        }

        /// <summary>
        /// 异步加载股票价格数据
        /// </summary>
        private async void LoadStockPriceDataAsync(int stockId)
        {
            try
            {
                if (_cancellationTokenSource != null)// 如果有正在进行的异步操作，取消它
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }// 创建新的取消令牌源
                _cancellationTokenSource = new System.Threading.CancellationTokenSource();
                var token = _cancellationTokenSource.Token;
                // 更新状态栏显示加载状态
                toolStripStatusLabel1.Text = "正在加载股票数据...";
                // 在后台线程加载价格数据
                await System.Threading.Tasks.Task.Run(() =>
                {
                    // 如果缓存中没有该股票的价格数据，则从数据库加载
                    if (!_priceCache.ContainsKey(stockId))
                    {
                        // 检查是否请求取消
                        if (token.IsCancellationRequested)
                            return;
                        // 加载并缓存价格数据
                        _priceCache[stockId] = _stockPriceService.GetStockPricesByStockId(stockId);
                    }
                }, token);
                // 如果操作已被取消，直接返回
                if (token.IsCancellationRequested)
                    return;
                // 确认所选股票未改变后绘制图表
                if (listViewStocks.SelectedItems.Count > 0)
                {
                    var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock;
                    if (selectedStock != null && selectedStock.StockId == stockId)
                    {
                        DrawStockPriceChart(stockId);
                    }
                }
                toolStripStatusLabel1.Text = "数据加载完成";
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                // 忽略任务取消异常
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载股票数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 清除价格缓存
        /// </summary>
        private void ClearPriceCache()
        {
            // 清空价格数据字典
            _priceCache.Clear();
            // 重置当前股票ID
            _currentStockId = -1;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果有未取消的异步操作，取消并释放资源
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            // 释放服务实例资源
            _stockService.Dispose();
            _stockPriceService.Dispose();
        }
    }
}