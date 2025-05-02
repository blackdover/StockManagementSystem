using System; // 导入基础命名空间，包含基本数据类型和异常处理
using System.Data; // 导入数据处理相关命名空间，提供数据表、行、列等数据结构
using System.Drawing; // 导入绘图相关命名空间，提供颜色、字体等图形功能
using System.Linq; // 导入LINQ命名空间，提供查询语法支持
using System.Windows.Forms; // 导入Windows窗体相关命名空间，提供UI控件和事件
using StockManagementSystem.Forms; // 导入自定义窗体命名空间
using StockManagementSystem.Models; // 导入数据模型命名空间
using StockManagementSystem.Services; // 导入服务层命名空间
using System.Collections.Generic; // 导入集合命名空间
using System.Threading; // 导入线程命名空间

namespace StockManagementSystem // 定义命名空间
{
    public partial class MainForm : Form // 定义主窗体类，继承自Form基类，使用partial关键字表示类定义分布在多个文件中
    // C#的继承语法使用冒号(:)表示，格式为"子类 : 父类"
    // 这里MainForm是子类，Form是父类，MainForm继承了Form类的所有公共和受保护成员
    // C#只支持单继承，一个类只能有一个直接父类，但可以实现多个接口(接口也使用冒号，如Class : Interface1, Interface2)
    // 使用继承后，子类可以重写父类的虚方法(virtual)和抽象方法(abstract)，使用override关键字
    // 子类构造函数可以通过base关键字调用父类构造函数
    // partial关键字允许将类定义拆分到多个文件中，常用于分离设计器生成的代码和用户代码
    {
        private readonly StockService _stockService; // 声明股票服务私有只读字段，用于处理股票数据
        private readonly StockPriceService _stockPriceService; // 声明股票价格服务私有只读字段，用于处理股票价格数据
        private System.Windows.Forms.DataVisualization.Charting.Chart stockChart; // 声明图表控件私有字段，用于显示股票价格走势

        // 缓存机制相关变量
        private Dictionary<int, List<StockPrice>> _priceCache = new Dictionary<int, List<StockPrice>>();
        private int _currentStockId = -1;
        private System.Threading.CancellationTokenSource _cancellationTokenSource;

        public MainForm() // 主窗体构造函数
        {
            InitializeComponent(); // 初始化窗体组件，这个方法通常由设计器自动生成
            _stockService = new StockService(); // 实例化股票服务对象
            _stockPriceService = new StockPriceService(); // 实例化股票价格服务对象

            // 绑定窗口大小改变事件，自动调整列宽
            this.SizeChanged += MainForm_SizeChanged; // 窗口大小变化事件绑定
            this.Resize += MainForm_Resize; // 窗口调整大小事件绑定

            // 初始化股票图表
            InitializeStockChart(); // 调用初始化图表方法
        }

        /// <summary>
        /// 窗口大小改变时自动调整列宽
        /// </summary>
        private void MainForm_SizeChanged(object sender, EventArgs e) // 窗口大小变化事件处理方法
        {
            AdjustColumnWidths(); // 调用调整列宽方法
        }

        /// <summary>
        /// 窗口大小改变时自动调整列宽
        /// </summary>
        private void MainForm_Resize(object sender, EventArgs e) // 窗口调整大小事件处理方法
        {
            AdjustColumnWidths(); // 调用调整列宽方法
        }

        /// <summary>
        /// 调整列表视图列宽
        /// </summary>
        private void AdjustColumnWidths() // 调整列宽的私有方法
        {
            if (listViewStocks.Columns.Count == 0) return; // 如果列表没有列，直接返回，避免后续操作出错

            // 获取ListView的可见宽度（减去滚动条宽度）
            int totalWidth = listViewStocks.ClientSize.Width - SystemInformation.VerticalScrollBarWidth; // 计算可用总宽度

            // 定义各列的相对宽度比例
            int[] columnRatios = new int[] { 5, 10, 15, 10, 15, 45 }; // ID, 代码, 名称, 类型, 行业, 描述的宽度比例
            int totalRatio = columnRatios.Sum(); // 计算总比例，使用LINQ的Sum方法

            // 根据比例计算并设置各列宽度
            for (int i = 0; i < listViewStocks.Columns.Count; i++) // 遍历所有列
            {
                if (i < columnRatios.Length) // 确保索引在比例数组范围内
                {
                    listViewStocks.Columns[i].Width = (int)(totalWidth * columnRatios[i] / totalRatio); // 根据比例计算列宽
                }
            }
        }

        /// <summary>
        /// 初始化股票图表
        /// </summary>
        private void InitializeStockChart() // 初始化股票图表的私有方法
        {
            // 创建图表控件
            stockChart = new System.Windows.Forms.DataVisualization.Charting.Chart(); // 实例化图表对象
            stockChart.Dock = DockStyle.Fill; // 设置图表填充整个容器

            // 创建图表区域
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("股票价格"); // 创建名为"股票价格"的图表区域
            stockChart.ChartAreas.Add(chartArea); // 将图表区域添加到图表中

            // 添加图例
            stockChart.Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend("Legend")); // 添加图例到图表

            // 配置坐标轴
            chartArea.AxisX.Title = "日期"; // 设置X轴标题
            chartArea.AxisY.Title = "价格"; // 设置Y轴标题
            chartArea.AxisY.LabelStyle.Format = "C2"; // 设置Y轴标签格式为货币，保留2位小数

            // 替换下方数据面板为图表
            this.splitContainer1.Panel2.Controls.Clear(); // 清空下方面板的所有控件
            this.splitContainer1.Panel2.Controls.Add(stockChart); // 将图表添加到下方面板
        }

        private void MainForm_Load(object sender, EventArgs e) // 窗体加载事件处理方法
        {
            // 初始化界面
            LoadStockData(); // 加载股票数据

            // 自动选择第一个股票
            if (listViewStocks.Items.Count > 0) // 检查是否有股票项
            {
                listViewStocks.Items[0].Selected = true; // 选中第一项
                // 确保选中项可见
                listViewStocks.EnsureVisible(0); // 确保第一项在视图中可见
            }

            // 调整列宽以填满窗口
            AdjustColumnWidths(); // 调用调整列宽方法
        }

        /// <summary>
        /// 加载股票数据
        /// </summary>
        private void LoadStockData() // 加载股票数据的私有方法
        {
            try // 尝试执行以下代码块
            {
                // 清除价格缓存，确保获取最新数据
                ClearPriceCache();

                listViewStocks.Items.Clear(); // 清空列表项
                var stocks = _stockService.GetAllStocks(); // 获取所有股票数据

                foreach (var stock in stocks) // 遍历所有股票
                {
                    var item = new ListViewItem(stock.StockId.ToString()); // 创建列表项，第一列为股票ID
                    item.SubItems.Add(stock.Code); // 添加股票代码子项
                    item.SubItems.Add(stock.Name); // 添加股票名称子项
                    item.SubItems.Add(stock.Type); // 添加股票类型子项
                    item.SubItems.Add(stock.Industry); // 添加股票行业子项
                    item.SubItems.Add(stock.Description); // 添加股票描述子项
                    item.Tag = stock; // 将股票对象存储在Tag属性中，便于后续访问

                    listViewStocks.Items.Add(item); // 将列表项添加到列表视图中
                }

                toolStripStatusLabel1.Text = $"共加载 {stocks.Count} 支股票"; // 更新状态栏显示加载的股票数量
            }
            catch (Exception ex) // 捕获可能发生的异常
            {
                MessageBox.Show($"加载股票数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误消息对话框
            }
        }

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        private void listViewStocks_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count > 0) // 检查是否有选中项
            {
                var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock; // 获取选中项的股票对象
                if (selectedStock != null) // 确保股票对象不为空
                {
                    // 使用异步加载避免UI卡顿
                    LoadStockPriceDataAsync(selectedStock.StockId);
                }
            }
        }

        /// <summary>
        /// 绘制股票价格图表
        /// </summary>
        private void DrawStockPriceChart(int stockId) // 绘制股票价格图表的私有方法，接收股票ID参数
        {
            try // 尝试执行以下代码块
            {
                // 使用缓存机制获取数据
                List<StockPrice> prices;
                var stock = _stockService.GetStockById(stockId);

                if (_currentStockId == stockId && _priceCache.ContainsKey(stockId))
                {
                    // 如果是相同股票且已有缓存，直接使用缓存数据
                    prices = _priceCache[stockId];
                }
                else
                {
                    // 获取该股票的所有价格数据
                    prices = _stockPriceService.GetStockPricesByStockId(stockId);

                    // 更新缓存
                    _priceCache[stockId] = prices;
                    _currentStockId = stockId;
                }

                if (prices.Count == 0) // 如果没有价格数据
                {
                    // 完全清空图表
                    stockChart.Series.Clear(); // 清空所有数据系列
                    stockChart.Titles.Clear(); // 清空所有标题

                    // 重置坐标轴设置
                    stockChart.ChartAreas[0].AxisX.Title = ""; // 清空X轴标题
                    stockChart.ChartAreas[0].AxisY.Title = ""; // 清空Y轴标题
                    stockChart.ChartAreas[0].AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False; // 禁用第二Y轴

                    // 添加提示文本
                    var noDataTitle = new System.Windows.Forms.DataVisualization.Charting.Title("暂无该股票的行情数据"); // 创建提示标题
                    noDataTitle.Font = new Font("Microsoft YaHei UI", 12, FontStyle.Regular); // 设置字体
                    noDataTitle.ForeColor = Color.Gray; // 设置颜色为灰色
                    stockChart.Titles.Add(noDataTitle); // 添加标题到图表

                    // 刷新图表
                    stockChart.Invalidate(); // 强制重绘图表
                    return; // 提前返回，不执行后续代码
                }

                // 排序价格数据（按日期升序）
                prices = prices.OrderBy(p => p.TradeDate).ToList(); // 使用LINQ对价格数据按交易日期排序

                // 数据抽样 - 当数据点过多时只绘制部分数据
                int sampleRate = Math.Max(1, prices.Count / 200); // 最多显示200个点
                List<StockPrice> sampledPrices = new List<StockPrice>();

                for (int i = 0; i < prices.Count; i += sampleRate)
                {
                    sampledPrices.Add(prices[i]);
                }

                // 使用抽样后的数据
                prices = sampledPrices;

                // 仅在必要时才清空和重建图表组件
                bool needRebuildChart = stockChart.Series.Count == 0 ||
                                       (stockChart.Series.Count > 0 &&
                                        stockChart.Series[0].Name.IndexOf(stock.Name) < 0);

                if (needRebuildChart)
                {
                    // 清除现有数据
                    stockChart.Series.Clear(); // 清空所有数据系列
                    stockChart.Titles.Clear(); // 清空所有标题

                    // 创建成交量柱形图（使用第二个Y轴）
                    var volumeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 成交量"); // 创建成交量数据系列
                    volumeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column; // 设置图表类型为柱状图
                    volumeSeries.Color = Color.LightGray; // 设置颜色为浅灰色
                    volumeSeries.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary; // 使用第二Y轴
                    // 设置柱状图宽度，避免太宽遮挡
                    volumeSeries["PointWidth"] = "0.6"; // 设置数据点宽度为60%

                    // 创建收盘价线图
                    var closeSeries = new System.Windows.Forms.DataVisualization.Charting.Series($"{stock.Name} 收盘价"); // 创建收盘价数据系列
                    closeSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line; // 设置图表类型为线图
                    closeSeries.Color = Color.Blue; // 设置颜色为蓝色
                    closeSeries.BorderWidth = 2; // 设置线宽为2像素
                    // 设置线图显示在柱状图前面
                    closeSeries["DrawingStyle"] = "Cylinder"; // 设置绘图样式为圆柱形

                    // 添加数据系列到图表 - 先添加成交量系列，后添加收盘价系列，确保收盘价线图显示在最上层
                    stockChart.Series.Add(volumeSeries); // 添加成交量数据系列
                    stockChart.Series.Add(closeSeries); // 添加收盘价数据系列

                    // 配置图表区域
                    var chartArea = stockChart.ChartAreas[0]; // 获取第一个图表区域

                    // 设置X轴
                    // chartArea.AxisX.Title = "日期"; // 设置X轴标题
                    // chartArea.AxisX.LabelStyle.Angle = -15; // 斜角显示日期，节省空间
                    chartArea.AxisX.LabelStyle.Font = new Font("Microsoft YaHei UI", 7); // 设置X轴标签字体
                    chartArea.AxisX.MajorGrid.LineColor = Color.LightGray; // 设置X轴主网格线颜色

                    // 设置主Y轴（收盘价）
                    chartArea.AxisY.Title = "价格"; // 设置Y轴标题
                    chartArea.AxisY.LabelStyle.Format = "C2"; // 设置Y轴标签格式为货币，保留2位小数
                    chartArea.AxisY.MajorGrid.LineColor = Color.LightGray; // 设置Y轴主网格线颜色

                    // 配置第二个Y轴（成交量）
                    chartArea.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True; // 启用第二Y轴
                    chartArea.AxisY2.Title = "成交量"; // 设置第二Y轴标题
                    chartArea.AxisY2.LabelStyle.Format = "N0"; // 设置第二Y轴标签格式为数字，无小数
                    chartArea.AxisY2.MajorGrid.Enabled = false; // 禁用第二Y轴的网格线，避免混淆

                    // 优化图表外观
                    chartArea.BackColor = Color.White; // 设置图表区域背景色为白色
                    chartArea.BorderColor = Color.LightGray; // 设置图表区域边框颜色为浅灰色
                    chartArea.BorderWidth = 1; // 设置图表区域边框宽度为1像素

                    // 设置内边距，确保所有元素都在视图内
                    chartArea.InnerPlotPosition.Auto = false; // 禁用自动内边距
                    chartArea.InnerPlotPosition.X = 10; // 设置左内边距为10%
                    chartArea.InnerPlotPosition.Y = 5; // 设置上内边距为5%
                    chartArea.InnerPlotPosition.Width = 85; // 设置宽度为85%
                    chartArea.InnerPlotPosition.Height = 85; // 设置高度为85%

                    // 设置标题
                    var title = new System.Windows.Forms.DataVisualization.Charting.Title($"{stock.Name}({stock.Code}) 股票价格走势"); // 创建图表标题
                    title.Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold); // 设置标题字体
                    stockChart.Titles.Add(title); // 添加标题到图表
                }
                else
                {
                    // 只清除数据点而不重建整个系列
                    stockChart.Series[0].Points.Clear();
                    stockChart.Series[1].Points.Clear();
                }

                // 添加数据点
                for (int i = 0; i < prices.Count; i++)
                {
                    var price = prices[i]; // 获取当前价格对象
                    var date = price.TradeDate.ToShortDateString(); // 获取交易日期的短日期字符串
                    stockChart.Series[0].Points.AddXY(date, price.Volume); // 添加成交量数据点
                    stockChart.Series[1].Points.AddXY(date, price.ClosePrice); // 添加收盘价数据点
                }

                // 动态调整X轴标签间隔，确保不会太密集
                stockChart.ChartAreas[0].AxisX.Interval = Math.Max(1, prices.Count / 10);

                // 获取收盘价的最大最小值，计算合适的Y轴范围
                decimal minPrice = prices.Min(p => p.LowPrice); // 获取最低价
                decimal maxPrice = prices.Max(p => p.HighPrice); // 获取最高价
                decimal priceRange = maxPrice - minPrice; // 计算价格范围
                // 设置Y轴范围，留有10%的余量
                stockChart.ChartAreas[0].AxisY.Minimum = (double)Math.Max(0, minPrice - priceRange * 0.1m); // 设置Y轴最小值，不小于0
                stockChart.ChartAreas[0].AxisY.Maximum = (double)(maxPrice + priceRange * 0.1m); // 设置Y轴最大值，增加10%余量

                // 获取成交量的最大值，计算合适的Y2轴范围
                long maxVolume = prices.Max(p => p.Volume); // 获取最大成交量
                // 设置Y2轴范围，留有20%的余量
                stockChart.ChartAreas[0].AxisY2.Minimum = 0; // 设置第二Y轴最小值为0
                stockChart.ChartAreas[0].AxisY2.Maximum = (double)(maxVolume * 1.2); // 设置第二Y轴最大值，增加20%余量

                // 刷新图表
                stockChart.Invalidate(); // 强制重绘图表
            }
            catch (Exception ex) // 捕获可能发生的异常
            {
                MessageBox.Show($"绘制股票图表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误消息对话框
            }
        }

        /// <summary>
        /// 添加股票信息
        /// </summary>
        private void toolStripButtonAddStock_Click(object sender, EventArgs e)
        {
            using (var form = new StockEditForm(_stockService)) // 创建股票编辑窗体，并在使用完后自动释放资源
            {
                if (form.ShowDialog() == DialogResult.OK) // 显示对话框，如果用户点击确定
                {
                    LoadStockData(); // 重新加载股票数据，刷新列表
                }
            }
        }

        /// <summary>
        /// 编辑股票信息
        /// </summary>
        private void toolStripButtonEditStock_Click(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count == 0) // 检查是否有选中项
            {
                MessageBox.Show("请先选择要编辑的股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); // 显示警告消息
                return; // 提前返回，不执行后续代码
            }

            var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock; // 获取选中项的股票对象
            using (var form = new StockEditForm(_stockService, selectedStock)) // 创建股票编辑窗体，传入选中的股票对象
            {
                if (form.ShowDialog() == DialogResult.OK) // 显示对话框，如果用户点击确定
                {
                    LoadStockData(); // 重新加载股票数据，刷新列表
                }
            }
        }

        /// <summary>
        /// 删除股票信息
        /// </summary>
        private void toolStripButtonDeleteStock_Click(object sender, EventArgs e)
        {
            if (listViewStocks.SelectedItems.Count == 0) // 检查是否有选中项
            {
                MessageBox.Show("请先选择要删除的股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); // 显示警告消息
                return; // 提前返回，不执行后续代码
            }

            var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock; // 获取选中项的股票对象
            var result = MessageBox.Show($"确定要删除股票 {selectedStock.Name} 吗？此操作将同时删除该股票的所有行情数据！", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning); // 显示确认对话框

            if (result == DialogResult.Yes) // 如果用户点击是
            {
                bool success = _stockService.DeleteStock(selectedStock.StockId); // 调用服务删除股票
                if (success) // 如果删除成功
                {
                    MessageBox.Show("删除成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); // 显示成功消息
                    LoadStockData(); // 重新加载股票数据，刷新列表

                    // 清空图表
                    stockChart.Series.Clear(); // 清空所有数据系列

                    // 如果列表中还有股票，则选中第一个
                    if (listViewStocks.Items.Count > 0) // 检查是否还有股票项
                    {
                        listViewStocks.Items[0].Selected = true; // 选中第一项
                        listViewStocks.EnsureVisible(0); // 确保第一项在视图中可见
                    }
                }
                else // 如果删除失败
                {
                    MessageBox.Show("删除失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误消息
                }
            }
        }

        /// <summary>
        /// 查看股票行情
        /// </summary>
        private void toolStripButtonViewPrice_Click(object sender, EventArgs e)
        {
            // 获取当前选中的股票ID
            int? selectedStockId = null; // 声明可空整型变量存储选中的股票ID
            if (listViewStocks.SelectedItems.Count > 0) // 检查是否有选中项
            {
                var selectedStock = listViewStocks.SelectedItems[0].Tag as Stock; // 获取选中项的股票对象
                selectedStockId = selectedStock.StockId; // 获取股票ID
            }

            using (var form = new StockPriceQueryForm(_stockService, _stockPriceService)) // 创建股票价格查询窗体
            {
                // 如果有选中股票，预先设置到查询表单
                if (selectedStockId.HasValue) // 检查是否有选中的股票ID
                {
                    form.PresetStockId = selectedStockId.Value; // 设置预选股票ID
                }
                form.ShowDialog(); // 显示对话框
            }
        }

        /// <summary>
        /// 启动股票筛选
        /// </summary>
        private void toolStripButtonFilter_Click(object sender, EventArgs e)
        {
            using (var filterForm = new StockFilterForm()) // 创建股票筛选窗体
            {
                if (filterForm.ShowDialog() == DialogResult.OK) // 显示对话框，如果用户点击确定
                {
                    // 如果用户在筛选表单中选择了股票，更新当前选中股票
                    if (filterForm.SelectedStock != null) // 检查是否有选中的股票
                    {
                        // 先取消所有项的选中状态
                        foreach (ListViewItem item in listViewStocks.Items)
                        {
                            item.Selected = false;
                        }

                        // 查找并选中新选择的股票
                        bool found = false;
                        foreach (ListViewItem item in listViewStocks.Items) // 遍历所有列表项
                        {
                            if (item.Tag is Stock stock && stock.StockId == filterForm.SelectedStock.StockId) // 检查是否是目标股票
                            {
                                // 确保只选中这一项
                                item.Selected = true; // 选中该项
                                item.EnsureVisible(); // 确保该项在视图中可见
                                found = true;

                                // 立即绘制股票图表，不等待异步加载
                                int stockId = filterForm.SelectedStock.StockId;
                                DrawStockPriceChart(stockId);

                                // 更新状态栏
                                toolStripStatusLabel1.Text = $"已选择: {stock.Name}({stock.Code})";

                                break; // 找到后跳出循环
                            }
                        }

                        // 如果在列表中找不到，也要尝试加载图表数据
                        if (!found && filterForm.SelectedStock != null)
                        {
                            int stockId = filterForm.SelectedStock.StockId;
                            DrawStockPriceChart(stockId);

                            // 更新状态栏
                            toolStripStatusLabel1.Text = $"已选择ID为{stockId}的股票(未在当前列表中显示)";
                        }
                    }
                }

                // 检查是否有删除操作，如果有则刷新数据
                if (filterForm.HasDeletedRecords) // 检查是否有删除记录
                {
                    LoadStockData(); // 重新加载股票数据，刷新列表
                }
            }
        }

        /// <summary>
        /// 启动数据导入导出功能
        /// </summary>
        private void toolStripButtonDataIO_Click(object sender, EventArgs e)
        {
            using (var dataIOForm = new DataIOForm()) // 创建数据导入导出窗体
            {
                if (dataIOForm.ShowDialog() == DialogResult.OK) // 显示对话框，如果用户点击确定
                {
                    // 刷新股票数据
                    LoadStockData(); // 重新加载股票数据，刷新列表
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
                // 取消之前的加载操作
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                // 创建新的取消令牌
                _cancellationTokenSource = new System.Threading.CancellationTokenSource();
                var token = _cancellationTokenSource.Token;

                // 显示加载提示
                toolStripStatusLabel1.Text = "正在加载股票数据...";

                // 异步获取数据
                await System.Threading.Tasks.Task.Run(() =>
                {
                    if (!_priceCache.ContainsKey(stockId))
                    {
                        if (token.IsCancellationRequested)
                            return;

                        _priceCache[stockId] = _stockPriceService.GetStockPricesByStockId(stockId);
                    }
                }, token);

                // 如果已经取消，则不执行后续操作
                if (token.IsCancellationRequested)
                    return;

                // 数据加载完成后绘制图表
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
                // 任务被取消，不做处理
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
            _priceCache.Clear();
            _currentStockId = -1;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) // 窗体关闭事件处理方法
        {
            // 取消任何正在进行的异步操作
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            // 释放资源
            _stockService.Dispose(); // 释放股票服务资源
            _stockPriceService.Dispose(); // 释放股票价格服务资源
        }
    }
}
