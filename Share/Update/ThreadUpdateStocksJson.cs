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
	public class ThreadUpdateStocksJson{
		public ThreadUpdateStocksJson(Stocks stocks,ThreadUpdateStocksConfig TSC)
	    {
	        bshowtimeout = false;
            _tsc = TSC;
	        _stocks = stocks;
	        _cfg = _stocks.Gcfg;
	        _exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";         
	        DealStocks = new List<Stock>();
	        MaxThreadSum = TSC.MaxThreadSum;
	        Tag = new tagstock[2000];
	        for (int i = 0; i < 2000; i++)
	            Tag[i] = new tagstock();
	        foreach (Stock s in _stocks.stocks)
	            Tag[s.ID].Init(s);           
	        showmsg=null;
	    }		
        public void DownLoadJsonData(Stock s)
        {
            string url = _tsc.UrlTemplate.Replace("[stockcode]", s.Code.ToLower());
            string txt = CWeb.GetWebClient(url);
            try
            {
                txt = CutJsonStringHead(txt, _datetpye);
                Tag[s.ID].kd = ConstructKdata(s.Code, txt);
                Tag[s.ID].value = 1;
            }
            catch (Exception e)
            {
                Tag[s.ID].txt = txt;
                Tag[s.ID].value = -2;
                MFile.AppendAllText("UpdatePrice.log", s.ID + "  " + Tag[s.ID].txt + "\t" + e.Message + "\r\n\r\n");
            }
        }
		private void UpdateItem(List<Stock> usstocks)
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
	    public List<Stock> DealStocks;
	    public ShowDeleGate showmsg;
	    public bool bshowtimeout;
	    private string _exceptionfilename;
	    public tagstock[] Tag;
        public ThreadUpdateStocksConfig _tsc;
        public ThreadUpdateStocksConfig TSC{get {return _tsc;}}
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
        public ThreadUpdateStocksConfig(string datetype, string daylength)
        {
            UrlT  = new ThreadUpdateUrlTemplate(datetype);
            _daylength = daylength;
            MaxThreadSum = 1;
        }
        public string UrlTemplate{get {return UrlT.UrlTemplate.Replace("[dayscount]",_daylength);}}
        public ThreadUpdateUrlTemplate UrlT;
        public ThreadUpdateJsonFiles Files;
        private string _daylength;
        public int MaxThreadSum { get; set; }
    }
    public class ThreadUpdateUrlTemplate
    {        
        private string _datetype;
        private string _Datetype;
        private string urlt;
        public ThreadUpdateUrlTemplate(string datetype)
        {
            this._datetype = datetype;
            if (!"|dayly|monthly|weekly|".Contains("|" + datetype + "|"))
                datetype = "dayly";
            if (_datetype == "weekly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_weekqfq&param=[stockcode],week,,,[dayscount],qfq";
            if (_datetype == "monthly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_monthqfq&param=[stockcode],month,,,[dayscount],qfq";
            if(_datetype == "dayly")
                 urlt =  @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_day&param=[stockcode],day,,,[dayscount],bfq";
        } 
        public string DateType{ get {return _Datetype;}}
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
}
