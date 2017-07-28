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
	public class StocksLog{
		public StocksLog(JSConfig _jscfg){
			this._jscfg = _jscfg;
			this._stocks = _jscfg.globalconfig.Stocks;
			_dailystocks = new List<DailyStocks>();
			Load();
		}
		
		public void SaveLog(UpdateMonitInfors umi)
		{
			DateTime  Date = _jscfg.globalconfig.StocksData.SaveTag.StoreDate;	
			DailyStocks[] ds = _dailystocks.Where( r => r.Date == Date).ToArray();
			if(ds == null || ds.Count() == 0)
				_dailystocks.Add(new DailyStocks( Date,umi));
			else
				ds[0].Add(umi);
		}
		public void Save(){
			string s = JsonConvert.SerializeObject( _dailystocks );
			string path = _jscfg.baseconfig.WorkPath + "data\\SaveLog.data";
			MFile.WriteAllText(path,s);
		}
		private void Load(){
			string path = _jscfg.baseconfig.WorkPath + "data\\SaveLog.data";
			if(File.Exists( path)){
				string s = File.ReadAllText(path);
				List<DailyStocks>   sks= JsonConvert.DeserializeObject<List<DailyStocks>>(s);
				this._dailystocks = sks;
			}
		}
		[JsonIgnore]
		private JSConfig _jscfg;
		[JsonIgnore]
		private Stocks _stocks;
		public List<DailyStocks> _dailystocks; 
	}
	public class DailyStocks{ //存储一天的 股票
		public DailyStocks(){ // for jsonserialize need a no-argument constructor
			
		}
		public DailyStocks( DateTime datetime,UpdateMonitInfors umi){
			Date = datetime;
			_stocks = new List<DailyStock>();
			foreach(UpdateMonitInfor ui in umi.ls){
				DailyStock ds = new DailyStock( );
				System.Data.DataRow dr = ui.DataRow();
				ds.StockCode = dr["代码"].ToString();
				ds.StockBaseInfor = dr["财务信息"].ToString();
				ds.StockTestInfo = dr["持续天数"].ToString() +"-"+ dr["后续天数"].ToString()+"-"+dr["后续天数的情况"].ToString();
				_stocks.Add( ds );
			}
		}
		public void Add(UpdateMonitInfors umi){
			List<string> listcode = _stocks.Select( r => r.StockCode).ToList();
			foreach(UpdateMonitInfor ui in umi.ls){
				System.Data.DataRow dr = ui.DataRow();
				if(listcode.Contains( dr["代码"].ToString() ))
					continue;				
				DailyStock ds = new DailyStock( );
				ds.StockCode = dr["代码"].ToString();
				ds.StockBaseInfor = dr["财务信息"].ToString();
				ds.StockTestInfo = dr["持续天数"].ToString() +"-"+ dr["后续天数"].ToString()+"-"+dr["后续天数的情况"].ToString();
				_stocks.Add( ds );
			}			
		}
		public DateTime Date;
		public int time;
		public List<DailyStock> _stocks;
	}
	public class DailyStock{ // 存储
		public DailyStock(){
		
		}
		[JsonIgnore]
		public Stock s;
		public string StockCode;
		public string StockBaseInfor;
		public string StockTestInfo;
	}
}
