using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Tools;

namespace JHStock.Update
{    
    public delegate void ShowDeleGate(string file);
	public delegate void CompleteDeleGate( );
	public class ThreadUpdateStocksQQFin{
		public ThreadUpdateStocksQQFin(Stocks stocks)
	    {
	        bshowtimeout = false;
	        _stocks = stocks;
	        _cfg = _stocks.Gcfg;
	        _exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";         
	        DealStocks = new List<Stock>();
	        MaxThreadSum = 1;
	        Tag = new tagkdstock[2000];
	        for (int i = 0; i < 2000; i++)
	            Tag[i] = new tagkdstock();
	        foreach (Stock s in _stocks.stocks)
	            Tag[s.ID].Init(s);
	        UpdateItemNames = new List<string>(){"MGZB","YLNL","YYNL","CZNL","DJCW"};
	        UrlTemplate = "http://comdata.finance.gtimg.cn/data/[itemname]/[scode]/[year]";
	        nowyear = DateTime.Now.Year.ToString();
	    }
		public void RunNetDownLoadData()
	    {
	    	DateTime dt1 = System.DateTime.Now;        	
	    	UpdateItem(DealStocks);
	    	DateTime dt2 = System.DateTime.Now;
			TimeSpan ts = dt2.Subtract(dt1);
			if(bshowtimeout)
				MessageBox.Show ( "时间"+ts.TotalSeconds);        	 
	    }  	 
	    public void DownLoadData(Stock s)
	    {
	        string u = UrlTemplate.Replace("[scode]",s.Code);
	        StringBuilder strall = new StringBuilder();
	        string finfilepath = _cfg.Baseconfig.WorkPath+ "data\\QQFin\\" + s.Code  + ".txt";
	        string allfiletxt = File.ReadAllText(finfilepath);
	        {
	           string itemname = UpdateItemNames[0];
	           string ui = u.Replace("[itemname]", itemname).ToLower();
	           string url = ui.Replace("[year]", nowyear);
	           string yearstr = CWeb.GetWebClient(url);
	           string checktxt = btcheck.BEPos(yearstr).String;
	           if(allfiletxt.Contains(checktxt))
	            	return; //不更新
	        }
	        
	        foreach (string itemname in UpdateItemNames)
	        {
	        	string r = "(?<="+itemname+"=\\[)[^=]*(?=])";
	        	string alltxt = Regex.Match(allfiletxt,r).Value;
	        		
	            ToolsCXml.BETag btItem =  new ToolsCXml.BETag("[item\":#@#@-@#@#]".Replace("item",itemname.ToLower()));
	            string ui = u.Replace("[itemname]", itemname).ToLower();
	            string url = ui.Replace("[year]", nowyear);
	            string yearstr = CWeb.GetWebClient(url);
	        
	            yearstr = btyear.BEPos(yearstr).String.Replace("\"","");
	            string[] years = yearstr.Split(',');
//	            StringBuilder stritem = new StringBuilder();
//	            foreach (string year in years)
//	            {
//	                url = ui.Replace("[year]", year);
//	                string txt = CWeb.GetWebClient(url);
//	                txt =  btItem.BEPos(txt).String	 ;               
//	                stritem.Append( txt );
//	            }
	            
	            StringBuilder stritem1 = new StringBuilder();
	            foreach (string year in years)
	            {
	                url = ui.Replace("[year]", year);
	                string txt = CWeb.GetWebClient(url);
	                txt =  btItem.BEPos(txt).String;
					string[] items = txt.Split(new string[]{"},{","{","}"},StringSplitOptions.RemoveEmptyEntries);
					bool bcontains = false;
					foreach(string str in items)
						if(alltxt.Contains(str))
							bcontains = true;
					if(bcontains){
	                	txt = "";
	                	foreach(string str in items)
	                		if(!alltxt.Contains(str))
	                			txt+="{"+str+"}";
	                		else
	                			break;
	                	stritem1.Append(txt);
	                	stritem1.Append(alltxt);
	                	
//	                	MFile.WriteAllText("merger.txt",stritem1.ToString().Replace("}{","},{").Replace("},{","},\r\n{"));
//	                	MFile.WriteAllText("whole.txt" ,stritem.ToString().Replace("}{","},{").Replace("},{","},\r\n{"));
	                	break;	                	
	                }	                		                	
	                stritem1.Append( txt );
	            }
	            
	            strall.Append(itemname + "=[").Append(stritem1).AppendLine("]");	
	        }
	        MFile.WriteAllText(finfilepath, strall.Replace("}{","},{").ToString());
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
				ThreadUpdateStockQQFin tus = new ThreadUpdateStockQQFin(s,this);
				System.Threading.Thread nonParameterThread = new Thread(new ThreadStart(tus.Run));
				nonParameterThread.Start();
			}
			while(threadsum > 0 )
				Thread.Sleep(10);
			if(CompleteRun!=null)
				CompleteRun();	
		}	
			
	    private Stocks _stocks;
	    private GlobalConfig _cfg;   
	    public int MaxThreadSum;
	    public int threadsum;
	    public int threadcompletesum;
	    public List<Stock> DealStocks;
	    public ShowDeleGate showmsg;
        public CompleteDeleGate CompleteRun;
	    public bool bshowtimeout;
	    private List<string> UpdateItemNames;
	    private string _exceptionfilename;
	    public tagkdstock[] Tag;
	    
	    private ToolsCXml.BETag btyear = new ToolsCXml.BETag("[nflb\":#@#@-@#@#]");
	    
	    private ToolsCXml.BETag btcheck = new ToolsCXml.BETag("[:#@#@{-}@#@#}]");
	    private string UrlTemplate;
	    private string nowyear;
	}
	public class ThreadUpdateStockQQFin{
    	private Stock s;
    	private ThreadUpdateStocksQQFin qqfin;
    	public ThreadUpdateStockQQFin(Stock s,ThreadUpdateStocksQQFin qqfin){
    		this.s = s;
    		this.qqfin = qqfin;
    	}
    	public void Run(){
    		try{
    			qqfin.DownLoadData(s);
    		}catch(Exception e){
    			MFile.AppendAllText("QQFinError.log",s.ID+"\t"+e.Message+"\r\n");
    		}
    		Interlocked.Decrement(ref qqfin.threadsum);
			Interlocked.Increment(ref qqfin.threadcompletesum);
    	}
    }
}
