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
using JHStock;

namespace JHStock
{
	public class NetKData
	{
        public NetKData(JSConfig _jscfg)
        {
            this._jscfg = _jscfg;
			this._stocks = _jscfg.globalconfig.Stocks;
			this.netsaveTag = null;
			DaysCount = 0;
            _ssestock = new Stock(0, "SH000001", "上证指数", _jscfg.globalconfig);
        }
		public void GetNetKData(){  //check and DownLoad
			try{
        		int dayscount = DaysCount;
        		if(dayscount < 1)
        			return;
				GetDataFromNet1(dayscount);
            }
            finally{
                CompleteRun();
			}
		}
        private void GetDataFromNet1(int dayscount)
        {
            string datetype = _jscfg.KdataType;
            ThreadUpdateStocksConfig TSC1 = new ThreadUpdateStocksConfig(datetype, dayscount);
            FormatDataFunction FD = new FormatDataFunction(datetype);
            TSC1.UrlT = new ThreadUpdateUrlTemplate(datetype);
            TSC1.Files = new ThreadUpdateJsonFiles();
            TSC1.MaxThreadSum = 50;
            TSC1.FormatData = FD.FormatKDData;
            ThreadUpdateStocksJson TS = new ThreadUpdateStocksJson(_stocks, TSC1);
            if (ThreadShowMsg != null)
               TS.showmsg = new ShowDeleGate(ThreadShowMsg);

            netsaveTag = new SaveTag(DateTime.Now, TS.Tag);

            List<Stock> Dealstocks = new List<Stock>(_stocks.stocks);

            //Dealstocks = Dealstocks.Take(10).ToList();
            // Add 上证指数
            TS.Tag[0].Init(_ssestock);
            Dealstocks.Add(_ssestock);

            TS.UpdateItem(Dealstocks );
            List<Stock> stocks = Dealstocks;
            int stockcountb = stocks.Count;

            
            stocks = TS.Tag.Where(r => r.value < 1 && r.value > -10 ).Select(r => r.s).ToList();
            if (stocks.Count > 0)
            {
                TS.UpdateItem(stocks);
            }
        }		
		private void GetDataFromNet(int dayscount){
			ThreadUpdateStocksQQDayly qf = new ThreadUpdateStocksQQDayly(_stocks,dayscount,_jscfg.KdataType);
			qf.MaxThreadSum = 50;
            if(ThreadShowMsg!=null)
			qf.showmsg = new ShowDeleGate(ThreadShowMsg);
            //List<Stock> ss = new List<Stock>(){
            //    _stocks.StockByIndex(2),
            //    _stocks.StockByIndex(3),
            //    _stocks.StockByIndex(4)
            //};
			qf.DealStocks = _stocks.stocks;
            // Add 上证指数
            qf.DealStocks.Add(_ssestock);
            qf.Tag[0].Init(_ssestock); 
            //netsaveTag = new SaveKdTag( DateTime.Now, qf.Tag);
			updatetime = DateTime.Now;
			qf.RunNetDownLoadData();
		}
		
		private JSConfig _jscfg;
		private Stocks _stocks;
		private DateTime updatetime;
		public string ErrorMsg;
		public SaveTag netsaveTag;
		public ShowDeleGate ThreadShowMsg;
        public CompleteDeleGate CompleteRun;
		public int DaysCount{ get; set; }

        //SSE 上证指数
        private Stock _ssestock;
	}
    public class ThreadUpdateStocksQQDaylyTest
    {
        public ThreadUpdateStocksQQDaylyTest(Stocks stocks, int Daylength, string datetype = "dayly")
        {
            bshowtimeout = false;
            showmsg = null;
            ThreadUpdateStocksConfig TSC1 = new ThreadUpdateStocksConfig(datetype, Daylength);
            FormatDataFunction FD = new FormatDataFunction(datetype);
            TSC1.UrlT = new ThreadUpdateUrlTemplate(datetype);
            TSC1.Files = new ThreadUpdateJsonFiles();
            TSC1.MaxThreadSum = 10;
            TSC1.FormatData = FD.FormatKDData;
            TS = new ThreadUpdateStocksJson(stocks, TSC1);
        }
        public void RunNetDownLoadData()
        {
            DateTime dt1 = System.DateTime.Now;
            List<Stock> DealStocks = new List<Stock>(TS.Stocks);
            TS.UpdateItem(DealStocks);
            List<Stock> stocks = DealStocks;
            int stockcountb = stocks.Count;
            stocks = TS.Tag.Where(r => r.value < 1 && r.value > -10).Select(r => r.s).ToList();
            if (TS.TSC.Debug)
            {
                int stockcounte = stocks.Count;
                int stockcountc = TS.Tag.Where(r => r.value == 1).Count();
                MFile.AppendAllText("Update.log", System.DateTime.Now.ToShortTimeString() + " 更新ItemstockHexin " +
                                    stockcountc + "/" + stockcountb + " 其中有 " + stockcounte + "需要再次更新\r\n");
            }
            if (stocks.Count > 0)
            {
                TS.UpdateItem(stocks);

                if (TS.TSC.Debug)
                {
                    int stockcounte = stocks.Count;
                    int stockcountc = TS.Tag.Where(r => r.value == 1).Count();
                    MFile.AppendAllText("Update.log", System.DateTime.Now.ToShortTimeString() + " 再次更新ItemstockHexin " +
                                        stockcountc + "/" + stockcounte + "\r\n");
                }
            }
            DateTime dt2 = System.DateTime.Now;
            TimeSpan ts = dt2.Subtract(dt1);
            if (bshowtimeout)
                MessageBox.Show("时间" + ts.TotalSeconds);
        }

        public ShowDeleGate showmsg;
        public bool bshowtimeout;
        private ThreadUpdateStocksJson TS;
    }
}
