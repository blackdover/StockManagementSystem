using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private readonly StockDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        public StockPriceService()
        {
            _dbContext = new StockDbContext();
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
                stockPrice.CreateTime = DateTime.Now;
                _dbContext.StockPrices.Add(stockPrice);
                return _dbContext.SaveChanges() > 0;
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
                foreach (var price in stockPrices)
                {
                    price.CreateTime = DateTime.Now;
                    _dbContext.StockPrices.Add(price);
                }
                return _dbContext.SaveChanges() > 0;
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
                var existingPrice = _dbContext.StockPrices.Find(stockPrice.PriceId);
                if (existingPrice == null)
                    return false;

                // 更新属性
                existingPrice.OpenPrice = stockPrice.OpenPrice;
                existingPrice.ClosePrice = stockPrice.ClosePrice;
                existingPrice.HighPrice = stockPrice.HighPrice;
                existingPrice.LowPrice = stockPrice.LowPrice;
                existingPrice.Volume = stockPrice.Volume;
                existingPrice.Amount = stockPrice.Amount;
                existingPrice.ChangePercent = stockPrice.ChangePercent;

                return _dbContext.SaveChanges() > 0;
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
                var price = _dbContext.StockPrices.Find(priceId);
                if (price == null)
                    return false;

                _dbContext.StockPrices.Remove(price);
                return _dbContext.SaveChanges() > 0;
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
            return _dbContext.StockPrices
                .Where(p => p.StockId == stockId)
                .OrderByDescending(p => p.TradeDate)
                .ToList();
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
            return _dbContext.StockPrices
                .Where(p => p.StockId == stockId && p.TradeDate >= startDate && p.TradeDate <= endDate)
                .OrderBy(p => p.TradeDate)
                .ToList();
        }

        /// <summary>
        /// 获取指定日期的所有股票行情
        /// </summary>
        /// <param name="date">交易日期</param>
        /// <returns>行情记录列表</returns>
        public List<StockPrice> GetStockPricesByDate(DateTime date)
        {
            return _dbContext.StockPrices
                .Where(p => DbFunctions.TruncateTime(p.TradeDate) == DbFunctions.TruncateTime(date))
                .Include(p => p.Stock)
                .OrderBy(p => p.Stock.Code)
                .ToList();
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
            decimal avgVolume = (decimal)prices.Average(p => p.Volume);

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
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }

    /// <summary>
    /// 股票价格波动统计信息类
    /// </summary>
    public class PriceFluctuationStatistics
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public int StockId { get; set; }

        /// <summary>
        /// 起始日期
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
        /// 价格波动范围百分比
        /// </summary>
        public decimal PriceRangePercent { get; set; }

        /// <summary>
        /// 总体涨跌幅
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