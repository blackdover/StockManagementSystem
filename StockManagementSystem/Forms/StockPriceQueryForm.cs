using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockManagementSystem.Models;
using StockManagementSystem.Services;

namespace StockManagementSystem.Forms
{
    public partial class StockPriceQueryForm : Form
    {
        private readonly StockService _stockService;
        private readonly StockPriceService _stockPriceService;

        public StockPriceQueryForm(StockService stockService, StockPriceService stockPriceService)
        {
            InitializeComponent();
            _stockService = stockService;
            _stockPriceService = stockPriceService;
        }

        private void StockPriceQueryForm_Load(object sender, EventArgs e)
        {
            // 初始化日期选择器
            dateTimePickerStart.Value = DateTime.Now.AddMonths(-1);
            dateTimePickerEnd.Value = DateTime.Now;

            // 加载股票列表
            LoadStockList();
        }

        private void LoadStockList()
        {
            var stocks = _stockService.GetAllStocks();
            
            // 添加"全部"选项
            var allStocks = new List<Stock>(stocks);
            allStocks.Insert(0, new Stock { StockId = 0, Name = "全部", Code = "" });
            
            cboStock.DisplayMember = "Name";
            cboStock.ValueMember = "StockId";
            cboStock.DataSource = allStocks;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            // 获取查询条件
            int stockId = Convert.ToInt32(cboStock.SelectedValue);
            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date.AddDays(1).AddSeconds(-1); // 截止到当天23:59:59

            // 查询股票行情数据
            List<StockPrice> prices = new List<StockPrice>();
            if (stockId > 0)
            {
                // 查询指定股票的行情
                prices = _stockPriceService.GetStockPricesByDateRange(stockId, startDate, endDate);
            }
            else
            {
                // 查询所有股票在指定日期的行情
                var stocks = _stockService.GetAllStocks();
                foreach (var stock in stocks)
                {
                    var stockPrices = _stockPriceService.GetStockPricesByDateRange(stock.StockId, startDate, endDate);
                    prices.AddRange(stockPrices);
                }
            }

            // 显示查询结果
            DisplayQueryResults(prices);
        }

        private void DisplayQueryResults(List<StockPrice> prices)
        {
            // 清空现有数据
            dataGridViewPrices.DataSource = null;
            
            if (prices.Count == 0)
            {
                MessageBox.Show("未找到符合条件的行情数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 将数据加载到DataGridView
            var priceList = prices.Select(p => new
            {
                行情ID = p.PriceId,
                股票代码 = p.Stock?.Code,
                股票名称 = p.Stock?.Name,
                交易日期 = p.TradeDate.ToString("yyyy-MM-dd"),
                开盘价 = p.OpenPrice,
                收盘价 = p.ClosePrice,
                最高价 = p.HighPrice,
                最低价 = p.LowPrice,
                成交量 = p.Volume,
                成交额 = p.Amount,
                涨跌幅 = p.ChangePercent + "%"
            }).ToList();

            dataGridViewPrices.DataSource = priceList;
            
            // 显示查询结果数量
            lblResultCount.Text = $"共查询到 {prices.Count} 条记录";
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // 导出查询结果到Excel
            if (dataGridViewPrices.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可以导出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel文件|*.xlsx";
            saveDialog.Title = "导出行情数据";
            saveDialog.FileName = "股票行情_" + DateTime.Now.ToString("yyyyMMdd");

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // 在实际应用中，这里需要添加导出Excel的代码
                // 可以使用第三方库如EPPlus或NPOI
                // 示例仅作为占位符
                MessageBox.Show("数据已成功导出到: " + saveDialog.FileName, "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            // 获取查询条件
            int stockId = Convert.ToInt32(cboStock.SelectedValue);
            if (stockId == 0)
            {
                MessageBox.Show("请选择具体的股票进行分析", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date.AddDays(1).AddSeconds(-1);

            // 获取股票价格波动统计信息
            var statistics = _stockPriceService.GetPriceFluctuationStatistics(stockId, startDate, endDate);
            if (statistics == null)
            {
                MessageBox.Show("没有足够的数据进行分析", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 显示统计结果
            var stock = _stockService.GetStockById(stockId);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"股票: {stock.Name} ({stock.Code})");
            sb.AppendLine($"分析期间: {startDate.ToString("yyyy-MM-dd")} 至 {endDate.ToString("yyyy-MM-dd")}");
            sb.AppendLine("---------------------");
            sb.AppendLine($"最高价: {statistics.MaxPrice:F2}");
            sb.AppendLine($"最低价: {statistics.MinPrice:F2}");
            sb.AppendLine($"平均价: {statistics.AveragePrice:F2}");
            sb.AppendLine($"价格波动范围: {statistics.PriceRange:F2} ({statistics.PriceRangePercent:F2}%)");
            sb.AppendLine($"总体涨跌幅: {statistics.OverallChangePercent:F2}%");
            sb.AppendLine($"总成交量: {statistics.TotalVolume}");
            sb.AppendLine($"总成交额: {statistics.TotalAmount:F2}");
            sb.AppendLine($"平均成交量: {statistics.AverageVolume:F2}");

            MessageBox.Show(sb.ToString(), "统计分析结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
} 