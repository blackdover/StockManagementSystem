using System;

namespace StockManagementSystem.Models
{
    /// <summary>
    /// 股票类型枚举
    /// </summary>
    public enum StockType
    {
        /// <summary>
        /// A股
        /// </summary>
        A股,

        /// <summary>
        /// B股
        /// </summary>
        B股,

        /// <summary>
        /// H股
        /// </summary>
        H股,

        /// <summary>
        /// N股
        /// </summary>
        N股,

        /// <summary>
        /// S股
        /// </summary>
        S股,

        /// <summary>
        /// 创业板
        /// </summary>
        创业板,

        /// <summary>
        /// 科创板
        /// </summary>
        科创板,

        /// <summary>
        /// 新三板
        /// </summary>
        新三板,

        /// <summary>
        /// 其他类型
        /// </summary>
        其他
    }

    /// <summary>
    /// 股票信息模型类
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public int StockId { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 股票类型（例如：A股、B股等）- 字符串表示，用于兼容
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 股票类型枚举值
        /// </summary>
        public StockType StockTypeEnum
        {
            get
            {
                // 如果Type为空，返回"其他"
                if (string.IsNullOrEmpty(Type))
                    return StockType.其他;

                // 尝试将字符串转换为枚举
                if (Enum.TryParse(Type, out StockType stockType))
                    return stockType;

                return StockType.其他;
            }
            set
            {
                // 当设置枚举值时，同时更新字符串属性
                Type = value.ToString();
            }
        }

        /// <summary>
        /// 所属行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 上市日期
        /// </summary>
        public DateTime ListingDate { get; set; }

        /// <summary>
        /// 股票描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}