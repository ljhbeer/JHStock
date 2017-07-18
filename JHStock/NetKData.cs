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
				GetDataFromNet(dayscount);
            }
            finally{
                CompleteRun();
			}
		}		
		private void GetDataFromNet(int dayscount){
			ThreadUpdateStocksQQDayly qf = new ThreadUpdateStocksQQDayly(_stocks,dayscount);
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
			netsaveTag = new SaveTag( DateTime.Now, qf.Tag);
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
}
