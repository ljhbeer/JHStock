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
        public Form1(JSConfig _jscfg)
        {
			InitializeComponent();
            this._jscfg = _jscfg;
            _stocks = null;
            //_jscfg = null;
			items = new List<string>();
			itemsShow = new List<string>();
			columntitles = new List<string>();
			DateTime dt = DateTime.Today;

            textBox_backendday.Text = ToIntDate(dt).ToString();
            textBox_backbeginday.Text = ToIntDate(dt.AddYears(-1)).ToString();
            textBox_backgreendays.Text = "20";
            textBox_backnowdays.Text = "1";
            _isrunning = false;
            _ErrorMsg = "";
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
            //string filename = textBoxMdbPath.Text;
            //if(!( filename != "" && File.Exists(filename))){				
            //    MessageBox.Show("配置文件不存在，请检查后重置");
            //    return;
            //}
			
            //JSConfig jscfg = new JSConfig();
            //jscfg.Load(filename);
            //BaseConfig cfg = jscfg.baseconfig;
            //MFile.cfg = cfg;
            //if(!  cfg.CheckWorkPath())
            //    MessageBox.Show("工作目录不存在，请手工创建"+cfg.NowWorkPath());
			
            //if (!jscfg.globalconfig.InitStocks( ))
            //{
            //    MessageBox.Show(_jscfg.globalconfig.ErrMsg); //退出
            //    MessageBox.Show("配置文件不正确，请检查后重置");
            //    return;
            //}
            //this._jscfg = jscfg;
			this._stocks = _jscfg.globalconfig.Stocks;
			items.AddRange(
				_stocks.stocks.Select(
					s=>s.Code + "-" + s.Name + "(" + s.PYCode + ")" ).ToArray() );
			List<string> names = DataTableTools.ReadTableNames(_jscfg.globalconfig.db);
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
		public  List<Stock> StocksByItemsShow()
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

        private void buttonApply_Click(object sender, EventArgs e)
        {
           
        }
        
        private void ButtonQQFinClick(object sender, EventArgs e)
		{
            Stocks _stocks =  _jscfg.globalconfig.Stocks;
			if (_stocks == null || _stocks.stocks.Count == 0)
				return;
			if(_stocks.Gcfg.db == null) return;
			if (!_isrunning)
			{
				_bshowtime = false;// checkBoxShowTimeOut.Checked;
				_isrunning = true;
				_completebtn = buttonQQFin;
				_completebtn.Enabled = false;				
				JHStock.Update.QQFin qff = new Update.QQFin(_stocks);
				JHStock.Update.ThreadUpdateStocksQQFin qf = qff.tupdate;
				qf.MaxThreadSum = 20;
				qf.showmsg = new ShowDeleGate(ThreadShowMsg);
                qf.CompleteRun = new CompleteDeleGate(ThreadCompleteRun);				
                //qf.DealStocks.Add(_stocks.StockByIndex(2));
                //qf.DealStocks.Add(_stocks.StockByIndex(3))
                qf.DealStocks =_stocks.stocks;				
				_updatetime = DateTime.Now;
				System.Threading.Thread nonParameterThread = new Thread(qf.RunNetDownLoadData);
				nonParameterThread.Start();
			}
		}
        
		private Boolean _bshowtime;
		private Boolean _isrunning;
		private Button _completebtn;
		private DateTime _updatetime;
		private string _ErrorMsg;
		private void ThreadShowMsg(string msg)
		{
			this.Invoke(new ShowDeleGate(showfiletxt), new object[] { msg });
		}
		private void ThreadAppendMsg(string msg)
		{
			Invoke(new ShowDeleGate(showappendfiletxt), new object[] { msg });
		}
		private void ThreadCompleteRun()
		{
			Invoke(new CompleteDeleGate(CompleteRun));
		}
		public void CompleteRun()
		{
			_completebtn.Enabled = true;
			_isrunning = false;
			string msg = "本次更新开始于"+_updatetime.ToLongTimeString()+
				"结束于"+DateTime.Now.ToLongTimeString()+" 共耗时 "+
				DateTime.Now.Subtract(_updatetime).TotalSeconds+" 秒，共更新了"+
				"条记录\r\n\r\n";
			if(_jscfg.globalconfig.ErrMsg != "")
                _ErrorMsg += _jscfg.globalconfig.ErrMsg;
			if(_ErrorMsg=="")
				showfiletxt("已全部完成"+msg);
			else
				showfiletxt("ErrorMsg:"+_ErrorMsg+" Msg:"+msg);
			MFile.AppendAllText("update.log",msg.Trim()+"ErrorMsg:"+_ErrorMsg+"\r\n\r\n");
		}
		public void showfiletxt(string file)
		{
			this.textBoxInfor.Text = file;
		}
		public void showappendfiletxt(string file)
		{
			this.textBoxInfor.Text += file;
		}

        private void buttonSaveDataSelfTest_Click(object sender, EventArgs e)
        {
            //TODO:SaveDataSelfTest
            SaveTag st = _jscfg.globalconfig.StocksData.SaveTag;
            int newdate = st.Tag[0].kd[st.Tag[0].kd.Count - 1].date;
            string outstr = "index\tCount\tbegindate\tenddate\thasdistinct\n"+
            st.Tag.Where(r  => r.kd!=null && r.kd[r.kd.Count-1].date !=  newdate ).Select( r => {
                if (r.kd.Count > 0)
                {
                    return r.index + "\t" + r.kd.Count + "\t" + r.kd[0].date + "\t" + r.kd[r.kd.Count - 1].date
                        +"\t"+(r.kd.Count-r.kd.Select( r1=>r1.date).Distinct().Count());

                }
                return  "";
               }).Aggregate((r1, r2) => r1 + "\r\n" + r2);
            MFile.WriteAllText("selftest.txt", outstr);
        }
	}	
	public class DTNameType{
		public string Name;
		public Type type;
	}    
}
