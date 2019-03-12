﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Newtonsoft.Json;
using Tools;

namespace JHStock.Update
{
	public class QQStocks{
		public int code { get; set; }
		public string msg { get; set; }
		public Dictionary<string ,QQStockDetail> data  { get; set; }		
        public static List<int> GetNetShdate(int daylength=254){
			try{
			string url = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_dayqfq&param=sh000001,day,,,[daylength],qfq"
				.Replace("[daylength]",daylength.ToString());
            string txt = CWeb.GetWebClient(url);
            txt = CutJsonStringHead(txt);
			QQStocks qs = JsonConvert.DeserializeObject<QQStocks>(txt);
			return qs.data["sh000001"].day.Select( r => Convert.ToInt32(   r[0].ToString().Replace("-",""))).ToList();
			}catch{
				return new List<int>();
			}
		}
        public static string CutJsonStringHead(String txt)
        {
            if (txt.IndexOf("=") != -1)
                txt = txt.Substring(txt.IndexOf("=") + 1);
            return txt;
        }
	}	
	public class  QQStockDetail{
		public List<List<object>> day {get; set;}
		public object qt { get; set; }
		public object mx_price { get; set; }
		public string prec { get; set; }
		public string version { get; set; }
	}
}
