namespace JHStock.UserForm
{
    partial class FormShow
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
            this.m_tab = new System.Windows.Forms.TabControl();
            this.Local = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonChangeGapChar = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dgv2 = new System.Windows.Forms.DataGridView();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.dgv3 = new System.Windows.Forms.DataGridView();
            this.dgv4 = new System.Windows.Forms.DataGridView();
            this.Net = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.m_tab.SuspendLayout();
            this.Local.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv4)).BeginInit();
            this.Net.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_tab
            // 
            this.m_tab.Controls.Add(this.Local);
            this.m_tab.Controls.Add(this.Net);
            this.m_tab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_tab.Location = new System.Drawing.Point(0, 0);
            this.m_tab.Name = "m_tab";
            this.m_tab.SelectedIndex = 0;
            this.m_tab.Size = new System.Drawing.Size(860, 636);
            this.m_tab.TabIndex = 0;
            // 
            // Local
            // 
            this.Local.Controls.Add(this.tableLayoutPanel1);
            this.Local.Location = new System.Drawing.Point(4, 22);
            this.Local.Name = "Local";
            this.Local.Padding = new System.Windows.Forms.Padding(3);
            this.Local.Size = new System.Drawing.Size(852, 610);
            this.Local.TabIndex = 0;
            this.Local.Text = "Local";
            this.Local.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.318083F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.68192F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(846, 604);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonSave);
            this.flowLayoutPanel1.Controls.Add(this.buttonChangeGapChar);
            this.flowLayoutPanel1.Controls.Add(this.textBox1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(840, 30);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(3, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(81, 20);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "保存";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Visible = false;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
            // 
            // buttonChangeGapChar
            // 
            this.buttonChangeGapChar.Location = new System.Drawing.Point(90, 3);
            this.buttonChangeGapChar.Name = "buttonChangeGapChar";
            this.buttonChangeGapChar.Size = new System.Drawing.Size(81, 20);
            this.buttonChangeGapChar.TabIndex = 2;
            this.buttonChangeGapChar.Text = "逗号改Tab";
            this.buttonChangeGapChar.UseVisualStyleBackColor = true;
            this.buttonChangeGapChar.Visible = false;
            this.buttonChangeGapChar.Click += new System.EventHandler(this.ButtonChangeGapCharClick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(177, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(499, 21);
            this.textBox1.TabIndex = 1;
            this.textBox1.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.dgv2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.dgv1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.dgv3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.dgv4, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 39);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(840, 541);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // dgv2
            // 
            this.dgv2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv2.Location = new System.Drawing.Point(3, 273);
            this.dgv2.Name = "dgv2";
            this.dgv2.RowHeadersVisible = false;
            this.dgv2.RowTemplate.Height = 23;
            this.dgv2.Size = new System.Drawing.Size(414, 265);
            this.dgv2.TabIndex = 4;
            // 
            // dgv1
            // 
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv1.Location = new System.Drawing.Point(3, 3);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersVisible = false;
            this.dgv1.RowTemplate.Height = 23;
            this.dgv1.Size = new System.Drawing.Size(414, 264);
            this.dgv1.TabIndex = 1;
            // 
            // dgv3
            // 
            this.dgv3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv3.Location = new System.Drawing.Point(423, 3);
            this.dgv3.Name = "dgv3";
            this.dgv3.RowHeadersVisible = false;
            this.dgv3.RowTemplate.Height = 23;
            this.dgv3.Size = new System.Drawing.Size(414, 264);
            this.dgv3.TabIndex = 4;
            // 
            // dgv4
            // 
            this.dgv4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv4.Location = new System.Drawing.Point(423, 273);
            this.dgv4.Name = "dgv4";
            this.dgv4.RowHeadersVisible = false;
            this.dgv4.RowTemplate.Height = 23;
            this.dgv4.Size = new System.Drawing.Size(414, 265);
            this.dgv4.TabIndex = 4;
            // 
            // Net
            // 
            this.Net.Controls.Add(this.webBrowser1);
            this.Net.Location = new System.Drawing.Point(4, 22);
            this.Net.Name = "Net";
            this.Net.Padding = new System.Windows.Forms.Padding(3);
            this.Net.Size = new System.Drawing.Size(852, 610);
            this.Net.TabIndex = 1;
            this.Net.Text = "Net";
            this.Net.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(3, 3);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(846, 605);
            this.webBrowser1.TabIndex = 0;
            // 
            // FormShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 636);
            this.Controls.Add(this.m_tab);
            this.Name = "FormShow";
            this.Text = "FormShow";
            this.m_tab.ResumeLayout(false);
            this.Local.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv4)).EndInit();
            this.Net.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.DataGridView dgv4;
        private System.Windows.Forms.DataGridView dgv3;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.DataGridView dgv2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonChangeGapChar;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TabPage Net;
        private System.Windows.Forms.TabPage Local;
        private System.Windows.Forms.TabControl m_tab;

        #endregion


    }
}