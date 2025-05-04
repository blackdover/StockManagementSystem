using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Models;
using StockManagementSystem.Services;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace StockManagementSystem.Forms
{
    public partial class DataIOForm : Form
    {
        private readonly StockService _stockService;
        private readonly StockPriceService _stockPriceService;
        private bool _dataChanged = false;
        private ProgressBar progressBarImport; // 导入进度条

        // 批处理大小
        private const int BATCH_SIZE = 500;

        public DataIOForm()
        {
            InitializeComponent();
            _stockService = new StockService();
            _stockPriceService = new StockPriceService();
            InitializeProgressBar();
        }

        public DataIOForm(StockService stockService, StockPriceService stockPriceService)
        {
            InitializeComponent();
            _stockService = stockService;
            _stockPriceService = stockPriceService;
            InitializeProgressBar();
        }

        /// <summary>
        /// 初始化进度条
        /// </summary>
        private void InitializeProgressBar()
        {
            // 创建进度条
            progressBarImport = new ProgressBar();
            progressBarImport.Location = new Point(15, 65);
            progressBarImport.Name = "progressBarImport";
            progressBarImport.Size = new Size(530, 20);
            progressBarImport.Style = ProgressBarStyle.Continuous;
            progressBarImport.Visible = false;
            progressBarImport.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            groupBoxImport.Controls.Add(progressBarImport);
            groupBoxImport.Height = 100;
        }

        /// <summary>
        /// 更新进度显示
        /// </summary>
        /// <param name="current">当前处理的项</param>
        /// <param name="total">总项数</param>
        /// <param name="operation">操作描述</param>
        private void UpdateProgress(int current, int total, string operation = "处理中")
        {
            if (current <= total)
            {
                // 更新进度条
                progressBarImport.Maximum = total;
                progressBarImport.Value = current;
                // 更新状态栏
                double percentage = (double)current / total * 100;
                UpdateStatus($"{operation}: {current}/{total} ({percentage:F1}%)");
                // 刷新界面
                Application.DoEvents();
            }
        }

        /// <summary>
        /// 重置进度显示
        /// </summary>
        private void ResetProgress()
        {
            progressBarImport.Visible = false;
            progressBarImport.Value = 0;
        }

        private void DataIOForm_Load(object sender, EventArgs e)
        {
            // 设置日期选择器默认日期
            dateTimePickerStart.Value = DateTime.Now.AddMonths(-1);
            dateTimePickerEnd.Value = DateTime.Now;
        }

        #region 导出功能

        private void btnExportStocks_Click(object sender, EventArgs e)
        {
            try
            {
                // 获取所有股票数据
                var stocks = _stockService.GetAllStocks();
                if (stocks.Count == 0)
                {
                    MessageBox.Show("没有股票数据可以导出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // 设置保存对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*";
                saveFileDialog.Title = "导出股票数据";
                saveFileDialog.FileName = $"股票数据_{DateTime.Now:yyyyMMdd}";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    // 添加表头
                    sb.AppendLine("ID,股票代码,股票名称,股票类型,所属行业,上市日期,描述");
                    // 添加数据行
                    foreach (var stock in stocks)
                    {
                        // 将日期格式修改为年-月-日格式，并用引号包裹，避免Excel显示######
                        string formattedDate = $"\"{stock.ListingDate:yyyy-MM-dd}\"";
                        string description = stock.Description?.Replace(",", " ") ?? "";
                        // 在股票代码前添加等号和引号，确保Excel将其视为文本并保留前导零
                        string formattedCode = $"=\"{stock.Code}\"";
                        sb.AppendLine($"{stock.StockId},{formattedCode},{stock.Name},{stock.Type},{stock.Industry},{formattedDate},{description}");
                    }
                    // 写入文件
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    // 更新状态
                    UpdateStatus($"成功导出 {stocks.Count} 条股票数据到 {saveFileDialog.FileName}");
                    MessageBox.Show($"成功导出 {stocks.Count} 条股票数据！\n\n提示：如果在Excel中打开时日期显示异常，请先选择该列然后调整列宽。", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出股票数据时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"导出股票数据失败：{ex.Message}");
            }
        }

        private void btnExportPrices_Click(object sender, EventArgs e)
        {
            try
            {
                // 获取日期范围
                DateTime startDate = dateTimePickerStart.Value.Date;
                DateTime endDate = dateTimePickerEnd.Value.Date;
                if (startDate > endDate)
                {
                    MessageBox.Show("开始日期不能大于结束日期！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // 获取指定日期范围内的股票价格数据
                List<StockPrice> prices;
                if (checkBoxAllPrices.Checked)
                {
                    // 获取所有股票价格数据
                    prices = _stockPriceService.GetStockPrices(null, DateTimeHelper.SqlMinDate, DateTimeHelper.SqlMaxDate);
                }
                else
                {
                    prices = _stockPriceService.GetStockPrices(null, startDate, endDate);
                }
                if (prices.Count == 0)
                {
                    MessageBox.Show("没有符合条件的股票价格数据可以导出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // 设置保存对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*";
                saveFileDialog.Title = "导出股票价格数据";
                saveFileDialog.FileName = $"股票价格数据_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 构建CSV内容
                    StringBuilder sb = new StringBuilder();
                    // 添加表头
                    sb.AppendLine("股票ID,股票代码,交易日期,开盘价,收盘价,最高价,最低价,成交量,成交额,涨跌幅");
                    // 获取所有股票信息（用于查找代码）
                    var stocks = _stockService.GetAllStocks().ToDictionary(s => s.StockId, s => s);
                    // 添加数据行
                    foreach (var price in prices)
                    {
                        string stockCode = stocks.ContainsKey(price.StockId) ? stocks[price.StockId].Code : price.StockId.ToString();
                        // 将日期格式修改为年-月-日格式，并用引号包裹，避免Excel显示######
                        string formattedDate = $"\"{price.TradeDate:yyyy-MM-dd}\"";
                        // 在股票代码前添加等号和引号，确保Excel将其视为文本并保留前导零
                        string formattedCode = $"=\"{stockCode}\"";

                        sb.AppendLine($"{price.StockId},{formattedCode},{formattedDate},{price.OpenPrice},{price.ClosePrice},{price.HighPrice},{price.LowPrice},{price.Volume},{price.Amount},{price.ChangePercent}");
                    }

                    // 写入文件
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                    // 更新状态
                    UpdateStatus($"成功导出 {prices.Count} 条股票价格数据到 {saveFileDialog.FileName}");
                    MessageBox.Show($"成功导出 {prices.Count} 条股票价格数据！\n\n提示：如果在Excel中打开时日期显示异常，请先选择该列然后调整列宽。", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出股票价格数据时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"导出股票价格数据失败：{ex.Message}");
            }
        }

        #endregion

        #region 导入功能

        private void btnImportStocks_Click(object sender, EventArgs e)
        {
            try
            {
                // 设置打开对话框
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*";
                openFileDialog.Title = "导入股票数据";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 读取文件内容 - 使用更高效的方式读取大文件
                    UpdateStatus("正在读取文件...");
                    // 检查并估算文件大小
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    if (fileInfo.Length > 10 * 1024 * 1024) // 如果大于10MB
                    {
                        if (MessageBox.Show("文件较大，导入可能需要一些时间。是否继续？", "文件大小提示",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    // 增加进度显示
                    progressBarImport.Visible = true;
                    progressBarImport.Style = ProgressBarStyle.Marquee;
                    UpdateStatus("正在读取文件内容...");
                    // 读取文件内容
                    List<string> lines = new List<string>();
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName, Encoding.UTF8))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                            if (lines.Count % 100 == 0)
                            {
                                UpdateStatus($"已读取 {lines.Count} 行...");
                                Application.DoEvents();
                            }
                        }
                    }
                    if (lines.Count <= 1)
                    {
                        progressBarImport.Visible = false;
                        MessageBox.Show("文件中没有有效的股票数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // 检查表头
                    string header = lines[0].Trim();
                    // 支持原格式和新格式
                    if (!header.StartsWith("ID,股票代码,股票名称,股票类型,所属行业") &&
                        !header.StartsWith("Code,Name,Type,Industry,ListingDate"))
                    {
                        progressBarImport.Visible = false;
                        MessageBox.Show("文件格式不正确！请确保文件包含正确的表头。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // 确定表头格式类型
                    bool isNewFormat = header.StartsWith("Code,Name,Type");
                    // 获取现有的股票代码列表（用于检查重复）
                    UpdateStatus("正在检查现有股票数据...");
                    var existingStocks = _stockService.GetAllStocks();
                    var existingCodes = existingStocks.Select(s => s.Code).ToHashSet();
                    // 显示并初始化进度条
                    progressBarImport.Style = ProgressBarStyle.Continuous;
                    int totalCount = lines.Count - 1; // 减去表头
                    UpdateProgress(0, totalCount, "正在解析股票数据");
                    // 使用并行处理解析数据
                    ConcurrentBag<Stock> stocksToAdd = new ConcurrentBag<Stock>();
                    int skippedCount = 0;
                    int maxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 4);
                    // 分批处理以减少内存压力
                    for (int batchStart = 1; batchStart < lines.Count; batchStart += BATCH_SIZE)
                    {
                        int batchEnd = Math.Min(batchStart + BATCH_SIZE, lines.Count);
                        int batchSize = batchEnd - batchStart;
                        UpdateStatus($"正在解析第 {batchStart}-{batchEnd - 1} 行数据...");
                        // 创建临时存储解析结果的列表
                        List<Stock> batchStocks = new List<Stock>();
                        int batchSkipped = 0;
                        // 使用并行处理加速解析
                        Parallel.For(batchStart, batchEnd, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, i =>
                        {
                            string line = lines[i].Trim();
                            if (string.IsNullOrEmpty(line)) return;
                            try
                            {
                                // 解析CSV行
                                string[] fields = ParseCsvLine(line);
                                if (fields.Length < 4) return;

                                string code;
                                string name;
                                string type;
                                string industry;
                                string description = "";
                                DateTime listingDate = DateTime.Now;

                                if (isNewFormat)
                                {
                                    // 新格式: Code,Name,Type,Industry,ListingDate,Description,CreateTime,UpdateTime
                                    code = fields[0].Trim().Trim('"'); // 删除可能存在的引号
                                    name = fields[1].Trim();
                                    type = fields[2].Trim();
                                    industry = fields[3].Trim();

                                    // 解析上市日期，如果无法解析则使用当前日期
                                    if (fields.Length > 4 && !fields[4].Contains("#"))
                                    {
                                        TryParseDate(fields[4].Trim().Trim('"'), out listingDate);
                                    }

                                    // 描述字段
                                    if (fields.Length > 5)
                                    {
                                        description = fields[5].Trim();
                                    }
                                }
                                else
                                {
                                    // 原格式: ID,股票代码,股票名称,股票类型,所属行业,上市日期,描述
                                    code = fields[1].Trim().Trim('"'); // 删除可能存在的引号
                                    name = fields[2].Trim();
                                    type = fields[3].Trim();
                                    industry = fields[4].Trim();

                                    // 解析上市日期
                                    if (fields.Length > 5)
                                    {
                                        TryParseDate(fields[5].Trim(), out listingDate);
                                    }

                                    // 描述字段
                                    if (fields.Length > 6)
                                    {
                                        description = fields[6].Trim();
                                    }
                                }
                                // 检查是否已存在
                                if (existingCodes.Contains(code))
                                {
                                    Interlocked.Increment(ref batchSkipped);
                                    return;
                                }
                                // 创建股票对象
                                Stock stock = new Stock
                                {
                                    Code = code,
                                    Name = name,
                                    Type = type,
                                    Industry = industry,
                                    ListingDate = listingDate,
                                    Description = description,
                                    CreateTime = DateTime.Now,
                                    UpdateTime = DateTime.Now
                                };

                                lock (batchStocks)
                                {
                                    batchStocks.Add(stock);
                                }
                            }
                            catch
                            {
                                // 忽略解析错误的行
                                Interlocked.Increment(ref batchSkipped);
                            }
                        });
                        // 更新进度
                        UpdateProgress(batchEnd - 1, totalCount, "正在解析股票数据");
                        // 将批次处理的结果合并到总结果中
                        foreach (var stock in batchStocks)
                        {
                            stocksToAdd.Add(stock);
                        }
                        skippedCount += batchSkipped;
                        // 释放批次内存
                        batchStocks.Clear();
                    }

                    int importedCount = stocksToAdd.Count;
                    // 批量添加股票
                    if (stocksToAdd.Count > 0)
                    {
                        UpdateStatus("正在保存股票数据到数据库...");
                        int savedCount = 0;
                        // 将并发包转换为列表并排序以确保一致性
                        List<Stock> orderedStocks = stocksToAdd.ToList();
                        // 显示并更新进度条设置
                        progressBarImport.Visible = true;
                        progressBarImport.Style = ProgressBarStyle.Continuous;
                        progressBarImport.Maximum = orderedStocks.Count;
                        progressBarImport.Value = 0;
                        // 批量保存，使用事务提高性能，并添加进度回调
                        savedCount = _stockService.AddStocksBatch(
                            orderedStocks,
                            (current, total) =>
                            {
                                // 使用委托在UI线程上更新进度条
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        UpdateProgress(current, total, "正在保存股票数据到数据库");
                                        Application.DoEvents();
                                    }));
                                }
                                else
                                {
                                    UpdateProgress(current, total, "正在保存股票数据到数据库");
                                    Application.DoEvents();
                                }
                            });

                        _dataChanged = true;

                        // 重置进度条
                        ResetProgress();

                        // 更新状态
                        UpdateStatus($"已成功导入 {savedCount} 条股票数据，跳过 {skippedCount} 条重复或无效数据");
                        MessageBox.Show($"导入完成！\n成功导入：{savedCount} 条\n跳过重复或无效：{skippedCount} 条", "导入结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // 重置进度条
                        ResetProgress();

                        MessageBox.Show("没有新的股票数据需要导入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // 重置进度条
                ResetProgress();

                MessageBox.Show($"导入股票数据时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"导入股票数据失败：{ex.Message}");
            }
        }

        private void btnImportPrices_Click(object sender, EventArgs e)
        {
            try
            {
                // 设置打开对话框
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV文件(*.csv)|*.csv|所有文件(*.*)|*.*";
                openFileDialog.Title = "导入股票价格数据";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 检查并估算文件大小
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    if (fileInfo.Length > 20 * 1024 * 1024) // 如果大于20MB
                    {
                        if (MessageBox.Show("文件较大，导入可能需要相当长的时间。是否继续？", "文件大小提示",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    // 增加进度显示
                    progressBarImport.Visible = true;
                    progressBarImport.Style = ProgressBarStyle.Marquee;
                    UpdateStatus("正在读取文件内容...");
                    // 高效读取文件内容
                    List<string> lines = new List<string>();
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName, Encoding.UTF8))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                            if (lines.Count % 100 == 0)
                            {
                                UpdateStatus($"已读取 {lines.Count} 行...");
                                Application.DoEvents();
                            }
                        }
                    }
                    if (lines.Count <= 1)
                    {
                        progressBarImport.Visible = false;
                        MessageBox.Show("文件中没有有效的股票价格数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // 检查表头
                    string header = lines[0].Trim();
                    // 支持原格式和新格式
                    if (!header.StartsWith("股票ID,股票代码,交易日期,开盘价,收盘价,最高价,最低价") &&
                        !header.StartsWith("Code,TradeDate,OpenPrice,ClosePrice,HighPrice,LowPrice") &&
                        !header.StartsWith("StockId,TradeDate,OpenPrice,ClosePrice,HighPrice,LowPrice"))
                    {
                        progressBarImport.Visible = false;
                        MessageBox.Show("文件格式不正确！请确保文件包含正确的表头。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 确定表头格式类型
                    bool isNewFormat = header.StartsWith("Code,TradeDate") || header.StartsWith("StockId,TradeDate");

                    // 获取所有股票（用于查询ID）
                    UpdateStatus("正在加载股票信息...");
                    var stocks = _stockService.GetAllStocks();
                    var stockCodeToId = stocks.ToDictionary(s => s.Code, s => s.StockId);

                    // 获取现有的价格数据（用于检查重复）- 优化：只加载必要的字段
                    UpdateStatus("正在创建价格数据索引...");

                    // 使用并发字典来存储索引，提高并发性能
                    ConcurrentDictionary<Tuple<int, DateTime>, bool> existingPriceKeys = new ConcurrentDictionary<Tuple<int, DateTime>, bool>();
                    // 分批加载现有价格数据，以降低内存压力
                    var stockIds = stocks.Select(s => s.StockId).ToList();
                    foreach (var stockIdBatch in BatchList(stockIds, 50))
                    {
                        UpdateStatus($"正在加载股票价格索引 (ID: {stockIdBatch.First()}-{stockIdBatch.Last()})...");
                        var batchPrices = _stockPriceService.GetMinimalPriceData(stockIdBatch);
                        foreach (var price in batchPrices)
                        {
                            existingPriceKeys[new Tuple<int, DateTime>(price.Item1, price.Item2.Date)] = true;
                        }
                    }
                    // 显示并初始化进度条
                    progressBarImport.Style = ProgressBarStyle.Continuous;
                    int totalCount = lines.Count - 1; // 减去表头
                    UpdateProgress(0, totalCount, "正在解析数据");
                    // 使用并行处理解析数据
                    ConcurrentBag<StockPrice> pricesToAdd = new ConcurrentBag<StockPrice>();
                    int skippedCount = 0;
                    // 使用并行处理加速解析，但不使用太多线程以避免过度消耗内存
                    int maxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 4);
                    // 分批处理解析，降低内存压力
                    for (int batchStart = 1; batchStart < lines.Count; batchStart += BATCH_SIZE)
                    {
                        int batchEnd = Math.Min(batchStart + BATCH_SIZE, lines.Count);
                        UpdateStatus($"正在解析第 {batchStart}-{batchEnd - 1} 行数据...");
                        // 创建临时存储解析结果的列表
                        List<StockPrice> batchPrices = new List<StockPrice>();
                        int batchSkipped = 0;
                        // 使用并行处理加速解析
                        Parallel.For(batchStart, batchEnd, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, i =>
                        {
                            string line = lines[i].Trim();
                            if (string.IsNullOrEmpty(line)) return;

                            try
                            {
                                // 解析CSV行
                                string[] fields = ParseCsvLine(line);
                                if (fields.Length < 7) return; // 至少需要7个字段

                                int stockId;
                                DateTime tradeDate;
                                decimal openPrice;
                                decimal closePrice;
                                decimal highPrice;
                                decimal lowPrice;
                                long volume;
                                decimal amount = 0;
                                decimal changePercent = 0;

                                if (isNewFormat)
                                {
                                    // 新格式: Code,TradeDate,OpenPrice,ClosePrice,HighPrice,LowPrice,Volume,Amount,ChangePercent,CreateTime

                                    // 解析股票代码
                                    string stockCode = fields[0].Trim();

                                    // 通过股票代码查找StockId
                                    if (!stockCodeToId.ContainsKey(stockCode))
                                    {
                                        Interlocked.Increment(ref batchSkipped);
                                        return; // 找不到对应的股票，跳过
                                    }
                                    stockId = stockCodeToId[stockCode];

                                    // 解析交易日期
                                    if (!TryParseDate(fields[1].Trim(), out tradeDate))
                                    {
                                        Interlocked.Increment(ref batchSkipped);
                                        return;
                                    }

                                    // 解析价格数据
                                    if (!decimal.TryParse(fields[2].Trim(), out openPrice) ||
                                        !decimal.TryParse(fields[3].Trim(), out closePrice) ||
                                        !decimal.TryParse(fields[4].Trim(), out highPrice) ||
                                        !decimal.TryParse(fields[5].Trim(), out lowPrice) ||
                                        !long.TryParse(fields[6].Trim(), out volume))
                                    {
                                        Interlocked.Increment(ref batchSkipped);
                                        return;
                                    }

                                    // 解析可选字段
                                    if (fields.Length > 7 && decimal.TryParse(fields[7].Trim(), out decimal parsedAmount))
                                    {
                                        amount = parsedAmount;
                                    }

                                    if (fields.Length > 8 && decimal.TryParse(fields[8].Trim(), out decimal parsedChangePercent))
                                    {
                                        changePercent = parsedChangePercent;
                                    }
                                }
                                else
                                {
                                    // 原格式: 股票ID,股票代码,交易日期,开盘价,收盘价,最高价,最低价,成交量,成交额,涨跌幅
                                    string stockCode = fields[1].Trim();

                                    // 尝试从第一列获取股票ID，如果失败则通过代码查找
                                    if (!int.TryParse(fields[0].Trim(), out stockId) || !stocks.Any(s => s.StockId == stockId))
                                    {
                                        // 尝试通过股票代码查找ID
                                        if (!stockCodeToId.ContainsKey(stockCode))
                                        {
                                            Interlocked.Increment(ref batchSkipped);
                                            return; // 找不到对应的股票，跳过
                                        }
                                        stockId = stockCodeToId[stockCode];
                                    }

                                    // 解析交易日期
                                    if (!TryParseDate(fields[2].Trim(), out tradeDate))
                                    {
                                        Interlocked.Increment(ref batchSkipped);
                                        return;
                                    }

                                    // 解析价格数据
                                    if (!decimal.TryParse(fields[3].Trim(), out openPrice) ||
                                        !decimal.TryParse(fields[4].Trim(), out closePrice) ||
                                        !decimal.TryParse(fields[5].Trim(), out highPrice) ||
                                        !decimal.TryParse(fields[6].Trim(), out lowPrice) ||
                                        !long.TryParse(fields[7].Trim(), out volume))
                                    {
                                        Interlocked.Increment(ref batchSkipped);
                                        return;
                                    }

                                    // 解析可选字段
                                    if (fields.Length > 8 && decimal.TryParse(fields[8].Trim(), out decimal parsedAmount))
                                    {
                                        amount = parsedAmount;
                                    }

                                    if (fields.Length > 9 && decimal.TryParse(fields[9].Trim(), out decimal parsedChangePercent))
                                    {
                                        changePercent = parsedChangePercent;
                                    }
                                }

                                // 检查是否已存在相同股票ID和交易日期的记录
                                var key = new Tuple<int, DateTime>(stockId, tradeDate.Date);
                                if (existingPriceKeys.ContainsKey(key))
                                {
                                    Interlocked.Increment(ref batchSkipped);
                                    return;
                                }

                                // 创建价格对象
                                StockPrice price = new StockPrice
                                {
                                    StockId = stockId,
                                    TradeDate = tradeDate,
                                    OpenPrice = openPrice,
                                    ClosePrice = closePrice,
                                    HighPrice = highPrice,
                                    LowPrice = lowPrice,
                                    Volume = volume,
                                    Amount = amount,
                                    ChangePercent = changePercent,
                                    CreateTime = DateTime.Now
                                };

                                lock (batchPrices)
                                {
                                    batchPrices.Add(price);
                                }

                                // 添加到已存在集合，防止同一次导入中出现重复
                                existingPriceKeys[key] = true;
                            }
                            catch
                            {
                                // 忽略解析错误的行
                                Interlocked.Increment(ref batchSkipped);
                            }
                        });

                        // 更新进度
                        UpdateProgress(batchEnd - 1, totalCount, "正在解析数据");

                        // 将批次处理的结果合并到总结果中
                        foreach (var price in batchPrices)
                        {
                            pricesToAdd.Add(price);
                        }
                        skippedCount += batchSkipped;

                        // 释放批次内存
                        batchPrices.Clear();
                    }

                    int importedCount = pricesToAdd.Count;

                    // 批量添加价格数据
                    if (pricesToAdd.Count > 0)
                    {
                        UpdateStatus("正在保存数据到数据库...");

                        // 将并发包转换为列表
                        List<StockPrice> orderedPrices = pricesToAdd.ToList();

                        // 显示进度条，并切换到连续模式
                        progressBarImport.Visible = true;
                        progressBarImport.Style = ProgressBarStyle.Continuous;
                        progressBarImport.Maximum = orderedPrices.Count;
                        progressBarImport.Value = 0;

                        // 批量保存，使用事务提高性能，并添加进度回调
                        int savedCount = _stockPriceService.AddStockPricesBatch(
                            orderedPrices,
                            (current, total) =>
                            {
                                // 使用委托在UI线程上更新进度条
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        UpdateProgress(current, total, "正在保存数据到数据库");
                                        Application.DoEvents(); // 使UI保持响应
                                    }));
                                }
                                else
                                {
                                    UpdateProgress(current, total, "正在保存数据到数据库");
                                    Application.DoEvents(); // 使UI保持响应
                                }
                            });

                        _dataChanged = true;

                        // 重置进度条
                        ResetProgress();

                        // 更新状态
                        UpdateStatus($"已成功导入 {savedCount} 条股票价格数据，跳过 {skippedCount} 条重复或无效数据");
                        MessageBox.Show($"导入完成！\n成功导入：{savedCount} 条\n跳过重复或无效：{skippedCount} 条", "导入结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // 重置进度条
                        ResetProgress();

                        MessageBox.Show("没有新的股票价格数据需要导入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // 重置进度条
                ResetProgress();

                MessageBox.Show($"导入股票价格数据时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"导入股票价格数据失败：{ex.Message}");
            }
        }

        #endregion

        #region 辅助方法

        private void UpdateStatus(string message)
        {
            labelStatus.Text = message;
            statusStrip1.Refresh();
        }

        /// <summary>
        /// 解析CSV行，支持引号字段
        /// </summary>
        private string[] ParseCsvLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return new string[0];
            List<string> results = new List<string>();
            bool inQuotes = false;
            StringBuilder field = new StringBuilder();
            // 遍历每个字符
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                // 处理引号
                if (c == '"')
                {
                    // 如果是转义的引号 (""), 则添加一个引号到字段中
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++; // 跳过下一个引号
                    }
                    else
                    {
                        // 切换引号状态
                        inQuotes = !inQuotes;
                    }
                }
                // 处理逗号
                else if (c == ',' && !inQuotes)
                {
                    // 字段结束，添加到结果中
                    results.Add(field.ToString());
                    field.Clear();
                }
                // 普通字符
                else
                {
                    field.Append(c);
                }
            }

            // 添加最后一个字段
            results.Add(field.ToString());

            return results.ToArray();
        }

        /// <summary>
        /// 尝试解析日期字符串，支持多种格式
        /// </summary>
        private bool TryParseDate(string dateString, out DateTime result)
        {
            result = DateTime.Now;

            if (string.IsNullOrWhiteSpace(dateString))
                return false;

            // 移除可能的引号
            dateString = dateString.Trim('"');

            // 尝试直接解析
            if (DateTime.TryParse(dateString, out result))
                return true;

            // 尝试ISO格式（YYYY-MM-DD）
            try
            {
                string[] parts = dateString.Split('-');
                if (parts.Length == 3)
                {
                    if (int.TryParse(parts[0], out int year) &&
                        int.TryParse(parts[1], out int month) &&
                        int.TryParse(parts[2], out int day))
                    {
                        if (year > 1900 && month >= 1 && month <= 12 && day >= 1 && day <= 31)
                        {
                            result = new DateTime(year, month, day);
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }

            // 尝试其他常见格式
            string[] formats = {
                "yyyy/MM/dd", "dd/MM/yyyy", "MM/dd/yyyy",
                "yyyyMMdd", "ddMMyyyy", "MMddyyyy",
                "yyyy.MM.dd", "dd.MM.yyyy", "MM.dd.yyyy"
            };

            return DateTime.TryParseExact(dateString, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out result);
        }

        private void checkBoxAllPrices_CheckedChanged(object sender, EventArgs e)
        {
            // 根据复选框状态启用或禁用日期选择控件
            dateTimePickerStart.Enabled = !checkBoxAllPrices.Checked;
            dateTimePickerEnd.Enabled = !checkBoxAllPrices.Checked;
            labelDateRange.Enabled = !checkBoxAllPrices.Checked;
            labelTo.Enabled = !checkBoxAllPrices.Checked;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_dataChanged)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        /// <summary>
        /// 批量处理列表的辅助方法
        /// </summary>
        private IEnumerable<List<T>> BatchList<T>(List<T> source, int batchSize)
        {
            for (int i = 0; i < source.Count; i += batchSize)
            {
                yield return source.Skip(i).Take(batchSize).ToList();
            }
        }

        #endregion
    }
}