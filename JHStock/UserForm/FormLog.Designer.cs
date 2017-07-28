/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2017-07-27
 * 时间: 16:23
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
namespace JHStock.UserForm
{
	partial class FormLog
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.dgv = new System.Windows.Forms.DataGridView();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.74074F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.25926F));
			this.tableLayoutPanel1.Controls.Add(this.dgv, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.207343F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.79266F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(864, 463);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// dgv
			// 
			this.dgv.AllowUserToAddRows = false;
			this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgv.Location = new System.Drawing.Point(138, 40);
			this.dgv.Name = "dgv";
			this.dgv.RowHeadersVisible = false;
			this.dgv.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgv.RowTemplate.Height = 23;
			this.dgv.Size = new System.Drawing.Size(723, 420);
			this.dgv.TabIndex = 3;
			this.dgv.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvCellContentClick);
			// 
			// treeView1
			// 
			this.treeView1.BackColor = System.Drawing.Color.Black;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.ForeColor = System.Drawing.Color.White;
			this.treeView1.Location = new System.Drawing.Point(3, 40);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(129, 420);
			this.treeView1.TabIndex = 2;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// FormLog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(864, 463);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "FormLog";
			this.Text = "FormLog";
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.DataGridView dgv;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
