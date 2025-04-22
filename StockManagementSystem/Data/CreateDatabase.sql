-- 创建数据库
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'StockManagementDB')
BEGIN
    CREATE DATABASE StockManagementDB;
END
GO

USE StockManagementDB;
GO

-- 创建股票表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stocks')
BEGIN
    CREATE TABLE Stocks (
        StockId INT PRIMARY KEY IDENTITY(1,1),
        Code NVARCHAR(10) NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        Type NVARCHAR(20) NULL,
        Industry NVARCHAR(50) NULL,
        ListingDate DATETIME NOT NULL,
        Description NVARCHAR(MAX) NULL,
        CreateTime DATETIME NOT NULL,
        UpdateTime DATETIME NOT NULL
    );
END
GO

-- 创建股票价格表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StockPrices')
BEGIN
    CREATE TABLE StockPrices (
        PriceId INT PRIMARY KEY IDENTITY(1,1),
        StockId INT NOT NULL,
        TradeDate DATETIME NOT NULL,
        OpenPrice DECIMAL(18,4) NOT NULL,
        ClosePrice DECIMAL(18,4) NOT NULL,
        HighPrice DECIMAL(18,4) NOT NULL,
        LowPrice DECIMAL(18,4) NOT NULL,
        Volume BIGINT NOT NULL,
        Amount DECIMAL(18,4) NOT NULL,
        ChangePercent DECIMAL(18,4) NOT NULL,
        CreateTime DATETIME NOT NULL,
        CONSTRAINT FK_StockPrices_Stocks FOREIGN KEY (StockId) REFERENCES Stocks(StockId)
    );
END
GO

-- 创建索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Stocks_Code' AND object_id = OBJECT_ID('Stocks'))
BEGIN
    CREATE UNIQUE INDEX IX_Stocks_Code ON Stocks(Code);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StockPrices_StockId_TradeDate' AND object_id = OBJECT_ID('StockPrices'))
BEGIN
    CREATE INDEX IX_StockPrices_StockId_TradeDate ON StockPrices(StockId, TradeDate);
END
GO

-- 创建示例数据（如果需要）
-- 插入示例股票
/*
IF NOT EXISTS (SELECT * FROM Stocks WHERE Code = '000001')
BEGIN
    INSERT INTO Stocks (Code, Name, Type, Industry, ListingDate, Description, CreateTime, UpdateTime)
    VALUES ('000001', '平安银行', 'A股', '银行', '1991-04-03', '平安银行股份有限公司是中国平安保险集团股份有限公司控股的一家股份制商业银行', GETDATE(), GETDATE());
END
GO
*/ 