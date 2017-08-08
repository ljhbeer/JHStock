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
			}else if(e.KeyCode == Keys.P || e.KeyCode ==Keys.O){
				if (listBox1.SelectedIndex == -1) return;
                ColorStyle cs = ColorStyles.classic;
                if (e.KeyCode == Keys.O)
                    cs = ColorStyles.print;
				string numcode = listBox1.SelectedItem.ToString().Substring(2, 6);
				
				StocksData _stockdata = _jscfg.globalconfig.StocksData;
				Stock s = _stocks.StockByNumCode(numcode);
				if (s!=null && _stockdata.SaveTag.Tag[s.ID]!=null)
				{
					//TODO:   drawdaily in form1
					KData[] kd = _stockdata.SaveTag.Tag[s.ID].kd.ToArray();
                    if(kd==null||kd.Length==0) return;
					Bitmap bmp = DrawDaily(kd,s.Name+s.Code+"(日线"+kd[0].date+"-"+kd[kd.Length-1].date+")",cs);
                    string baseinfor = "";
                    try
                    {
                        if (_stockdata.Netdate.Inline)
                            baseinfor = UpdateMonitInfor.GetCWXXS(s);
                    }catch(Exception ee){}
                    DrawBaseInformation(s,baseinfor, bmp, cs);
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
			string msg = "本次更新开始于"+_updatetime +
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
                	var query = from p in r.kd
                		group p by p.date into g                 		
                		where g.Count() > 1
                		select g;
                	string mult = "\t";
                	foreach(var qu in query)
                		mult+= qu.Key + " "+qu.Count()+"\t";
                    return r.index + "\t" + r.kd.Count + "\t" + r.kd[0].date + "\t" + r.kd[r.kd.Count - 1].date
                        +"\t"+(r.kd.Count-r.kd.Select( r1=>r1.date).Distinct().Count())+mult;

                }
                return  "";
               }).Aggregate((r1, r2) => r1 + "\r\n" + r2);
            List<tagstock> hasdistinctarray = 
                st.Tag.Where(r => 
                    {
                        if (r.kd != null && r.kd[r.kd.Count - 1].date != newdate)
                            return r.kd.Count > r.kd.Select(r1 => r1.date).Distinct().Count();    // 有重复
                        return false;
                    })  .ToList();        
            if(hasdistinctarray.Count>0){
            outstr+= "\r\n==========================================\r\n"+  hasdistinctarray
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
        public static void DrawBitmap(Bitmap bmp,List<Rectangle> ListItem,List<bool> isred,ColorStyle cs){  
    		using (Graphics g = Graphics.FromImage(bmp))
            {                
                for (int i = 0; i < ListItem.Count; i++)
                { 
                	g.FillRectangle(cs.backgroundbrush,ListItem[i]);           
					
                	if (isred[i] ){   
                		if(ListItem[i].Width ==1)
                			g.DrawLine(cs.klinerosepen,ListItem[i].X,ListItem[i].Y,ListItem[i].X,ListItem[i].Bottom);
                		else
							g.DrawRectangle(cs.klinerosepen, ListItem[i]);     		
                	}else{
                		if(ListItem[i].Width ==1)
                			g.DrawLine(cs.klinedeclinepen,ListItem[i].X,ListItem[i].Y,ListItem[i].X,ListItem[i].Bottom);
                		else
                    		g.FillRectangle(cs.klinedeclinebush, ListItem[i]);
                	}
                }
            }
    	}
        public static void DrawBitmapLinesAndPriceText(Bitmap bmp,List<Rectangle> Lines,List<string> txts,ColorStyle cs){        
    		using (Graphics g = Graphics.FromImage(bmp))
            {                
                for (int i = 0; i < Lines.Count; i++)
                {       
                    g.DrawLine(cs.textpricelinepen2, Lines[i].X,Lines[i].Y,Lines[i].Right,Lines[i].Bottom);
                    g.DrawString(txts[i], cs.textfont, cs.textpricelinebrush, Lines[i].Right, Lines[i].Bottom);
                }
            }
        }
        public static bool DrawBitmapPrice(KData[] kd,Bitmap bmp,Rectangle rect,ColorStyle cs){
        	//TODO: UnDraw Date
        	if(bmp == null || kd==null || kd.Count()==0 || rect.X+rect.Width>bmp.Width || rect.Y + rect.Height>bmp.Height)
        		return false;
        	int startx = rect.X;
        	int starty = rect.Y;
        	int width = rect.Width;
        	int height =rect.Height;
        	
        	int maxprice = kd.Max( r=> r.high);
        	int minprice = kd.Min( r=> r.low);
        	int pricemaxplusmin = maxprice  - minprice;
        	double dx = width*1.0/kd.Count();
        	double dy = height*1.0/pricemaxplusmin;
        	int i=0;
        	List<Rectangle> hlr = kd
        		.Select( r=> {
        		        	int x = (int)( i*dx +dx *0.5) + startx;
        		        	int y = (int)((maxprice-r.high)*dy) + starty;
        		        	int h =(int)( (r.high-r.low)*dy);
        		        	h = h>0?h:1;        		        	
        		        	Rectangle trect = new Rectangle(x,y,1,h );
        		        	i++;
        		        	return trect;
        		        }).ToList();
        	i=0;
        	List<Rectangle> ocr = kd
        		.Select( r=> {
        		        	int x = (int)( i*dx + dx * 0.2) +startx;
        		        	int y = (int)((maxprice- Math.Max(r.open,r.close))*dy) + starty;
        		        	int h =(int)( (Math.Abs(r.open-r.close))*dy);
        		        	    h = h>0?h:1;
        		        	int w = (int)( dx*0.6 + 0.5) > 0 ? (int)( dx*0.6+0.5):1;
        		        	Rectangle trect = new Rectangle(x,y,w,h );
        		        	i++;
        		        	return trect;
        		        }).ToList();
        	
        	//Drawprice
        	List<Rectangle> Lines = new List<Rectangle>();
        	List<string> pricetxt = new List<string>();
        	for( i=0; i<11; i++){
        		Lines.Add( new Rectangle( rect.X, rect.Y + (int)( rect.Height*i/10.0),rect.Width,1));
        		pricetxt.Add( ((maxprice- pricemaxplusmin*i/10.0)/100.0).ToString());
        	}
        	DrawBitmapLinesAndPriceText(bmp,Lines,pricetxt,cs);
            // DrawKLine
        	List<bool> lb = kd.Select( r => r.open<r.close).ToList();
        	DrawBitmap(bmp,hlr.Concat(ocr).ToList(),lb.Concat(lb).ToList(),cs); 
        	return true;
        }
        public static bool DrawBitmapVol(KData[] kd,Bitmap bmp,Rectangle rect,ColorStyle cs){
        	//TODO: DrawVol
        	if(bmp == null || kd==null || kd.Count()==0 || rect.X+rect.Width>bmp.Width || rect.Y + rect.Height>bmp.Height)
        		return false;
        	int startx = rect.X;
        	int starty = rect.Y;
        	int width = rect.Width;
        	int height =rect.Height;
        	
        	int max = kd.Max( r=> r.vol);        	
        	double dx = width*1.0/kd.Count();
        	double dy = height*1.0/max;
        	int i=0;
        	List<Rectangle> hlr = kd
        		.Select( r=> {
        		        	int x = (int)( i*dx) + startx;
        		        	int y = (int)((max-r.vol)*dy) + starty;
        		        	int h =(int)( r.vol*dy);
        		        	h = h>0?h:1;
                            int w = (int)(dx + 0.5);
                            int gap = w / 4 > 0 ? w / 4 : 1;
                            w = w - gap > 0 ? w - gap : 1;
        		        	Rectangle trect = new Rectangle(x,y,w,h );
        		        	i++;
        		        	return trect;
        		        }).ToList();        	
        	//DrawVolTest
        	List<Rectangle> Lines = new List<Rectangle>();
        	List<string> pricetxt = new List<string>();
        	for( i=0; i<4; i++){
        		Lines.Add( new Rectangle( rect.X, rect.Y + (int)( rect.Height*i/4.0),rect.Width,1));
        		pricetxt.Add( ((max*i/4.0)/100.0).ToString());
        	}
        	DrawBitmapLinesAndPriceText(bmp,Lines,pricetxt,cs);
            // DrawKLine
        	List<bool> redgreen = kd.Select( r => r.open<r.close).ToList();
        	DrawBitmap(bmp,hlr,redgreen,cs); 
        	return true;
        }
        private static bool DrawBitmapDate(KData[] kd, Bitmap bmp, Rectangle rect, ColorStyle cs)
        {
            if (bmp == null || kd == null || kd.Count() == 0 || rect.X + rect.Width > bmp.Width || rect.Y + rect.Height > bmp.Height)
                return false;
            //List<int> years = kd.Select(r => r.date/10000).Distinct().ToList();            
            List<List<int>> monthcount = ComputeYearMonthPostion(kd);
            DateTime dt = new DateTime();
            IFormatProvider format = new System.Globalization.CultureInfo("zh-CN");
            List<DayOfWeek> dayofweek = kd.Select(r => DateTime.ParseExact(r.date.ToString(),"yyyyMMdd",format).DayOfWeek).ToList();

            int sum = 0;
            double dx = rect.Width*1.0 / kd.Length;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int k = 0; k < monthcount.Count; k++)
                {
                    int X = (int)(dx * sum+0.5)+rect.X;
                    string str = monthcount[k][0].ToString();
                    g.DrawLine(cs.textpricelinepen2, X, rect.Top, X, rect.Bottom);
                    g.DrawString(str, cs.textfont, cs.textpricelinebrush, X, rect.Y);
                    sum += monthcount[k][1];
                }
                DayOfWeek nowdofw = dayofweek[0];
                for (int k = 1; k <dayofweek.Count; k++)
                {
                    if (nowdofw > dayofweek[k]) //new week
                    {
                        int X = (int)(dx * k + 0.5) + rect.X;
                        string str =( kd[k].date % 100).ToString();
                        g.DrawLine(cs.textpricelinepen2, X, rect.Top, X, rect.Top+rect.Height/2);
                        g.DrawString(str, cs.textfont2, cs.textpricelinebrush, X, rect.Top + rect.Height / 2);
                    }
                    nowdofw = dayofweek[k];
                }
            }
            return true;
        }
        public static Bitmap DrawDaily(KData[] kd,string namecode,ColorStyle cs){        	
        	int width = 1200;
        	int height = 840;
            Rectangle pricerect = new Rectangle(20,20,920,600);
            Rectangle volrect = new Rectangle(20, 630, 920, 160);
            Rectangle daterect = new Rectangle(20, 790, 920, 25);
        	Bitmap bmp = new Bitmap(width,height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);        	
    		using (Graphics g = Graphics.FromImage(bmp)) //drawground
            { 
    			g.FillRectangle(cs.backgroundbrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.DrawString(namecode, cs.textfont, cs.textlablebrush, 20, 0);
                g.DrawRectangle(cs.textpricelinepen, pricerect);
                g.DrawRectangle(cs.textpricelinepen, volrect);
                g.DrawRectangle(cs.textpricelinepen, daterect);
    		}
    		
        	DrawBitmapPrice(kd,bmp,pricerect,cs); // Drawprice  
        	DrawBitmapVol(kd,bmp,volrect,cs);//
            DrawBitmapDate(kd, bmp, daterect, cs);
            
        	return bmp;
        }
        private static List<List<int>> ComputeYearMonthPostion(KData[] kd)
        {
            var query = from p in kd
                        group p by p.date / 100 into g
                        select g;
            List<List<int>> monthcount = new List<List<int>>();
            foreach (var qu in query)
                monthcount.Add(new List<int>() { qu.Key, qu.Count() });
            int count = monthcount.Sum(r => r[1]) - kd.Count();
            ////string str= count+"\r\n"+ monthcount.Select( r => r[0]+"\t"+r[1]).Aggregate((r1, r2) => r1 + "\r\n" + r2);
            ////MessageBox.Show(str);
            if (count != 0) return new List<List<int>>();
            int nowyear = 0;
            for (int k = 0; k < monthcount.Count; k++)
            {
                if (nowyear == monthcount[k][0] / 100)
                    monthcount[k][0] = monthcount[k][0] % 100;
                else
                {
                    nowyear = monthcount[k][0] / 100;
                    monthcount[k][0] = nowyear;
                }
            }
            return monthcount;
        }
        private void DrawBaseInformation(Stock s,string baseinfor, Bitmap bmp, ColorStyle cs)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Rectangle  rect = new Rectangle(1000, 20, 198, 800); //informationrect
            Rectangle rectinfor = new Rectangle(rect.X, rect.Y + 30, rect.Width, rect.Height - 30);
            using (Graphics g = Graphics.FromImage(bmp)) //drawground
            {
                g.DrawRectangle(cs.textpricelinepen, rect);
                g.DrawString(s.Name +s.Code, cs.textfont, cs.textlablebrush, rect.Location);

                g.DrawString(baseinfor, cs.textfont2, cs.textlablebrush, rectinfor);
            }

        }

	}	
	public class DTNameType{
		public string Name;
		public Type type;
	}
    public class ColorStyle
    {
        public ColorStyle( Color background,Color klinerose,Color klinedecline,Color textprice,Color textlable,Font defaultfont=null)
        {
            backgroundbrush = new SolidBrush(background);
            backgroundpen = new Pen(background);
            klinedeclinebush = new SolidBrush(klinedecline);
            klinedeclinepen = new Pen(klinedecline);
            klinerosebrush = new SolidBrush(klinerose);
            klinerosepen = new Pen(klinerose);
            textpricelinepen = new Pen(textprice);
            textpricelinepen2 = new Pen(textprice);
            textpricelinepen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            textpricelinepen2.DashPattern = new float[] { 1f, 1f };

            textpricelinebrush = new SolidBrush(textprice);
            textlablepen = new Pen(textlable);
            textlablebrush = new SolidBrush(textlable);
            if(defaultfont==null) 
                textfont = new Font("宋体", 12);
            else
                textfont = defaultfont;

            textfont2 = new Font("宋体", 9);
        }
        public Brush backgroundbrush;
        public Pen backgroundpen;
        public Brush klinerosebrush;
        public Pen klinerosepen;
        public Brush klinedeclinebush;
        public Pen klinedeclinepen;
        public Brush textpricelinebrush;
        public Pen textpricelinepen;
        public Pen textpricelinepen2; //绘制虚线
        public Brush textlablebrush;
        public Pen textlablepen;
        public Font textfont;
        public Font textfont2; 
    }
    public class ColorStyles
    {
        public static ColorStyle classic = new ColorStyle(Color.Black, Color.Red, Color.FromArgb(128,255,255), Color.Red, Color.White);
        public static ColorStyle print = new ColorStyle(Color.White, Color.Black, Color.Gray, Color.Black, Color.Gray);
    }
}
