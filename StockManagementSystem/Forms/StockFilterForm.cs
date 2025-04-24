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

        public StockFilterForm()
        {
            InitializeComponent();
            _stockService = new StockService();
        }

        private void StockFilterForm_Load(object sender, EventArgs e)
        {
            LoadStockTypes();
            LoadIndustries();
            LoadAllStocks();
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
            // 清空列表
            listViewFilteredStocks.Items.Clear();

            // 获取筛选条件
            string typeFilter = cboStockType.SelectedIndex > 0 ? cboStockType.SelectedItem.ToString() : null;
            string industryFilter = cboIndustry.SelectedIndex > 0 ? cboIndustry.SelectedItem.ToString() : null;
            string codeFilter = string.IsNullOrWhiteSpace(txtStockCode.Text) ? null : txtStockCode.Text.Trim();
            string nameFilter = string.IsNullOrWhiteSpace(txtStockName.Text) ? null : txtStockName.Text.Trim();

            // 筛选股票
            var filteredStocks = _allStocks.Where(s =>
                (typeFilter == null || s.Type == typeFilter) &&
                (industryFilter == null || s.Industry == industryFilter) &&
                (codeFilter == null || s.Code.Contains(codeFilter)) &&
                (nameFilter == null || s.Name.Contains(nameFilter))
            ).ToList();

            // 添加到列表中
            foreach (var stock in filteredStocks)
            {
                var item = new ListViewItem(stock.Code);
                item.SubItems.Add(stock.Name);
                item.SubItems.Add(stock.Type);
                item.SubItems.Add(stock.Industry);
                item.Tag = stock;
                listViewFilteredStocks.Items.Add(item);
            }

            // 更新结果计数
            lblResultCount.Text = $"共找到 {filteredStocks.Count} 条记录";
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
                SelectedStock = listViewFilteredStocks.SelectedItems[0].Tag as Stock;
                DialogResult = DialogResult.OK;
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
                SelectedStock = listViewFilteredStocks.SelectedItems[0].Tag as Stock;
                DialogResult = DialogResult.OK;
            }
        }
    }
}