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
			}else if(e.KeyCode == Keys.P){
				if (listBox1.SelectedIndex == -1) return;				
				string numcode = listBox1.SelectedItem.ToString().Substring(2, 6);
				
				StocksData _stockdata = _jscfg.globalconfig.StocksData;
				Stock s = _stocks.StockByNumCode(numcode);
				if (s!=null && _stockdata.SaveTag.Tag[s.ID]!=null)
				{
					//TODO:   drawdaily in form1
					KData[] kd = _stockdata.SaveTag.Tag[s.ID].kd.ToArray();
					Bitmap bmp = DrawDaily(kd);
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
        public static void DrawBitmap(Bitmap bmp,List<Rectangle> ListItem,List<bool> isred,Brush brbackground){    		
    		Pen pr = Pens.Red;
    		Pen pc = Pens.Green;
            Brush br = Brushes.Green;            
    		using (Graphics g = Graphics.FromImage(bmp))
            {                
                for (int i = 0; i < ListItem.Count; i++)
                { 
                	g.FillRectangle(brbackground,ListItem[i]);           
					
                	if (isred[i] ){   
                		if(ListItem[i].Width ==1)
                			g.DrawLine(pr,ListItem[i].X,ListItem[i].Y,ListItem[i].Right,ListItem[i].Bottom);
                		else
							g.DrawRectangle(pr, ListItem[i]);     		
                	}else{
                		if(ListItem[i].Width ==1)
                			g.DrawLine(pc,ListItem[i].X,ListItem[i].Y,ListItem[i].Right,ListItem[i].Bottom);
                		else
                    		g.FillRectangle(br, ListItem[i]);
                	}
                }
            }
    	}
        public static void DrawBitmapPrice(Bitmap bmp,List<Rectangle> Lines,List<string> txts){
        	Pen pr = Pens.Red;
            Brush br = Brushes.Green;        
            Font f = new Font( DefaultFont.Name,12);
    		using (Graphics g = Graphics.FromImage(bmp))
            {                
                for (int i = 0; i < Lines.Count; i++)
                {       
                    //g.DrawRectangle(pr, Lines[i]);
                    g.DrawLine(pr, Lines[i].X,Lines[i].Y,Lines[i].Right,Lines[i].Bottom);
                    g.DrawString(txts[i],f,Brushes.Red,Lines[i].Right,Lines[i].Bottom);
                }
            }
        }
        public static bool DrawBitmap(KData[] kd,Bitmap bmp,Rectangle rect){
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
        	
        	List<bool> lb = kd.Select( r => r.open<r.close).ToList();
        	DrawBitmap(bmp,hlr.Concat(ocr).ToList(),lb.Concat(lb).ToList(),Brushes.Black); // DrawKLine
        	//TODO: Drawprice
        	List<Rectangle> Lines = new List<Rectangle>();
        	List<string> pricetxt = new List<string>();
        	for( i=0; i<11; i++){
        		Lines.Add( new Rectangle( rect.X, rect.Y + (int)( rect.Height*i/10.0),rect.Width,1));
        		pricetxt.Add( ((maxprice+ pricemaxplusmin*i/10.0)/100.0).ToString());
        	}
        	DrawBitmapPrice(bmp,Lines,pricetxt);
        	return true;
        }
        public static bool DrawBitmapVol(KData[] kd,Bitmap bmp,Rectangle rect){
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
        		        	int w = (int)(dx+0.5) >0 ? (int)(dx+0.5) :1;
        		        	Rectangle trect = new Rectangle(x,y,w,h );
        		        	i++;
        		        	return trect;
        		        }).ToList();
        	i=0;
        	
        	
        	List<bool> lb = kd.Select( r => r.open<r.close).ToList();
        	DrawBitmap(bmp,hlr,lb,Brushes.Black); // DrawKLine
        	
        	//TODO: DrawVolTest
        	List<Rectangle> Lines = new List<Rectangle>();
        	List<string> pricetxt = new List<string>();
        	for( i=0; i<4; i++){
        		Lines.Add( new Rectangle( rect.X, rect.Y + (int)( rect.Height*i/4.0),rect.Width,1));
        		pricetxt.Add( ((max*i/4.0)/100.0).ToString());
        	}
        	DrawBitmapPrice(bmp,Lines,pricetxt);
        	return true;
        }
        public static Bitmap DrawDaily(KData[] kd){        	
        	int width = 1200;
        	int height = 800;
        	Bitmap Bmp = new Bitmap(width,height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        	Brush brback = Brushes.Black;
    		Bitmap bmp = Bmp;
    		using (Graphics g = Graphics.FromImage(bmp))
            { 
    			g.FillRectangle(brback, new Rectangle(0, 0, bmp.Width, bmp.Height));    			
    		}
    		
        	DrawBitmap(kd,bmp,new Rectangle(20,20,920,620)); // Drawprice  
        	DrawBitmapVol(kd,bmp,new Rectangle(20,620,920,160));//
        	//Draw Txt  
        	// 自动换行  URL
        	// http://bbs.csdn.net/topics/391036711
        	// http://www.cnblogs.com/dannyqiu/articles/2837515.html
        	return Bmp;
        }
	}	
	public class DTNameType{
		public string Name;
		public Type type;
	}    
}
