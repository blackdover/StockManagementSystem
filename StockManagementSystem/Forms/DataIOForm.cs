using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockManagementSystem.Models;
using StockManagementSystem.Services;

namespace StockManagementSystem.Forms
{
    public partial class DataIOForm : Form
    {
        private readonly StockService _stockService;
        private readonly StockPriceService _stockPriceService;
        private bool _dataChanged = false;

        public DataIOForm()
        {
            InitializeComponent();
            _stockService = new StockService();
            _stockPriceService = new StockPriceService();
        }

        public DataIOForm(StockService stockService, StockPriceService stockPriceService)
        {
            InitializeComponent();
            _stockService = stockService;
            _stockPriceService = stockPriceService;
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
                    // 构建CSV内容
                    StringBuilder sb = new StringBuilder();

                    // 添加表头
                    sb.AppendLine("ID,股票代码,股票名称,股票类型,所属行业,上市日期,描述");

                    // 添加数据行
                    foreach (var stock in stocks)
                    {
                        sb.AppendLine($"{stock.StockId},{stock.Code},{stock.Name},{stock.Type},{stock.Industry},{stock.ListingDate:yyyy-MM-dd},{stock.Description?.Replace(",", " ")}");
                    }

                    // 写入文件
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                    // 更新状态
                    UpdateStatus($"成功导出 {stocks.Count} 条股票数据到 {saveFileDialog.FileName}");
                    MessageBox.Show($"成功导出 {stocks.Count} 条股票数据！", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    prices = _stockPriceService.GetStockPrices(null, DateTime.MinValue, DateTime.MaxValue);
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
                        sb.AppendLine($"{price.StockId},{stockCode},{price.TradeDate:yyyy-MM-dd},{price.OpenPrice},{price.ClosePrice},{price.HighPrice},{price.LowPrice},{price.Volume},{price.Amount},{price.ChangePercent}");
                    }

                    // 写入文件
                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                    // 更新状态
                    UpdateStatus($"成功导出 {prices.Count} 条股票价格数据到 {saveFileDialog.FileName}");
                    MessageBox.Show($"成功导出 {prices.Count} 条股票价格数据！", "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    // 读取文件内容
                    string[] lines = File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);
                    if (lines.Length <= 1)
                    {
                        MessageBox.Show("文件中没有有效的股票数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 检查表头
                    string header = lines[0].Trim();
                    if (!header.StartsWith("ID,股票代码,股票名称,股票类型,所属行业"))
                    {
                        MessageBox.Show("文件格式不正确！请确保文件包含正确的表头。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 获取现有的股票代码列表（用于检查重复）
                    var existingStocks = _stockService.GetAllStocks();
                    var existingCodes = existingStocks.Select(s => s.Code).ToHashSet();

                    // 解析并导入数据
                    int importedCount = 0;
                    int skippedCount = 0;
                    List<Stock> stocksToAdd = new List<Stock>();

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        try
                        {
                            // 解析CSV行
                            string[] fields = ParseCsvLine(line);
                            if (fields.Length < 6) continue;

                            string code = fields[1].Trim();

                            // 检查是否已存在
                            if (existingCodes.Contains(code))
                            {
                                skippedCount++;
                                continue;
                            }

                            // 创建股票对象
                            Stock stock = new Stock
                            {
                                Code = code,
                                Name = fields[2].Trim(),
                                Type = fields[3].Trim(),
                                Industry = fields[4].Trim(),
                                ListingDate = DateTime.TryParse(fields[5].Trim(), out DateTime listingDate) ? listingDate : DateTime.Now,
                                Description = fields.Length > 6 ? fields[6].Trim() : "",
                                CreateTime = DateTime.Now,
                                UpdateTime = DateTime.Now
                            };

                            stocksToAdd.Add(stock);
                            importedCount++;
                        }
                        catch
                        {
                            // 忽略解析错误的行
                            skippedCount++;
                        }
                    }

                    // 批量添加股票
                    if (stocksToAdd.Count > 0)
                    {
                        foreach (var stock in stocksToAdd)
                        {
                            _stockService.AddStock(stock);
                        }

                        _dataChanged = true;

                        // 更新状态
                        UpdateStatus($"已成功导入 {importedCount} 条股票数据，跳过 {skippedCount} 条重复或无效数据");
                        MessageBox.Show($"导入完成！\n成功导入：{importedCount} 条\n跳过重复或无效：{skippedCount} 条", "导入结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("没有新的股票数据需要导入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
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
                    // 读取文件内容
                    string[] lines = File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);
                    if (lines.Length <= 1)
                    {
                        MessageBox.Show("文件中没有有效的股票价格数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 检查表头
                    string header = lines[0].Trim();
                    if (!header.StartsWith("股票ID,股票代码,交易日期,开盘价,收盘价,最高价,最低价"))
                    {
                        MessageBox.Show("文件格式不正确！请确保文件包含正确的表头。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 获取所有股票（用于查询ID）
                    var stocks = _stockService.GetAllStocks();
                    var stockCodeToId = stocks.ToDictionary(s => s.Code, s => s.StockId);

                    // 获取现有的价格数据（用于检查重复）
                    var existingPrices = _stockPriceService.GetStockPrices(null, DateTime.MinValue, DateTime.MaxValue);
                    var existingPriceKeys = new HashSet<Tuple<int, DateTime>>();
                    foreach (var price in existingPrices)
                    {
                        existingPriceKeys.Add(new Tuple<int, DateTime>(price.StockId, price.TradeDate.Date));
                    }

                    // 解析并导入数据
                    int importedCount = 0;
                    int skippedCount = 0;
                    List<StockPrice> pricesToAdd = new List<StockPrice>();

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        try
                        {
                            // 解析CSV行
                            string[] fields = ParseCsvLine(line);
                            if (fields.Length < 8) continue;

                            int stockId;
                            string stockCode = fields[1].Trim();

                            // 尝试从第一列获取股票ID，如果失败则通过代码查找
                            if (!int.TryParse(fields[0].Trim(), out stockId) || !stocks.Any(s => s.StockId == stockId))
                            {
                                // 尝试通过股票代码查找ID
                                if (!stockCodeToId.ContainsKey(stockCode))
                                {
                                    skippedCount++;
                                    continue; // 找不到对应的股票，跳过
                                }
                                stockId = stockCodeToId[stockCode];
                            }

                            // 解析交易日期
                            if (!DateTime.TryParse(fields[2].Trim(), out DateTime tradeDate))
                            {
                                skippedCount++;
                                continue;
                            }

                            // 检查是否已存在相同股票ID和交易日期的记录
                            var key = new Tuple<int, DateTime>(stockId, tradeDate.Date);
                            if (existingPriceKeys.Contains(key))
                            {
                                skippedCount++;
                                continue;
                            }

                            // 解析价格数据
                            if (!decimal.TryParse(fields[3].Trim(), out decimal openPrice) ||
                                !decimal.TryParse(fields[4].Trim(), out decimal closePrice) ||
                                !decimal.TryParse(fields[5].Trim(), out decimal highPrice) ||
                                !decimal.TryParse(fields[6].Trim(), out decimal lowPrice) ||
                                !long.TryParse(fields[7].Trim(), out long volume))
                            {
                                skippedCount++;
                                continue;
                            }

                            // 解析可选字段
                            decimal amount = 0;
                            decimal changePercent = 0;

                            if (fields.Length > 8 && decimal.TryParse(fields[8].Trim(), out decimal parsedAmount))
                            {
                                amount = parsedAmount;
                            }

                            if (fields.Length > 9 && decimal.TryParse(fields[9].Trim(), out decimal parsedChangePercent))
                            {
                                changePercent = parsedChangePercent;
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

                            pricesToAdd.Add(price);
                            importedCount++;

                            // 添加到已存在集合，防止同一次导入中出现重复
                            existingPriceKeys.Add(key);
                        }
                        catch
                        {
                            // 忽略解析错误的行
                            skippedCount++;
                        }
                    }

                    // 批量添加价格数据
                    if (pricesToAdd.Count > 0)
                    {
                        foreach (var price in pricesToAdd)
                        {
                            _stockPriceService.AddStockPrice(price);
                        }

                        _dataChanged = true;

                        // 更新状态
                        UpdateStatus($"已成功导入 {importedCount} 条股票价格数据，跳过 {skippedCount} 条重复或无效数据");
                        MessageBox.Show($"导入完成！\n成功导入：{importedCount} 条\n跳过重复或无效：{skippedCount} 条", "导入结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("没有新的股票价格数据需要导入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
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

        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            StringBuilder field = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(c);
                }
            }

            // 添加最后一个字段
            result.Add(field.ToString());

            return result.ToArray();
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

        #endregion
    }
}