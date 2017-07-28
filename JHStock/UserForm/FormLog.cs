/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2017-07-27
 * 时间: 16:23
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;

namespace JHStock.UserForm
{
	public partial class FormLog : Form
	{
		private JSConfig _jscfg;
		private StocksLog _stockslog;
		public FormLog(JSConfig _jscfg,StocksLog _stockslog)
		{			
			InitializeComponent();
			this._jscfg = _jscfg;
			this._stockslog = _stockslog;
			// TODO: Fromlog
			Init();
			InitMaDataTable();
			_umi = new UpdateMonitInfors();
		}
		private void Init(){
			 _tn = new TreeNode();
             _tn.Text = "入手日期";
             TreeNode[] vt = new TreeNode[2];
             for (int i = 0; i < vt.Count(); i++)
                 vt[i] = new TreeNode();
             vt[0].Name = vt[0].Text = "2016";
             vt[1].Name = vt[1].Text = "2017";
             vt[0].Tag = 2016;
             vt[1].Tag = 2017;
             _tn.Nodes.AddRange(vt);
             InitMonth();
             treeView1.Nodes.Add(_tn);
             treeView1.ExpandAll();
		}
		private void InitMonth()
        {
			List<int> years = _stockslog._dailystocks.Select( r => r.Date.Year).Distinct().ToList();
			List<List<int>> yearmonths = new List<List<int>>();
			foreach(int year in years)
             yearmonths.Add(
					_stockslog._dailystocks.Where( r=> r.Date.Year == year)
			   			.Select( r => r.Date.Month).Distinct().ToList()
			 );
			
			if(years.Count == 0 || yearmonths.Count==0) return;
			foreach (TreeNode n in _tn.Nodes)
            {
				int year = (int)n.Tag;
				int pos = years.IndexOf( year );
				if(pos == -1) continue;
                List<TreeNode> ltn = new List<TreeNode>();
                foreach (int m in yearmonths[pos])
                {
                	TreeNode t = new TreeNode( (year*100+ m).ToString());
                	t.Tag = (year*100+ m);
                	ltn.Add(t);
                }
                n.Nodes.AddRange(ltn.ToArray());
            }
        }		
        
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode t = treeView1.SelectedNode;
            if(t!=null)
            if (t.Text.Length  == 6)
            {
                if (t.Nodes.Count == 0)
                {
                	int yearmonth = (int)t.Tag;
                	int year = yearmonth/100;
                	int month = yearmonth%100;
                	List<DateTime> days = _stockslog._dailystocks.Where( r => (r.Date.Year == year && r.Date.Month == month) )
                		.Select( r => r.Date).Distinct().ToList();
                   
                    List<TreeNode> ltn = new List<TreeNode>();
                    foreach (DateTime  dr in days)
                    {
                    	string s=Regex.Replace(dr.ToShortDateString(),"\\D","");
                    	TreeNode tt = new TreeNode(s);
                    	tt.Tag = dr;
                        ltn.Add(tt);
                    }
                    t.Nodes.AddRange(ltn.ToArray());
                    
                }
            }
            else if (t.Text.Length == 8)
            {
            	DateTime day = (DateTime)t.Tag;
                ShowStockDaily(day);
            }

        } 
		private void ShowStockDaily(DateTime day)
        {
			List<DailyStocks> ds = _stockslog._dailystocks.Where( r => r.Date.ToShortDateString() == day.ToShortDateString()).ToList();
			if(ds.Count>0){
				dt.Rows.Clear();		
				_umi.Clear();
				int i = 0;
				_umi.b5years = false;
				_umi.bshownet = false;				
				foreach (DailyStock d in ds[0]._stocks)
				{
					//TODO: formlog.add  runnet
					DataRow dr = dt.NewRow();
					Stock s = this._jscfg.globalconfig.Stocks.StockByNumCode( d.StockCode.Substring(2,6));
					dr["名称"] = s.Name;
					dr["代码"] =s.Code;
					string[] ss = d.StockTestInfo.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
					if (ss.Length == 3)
					{
						dr["持续天数"] = ss[0];
						dr["后续天数"] = ss[1];
						dr["后续天数的情况"] = ss[2];
					}					
						dr["财务信息"] = d.StockBaseInfor;
					dt.Rows.Add(dr);
					if (_jscfg.globalconfig.StocksData.Netdate.Inline)
					{					
						//UpdataOthers( s, ref dr);
						_umi.Add(s,dr);
					}
				}
				_umi.run("stocklog");
			}
        }  
		private void InitMaDataTable()
		{
			dt = new DataTable();
			List<string> columntitles = new List<string>() { "名称", "代码",  "持续天数" ,"后续天数","后续天数的情况","画图","财务信息" };//   "日期",
			//columntitles = new List<string>() { "名称", "代码","杂项" };//,"杂项"
			for (int count = 0; count < columntitles.Count; count++)
			{
				DataColumn dc = new DataColumn(columntitles[count]);
				if ("代码名称状态杂项日期".Contains(columntitles[count]))
				{
					dc.DataType = typeof(string);
				}
				else if ("上一状态持续天数后续天数".Contains(columntitles[count]))
				{
					dc.DataType = typeof(int);
				}
				else if ("画图分时图".Contains(columntitles[count]))
				{
					dc.DataType = typeof(Image);
				}
				else if ("选择".Contains(columntitles[count]))
				{
					dc.DataType = typeof(Boolean);
				}
				else
				{
					dc.DataType = typeof(string);
					//dc.MaxLength = 30;
				}
				dt.Columns.Add(dc);
			}
			dgv.DataSource = dt; // 宽度应设置在 dgv 上

			for (int i = 0; i < dgv.ColumnCount; i++)
			{
				if ("代码名称状态杂项日期".Contains( dgv.Columns[i].Name))
				{
					dgv.Columns[i].Width = 60;
				}
				else if ("上一状态持续天数后续天数".Contains(dgv.Columns[i].Name))
				{
					dgv.Columns[i].Width = 40;
				}
				else if ("画图分时图".Contains(dgv.Columns[i].Name))
				{
					dgv.Columns[i].Width = 200;
				}
				else if ("财务信息".Contains(dgv.Columns[i].Name))
				{
					dgv.Columns[i].Width = 200;
				}
				else
				{
					dgv.Columns[i].Width = 120;
					//dc.MaxLength = 30;
				}
			}
			dgv.RowTemplate.Height = 100;
		}
		private TreeNode _tn;		
		private DataTable dt;
		public UpdateMonitInfors _umi;
		
		void DgvCellContentClick(object sender, DataGridViewCellEventArgs e)		
		{
			if (e.RowIndex != -1 && e.ColumnIndex != -1)
			{
				string numcode = dgv[1, e.RowIndex].Value.ToString().Substring(2, 6);
				Stock s = _jscfg.globalconfig.Stocks.StockByNumCode(numcode);
				if (s != null && s.Bmp != null)
				{//showImg
					if (e.ColumnIndex == 5)
					{
						FormPictureBox f = new FormPictureBox(s.Bmp);
						f.ShowDialog();
					}					
				}
			}
		}
	}
}
