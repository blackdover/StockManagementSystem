using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Models;
using StockManagementSystem.Services;

namespace StockManagementSystem.Forms
{
    public partial class StockPriceQueryForm : Form
    {
        private readonly StockService _stockService;
        private readonly StockPriceService _stockPriceService;
        private StockPrice _selectedPrice;
        private List<Stock> _stocks;
        private List<StockPrice> currentPrices;
        private Label lblStatistics;

        /// <summary>
        /// 预设的股票ID，用于初始化时自动选择
        /// </summary>
        public int? PresetStockId { get; set; }

        public StockPriceQueryForm(StockService stockService, StockPriceService stockPriceService)
        {
            InitializeComponent();
            _stockService = stockService;
            _stockPriceService = stockPriceService;
            PresetStockId = null;
            _selectedPrice = null;

            // 初始化统计结果面板
            InitializeStatisticsPanel();

            // 设置DataGridView属性以自动调整列宽
            ConfigureDataGridView();
        }

        /// <summary>
        /// 配置DataGridView的显示属性
        /// </summary>
        private void ConfigureDataGridView()
        {
            // 设置自动调整列宽
            dataGridViewPrices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 可选：根据内容调整列宽（如果Fill模式不满足需求）
            // dataGridViewPrices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // 设置行高自动调整
            dataGridViewPrices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // 设置调整大小时刷新显示
            dataGridViewPrices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 设置选中整行
            dataGridViewPrices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        /// 初始化统计结果面板
        /// </summary>
        private void InitializeStatisticsPanel()
        {
            // 创建标签显示统计结果
            Label lblStatistics = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                Text = "请选择特定股票以显示统计分析",
                Padding = new Padding(10)
            };
            panelStatistics.Controls.Add(lblStatistics);
            this.lblStatistics = lblStatistics;
        }

        private void StockPriceQueryForm_Load(object sender, EventArgs e)
        {
            // 加载股票列表
            LoadStockList();

            // 设置默认日期范围：最近30天
            dateTimePickerEnd.Value = DateTime.Now;
            dateTimePickerStart.Value = DateTime.Now.AddDays(-30);

            // 确保按钮可见
            EnsureButtonsVisible();

            // 自动触发查询
            if (cboStock.Items.Count > 0)
            {
                PerformQuery();
            }
        }

        /// <summary>
        /// 确保所有按钮正确显示
        /// </summary>
        private void EnsureButtonsVisible()
        {
            // 确保所有按钮可见
            btnAdd.Visible = true;
            btnEdit.Visible = true;
            btnDelete.Visible = true;
            btnExport.Visible = true;

            // 不再需要调整位置，位置已在Designer中设置
        }

        /// <summary>
        /// 加载股票列表
        /// </summary>
        private void LoadStockList()
        {
            // 使用StockService加载股票列表
            var stocks = _stockService.GetAllStocks();

            // 添加"全部"选项
            var allStocks = new List<Stock>
            {
                new Stock { StockId = 0, Code = "全部", Name = "全部" }
            };
            allStocks.AddRange(stocks);

            // 设置数据源
            _stocks = stocks;

            // 创建ComboBox的显示项
            cboStock.Items.Clear();
            foreach (var stock in allStocks)
            {
                string displayText = stock.StockId == 0 ? "全部" : $"{stock.Code} - {stock.Name}";
                ComboBoxItem item = new ComboBoxItem(stock.StockId, displayText, stock);
                cboStock.Items.Add(item);
            }

            cboStock.DisplayMember = "Text";
            cboStock.ValueMember = "Value";

            // 如果有预设的股票ID，选择它
            if (PresetStockId.HasValue)
            {
                for (int i = 0; i < cboStock.Items.Count; i++)
                {
                    var item = cboStock.Items[i] as ComboBoxItem;
                    if (item != null && item.Value == PresetStockId.Value)
                    {
                        cboStock.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                // 默认选择第一项
                if (cboStock.Items.Count > 0)
                    cboStock.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 用于ComboBox的项目类
        /// </summary>
        private class ComboBoxItem
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public object Tag { get; set; }

            public ComboBoxItem(int value, string text, object tag = null)
            {
                Value = value;
                Text = text;
                Tag = tag;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void cboStock_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // 股票更改时自动查询
                PerformQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更改股票选择时发生错误: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 日期更改时自动查询
                PerformQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更改日期选择时发生错误: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        private void PerformQuery()
        {
            // 检查是否已选择股票
            if (cboStock.SelectedItem == null)
            {
                MessageBox.Show("请先选择股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 获取选中的项
            var selectedItem = cboStock.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            var stock = selectedItem.Tag as Stock;
            if (stock == null) return;

            // 获取日期范围并确保在SQL Server支持的范围内
            DateTime startDate = DateTimeHelper.EnsureSqlDateRange(dateTimePickerStart.Value.Date);
            DateTime endDate = DateTimeHelper.EnsureSqlDateRange(dateTimePickerEnd.Value.Date.AddDays(1)); // 设置为当天最后一秒

            if (startDate > endDate)
            {
                MessageBox.Show("开始日期不能大于结束日期！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int stockId = stock.StockId;
                // 如果选择了"全部"选项，则传递null作为股票ID
                if (stockId == 0)
                {
                    currentPrices = _stockPriceService.GetStockPrices(null, startDate, endDate);
                }
                else
                {
                    currentPrices = _stockPriceService.GetStockPrices(stockId, startDate, endDate);
                }

                DisplayQueryResults();

                // 自动显示统计分析结果
                DisplayStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查询失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayStatistics()
        {
            if (currentPrices == null || currentPrices.Count == 0)
            {
                panelStatistics.Controls.Clear();
                Label noDataLabel = new Label();
                noDataLabel.Text = "没有数据可以进行统计分析";
                noDataLabel.AutoSize = true;
                noDataLabel.Location = new Point(10, 10);
                panelStatistics.Controls.Add(noDataLabel);
                return;
            }

            panelStatistics.Controls.Clear();

            // 创建统计信息标签
            Label lblStatistics = new Label();
            lblStatistics.AutoSize = true;
            lblStatistics.Font = new Font(lblStatistics.Font, FontStyle.Bold);
            lblStatistics.Text = "统计分析结果";
            lblStatistics.Location = new Point(10, 10);
            panelStatistics.Controls.Add(lblStatistics);

            // 计算统计数据
            decimal highest = currentPrices.Max(p => p.HighestPrice);
            decimal lowest = currentPrices.Min(p => p.LowestPrice);
            decimal avgClose = currentPrices.Average(p => p.ClosePrice);
            decimal avgVolume = (decimal)currentPrices.Average(p => (double)p.Volume);
            decimal totalVolume = currentPrices.Sum(p => p.Volume);

            // 计算涨跌幅
            if (currentPrices.Count >= 2)
            {
                var orderedPrices = currentPrices.OrderBy(p => p.Date).ToList();
                decimal firstPrice = orderedPrices.First().ClosePrice;
                decimal lastPrice = orderedPrices.Last().ClosePrice;
                decimal changePercent = (lastPrice - firstPrice) / firstPrice * 100;

                // 添加涨跌幅标签
                Label lblChange = new Label();
                lblChange.AutoSize = true;
                lblChange.Text = $"期间涨跌幅: {changePercent:F2}%";
                lblChange.Location = new Point(10, 40);
                lblChange.ForeColor = changePercent >= 0 ? Color.Red : Color.Green;
                panelStatistics.Controls.Add(lblChange);
            }

            // 添加其他统计信息
            Label lblHighest = new Label();
            lblHighest.AutoSize = true;
            lblHighest.Text = $"最高价: {highest:F2}";
            lblHighest.Location = new Point(10, 70);
            panelStatistics.Controls.Add(lblHighest);

            Label lblLowest = new Label();
            lblLowest.AutoSize = true;
            lblLowest.Text = $"最低价: {lowest:F2}";
            lblLowest.Location = new Point(10, 100);
            panelStatistics.Controls.Add(lblLowest);

            Label lblAvgClose = new Label();
            lblAvgClose.AutoSize = true;
            lblAvgClose.Text = $"平均收盘价: {avgClose:F2}";
            lblAvgClose.Location = new Point(10, 130);
            panelStatistics.Controls.Add(lblAvgClose);

            Label lblAvgVolume = new Label();
            lblAvgVolume.AutoSize = true;
            lblAvgVolume.Text = $"平均成交量: {avgVolume:F0}";
            lblAvgVolume.Location = new Point(250, 70);
            panelStatistics.Controls.Add(lblAvgVolume);

            Label lblTotalVolume = new Label();
            lblTotalVolume.AutoSize = true;
            lblTotalVolume.Text = $"总成交量: {totalVolume:F0}";
            lblTotalVolume.Location = new Point(250, 100);
            panelStatistics.Controls.Add(lblTotalVolume);
        }

        /// <summary>
        /// 显示查询结果
        /// </summary>
        private void DisplayQueryResults()
        {
            // 创建DataTable
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("股票代码", typeof(string));
            dt.Columns.Add("股票名称", typeof(string));
            dt.Columns.Add("日期", typeof(DateTime));
            dt.Columns.Add("开盘价", typeof(decimal));
            dt.Columns.Add("最高价", typeof(decimal));
            dt.Columns.Add("最低价", typeof(decimal));
            dt.Columns.Add("收盘价", typeof(decimal));
            dt.Columns.Add("涨跌幅", typeof(string));
            dt.Columns.Add("成交量", typeof(decimal));

            // 添加数据行
            foreach (var price in currentPrices)
            {
                // 获取股票信息
                var stock = price.Stock;
                if (stock == null) continue;

                // 计算涨跌幅 (使用ChangePercent属性或自行计算)
                decimal changePercent = price.ChangePercent;
                if (changePercent == 0 && price.PrevClosePrice > 0)
                {
                    changePercent = (price.ClosePrice - price.PrevClosePrice) / price.PrevClosePrice * 100;
                }
                string changePercentStr = $"{changePercent:F2}%";

                // 添加行数据
                dt.Rows.Add(
                    price.PriceId,
                    stock.Code,
                    stock.Name,
                    price.TradeDate,
                    price.OpenPrice,
                    price.HighestPrice,  // 修正变量名 HighPrice -> HighestPrice
                    price.LowestPrice,   // 修正变量名 LowPrice -> LowestPrice
                    price.ClosePrice,
                    changePercentStr,
                    price.Volume
                );
            }

            // 使用BindingSource绑定数据
            var bindingSource = new BindingSource();
            bindingSource.DataSource = dt;
            dataGridViewPrices.DataSource = bindingSource;

            // 隐藏ID列
            if (dataGridViewPrices.Columns["Id"] != null)
            {
                dataGridViewPrices.Columns["Id"].Visible = false;
            }

            // 设置日期列格式
            if (dataGridViewPrices.Columns["日期"] != null)
            {
                dataGridViewPrices.Columns["日期"].DefaultCellStyle.Format = "yyyy-MM-dd";
            }

            // 设置价格列的格式
            string[] priceColumns = { "开盘价", "最高价", "最低价", "收盘价" };
            foreach (string columnName in priceColumns)
            {
                if (dataGridViewPrices.Columns[columnName] != null)
                {
                    dataGridViewPrices.Columns[columnName].DefaultCellStyle.Format = "F2";  // 显示两位小数
                }
            }

            // 设置成交量列的格式
            if (dataGridViewPrices.Columns["成交量"] != null)
            {
                dataGridViewPrices.Columns["成交量"].DefaultCellStyle.Format = "N0";  // 显示千位分隔符
            }

            // 自动调整列宽以适应内容
            dataGridViewPrices.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            // 然后设置为Fill模式以填充可用空间
            dataGridViewPrices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 更新结果计数
            lblResultCount.Text = $"查询结果：共{currentPrices.Count}条记录";

            // 重置选中的价格并更新按钮状态
            _selectedPrice = null;
            UpdateButtonStates();
        }

        /// <summary>
        /// 导出数据按钮点击事件
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dataGridViewPrices.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV文件(*.csv)|*.csv",
                Title = "导出数据",
                FileName = $"股票行情数据_{DateTime.Now:yyyyMMdd}"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 导出表头
                    List<string> headers = new List<string>();
                    for (int i = 1; i < dataGridViewPrices.Columns.Count; i++) // 跳过ID列
                    {
                        if (dataGridViewPrices.Columns[i].Visible)
                        {
                            headers.Add(dataGridViewPrices.Columns[i].HeaderText);
                        }
                    }

                    // 导出数据行
                    List<string> lines = new List<string> { string.Join(",", headers) };
                    foreach (DataGridViewRow row in dataGridViewPrices.Rows)
                    {
                        List<string> cells = new List<string>();
                        for (int i = 1; i < dataGridViewPrices.Columns.Count; i++) // 跳过ID列
                        {
                            if (dataGridViewPrices.Columns[i].Visible)
                            {
                                string value = row.Cells[i].Value?.ToString() ?? "";
                                // 如果值包含逗号，用引号括起来
                                if (value.Contains(","))
                                {
                                    value = $"\"{value}\"";
                                }
                                cells.Add(value);
                            }
                        }
                        lines.Add(string.Join(",", cells));
                    }

                    // 写入文件
                    System.IO.File.WriteAllLines(saveFileDialog.FileName, lines, Encoding.UTF8);
                    MessageBox.Show("数据导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 数据表选择变更事件
        /// </summary>
        private void dataGridViewPrices_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewPrices.SelectedRows.Count > 0 && currentPrices != null)
                {
                    DataGridViewRow row = dataGridViewPrices.SelectedRows[0];
                    if (row.Cells["Id"] != null && row.Cells["Id"].Value != null)
                    {
                        int priceId = Convert.ToInt32(row.Cells["Id"].Value);
                        _selectedPrice = currentPrices.FirstOrDefault(p => p.PriceId == priceId);
                    }
                }
                else
                {
                    _selectedPrice = null;
                }

                // 更新按钮状态
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"选择行情数据时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 更新按钮启用状态
        /// </summary>
        private void UpdateButtonStates()
        {
            bool hasSelected = _selectedPrice != null;
            btnEdit.Enabled = hasSelected;
            btnDelete.Enabled = hasSelected;

            // 确保按钮可见
            btnEdit.Visible = true;
            btnDelete.Visible = true;
        }

        /// <summary>
        /// 编辑行情按钮点击事件
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // 检查是否已选择行情数据
            if (_selectedPrice == null)
            {
                MessageBox.Show("请先选择要编辑的行情数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 创建编辑行情对话框
            using (var editForm = new StockPriceEditForm(_stockService, _stockPriceService, _selectedPrice))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 重新查询刷新数据
                        PerformQuery();
                        MessageBox.Show("行情数据已成功更新", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"更新后刷新数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 删除行情按钮点击事件
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 检查是否已选择行情数据
            if (_selectedPrice == null)
            {
                MessageBox.Show("请先选择要删除的行情数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 确认删除
            if (MessageBox.Show($"确定要删除 {_selectedPrice.TradeDate.ToShortDateString()} 的行情数据吗？", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // 删除行情数据
                    _stockPriceService.DeleteStockPrice(_selectedPrice.PriceId);
                    MessageBox.Show("行情数据已成功删除", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 重新查询刷新数据
                    PerformQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 添加按钮点击事件
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 获取当前选择的股票
            int? selectedStockId = null;
            if (cboStock.SelectedItem is ComboBoxItem item && item.Value != 0) // 不是"全部"选项
            {
                selectedStockId = item.Value;
            }

            // 创建添加行情对话框
            using (var addForm = new StockPriceEditForm(_stockService, _stockPriceService, null, selectedStockId))
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 重新查询刷新数据
                        PerformQuery();
                        // MessageBox.Show("行情数据已成功添加", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"添加后刷新数据失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}