using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockManagementSystem.Models;

namespace StockManagementSystem.Data
{
    /// <summary>
    /// 股票管理系统数据库上下文
    /// </summary>
    public class StockDbContext : DbContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StockDbContext() : base("name=StockDbConnection")
        {
            // 初始化数据库策略
            Database.SetInitializer(new CreateDatabaseIfNotExists<StockDbContext>());
        }

        /// <summary>
        /// 股票信息表
        /// </summary>
        public DbSet<Stock> Stocks { get; set; }

        /// <summary>
        /// 股票行情记录表
        /// </summary>
        public DbSet<StockPrice> StockPrices { get; set; }

        /// <summary>
        /// 模型创建配置
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 配置Stock实体
            modelBuilder.Entity<Stock>()
                .HasKey(s => s.StockId);

            modelBuilder.Entity<Stock>()
                .Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<Stock>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            // 配置StockPrice实体
            modelBuilder.Entity<StockPrice>()
                .HasKey(p => p.PriceId);

            // 配置关系：一个股票有多个行情记录
            modelBuilder.Entity<StockPrice>()
                .HasRequired(p => p.Stock)
                .WithMany()
                .HasForeignKey(p => p.StockId);

            base.OnModelCreating(modelBuilder);
        }
    }
} 