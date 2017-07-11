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
namespace JHStock
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			dtsave = null;
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
			itemsShow.Add("SH600221-海南航空(HNHK)");
			listBox2.Items.Clear();
			listBox2.Items.AddRange(itemsShow.ToArray());
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
		private void ButtonCheckDataClick(object sender, EventArgs e)
		{
			completebtn = (Button)sender;
			completebtn.Enabled = false;
			if(!isrunning){
				isrunning = false;
				NetKData nkd = new NetKData(_jscfg);
                nkd.ThreadShowMsg = ThreadShowMsg;
				System.Threading.Thread nonParameterThread = 
					new Thread( new ThreadStart( nkd.GetNetKData ));
				nonParameterThread.Start();
			}
		} ///??
		

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

		private void buttonMACD_Click(object sender, EventArgs e)
		{
			String Msg = "";
			((Button)sender).Enabled = false;
			_stocks.ReloadListDate();
			if (exchangestatus.StatusCheck(_stocks, ref Msg))
				ShowFormMonit(exchangestatus, "MACD");
			else
				MessageBox.Show(Msg);
			((Button)sender).Enabled = true;
		}
		private void buttonMA_Click(object sender, EventArgs e)
		{
			String Msg = "";
			((Button)sender).Enabled = false;
			_stocks.ReloadListDate();
			if (exchangestatus.StatusCheck(_stocks, ref Msg))
				ShowFormMonit(exchangestatus, "MA");
			else
				MessageBox.Show(Msg);
			((Button)sender).Enabled = true;
		}
		private void buttonCreateMacd_Click(object sender, EventArgs e)
		{
//			((Button)sender).Enabled = false;
//			MACD macd = new MACD();
//			if (checkBoxBeforeDate.Checked)
//			{
//				int backdate = Config.ToIntDate(DateTime.Today.AddDays(-1));
//				int datepos = _stocks.GetStartDatePos(backdate);
//				
//				CreateAllStockMACDData(cfg, _stocks, cfg.StaticDays,datepos );
//			}
//			else
//			{
//				CreateAllStockMACDData(cfg, _stocks, cfg.StaticDays);
//			}
//			_stocks.ResetMacdData();
//			((Button)sender).Enabled = true;
		}
		private void BTN_BACKMACDClick(object sender, EventArgs e)
		{
//			int begindate = Convert.ToInt32( textBox_backbeginday.Text);
//			int enddate = Convert.ToInt32(textBox_backendday.Text);
//			int greendays = Convert.ToInt32(textBox_backgreendays.Text);
//			int nowdays = Convert.ToInt32(textBox_backnowdays.Text);
//			string type = "MACD";
//			if (itemsShow.Count > 0)
//			{
//				FormMonitBack f = new FormMonitBack(_stocks, itemsShow, exchangestatus, type, begindate, enddate, greendays, nowdays);
//				f.ShowDialog();
//			}
//			else
//			{
//				MessageBox.Show("没有添加回测股票");
//			}
		}

		public void OutThreadMsg()
		{
			GlobalConfig cfg = _jscfg.globalconfig;
			string msg = "本次更新开始于" + updatetime.ToLongTimeString() + "结束于" + DateTime.Now.ToLongTimeString() + " 共耗时 " + DateTime.Now.Subtract(updatetime).TotalSeconds + " 秒，共更新了" + "条记录\r\n\r\n";
			if (cfg.ErrMsg != "")
				ErrorMsg += cfg.ErrMsg;
			if (ErrorMsg == "")
				ThreadShowMsg("已全部完成" + msg);
			else
				ThreadShowMsg("ErrorMsg:" + ErrorMsg + " Msg:" + msg);
			MFile.AppendAllText("update.log", msg.Trim() + "ErrorMsg:" + ErrorMsg + "\r\n\r\n");
		}
		public void CompleteRun(){
			;
		}
		public void showfiletxt(string file)
		{
			this.textBoxInfor.Text = file;
		}
		public void showappendfiletxt(string file)
		{
			this.textBoxInfor.Text += file;
		}
		
		private void ShowFormMonit(ExChangeStatusCheck exchangestatus, string type)
		{
//			this.Hide();
//			if (f == null)
//				f = new FormMonit(_stocks, exchangestatus, type);
//			f.ShowDialog();
//			f = null;
//			this.Show();
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
		private void ThreadShowMsg(string msg)
		{
			this.Invoke(new ShowDeleGate(showfiletxt), new object[] { msg });
		}
		private void ThreadAppendMsg(string msg)
		{
			Invoke(new ShowDeleGate(showappendfiletxt), new object[] { msg });
		}
		private void ThreadCompleteRun(){
			Invoke(new CompleteDeleGate(CompleteRun));
		}
		
		
		private Button completebtn;
		private Boolean isrunning;
		private string ErrorMsg;
		private bool bshowtime;
		private DateTime updatetime;
		
		private List<string> items;
		private List<string> itemsShow;
		private DataTable dtsave;
		private JHStock.JSConfig _jscfg;
		private JHStock.Stocks _stocks;
		private List<string> columntitles { get; set; }
		private  ExChangeStatusCheck exchangestatus = new ExChangeStatusCheck();
		public new tagstock[]  Tag;
	}
	
	public class DTNameType{
		public string Name;
		public Type type;
	}
}
