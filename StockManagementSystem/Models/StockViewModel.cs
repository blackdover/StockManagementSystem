using System;

namespace StockManagementSystem.Models
{
    /// <summary>
    /// 股票信息视图模型，用于在UI中显示股票信息
    /// </summary>
    public class StockViewModel
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 股票类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 上市日期
        /// </summary>
        public DateTime ListingDate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 从Stock对象创建StockViewModel对象
        /// </summary>
        /// <param name="stock">Stock对象</param>
        /// <returns>StockViewModel对象</returns>
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