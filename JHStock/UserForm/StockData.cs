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
            NeedDownLoadDays = 0;
            DaysLength = _jscfg.staticsconfig.KDataDaysCount;
            _netdate = new NetDate(DaysLength, _jscfg.KdataType);
            _locatepricedata = "data\\" + _jscfg.KdataType + "_ExpPrice.dat";
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
            {   //GetExChangeData();
                int mergersday = 0;
                if (NeedDownLoadAllData(ref mergersday)) // mergersday > 10 则重新下载全部
                {
                    DownLoadNetDataAndCheckSave(DaysLength); //由 DaysLength
                    return "OK: 已下载完整数据并替换，当前数据日期为" + _savetag.StoreDate.ToShortDateString(); // +本地数据最新日期
                }
                else
                {
                    if (mergersday == 0)
                        return "OK:本地数据已经最新，当前数据日期为" + _savetag.StoreDate.ToShortDateString();
                    else
                    {
                        DownLoadNetDataAndCheckSave(mergersday);
                        return "OK:已下载并合并至最新数据，，当前数据日期为" + _savetag.StoreDate.ToShortDateString(); // +本地数据最新日期
                    }
                }
            }
        }
        private bool NeedDownLoadAllData(ref int mergersday)
        {
            if (!LoadLocalData()) //下载完整数据
                return true;
            if (_savetag.Tag[0] == null || _savetag.Tag[0].kd == null || _savetag.Tag[0].kd.Count == 0)
                return true;
            List<int> LocalListDate = _savetag.Tag[0].kd.Select(r => r.date).ToList();
            int LocalNearestDate = LocalListDate.Max();
            int cnt = _netdate.ListHistoryDate.Where(r => r > LocalNearestDate).Count();
            if (LocalListDate.Count != _netdate.ListHistoryDate.Count)
                return true;
            mergersday = cnt;
            if (mergersday > 10) //10个周期没有更新，则全部重新更新
                return true;
            return false;
        }
        public void GetExChangeData()
        {
            if (_netdate.IncludeToday && _netdate.Exchanging)
            {
                ActionMsg("showexchangingtime-" + DateTime.Now.ToLongTimeString());
                activeKD = new KData[2000]; // struct 初始默认值为 000....
                CActivePrices _activeprices = new CActivePrices(_jscfg.globalconfig.Stocks);
                string html = _activeprices.ActivePriceFromNet();
                string[] item = html.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in item)
                {
                    Stock s = _stocks.StockByNumCode(str.Trim().Substring(2, 6));
                    if (str.Contains(s.Name) && str.Contains("\t"))
                    {
                        ActivePrice ap = new ActivePrice(str);
                        activeKD[s.ID].open = (int)(100 * ap.o);
                        activeKD[s.ID].close = (int)(100 * ap.c);
                        activeKD[s.ID].high = (int)(100 * ap.h);
                        activeKD[s.ID].low = (int)(100 * ap.l);
                        activeKD[s.ID].amount = (int)(100 * ap.amount);
                        activeKD[s.ID].vol = (int)(ap.vol);
                        //activeKD[s.ID].date = (int)(100 * ap.d);
                        activeKD[s.ID].reservation = 0;

                    }
                }
            }
        }
        private bool LoadLocalData()
        {
            Msg = "";
            if (!File.Exists(_jscfg.baseconfig.WorkPath + _locatepricedata))
            {
                Msg = "本地记录不存在";
                return false;
            }
            string txt = File.ReadAllText(_jscfg.baseconfig.WorkPath + _locatepricedata);
            _savetag = JsonConvert.DeserializeObject<SaveKdTag>(txt);
            if (_savetag.Tag.Count() != 2000)
            {
                Msg = "本地数据记录有误，或者格式不对，请从新下载生成";
                return false;
            }
            return true; //应改为 false
        }
        private void DownLoadNetDataAndCheckSave(int Dayscount)
        {
            this.NeedDownLoadDays = Dayscount;
            if (Dayscount > 1)
                DownDataFromNet(ThreadShowMsg);
            else
            {
                List<int> listdays = _netdate.ListHistoryDate.Skip(_netdate.ListHistoryDate.Count - Dayscount).ToList();
                foreach (int day in listdays)
                {
                    UpdataActiveDays(day);
                }
            }
        }
        private void UpdataActiveDays(int day)
        {
            activeKD = new KData[2000]; // struct 初始默认值为 000....
            CActivePrices _activeprices = new CActivePrices(_jscfg.globalconfig.Stocks);
            string html = _activeprices.ActivePriceFromNet();
            string[] item = html.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in item)
            {
                Stock s = _stocks.StockByNumCode(str.Trim().Substring(2, 6));
                if (str.Contains(s.Name) && str.Contains("\t"))
                {
                    ActivePrice ap = new ActivePrice(str);
                    activeKD[s.ID].open = (int)(100 * ap.o);
                    activeKD[s.ID].close = (int)(100 * ap.c);
                    activeKD[s.ID].high = (int)(100 * ap.h);
                    activeKD[s.ID].low = (int)(100 * ap.l);
                    activeKD[s.ID].amount = (int)(100 * ap.amount);
                    activeKD[s.ID].vol = (int)(ap.vol);
                    //activeKD[s.ID].date = (int)(100 * ap.d);
                    activeKD[s.ID].reservation = 0;

                }
            }
            //合并到
            nkd = new NetKData(_jscfg);
            nkd.ThreadShowMsg = ThreadShowMsg;
            nkd.CompleteRun = CompleteRun;
            nkd.DaysCount = NeedDownLoadDays;

            //netsaveTag = new SaveTag(DateTime.Now, TS.Tag);
            Tagstock[] Tag = new Tagstock[2000];
            for (int i = 0; i < 2000; i++)
                Tag[i] = new Tagstock();
            foreach (Stock s in _stocks.stocks)
                Tag[s.ID].Init(s);
            for (int i = 0; i < 2000; i++)
            {
                if (activeKD[i].date != 0)
                {
                    Tag[i].value = 1;
                    Tag[i].Tag = new List<KData>() { activeKD[i] };
                }

            }
        }
        private void DownDataFromNet(ShowDeleGate ThreadShowMsg) //等待线程结束 才返回
        {
            if (!isrunning)
            {
                isrunning = true;
                nkd = new NetKData(_jscfg);
                nkd.ThreadShowMsg = ThreadShowMsg;
                nkd.CompleteRun = CompleteRun;
                nkd.DaysCount = NeedDownLoadDays;
                System.Threading.Thread nonParameterThread =
                    new Thread(new ThreadStart(nkd.GetNetKData));
                nonParameterThread.Start();
            }
            while (isrunning)
                Thread.Sleep(100);
        }
        public void CompleteRun() //数据的处理合并
        {
            string Msg = ExCludeExchangeDayData();
            if (NeedDownLoadDays == DaysLength) //全部下载
            {
                _savetag = new SaveKdTag(nkd.netsaveTag.StoreDate, nkd.netsaveTag.Tag.Length);
                _savetag.Init(nkd.netsaveTag);
            }
            else
            {
                int oldestday = GetNetOldestDate();
                bool weekmonthexchange = CheckWeekMonthReplace();
                StringBuilder sb = new StringBuilder(); // 合并   
                for (int i = 0; i < 2000; i++)
                {
                    Tagstock tgi = nkd.netsaveTag.Tag[i];
                    tagkdstock _tgi = _savetag.Tag[i];
                    List<KData> kdi = (List<KData>)tgi.Tag;
                    if (_tgi.kd != null)
                    {
                        if (kdi == null) //已退市 删除
                        {
                            sb.Append(_tgi.index + "\t");
                        }
                        else // kdi.Count ====  NeedDownLoadDays
                        {
                            if (kdi[NeedDownLoadDays - 1].date != oldestday)
                            {
                                int maxlocalday = _tgi.kd.Max(r => r.date);
                                kdi = kdi.Where(r => r.date > maxlocalday).ToList();    //如已停牌，不予替换  
                            }
                            if (kdi.Count > 0)
                            {
                                if (weekmonthexchange) //除去最新那一天
                                    _tgi.kd = _tgi.kd.Take(_tgi.kd.Count - 1).ToList();
                                if (kdi.Count > 0)
                                {
                                    _tgi.kd = _tgi.kd.Skip(kdi.Count).ToList();
                                    _tgi.kd.AddRange(kdi);
                                }
                            }
                        }
                    }
                }
            }// 理论上不存在
            _savetag.StoreDate = _netdate.NearestDate;
            _savetag.Save(_jscfg.baseconfig.WorkPath + _locatepricedata);
            isrunning = false;
        }
        private int GetNetOldestDate()
        {
            List<KData> kd = (List<KData>)nkd.netsaveTag.Tag[0].Tag;
            return kd[kd.Count - 1].date;
        }
        private bool CheckWeekMonthReplace()
        {
            bool weekmonthexchange = false;
            if (_savetag != null)
            {
                int testweekdate = _savetag.Tag[0].kd[_savetag.Tag[0].kd.Count - 1].date;
                if (!Netdate.ListHistoryDate.Contains(testweekdate))
                {
                    //周线 月线 替换首日
                    weekmonthexchange = true;
                }
            }
            return weekmonthexchange;
        }
        private string ExCludeExchangeDayData()
        {
            int NeedDownLoadDays = this.NeedDownLoadDays;
            String Msg = "";
            for (int i = 0; i < 2000; i++)
            {
                Tagstock tgi = nkd.netsaveTag.Tag[i];
                List<KData> kdi = (List<KData>)tgi.Tag;
                if (kdi != null)
                {
                    //TODO：  Dayly时 非交易日  天数未知
                    if (kdi.Count == NeedDownLoadDays + 1) // Dayly时 交易日才会多一天，  weekly和monthly 连同当期 实际周期数
                    {
                        if (_netdate.Exchanging)
                            kdi = kdi.Take(NeedDownLoadDays).ToList();
                        //else //if( mergedays == DaysLength)  //15:00后可以  更新所有数据                 //15:00 后要保留当天数据，因而要删除前一天
                        if (kdi.Count > NeedDownLoadDays)
                        kdi = kdi.Skip(1).Take(NeedDownLoadDays).ToList();
                        tgi.Tag = kdi;
                    }
                    else if (kdi.Count == NeedDownLoadDays)
                    {
                        tgi.Tag = kdi;
                    }
                    else if (kdi.Count < NeedDownLoadDays) //已退市，或者停牌 股票，删除  //待Debug
                    {
                        Msg += nkd.netsaveTag.Tag[i].s.Name + nkd.netsaveTag.Tag[i].s.Code;
                    }

                }
                
            }
            return Msg;
        }
        public SaveKdTag SavekdTag
        {
            get
            {
                if (_savetag == null)
                    LoadLocalData();
                return _savetag;
            }
        }
        //init localdate
        public List<int> SHLocalDate()
        {
            BaseConfig cfg = _jscfg.baseconfig;
            if (File.Exists(cfg.WorkPath + _locatepricedata))
            {
                string txt = File.ReadAllText(cfg.WorkPath + _locatepricedata);
                int bpos = txt.IndexOf("\"Tag\":[");
                int epos = txt.IndexOf("}]}");
                if (bpos != -1 && epos != -1 && epos > bpos)
                {
                    txt = txt.Substring(bpos, epos - bpos + 3);
                    tagkdstock t = JsonConvert.DeserializeObject<tagkdstock>(txt);
                    return t.kd.Select(r => r.date).ToList();
                }
            }
            return new List<int>();
        }

        private JSConfig _jscfg;
        private Stocks _stocks;
        private Stock _ssestock;
        private SaveKdTag _savetag;
        private NetDate _netdate;
        private int DaysLength;  		//特定长度 84
        private int NeedDownLoadDays;  // 临时记录长度
        private bool isrunning;
        private NetKData nkd;
        public string Msg;
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
            if (urltype == "weekly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_weekqfq&param=[stockcode],week,,,[dayscount],qfq";
            if (urltype == "monthly")
                urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_monthqfq&param=[stockcode],month,,,[dayscount],qfq";
            daytpye = urltype.Replace("ly", "");
            int daylength = ListHistoryDate.Count;

		}
        public void Refresh()
        {
            string txt="";
            try
            {
                string url = urlt.Replace("[stockcode]", "sh000001").Replace("[dayscount]", dayscount.ToString());
                txt = CWeb.GetWebClient(url).Substring(10 + daytpye.Length);
                txt = txt.Replace(daytpye, "day");
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
            catch //(Exception emsg)
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
        private string daytpye;
	}
}
