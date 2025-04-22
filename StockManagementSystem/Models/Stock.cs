using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManagementSystem.Models
{
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
        /// 股票类型（例如：A股、B股等）
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