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
	public class ThreadUpdateStocksQQDayly{
		public ThreadUpdateStocksQQDayly(Stocks stocks,int Daylength)
	    {
	        bshowtimeout = false;
	        _stocks = stocks;
	        _cfg = _stocks.Gcfg;
	        _exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";         
	        DealStocks = new List<Stock>();
	        MaxThreadSum = 1;
	        Tag = new tagstock[2000];
	        for (int i = 0; i < 2000; i++)
	            Tag[i] = new tagstock();
	        foreach (Stock s in _stocks.stocks)
	            Tag[s.ID].Init(s);
	        UrlTemplate = urlt.Replace("[daylength]",Daylength.ToString());
	        showmsg=null;
	    }
		public void RunNetDownLoadData()
	    {
	    	DateTime dt1 = System.DateTime.Now;        	
	    	UpdateItem(DealStocks);
	    	List<Stock> stocks = DealStocks;
	    	int stockcountb = stocks.Count;
			stocks = Tag.Where( r=> r.value <1 && r.value>-10).Select(r=>r.s).ToList();	
			if(_cfg.Debug){
				int stockcounte = stocks.Count;
				int stockcountc = Tag.Where(r => r.value==1).Count();
				MFile.AppendAllText( "Update.log",System.DateTime.Now.ToShortTimeString()+" 更新ItemstockHexin "+
				                    stockcountc+"/"+stockcountb+" 其中有 "+stockcounte+"需要再次更新\r\n");
			}
			if(stocks.Count>0){
				UpdateItem(stocks);
				
				if(_cfg.Debug){
					int stockcounte = stocks.Count;
					int stockcountc = Tag.Where(r => r.value==1).Count();
					MFile.AppendAllText( "Update.log",System.DateTime.Now.ToShortTimeString()+" 再次更新ItemstockHexin "+
					                    stockcountc+"/"+stockcounte+"\r\n");
				}
			}
	    	DateTime dt2 = System.DateTime.Now;
			TimeSpan ts = dt2.Subtract(dt1);
			if(bshowtimeout)
				MessageBox.Show ( "时间"+ts.TotalSeconds);
	    }  	 
	    public void DownLoadData(Stock s)
	    {
	    	string url = UrlTemplate.Replace("[stockcode]",s.Code.ToLower());
	        StringBuilder strall = new StringBuilder();
	        string txt = CWeb.GetWebClient(url).Substring(10);
			try{
				 QQStocks qs = JsonConvert.DeserializeObject<QQStocks>( txt);
				 Tag[s.ID].kd = qs.data[s.Code.ToLower()].day
	        		.Select( r => {
	        		 KData k = 	new KData();
	        		 k.date =Convert.ToInt32( r[0].Replace("-",""));	
	        		 k.open = (int)(Convert.ToSingle(r[1])*100);
	        		 k.close = (int)(Convert.ToSingle(r[2])*100);
	        		 k.high= (int)(Convert.ToSingle(r[3])*100);
	        		 k.low = (int)(Convert.ToSingle(r[4])*100);
	        		 k.vol = (int)(Convert.ToSingle(r[4])*100);
	        		 return k;
	        		}).ToList();
				 Tag[s.ID].value = 1;
			}catch{
	        	Tag[s.ID].txt = txt;
	        	Tag[s.ID].value = -2;
//				MFile.AppendAllText("UpdatePrice.log",s.ID + "  "+ Tag[s.ID].txt+"\r\n\r\n");
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
				ThreadUpdateStockQQDayly tus = new ThreadUpdateStockQQDayly(s,this);
				System.Threading.Thread nonParameterThread = new Thread(new ThreadStart(tus.Run));
				nonParameterThread.Start();
			}
			while(threadsum > 0 )
				Thread.Sleep(10);
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
	    
	    private string UrlTemplate;
	    private ToolsCXml.BETag btyear = new ToolsCXml.BETag("[nflb\":#@#@-@#@#]");
	    private ToolsCXml.BETag btcheck = new ToolsCXml.BETag("[:#@#@{-}@#@#}]");
	    private string urlt =  @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_day&param=[stockcode],day,,,[daylength],bfq";
		
	}
	public class ThreadUpdateStockQQDayly{
    	private Stock s;
    	private ThreadUpdateStocksQQDayly QQDayly;
    	public ThreadUpdateStockQQDayly(Stock s,ThreadUpdateStocksQQDayly QQDayly){
    		this.s = s;
    		this.QQDayly = QQDayly;
    	}
    	public void Run(){
    		try{
    			QQDayly.DownLoadData(s);
    		}catch(Exception e){
    			MFile.AppendAllText("QQDaylyError.log",s.ID+"\t"+e.Message+"\r\n");
    		}
    		Interlocked.Decrement(ref QQDayly.threadsum);
			Interlocked.Increment(ref QQDayly.threadcompletesum);
    	}
    }
}
