using System;

namespace StockManagementSystem.Models
{
    // 股票类型枚举
    public enum StockType
    {
        A股,
        B股,
        H股,
        N股,
        S股,
        创业板,
        科创板,
        新三板,
        其他
    }

    // 股票信息模型类
    public class Stock
    {
        // 股票ID
        public int StockId { get; set; }

        // 股票代码
        public string Code { get; set; }

        // 股票名称
        public string Name { get; set; }

        // 股票类型
        public string Type { get; set; }

        // 股票类型枚举值
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

        // 所属行业
        public string Industry { get; set; }

        // 上市日期
        public DateTime ListingDate { get; set; }

        // 股票描述
        public string Description { get; set; }

        // 创建时间
        public DateTime CreateTime { get; set; }

        // 最后更新时间
        public DateTime UpdateTime { get; set; }
    }
}