using System;

namespace StockManagementSystem.Services.Helpers
{
    // 日期时间辅助类，提供SQL Server兼容的日期常量和验证方法
    public static class DateTimeHelper
    {
        // SQL Server支持的最小日期 (1753-01-01)
        public static readonly DateTime SqlMinDate = new DateTime(1753, 1, 1);

        // SQL Server支持的最大日期 (9999-12-31)
        public static readonly DateTime SqlMaxDate = new DateTime(9999, 12, 31, 23, 59, 59);

        // 将日期调整到SQL Server支持的范围内
        public static DateTime EnsureSqlDateRange(DateTime date)
        {
            if (date < SqlMinDate)
                return SqlMinDate;
            if (date > SqlMaxDate)
                return SqlMaxDate;
            return date;
        }

        // 检查日期是否在SQL Server支持的范围内
        public static bool IsValidSqlDate(DateTime date)
        {
            return date >= SqlMinDate && date <= SqlMaxDate;
        }

        // 获取当前日期或默认日期，确保在SQL Server支持的范围内
        public static DateTime GetSafeDate()
        {
            return DateTime.Now;
        }
    }
}