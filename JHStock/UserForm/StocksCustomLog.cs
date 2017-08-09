using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Tools;
using System.IO;

namespace JHStock 
{
    public class StocksCustomLog
    {
        public StocksCustomLog(JSConfig _jscfg){
			this._jscfg = _jscfg;
			this._stocks = _jscfg.globalconfig.Stocks;
			_dailystocks = new List<DailyLogStocks>();
			Load();
		}       
        public String SaveLog(string title, string content, bool ignore = false) //
        {
            DateTime Date = _jscfg.globalconfig.StocksData.SaveTag.StoreDate;
            DailyLogStocks[] ds = _dailystocks.Where(r => r.Date == Date).ToArray();
            if (ds == null || ds.Count() == 0)
                _dailystocks.Add(new DailyLogStocks(Date, title, content));
            else
            {
               return ds[0].Add(title, content,ignore);
            }
            // 同一天标题重复问题
            return "";
        }
		public void Save(){
			string s = JsonConvert.SerializeObject( _dailystocks );
			string path = _jscfg.baseconfig.WorkPath + "data\\SaveCustomLog.data";
			MFile.WriteAllText(path,s);
		}
		private void Load(){
			string path = _jscfg.baseconfig.WorkPath + "data\\SaveCustomLog.data";
			if(File.Exists( path)){
				string s = File.ReadAllText(path);
				List<DailyLogStocks>   sks= JsonConvert.DeserializeObject<List<DailyLogStocks>>(s);
				this._dailystocks = sks;
			}
		}
		[JsonIgnore]
		private JSConfig _jscfg;
		[JsonIgnore]
		private Stocks _stocks;
		public List<DailyLogStocks> _dailystocks;

    }

    public class DailyLogStocks
    { //存储一天的 股票
        public DailyLogStocks()
        { // for jsonserialize need a no-argument constructor
        }
        public DailyLogStocks(DateTime datetime, UpdateMonitInfors umi)
        {
            Date = datetime;
            _stocks = new List<DailyLogStock>();
        }

        public DailyLogStocks(DateTime Date, string title, string content)
        {
            this.Date = Date;
            _stocks = new List<DailyLogStock>();
            _stocks.Add(new DailyLogStock(title,content));
        }
        public string Add(string title, string content, bool ignore)
        {
            bool sametitle = _stocks.Exists(r => r.Title == title);
            if (sametitle)
            {
                if (ignore)
                {
                    List<DailyLogStock> ds = _stocks.Where(r => r.Title == title).ToList();
                    ds[0].ChangeContent(content);
                }
                else
                    return "ExistTitle";
            }
            else
            {
                _stocks.Add(new DailyLogStock(title, content));
            }
            return "";
        }       
        public DateTime Date;
        public List<DailyLogStock> _stocks;
    }
    public class DailyLogStock
    { // 存储
        public DailyLogStock()
        {

        }
        public DailyLogStock(string title, string content)
        {
            this.Title = title;
            this.Content = content;
        }
        public void ChangeContent(string content)
        {
            this.Content = content;
        }
        [JsonIgnore]
        public Stock s;
        public string Title;
        public string Content;
    }
}
