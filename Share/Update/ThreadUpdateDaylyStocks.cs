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
using ToolsCXml;
using Tools;
using JHStock.Update;
namespace JHStock
{
	public class ThreadUpdateDaylyStocks{
		public ThreadUpdateDaylyStocks(Stocks stocks){
			bshowtimeout = false;
			_stocks  = stocks;
			_cfg = _stocks.Gcfg;
			_exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";
            //_XmlFileName = "";
			RunsStocks = new List<Stock>();
			MaxThreadSum = 1;
			Tag = new tagkdstock[2000];	
			for(int i=0; i<2000; i++)
				Tag[i] = new tagkdstock();
			foreach(Stock s in _stocks.stocks)
				Tag[s.ID].Init(s);
		}
		
		public void RunNetTime(bool bUpdateAllBounsAndTopten = false ) //unuse
		{
			DateTime dt1 = System.DateTime.Now;
		    if(true){
				#region hexin				
					List<Stock> stocks = _stocks.stocks;
					InitTag(stocks);
					UpdateItem(stocks);	
					int stockcountb = stocks.Count;
					stocks = Tag.Where( r=> r.value <1 && r.value>-10).Select(r=>r.s).ToList();	
					if(_cfg.Debug){
						int stockcounte = stocks.Count;
						int stockcountc = Tag.Where(r => r.value==1).Count();
						MFile.AppendAllText( "Update.log",System.DateTime.Now.ToShortTimeString()+" 更新ItemstockHexin "+
						                    stockcountc+"/"+stockcountb+" 其中有 "+stockcounte+"需要再次更新\r\n");
					}
					InitTag(stocks);
					UpdateItem( stocks,false);
					if(_cfg.Debug){
						int stockcounte = stocks.Count;
						int stockcountc = Tag.Where(r => r.value==1).Count();
						MFile.AppendAllText( "Update.log",System.DateTime.Now.ToShortTimeString()+" 再次更新ItemstockHexin "+
						                    stockcountc+"/"+stockcounte+"\r\n");
					}
				#endregion			
		    }
			DateTime dt2 = System.DateTime.Now;
			TimeSpan ts = dt2.Subtract(dt1);
			if(bshowtimeout)
			MessageBox.Show ( "时间"+ts.TotalSeconds);
		}
		private void UpdateItem(List<Stock> usstocks,Boolean bUpdateUrl=true){			
			
			if(bUpdateUrl){
//				_stocks.InitStocksUpdateUrl(_xcfg.GetSqlTemplate());	
//				_stocks.InitStocksItem(_xcfg.GetSqlItemtxtTemplate());	
			}
			threadsum = 0;
			threadcompletesum=0;	
			int time = 0;		
//			if(usstocks.Count>0)
//				CItem.web.GetOKUrl(usstocks[0].UpdateUrl);
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
				ThreadUpdateDaylyStock tus = new ThreadUpdateDaylyStock(s,this);
				System.Threading.Thread nonParameterThread = new Thread(new ThreadStart(tus.Run));
				nonParameterThread.Start();
			}
			while(threadsum > 0 )
				Thread.Sleep(10);
			if(CompleteRun!=null)
				CompleteRun();			
		}
		
		private void InitTag(List<Stock> stocks){
			for(int i=0; i<2000; i++)  Tag[i].value = -100;
			foreach(Stock s in stocks) Tag[s.ID].value = 0;
		}
		
		
		public int MaxThreadSum;		
		public int threadsum;
		public int threadcompletesum;
		public tagkdstock[] Tag;
		public List<Stock> RunsStocks;
		public ShowDeleGate showmsg;
		public CompleteDeleGate CompleteRun;
		public bool bshowtimeout;
		private Stocks _stocks;
		private GlobalConfig _cfg;
		private string _exceptionfilename;
        //private string _XmlFileName;
	}
}
