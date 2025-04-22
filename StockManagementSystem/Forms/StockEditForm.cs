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

        // 预定义的股票类型选项
        private readonly string[] stockTypes = new string[]
        {
            "A股",
            "B股",
            "H股",
            "N股",
            "S股",
            "创业板",
            "科创板",
            "新三板"
        };

        // 预定义的行业选项
        private readonly string[] industries = new string[]
        {
            "金融",
            "科技",
            "医药",
            "房地产",
            "能源",
            "消费品",
            "制造业",
            "电信",
            "互联网",
            "汽车",
            "医疗",
            "教育",
            "餐饮",
            "零售",
            "传媒",
            "娱乐",
            "其他"
        };

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

            // 填充股票类型下拉框
            cboType.Items.Clear();
            cboType.Items.AddRange(stockTypes);

            // 填充行业下拉框
            cboIndustry.Items.Clear();
            cboIndustry.Items.AddRange(industries);

            // 如果是编辑，则填充现有数据
            if (!_isAdd)
            {
                txtCode.Text = _stock.Code;
                txtName.Text = _stock.Name;

                // 设置股票类型下拉框选中项
                if (!string.IsNullOrEmpty(_stock.Type))
                {
                    int typeIndex = Array.IndexOf(stockTypes, _stock.Type);
                    if (typeIndex >= 0)
                        cboType.SelectedIndex = typeIndex;
                    else
                        cboType.Text = _stock.Type; // 如果不在预定义列表中，直接显示文本
                }

                // 设置行业下拉框选中项
                if (!string.IsNullOrEmpty(_stock.Industry))
                {
                    int industryIndex = Array.IndexOf(industries, _stock.Industry);
                    if (industryIndex >= 0)
                        cboIndustry.SelectedIndex = industryIndex;
                    else
                        cboIndustry.Text = _stock.Industry; // 如果不在预定义列表中，直接显示文本
                }

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
            _stock.Type = cboType.Text.Trim();
            _stock.Industry = cboIndustry.Text.Trim();
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