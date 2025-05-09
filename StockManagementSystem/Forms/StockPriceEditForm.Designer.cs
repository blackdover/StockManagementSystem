namespace StockManagementSystem.Forms
{
    partial class StockPriceEditForm
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
            this.lblStock = new System.Windows.Forms.Label();
            this.cboStock = new System.Windows.Forms.ComboBox();
            this.lblTradeDate = new System.Windows.Forms.Label();
            this.dateTimePickerTradeDate = new System.Windows.Forms.DateTimePicker();
            this.lblOpenPrice = new System.Windows.Forms.Label();
            this.numOpenPrice = new System.Windows.Forms.NumericUpDown();
            this.lblClosePrice = new System.Windows.Forms.Label();
            this.numClosePrice = new System.Windows.Forms.NumericUpDown();
            this.lblHighPrice = new System.Windows.Forms.Label();
            this.numHighPrice = new System.Windows.Forms.NumericUpDown();
            this.lblLowPrice = new System.Windows.Forms.Label();
            this.numLowPrice = new System.Windows.Forms.NumericUpDown();
            this.lblVolume = new System.Windows.Forms.Label();
            this.numVolume = new System.Windows.Forms.NumericUpDown();
            this.lblAmount = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.lblChangePercent = new System.Windows.Forms.Label();
            this.numChangePercent = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numOpenPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClosePrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHighPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLowPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChangePercent)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStock
            // 
            this.lblStock.AutoSize = true;
            this.lblStock.Location = new System.Drawing.Point(45, 38);
            this.lblStock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(98, 18);
            this.lblStock.TabIndex = 0;
            this.lblStock.Text = "选择股票：";
            // 
            // cboStock
            // 
            this.cboStock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStock.FormattingEnabled = true;
            this.cboStock.Location = new System.Drawing.Point(152, 33);
            this.cboStock.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboStock.Name = "cboStock";
            this.cboStock.Size = new System.Drawing.Size(298, 26);
            this.cboStock.TabIndex = 1;
            // 
            // lblTradeDate
            // 
            this.lblTradeDate.AutoSize = true;
            this.lblTradeDate.Location = new System.Drawing.Point(45, 90);
            this.lblTradeDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTradeDate.Name = "lblTradeDate";
            this.lblTradeDate.Size = new System.Drawing.Size(98, 18);
            this.lblTradeDate.TabIndex = 2;
            this.lblTradeDate.Text = "交易日期：";
            // 
            // dateTimePickerTradeDate
            // 
            this.dateTimePickerTradeDate.Location = new System.Drawing.Point(152, 86);
            this.dateTimePickerTradeDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerTradeDate.Name = "dateTimePickerTradeDate";
            this.dateTimePickerTradeDate.Size = new System.Drawing.Size(298, 28);
            this.dateTimePickerTradeDate.TabIndex = 3;
            // 
            // lblOpenPrice
            // 
            this.lblOpenPrice.AutoSize = true;
            this.lblOpenPrice.Location = new System.Drawing.Point(45, 142);
            this.lblOpenPrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOpenPrice.Name = "lblOpenPrice";
            this.lblOpenPrice.Size = new System.Drawing.Size(80, 18);
            this.lblOpenPrice.TabIndex = 4;
            this.lblOpenPrice.Text = "开盘价：";
            // 
            // numOpenPrice
            // 
            this.numOpenPrice.DecimalPlaces = 2;
            this.numOpenPrice.Location = new System.Drawing.Point(152, 140);
            this.numOpenPrice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numOpenPrice.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numOpenPrice.Name = "numOpenPrice";
            this.numOpenPrice.Size = new System.Drawing.Size(180, 28);
            this.numOpenPrice.TabIndex = 5;
            // 
            // lblClosePrice
            // 
            this.lblClosePrice.AutoSize = true;
            this.lblClosePrice.Location = new System.Drawing.Point(45, 195);
            this.lblClosePrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClosePrice.Name = "lblClosePrice";
            this.lblClosePrice.Size = new System.Drawing.Size(80, 18);
            this.lblClosePrice.TabIndex = 6;
            this.lblClosePrice.Text = "收盘价：";
            // 
            // numClosePrice
            // 
            this.numClosePrice.DecimalPlaces = 2;
            this.numClosePrice.Location = new System.Drawing.Point(152, 192);
            this.numClosePrice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numClosePrice.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numClosePrice.Name = "numClosePrice";
            this.numClosePrice.Size = new System.Drawing.Size(180, 28);
            this.numClosePrice.TabIndex = 7;
            this.numClosePrice.ValueChanged += new System.EventHandler(this.numClosePrice_ValueChanged);
            // 
            // lblHighPrice
            // 
            this.lblHighPrice.AutoSize = true;
            this.lblHighPrice.Location = new System.Drawing.Point(45, 248);
            this.lblHighPrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHighPrice.Name = "lblHighPrice";
            this.lblHighPrice.Size = new System.Drawing.Size(80, 18);
            this.lblHighPrice.TabIndex = 8;
            this.lblHighPrice.Text = "最高价：";
            // 
            // numHighPrice
            // 
            this.numHighPrice.DecimalPlaces = 2;
            this.numHighPrice.Location = new System.Drawing.Point(152, 244);
            this.numHighPrice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numHighPrice.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numHighPrice.Name = "numHighPrice";
            this.numHighPrice.Size = new System.Drawing.Size(180, 28);
            this.numHighPrice.TabIndex = 9;
            this.numHighPrice.ValueChanged += new System.EventHandler(this.numHighPrice_ValueChanged);
            // 
            // lblLowPrice
            // 
            this.lblLowPrice.AutoSize = true;
            this.lblLowPrice.Location = new System.Drawing.Point(45, 300);
            this.lblLowPrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLowPrice.Name = "lblLowPrice";
            this.lblLowPrice.Size = new System.Drawing.Size(80, 18);
            this.lblLowPrice.TabIndex = 10;
            this.lblLowPrice.Text = "最低价：";
            // 
            // numLowPrice
            // 
            this.numLowPrice.DecimalPlaces = 2;
            this.numLowPrice.Location = new System.Drawing.Point(152, 297);
            this.numLowPrice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numLowPrice.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numLowPrice.Name = "numLowPrice";
            this.numLowPrice.Size = new System.Drawing.Size(180, 28);
            this.numLowPrice.TabIndex = 11;
            this.numLowPrice.ValueChanged += new System.EventHandler(this.numLowPrice_ValueChanged);
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(45, 352);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(80, 18);
            this.lblVolume.TabIndex = 12;
            this.lblVolume.Text = "成交量：";
            // 
            // numVolume
            // 
            this.numVolume.Location = new System.Drawing.Point(152, 350);
            this.numVolume.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numVolume.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numVolume.Name = "numVolume";
            this.numVolume.Size = new System.Drawing.Size(180, 28);
            this.numVolume.TabIndex = 13;
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(45, 405);
            this.lblAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(80, 18);
            this.lblAmount.TabIndex = 14;
            this.lblAmount.Text = "成交额：";
            // 
            // numAmount
            // 
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Location = new System.Drawing.Point(152, 402);
            this.numAmount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numAmount.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numAmount.Name = "numAmount";
            this.numAmount.Size = new System.Drawing.Size(180, 28);
            this.numAmount.TabIndex = 15;
            // 
            // lblChangePercent
            // 
            this.lblChangePercent.AutoSize = true;
            this.lblChangePercent.Location = new System.Drawing.Point(45, 458);
            this.lblChangePercent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblChangePercent.Name = "lblChangePercent";
            this.lblChangePercent.Size = new System.Drawing.Size(80, 18);
            this.lblChangePercent.TabIndex = 16;
            this.lblChangePercent.Text = "涨跌幅：";
            // 
            // numChangePercent
            // 
            this.numChangePercent.DecimalPlaces = 2;
            this.numChangePercent.Location = new System.Drawing.Point(152, 454);
            this.numChangePercent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numChangePercent.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numChangePercent.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numChangePercent.Name = "numChangePercent";
            this.numChangePercent.Size = new System.Drawing.Size(180, 28);
            this.numChangePercent.TabIndex = 17;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(152, 525);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 34);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(339, 525);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 34);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // StockPriceEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 616);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numChangePercent);
            this.Controls.Add(this.lblChangePercent);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.numVolume);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.numLowPrice);
            this.Controls.Add(this.lblLowPrice);
            this.Controls.Add(this.numHighPrice);
            this.Controls.Add(this.lblHighPrice);
            this.Controls.Add(this.numClosePrice);
            this.Controls.Add(this.lblClosePrice);
            this.Controls.Add(this.numOpenPrice);
            this.Controls.Add(this.lblOpenPrice);
            this.Controls.Add(this.dateTimePickerTradeDate);
            this.Controls.Add(this.lblTradeDate);
            this.Controls.Add(this.cboStock);
            this.Controls.Add(this.lblStock);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StockPriceEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "股票行情编辑";
            this.Load += new System.EventHandler(this.StockPriceEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numOpenPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClosePrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHighPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLowPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChangePercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.ComboBox cboStock;
        private System.Windows.Forms.Label lblTradeDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerTradeDate;
        private System.Windows.Forms.Label lblOpenPrice;
        private System.Windows.Forms.NumericUpDown numOpenPrice;
        private System.Windows.Forms.Label lblClosePrice;
        private System.Windows.Forms.NumericUpDown numClosePrice;
        private System.Windows.Forms.Label lblHighPrice;
        private System.Windows.Forms.NumericUpDown numHighPrice;
        private System.Windows.Forms.Label lblLowPrice;
        private System.Windows.Forms.NumericUpDown numLowPrice;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.NumericUpDown numVolume;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.Label lblChangePercent;
        private System.Windows.Forms.NumericUpDown numChangePercent;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
} 