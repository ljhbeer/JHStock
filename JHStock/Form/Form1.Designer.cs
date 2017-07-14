namespace JHStock
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonImportDB = new System.Windows.Forms.Button();
            this.textBoxMdbPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonCheckData = new System.Windows.Forms.Button();
            this.checkBoxBeforeDate = new System.Windows.Forms.CheckBox();
            this.buttonMA = new System.Windows.Forms.Button();
            this.textBoxShow = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.buttonClearNextList = new System.Windows.Forms.Button();
            this.checkBoxIDIndex = new System.Windows.Forms.CheckBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.comboBoxCol = new System.Windows.Forms.ComboBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonAddNextlist = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBoxInfor = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_backnowdays = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_backgreendays = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_backendday = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_backbeginday = new System.Windows.Forms.TextBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonImportDB
            // 
            this.buttonImportDB.Location = new System.Drawing.Point(188, 3);
            this.buttonImportDB.Name = "buttonImportDB";
            this.buttonImportDB.Size = new System.Drawing.Size(73, 21);
            this.buttonImportDB.TabIndex = 0;
            this.buttonImportDB.Text = "载入数据库";
            this.buttonImportDB.UseVisualStyleBackColor = true;
            this.buttonImportDB.Click += new System.EventHandler(this.buttonImportDB_Click);
            // 
            // textBoxMdbPath
            // 
            this.textBoxMdbPath.Location = new System.Drawing.Point(54, 3);
            this.textBoxMdbPath.Name = "textBoxMdbPath";
            this.textBoxMdbPath.Size = new System.Drawing.Size(128, 21);
            this.textBoxMdbPath.TabIndex = 1;
            this.textBoxMdbPath.Text = "jsconfig.ini";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "配置";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonCheckData);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxBeforeDate);
            this.splitContainer1.Panel1.Controls.Add(this.buttonMA);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxShow);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxMdbPath);
            this.splitContainer1.Panel1.Controls.Add(this.buttonImportDB);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(730, 564);
            this.splitContainer1.SplitterDistance = 29;
            this.splitContainer1.TabIndex = 3;
            // 
            // buttonCheckData
            // 
            this.buttonCheckData.Location = new System.Drawing.Point(450, 3);
            this.buttonCheckData.Name = "buttonCheckData";
            this.buttonCheckData.Size = new System.Drawing.Size(97, 21);
            this.buttonCheckData.TabIndex = 29;
            this.buttonCheckData.Text = "检测并更新数据";
            this.buttonCheckData.UseVisualStyleBackColor = true;
            this.buttonCheckData.Click += new System.EventHandler(this.ButtonCheckDataClick);
            // 
            // checkBoxBeforeDate
            // 
            this.checkBoxBeforeDate.AutoSize = true;
            this.checkBoxBeforeDate.Location = new System.Drawing.Point(553, 6);
            this.checkBoxBeforeDate.Name = "checkBoxBeforeDate";
            this.checkBoxBeforeDate.Size = new System.Drawing.Size(84, 16);
            this.checkBoxBeforeDate.TabIndex = 28;
            this.checkBoxBeforeDate.Text = "回测前一天";
            this.checkBoxBeforeDate.UseVisualStyleBackColor = true;
            // 
            // buttonMA
            // 
            this.buttonMA.Location = new System.Drawing.Point(267, 4);
            this.buttonMA.Name = "buttonMA";
            this.buttonMA.Size = new System.Drawing.Size(39, 19);
            this.buttonMA.TabIndex = 27;
            this.buttonMA.Text = "MA-M";
            this.buttonMA.UseVisualStyleBackColor = true;
            this.buttonMA.Click += new System.EventHandler(this.buttonMA_Click);
            // 
            // textBoxShow
            // 
            this.textBoxShow.Location = new System.Drawing.Point(645, 4);
            this.textBoxShow.Name = "textBoxShow";
            this.textBoxShow.ReadOnly = true;
            this.textBoxShow.Size = new System.Drawing.Size(74, 21);
            this.textBoxShow.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.buttonClearNextList);
            this.splitContainer2.Panel1.Controls.Add(this.checkBoxIDIndex);
            this.splitContainer2.Panel1.Controls.Add(this.listBox2);
            this.splitContainer2.Panel1.Controls.Add(this.comboBoxCol);
            this.splitContainer2.Panel1.Controls.Add(this.buttonApply);
            this.splitContainer2.Panel1.Controls.Add(this.buttonAddNextlist);
            this.splitContainer2.Panel1.Controls.Add(this.listBox1);
            this.splitContainer2.Panel1.Controls.Add(this.textBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBoxInfor);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel2.Controls.Add(this.dgv);
            this.splitContainer2.Size = new System.Drawing.Size(730, 531);
            this.splitContainer2.SplitterDistance = 197;
            this.splitContainer2.TabIndex = 0;
            // 
            // buttonClearNextList
            // 
            this.buttonClearNextList.Location = new System.Drawing.Point(4, 312);
            this.buttonClearNextList.Name = "buttonClearNextList";
            this.buttonClearNextList.Size = new System.Drawing.Size(92, 22);
            this.buttonClearNextList.TabIndex = 24;
            this.buttonClearNextList.Text = "ClearNextList";
            this.buttonClearNextList.UseVisualStyleBackColor = true;
            this.buttonClearNextList.Click += new System.EventHandler(this.buttonClearNextList_Click);
            // 
            // checkBoxIDIndex
            // 
            this.checkBoxIDIndex.AutoSize = true;
            this.checkBoxIDIndex.Location = new System.Drawing.Point(110, 298);
            this.checkBoxIDIndex.Name = "checkBoxIDIndex";
            this.checkBoxIDIndex.Size = new System.Drawing.Size(84, 16);
            this.checkBoxIDIndex.TabIndex = 23;
            this.checkBoxIDIndex.Text = "输入为ID号";
            this.checkBoxIDIndex.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.BackColor = System.Drawing.SystemColors.ControlText;
            this.listBox2.ForeColor = System.Drawing.SystemColors.Window;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(0, 332);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(197, 196);
            this.listBox2.TabIndex = 19;
            // 
            // comboBoxCol
            // 
            this.comboBoxCol.FormattingEnabled = true;
            this.comboBoxCol.Location = new System.Drawing.Point(105, 272);
            this.comboBoxCol.Name = "comboBoxCol";
            this.comboBoxCol.Size = new System.Drawing.Size(89, 20);
            this.comboBoxCol.TabIndex = 16;
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(4, 266);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(92, 22);
            this.buttonApply.TabIndex = 22;
            this.buttonApply.Text = "Apply=None";
            this.buttonApply.UseVisualStyleBackColor = true;
            // 
            // buttonAddNextlist
            // 
            this.buttonAddNextlist.Location = new System.Drawing.Point(4, 289);
            this.buttonAddNextlist.Name = "buttonAddNextlist";
            this.buttonAddNextlist.Size = new System.Drawing.Size(92, 22);
            this.buttonAddNextlist.TabIndex = 18;
            this.buttonAddNextlist.Text = "AddNextList";
            this.buttonAddNextlist.UseVisualStyleBackColor = true;
            this.buttonAddNextlist.Click += new System.EventHandler(this.buttonAddNextlist_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.SystemColors.ControlText;
            this.listBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(4, 22);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(191, 244);
            this.listBox1.TabIndex = 17;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyUp);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlText;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(3, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(194, 21);
            this.textBox1.TabIndex = 15;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBoxInfor
            // 
            this.textBoxInfor.Location = new System.Drawing.Point(352, 7);
            this.textBoxInfor.Name = "textBoxInfor";
            this.textBoxInfor.Size = new System.Drawing.Size(174, 21);
            this.textBoxInfor.TabIndex = 30;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_backnowdays);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox_backgreendays);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_backendday);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_backbeginday);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 59);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "回测";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(189, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 36;
            this.label4.Text = "金叉天数";
            // 
            // textBox_backnowdays
            // 
            this.textBox_backnowdays.Location = new System.Drawing.Point(242, 33);
            this.textBox_backnowdays.Name = "textBox_backnowdays";
            this.textBox_backnowdays.Size = new System.Drawing.Size(64, 21);
            this.textBox_backnowdays.TabIndex = 35;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(189, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 34;
            this.label5.Text = "绿线天数";
            // 
            // textBox_backgreendays
            // 
            this.textBox_backgreendays.Location = new System.Drawing.Point(242, 10);
            this.textBox_backgreendays.Name = "textBox_backgreendays";
            this.textBox_backgreendays.Size = new System.Drawing.Size(64, 21);
            this.textBox_backgreendays.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 32;
            this.label3.Text = "结束时间";
            // 
            // textBox_backendday
            // 
            this.textBox_backendday.Location = new System.Drawing.Point(123, 32);
            this.textBox_backendday.Name = "textBox_backendday";
            this.textBox_backendday.Size = new System.Drawing.Size(64, 21);
            this.textBox_backendday.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 30;
            this.label2.Text = "起始时间";
            // 
            // textBox_backbeginday
            // 
            this.textBox_backbeginday.Location = new System.Drawing.Point(123, 9);
            this.textBox_backbeginday.Name = "textBox_backbeginday";
            this.textBox_backbeginday.Size = new System.Drawing.Size(64, 21);
            this.textBox_backbeginday.TabIndex = 29;
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(0, 68);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 23;
            this.dgv.Size = new System.Drawing.Size(529, 463);
            this.dgv.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 564);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TextBox textBoxInfor;
        private System.Windows.Forms.Button buttonCheckData;

        #endregion

        private System.Windows.Forms.Button buttonImportDB;
        private System.Windows.Forms.TextBox textBoxMdbPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ComboBox comboBoxCol;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonAddNextlist;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.CheckBox checkBoxIDIndex;
        private System.Windows.Forms.TextBox textBoxShow;
        private System.Windows.Forms.Button buttonMA;
        private System.Windows.Forms.CheckBox checkBoxBeforeDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_backnowdays;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_backgreendays;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_backendday;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_backbeginday;
        private System.Windows.Forms.Button buttonClearNextList;
    }
}

