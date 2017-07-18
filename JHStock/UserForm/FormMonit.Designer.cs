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
            this.buttonAddToTXDBlock = new System.Windows.Forms.Button();
            this.buttonToTxt = new System.Windows.Forms.Button();
            this.checkBoxTable = new System.Windows.Forms.CheckBox();
            this.buttonReCompute = new System.Windows.Forms.Button();
            this.checkBoxMonitdays = new System.Windows.Forms.CheckBox();
            this.checkBoxUserDefinitionStocks = new System.Windows.Forms.CheckBox();
            this.buttonCheckData = new System.Windows.Forms.Button();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
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
            this.flowLayoutPanel1.Controls.Add(this.buttonAddToTXDBlock);
            this.flowLayoutPanel1.Controls.Add(this.buttonToTxt);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxTable);
            this.flowLayoutPanel1.Controls.Add(this.buttonReCompute);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxMonitdays);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxUserDefinitionStocks);
            this.flowLayoutPanel1.Controls.Add(this.buttonCheckData);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(37, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(689, 39);
            this.flowLayoutPanel1.TabIndex = 15;
            // 
            // buttonAddToTXDBlock
            // 
            this.buttonAddToTXDBlock.Location = new System.Drawing.Point(3, 3);
            this.buttonAddToTXDBlock.Name = "buttonAddToTXDBlock";
            this.buttonAddToTXDBlock.Size = new System.Drawing.Size(90, 19);
            this.buttonAddToTXDBlock.TabIndex = 14;
            this.buttonAddToTXDBlock.Text = "AddToTXDBlock";
            this.buttonAddToTXDBlock.UseVisualStyleBackColor = true;
            this.buttonAddToTXDBlock.Click += new System.EventHandler(this.buttonAddToTXDBlock_Click);
            // 
            // buttonToTxt
            // 
            this.buttonToTxt.Location = new System.Drawing.Point(99, 3);
            this.buttonToTxt.Name = "buttonToTxt";
            this.buttonToTxt.Size = new System.Drawing.Size(40, 18);
            this.buttonToTxt.TabIndex = 18;
            this.buttonToTxt.Text = "ToTxt";
            this.buttonToTxt.UseVisualStyleBackColor = true;
            this.buttonToTxt.Click += new System.EventHandler(this.buttonToTxt_Click);
            // 
            // checkBoxTable
            // 
            this.checkBoxTable.AutoSize = true;
            this.checkBoxTable.Location = new System.Drawing.Point(145, 3);
            this.checkBoxTable.Name = "checkBoxTable";
            this.checkBoxTable.Size = new System.Drawing.Size(78, 16);
            this.checkBoxTable.TabIndex = 25;
            this.checkBoxTable.Text = "Table分隔";
            this.checkBoxTable.UseVisualStyleBackColor = true;
            // 
            // buttonReCompute
            // 
            this.buttonReCompute.Location = new System.Drawing.Point(229, 3);
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
            this.checkBoxMonitdays.Location = new System.Drawing.Point(277, 3);
            this.checkBoxMonitdays.Name = "checkBoxMonitdays";
            this.checkBoxMonitdays.Size = new System.Drawing.Size(162, 16);
            this.checkBoxMonitdays.TabIndex = 25;
            this.checkBoxMonitdays.Text = "后续监测1天（默认两天）";
            this.checkBoxMonitdays.UseVisualStyleBackColor = true;
            // 
            // checkBoxUserDefinitionStocks
            // 
            this.checkBoxUserDefinitionStocks.AutoSize = true;
            this.checkBoxUserDefinitionStocks.Location = new System.Drawing.Point(445, 3);
            this.checkBoxUserDefinitionStocks.Name = "checkBoxUserDefinitionStocks";
            this.checkBoxUserDefinitionStocks.Size = new System.Drawing.Size(108, 16);
            this.checkBoxUserDefinitionStocks.TabIndex = 26;
            this.checkBoxUserDefinitionStocks.Text = "自定义检测股票";
            this.checkBoxUserDefinitionStocks.UseVisualStyleBackColor = true;
            // 
            // buttonCheckData
            // 
            this.buttonCheckData.Location = new System.Drawing.Point(559, 3);
            this.buttonCheckData.Name = "buttonCheckData";
            this.buttonCheckData.Size = new System.Drawing.Size(38, 21);
            this.buttonCheckData.TabIndex = 30;
            this.buttonCheckData.Text = "更新";
            this.buttonCheckData.UseVisualStyleBackColor = true;
            this.buttonCheckData.Visible = false;
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
            this.dgv.Location = new System.Drawing.Point(37, 48);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 23;
            this.dgv.Size = new System.Drawing.Size(689, 428);
            this.dgv.TabIndex = 2;
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
            this.flowLayoutPanel1.PerformLayout();
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
    }
}