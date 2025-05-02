using System;

namespace StockManagementSystem.Models
{
    // 股票行情记录模型类
    public class StockPrice
    {
        // 行情记录ID
        public int PriceId { get; set; }

        // 对应股票ID
        public int StockId { get; set; }

        // 交易日期
        public DateTime TradeDate { get; set; }

        // 开盘价
        public decimal OpenPrice { get; set; }

        // 收盘价
        public decimal ClosePrice { get; set; }

        // 最高价
        public decimal HighPrice { get; set; }

        // 最低价
        public decimal LowPrice { get; set; }

        // 成交量（手）
        public long Volume { get; set; }

        // 成交额（元）
        public decimal Amount { get; set; }

        // 涨跌幅（百分比）
        public decimal ChangePercent { get; set; }

        // 创建时间
        public DateTime CreateTime { get; set; }

        // 关联的股票对象（导航属性）
        public virtual Stock Stock { get; set; }

        // 前一交易日收盘价（用于计算涨跌幅）
        public decimal PrevClosePrice { get; set; }

        #region 兼容性属性

        // 兼容性属性，映射到PriceId
        public int Id
        {
            get { return PriceId; }
            set { PriceId = value; }
        }

        // 兼容性属性，映射到TradeDate
        public DateTime Date
        {
            get { return TradeDate; }
            set { TradeDate = value; }
        }

        // 兼容性属性，映射到HighPrice
        public decimal HighestPrice
        {
            get { return HighPrice; }
            set { HighPrice = value; }
        }

        // 兼容性属性，映射到LowPrice
        public decimal LowestPrice
        {
            get { return LowPrice; }
            set { LowPrice = value; }
        }

        #endregion
    }
}