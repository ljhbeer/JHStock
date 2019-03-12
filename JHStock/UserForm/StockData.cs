using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using JHStock.Update;
using Newtonsoft.Json;
using Tools;

namespace JHStock
{
    public delegate void ActionDeleGate(string action);

	public class StocksData
	{
		public StocksData(JSConfig _jscfg)
		{
			this._jscfg = _jscfg;
			this._stocks = _jscfg.globalconfig.Stocks;
			_ssestock = new Stock(0, "SH000001", "上证指数", _jscfg.globalconfig);
			isrunning = false;
			Msg = "";
            nkd = null;
			activeKD = new KData[2000];
			TempDaysCount = 0;
			DaysLength = 84;
            _netdate = new NetDate(DaysLength, _jscfg.KdataType);
            _locatepricedata = "data\\"+_jscfg.KdataType+"_ExpPrice.dat";
		}
		public bool HasKdata(int sID)
		{
			if (_savetag.Tag != null && _savetag.Tag[sID] != null)
				return true;
			return false;
		}
		public List<KData> GetKD(int sID)
		{
			if (HasKdata(sID))
				return _savetag.Tag[sID].kd;
			return new List<KData>();
		}
		
        public string InitData() //必须等待完成
        {
            _netdate.Refresh();
            if (!_netdate.Inline)
            {
                if (LoadLocalData())
                    return "OK:断网状态，使用本地数据统计，本地数据最新日期为" + _savetag.StoreDate.ToShortDateString(); // +本地数据最新日期
                else
                    return "Quit:断网且无本地数据，无法工作，请退出";
            }
            else
            {
                GetExChangeData();
                if (!LoadLocalData()) //下载完整数据
                {
                    DownLoadNetDataAndCheckSave(DaysLength); //由 DaysLength
                    return "OK: 已下载完整数据并替换，当前数据日期为" + _savetag.StoreDate.ToShortDateString(); // +本地数据最新日期
                }
                else
                {
                    if (_savetag.Tag[0] == null || _savetag.Tag[0].kd == null || _savetag.Tag[0].kd.Count == 0)//本地数据为空 或者格式不对
                    {
                        DownLoadNetDataAndCheckSave(DaysLength); //由 DaysLength
                        return "OK: 已下载完整数据并替换，当前数据日期为"; // +本地数据最新日期
                    }
                    List<int> LocalListDate = _savetag.Tag[0].kd.Select(r => r.date).ToList();
                    int LocalNearestDate = LocalListDate.Max();
                    int cnt = _netdate.ListHistoryDate.Where(r => r > LocalNearestDate).Count();                   
                    if (cnt == 0)
                    {
                        return "OK:本地数据已经最新，当前数据日期为"+_savetag.StoreDate.ToShortDateString(); // +本地数据最新日期
                    }
                    else
                    { //下载部分数据
                        DownLoadNetDataAndCheckSave(cnt);
                        return "OK:已下载并合并至最新数据，，当前数据日期为"+_savetag.StoreDate.ToShortDateString(); // +本地数据最新日期
                    }
                }

                //下载当天数据
            }
            //return "error 逻辑错误，此处不应出现";
        }
        public void GetExChangeData()
        {
            if (_netdate.IncludeToday && _netdate.Exchanging)
            {
                ActionMsg("showexchangingtime-" + DateTime.Now.ToLongTimeString());
                activeKD  = new KData[2000]; // struct 初始默认值为 000....
                CActivePrices _activeprices = new CActivePrices(_jscfg.globalconfig.Stocks);
                string html = _activeprices.ActivePriceFromNet();                
                string[] item = html.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in item)
                {
                    Stock s = _stocks.StockByNumCode(str.Trim().Substring(2, 6));
                    if (str.Contains(s.Name) && str.Contains("\t"))
                    {
                        ActivePrice ap = new ActivePrice(str);
                        activeKD[s.ID].open = (int)(100* ap.o);
                        activeKD[s.ID].close = (int)(100* ap.c);
                        activeKD[s.ID].high = (int)(100* ap.h);
                        activeKD[s.ID].low = (int)(100* ap.l);
                        activeKD[s.ID].amount = (int)(100* ap.amount);
                        activeKD[s.ID].vol = (int)(ap.vol);
                        //activeKD[s.ID].date = (int)(100 * ap.d);
                        activeKD[s.ID].reservation = 0;
                        
                    }
                }
            }
        }
        private bool LoadLocalData( )
		{
			Msg="";
			if(!File.Exists(_jscfg.baseconfig.WorkPath + _locatepricedata)){
				Msg = "本地记录不存在";
				return false;
			}
			string txt = File.ReadAllText(_jscfg.baseconfig.WorkPath + _locatepricedata);
			_savetag = JsonConvert.DeserializeObject<SaveTag>(txt);
			if(_savetag.Tag.Count()!=2000){
				Msg = "本地数据记录有误，或者格式不对，请从新下载生成";
				return false;
			}
			return true; //应改为 false
		}

        private void DownLoadNetDataAndCheckSave(int Dayscount)
        {
            this.TempDaysCount = Dayscount;
            DownDataFromNet(ThreadShowMsg, CompleteRun);            
        }
        private void DownDataFromNet(ShowDeleGate ThreadShowMsg, CompleteDeleGate ThreadCompleteRun) //等待线程结束 才返回
        {           
            if (!isrunning)
            {
                isrunning = true;
                nkd = new NetKData(_jscfg);
                nkd.ThreadShowMsg = ThreadShowMsg;
                nkd.CompleteRun = ThreadCompleteRun;
                nkd.DaysCount = TempDaysCount;
                System.Threading.Thread nonParameterThread =
                    new Thread(new ThreadStart(nkd.GetNetKData));
                nonParameterThread.Start();
            }
            while (isrunning)
                Thread.Sleep(100);

        }
        public void CompleteRun() //数据的处理合并
        {
            string Msg="\r\n MergeData Error: ";
            int mergedays = nkd.netsaveTag.Tag[0].kd.Count - 1;
            //处理 网上下载数据
            if (_netdate.IncludeToday ) //交易日 
               for (int i = 0; i < 2000; i++)
                   if (nkd.netsaveTag.Tag[i].kd != null && nkd.netsaveTag.Tag[i].kd.Count == mergedays + 1)
                   {
                       if(_netdate.Exchanging)
                            nkd.netsaveTag.Tag[i].kd = nkd.netsaveTag.Tag[i].kd.Take(mergedays).ToList();
                       else //if( mergedays == DaysLength)  //15:00后可以  更新所有数据                 //15:00 后要保留当天数据，因而要删除前一天
                           nkd.netsaveTag.Tag[i].kd = nkd.netsaveTag.Tag[i].kd.Skip(1).Take(mergedays).ToList();

                   }
                   else if (nkd.netsaveTag.Tag[i].kd != null)
                       Msg += nkd.netsaveTag.Tag[i].s.Name + nkd.netsaveTag.Tag[i].s.Code;
            mergedays = nkd.netsaveTag.Tag[0].kd.Count;
            
            int beginday = nkd.netsaveTag.Tag[0].kd[0].date;
            int endday = nkd.netsaveTag.Tag[0].kd[mergedays-1].date;
            
            if (mergedays == DaysLength)
                _savetag = nkd.netsaveTag;
            else
            {
            	// 合并
                for (int i = 0; i < 2000; i++)
                    if (_savetag.Tag[i].kd != null && nkd.netsaveTag.Tag[i].kd.Count == mergedays)
                    { //合并kd 
	                	if( nkd.netsaveTag.Tag[i].kd[0].date == beginday && nkd.netsaveTag.Tag[i].kd[mergedays-1].date == endday){
	                        _savetag.Tag[i].kd = _savetag.Tag[i].kd.Skip(mergedays).ToList();
	                        _savetag.Tag[i].kd.AddRange( nkd.netsaveTag.Tag[i].kd);
                		}else{
                			int maxlocalday = _savetag.Tag[i].kd.Max(r => r.date);
                			KData[] kd = nkd.netsaveTag.Tag[i].kd.Where( r => r.date>maxlocalday).ToArray();
                			if(kd.Length>0){
                				_savetag.Tag[i].kd = _savetag.Tag[i].kd.Skip(kd.Length).ToList();
                				_savetag.Tag[i].kd.AddRange( kd );
                			}
                		}
                    }
            }
            _savetag.StoreDate = _netdate.NearestDate;
            _savetag.Save(_jscfg.baseconfig.WorkPath + _locatepricedata);	
            isrunning = false;
        }

        public SaveTag SaveTag
        {
            get { return _savetag; }			
		}
		//init localdate
		public List<int> SHLocalDate()
		{
			BaseConfig cfg = _jscfg.baseconfig;
			if (File.Exists(cfg.WorkPath + _locatepricedata)) {
				string txt = File.ReadAllText(cfg.WorkPath + _locatepricedata);
				int bpos = txt.IndexOf("\"Tag\":[");
				int epos = txt.IndexOf("}]}");
				if (bpos != -1 && epos != -1 && epos > bpos) {
					txt = txt.Substring(bpos, epos - bpos + 3);
					tagstock t = JsonConvert.DeserializeObject<tagstock>(txt);
					return t.kd.Select(r => r.date).ToList();
				}
			}
			return new List<int>();
		}		

		private JSConfig _jscfg;
		private Stocks _stocks;
		private Stock _ssestock;
		private SaveTag _savetag;
		private NetDate _netdate;
		private int DaysLength;  		//特定长度 84
		private int TempDaysCount;  // 临时记录长度
		private bool isrunning;		
        private NetKData nkd;
		public  string Msg;
        public KData[] activeKD; // 当前时间
        public ShowDeleGate ThreadShowMsg;
        public ActionDeleGate ActionMsg;
        private string _locatepricedata;
        public NetDate Netdate
        {
            get { _netdate.Refresh(); return _netdate; }
        }
    }
	public class NetDate{
		public NetDate(int DaysCount,string urltype="dayly"){
			this.dayscount = DaysCount;			
			Inline = false;
			IncludeToday = false;
            Exchanging = false;
            NearestDate = DateTime.Now;
            int daylength = ListHistoryDate.Count;
            if (urltype == "weekly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_weekqfq&param=[stockcode],week,,,[dayscount],qfq";
            if (urltype == "monthly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_monthqfq&param=[stockcode],month,,,[dayscount],qfq";
		}
        public void Refresh()
        {
            try
            {
                string url = urlt.Replace("[stockcode]", "sh000001").Replace("[dayscount]", dayscount.ToString());
                string txt = CWeb.GetWebClient(url).Substring(13);
                QQStocks qs = JsonConvert.DeserializeObject<QQStocks>(txt);
                _netszdate = qs.data["sh000001"].day.Select(
                    r => Convert.ToInt32(r[0].ToString().Replace("-", ""))).ToList();
                Inline = true;
                IncludeToday = false;
                Exchanging = false;
                if (_netszdate.Count  ==  dayscount + 1 )
                {
                    IncludeToday = true;
                    Exchanging = true;
                    txt = txt.Substring(0, txt.IndexOf("market")>0?txt.IndexOf("market"):0 );
                    if (txt.Contains("15:00:0"))
                        Exchanging = false;
                    if(Exchanging)
                        _netszdate = _netszdate.Take(_netszdate.Count - 1).ToList();
                    else
                        _netszdate = _netszdate.Skip(1).Take(_netszdate.Count - 1).ToList(); // 交易日的 15:00后  也被加入历史
                }

                int nd = _netszdate.Max();
                NearestDate = new DateTime(nd / 10000, nd / 100 % 100, nd % 100);
            }
            catch
            {
                Inline = false;
                IncludeToday = false;
                _netszdate = new List<int>();
            }
        }
		public bool Inline{get;set;}
		public bool IncludeToday{get;set;}
        public bool Exchanging {get;set;}
		public List<int> ListHistoryDate{
        	get{
        		if(_netszdate==null){
                    Refresh();
        		}
        		return _netszdate;
        	}
        }
		public int NearestHistoryDate{
			get{		
				if(ListHistoryDate.Count>0)
					return ListHistoryDate.Max();
				return 0;
			}		
		}
        public DateTime NearestDate;
        private List<int> _netszdate;
        private int dayscount;
        private string urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_dayqfq&param=[stockcode],day,,,[dayscount],qfq";
	}
}
