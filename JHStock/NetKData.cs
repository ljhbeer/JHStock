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
        public NetKData(JSConfig _jscfg, Stock _ssestock)
        {
            this._jscfg = _jscfg;
            this._ssestock = _ssestock;
			this._stocks = _jscfg.globalconfig.Stocks;
        }
		public void GetNetKData(){  //check and DownLoad
			try{
				BaseConfig cfg = _jscfg.baseconfig;
                List<int> shlocaldate = new List<int>();
                //init localdate
                if (File.Exists(cfg.WorkPath + "data\\ExpPrice.dat"))
                {
                    string txt = File.ReadAllText(cfg.WorkPath + "data\\ExpPrice.dat");
                    int bpos =  txt.IndexOf( "\"Tag\":[");
                    int epos =  txt.IndexOf("}]}");
                    if(bpos!=-1 && epos!=-1 && epos>bpos)
                    {
                        txt = txt.Substring(bpos, epos - bpos + 3);
                        tagstock t = JsonConvert.DeserializeObject<tagstock>(txt);
                        shlocaldate = t.kd.Select(r => r.date).ToList();
                    }
                }
				List<int> shnetdate = _stocks.ShNetDate;
				int cnt = shnetdate.Intersect(shlocaldate).Count();
				if(cnt == 0 && shlocaldate.Count!=0){
                    ErrorMsg = "无法核对完整数据，请更新数据后再运行本程序(数据已经超过3个月没有更新，请删除 data\\ExpPrice.dat 后再试)";
					return;
				}

                //debug 102
                List<int> netexcept = shnetdate.Where(r => r > shlocaldate.Min()).ToList();
                netexcept = netexcept.Except(shlocaldate).ToList();  // 需要Debug
                //当天 Day 数据

                if (netexcept.Count == 0)
                    netexcept.Add(Convert.ToInt32(Tools.TimeStringTools.UpdateNumber()));

				bool bfromnet = true;
				if(File.Exists( cfg.WorkPath +"data\\ExpPrice.dat")){
					string txt = File.ReadAllText(cfg.WorkPath +"data\\ExpPrice.dat");
					SaveTag sst = JsonConvert.DeserializeObject<SaveTag>(txt);
                    //if(sst.now.Date == DateTime.Now.Date){
                    //    bfromnet = false;
                    //    this.Tag = sst.Tag;
                    //}
				}

				if(bfromnet){
					GetDataFromNet(cnt,netexcept);
                    // 获取上证指数
                    tagstock ssetag = ThreadUpdateStocksQQDayly.DownLoadData(this._ssestock,netexcept.Count);
                    ssetag.s = _ssestock;
                    Tag[_ssestock.ID] = ssetag;

                    //合并Tag 

					new SaveTag(DateTime.Now, Tag).Save(cfg.WorkPath+"data\\ExpPrice.dat");					
					//OutThreadMsg();
				}



                #region DownLoadAndWriteAllData
                /*
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
                    // 获取上证指数
                    tagstock ssetag = ThreadUpdateStocksQQDayly.DownLoadData(this._ssestock,netexcept.Count);
                    ssetag.s = _ssestock;
                    Tag[_ssestock.ID] = ssetag;
					new SaveTag(DateTime.Now, Tag).Save(cfg.WorkPath+"data\\ExpPrice.dat");					
					//OutThreadMsg();
				}

                ////*/
                #endregion
            }
            finally{
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
        private Stock _ssestock; 
	}
}
