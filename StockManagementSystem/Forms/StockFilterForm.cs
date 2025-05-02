using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using StockManagementSystem.Models;
using StockManagementSystem.Services;

namespace StockManagementSystem
{
    public partial class StockFilterForm : Form
    {
        private readonly StockService _stockService;
        private List<Stock> _allStocks;

        public Stock SelectedStock { get; private set; }

        /// <summary>
        /// 标记是否有删除操作发生
        /// </summary>
        public bool HasDeletedRecords { get; private set; } = false;

        public StockFilterForm()
        {
            InitializeComponent();
            _stockService = new StockService();
        }

        private void StockFilterForm_Load(object sender, EventArgs e)
        {
            LoadStockTypes();
            LoadIndustries();

            // 设置列表视图的列标题
            listViewFilteredStocks.Columns[0].Text = "股票代码";
            listViewFilteredStocks.Columns[1].Text = "股票名称";
            listViewFilteredStocks.Columns[2].Text = "股票类型";
            listViewFilteredStocks.Columns[3].Text = "所属行业";

            // 应用默认筛选（加载所有股票）
            ApplyFilters();
        }

        private void LoadStockTypes()
        {
            // 获取所有股票类型
            var allStocks = _stockService.GetAllStocks();
            var types = allStocks.Select(s => s.Type).Where(t => !string.IsNullOrEmpty(t)).Distinct().OrderBy(t => t).ToList();

            // 添加"全部"选项
            cboStockType.Items.Add("全部");

            // 添加股票类型
            foreach (var type in types)
            {
                cboStockType.Items.Add(type);
            }

            // 默认选择"全部"
            cboStockType.SelectedIndex = 0;
        }

        private void LoadIndustries()
        {
            // 获取所有行业
            var allStocks = _stockService.GetAllStocks();
            var industries = allStocks.Select(s => s.Industry).Where(i => !string.IsNullOrEmpty(i)).Distinct().OrderBy(i => i).ToList();

            // 添加"全部"选项
            cboIndustry.Items.Add("全部");

            // 添加行业
            foreach (var industry in industries)
            {
                cboIndustry.Items.Add(industry);
            }

            // 默认选择"全部"
            cboIndustry.SelectedIndex = 0;
        }

        private void LoadAllStocks()
        {
            _allStocks = _stockService.GetAllStocks();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            // 获取筛选条件
            string stockType = cboStockType.SelectedIndex > 0 ? cboStockType.SelectedItem.ToString() : null;
            string industry = cboIndustry.SelectedIndex > 0 ? cboIndustry.SelectedItem.ToString() : null;
            string stockCode = string.IsNullOrWhiteSpace(txtStockCode.Text) ? null : txtStockCode.Text.Trim();
            string stockName = string.IsNullOrWhiteSpace(txtStockName.Text) ? null : txtStockName.Text.Trim();

            // 清空现有结果
            listViewFilteredStocks.Items.Clear();

            // 应用筛选
            IEnumerable<StockViewModel> filteredStocks = _stockService.GetFilteredStocks(stockType, industry, stockCode, stockName);
            filteredStocks = filteredStocks.OrderBy(s => s.Code);

            // 显示筛选结果
            foreach (var stock in filteredStocks)
            {
                ListViewItem item = new ListViewItem(stock.Code);
                item.SubItems.Add(stock.Name);
                item.SubItems.Add(stock.Type);
                item.SubItems.Add(stock.Industry);
                item.Tag = stock; // 将股票对象关联到ListViewItem
                listViewFilteredStocks.Items.Add(item);
            }

            // 更新结果计数
            lblResultCount.Text = $"找到 {listViewFilteredStocks.Items.Count} 条记录";
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // 重置筛选条件
            cboStockType.SelectedIndex = 0;
            cboIndustry.SelectedIndex = 0;
            txtStockCode.Text = string.Empty;
            txtStockName.Text = string.Empty;

            // 重新应用筛选
            ApplyFilters();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 获取选中的股票
            if (listViewFilteredStocks.SelectedItems.Count > 0)
            {
                var stockViewModel = listViewFilteredStocks.SelectedItems[0].Tag as StockViewModel;
                if (stockViewModel != null)
                {
                    try
                    {
                        // 获取完整的Stock对象
                        SelectedStock = _stockService.GetStockById(stockViewModel.Id);

                        // 检查是否成功获取到Stock对象
                        if (SelectedStock == null)
                        {
                            MessageBox.Show($"无法获取ID为{stockViewModel.Id}的股票详细信息", "数据错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 设置对话框结果并关闭
                        DialogResult = DialogResult.OK;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"获取股票信息时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("股票数据异常，请重新选择", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("请选择一个股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void listViewFilteredStocks_DoubleClick(object sender, EventArgs e)
        {
            // 双击选择股票并确认
            if (listViewFilteredStocks.SelectedItems.Count > 0)
            {
                var stockViewModel = listViewFilteredStocks.SelectedItems[0].Tag as StockViewModel;
                if (stockViewModel != null)
                {
                    try
                    {
                        // 获取完整的Stock对象
                        SelectedStock = _stockService.GetStockById(stockViewModel.Id);

                        // 检查是否成功获取到Stock对象
                        if (SelectedStock == null)
                        {
                            MessageBox.Show($"无法获取ID为{stockViewModel.Id}的股票详细信息", "数据错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 设置对话框结果并关闭
                        DialogResult = DialogResult.OK;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"获取股票信息时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 全选按钮点击事件处理
        /// </summary>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewFilteredStocks.Items)
            {
                item.Checked = true;
            }
        }

        /// <summary>
        /// 删除按钮点击事件处理
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 获取选中的项目
            List<ListViewItem> checkedItems = new List<ListViewItem>();
            foreach (ListViewItem item in listViewFilteredStocks.Items)
            {
                if (item.Checked)
                {
                    checkedItems.Add(item);
                }
            }

            // 如果没有选中任何项目，显示提示并返回
            if (checkedItems.Count == 0)
            {
                MessageBox.Show("请至少选择一项进行删除！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 显示确认对话框
            DialogResult result = MessageBox.Show($"确定要删除选中的 {checkedItems.Count} 项股票记录吗？此操作不可撤销。",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            // 如果用户确认删除
            if (result == DialogResult.Yes)
            {
                int successCount = 0;
                List<string> failedStocks = new List<string>();

                // 删除每个选中的股票
                foreach (ListViewItem item in checkedItems)
                {
                    StockViewModel stock = (StockViewModel)item.Tag;
                    try
                    {
                        bool deleteResult = _stockService.DeleteStock(stock.Id);
                        if (deleteResult)
                        {
                            successCount++;
                            listViewFilteredStocks.Items.Remove(item);
                            HasDeletedRecords = true; // 标记有删除操作发生
                        }
                        else
                        {
                            failedStocks.Add($"{stock.Code} - {stock.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        failedStocks.Add($"{stock.Code} - {stock.Name} (错误: {ex.Message})");
                    }
                }

                // 更新结果计数
                lblResultCount.Text = $"找到 {listViewFilteredStocks.Items.Count} 条记录";

                // 显示操作结果
                if (failedStocks.Count == 0)
                {
                    MessageBox.Show($"成功删除 {successCount} 项股票记录。", "删除成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string failedMessage = string.Join("\n", failedStocks);
                    MessageBox.Show($"删除操作完成。成功: {successCount} 项，失败: {failedStocks.Count} 项。\n\n删除失败的股票:\n{failedMessage}",
                        "删除部分成功", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}