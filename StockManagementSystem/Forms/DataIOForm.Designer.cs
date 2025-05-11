namespace StockManagementSystem.Forms
{
    partial class DataIOForm
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
            this.groupBoxExport = new System.Windows.Forms.GroupBox();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.labelTo = new System.Windows.Forms.Label();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.labelDateRange = new System.Windows.Forms.Label();
            this.checkBoxAllPrices = new System.Windows.Forms.CheckBox();
            this.btnExportPrices = new System.Windows.Forms.Button();
            this.btnExportStocks = new System.Windows.Forms.Button();
            this.groupBoxImport = new System.Windows.Forms.GroupBox();
            this.btnImportPrices = new System.Windows.Forms.Button();
            this.btnImportStocks = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBoxExport.SuspendLayout();
            this.groupBoxImport.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxExport
            // 
            this.groupBoxExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxExport.Controls.Add(this.dateTimePickerEnd);
            this.groupBoxExport.Controls.Add(this.labelTo);
            this.groupBoxExport.Controls.Add(this.dateTimePickerStart);
            this.groupBoxExport.Controls.Add(this.labelDateRange);
            this.groupBoxExport.Controls.Add(this.checkBoxAllPrices);
            this.groupBoxExport.Controls.Add(this.btnExportPrices);
            this.groupBoxExport.Controls.Add(this.btnExportStocks);
            this.groupBoxExport.Location = new System.Drawing.Point(18, 18);
            this.groupBoxExport.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxExport.Name = "groupBoxExport";
            this.groupBoxExport.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxExport.Size = new System.Drawing.Size(840, 200);
            this.groupBoxExport.TabIndex = 0;
            this.groupBoxExport.TabStop = false;
            this.groupBoxExport.Text = "数据导出";
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Location = new System.Drawing.Point(490, 148);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(193, 28);
            this.dateTimePickerEnd.TabIndex = 6;
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(460, 154);
            this.labelTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(26, 18);
            this.labelTo.TabIndex = 5;
            this.labelTo.Text = "至";
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Location = new System.Drawing.Point(261, 148);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(193, 28);
            this.dateTimePickerStart.TabIndex = 4;
            // 
            // labelDateRange
            // 
            this.labelDateRange.AutoSize = true;
            this.labelDateRange.Location = new System.Drawing.Point(172, 154);
            this.labelDateRange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDateRange.Name = "labelDateRange";
            this.labelDateRange.Size = new System.Drawing.Size(98, 18);
            this.labelDateRange.TabIndex = 3;
            this.labelDateRange.Text = "日期范围：";
            // 
            // checkBoxAllPrices
            // 
            this.checkBoxAllPrices.AutoSize = true;
            this.checkBoxAllPrices.Location = new System.Drawing.Point(22, 154);
            this.checkBoxAllPrices.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxAllPrices.Name = "checkBoxAllPrices";
            this.checkBoxAllPrices.Size = new System.Drawing.Size(142, 22);
            this.checkBoxAllPrices.TabIndex = 2;
            this.checkBoxAllPrices.Text = "导出所有价格";
            this.checkBoxAllPrices.UseVisualStyleBackColor = true;
            this.checkBoxAllPrices.CheckedChanged += new System.EventHandler(this.checkBoxAllPrices_CheckedChanged);
            // 
            // btnExportPrices
            // 
            this.btnExportPrices.Location = new System.Drawing.Point(22, 94);
            this.btnExportPrices.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportPrices.Name = "btnExportPrices";
            this.btnExportPrices.Size = new System.Drawing.Size(214, 34);
            this.btnExportPrices.TabIndex = 1;
            this.btnExportPrices.Text = "导出股票行情数据";
            this.btnExportPrices.UseVisualStyleBackColor = true;
            this.btnExportPrices.Click += new System.EventHandler(this.btnExportPrices_Click);
            // 
            // btnExportStocks
            // 
            this.btnExportStocks.Location = new System.Drawing.Point(22, 44);
            this.btnExportStocks.Margin = new System.Windows.Forms.Padding(4);
            this.btnExportStocks.Name = "btnExportStocks";
            this.btnExportStocks.Size = new System.Drawing.Size(214, 34);
            this.btnExportStocks.TabIndex = 0;
            this.btnExportStocks.Text = "导出股票基本数据";
            this.btnExportStocks.UseVisualStyleBackColor = true;
            this.btnExportStocks.Click += new System.EventHandler(this.btnExportStocks_Click);
            // 
            // groupBoxImport
            // 
            this.groupBoxImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxImport.Controls.Add(this.btnImportPrices);
            this.groupBoxImport.Controls.Add(this.btnImportStocks);
            this.groupBoxImport.Location = new System.Drawing.Point(18, 226);
            this.groupBoxImport.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxImport.Name = "groupBoxImport";
            this.groupBoxImport.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxImport.Size = new System.Drawing.Size(840, 161);
            this.groupBoxImport.TabIndex = 1;
            this.groupBoxImport.TabStop = false;
            this.groupBoxImport.Text = "数据导入";
            // 
            // btnImportPrices
            // 
            this.btnImportPrices.Location = new System.Drawing.Point(246, 44);
            this.btnImportPrices.Margin = new System.Windows.Forms.Padding(4);
            this.btnImportPrices.Name = "btnImportPrices";
            this.btnImportPrices.Size = new System.Drawing.Size(214, 34);
            this.btnImportPrices.TabIndex = 1;
            this.btnImportPrices.Text = "导入股票行情数据";
            this.btnImportPrices.UseVisualStyleBackColor = true;
            this.btnImportPrices.Click += new System.EventHandler(this.btnImportPrices_Click);
            // 
            // btnImportStocks
            // 
            this.btnImportStocks.Location = new System.Drawing.Point(22, 44);
            this.btnImportStocks.Margin = new System.Windows.Forms.Padding(4);
            this.btnImportStocks.Name = "btnImportStocks";
            this.btnImportStocks.Size = new System.Drawing.Size(214, 34);
            this.btnImportStocks.TabIndex = 0;
            this.btnImportStocks.Text = "导入股票基本数据";
            this.btnImportStocks.UseVisualStyleBackColor = true;
            this.btnImportStocks.Click += new System.EventHandler(this.btnImportStocks_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 391);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(876, 31);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(202, 24);
            this.labelStatus.Text = "准备执行数据导入导出...";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(746, 353);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 34);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // DataIOForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 422);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBoxImport);
            this.Controls.Add(this.groupBoxExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataIOForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数据导入导出";
            this.Load += new System.EventHandler(this.DataIOForm_Load);
            this.groupBoxExport.ResumeLayout(false);
            this.groupBoxExport.PerformLayout();
            this.groupBoxImport.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxExport;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label labelDateRange;
        private System.Windows.Forms.CheckBox checkBoxAllPrices;
        private System.Windows.Forms.Button btnExportPrices;
        private System.Windows.Forms.Button btnExportStocks;
        private System.Windows.Forms.GroupBox groupBoxImport;
        private System.Windows.Forms.Button btnImportPrices;
        private System.Windows.Forms.Button btnImportStocks;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.Button btnClose;
    }
}