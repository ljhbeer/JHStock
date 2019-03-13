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
using Tools;
using ToolsCXml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace JHStock.Update
{
    public class ThreadUpdateStocks
    {
        public ThreadUpdateStocks(Stocks stocks)
        {
            bshowtimeout = false;
            _stocks = stocks;
            _cfg = _stocks.Gcfg;
            _exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";
            _XmlFileName = "";
            BounsStocks = new List<Stock>();
            MaxThreadSum = 1;
            Tag = new tagkdstock[2000];
            for (int i = 0; i < 2000; i++)
                Tag[i] = new tagkdstock();
            foreach (Stock s in _stocks.stocks)
                Tag[s.ID].Init(s);
        }
        public bool InitXmlConfig(string xmlfilename)
        {
            _XmlFileName = xmlfilename.ToLower();
            _xcfg = new XmlConfig();
            _xcfg.db = _cfg.db;
            if (_xcfg.LoadXml(xmlfilename) && _xcfg.InitData())
                if (_xcfg.SrcMode != "DB" || _xcfg.ProcessMode == "txt" || _xcfg.ProcessMode == "url")
                    return true;
            return false;
        }
        public void RunNetTime(bool bUpdateAllBounsAndTopten = false) //
        {
            DateTime dt1 = System.DateTime.Now;
            while (true)
            {
                if (_XmlFileName.Contains("hexin"))
                {
                    #region hexin
                    List<Stock> stocks = GetItemUnUpdateStocks();
                    InitTag(stocks);
                    UpdateItem(stocks);
                    UpdateUpdateNumber("ItemStockhexin");
                    int stockcountb = stocks.Count;
                    stocks = Tag.Where(r => r.value < 1 && r.value > -10).Select(r => r.s).ToList();
                    if (_cfg.UpdateDebug)
                    {
                        int stockcounte = stocks.Count;
                        int stockcountc = Tag.Where(r => r.value == 1).Count();
                        MFile.AppendAllText("Update.log", System.DateTime.Now.ToShortTimeString() + " 更新ItemstockHexin " +
                                            stockcountc + "/" + stockcountb + " 其中有 " + stockcounte + "需要再次更新\r\n");
                    }
                    InitTag(stocks);
                    UpdateItem(stocks, false);
                    UpdateUpdateNumber("ItemStockhexin");
                    if (_cfg.UpdateDebug)
                    {
                        int stockcounte = stocks.Count;
                        int stockcountc = Tag.Where(r => r.value == 1).Count();
                        MFile.AppendAllText("Update.log", System.DateTime.Now.ToShortTimeString() + " 再次更新ItemstockHexin " +
                                            stockcountc + "/" + stockcounte + "\r\n");
                    }
                    #endregion
                }
                else if ((_XmlFileName.Contains("bouns") || _XmlFileName.Contains("topten")) && BounsStocks.Count > 0)
                {
                    #region bouns topten
                    string tablename = "Item" + _xcfg.DstTableName;
                    string sql = "";
                    if (bUpdateAllBounsAndTopten)
                    {
                        sql = "select id from [tablename] where UpdateNumber <> [updatenumber]"
                                 .Replace("[tablename]", tablename)
                                 .Replace("[updatenumber]", TimeStringTools.UpdateNumber());
                    }
                    else
                    {
                        string ids = string.Join(",", BounsStocks.Select(r => r.ID));
                        sql = "select id from [tablename] where UpdateNumber <> [updatenumber] and id in([ids])"
                                 .Replace("[tablename]", tablename)
                                 .Replace("[updatenumber]", TimeStringTools.UpdateNumber())
                                 .Replace("[ids]", ids);
                    }
                    List<Stock> stocks = GetStocks(sql, _stocks);
                    if (stocks.Count == 0)
                    {
                        _cfg.ErrMsg += "表" + tablename + "中没有需要更新的纪录\t";
                        break;
                    }
                    InitTag(stocks);
                    UpdateItem(stocks);
                    UpdateUpdateNumber(tablename);

                    int stockcountb = stocks.Count;
                    stocks = Tag.Where(r => r.value < 1 && r.value > -10).Select(r => r.s).ToList();
                    if (_cfg.UpdateDebug)
                    {
                        int stockcounte = stocks.Count;
                        int stockcountc = Tag.Where(r => r.value == 1).Count();
                        MFile.AppendAllText("Update.log", System.DateTime.Now.ToShortTimeString() + " 更新" + tablename +
                                            stockcountc + "/" + stockcountb + " 其中有 " + stockcounte + "需要再次更新\r\n");
                    }

                    if (stocks.Count == 0) break;
                    InitTag(stocks);
                    UpdateItem(stocks, false);
                    UpdateUpdateNumber(tablename);
                    if (_cfg.UpdateDebug)
                    {
                        int stockcounte = stocks.Count;
                        int stockcountc = Tag.Where(r => r.value == 1).Count();
                        MFile.AppendAllText("Update.log", System.DateTime.Now.ToShortTimeString() + " 再次更新" + tablename +
                                            stockcountc + "/" + stockcounte + "\r\n");
                    }
                    #endregion
                }
                break;
            }
            DateTime dt2 = System.DateTime.Now;
            TimeSpan ts = dt2.Subtract(dt1);
            if (bshowtimeout)
                MessageBox.Show("时间" + ts.TotalSeconds);
        }
        public void Run(string UpdateUrl, string ItemTxt, Stock s)
        {
            bool debug = _cfg.Debug;
            string sql = _xcfg.SqlItemUpdateTemplate;
            try
            {
                string txt = CItem.web.GetOKUrl(UpdateUrl);
                _xcfg.PreDealTxt(ref txt);
                txt = regex.Replace(txt.Trim(), " ");
                //				MFile.WriteAllText(s.ID+ ".log",txt);
                if (txt != "")
                {
                    if (ItemTxt != txt)
                    {
                        if (debug && (s.ID < 5 || s.ID == 56 || s.ID == 47 || s.ID == 100))
                        {
                            MFile.WriteAllText(s.ID + "_3.log", ItemTxt);
                            MFile.WriteAllText(s.ID + "_4.log", txt);
                        }
                        sql = sql.Replace("[-id-]", s.ID.ToString()).Replace("'[-Item-]'", "'[-Item-]',Tag = false").Replace("[-Item-]", txt.Replace("'", "''"));
                        _xcfg.db.update(sql);
                    }
                    Tag[s.ID].value = 1;
                }
                else
                {
                    Tag[s.ID].value = -1;
                }
            }
            catch
            {
                Tag[s.ID].value = -2;
            }
            Interlocked.Decrement(ref threadsum);
            Interlocked.Increment(ref threadcompletesum);
        }

        public static List<Stock> GetStocks(string sql, Stocks stocks)
        {
            List<Stock> usstocks = new List<Stock>();
            DataTable dt = stocks.Gcfg.db.query(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
                usstocks.Add(stocks.StockByIndex(Convert.ToInt32(dr["id"].ToString())));
            return usstocks;
        }
        private void UpdateItem(List<Stock> usstocks, Boolean bUpdateUrl = true)
        {
        	if(usstocks.Count==0){
        		if (CompleteRun != null)
                CompleteRun();
        		return;
        	}
            if (bUpdateUrl)
            {
            	UrlItemTxt.Clear();
            	string sql = _xcfg.GetSqlUpdateUrlItemtxtTemplate();
            	if(usstocks.Count>500){
            		sql = sql.Replace("[IDS]","");
            		}else{
            		string ids = string.Join(",", usstocks.Select( r => r.ID));
            		sql = sql.Replace("[IDS]"," and A.id in(" + ids+")");
            	}
            	DataTable dt = _cfg.db.query(sql).Tables[0];	   
            	foreach(DataRow dr in dt.Rows)
            		UrlItemTxt[(int)dr["ID"]] = dr;
            }
            threadsum = 0;
            threadcompletesum = 0;
            int time = 0;
            if (usstocks.Count > 0)
            	CItem.web.GetOKUrl(UrlItemTxt[usstocks[0].ID]["url"].ToString());
            DateTime dt1 = System.DateTime.Now;
            foreach (Stock s in usstocks)
            {            	
            	string ItemTxt =(string) UrlItemTxt[s.ID]["Item"];
            	string UpdateUrl =(string ) UrlItemTxt[s.ID]["url"];
                while (threadsum == MaxThreadSum)
                {
                    time++;
                    Thread.Sleep(1);
                }
                Interlocked.Increment(ref threadsum);
                if (time > 10)
                {
                    TimeSpan ts = System.DateTime.Now.Subtract(dt1);
                    if (ts.TotalSeconds > 5)
                    {
                        showmsg("已完成：" + threadcompletesum + "");
                        time = 0;
                        dt1 = System.DateTime.Now;
                    }
                }
                if (threadcompletesum < MaxThreadSum)
                    Thread.Sleep(10);
                ThreadUpdateStock tus = new ThreadUpdateStock( UpdateUrl,ItemTxt,s,this);
                System.Threading.Thread nonParameterThread = new Thread(new ThreadStart(tus.Run));
                nonParameterThread.Start();
            }
            while (threadsum > 0)
                Thread.Sleep(10);
            if (CompleteRun != null)
                CompleteRun();
        }
        private List<Stock> GetItemUnUpdateStocks()  //xcfg  放到外面 //forstockhexin
        {
            string sql = "select * from [ItemTableName] where UpdateNumber<>[UpdateNumber]  order by ID ";
            sql = sql.Replace("[UpdateNumber]", TimeStringTools.UpdateNumber())
                     .Replace("[ItemTableName]", "Item" + _xcfg.DstTableName);
            return GetStocks(sql, _stocks);
        }
        private void InitTag(List<Stock> stocks)
        {
            for (int i = 0; i < 2000; i++) Tag[i].value = -100;
            foreach (Stock s in stocks) Tag[s.ID].value = 0;
        }
        private void UpdateUpdateNumber(string tablename)
        {
            string ids = string.Join(",", Tag.Where(r => r.value == 1).Select(r => r.index));
            string sql = "update [tablename] set UpdateNumber = [updatenumber] where id in ([ids])"
                         .Replace("[tablename]", tablename)
                         .Replace("[updatenumber]", TimeStringTools.UpdateNumber())
                         .Replace("[ids]", ids);
            if (ids != "")
                _cfg.db.update(sql);
        }

        public int MaxThreadSum;
        public int threadsum;
        public int threadcompletesum;
        public tagkdstock[] Tag;
        public List<Stock> BounsStocks;
        public ShowDeleGate showmsg;
        public CompleteDeleGate CompleteRun;
        public bool bshowtimeout;
        private Stocks _stocks;
        private GlobalConfig _cfg;
        private XmlConfig _xcfg;
        private string _exceptionfilename;
        private string _XmlFileName;
        private static Regex regex = new Regex("<td>PE\\(动\\).*?</td>|<td>市净率.*?</td>|<td>总值.*?</td>|<td>流值.*?</td>|<a.*?>|</a>|[ \t]+", RegexOptions.IgnoreCase);
     	private Dictionary<int,DataRow> UrlItemTxt = new Dictionary<int, DataRow>();
    }
    public class ThreadUpdateStock
    {
        public ThreadUpdateStock(string UpdateUrl, string ItemTxt, Stock s,ThreadUpdateStocks tuss)
        {
            this.UpdateUrl = UpdateUrl;
            this.ItemTxt = ItemTxt;
            this.s = s;
            this.tuss = tuss;
        }
        public void Run()
        {
            tuss.Run(UpdateUrl, ItemTxt, s);
        }
        private Stock s;
        private string ItemTxt;
        private string UpdateUrl;
        private ThreadUpdateStocks tuss;
    }
	public class SaveKdTag{
        public SaveKdTag(DateTime StoreDate, int length)
        {
            this.StoreDate = StoreDate;
			this.Tag =new tagkdstock[length];
		}
		public DateTime StoreDate{get;set;}
		public tagkdstock[] Tag{get;set;}		
		public void Save(string path)
		{
			MFile.WriteAllText( path,JsonConvert.SerializeObject(this));
		}

        public  void Init(SaveTag saveTag)
        {
            Tag = new tagkdstock[saveTag.Tag.Length];
            for(int i=0; i<Tag.Length; i++)
                Tag[i] = new tagkdstock();
            foreach (Tagstock s in saveTag.Tag)
            {               
                if (s.s != null)               
                {
                    if (s.index == 0)
                    {
                        //MessageBox.Show(s.s.Name);
                    }
                    Tag[s.index].Init(s.s);
                    if (s.Tag != null)
                        Tag[s.index].kd = (List<KData>)s.Tag;
                   
                }
            }
        }
    }
	public class tagkdstock{
		public tagkdstock(){
			index = 0;
			value = -100;
			s = null;
			/// ///
			txt = "";
			
		}
		public tagkdstock(Stock s){
			Init(s);
		}
		public void Init(Stock s){
			this.s = s;
			index = s.ID;
			value = 0;			
		}
		public List<KData> kd{get;set;}
	    public int index;
		public int value;
        [JsonIgnore]
		public Stock s;    // for initlize
        [JsonIgnore]
		public string txt;	 // for out debug infor
	}
}
