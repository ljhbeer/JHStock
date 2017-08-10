namespace JHStock
{
    partial class FormCustomLog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabwrite = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabbrows = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxtitle = new System.Windows.Forms.TextBox();
            this.buttonSaveLog = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxColorstylePrint = new System.Windows.Forms.CheckBox();
            this.checkBoxImgreDraw = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabwrite.SuspendLayout();
            this.tabbrows.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.74074F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.25926F));
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.997211F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.00279F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1062, 717);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabwrite);
            this.tabControl1.Controls.Add(this.tabbrows);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(170, 46);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(889, 668);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabwrite
            // 
            this.tabwrite.Controls.Add(this.textBox1);
            this.tabwrite.Location = new System.Drawing.Point(4, 21);
            this.tabwrite.Name = "tabwrite";
            this.tabwrite.Padding = new System.Windows.Forms.Padding(3);
            this.tabwrite.Size = new System.Drawing.Size(881, 643);
            this.tabwrite.TabIndex = 0;
            this.tabwrite.Text = "编辑";
            this.tabwrite.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(875, 637);
            this.textBox1.TabIndex = 0;
            // 
            // tabbrows
            // 
            this.tabbrows.Controls.Add(this.richTextBox1);
            this.tabbrows.Location = new System.Drawing.Point(4, 21);
            this.tabbrows.Name = "tabbrows";
            this.tabbrows.Padding = new System.Windows.Forms.Padding(3);
            this.tabbrows.Size = new System.Drawing.Size(881, 643);
            this.tabbrows.TabIndex = 1;
            this.tabbrows.Text = "浏览";
            this.tabbrows.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(875, 637);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonSave);
            this.panel1.Controls.Add(this.textBoxtitle);
            this.panel1.Controls.Add(this.buttonSaveLog);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(170, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(889, 37);
            this.panel1.TabIndex = 5;
            // 
            // buttonSave
            // 
            this.buttonSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonSave.Location = new System.Drawing.Point(92, 0);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(40, 37);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "保存";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxtitle
            // 
            this.textBoxtitle.Location = new System.Drawing.Point(138, 9);
            this.textBoxtitle.Name = "textBoxtitle";
            this.textBoxtitle.Size = new System.Drawing.Size(694, 21);
            this.textBoxtitle.TabIndex = 5;
            this.textBoxtitle.Text = "请输入标题";
            // 
            // buttonSaveLog
            // 
            this.buttonSaveLog.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonSaveLog.Location = new System.Drawing.Point(0, 0);
            this.buttonSaveLog.Name = "buttonSaveLog";
            this.buttonSaveLog.Size = new System.Drawing.Size(92, 37);
            this.buttonSaveLog.TabIndex = 4;
            this.buttonSaveLog.Text = "保存当前日志";
            this.buttonSaveLog.UseVisualStyleBackColor = true;
            this.buttonSaveLog.Click += new System.EventHandler(this.buttonSaveLog_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.checkBoxColorstylePrint);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxImgreDraw);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(161, 37);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // checkBoxColorstylePrint
            // 
            this.checkBoxColorstylePrint.AutoSize = true;
            this.checkBoxColorstylePrint.Location = new System.Drawing.Point(3, 3);
            this.checkBoxColorstylePrint.Name = "checkBoxColorstylePrint";
            this.checkBoxColorstylePrint.Size = new System.Drawing.Size(72, 16);
            this.checkBoxColorstylePrint.TabIndex = 6;
            this.checkBoxColorstylePrint.Text = "打印模式";
            this.checkBoxColorstylePrint.UseVisualStyleBackColor = true;
            // 
            // checkBoxImgreDraw
            // 
            this.checkBoxImgreDraw.AutoSize = true;
            this.checkBoxImgreDraw.Location = new System.Drawing.Point(81, 3);
            this.checkBoxImgreDraw.Name = "checkBoxImgreDraw";
            this.checkBoxImgreDraw.Size = new System.Drawing.Size(72, 16);
            this.checkBoxImgreDraw.TabIndex = 7;
            this.checkBoxImgreDraw.Text = "强制更新";
            this.checkBoxImgreDraw.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 46);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(161, 668);
            this.splitContainer1.SplitterDistance = 507;
            this.splitContainer1.TabIndex = 8;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.Black;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(161, 507);
            this.treeView1.TabIndex = 2;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(161, 157);
            this.listBox1.TabIndex = 7;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // FormCustomLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 717);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormCustomLog";
            this.Text = "FormCustomLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCustomLog_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabwrite.ResumeLayout(false);
            this.tabwrite.PerformLayout();
            this.tabbrows.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabwrite;
        private System.Windows.Forms.TabPage tabbrows;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSaveLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxtitle;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox checkBoxColorstylePrint;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxImgreDraw;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}