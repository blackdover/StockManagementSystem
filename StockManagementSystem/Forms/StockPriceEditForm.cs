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
    public partial class StockPriceEditForm : Form
    {
        private readonly StockService _stockService;
        private readonly StockPriceService _stockPriceService;
        private StockPrice _stockPrice;
        private bool _isAdd;

        public StockPriceEditForm(StockService stockService, StockPriceService stockPriceService, StockPrice stockPrice = null, int? stockId = null)
        {
            InitializeComponent();
            _stockService = stockService;
            _stockPriceService = stockPriceService;
            
            if (stockPrice == null)
            {
                _stockPrice = new StockPrice();
                if (stockId.HasValue)
                {
                    _stockPrice.StockId = stockId.Value;
                }
                _isAdd = true;
            }
            else
            {
                _stockPrice = stockPrice;
                _isAdd = false;
            }
        }

        private void StockPriceEditForm_Load(object sender, EventArgs e)
        {
            // 设置窗体标题
            this.Text = _isAdd ? "添加股票行情" : "编辑股票行情";

            // 加载股票列表
            LoadStockList();

            // 如果是编辑，则填充现有数据
            if (!_isAdd)
            {
                cboStock.SelectedValue = _stockPrice.StockId;
                dateTimePickerTradeDate.Value = _stockPrice.TradeDate;
                numOpenPrice.Value = _stockPrice.OpenPrice;
                numClosePrice.Value = _stockPrice.ClosePrice;
                numHighPrice.Value = _stockPrice.HighPrice;
                numLowPrice.Value = _stockPrice.LowPrice;
                numVolume.Value = _stockPrice.Volume;
                numAmount.Value = _stockPrice.Amount;
                numChangePercent.Value = _stockPrice.ChangePercent;
            }
            else
            {
                // 默认设置交易日期为当前日期
                dateTimePickerTradeDate.Value = DateTime.Now;
                
                // 如果有指定股票ID，则选中
                if (_stockPrice.StockId > 0)
                {
                    cboStock.SelectedValue = _stockPrice.StockId;
                }
            }
        }

        private void LoadStockList()
        {
            var stocks = _stockService.GetAllStocks();
            cboStock.DisplayMember = "Name";
            cboStock.ValueMember = "StockId";
            cboStock.DataSource = stocks;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (cboStock.SelectedValue == null)
            {
                MessageBox.Show("请选择股票", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboStock.Focus();
                return;
            }

            // 设置股票行情信息
            _stockPrice.StockId = (int)cboStock.SelectedValue;
            _stockPrice.TradeDate = dateTimePickerTradeDate.Value;
            _stockPrice.OpenPrice = numOpenPrice.Value;
            _stockPrice.ClosePrice = numClosePrice.Value;
            _stockPrice.HighPrice = numHighPrice.Value;
            _stockPrice.LowPrice = numLowPrice.Value;
            _stockPrice.Volume = (long)numVolume.Value;
            _stockPrice.Amount = numAmount.Value;
            _stockPrice.ChangePercent = numChangePercent.Value;

            bool success;
            if (_isAdd)
            {
                // 添加新行情
                success = _stockPriceService.AddStockPrice(_stockPrice);
            }
            else
            {
                // 更新现有行情
                success = _stockPriceService.UpdateStockPrice(_stockPrice);
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

        private void numHighPrice_ValueChanged(object sender, EventArgs e)
        {
            // 确保最高价不低于开盘价和收盘价
            if (numHighPrice.Value < numOpenPrice.Value)
            {
                numHighPrice.Value = numOpenPrice.Value;
            }
            if (numHighPrice.Value < numClosePrice.Value)
            {
                numHighPrice.Value = numClosePrice.Value;
            }
            
            // 确保最高价不低于最低价
            if (numHighPrice.Value < numLowPrice.Value)
            {
                numHighPrice.Value = numLowPrice.Value;
            }
        }

        private void numLowPrice_ValueChanged(object sender, EventArgs e)
        {
            // 确保最低价不高于开盘价和收盘价
            if (numLowPrice.Value > numOpenPrice.Value)
            {
                numLowPrice.Value = numOpenPrice.Value;
            }
            if (numLowPrice.Value > numClosePrice.Value)
            {
                numLowPrice.Value = numClosePrice.Value;
            }
            
            // 确保最低价不高于最高价
            if (numLowPrice.Value > numHighPrice.Value)
            {
                numLowPrice.Value = numHighPrice.Value;
            }
        }

        private void numClosePrice_ValueChanged(object sender, EventArgs e)
        {
            // 自动计算涨跌幅
            if (numOpenPrice.Value > 0)
            {
                decimal change = (numClosePrice.Value - numOpenPrice.Value) / numOpenPrice.Value * 100;
                numChangePercent.Value = Math.Round(change, 2);
            }
            
            // 更新最高价和最低价
            if (numClosePrice.Value > numHighPrice.Value)
            {
                numHighPrice.Value = numClosePrice.Value;
            }
            if (numClosePrice.Value < numLowPrice.Value)
            {
                numLowPrice.Value = numClosePrice.Value;
            }
        }
    }
} 