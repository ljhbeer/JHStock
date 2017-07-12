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
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddToTXDBlock = new System.Windows.Forms.Button();
            this.buttonToTxt = new System.Windows.Forms.Button();
            this.checkBoxTable = new System.Windows.Forms.CheckBox();
            this.buttonReCompute = new System.Windows.Forms.Button();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 95.0376F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.textBox2, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBox3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.dgv, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(671, 473);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(37, 450);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(611, 21);
            this.textBox2.TabIndex = 0;
            // 
            // textBox3
            // 
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Location = new System.Drawing.Point(3, 450);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(28, 21);
            this.textBox3.TabIndex = 1;
            this.textBox3.Text = "状态";
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(37, 29);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 23;
            this.dgv.Size = new System.Drawing.Size(611, 415);
            this.dgv.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonAddToTXDBlock);
            this.flowLayoutPanel1.Controls.Add(this.buttonToTxt);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxTable);
            this.flowLayoutPanel1.Controls.Add(this.buttonReCompute);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(37, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(611, 20);
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
            this.buttonReCompute.Size = new System.Drawing.Size(64, 18);
            this.buttonReCompute.TabIndex = 23;
            this.buttonReCompute.Text = "计算";
            this.buttonReCompute.UseVisualStyleBackColor = true;
            this.buttonReCompute.Click += new System.EventHandler(this.buttonReCompute_Click);
            // 
            // FormMonit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 473);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "FormMonit";
            this.Text = "FormMonit";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonAddToTXDBlock;
        private System.Windows.Forms.Button buttonToTxt;
        private System.Windows.Forms.CheckBox checkBoxTable;
        private System.Windows.Forms.Button buttonReCompute;
    }
}