using System;

namespace StockManagementSystem.Models
{
    // 股票信息视图模型，用于在UI中显示股票信息
    public class StockViewModel
    {
        // 股票ID
        public int Id { get; set; }

        // 股票代码
        public string Code { get; set; }

        // 股票名称
        public string Name { get; set; }

        // 股票类型
        public string Type { get; set; }

        // 所属行业
        public string Industry { get; set; }

        // 上市日期
        public DateTime ListingDate { get; set; }

        // 描述
        public string Description { get; set; }

        // 从Stock对象创建StockViewModel对象
        public static StockViewModel FromStock(Stock stock)
        {
            if (stock == null) return null;

            return new StockViewModel
            {
                Id = stock.StockId,
                Code = stock.Code,
                Name = stock.Name,
                Type = stock.Type ?? "未知",
                Industry = stock.Industry ?? "未知",
                ListingDate = stock.ListingDate,
                Description = stock.Description
            };
        }
    }
}