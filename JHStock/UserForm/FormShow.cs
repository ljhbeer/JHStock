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
using System.Reflection;

namespace JHStock.UserForm
{
	public partial class FormShow : Form
	{
		private Stock _s;
		private JSConfig _jscfg;
		private string _qqfinpath;
        private static ChineseName _schinesename;
		public FormShow(Stock s, JSConfig _jscfg)
		{
			InitializeComponent();
			this._s = s;
			this._jscfg = _jscfg;
            _schinesename = new ChineseName();
			_qqfinpath = _jscfg.baseconfig.WorkPath +"Data\\QQFin\\";
            InitDataTable();
			Init();
		}
		private void Init(){
            string urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/Index?type=web&code=[stockcode]#zyzb-0";
            try
            {
                //if (_jscfg.globalconfig.StocksData.Netdate.Inline)
                //{
                //    m_tab.TabPages[1].Show();
                //    string url = urlt.Replace("[stockcode]", _s.Code.ToLower());
                //    webBrowser1.Navigate(url);
                //}
                //else
                {
                    m_tab.TabPages[1].Visible = false;
                }
                InitLocalData();
            }
            catch
            {
            }
		}
        private void InitDataTable()
        {
            List<string> NameList = JsonMainCWFX.GetNameList();
            dgv2.DataSource = null;
            DataTable dt = new DataTable("财务报表");
            foreach (string Name in NameList)
            {
                DataColumn dc = new DataColumn(_schinesename.GetChineseName(Name));
                dt.Columns.Add(dc);
            }
            dgv1.DataSource = dt;
            foreach (string Name in NameList)
            {
                dgv1.Columns[_schinesename.GetChineseName(Name)].DataPropertyName = Name;
            }

        }        
		void InitLocalData()
		{
            Tagstock t = _jscfg.globalconfig.Stocks.GetTagstock(_s.ID);
                if (t != null && t.Tag!=null)
                {
                    List<JsonMainCWFX> ls = JsonConvert.DeserializeObject<List<JsonMainCWFX>>(t.Tag.ToString());
                   
                    dgv1.DataSource = ls;                   
                }             
		}

        private void  InitDgvColumnName(DataGridView dgv)
        {                  
            foreach (DataGridViewColumn dc in dgv.Columns)
            {
                string chinesename = _schinesename.GetChineseName(dc.Name);
                if (chinesename != "未知")
                    dc.DataPropertyName = chinesename;
            }
           
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
