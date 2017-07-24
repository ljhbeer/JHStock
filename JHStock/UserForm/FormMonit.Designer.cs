namespace JHStock
{
    partial class FormMonit
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
        	this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
        	this.textBoxInfor = new System.Windows.Forms.TextBox();
        	this.textBox3 = new System.Windows.Forms.TextBox();
        	this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.buttonToTxt = new System.Windows.Forms.Button();
        	this.checkBoxTable = new System.Windows.Forms.CheckBox();
        	this.buttonAddToTXDBlock = new System.Windows.Forms.Button();
        	this.groupBox3 = new System.Windows.Forms.GroupBox();
        	this.checkBoxDebugOutPut = new System.Windows.Forms.CheckBox();
        	this.buttonReCompute = new System.Windows.Forms.Button();
        	this.checkBoxMonitdays = new System.Windows.Forms.CheckBox();
        	this.checkBoxUserDefinitionStocks = new System.Windows.Forms.CheckBox();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.textBoxExchangeTime = new System.Windows.Forms.TextBox();
        	this.buttonCheckData = new System.Windows.Forms.Button();
        	this.buttonConfig = new System.Windows.Forms.Button();
        	this.dgv = new System.Windows.Forms.DataGridView();
        	this.tableLayoutPanel2.SuspendLayout();
        	this.flowLayoutPanel1.SuspendLayout();
        	this.groupBox2.SuspendLayout();
        	this.groupBox3.SuspendLayout();
        	this.groupBox1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// tableLayoutPanel2
        	// 
        	this.tableLayoutPanel2.ColumnCount = 3;
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34F));
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 95.0376F));
        	this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
        	this.tableLayoutPanel2.Controls.Add(this.textBoxInfor, 1, 2);
        	this.tableLayoutPanel2.Controls.Add(this.textBox3, 0, 2);
        	this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 0);
        	this.tableLayoutPanel2.Controls.Add(this.buttonConfig, 0, 0);
        	this.tableLayoutPanel2.Controls.Add(this.dgv, 1, 1);
        	this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
        	this.tableLayoutPanel2.Name = "tableLayoutPanel2";
        	this.tableLayoutPanel2.RowCount = 3;
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        	this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
        	this.tableLayoutPanel2.Size = new System.Drawing.Size(749, 505);
        	this.tableLayoutPanel2.TabIndex = 3;
        	// 
        	// textBoxInfor
        	// 
        	this.textBoxInfor.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.textBoxInfor.Location = new System.Drawing.Point(37, 482);
        	this.textBoxInfor.Name = "textBoxInfor";
        	this.textBoxInfor.Size = new System.Drawing.Size(689, 21);
        	this.textBoxInfor.TabIndex = 0;
        	// 
        	// textBox3
        	// 
        	this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.textBox3.Location = new System.Drawing.Point(3, 482);
        	this.textBox3.Name = "textBox3";
        	this.textBox3.ReadOnly = true;
        	this.textBox3.Size = new System.Drawing.Size(28, 21);
        	this.textBox3.TabIndex = 1;
        	this.textBox3.Text = "状态";
        	// 
        	// flowLayoutPanel1
        	// 
        	this.flowLayoutPanel1.Controls.Add(this.groupBox2);
        	this.flowLayoutPanel1.Controls.Add(this.groupBox3);
        	this.flowLayoutPanel1.Controls.Add(this.groupBox1);
        	this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.flowLayoutPanel1.Location = new System.Drawing.Point(37, 3);
        	this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        	this.flowLayoutPanel1.Size = new System.Drawing.Size(689, 51);
        	this.flowLayoutPanel1.TabIndex = 15;
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Controls.Add(this.buttonToTxt);
        	this.groupBox2.Controls.Add(this.checkBoxTable);
        	this.groupBox2.Controls.Add(this.buttonAddToTXDBlock);
        	this.groupBox2.Location = new System.Drawing.Point(3, 3);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Size = new System.Drawing.Size(124, 46);
        	this.groupBox2.TabIndex = 28;
        	this.groupBox2.TabStop = false;
        	// 
        	// buttonToTxt
        	// 
        	this.buttonToTxt.Location = new System.Drawing.Point(89, 6);
        	this.buttonToTxt.Name = "buttonToTxt";
        	this.buttonToTxt.Size = new System.Drawing.Size(31, 18);
        	this.buttonToTxt.TabIndex = 18;
        	this.buttonToTxt.Text = "ToTxt";
        	this.buttonToTxt.UseVisualStyleBackColor = true;
        	this.buttonToTxt.Click += new System.EventHandler(this.buttonToTxt_Click);
        	// 
        	// checkBoxTable
        	// 
        	this.checkBoxTable.AutoSize = true;
        	this.checkBoxTable.Location = new System.Drawing.Point(0, 27);
        	this.checkBoxTable.Name = "checkBoxTable";
        	this.checkBoxTable.Size = new System.Drawing.Size(78, 16);
        	this.checkBoxTable.TabIndex = 25;
        	this.checkBoxTable.Text = "Table分隔";
        	this.checkBoxTable.UseVisualStyleBackColor = true;
        	// 
        	// buttonAddToTXDBlock
        	// 
        	this.buttonAddToTXDBlock.Location = new System.Drawing.Point(0, 5);
        	this.buttonAddToTXDBlock.Name = "buttonAddToTXDBlock";
        	this.buttonAddToTXDBlock.Size = new System.Drawing.Size(87, 19);
        	this.buttonAddToTXDBlock.TabIndex = 14;
        	this.buttonAddToTXDBlock.Text = "AddToTXDBlock";
        	this.buttonAddToTXDBlock.UseVisualStyleBackColor = true;
        	this.buttonAddToTXDBlock.Click += new System.EventHandler(this.buttonAddToTXDBlock_Click);
        	// 
        	// groupBox3
        	// 
        	this.groupBox3.Controls.Add(this.checkBoxDebugOutPut);
        	this.groupBox3.Controls.Add(this.buttonReCompute);
        	this.groupBox3.Controls.Add(this.checkBoxMonitdays);
        	this.groupBox3.Controls.Add(this.checkBoxUserDefinitionStocks);
        	this.groupBox3.Location = new System.Drawing.Point(133, 3);
        	this.groupBox3.Name = "groupBox3";
        	this.groupBox3.Size = new System.Drawing.Size(247, 46);
        	this.groupBox3.TabIndex = 29;
        	this.groupBox3.TabStop = false;
        	// 
        	// checkBoxDebugOutPut
        	// 
        	this.checkBoxDebugOutPut.AutoSize = true;
        	this.checkBoxDebugOutPut.Location = new System.Drawing.Point(114, 10);
        	this.checkBoxDebugOutPut.Name = "checkBoxDebugOutPut";
        	this.checkBoxDebugOutPut.Size = new System.Drawing.Size(78, 16);
        	this.checkBoxDebugOutPut.TabIndex = 27;
        	this.checkBoxDebugOutPut.Text = "Debug输出";
        	this.checkBoxDebugOutPut.UseVisualStyleBackColor = true;
        	// 
        	// buttonReCompute
        	// 
        	this.buttonReCompute.Location = new System.Drawing.Point(205, 9);
        	this.buttonReCompute.Name = "buttonReCompute";
        	this.buttonReCompute.Size = new System.Drawing.Size(42, 18);
        	this.buttonReCompute.TabIndex = 23;
        	this.buttonReCompute.Text = "计算";
        	this.buttonReCompute.UseVisualStyleBackColor = true;
        	this.buttonReCompute.Click += new System.EventHandler(this.buttonReCompute_Click);
        	// 
        	// checkBoxMonitdays
        	// 
        	this.checkBoxMonitdays.AutoSize = true;
        	this.checkBoxMonitdays.Location = new System.Drawing.Point(0, 32);
        	this.checkBoxMonitdays.Name = "checkBoxMonitdays";
        	this.checkBoxMonitdays.Size = new System.Drawing.Size(162, 16);
        	this.checkBoxMonitdays.TabIndex = 25;
        	this.checkBoxMonitdays.Text = "后续监测1天（默认两天）";
        	this.checkBoxMonitdays.UseVisualStyleBackColor = true;
        	// 
        	// checkBoxUserDefinitionStocks
        	// 
        	this.checkBoxUserDefinitionStocks.AutoSize = true;
        	this.checkBoxUserDefinitionStocks.Location = new System.Drawing.Point(0, 11);
        	this.checkBoxUserDefinitionStocks.Name = "checkBoxUserDefinitionStocks";
        	this.checkBoxUserDefinitionStocks.Size = new System.Drawing.Size(108, 16);
        	this.checkBoxUserDefinitionStocks.TabIndex = 26;
        	this.checkBoxUserDefinitionStocks.Text = "自定义检测股票";
        	this.checkBoxUserDefinitionStocks.UseVisualStyleBackColor = true;
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Controls.Add(this.textBoxExchangeTime);
        	this.groupBox1.Controls.Add(this.buttonCheckData);
        	this.groupBox1.Location = new System.Drawing.Point(386, 3);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(89, 46);
        	this.groupBox1.TabIndex = 27;
        	this.groupBox1.TabStop = false;
        	// 
        	// textBoxExchangeTime
        	// 
        	this.textBoxExchangeTime.Location = new System.Drawing.Point(4, 27);
        	this.textBoxExchangeTime.Name = "textBoxExchangeTime";
        	this.textBoxExchangeTime.ReadOnly = true;
        	this.textBoxExchangeTime.Size = new System.Drawing.Size(80, 21);
        	this.textBoxExchangeTime.TabIndex = 31;
        	// 
        	// buttonCheckData
        	// 
        	this.buttonCheckData.Location = new System.Drawing.Point(0, 6);
        	this.buttonCheckData.Name = "buttonCheckData";
        	this.buttonCheckData.Size = new System.Drawing.Size(85, 21);
        	this.buttonCheckData.TabIndex = 30;
        	this.buttonCheckData.Text = "实时交易数据";
        	this.buttonCheckData.UseVisualStyleBackColor = true;
        	this.buttonCheckData.Click += new System.EventHandler(this.buttonCheckData_Click);
        	// 
        	// buttonConfig
        	// 
        	this.buttonConfig.Location = new System.Drawing.Point(3, 3);
        	this.buttonConfig.Name = "buttonConfig";
        	this.buttonConfig.Size = new System.Drawing.Size(28, 39);
        	this.buttonConfig.TabIndex = 23;
        	this.buttonConfig.Text = "配置";
        	this.buttonConfig.UseVisualStyleBackColor = true;
        	this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
        	// 
        	// dgv
        	// 
        	this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.dgv.Location = new System.Drawing.Point(37, 60);
        	this.dgv.Name = "dgv";
        	this.dgv.RowTemplate.Height = 23;
        	this.dgv.Size = new System.Drawing.Size(689, 416);
        	this.dgv.TabIndex = 2;
        	this.dgv.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellContentClick);
        	// 
        	// FormMonit
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(749, 505);
        	this.Controls.Add(this.tableLayoutPanel2);
        	this.Name = "FormMonit";
        	this.Text = "FormMonit";
        	this.Load += new System.EventHandler(this.FormMonit_Load);
        	this.tableLayoutPanel2.ResumeLayout(false);
        	this.tableLayoutPanel2.PerformLayout();
        	this.flowLayoutPanel1.ResumeLayout(false);
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	this.groupBox3.ResumeLayout(false);
        	this.groupBox3.PerformLayout();
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
        	this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.TextBox textBoxInfor;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button buttonAddToTXDBlock;
        private System.Windows.Forms.Button buttonToTxt;
        private System.Windows.Forms.Button buttonReCompute;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Button buttonCheckData;
        private System.Windows.Forms.CheckBox checkBoxTable;
        private System.Windows.Forms.CheckBox checkBoxMonitdays;
        private System.Windows.Forms.CheckBox checkBoxUserDefinitionStocks;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxExchangeTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBoxDebugOutPut;
    }
}