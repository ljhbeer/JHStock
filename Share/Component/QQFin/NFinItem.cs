using System;
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
	public class NFinItem{	
		public NFinItem(){
			NDJCW = new List<NDJCW>();
			NCZNL = new List<NCZNL>();
		}
		public List<NDJCW> NDJCW{
			get;set;
		}
		public List<NCZNL> NCZNL{
			get;set;
		}
	}
	
	public class NDJCW{
		public NDJCW( DateTime  bgrq, string mgsytb,string jzcsyl,string zzcsyl){
			this.bgrq = bgrq;
			this.mgsy = mgsytb;
			this.jzcsyl = jzcsyl;
			this.zzcsyl = zzcsyl;			
		}
        public DateTime bgrq { get; set; }
        public string mgsy { get; set; } //每股收益(摊薄)(元)
        public string jzcsyl { get; set; }  //净资产收益率(%)
        public string zzcsyl { get; set; }  //总资产收益率(%)		
	}
	public class NCZNL{ //默认增长率
		public NCZNL(DateTime  bgrq,string mgsy, string jlr,string zzc,string yylr,string zysr){
			this.bgrq = bgrq;
			this.mgsy = mgsy;
			this.zysr = zysr;
			this.yylr = yylr;
			this.jlr = jlr;
			this.zzc = zzc;
		}
        public DateTime bgrq { get; set; }
        public string mgsy { get; set; } //每股收益增长率(%)
        public string zysr { get; set; } //主营收入增长率(%)
        public string yylr { get; set; } //营业利润增长率(%)
        public string jlr { get; set; }  //净利润增长率(%)
        public string zzc { get; set; }  //总资产增长率(%)
		
	}
}
