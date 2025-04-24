using System;

namespace StockManagementSystem.Helpers
{
    /// <summary>
    /// 日期时间辅助类，提供SQL Server兼容的日期常量和验证方法
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// SQL Server支持的最小日期 (1753-01-01)
        /// </summary>
        public static readonly DateTime SqlMinDate = new DateTime(1753, 1, 1);

        /// <summary>
        /// SQL Server支持的最大日期 (9999-12-31)
        /// </summary>
        public static readonly DateTime SqlMaxDate = new DateTime(9999, 12, 31, 23, 59, 59);

        /// <summary>
        /// 将日期调整到SQL Server支持的范围内
        /// </summary>
        /// <param name="date">要检查的日期</param>
        /// <returns>调整后的日期</returns>
        public static DateTime EnsureSqlDateRange(DateTime date)
        {
            if (date < SqlMinDate)
                return SqlMinDate;
            if (date > SqlMaxDate)
                return SqlMaxDate;
            return date;
        }

        /// <summary>
        /// 检查日期是否在SQL Server支持的范围内
        /// </summary>
        /// <param name="date">要检查的日期</param>
        /// <returns>如果日期在有效范围内，返回true；否则返回false</returns>
        public static bool IsValidSqlDate(DateTime date)
        {
            return date >= SqlMinDate && date <= SqlMaxDate;
        }

        /// <summary>
        /// 获取当前日期或默认日期，确保在SQL Server支持的范围内
        /// </summary>
        /// <returns>有效的日期</returns>
        public static DateTime GetSafeDate()
        {
            return DateTime.Now;
        }
    }
}