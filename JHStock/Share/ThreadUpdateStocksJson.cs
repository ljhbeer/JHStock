using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using JHStock.Update;
using Tools;
using ToolsCXml;
namespace JHStock
{
    public delegate void DeleteFormatData(Tagstock tagstock, Stock s, string txt);    
    public class FormatDataFunction
    {
        private string _datetype;
        public FormatDataFunction(string datetype)
        {
            if (datetype.EndsWith("ly"))
                datetype = datetype.Replace("ly", "");
            this._datetype = datetype;
        }
        public void FormatKDData(Tagstock tagstock, Stock s, string txt)
        {
            txt = CutJsonStringHead(txt);
            tagstock.Tag = ConstructKdata(s.Code, txt);
            tagstock.value = 1;
        }
        public static List<KData> ConstructKdata(string stockcode, string txt)
        {
            QQStocks qs = JsonConvert.DeserializeObject<QQStocks>(txt);
            List<KData> kd = qs.data[stockcode.ToLower()].day
               .Select(r =>
               {
                   KData k = new KData();
                   k.date = Convert.ToInt32(r[0].ToString().Replace("-", ""));
                   k.open = (int)(Convert.ToSingle(r[1]) * 100);
                   k.close = (int)(Convert.ToSingle(r[2]) * 100);
                   k.high = (int)(Convert.ToSingle(r[3]) * 100);
                   k.low = (int)(Convert.ToSingle(r[4]) * 100);
                   k.vol = (int)(Convert.ToSingle(r[5]) * 100);
                   return k;
               }).ToList();
            return kd;
        } 
        private string CutJsonStringHead(string txt)
        {
            if (txt.IndexOf("=") != -1)
                txt = txt.Substring(txt.IndexOf("=") + 1);
            txt = txt.Replace(_datetype, "day");
            txt = txt.Replace("qfqday", "day");
            return txt;
        }
    }
	public class ThreadUpdateStocksJson{
		public ThreadUpdateStocksJson(Stocks stocks,ThreadUpdateStocksConfig TSC)
	    {
	        bshowtimeout = false;
            _tsc = TSC;
	        _stocks = stocks;
	        _cfg = _stocks.Gcfg;
	        _exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";         
	        
	        MaxThreadSum = TSC.MaxThreadSum;
	        Tag = new Tagstock[2000];
	        for (int i = 0; i < 2000; i++)
	            Tag[i] = new Tagstock();
	        foreach (Stock s in _stocks.stocks)
	            Tag[s.ID].Init(s);           
	        showmsg=null;
	    }		
        public void DownLoadJsonData(Stock s)
        {
            string url = _tsc.UrlTemplate.Replace("[stockcode]", s.Code.ToLower());
            string txt = CWeb.GetWebClient(url,"utf-8");
            try
            {
                TSC.FormatData(Tag[s.ID], s, txt);               
                Tag[s.ID].value = 1;
            }
            catch (Exception e)
            {
                Tag[s.ID].txt = txt;
                Tag[s.ID].value = -2;
                MFile.AppendAllText("UpdatePrice.log", s.ID + "  " + Tag[s.ID].txt + "\t" + e.Message + "\r\n\r\n");
            }
        }
		public void UpdateItem(List<Stock> usstocks)
		{
			threadsum = 0;
			threadcompletesum=0;	
			int time = 0;		
			DateTime dt1 = System.DateTime.Now;	
			foreach(Stock s in usstocks){
				while(threadsum==MaxThreadSum){
					time++;
					Thread.Sleep(1);
				}
				Interlocked.Increment(ref threadsum);
				if( time>10){
					TimeSpan ts = System.DateTime.Now.Subtract(dt1);
					if(ts.TotalSeconds>5){
						showmsg("已完成："+threadcompletesum+"");
						time=0;
						dt1 = System.DateTime.Now;
					}
				}
				if(threadcompletesum < MaxThreadSum)
					Thread.Sleep(10);
				ThreadUpdateStockJson tus = new ThreadUpdateStockJson(s,this);
				System.Threading.Thread nonParameterThread = new Thread(new ThreadStart(tus.Run));
				nonParameterThread.Start();
			}
			while(threadsum > 0 )
				Thread.Sleep(10);
            if(showmsg!=null)
            showmsg("已完成：" + threadcompletesum + "");
		}
        private Stocks _stocks;
	    private GlobalConfig _cfg;   
	    public int MaxThreadSum;
	    public int threadsum;
	    public int threadcompletesum;
	    public ShowDeleGate showmsg;
	    public bool bshowtimeout;
	    private string _exceptionfilename;
	    public Tagstock[] Tag;
        public ThreadUpdateStocksConfig _tsc;
        public ThreadUpdateStocksConfig TSC{get {return _tsc;}}
        public List<Stock> Stocks { get { return _stocks.stocks; } }
    }
	public class ThreadUpdateStockJson{
    	private Stock s;
    	private ThreadUpdateStocksJson _parent;
    	public ThreadUpdateStockJson(Stock s,ThreadUpdateStocksJson parent){
    		this.s = s;
    		this._parent = parent;
    	}
    	public void Run(){
    		try{
    			_parent.DownLoadJsonData(s);
    		}catch(Exception e){
    			MFile.AppendAllText(_parent.TSC.Files.ChildErrorlog,s.ID+"\t"+e.Message+"\r\n");
    		}
    		Interlocked.Decrement(ref _parent.threadsum);
			Interlocked.Increment(ref _parent.threadcompletesum);
    	}
    }
    public class ThreadUpdateStocksConfig
    {
        public ThreadUpdateStocksConfig(string datetype, int daylength)
        {
            UrlT  = new ThreadUpdateUrlTemplate(datetype);
            _daylength = daylength.ToString();
            MaxThreadSum = 1;
            FormatData = null;
            Debug = false;
        }
        public string UrlTemplate{get {return UrlT.UrlTemplate.Replace("[dayscount]",_daylength);}}
        public ThreadUpdateUrlTemplate UrlT;
        public ThreadUpdateJsonFiles Files;
        private string _daylength;
        public int MaxThreadSum { get; set; }
        public DeleteFormatData FormatData { get; set; }

        public bool Debug { get; set; }
    }
    public class ThreadUpdateUrlTemplate
    {        
        private string _datetype;
        private string urlt;
        public ThreadUpdateUrlTemplate(string datetype)
        {
            this._datetype = datetype;
            if (!"|dayly|monthly|weekly|maincwfx".Contains("|" + datetype + "|"))
                datetype = "dayly";
            if (_datetype == "weekly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_weekqfq&param=[stockcode],week,,,[dayscount],qfq";
            if (_datetype == "monthly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_monthqfq&param=[stockcode],month,,,[dayscount],qfq";
            if(_datetype == "dayly")
                 urlt =  @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_day&param=[stockcode],day,,,[dayscount],bfq";
            if (_datetype == "maincwfx")
                urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/MainTargetAjax?type=[type]&code=[stockcode]";
            if (_datetype == "DubangAnalysis")
            {
                urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/DubangAnalysisAjax?code=[stockcode]";
                urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/zcfzbAjax?companyType=4&reportDateType=0&reportType=1&endDate=&code=SH600856";
                urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/lrbAjax?companyType=4&reportDateType=0&reportType=1&endDate=&code=SH600856";
                urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/xjllbAjax?companyType=4&reportDateType=0&reportType=1&endDate=&code=SH600856";
                urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/PercentAjax_Index?companyType=4&reportDateType=0&reportType=1&endDate=&code=SH600856";
                urlt = @"http://f10.eastmoney.com/NewFinanceAnalysis/PercentAjax?companyType=4&reportDateType=0&reportType=1&endDate=&code=SH600856";
            }
        }
        public  void SetParam(string src, string dst)
        {
            urlt = urlt.Replace(src, dst);
        }
        public string UrlTemplate {get {return urlt;}}

    }
    public class ThreadUpdateJsonFiles
    {
        public ThreadUpdateJsonFiles()
        {
            ChildErrorlog = "QQDaylyError.log";
        }
        private ToolsCXml.BETag btyear = new ToolsCXml.BETag("[nflb\":#@#@-@#@#]");
        private ToolsCXml.BETag btcheck = new ToolsCXml.BETag("[:#@#@{-}@#@#}]");

        public string ChildErrorlog { get; set; }
    }
    
    public class Tagstock
    {
        public Tagstock()
        {
            index = 0;
            value = -100;
            s = null;
            txt = "";
            TagClass = "";
        }
        public Tagstock(Stock s)
        {
            Init(s);
        }
        public void Init(Stock s)
        {
            this.s = s;
            index = s.ID;
            value = 0;
        }
        public Object Tag { get; set; }
        public int index;
        public int value;
        public string TagClass;
        [JsonIgnore]
        public Stock s;    // for initlize
        [JsonIgnore]
        public string txt;	// for out debug infor
    }
    public class SaveTag
    {
        public SaveTag(DateTime StoreDate, Tagstock[] Tag)
        {
            this.StoreDate = StoreDate;
            this.Tag = Tag;
        }
        public DateTime StoreDate { get; set; }
        public Tagstock[] Tag { get; set; }
        public void Save(string path)
        {
            MFile.WriteAllText(path, JsonConvert.SerializeObject(this));
        }
    }
}
