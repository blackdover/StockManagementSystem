using System;
using System.Linq;
using System.Windows.Forms;
using StockManagementSystem.Services.Helpers;
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
        private bool _isEditMode;
        private int _priceId;

        /// <summary>
        /// 获取编辑后的股票行情对象
        /// </summary>
        public StockPrice StockPrice => _stockPrice;

        #region 构造函数

        /// <summary>
        /// 基于依赖注入的构造函数
        /// </summary>
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
                _isEditMode = false;
            }
            else
            {
                _stockPrice = stockPrice;
                _isAdd = false;
                _isEditMode = true;
                _priceId = stockPrice.PriceId;
            }
        }

        /// <summary>
        /// 添加模式的简化构造函数
        /// </summary>
        public StockPriceEditForm()
        {
            InitializeComponent();
            _isEditMode = false;
            _isAdd = true;
            _priceId = 0;
            _stockPrice = new StockPrice();

            this.Text = "添加股票行情";

            // 设置默认日期为今天
            dateTimePickerTradeDate.Value = DateTime.Today;

            // 允许选择股票
            cboStock.Enabled = true;
        }

        /// <summary>
        /// 编辑模式的简化构造函数
        /// </summary>
        public StockPriceEditForm(int priceId)
        {
            InitializeComponent();
            _isEditMode = true;
            _isAdd = false;
            _priceId = priceId;

            this.Text = "编辑股票行情";

            // 编辑模式下不允许更改股票
            cboStock.Enabled = false;
        }

        /// <summary>
        /// 编辑模式的简化构造函数
        /// </summary>
        public StockPriceEditForm(StockPrice stockPrice)
        {
            InitializeComponent();
            _stockPrice = stockPrice;
            _isEditMode = true;
            _isAdd = false;
            _priceId = stockPrice.PriceId;

            this.Text = "编辑股票行情";
        }

        #endregion

        private void StockPriceEditForm_Load(object sender, EventArgs e)
        {
            // 设置窗体标题
            this.Text = _isAdd ? "添加股票行情" : "编辑股票行情";

            // 加载股票列表
            LoadStockList();

            // 如果是编辑模式，加载股票行情数据
            if (_isEditMode && _stockPrice != null)
            {
                // 设置股票
                cboStock.SelectedValue = _stockPrice.StockId;

                // 设置日期
                if (_stockPrice.TradeDate != DateTime.MinValue && DateTimeHelper.IsValidSqlDate(_stockPrice.TradeDate))
                {
                    dateTimePickerTradeDate.Value = _stockPrice.TradeDate;
                }
                else if (_stockPrice.Date != DateTime.MinValue && DateTimeHelper.IsValidSqlDate(_stockPrice.Date))
                {
                    dateTimePickerTradeDate.Value = _stockPrice.Date;
                }
                else
                {
                    dateTimePickerTradeDate.Value = DateTime.Now;
                }

                // 设置价格
                numOpenPrice.Value = _stockPrice.OpenPrice;
                numClosePrice.Value = _stockPrice.ClosePrice;
                numHighPrice.Value = _stockPrice.HighPrice;
                numLowPrice.Value = _stockPrice.LowPrice;
                numVolume.Value = _stockPrice.Volume;
                numAmount.Value = _stockPrice.Amount;
                numChangePercent.Value = _stockPrice.ChangePercent;
            }
            else if (_isEditMode && _priceId > 0)
            {
                // 需要从数据库加载数据
                LoadStockPriceData();
            }
            else
            {
                // 默认设置交易日期为当前日期
                dateTimePickerTradeDate.Value = DateTime.Now;

                // 如果有指定股票ID，则选中
                if (_stockPrice != null && _stockPrice.StockId > 0)
                {
                    cboStock.SelectedValue = _stockPrice.StockId;
                }
            }
        }

        /// <summary>
        /// 加载股票列表
        /// </summary>
        private void LoadStockList()
        {
            if (_stockService != null)
            {
                var stocks = _stockService.GetAllStocks();
                cboStock.DisplayMember = "Name";
                cboStock.ValueMember = "StockId";
                cboStock.DataSource = stocks;
            }
            else
            {
                // 由于无法确定StockContext的具体实现，使用StockService替代
                var stockService = new StockService();
                var stocks = stockService.GetAllStocks();
                cboStock.DisplayMember = "Name";
                cboStock.ValueMember = "Id";
                cboStock.DataSource = stocks;
            }
        }

        /// <summary>
        /// 加载股票行情数据
        /// </summary>
        private void LoadStockPriceData()
        {
            try
            {
                // 由于无法确定StockContext的具体实现，使用StockPriceService替代
                var stockPriceService = new StockPriceService();
                var stockPrices = stockPriceService.GetStockPricesByStockId(_priceId);
                if (stockPrices != null && stockPrices.Count > 0)
                {
                    _stockPrice = stockPrices.First();

                    // 设置股票
                    cboStock.SelectedValue = _stockPrice.StockId;

                    // 设置日期
                    if (_stockPrice.TradeDate != DateTime.MinValue && DateTimeHelper.IsValidSqlDate(_stockPrice.TradeDate))
                    {
                        dateTimePickerTradeDate.Value = _stockPrice.TradeDate;
                    }
                    else if (_stockPrice.Date != DateTime.MinValue && DateTimeHelper.IsValidSqlDate(_stockPrice.Date))
                    {
                        dateTimePickerTradeDate.Value = _stockPrice.Date;
                    }
                    else
                    {
                        dateTimePickerTradeDate.Value = DateTime.Now;
                    }

                    // 设置价格
                    numOpenPrice.Value = _stockPrice.OpenPrice;
                    numHighPrice.Value = _stockPrice.HighPrice;
                    numLowPrice.Value = _stockPrice.LowPrice;
                    numClosePrice.Value = _stockPrice.ClosePrice;
                    numVolume.Value = _stockPrice.Volume;
                    numAmount.Value = _stockPrice.Amount;
                    numChangePercent.Value = _stockPrice.ChangePercent;
                }
                else
                {
                    MessageBox.Show("未找到指定的行情记录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        /// <summary>
        /// 保存按钮点击事件
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (!ValidateInput())
                return;

            try
            {
                if (_stockService != null && _stockPriceService != null)
                {
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
                else
                {
                    // 由于无法确定StockContext的具体实现，使用StockPriceService替代
                    var stockPriceService = new StockPriceService();

                    // 构建StockPrice对象
                    if (_stockPrice == null)
                    {
                        _stockPrice = new StockPrice();
                    }

                    _stockPrice.StockId = (int)cboStock.SelectedValue;
                    _stockPrice.TradeDate = dateTimePickerTradeDate.Value;
                    _stockPrice.OpenPrice = numOpenPrice.Value;
                    _stockPrice.HighPrice = numHighPrice.Value;
                    _stockPrice.LowPrice = numLowPrice.Value;
                    _stockPrice.ClosePrice = numClosePrice.Value;
                    _stockPrice.Volume = (long)numVolume.Value;
                    _stockPrice.Amount = numAmount.Value;
                    _stockPrice.ChangePercent = numChangePercent.Value;

                    bool success;
                    if (_isAdd)
                    {
                        // 添加新行情
                        success = stockPriceService.AddStockPrice(_stockPrice);
                    }
                    else
                    {
                        // 更新现有行情
                        success = stockPriceService.UpdateStockPrice(_stockPrice);
                    }

                    if (success)
                    {
                        MessageBox.Show(_isAdd ? "添加成功！" : "更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(_isAdd ? "添加失败！" : "更新失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存数据时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #region 其他事件处理程序

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

        /// <summary>
        /// 验证输入
        /// </summary>
        private bool ValidateInput()
        {
            // 验证股票是否选择
            if (cboStock.SelectedIndex < 0)
            {
                MessageBox.Show("请选择股票", "验证错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboStock.Focus();
                return false;
            }

            // 所有验证都通过
            return true;
        }

        #endregion
    }
}