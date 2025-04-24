namespace StockManagementSystem
{
    partial class StockFilterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxFilter = new System.Windows.Forms.GroupBox();
            this.txtStockName = new System.Windows.Forms.TextBox();
            this.txtStockCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.cboStockType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboIndustry = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxResult = new System.Windows.Forms.GroupBox();
            this.listViewFilteredStocks = new System.Windows.Forms.ListView();
            this.columnCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnIndustry = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblResultCount = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBoxFilter.SuspendLayout();
            this.groupBoxResult.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxFilter
            // 
            this.groupBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFilter.Controls.Add(this.txtStockName);
            this.groupBoxFilter.Controls.Add(this.txtStockCode);
            this.groupBoxFilter.Controls.Add(this.label3);
            this.groupBoxFilter.Controls.Add(this.label4);
            this.groupBoxFilter.Controls.Add(this.btnReset);
            this.groupBoxFilter.Controls.Add(this.btnFilter);
            this.groupBoxFilter.Controls.Add(this.cboStockType);
            this.groupBoxFilter.Controls.Add(this.label2);
            this.groupBoxFilter.Controls.Add(this.cboIndustry);
            this.groupBoxFilter.Controls.Add(this.label1);
            this.groupBoxFilter.Location = new System.Drawing.Point(12, 12);
            this.groupBoxFilter.Name = "groupBoxFilter";
            this.groupBoxFilter.Size = new System.Drawing.Size(760, 100);
            this.groupBoxFilter.TabIndex = 4;
            this.groupBoxFilter.TabStop = false;
            this.groupBoxFilter.Text = "筛选条件";
            // 
            // txtStockName
            // 
            this.txtStockName.Location = new System.Drawing.Point(380, 62);
            this.txtStockName.Name = "txtStockName";
            this.txtStockName.Size = new System.Drawing.Size(193, 21);
            this.txtStockName.TabIndex = 9;
            // 
            // txtStockCode
            // 
            this.txtStockCode.Location = new System.Drawing.Point(92, 62);
            this.txtStockCode.Name = "txtStockCode";
            this.txtStockCode.Size = new System.Drawing.Size(193, 21);
            this.txtStockCode.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(309, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "股票名称：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "股票代码：";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(679, 40);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(598, 40);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 4;
            this.btnFilter.Text = "筛选";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // cboStockType
            // 
            this.cboStockType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStockType.FormattingEnabled = true;
            this.cboStockType.Location = new System.Drawing.Point(380, 29);
            this.cboStockType.Name = "cboStockType";
            this.cboStockType.Size = new System.Drawing.Size(193, 20);
            this.cboStockType.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "股票类型：";
            // 
            // cboIndustry
            // 
            this.cboIndustry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIndustry.FormattingEnabled = true;
            this.cboIndustry.Location = new System.Drawing.Point(92, 29);
            this.cboIndustry.Name = "cboIndustry";
            this.cboIndustry.Size = new System.Drawing.Size(193, 20);
            this.cboIndustry.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "所属行业：";
            // 
            // groupBoxResult
            // 
            this.groupBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxResult.Controls.Add(this.listViewFilteredStocks);
            this.groupBoxResult.Location = new System.Drawing.Point(12, 118);
            this.groupBoxResult.Name = "groupBoxResult";
            this.groupBoxResult.Size = new System.Drawing.Size(760, 383);
            this.groupBoxResult.TabIndex = 5;
            this.groupBoxResult.TabStop = false;
            this.groupBoxResult.Text = "筛选结果";
            // 
            // listViewFilteredStocks
            // 
            this.listViewFilteredStocks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCode,
            this.columnName,
            this.columnType,
            this.columnIndustry});
            this.listViewFilteredStocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFilteredStocks.FullRowSelect = true;
            this.listViewFilteredStocks.HideSelection = false;
            this.listViewFilteredStocks.Location = new System.Drawing.Point(3, 17);
            this.listViewFilteredStocks.MultiSelect = false;
            this.listViewFilteredStocks.Name = "listViewFilteredStocks";
            this.listViewFilteredStocks.Size = new System.Drawing.Size(754, 363);
            this.listViewFilteredStocks.TabIndex = 0;
            this.listViewFilteredStocks.UseCompatibleStateImageBehavior = false;
            this.listViewFilteredStocks.View = System.Windows.Forms.View.Details;
            this.listViewFilteredStocks.DoubleClick += new System.EventHandler(this.listViewFilteredStocks_DoubleClick);
            // 
            // columnCode
            // 
            this.columnCode.Text = "股票代码";
            this.columnCode.Width = 80;
            // 
            // columnName
            // 
            this.columnName.Text = "股票名称";
            this.columnName.Width = 120;
            // 
            // columnType
            // 
            this.columnType.Text = "股票类型";
            this.columnType.Width = 100;
            // 
            // columnIndustry
            // 
            this.columnIndustry.Text = "所属行业";
            this.columnIndustry.Width = 150;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lblResultCount);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Location = new System.Drawing.Point(12, 507);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(760, 42);
            this.panel1.TabIndex = 6;
            // 
            // lblResultCount
            // 
            this.lblResultCount.AutoSize = true;
            this.lblResultCount.Location = new System.Drawing.Point(13, 15);
            this.lblResultCount.Name = "lblResultCount";
            this.lblResultCount.Size = new System.Drawing.Size(101, 12);
            this.lblResultCount.TabIndex = 2;
            this.lblResultCount.Text = "准备执行筛选...";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(682, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(601, 10);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // StockFilterForm
            // 
            this.AcceptButton = this.btnFilter;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBoxResult);
            this.Controls.Add(this.groupBoxFilter);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "StockFilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "股票筛选";
            this.Load += new System.EventHandler(this.StockFilterForm_Load);
            this.groupBoxFilter.ResumeLayout(false);
            this.groupBoxFilter.PerformLayout();
            this.groupBoxResult.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxFilter;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.ComboBox cboStockType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboIndustry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxResult;
        private System.Windows.Forms.ListView listViewFilteredStocks;
        private System.Windows.Forms.ColumnHeader columnCode;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ColumnHeader columnIndustry;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblResultCount;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtStockName;
        private System.Windows.Forms.TextBox txtStockCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}