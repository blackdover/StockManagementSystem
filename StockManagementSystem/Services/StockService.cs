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
    /// 股票信息服务类
    /// </summary>
    public class StockService
    {
        private readonly StockDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        public StockService()
        {
            _dbContext = new StockDbContext();
        }

        /// <summary>
        /// 添加股票信息
        /// </summary>
        /// <param name="stock">股票信息对象</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public bool AddStock(Stock stock)
        {
            try
            {
                stock.CreateTime = DateTime.Now;
                stock.UpdateTime = DateTime.Now;
                _dbContext.Stocks.Add(stock);
                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新股票信息
        /// </summary>
        /// <param name="stock">股票信息对象</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        public bool UpdateStock(Stock stock)
        {
            try
            {
                var existingStock = _dbContext.Stocks.Find(stock.StockId);
                if (existingStock == null)
                    return false;

                // 更新属性
                existingStock.Code = stock.Code;
                existingStock.Name = stock.Name;
                existingStock.Type = stock.Type;
                existingStock.Industry = stock.Industry;
                existingStock.ListingDate = stock.ListingDate;
                existingStock.Description = stock.Description;
                existingStock.UpdateTime = DateTime.Now;

                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除股票信息
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool DeleteStock(int stockId)
        {
            try
            {
                var stock = _dbContext.Stocks.Find(stockId);
                if (stock == null)
                    return false;

                _dbContext.Stocks.Remove(stock);
                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有股票信息
        /// </summary>
        /// <returns>股票信息列表</returns>
        public List<Stock> GetAllStocks()
        {
            return _dbContext.Stocks.ToList();
        }

        /// <summary>
        /// 根据ID获取股票信息
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <returns>股票信息对象</returns>
        public Stock GetStockById(int stockId)
        {
            return _dbContext.Stocks.Find(stockId);
        }

        /// <summary>
        /// 根据股票代码获取股票信息
        /// </summary>
        /// <param name="code">股票代码</param>
        /// <returns>股票信息对象</returns>
        public Stock GetStockByCode(string code)
        {
            return _dbContext.Stocks.FirstOrDefault(s => s.Code == code);
        }

        /// <summary>
        /// 根据股票名称查询股票列表
        /// </summary>
        /// <param name="name">股票名称（支持模糊查询）</param>
        /// <returns>股票信息列表</returns>
        public List<Stock> GetStocksByName(string name)
        {
            return _dbContext.Stocks.Where(s => s.Name.Contains(name)).ToList();
        }

        /// <summary>
        /// 根据行业查询股票列表
        /// </summary>
        /// <param name="industry">行业名称</param>
        /// <returns>股票信息列表</returns>
        public List<Stock> GetStocksByIndustry(string industry)
        {
            return _dbContext.Stocks.Where(s => s.Industry == industry).ToList();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
} 