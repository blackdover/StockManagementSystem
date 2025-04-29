using System;

namespace StockManagementSystem.Models
{
    /// <summary>
    /// 股票行情记录模型类
    /// </summary>
    public class StockPrice
    {
        /// <summary>
        /// 行情记录ID
        /// </summary>
        public int PriceId { get; set; }

        /// <summary>
        /// 对应股票ID
        /// </summary>
        public int StockId { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public decimal HighPrice { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        public decimal LowPrice { get; set; }

        /// <summary>
        /// 成交量（手）
        /// </summary>
        public long Volume { get; set; }

        /// <summary>
        /// 成交额（元）
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 涨跌幅（百分比）
        /// </summary>
        public decimal ChangePercent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 关联的股票对象（导航属性）
        /// </summary>
        public virtual Stock Stock { get; set; }

        /// <summary>
        /// 前一交易日收盘价（用于计算涨跌幅）
        /// </summary>
        public decimal PrevClosePrice { get; set; }

        #region 兼容性属性

        /// <summary>
        /// 兼容性属性，映射到PriceId
        /// </summary>
        public int Id
        {
            get { return PriceId; }
            set { PriceId = value; }
        }

        /// <summary>
        /// 兼容性属性，映射到TradeDate
        /// </summary>
        public DateTime Date
        {
            get { return TradeDate; }
            set { TradeDate = value; }
        }

        /// <summary>
        /// 兼容性属性，映射到HighPrice
        /// </summary>
        public decimal HighestPrice
        {
            get { return HighPrice; }
            set { HighPrice = value; }
        }

        /// <summary>
        /// 兼容性属性，映射到LowPrice
        /// </summary>
        public decimal LowestPrice
        {
            get { return LowPrice; }
            set { LowPrice = value; }
        }

        #endregion
    }
}