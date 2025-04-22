namespace StockManagementSystem
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddStock = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEditStock = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteStock = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAddPrice = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonViewPrice = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewStocks = new System.Windows.Forms.ListView();
            this.columnHeaderId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderIndustry = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dataGridViewPrices = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrices)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddStock,
            this.toolStripButtonEditStock,
            this.toolStripButtonDeleteStock,
            this.toolStripSeparator1,
            this.toolStripButtonAddPrice,
            this.toolStripButtonViewPrice,
            this.toolStripButtonExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(984, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAddStock
            // 
            this.toolStripButtonAddStock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonAddStock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddStock.Name = "toolStripButtonAddStock";
            this.toolStripButtonAddStock.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonAddStock.Text = "添加股票";
            // 
            // toolStripButtonEditStock
            // 
            this.toolStripButtonEditStock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonEditStock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEditStock.Name = "toolStripButtonEditStock";
            this.toolStripButtonEditStock.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonEditStock.Text = "编辑股票";
            // 
            // toolStripButtonDeleteStock
            // 
            this.toolStripButtonDeleteStock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonDeleteStock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteStock.Name = "toolStripButtonDeleteStock";
            this.toolStripButtonDeleteStock.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonDeleteStock.Text = "删除股票";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonAddPrice
            // 
            this.toolStripButtonAddPrice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonAddPrice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddPrice.Name = "toolStripButtonAddPrice";
            this.toolStripButtonAddPrice.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonAddPrice.Text = "添加行情";
            // 
            // toolStripButtonViewPrice
            // 
            this.toolStripButtonViewPrice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonViewPrice.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonViewPrice.Name = "toolStripButtonViewPrice";
            this.toolStripButtonViewPrice.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonViewPrice.Text = "查看行情";
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonExit.Text = "退出";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(984, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "欢迎使用股票管理系统";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewStocks);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewPrices);
            this.splitContainer1.Size = new System.Drawing.Size(984, 514);
            this.splitContainer1.SplitterDistance = 244;
            this.splitContainer1.TabIndex = 3;
            // 
            // listViewStocks
            // 
            this.listViewStocks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderCode,
            this.columnHeaderName,
            this.columnHeaderType,
            this.columnHeaderIndustry,
            this.columnHeaderDescription});
            this.listViewStocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewStocks.FullRowSelect = true;
            this.listViewStocks.GridLines = true;
            this.listViewStocks.HideSelection = false;
            this.listViewStocks.Location = new System.Drawing.Point(0, 0);
            this.listViewStocks.Name = "listViewStocks";
            this.listViewStocks.Size = new System.Drawing.Size(984, 244);
            this.listViewStocks.TabIndex = 0;
            this.listViewStocks.UseCompatibleStateImageBehavior = false;
            this.listViewStocks.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderId
            // 
            this.columnHeaderId.Text = "ID";
            this.columnHeaderId.Width = 50;
            // 
            // columnHeaderCode
            // 
            this.columnHeaderCode.Text = "股票代码";
            this.columnHeaderCode.Width = 100;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "股票名称";
            this.columnHeaderName.Width = 150;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "股票类型";
            this.columnHeaderType.Width = 100;
            // 
            // columnHeaderIndustry
            // 
            this.columnHeaderIndustry.Text = "所属行业";
            this.columnHeaderIndustry.Width = 150;
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "股票描述";
            this.columnHeaderDescription.Width = 300;
            // 
            // dataGridViewPrices
            // 
            this.dataGridViewPrices.AllowUserToAddRows = false;
            this.dataGridViewPrices.AllowUserToDeleteRows = false;
            this.dataGridViewPrices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPrices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPrices.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPrices.Name = "dataGridViewPrices";
            this.dataGridViewPrices.ReadOnly = true;
            this.dataGridViewPrices.RowTemplate.Height = 23;
            this.dataGridViewPrices.Size = new System.Drawing.Size(984, 266);
            this.dataGridViewPrices.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "股票行情查看与分析管理系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddStock;
        private System.Windows.Forms.ToolStripButton toolStripButtonEditStock;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteStock;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddPrice;
        private System.Windows.Forms.ToolStripButton toolStripButtonViewPrice;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewStocks;
        private System.Windows.Forms.ColumnHeader columnHeaderId;
        private System.Windows.Forms.ColumnHeader columnHeaderCode;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderIndustry;
        private System.Windows.Forms.ColumnHeader columnHeaderDescription;
        private System.Windows.Forms.DataGridView dataGridViewPrices;
    }
}

