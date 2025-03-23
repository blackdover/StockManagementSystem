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
    public partial class StockEditForm : Form
    {
        private readonly StockService _stockService;
        private Stock _stock;
        private bool _isAdd;

        public StockEditForm(StockService stockService, Stock stock = null)
        {
            InitializeComponent();
            _stockService = stockService;
            _stock = stock ?? new Stock();
            _isAdd = stock == null;
        }

        private void StockEditForm_Load(object sender, EventArgs e)
        {
            // 设置窗体标题
            this.Text = _isAdd ? "添加股票信息" : "编辑股票信息";

            // 如果是编辑，则填充现有数据
            if (!_isAdd)
            {
                txtCode.Text = _stock.Code;
                txtName.Text = _stock.Name;
                txtType.Text = _stock.Type;
                txtIndustry.Text = _stock.Industry;
                dateTimePickerListingDate.Value = _stock.ListingDate;
                txtDescription.Text = _stock.Description;
            }
            else
            {
                // 默认设置上市日期为当前日期
                dateTimePickerListingDate.Value = DateTime.Now;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("请输入股票代码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("请输入股票名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // 设置股票信息
            _stock.Code = txtCode.Text.Trim();
            _stock.Name = txtName.Text.Trim();
            _stock.Type = txtType.Text.Trim();
            _stock.Industry = txtIndustry.Text.Trim();
            _stock.ListingDate = dateTimePickerListingDate.Value;
            _stock.Description = txtDescription.Text.Trim();

            bool success;
            if (_isAdd)
            {
                // 添加新股票
                success = _stockService.AddStock(_stock);
            }
            else
            {
                // 更新现有股票
                success = _stockService.UpdateStock(_stock);
            }

            if (success)
            {
                MessageBox.Show(_isAdd ? "添加成功！" : "更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(_isAdd ? "添加失败！" : "更新失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
} 