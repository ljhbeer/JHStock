using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Tools;
using JHStock.Update;
using System.Xml;

namespace JHStock
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			_stocks = null;
			_jscfg = null;
			items = new List<string>();
			itemsShow = new List<string>();
			columntitles = new List<string>();
			DateTime dt = DateTime.Today;

            textBox_backendday.Text = ToIntDate(dt).ToString();
            textBox_backbeginday.Text = ToIntDate(dt.AddYears(-1)).ToString();
            textBox_backgreendays.Text = "20";
            textBox_backnowdays.Text = "1";
		}
        private int ToIntDate(DateTime dt)
        {
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }
		private void Form1_Load(object sender, EventArgs e)
		{
			InitDBAndStocks();
            buttonMA.PerformClick();
		}
		private void buttonImportDB_Click(object sender, EventArgs e)
		{
			InitDBAndStocks();
		}
		private void InitDBAndStocks(){
			string filename = textBoxMdbPath.Text;
			if(!( filename != "" && File.Exists(filename))){				
				MessageBox.Show("配置文件不存在，请检查后重置");
				return;
			}
			
			JSConfig jscfg = new JSConfig();
			jscfg.Load(filename);
			BaseConfig cfg = jscfg.baseconfig;
			MFile.cfg = cfg;
			if(!  cfg.CheckWorkPath())
				MessageBox.Show("工作目录不存在，请手工创建"+cfg.NowWorkPath());
			
			if (!jscfg.globalconfig.InitStocks( ))
            {
                MessageBox.Show(_jscfg.globalconfig.ErrMsg); //退出
                MessageBox.Show("配置文件不正确，请检查后重置");
                return;
            }
			this._jscfg = jscfg;
			this._stocks = jscfg.globalconfig.Stocks;
			items.AddRange(
				_stocks.stocks.Select(
					s=>s.Code + "-" + s.Name + "(" + s.PYCode + ")" ).ToArray() );
			List<string> names = DataTableTools.ReadTableNames(jscfg.globalconfig.db);
			if(names.Count>0){
				comboBoxCol.Items.AddRange(names.ToArray());
				comboBoxCol.SelectedIndex = names.FindIndex( r=>r.ToLower() == "stockcode");
			}

            string importtext = "600221";
            if (File.Exists("select.txt"))
                importtext = File.ReadAllText("select.txt").Trim();
            List<string> find = items.FindAll(s => importtext.Contains(s.Substring(2,6)) );  //s.Contains(importtext.ToUpper())
            if (find.Count > 0)
            {
                listBox1.Items.Clear();
                listBox1.Items.AddRange(find.ToArray());
                buttonAddNextlist.PerformClick();
                listBox1.Items.Clear();
            }          
		}		
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			string importtext = textBox1.Text;
			if (!checkBoxIDIndex.Checked)
			{
				List<string> find = items.FindAll(s => s.Contains(importtext.ToUpper()));
				if (find.Count > 0)
				{
					listBox1.Items.Clear();
					listBox1.Items.AddRange(find.ToArray());
				}
			}
			else
			{
				try
				{
					Stock s = _stocks.StockByIndex(Convert.ToInt32(importtext));
					if (s != null)
					{
						listBox1.Items.Clear();
						string str =s.Code + "-" + s.Name + "(" + s.PYCode + ")" + "-" +  s.ID;
						listBox1.Items.Add(str);
					}
				}
				catch (Exception ex)
				{
					textBoxShow.Text = ex.Message;
				}
			}
		}
		private void buttonAddNextlist_Click(object sender, EventArgs e)
		{
			bool change = false;
			foreach (object o in listBox1.Items)
			{
				string s = (string)o;
				if (!itemsShow.Contains(s))
				{
					itemsShow.Add(s);
					change = true;
				}
			}
			if (change)
			{
				listBox2.Items.Clear();
				listBox2.Items.AddRange(itemsShow.ToArray());
			}
		}
		private void buttonClearNextList_Click(object sender, EventArgs e)
		{
			if (itemsShow.Count > 0)
			{
				itemsShow.Clear();
				listBox2.Items.Clear();
			}
		}		
		private void listBox1_KeyUp(object sender, KeyEventArgs e)
		{
			if (listBox1.SelectedIndex == -1) return;
			if (e.KeyCode == Keys.D)
			{
				listBox1.Items.RemoveAt(listBox1.SelectedIndex);
			}
		}
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListBox lc = (ListBox)sender;
			if (lc.SelectedIndex == -1) return;
			string sqltemplate = "select * from [tablename] where [id] = [-id-] ";
			string numcode = lc.SelectedItem.ToString().Substring(2, 6);
			int index = _stocks.StockByNumCode(numcode).ID;
			string sql = sqltemplate.Replace("[tablename]", "["+comboBoxCol.SelectedItem.ToString()+"]")
				.Replace("[-id-]", index.ToString());
			if ("|bouns|Fin|NewFin|sinabouns|".Contains("|" + comboBoxCol.SelectedItem.ToString() + "|"))
				sql = sql.Replace("[id]", "stockid");
			dgv.DataSource =_stocks.Gcfg.db.query(sql).Tables[0];
		}		
		private void buttonMA_Click(object sender, EventArgs e)
		{
            //配置相关参数
            this.Hide();
		}
		private void LoadCfg()
		{
			string filename = textBoxMdbPath.Text;
			if (File.Exists(filename))
			{
                _jscfg.Load(filename);
				
			}
		}
		private List<Stock> StocksByItemsShow()
		{
			List<Stock> ls = new List<Stock>();
			foreach (string str in itemsShow)
			{
				Stock s = _stocks.StockByNumCode(str.Substring(2, 6));/////
				ls.Add(s);
			}
			return ls;
		}

		private List<string> items;
		private List<string> itemsShow;
		private JHStock.JSConfig _jscfg;
		private JHStock.Stocks _stocks;
		private List<string> columntitles { get; set; }
	}	
	public class DTNameType{
		public string Name;
		public Type type;
	}    
}
