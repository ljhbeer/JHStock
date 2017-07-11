using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ToolsCXml;
using Tools;
namespace JHStock
{
	public class ThreadUpdateDaylyStock
	{
		public ThreadUpdateDaylyStock(Stock s,ThreadUpdateDaylyStocks tuss,bool debug = false){
			this.debug = debug;
			this.s=s;
			this.tuss = tuss;
		}
		public void Run()
		{
			try{
				string url = urlt1.Replace("[stockcode]",s.Code);
				string txt = web.GetOKUrl(url);
				if(txt!=""){
					tuss.Tag[s.ID].value = 1;
				}else{
					tuss.Tag[s.ID].value = -1;
				}
			}catch{
					tuss.Tag[s.ID].value = -2;				
			}
			Interlocked.Decrement(ref tuss.threadsum);
			Interlocked.Increment(ref tuss.threadcompletesum);
		}
		public void SetGetDayLength(int daylength){
			urlt1 = urlt.Replace("[daylength]",daylength.ToString());
		}
		private static string urlt1=""; 
		private static string urlt =  @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_dayqfq&param=[stockcode],day,,,[daylength],qfq";
		private static CWeb web = new CWeb();
		private Stock s;
		private ThreadUpdateDaylyStocks tuss;
		private bool debug;
	}
}
