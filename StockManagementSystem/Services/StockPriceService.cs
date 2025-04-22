using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockManagementSystem.Data;
using StockManagementSystem.Models;

namespace StockManagementSystem.Services
{
    /// <summary>
    /// 股票行情服务类
    /// </summary>
    public class StockPriceService
    {
        private readonly StockService _stockService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public StockPriceService()
        {
            _stockService = new StockService();
        }

        /// <summary>
        /// 添加股票行情记录
        /// </summary>
        /// <param name="stockPrice">股票行情对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public bool AddStockPrice(StockPrice stockPrice)
        {
            try
            {
                string sql = @"INSERT INTO StockPrices 
                              (StockId, TradeDate, OpenPrice, ClosePrice, HighPrice, LowPrice, Volume, Amount, ChangePercent, CreateTime) 
                              VALUES 
                              (@StockId, @TradeDate, @OpenPrice, @ClosePrice, @HighPrice, @LowPrice, @Volume, @Amount, @ChangePercent, @CreateTime)";

                SqlParameter[] parameters = {
                    new SqlParameter("@StockId", stockPrice.StockId),
                    new SqlParameter("@TradeDate", stockPrice.TradeDate),
                    new SqlParameter("@OpenPrice", stockPrice.OpenPrice),
                    new SqlParameter("@ClosePrice", stockPrice.ClosePrice),
                    new SqlParameter("@HighPrice", stockPrice.HighPrice),
                    new SqlParameter("@LowPrice", stockPrice.LowPrice),
                    new SqlParameter("@Volume", stockPrice.Volume),
                    new SqlParameter("@Amount", stockPrice.Amount),
                    new SqlParameter("@ChangePercent", stockPrice.ChangePercent),
                    new SqlParameter("@CreateTime", DateTime.Now)
                };

                int result = SqlHelper.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 批量添加股票行情记录
        /// </summary>
        /// <param name="stockPrices">股票行情对象列表</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public bool AddStockPrices(List<StockPrice> stockPrices)
        {
            try
            {
                int successCount = 0;
                foreach (var price in stockPrices)
                {
                    if (AddStockPrice(price))
                    {
                        successCount++;
                    }
                }
                return successCount > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新股票行情记录
        /// </summary>
        /// <param name="stockPrice">股票行情对象</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        public bool UpdateStockPrice(StockPrice stockPrice)
        {
            try
            {
                string sql = @"UPDATE StockPrices SET 
                               OpenPrice = @OpenPrice, 
                               ClosePrice = @ClosePrice, 
                               HighPrice = @HighPrice, 
                               LowPrice = @LowPrice, 
                               Volume = @Volume, 
                               Amount = @Amount, 
                               ChangePercent = @ChangePercent
                               WHERE PriceId = @PriceId";

                SqlParameter[] parameters = {
                    new SqlParameter("@PriceId", stockPrice.PriceId),
                    new SqlParameter("@OpenPrice", stockPrice.OpenPrice),
                    new SqlParameter("@ClosePrice", stockPrice.ClosePrice),
                    new SqlParameter("@HighPrice", stockPrice.HighPrice),
                    new SqlParameter("@LowPrice", stockPrice.LowPrice),
                    new SqlParameter("@Volume", stockPrice.Volume),
                    new SqlParameter("@Amount", stockPrice.Amount),
                    new SqlParameter("@ChangePercent", stockPrice.ChangePercent)
                };

                int result = SqlHelper.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除股票行情记录
        /// </summary>
        /// <param name="priceId">行情记录ID</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool DeleteStockPrice(int priceId)
        {
            try
            {
                string sql = "DELETE FROM StockPrices WHERE PriceId = @PriceId";
                SqlParameter parameter = new SqlParameter("@PriceId", priceId);

                int result = SqlHelper.ExecuteNonQuery(sql, parameter);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据股票ID获取所有行情记录
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <returns>行情记录列表</returns>
        public List<StockPrice> GetStockPricesByStockId(int stockId)
        {
            try
            {
                string sql = "SELECT * FROM StockPrices WHERE StockId = @StockId ORDER BY TradeDate DESC";
                SqlParameter parameter = new SqlParameter("@StockId", stockId);

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameter);
                List<StockPrice> prices = ConvertToStockPriceList(dt);

                // 加载关联的Stock对象
                foreach (var price in prices)
                {
                    price.Stock = _stockService.GetStockById(stockId);
                }

                return prices;
            }
            catch (Exception)
            {
                return new List<StockPrice>();
            }
        }

        /// <summary>
        /// 根据股票ID和日期范围获取行情记录
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>行情记录列表</returns>
        public List<StockPrice> GetStockPricesByDateRange(int stockId, DateTime startDate, DateTime endDate)
        {
            try
            {
                string sql = "SELECT * FROM StockPrices WHERE StockId = @StockId AND TradeDate >= @StartDate AND TradeDate <= @EndDate ORDER BY TradeDate";

                SqlParameter[] parameters = {
                    new SqlParameter("@StockId", stockId),
                    new SqlParameter("@StartDate", startDate.Date),
                    new SqlParameter("@EndDate", endDate.Date)
                };

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameters);
                List<StockPrice> prices = ConvertToStockPriceList(dt);

                // 加载关联的Stock对象
                foreach (var price in prices)
                {
                    price.Stock = _stockService.GetStockById(stockId);
                }

                return prices;
            }
            catch (Exception)
            {
                return new List<StockPrice>();
            }
        }

        /// <summary>
        /// 查询股票行情记录
        /// </summary>
        /// <param name="stockId">股票ID，传入null表示查询所有股票</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>行情记录列表</returns>
        public List<StockPrice> GetStockPrices(int? stockId, DateTime startDate, DateTime endDate)
        {
            try
            {
                string sql;
                SqlParameter[] parameters;

                if (stockId.HasValue && stockId.Value > 0)
                {
                    // 查询特定股票
                    sql = "SELECT p.*, s.Code, s.Name FROM StockPrices p " +
                          "JOIN Stocks s ON p.StockId = s.StockId " +
                          "WHERE p.StockId = @StockId AND p.TradeDate >= @StartDate AND p.TradeDate <= @EndDate " +
                          "ORDER BY p.TradeDate DESC";

                    parameters = new SqlParameter[] {
                        new SqlParameter("@StockId", stockId.Value),
                        new SqlParameter("@StartDate", startDate.Date),
                        new SqlParameter("@EndDate", endDate.Date)
                    };
                }
                else
                {
                    // 查询所有股票
                    sql = "SELECT p.*, s.Code, s.Name FROM StockPrices p " +
                          "JOIN Stocks s ON p.StockId = s.StockId " +
                          "WHERE p.TradeDate >= @StartDate AND p.TradeDate <= @EndDate " +
                          "ORDER BY p.TradeDate DESC, s.Code";

                    parameters = new SqlParameter[] {
                        new SqlParameter("@StartDate", startDate.Date),
                        new SqlParameter("@EndDate", endDate.Date)
                    };
                }

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameters);
                List<StockPrice> prices = new List<StockPrice>();

                foreach (DataRow row in dt.Rows)
                {
                    StockPrice price = ConvertToStockPrice(row);
                    price.Stock = new Stock
                    {
                        StockId = Convert.ToInt32(row["StockId"]),
                        Code = row["Code"].ToString(),
                        Name = row["Name"].ToString()
                    };
                    prices.Add(price);
                }

                return prices;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"查询股票行情发生错误: {ex.Message}", "错误",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return new List<StockPrice>();
            }
        }

        /// <summary>
        /// 获取指定日期的所有股票行情
        /// </summary>
        /// <param name="date">交易日期</param>
        /// <returns>行情记录列表</returns>
        public List<StockPrice> GetStockPricesByDate(DateTime date)
        {
            try
            {
                string sql = @"SELECT sp.*, s.Code, s.Name, s.Type, s.Industry, s.ListingDate, s.Description, s.CreateTime, s.UpdateTime 
                              FROM StockPrices sp
                              INNER JOIN Stocks s ON sp.StockId = s.StockId
                              WHERE CONVERT(date, sp.TradeDate) = @TradeDate
                              ORDER BY s.Code";

                SqlParameter parameter = new SqlParameter("@TradeDate", date.Date);

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameter);
                List<StockPrice> prices = new List<StockPrice>();

                foreach (DataRow row in dt.Rows)
                {
                    StockPrice price = ConvertToStockPrice(row);
                    price.Stock = new Stock
                    {
                        StockId = Convert.ToInt32(row["StockId"]),
                        Code = row["Code"].ToString(),
                        Name = row["Name"].ToString(),
                        Type = row["Type"] == DBNull.Value ? null : row["Type"].ToString(),
                        Industry = row["Industry"] == DBNull.Value ? null : row["Industry"].ToString(),
                        ListingDate = Convert.ToDateTime(row["ListingDate"]),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        CreateTime = Convert.ToDateTime(row["CreateTime"]),
                        UpdateTime = Convert.ToDateTime(row["UpdateTime"])
                    };
                    prices.Add(price);
                }

                return prices;
            }
            catch (Exception)
            {
                return new List<StockPrice>();
            }
        }

        /// <summary>
        /// 获取股票价格波动统计
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>股票价格波动统计信息</returns>
        public PriceFluctuationStatistics GetPriceFluctuationStatistics(int stockId, DateTime startDate, DateTime endDate)
        {
            var prices = GetStockPricesByDateRange(stockId, startDate, endDate);
            if (prices == null || prices.Count == 0)
                return null;

            decimal maxPrice = prices.Max(p => p.HighPrice);
            decimal minPrice = prices.Min(p => p.LowPrice);
            decimal avgPrice = prices.Average(p => p.ClosePrice);
            decimal priceRange = maxPrice - minPrice;
            decimal priceRangePercent = minPrice == 0 ? 0 : priceRange / minPrice * 100;

            decimal firstPrice = prices.First().OpenPrice;
            decimal lastPrice = prices.Last().ClosePrice;
            decimal overallChangePercent = firstPrice == 0 ? 0 : (lastPrice - firstPrice) / firstPrice * 100;

            long totalVolume = prices.Sum(p => p.Volume);
            decimal totalAmount = prices.Sum(p => p.Amount);
            decimal avgVolume = prices.Count > 0 ? (decimal)prices.Average(p => (double)p.Volume) : 0;

            return new PriceFluctuationStatistics
            {
                StockId = stockId,
                StartDate = startDate,
                EndDate = endDate,
                MaxPrice = maxPrice,
                MinPrice = minPrice,
                AveragePrice = avgPrice,
                PriceRange = priceRange,
                PriceRangePercent = priceRangePercent,
                OverallChangePercent = overallChangePercent,
                TotalVolume = totalVolume,
                TotalAmount = totalAmount,
                AverageVolume = avgVolume
            };
        }

        /// <summary>
        /// 将DataRow转换为StockPrice对象
        /// </summary>
        private StockPrice ConvertToStockPrice(DataRow row)
        {
            return new StockPrice
            {
                PriceId = Convert.ToInt32(row["PriceId"]),
                StockId = Convert.ToInt32(row["StockId"]),
                TradeDate = Convert.ToDateTime(row["TradeDate"]),
                OpenPrice = Convert.ToDecimal(row["OpenPrice"]),
                ClosePrice = Convert.ToDecimal(row["ClosePrice"]),
                HighPrice = Convert.ToDecimal(row["HighPrice"]),
                LowPrice = Convert.ToDecimal(row["LowPrice"]),
                Volume = Convert.ToInt64(row["Volume"]),
                Amount = Convert.ToDecimal(row["Amount"]),
                ChangePercent = Convert.ToDecimal(row["ChangePercent"]),
                CreateTime = Convert.ToDateTime(row["CreateTime"])
            };
        }

        /// <summary>
        /// 将DataTable转换为StockPrice列表
        /// </summary>
        private List<StockPrice> ConvertToStockPriceList(DataTable dt)
        {
            List<StockPrice> prices = new List<StockPrice>();
            foreach (DataRow row in dt.Rows)
            {
                prices.Add(ConvertToStockPrice(row));
            }
            return prices;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 无需释放资源
        }
    }

    /// <summary>
    /// 价格波动统计类
    /// </summary>
    public class PriceFluctuationStatistics
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public int StockId { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 平均价
        /// </summary>
        public decimal AveragePrice { get; set; }

        /// <summary>
        /// 价格波动范围
        /// </summary>
        public decimal PriceRange { get; set; }

        /// <summary>
        /// 价格波动百分比
        /// </summary>
        public decimal PriceRangePercent { get; set; }

        /// <summary>
        /// 总体涨跌百分比
        /// </summary>
        public decimal OverallChangePercent { get; set; }

        /// <summary>
        /// 总成交量
        /// </summary>
        public long TotalVolume { get; set; }

        /// <summary>
        /// 总成交额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 平均成交量
        /// </summary>
        public decimal AverageVolume { get; set; }
    }
}