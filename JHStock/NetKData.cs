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
		public NetKData( JSConfig jscfg)
		{
			this._jscfg = jscfg;
			this._stocks = _jscfg.globalconfig.Stocks;
		}
		public void GetNetKData(){  //check and DownLoad
			try{
				BaseConfig cfg = _jscfg.baseconfig;
				_stocks.ReloadListDate();
				List<int> shdate = _stocks.ListDate;
				List<int> shnetdate = _stocks.ShNetDate;
				int cnt = shnetdate.Intersect(shdate).Count();
				if(cnt == 0){
					ErrorMsg = "无法核对完整数据，请更新数据后再运行本程序";
					return;
				}

                //debug 102
				List<int> netexcept = shnetdate.Except(shdate).ToList();  // 需要Debug
                netexcept = shnetdate.Skip(254- 84).Take(84).ToList(); //modified 254 to 84
				bool bfromnet = true;
				if(File.Exists( cfg.WorkPath +"data\\ExpPrice.dat")){
					string txt = File.ReadAllText(cfg.WorkPath +"data\\ExpPrice.dat");
					SaveTag sst = JsonConvert.DeserializeObject<SaveTag>(txt);
					if(sst.now.Date == DateTime.Now.Date){
						bfromnet = false;
						this.Tag = sst.Tag;
					}
				}
				if(bfromnet){
					GetDataFromNet(cnt,netexcept);
					new SaveTag(DateTime.Now, Tag).Save(cfg.WorkPath+"data\\ExpPrice.dat");					
					//OutThreadMsg();
				}
			}finally{
                CompleteRun();
			}
		}
		
		private void GetDataFromNet(int locallength, List<int> Netexcept){
			ThreadUpdateStocksQQDayly qf = new ThreadUpdateStocksQQDayly(_stocks,Netexcept.Count);
			qf.MaxThreadSum = 50;
            if(ThreadShowMsg!=null)
			qf.showmsg = new ShowDeleGate(ThreadShowMsg);
            //List<Stock> ss = new List<Stock>(){
            //    _stocks.StockByIndex(2),
            //    _stocks.StockByIndex(3),
            //    _stocks.StockByIndex(4)
            //};
			qf.DealStocks = _stocks.stocks;
			this.Tag = qf.Tag;
			updatetime = DateTime.Now;
			qf.RunNetDownLoadData();
		}
		
		private JSConfig _jscfg;
		private Stocks _stocks;
		private DateTime updatetime;
		public string ErrorMsg;
		public tagstock[] Tag;
		public ShowDeleGate ThreadShowMsg;
        public CompleteDeleGate CompleteRun; 
	}
}
