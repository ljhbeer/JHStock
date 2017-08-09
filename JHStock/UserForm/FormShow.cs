using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tools;
using System.Text.RegularExpressions;
using JHStock.Update;
using Newtonsoft.Json;
using System.IO;

namespace JHStock.UserForm
{
	public partial class FormShow : Form
	{
		private Stock _s;
		private JSConfig _jscfg;
		private string _qqfinpath;
		public FormShow(Stock s, JSConfig _jscfg)
		{
			InitializeComponent();
			this._s = s;
			this._jscfg = _jscfg;
			_qqfinpath = _jscfg.baseconfig.WorkPath +"Data\\QQFin\\";
			Init();
		}
		private void Init(){
			string urlt = @"http://stock.finance.qq.com/corp1/cwfx.html?mgzb-[stockcode]";
			if(_jscfg.globalconfig.StocksData.Netdate.Inline){
				m_tab.TabPages[1].Show();
				string url = urlt.Replace("[stockcode]",_s.Code.ToLower());
				webBrowser1.Navigate(url );
			}else{
				m_tab.TabPages[1].Visible = false;		
			}
			InitLocalData();			
		}

		void InitLocalData()
		{
			QQfinItem qf = new QQfinItem();
			qf.LoadData(_qqfinpath + _s.Code + ".txt");
			dgv1.DataSource = JsonToDataTable(JsonConvert.SerializeObject(qf.CZNL).Replace("00:00:00", ""));
			dgv2.DataSource = JsonToDataTable(JsonConvert.SerializeObject(qf.DJCW).Replace("00:00:00", ""));
			dgv3.DataSource = JsonToDataTable(JsonConvert.SerializeObject(qf.YLNL).Replace("00:00:00", ""));
			dgv4.DataSource = JsonToDataTable(JsonConvert.SerializeObject(qf.YYNL).Replace("00:00:00", ""));

			string txt = File.ReadAllText("E:\\Project\\Source\\Stock\\Data\\QQFinConfig.txt");
			Component.QQFinConfig.QQfinConfig fc = new Component.QQFinConfig.QQfinConfig();
			foreach (DataColumn dc in ((DataTable)(dgv1.DataSource)).Columns) {
				string n = fc.fincfg["cznl"].col[dc.ColumnName][0];
				if (n.Length >= 8)
					n = n.Substring(0, 8);
				dc.ColumnName = n;
			}
			foreach (DataColumn dc in ((DataTable)(dgv2.DataSource)).Columns) {
				string n = fc.fincfg["djcw"].col[dc.ColumnName][0];
				if (n.Length >= 8)
					n = n.Substring(0, 8);
				dc.ColumnName = n;
			}
			foreach (DataColumn dc in ((DataTable)(dgv3.DataSource)).Columns) {
				string n = fc.fincfg["ylnl"].col[dc.ColumnName][0];
				if (n.Length >= 8)
					n = n.Substring(0, 8);
				dc.ColumnName = n;
			}
			foreach (DataColumn dc in ((DataTable)(dgv4.DataSource)).Columns) {
				string n = fc.fincfg["yynl"].col[dc.ColumnName][0];
				if (n.Length >= 8)
					n = n.Substring(0, 8);
				dc.ColumnName = n;
			}
			List<DataGridView> dgvs = new List<DataGridView> {
				dgv1,
				dgv2,
				dgv3,
				dgv4
			};
			foreach (DataGridView dgv in dgvs)
				foreach (DataGridViewColumn dcc in dgv.Columns)
					dcc.Width = 65;
		}
		private void Initmytest()
		{
			string urlt = "http://quote.eastmoney.com/[scode].html";
			string url = urlt.Replace("[scode]", _s.Code);
			string html = CWeb.GetWebClient(url);
			string pattern = @"(?<=公司核心数据[^01]*)<div class=\""box-x1 mb10\"">[\S\s]*(?=<div class=\""title1 nonal ie rtab\"">)";
			//html = html.
			html = Regex.Match(html, pattern).Value;
			webBrowser1.DocumentText = "<H>" + _s.Name + _s.Code + "</H>" + html;
		}
		private DataTable JsonToDataTable(string strJson)
		{
			//转换json格式
			strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
			//取出表名
			var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
			string strName = rg.Match(strJson).Value;
			DataTable tb = null;
			//去除表名
			strJson = strJson.Substring(strJson.IndexOf("[") + 1);
			strJson = strJson.Substring(0, strJson.IndexOf("]"));
			//获取数据
			rg = new Regex(@"(?<={)[^}]+(?=})");
			MatchCollection mc = rg.Matches(strJson);
			for (int i = 0; i < mc.Count; i++)
			{
				string strRow = mc[i].Value;
				string[] strRows = strRow.Split('*');
				//创建表
				if (tb == null)
				{
					tb = new DataTable();
					tb.TableName = strName;
					foreach (string str in strRows)
					{
						var dc = new DataColumn();
						string[] strCell = str.Split('#');
						if (strCell[0].Substring(0, 1) == "\"")
						{
							int a = strCell[0].Length;
							dc.ColumnName = strCell[0].Substring(1, a - 2);
						}
						else
						{
							dc.ColumnName = strCell[0];
						}
						tb.Columns.Add(dc);
					}
					tb.AcceptChanges();
				}
				//增加内容
				DataRow dr = tb.NewRow();
				for (int r = 0; r < strRows.Length; r++)
				{
					dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
				}
				tb.Rows.Add(dr);
				tb.AcceptChanges();
			}
			return tb;
		}
		
		private void ButtonSaveClick(object sender, EventArgs e)
		{
			SaveFile(textBox1.Text);
		}
		private void SaveFile(string str){
			SaveFileDialog fd = new SaveFileDialog();
			fd.Title = "txt文件";
			fd.Filter = "文本文件(*.txt)|*.txt";
			fd.FileName = "文本.txt";
			if (fd.ShowDialog() == DialogResult.OK)
			{
				if (fd.FileName.EndsWith(".txt"))
				{
					File.WriteAllText(fd.FileName,str);
				}
			}
		}
		
		private void ButtonChangeGapCharClick(object sender, EventArgs e)
		{
			textBox1.Text = textBox1.Text.Replace(",","\t");
		}
	}
}
