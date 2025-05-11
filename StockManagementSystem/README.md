## 使用指南

### 主界面

主界面分为股票列表和价格图表两个区域：

- 左侧显示股票列表，可以点击列表选择股票
- 右侧显示所选股票的价格走势图和成交量图表

### 常用操作

- **添加股票**：点击工具栏的"添加"按钮
- **编辑股票**：选择一只股票后点击"编辑"按钮
- **查看行情**：选择股票后自动显示图表，或点击"行情"按钮查看详细数据
- **筛选数据**：点击"筛选"按钮打开筛选窗口
- **导入导出**：点击"数据 IO"按钮打开导入导出窗口，**项目附带了一组样例数据集，在 data 文件夹下 stock 是股票信息、stock price 是股票行情信息**

### 数据导入导出

- 支持股票信息和行情数据的 CSV 导入导出
- 可选择时间范围导出行情数据
- 导入时自动检测重复数据
- 支持大文件批量导入，有进度显示
- **项目附带了一组样例数据集，在 data 文件夹下 stock 是股票信息、stock price 是股票行情信息**

## 项目结构

- **Forms**：用户界面窗体
  - MainForm：主窗体
  - StockEditForm：股票编辑窗体
  - StockPriceEditForm：价格编辑窗体
  - StockFilterForm：筛选窗体
  - DataIOForm：数据导入导出窗体
  - StockPriceQueryForm：价格查询窗体
- **Models**：数据模型
  - Stock：股票基础信息
  - StockPrice：股票价格数据
- **Data**：业务逻辑
  - StockService：股票业务逻辑
  - StockPriceService：价格数据业务逻辑
  - SqlHelper：数据库访问封装
- **Service**:辅助类
  - DateTimeHelper:时间辅助类
