using System;
using System.Windows.Forms;
using StockManagementSystem.Models;
using StockManagementSystem.Services;
using System.Collections.Generic;
using System.Linq;

namespace StockManagementSystem.Forms
{
    public partial class StockEditForm : Form
    {
        private readonly StockService _stockService;
        private Stock _stock;
        private bool _isAdd;

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
            foreach (StockType type in Enum.GetValues(typeof(StockType)))
            {
                cboType.Items.Add(type.ToString());
            }

            // 填充行业下拉框
            cboIndustry.Items.Clear();

            // 获取数据库中的所有行业
            List<string> dbIndustries = _stockService.GetAllIndustries();

            // 合并预定义行业和数据库中的行业
            HashSet<string> allIndustries = new HashSet<string>(industries);
            foreach (string industry in dbIndustries)
            {
                allIndustries.Add(industry);
            }

            // 添加所有行业到下拉框
            cboIndustry.Items.AddRange(allIndustries.OrderBy(i => i).ToArray());

            // 设置行业下拉框为可编辑模式，允许输入不在预定义列表中的选项
            cboIndustry.DropDownStyle = ComboBoxStyle.DropDown;

            // 如果是编辑，则填充现有数据
            if (!_isAdd)
            {
                txtCode.Text = _stock.Code;
                txtName.Text = _stock.Name;

                // 设置股票类型下拉框选中项
                if (!string.IsNullOrEmpty(_stock.Type))
                {
                    cboType.Text = _stock.Type;
                }

                // 设置行业下拉框选中项
                if (!string.IsNullOrEmpty(_stock.Industry))
                {
                    cboIndustry.Text = _stock.Industry; // 直接设置文本，无论是否在列表中
                }

                dateTimePickerListingDate.Value = _stock.ListingDate;
                txtDescription.Text = _stock.Description;
            }
            else
            {
                // 默认设置上市日期为当前日期
                dateTimePickerListingDate.Value = DateTime.Now;
                // 默认选择A股
                cboType.SelectedIndex = 0;
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

            // 设置股票类型 - 使用新的枚举属性
            if (Enum.TryParse(cboType.Text.Trim(), out StockType stockType))
            {
                _stock.StockTypeEnum = stockType;
            }
            else
            {
                _stock.Type = cboType.Text.Trim();
            }

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