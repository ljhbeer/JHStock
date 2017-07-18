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
			_activeKData = new KData[2000];
			TempDaysCount = 0;
			DaysLength = 84;	
			_netdate = new NetDate(DaysLength);
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
		
        /// 
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
                if (!LoadLocalData()) //下载完整数据
                {
                    DownLoadNetDataAndCheckSave(DaysLength); //由 DaysLength
                    return "OK: 已下载完整数据并替换，当前数据日期为" + _savetag.StoreDate.ToShortDateString(); // +本地数据最新日期
                }
                else
                {
                    if (_savetag.Tag[0] == null || _savetag.Tag[0].kd == null || _savetag.Tag[0].kd.Count == 0)
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
            }
            return "error 逻辑错误，此处不应出现";
        }
        private bool LoadLocalData( )
		{
			Msg="";
			if(!File.Exists(_jscfg.baseconfig.WorkPath + "data\\ExpPrice.dat")){
				Msg = "本地记录不存在";
				return false;
			}
			string txt = File.ReadAllText(_jscfg.baseconfig.WorkPath + "data\\ExpPrice.dat");
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
            int mergedays = nkd.netsaveTag.Tag[0].kd.Count - 1;
            if (_netdate.IncludeToday)
            {
                Msg+="\r\n MergeData Error: ";
                if ( mergedays == DaysLength) //不合并  除去当天数据  保存
                {
                    for (int i = 0; i < 2000; i++)
                    {
                        if (nkd.netsaveTag.Tag[i].kd != null && nkd.netsaveTag.Tag[i].kd.Count == mergedays + 1)
                        { //合并kd 
                            nkd.netsaveTag.Tag[i].kd = nkd.netsaveTag.Tag[i].kd.Take(mergedays).ToList();
                        }
                        else if (nkd.netsaveTag.Tag[i].kd != null)
                        {
                            Msg += nkd.netsaveTag.Tag[i].s.Name + nkd.netsaveTag.Tag[i].s.Code;
                        }

                    }//合并完成
                    // 保存
                    _savetag = nkd.netsaveTag;
                    _savetag.StoreDate = _netdate.NearestDate;
			        _savetag.Save(_jscfg.baseconfig.WorkPath+"data\\ExpPrice.dat");	
                }
                else // 合并
                {
                    // 计算合并天数                    
                    _savetag.StoreDate = DateTime.Now;
                    for (int i = 0; i < 2000; i++)
                    {
                        if (_savetag.Tag[i].kd != null && nkd.netsaveTag.Tag[i].kd.Count == mergedays+1)
                        { //合并kd 
                            _savetag.Tag[i].kd= _savetag.Tag[i].kd.Skip( mergedays).ToList();
                            _savetag.Tag[i].kd.AddRange(nkd.netsaveTag.Tag[i].kd.Take( mergedays ).ToList());
                        }
                    }//合并完成
                    // 保存

                    _savetag.StoreDate = _netdate.NearestDate;
			        _savetag.Save(_jscfg.baseconfig.WorkPath+"data\\ExpPrice.dat");	
                }
            }
            else
            {
                if (mergedays == DaysLength)//直接存储
                {
                    _savetag = nkd.netsaveTag;
                    _savetag.StoreDate = _netdate.NearestDate;
                    _savetag.Save(_jscfg.baseconfig.WorkPath + "data\\ExpPrice.dat");	
                }
                else //合并后 保存
                {
                    _savetag.StoreDate = DateTime.Now;
                    for (int i = 0; i < 2000; i++)
                    {
                        if (_savetag.Tag[i].kd != null && nkd.netsaveTag.Tag[i].kd.Count == mergedays)
                        { //合并kd 
                            _savetag.Tag[i].kd = _savetag.Tag[i].kd.Skip(mergedays).ToList();
                            _savetag.Tag[i].kd.AddRange(nkd.netsaveTag.Tag[i].kd);
                        }
                    }//合并完成
                    // 保存

                    _savetag.StoreDate = _netdate.NearestDate;
                    _savetag.Save(_jscfg.baseconfig.WorkPath + "data\\ExpPrice.dat");	
                }
            }
            isrunning = false;
        }
        
		public void SaveData(){
			///
			throw new Exception();
			//// 获取上证指数
			tagstock ssetag = ThreadUpdateStocksQQDayly.DownLoadData(this._ssestock,TempDaysCount);
                    ssetag.s = _ssestock;
            _savetag.Tag[_ssestock.ID] = ssetag;
			
		}
		//init localdate
		public List<int> SHLocalDate()
		{
			BaseConfig cfg = _jscfg.baseconfig;
			if (File.Exists(cfg.WorkPath + "data\\ExpPrice.dat")) {
				string txt = File.ReadAllText(cfg.WorkPath + "data\\ExpPrice.dat");
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
		private KData[]    _activeKData; // 当前时间
		private NetDate _netdate;
		private int DaysLength;  		//特定长度 84
		private int TempDaysCount;  // 临时记录长度
		private bool isrunning;		
		public  string Msg;

        private NetKData nkd;
        public ShowDeleGate ThreadShowMsg;       
    }
	public class NetDate{
		public NetDate(int DaysCount){
			this.dayscount = DaysCount;			
			Inline = false;
			IncludeToday = false;
            NearestDate = DateTime.Now;
			int daylength=ListHistoryDate.Count;
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

                if (_netszdate.Count  ==  dayscount + 1 )
                {
                    IncludeToday = true;
                    _netszdate = _netszdate.Take(_netszdate.Count - 1).ToList();
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
