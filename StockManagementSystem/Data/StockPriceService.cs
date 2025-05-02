using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Helpers;
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
        /// 批量添加股票行情记录（使用事务提高性能）
        /// </summary>
        /// <param name="stockPrices">股票行情对象列表</param>
        /// <param name="progressCallback">进度回调函数，参数为(当前进度,总数量)</param>
        /// <returns>添加成功的记录数</returns>
        public int AddStockPricesBatch(List<StockPrice> stockPrices, Action<int, int> progressCallback = null)
        {
            if (stockPrices == null || stockPrices.Count == 0)
                return 0;

            int successCount = 0;
            int totalCount = stockPrices.Count;
            try
            {
                // 使用事务来批量插入数据
                using (SqlConnection connection = new SqlConnection(SqlHelper.GetConnectionString()))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;
                            command.CommandText = @"INSERT INTO StockPrices 
                                              (StockId, TradeDate, OpenPrice, ClosePrice, HighPrice, LowPrice, Volume, Amount, ChangePercent, CreateTime) 
                                              VALUES 
                                              (@StockId, @TradeDate, @OpenPrice, @ClosePrice, @HighPrice, @LowPrice, @Volume, @Amount, @ChangePercent, @CreateTime)";

                            // 创建参数
                            command.Parameters.Add("@StockId", SqlDbType.Int);
                            command.Parameters.Add("@TradeDate", SqlDbType.DateTime);
                            command.Parameters.Add("@OpenPrice", SqlDbType.Decimal);
                            command.Parameters.Add("@ClosePrice", SqlDbType.Decimal);
                            command.Parameters.Add("@HighPrice", SqlDbType.Decimal);
                            command.Parameters.Add("@LowPrice", SqlDbType.Decimal);
                            command.Parameters.Add("@Volume", SqlDbType.BigInt);
                            command.Parameters.Add("@Amount", SqlDbType.Decimal);
                            command.Parameters.Add("@ChangePercent", SqlDbType.Decimal);
                            command.Parameters.Add("@CreateTime", SqlDbType.DateTime);

                            // 批量执行
                            const int batchCommitSize = 1000; // 每1000条提交一次事务
                            int batchCount = 0;

                            for (int i = 0; i < stockPrices.Count; i++)
                            {
                                var price = stockPrices[i];
                                command.Parameters["@StockId"].Value = price.StockId;
                                command.Parameters["@TradeDate"].Value = price.TradeDate;
                                command.Parameters["@OpenPrice"].Value = price.OpenPrice;
                                command.Parameters["@ClosePrice"].Value = price.ClosePrice;
                                command.Parameters["@HighPrice"].Value = price.HighPrice;
                                command.Parameters["@LowPrice"].Value = price.LowPrice;
                                command.Parameters["@Volume"].Value = price.Volume;
                                command.Parameters["@Amount"].Value = price.Amount;
                                command.Parameters["@ChangePercent"].Value = price.ChangePercent;
                                command.Parameters["@CreateTime"].Value = DateTime.Now;

                                int result = command.ExecuteNonQuery();
                                if (result > 0)
                                {
                                    successCount++;
                                }

                                batchCount++;

                                // 每处理100条数据报告一次进度
                                if (i % 100 == 0 && progressCallback != null)
                                {
                                    progressCallback(i, totalCount);
                                }

                                // 如果达到批处理大小，执行一次中间提交
                                if (batchCount >= batchCommitSize)
                                {
                                    transaction.Commit();
                                    // 创建新事务继续处理
                                    transaction = connection.BeginTransaction();
                                    command.Transaction = transaction;
                                    batchCount = 0;
                                }
                            }

                            // 提交剩余的事务
                            transaction.Commit();

                            // 最后一次进度回调
                            progressCallback?.Invoke(totalCount, totalCount);
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                return successCount;
            }
            catch (Exception)
            {
                return successCount;
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
                // 确保日期在SQL Server支持的范围内
                startDate = DateTimeHelper.EnsureSqlDateRange(startDate);
                endDate = DateTimeHelper.EnsureSqlDateRange(endDate);

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
                // 确保日期在SQL Server支持的范围内
                startDate = DateTimeHelper.EnsureSqlDateRange(startDate);
                endDate = DateTimeHelper.EnsureSqlDateRange(endDate);

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
                throw new Exception("查询股票行情发生错误:" + ex.Message, ex);
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
        /// 获取最小价格数据，只返回StockId和TradeDate，用于快速检查重复
        /// </summary>
        /// <param name="stockIds">股票ID列表</param>
        /// <returns>元组列表(StockId, TradeDate)</returns>
        public List<Tuple<int, DateTime>> GetMinimalPriceData(List<int> stockIds)
        {
            try
            {
                if (stockIds == null || stockIds.Count == 0)
                    return new List<Tuple<int, DateTime>>();

                // 将ID列表转换为以逗号分隔的字符串
                string stockIdsStr = string.Join(",", stockIds);

                string sql = $"SELECT StockId, TradeDate FROM StockPrices WHERE StockId IN ({stockIdsStr})";

                DataTable dt = SqlHelper.ExecuteQuery(sql);
                List<Tuple<int, DateTime>> result = new List<Tuple<int, DateTime>>();

                foreach (DataRow row in dt.Rows)
                {
                    int stockId = Convert.ToInt32(row["StockId"]);
                    DateTime tradeDate = Convert.ToDateTime(row["TradeDate"]);
                    result.Add(new Tuple<int, DateTime>(stockId, tradeDate));
                }

                return result;
            }
            catch (Exception)
            {
                return new List<Tuple<int, DateTime>>();
            }
        }

        /// <summary>
        /// 将DataRow转换为StockPrice对象
        /// </summary>
        private StockPrice ConvertToStockPrice(DataRow row)
        {
            var stockPrice = new StockPrice
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

            // 如果结果集包含股票代码和名称，则创建一个简单的股票对象
            if (row.Table.Columns.Contains("Code") && row.Table.Columns.Contains("Name"))
            {
                stockPrice.Stock = new Stock
                {
                    StockId = stockPrice.StockId,
                    Code = row["Code"].ToString(),
                    Name = row["Name"].ToString()
                    // 其他股票属性将在需要时通过StockService加载
                };
            }

            return stockPrice;
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