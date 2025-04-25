# 库存管理系统 (Stock Management System)

## 项目概述

这是一个用 C#开发的库存管理系统，适合 C#初学者学习和理解 Windows 窗体应用程序的结构和开发方式。该系统允许用户管理股票信息和价格数据。

## 项目结构

### 1. 解决方案结构

```
StockManagementSystem/
├── StockManagementSystem.sln      # Visual Studio 解决方案文件
└── StockManagementSystem/         # 主项目文件夹
```

### 2. 主项目结构

```
StockManagementSystem/
├── Program.cs                     # 应用程序入口点
├── App.config                     # 应用程序配置文件
├── MainForm.cs                    # 主窗体代码
├── MainForm.Designer.cs           # 主窗体设计器生成的代码
├── MainForm.resx                  # 主窗体资源文件
├── Models/                        # 数据模型文件夹
├── Forms/                         # 窗体文件夹
├── Services/                      # 服务层文件夹
├── Data/                          # 数据访问层文件夹
├── Helpers/                       # 辅助工具类文件夹
└── Properties/                    # 项目属性文件夹
```

### 3. 模型层 (Models/)

存放数据模型类，定义了系统中的实体对象：

```
Models/
├── Stock.cs                       # 股票实体类
├── StockPrice.cs                  # 股票价格实体类
└── StockViewModel.cs              # 股票视图模型
```

### 4. 窗体层 (Forms/)

包含应用程序的各种窗体：

```
Forms/
├── DataIOForm.cs                  # 数据导入导出窗体
├── StockEditForm.cs               # 股票编辑窗体
├── StockFilterForm.cs             # 股票筛选窗体
├── StockPriceEditForm.cs          # 股票价格编辑窗体
└── StockPriceQueryForm.cs         # 股票价格查询窗体
```

每个窗体文件通常有三个关联文件：

- `.cs` - 包含窗体的业务逻辑
- `.Designer.cs` - 包含自动生成的界面设计代码
- `.resx` - 包含窗体资源

### 5. 服务层 (Services/)

包含业务逻辑和数据处理：

```
Services/
├── StockService.cs                # 股票服务
└── StockPriceService.cs           # 股票价格服务
```

### 6. 数据层 (Data/)

负责数据库操作和数据访问：

```
Data/
├── CreateDatabase.sql             # 数据库创建脚本
├── SqlHelper.cs                   # SQL助手类
└── StockDbContext.cs              # 数据库上下文
```

### 7. 辅助工具 (Helpers/)

包含辅助功能的类：

```
Helpers/
└── DateTimeHelper.cs              # 日期时间处理辅助类
```

## 应用程序流程

1. 应用程序从 `Program.cs` 的 `Main()` 方法启动
2. 初始化数据库连接
3. 创建并显示主窗体 `MainForm`
4. 用户通过主窗体访问各种功能

## 面向初学者的说明

### C# Windows Forms 应用程序基础知识

1. **Program.cs**：每个 Windows Forms 应用程序的入口点，包含 Main 方法
2. **Form 文件**：

   - `.cs` 文件包含窗体的业务逻辑代码
   - `.Designer.cs` 包含由设计器自动生成的 UI 代码
   - `.resx` 包含窗体的资源文件

3. **命名空间**：
   项目使用 `StockManagementSystem` 作为主命名空间，各个子文件夹对应子命名空间

4. **项目结构**：
   - **Models**: 定义数据结构
   - **Forms**: 用户界面
   - **Services**: 业务逻辑
   - **Data**: 数据访问
   - **Helpers**: 工具函数

### 学习建议

1. 从 `Program.cs` 开始，了解程序的启动流程
2. 查看 `MainForm.cs` 了解主窗体的功能和布局
3. 探索各个 Model 类，理解数据结构
4. 学习 Services 类中的业务逻辑
5. 了解 Data 文件夹中数据访问的实现

## 数据库结构

数据库包含以下主要表：

- Stocks - 存储股票基本信息
- StockPrices - 存储股票价格数据

数据库创建脚本位于 `Data/CreateDatabase.sql`。
