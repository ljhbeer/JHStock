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
using JHStock.UserForm;
using System.Reflection;

namespace JHStock
{
	public partial class Form1 : Form
	{
        public Form1(JSConfig jscfg=null)
        {
			InitializeComponent();
            this._jscfg = jscfg;
            
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
            _fd = _fm = _fw = null;

            _CN = new ChineseName();
            _MonitDateType = "dayly";
        }

        private void InitJsconfig()
        {
            string filename = textBoxMdbPath.Text;
            if (!(filename != "" && File.Exists(filename)))
            {
                MessageBox.Show("配置文件不存在，请检查后重置");
                return;
            }

            JSConfig jscfg = new JSConfig();
            jscfg.Load(filename);
            BaseConfig cfg = jscfg.baseconfig;
            MFile.cfg = cfg;
            if (!cfg.CheckWorkPath())
                MessageBox.Show("工作目录不存在，请手工创建" + cfg.NowWorkPath());

            if (!jscfg.globalconfig.InitStocks())
            {
                MessageBox.Show(_jscfg.globalconfig.ErrMsg); //退出
                MessageBox.Show("配置文件不正确，请检查后重置");
                return;
            }
            this._jscfg = jscfg;
        }
        private int ToIntDate(DateTime dt)
        {
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }
		private void Form1_Load(object sender, EventArgs e)
		{
            if (this._jscfg == null)
                InitJsconfig();
            if (this._jscfg == null)
            {
                this.Close();
            }
			InitDBAndStocks();
            radioButtonyears.Checked = true;
            foreach (string str in JsonMainCWFX.GetNameList())
            {
                comboBoxProper.Items.Add(_CN.GetChineseName(str));
            }
            if (_jscfg.Memostr != null && _jscfg.Memostr.Contains("LoadShowDayly"))
            {
                HideShowMonit(ref _fd, "dayly");
                Thread t = new Thread(new ThreadStart(ThreadHideMyself));
                t.Start();
            }
		}
        public void ThreadHideMyself()
        {
            //Thread.Sleep(1000);
            this.Invoke(  new CompleteDeleGate( HideMySelf));            
        }
        public void HideMySelf()
        {
            this.Hide();
        }
		private void buttonImportDB_Click(object sender, EventArgs e)
		{
			InitDBAndStocks();
		}
		private void InitDBAndStocks(){           
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
		private void listBox1_KeyUp(object sender, KeyEventArgs e)
		{
			if (listBox1.SelectedIndex == -1) return;
			if (e.KeyCode == Keys.D)
			{
				listBox1.Items.RemoveAt(listBox1.SelectedIndex);
			}else if(e.KeyCode == Keys.P || e.KeyCode ==Keys.O){
				if (listBox1.SelectedIndex == -1) return;
                ColorStyle cs = ColorStyles.classic;
                if (e.KeyCode == Keys.O)
                    cs = ColorStyles.print;
				string numcode = listBox1.SelectedItem.ToString().Substring(2, 6);
				Stock s = _stocks.StockByNumCode(numcode);
                Bitmap bmp = StockDraw.DrawDailyLocal(s, cs);
				if (bmp!=null)				
                {				
					FormPictureBox f = new FormPictureBox(bmp);
					f.ShowDialog();
				}
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
        private void radioButtonDay_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            string txt = rb.Text.ToLower() + "ly";
            _MonitDateType = "dayly";
            if ("|dayly|weekly|monthly|".Contains(txt))
                _MonitDateType = txt;
            _jscfg.KdataType = _MonitDateType;
        }
        private void buttonMA_Click(object sender, EventArgs e)
        {
            if (_MonitDateType == "dayly")
                HideShowMonit(ref _fd, _MonitDateType);
            else if (_MonitDateType == "weekly")
                HideShowMonit(ref _fw, _MonitDateType);
            else if (_MonitDateType == "monthly")
                HideShowMonit(ref _fm, _MonitDateType);
        }
        private void HideShowMonit(ref FormMonit f,string datetype)
        {
            this.Hide();
            if (f == null)
                f = new FormMonit(datetype, this);
            f.InitShowConfig(_jscfg);
            f.DebugStocks =  StocksByItemsShow();
            f.Show();
            //MessageBox.Show("show2");
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonyears.Checked)
                _CWDateType = "years";
            if (radioButtonreport.Checked)
                _CWDateType = "reports";
            if (_jscfg != null)
            {
                _jscfg.baseconfig.SetCWFilePath(_CWDateType);
                _jscfg.baseconfig.ReLoadCWFX = true;
            }
           
        }
        private void buttonApply_Click(object sender, EventArgs e)
        {

        }
        private void ButtonFinCW1Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string Type = _CWDateType;
            Stocks _stocks = _jscfg.globalconfig.Stocks;
            if (_stocks == null || _stocks.stocks.Count == 0)
                return;
            if (_stocks.Gcfg.db == null) return;
            if (!_isrunning)
            {
                //_bshowtime = false;// checkBoxShowTimeOut.Checked;
                _isrunning = true;
                _completebtn = btn;
                _completebtn.Enabled = false;
                UpdateFin updatefin = new UpdateFin(_stocks);
                updatefin.SetDateType(Type); ;
                updatefin.MaxThreadSum = 20;
                updatefin.showmsg = new ShowDeleGate(ThreadShowMsg);
                updatefin.ThreadCompleteRun = new CompleteDeleGate(ThreadCompleteRun);
                ////qf.DealStocks.Add(_stocks.StockByIndex(2));
                ////qf.DealStocks.Add(_stocks.StockByIndex(3))
                updatefin.DealStocks = _stocks.stocks;
                _updatetime = DateTime.Now;
                System.Threading.Thread nonParameterThread = new Thread(updatefin.DownLoadFinData);
                nonParameterThread.Start();
            }
        }
        private void buttonDownloadsAllKdata_Click(object sender, EventArgs e)
        {
            DealClass DC = new DealClass(_jscfg , _stocks,this);
            Button btn = (Button)sender;
            //DC.TestDownLoads(btn);          
        }
        private void buttonExportStockCW_Click(object sender, EventArgs e)
        {
            if (comboBoxProper.SelectedIndex == -1) return;
            string ChineseName = comboBoxProper.SelectedItem.ToString();
            string propertyname = _CN.GetProperName(comboBoxProper.SelectedItem.ToString());
            if (propertyname == "") return;
            Type type = typeof(JsonMainCWFX);
            PropertyInfo property = type.GetProperty(propertyname);
            if (property == null) MessageBox.Show("找不到属性名称");
            StringBuilder sb = new StringBuilder();
            List<Stock> DealStocks = StocksByItemsShow();
            if (DealStocks.Count == 0)
                DealStocks = _stocks.stocks;
            try
            {
                foreach (Stock s in DealStocks)
                {
                    sb.Append(s.Name + "\t" + s.Code + "\t");
                    Tagstock t = _jscfg.globalconfig.Stocks.GetTagstock(s.ID);
                    if (t != null && t.Tag != null)
                    {
                        List<JsonMainCWFX> ls = JsonConvert.DeserializeObject<List<JsonMainCWFX>>(t.Tag.ToString());
                        sb.AppendLine(string.Join("\t", ls.Select(rr =>
                        {
                            object o = property.GetValue(rr, null);
                            if (o == null)
                                return rr.date + "\t" + string.Empty;
                            return rr.date + "\t" + o.ToString();
                        }).ToList()));
                    }
                }
                string savefilename = _jscfg.baseconfig.NowWorkPath() + "Export_"+_CWDateType + ChineseName + ".txt";
                MFile.WriteAllText(savefilename, sb.ToString());
                MessageBox.Show("已保存到文件：" + savefilename);
            }
            catch (Exception ee)
            {
                MessageBox.Show("保存失败,原因是：" + ee.Message);
            }
        }
        private void buttonSaveDataSelfTest_Click(object sender, EventArgs e)
        {
            //TODO:SaveDataSelfTest
            SaveKdTag st = _jscfg.globalconfig.StocksData.SavekdTag;
            int newdate = st.Tag[0].kd[st.Tag[0].kd.Count - 1].date;


            string outstr = "index\tCount\tbegindate\tenddate\thasdistinct\n" +
            st.Tag.Where(r => r.kd != null && r.kd[r.kd.Count - 1].date != newdate).Select(r =>
            {
                if (r.kd.Count > 0)
                {
                    var query = from p in r.kd
                                group p by p.date into g
                                where g.Count() > 1
                                select g;
                    string mult = "\t";
                    foreach (var qu in query)
                        mult += qu.Key + " " + qu.Count() + "\t";
                    return r.index + "\t" + r.kd.Count + "\t" + r.kd[0].date + "\t" + r.kd[r.kd.Count - 1].date
                        + "\t" + (r.kd.Count - r.kd.Select(r1 => r1.date).Distinct().Count()) + mult;

                }
                return "";
            }).Aggregate((r1, r2) => r1 + "\r\n" + r2);
            List<tagkdstock> hasdistinctarray =
                st.Tag.Where(r =>
                {
                    if (r.kd != null && r.kd[r.kd.Count - 1].date != newdate)
                        return r.kd.Count > r.kd.Select(r1 => r1.date).Distinct().Count();    // 有重复
                    return false;
                }).ToList();
            if (hasdistinctarray.Count > 0)
            {
                outstr += "\r\n==========================================\r\n" + hasdistinctarray
                    .Select(r =>
                    {
                        if (r.kd.Count > 0)
                        {
                            var query = from p in r.kd
                                        group p by p.date into g
                                        where g.Count() > 1
                                        select g;
                            string mult = "\t";
                            foreach (var qu in query)
                                mult += qu.Key + " " + qu.Count() + "\t";
                            return r.index + "\t" + r.kd.Count + "\t" + r.kd[0].date + "\t" + r.kd[r.kd.Count - 1].date
                                + "\t" + (r.kd.Count - r.kd.Select(r1 => r1.date).Distinct().Count()) + mult;

                        }
                        return "";
                    }).Aggregate((r1, r2) => r1 + "\r\n" + r2);
            }
            MFile.WriteAllText("selftest.txt", outstr);
        }
        private void buttonExportStock_Click(object sender, EventArgs e)
        {          
            if (listBox2.Items.Count == 0) return;         
            DealClass DC = new DealClass(_jscfg, _stocks, this);
            DC.ExportKData( StocksByItemsShow(),  GetActiveFormMonit());           
        }
        private FormMonit GetActiveFormMonit()
        {
            FormMonit f = null;
            if (_MonitDateType == "dayly")
            {
                if (_fd == null)
                    _fd = new FormMonit(_MonitDateType, this);
                f = _fd;
            }
            else if (_MonitDateType == "weekly")
            {
                if (_fw == null)
                    _fw = new FormMonit(_MonitDateType, this);
                f = _fw;
            }
            else if (_MonitDateType == "monthly")
            {
                if (_fm == null)
                    _fm = new FormMonit(_MonitDateType, this);
                f = _fm;
            }
            f.InitShowConfig(_jscfg);
            return f;
        }
        private void buttonExportFhKgData_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            DealClass DC = new DealClass(_jscfg, _stocks, this);
            DC.ExportFhKgData(StocksByItemsShow());  
        }
        private void buttonExportMacd_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            DealClass DC = new DealClass(_jscfg, _stocks, this);
          
            DC.ExportMacd(StocksByItemsShow(), GetActiveFormMonit()  , checkBoxOrginKdata.Checked && _MonitDateType == "dayly");   
        }
        private void buttonCreateMacd_Click(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;
            DealClass DC = new DealClass(_jscfg, _stocks, this);
            DC.CreateAllMacd( GetActiveFormMonit() );   
           
            _stocks.ResetMacdData();
            ((Button)sender).Enabled = true;
        }
        private void radioButtonVol_CheckedChanged(object sender, EventArgs e)
        {
            ComputeVol = radioButtonVol.Checked;
        }
        private void ReCompute_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            DealClass DC = new DealClass(_jscfg, _stocks, this);
            int days = 0;
            if (!ComputeVol && _MonitDateType == "dayly" && textBoxExchangeTime.Text.Trim()!="")
            {
                try
                {
                    days = Convert.ToInt32(textBoxExchangeTime.Text);
                    if (days < 10 || days > 60)
                        days = 0;
                }
                catch
                {
                    days = 0;
                }
            }


            DC.ExportTestKData(StocksByItemsShow(), GetActiveFormMonit(),ComputeVol,checkBoxKDate.Checked,days); 
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

        public void ThreadActionMsg(string msg)
        {
            Invoke(new ActionDeleGate(ActionMsg), new object[] { msg });
        }
        public void ActionMsg(string msg)
        {
            if (msg.StartsWith("showexchangingtime-"))
                textBoxExchangeTime.Text = msg.Substring(19);

        }
        public void ThreadShowMsg(string msg)
        {
            this.Invoke(new ShowDeleGate(showfiletxt), new object[] { msg });
        }
        public void ThreadAppendMsg(string msg)
        {
            Invoke(new ShowDeleGate(showappendfiletxt), new object[] { msg });
        }
        public void ThreadCompleteRun()
        {
            Invoke(new CompleteDeleGate(CompleteRun));
        }
        public void CompleteRun()
        {
            _completebtn.Enabled = true;
            _isrunning = false;
            string msg = "本次更新开始于" + _updatetime +
                "结束于" + DateTime.Now.ToLongTimeString() + " 共耗时 " +
                DateTime.Now.Subtract(_updatetime).TotalSeconds + " 秒，共更新了" +
                "条记录\r\n\r\n";
            if (_jscfg.globalconfig.ErrMsg != "")
                _ErrorMsg += _jscfg.globalconfig.ErrMsg;
            if (_ErrorMsg == "")
                showfiletxt("已全部完成" + msg);
            else
                showfiletxt("ErrorMsg:" + _ErrorMsg + " Msg:" + msg);
            MFile.AppendAllText("update.log", msg.Trim() + "ErrorMsg:" + _ErrorMsg + "\r\n\r\n");
        }
        public void showfiletxt(string file)
        {
            this.textBoxInfor.Text = file;
        }
        public void showappendfiletxt(string file)
        {
            this.textBoxInfor.Text += file;
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
        //private Boolean _bshowtime;
		private Boolean _isrunning;
		private Button _completebtn;
		public DateTime _updatetime;
		private string _ErrorMsg;
        private FormMonit _fw;
        private FormMonit _fm;
        private FormMonit _fd;
        private ChineseName _CN;
        private StocksData _stockdata;
        private bool bComplete;
        private bool ComputeVol;
        public string _CWDateType { get; set; }
        public string _MonitDateType { get; set; }

        private void buttonImportCustom_Click(object sender, EventArgs e)
        {
            FormImportCustomStocks f = new FormImportCustomStocks(items,_stocks);
            f.ShowDialog();
            if (f.ImportOK)
            {
                bool change = false;
                foreach (string txt in f.ImportItems)
                {
                        if (!itemsShow.Contains(txt))
                        {
                            itemsShow.Add(txt);
                            change = true;
                        }
                }
                if (change)
                {
                    listBox2.Items.Clear();
                    listBox2.Items.AddRange(itemsShow.ToArray());
                }
            }
        }
       
    }	
	public class DTNameType{
		public string Name;
		public Type type;
	}
}

