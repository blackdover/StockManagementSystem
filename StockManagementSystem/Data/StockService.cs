using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using StockManagementSystem.Data;
using StockManagementSystem.Models;

namespace StockManagementSystem.Services
{
    /// <summary>
    /// 股票信息服务类
    /// </summary>
    public class StockService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StockService()
        {
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
                string sql = @"INSERT INTO Stocks (Code, Name, Type, Industry, ListingDate, Description, CreateTime, UpdateTime)
                              VALUES (@Code, @Name, @Type, @Industry, @ListingDate, @Description, @CreateTime, @UpdateTime)";

                SqlParameter[] parameters = {
                    new SqlParameter("@Code", stock.Code),
                    new SqlParameter("@Name", stock.Name),
                    new SqlParameter("@Type", stock.Type ?? (object)DBNull.Value),
                    new SqlParameter("@Industry", stock.Industry ?? (object)DBNull.Value),
                    new SqlParameter("@ListingDate", stock.ListingDate),
                    new SqlParameter("@Description", stock.Description ?? (object)DBNull.Value),
                    new SqlParameter("@CreateTime", DateTime.Now),
                    new SqlParameter("@UpdateTime", DateTime.Now)
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
        /// 更新股票信息
        /// </summary>
        /// <param name="stock">股票信息对象</param>
        /// <returns>更新成功返回true，否则返回false</returns>
        public bool UpdateStock(Stock stock)
        {
            try
            {
                string sql = @"UPDATE Stocks SET 
                               Code = @Code, 
                               Name = @Name, 
                               Type = @Type, 
                               Industry = @Industry, 
                               ListingDate = @ListingDate, 
                               Description = @Description, 
                               UpdateTime = @UpdateTime
                               WHERE StockId = @StockId";

                SqlParameter[] parameters = {
                    new SqlParameter("@StockId", stock.StockId),
                    new SqlParameter("@Code", stock.Code),
                    new SqlParameter("@Name", stock.Name),
                    new SqlParameter("@Type", stock.Type ?? (object)DBNull.Value),
                    new SqlParameter("@Industry", stock.Industry ?? (object)DBNull.Value),
                    new SqlParameter("@ListingDate", stock.ListingDate),
                    new SqlParameter("@Description", stock.Description ?? (object)DBNull.Value),
                    new SqlParameter("@UpdateTime", DateTime.Now)
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
        /// 删除股票信息
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public bool DeleteStock(int stockId)
        {
            try
            {
                // 首先删除关联的股票价格记录
                string deletePricesSql = "DELETE FROM StockPrices WHERE StockId = @StockId";
                SqlParameter priceParameter = new SqlParameter("@StockId", stockId);
                SqlHelper.ExecuteNonQuery(deletePricesSql, priceParameter);

                // 然后删除股票记录
                string deleteStockSql = "DELETE FROM Stocks WHERE StockId = @StockId";
                SqlParameter stockParameter = new SqlParameter("@StockId", stockId);

                int result = SqlHelper.ExecuteNonQuery(deleteStockSql, stockParameter);
                return result > 0;
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
            try
            {
                string sql = "SELECT * FROM Stocks";
                DataTable dt = SqlHelper.ExecuteQuery(sql);

                return ConvertToStockList(dt);
            }
            catch (Exception)
            {
                return new List<Stock>();
            }
        }

        /// <summary>
        /// 根据ID获取股票信息
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <returns>股票信息对象</returns>
        public Stock GetStockById(int stockId)
        {
            try
            {
                string sql = "SELECT * FROM Stocks WHERE StockId = @StockId";
                SqlParameter parameter = new SqlParameter("@StockId", stockId);

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameter);
                if (dt.Rows.Count > 0)
                {
                    return ConvertToStock(dt.Rows[0]);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据股票代码获取股票信息
        /// </summary>
        /// <param name="code">股票代码</param>
        /// <returns>股票信息对象</returns>
        public Stock GetStockByCode(string code)
        {
            try
            {
                string sql = "SELECT * FROM Stocks WHERE Code = @Code";
                SqlParameter parameter = new SqlParameter("@Code", code);

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameter);
                if (dt.Rows.Count > 0)
                {
                    return ConvertToStock(dt.Rows[0]);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据股票名称查询股票列表
        /// </summary>
        /// <param name="name">股票名称（支持模糊查询）</param>
        /// <returns>股票信息列表</returns>
        public List<Stock> GetStocksByName(string name)
        {
            try
            {
                string sql = "SELECT * FROM Stocks WHERE Name LIKE @Name";
                SqlParameter parameter = new SqlParameter("@Name", "%" + name + "%");

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameter);
                return ConvertToStockList(dt);
            }
            catch (Exception)
            {
                return new List<Stock>();
            }
        }

        /// <summary>
        /// 根据行业查询股票列表
        /// </summary>
        /// <param name="industry">行业名称</param>
        /// <returns>股票信息列表</returns>
        public List<Stock> GetStocksByIndustry(string industry)
        {
            try
            {
                string sql = "SELECT * FROM Stocks WHERE Industry = @Industry";
                SqlParameter parameter = new SqlParameter("@Industry", industry);

                DataTable dt = SqlHelper.ExecuteQuery(sql, parameter);
                return ConvertToStockList(dt);
            }
            catch (Exception)
            {
                return new List<Stock>();
            }
        }

        /// <summary>
        /// 根据条件筛选股票
        /// </summary>
        /// <param name="stockType">股票类型</param>
        /// <param name="industry">行业</param>
        /// <param name="stockCode">股票代码</param>
        /// <param name="stockName">股票名称</param>
        /// <returns>符合条件的股票视图模型列表</returns>
        public IEnumerable<StockViewModel> GetFilteredStocks(string stockType, string industry, string stockCode, string stockName)
        {
            try
            {
                StringBuilder sqlBuilder = new StringBuilder("SELECT * FROM Stocks WHERE 1=1");
                List<SqlParameter> parameters = new List<SqlParameter>();

                // 添加筛选条件
                if (!string.IsNullOrEmpty(stockType))
                {
                    sqlBuilder.Append(" AND Type = @Type");
                    parameters.Add(new SqlParameter("@Type", stockType));
                }

                if (!string.IsNullOrEmpty(industry))
                {
                    sqlBuilder.Append(" AND Industry = @Industry");
                    parameters.Add(new SqlParameter("@Industry", industry));
                }

                if (!string.IsNullOrEmpty(stockCode))
                {
                    sqlBuilder.Append(" AND Code LIKE @Code");
                    parameters.Add(new SqlParameter("@Code", "%" + stockCode + "%"));
                }

                if (!string.IsNullOrEmpty(stockName))
                {
                    sqlBuilder.Append(" AND Name LIKE @Name");
                    parameters.Add(new SqlParameter("@Name", "%" + stockName + "%"));
                }

                DataTable dt = SqlHelper.ExecuteQuery(sqlBuilder.ToString(), parameters.ToArray());
                List<Stock> stocks = ConvertToStockList(dt);

                // 转换为视图模型
                return stocks.Select(s => StockViewModel.FromStock(s));
            }
            catch (Exception ex)
            {
                // 记录异常
                Console.WriteLine($"筛选股票时发生错误: {ex.Message}");
                return new List<StockViewModel>();
            }
        }

        /// <summary>
        /// 将DataRow转换为Stock对象
        /// </summary>
        private Stock ConvertToStock(DataRow row)
        {
            var stock = new Stock
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

            // StockTypeEnum属性会自动根据Type属性值设置

            return stock;
        }

        /// <summary>
        /// 将DataTable转换为Stock列表
        /// </summary>
        private List<Stock> ConvertToStockList(DataTable dt)
        {
            List<Stock> stocks = new List<Stock>();
            foreach (DataRow row in dt.Rows)
            {
                stocks.Add(ConvertToStock(row));
            }
            return stocks;
        }

        /// <summary>
        /// 批量添加股票
        /// </summary>
        /// <param name="stocks">股票列表</param>
        /// <param name="progressCallback">进度回调函数，参数为(当前进度,总数量)</param>
        /// <returns>成功添加的记录数</returns>
        public int AddStocksBatch(List<Stock> stocks, Action<int, int> progressCallback = null)
        {
            if (stocks == null || stocks.Count == 0)
                return 0;

            int successCount = 0;
            int totalCount = stocks.Count;
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
                            command.CommandText = @"INSERT INTO Stocks 
                                              (Code, Name, Type, Industry, ListingDate, Description, CreateTime, UpdateTime)
                                              VALUES 
                                              (@Code, @Name, @Type, @Industry, @ListingDate, @Description, @CreateTime, @UpdateTime)";

                            // 创建参数
                            command.Parameters.Add("@Code", SqlDbType.NVarChar, 10);
                            command.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
                            command.Parameters.Add("@Type", SqlDbType.NVarChar, 20);
                            command.Parameters.Add("@Industry", SqlDbType.NVarChar, 50);
                            command.Parameters.Add("@ListingDate", SqlDbType.DateTime);
                            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1);
                            command.Parameters.Add("@CreateTime", SqlDbType.DateTime);
                            command.Parameters.Add("@UpdateTime", SqlDbType.DateTime);

                            // 批量执行
                            const int batchCommitSize = 1000; // 每1000条提交一次事务
                            int batchCount = 0;

                            for (int i = 0; i < stocks.Count; i++)
                            {
                                var stock = stocks[i];
                                command.Parameters["@Code"].Value = stock.Code;
                                command.Parameters["@Name"].Value = stock.Name;
                                command.Parameters["@Type"].Value = stock.Type ?? (object)DBNull.Value;
                                command.Parameters["@Industry"].Value = stock.Industry ?? (object)DBNull.Value;
                                command.Parameters["@ListingDate"].Value = stock.ListingDate;
                                command.Parameters["@Description"].Value = stock.Description ?? (object)DBNull.Value;
                                command.Parameters["@CreateTime"].Value = DateTime.Now;
                                command.Parameters["@UpdateTime"].Value = DateTime.Now;

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

                            // 最终提交
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
        /// 获取所有不重复的行业信息
        /// </summary>
        /// <returns>所有行业的列表</returns>
        public List<string> GetAllIndustries()
        {
            try
            {
                string sql = "SELECT DISTINCT Industry FROM Stocks WHERE Industry IS NOT NULL AND Industry <> '' ORDER BY Industry";
                DataTable dt = SqlHelper.ExecuteDataTable(sql);

                List<string> industries = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    industries.Add(row["Industry"].ToString());
                }

                return industries;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("获取行业列表失败: " + ex.Message);
                return new List<string>();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 无需释放资源，因为SqlHelper是静态类
        }
    }
}