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
	public class QQfinItem{
			public void LoadData(string path)
			{
				this._path = path;
	//			string txt = File.ReadAllText(path).Replace("--","0");
	//			_ylnl = JsonConvert.DeserializeObject<List<YLNL>>(
	//			       "["+ btItemYLNL.BEPos(txt).String+"]"   );
	//			_yynl = JsonConvert.DeserializeObject<List<YYNL>>(
	//			       "["+ btItemYYNL.BEPos(txt).String+"]"   );
	//			_cznl = JsonConvert.DeserializeObject<List<CZNL>>(
	//			       "["+ btItemCZNL.BEPos(txt).String+"]"   );
	//			_djcw = JsonConvert.DeserializeObject<List<DJCW>>(
	//			       "["+ btItemDJCW.BEPos(txt).String+"]"   );
				
			}
			public  List<YLNL>   YLNL{
				get{
					if(_ylnl == null){
						string txt = File.ReadAllText(_path).Replace("--","0");
						_ylnl = JsonConvert.DeserializeObject<List<YLNL>>(
						       "["+ btItemYLNL.BEPos(txt).String+"]"   );					
					}				   
					return _ylnl;
				}
			}
		public  List<YYNL>   YYNL{
				get{
					if(_yynl==null){
						string txt = File.ReadAllText(_path).Replace("--","0");					
						_yynl = JsonConvert.DeserializeObject<List<YYNL>>(
						       "["+ btItemYYNL.BEPos(txt).String+"]"   );
					}
					return _yynl;
				}
			}
		public  List<CZNL>   CZNL{
				get{
					if(_cznl==null){
						string txt = File.ReadAllText(_path).Replace("--","0");
						_cznl = JsonConvert.DeserializeObject<List<CZNL>>(
						       "["+ btItemCZNL.BEPos(txt).String+"]"   );					
					}
					return _cznl;
				}
			}
		public  List<DJCW>   DJCW{
				get{
					if(_djcw==null){
						if(File.Exists (_path) ){
						string txt = File.ReadAllText(_path).Replace("--","0");
						_djcw = JsonConvert.DeserializeObject<List<DJCW>>(
							"["+ btItemDJCW.BEPos(txt).String+"]"   );	
						}else{
							MFile.AppendAllText("QQFin_noneTxt.log",_path+"\r\n");
						}
					}				
					return _djcw;
				}
			}
		
		
		private List<YLNL> _ylnl;
		private List<YYNL> _yynl;
		private List<CZNL> _cznl;
		private List<DJCW> _djcw;
		
		private static ToolsCXml.BETag btItemYLNL =  new ToolsCXml.BETag("[item=#@#@-@#@#]".Replace("item","YLNL"));
	    private static ToolsCXml.BETag btItemYYNL =  new ToolsCXml.BETag("[item=#@#@-@#@#]".Replace("item","YYNL"));
	    private static ToolsCXml.BETag btItemCZNL =  new ToolsCXml.BETag("[item=#@#@-@#@#]".Replace("item","CZNL"));
	    private static ToolsCXml.BETag btItemDJCW =  new ToolsCXml.BETag("[item=#@#@-@#@#]".Replace("item","DJCW"));
	    
	    private string _path;    	
	}
	public class YLNL
    {
        /// <summary>
        /// 2016-09-30
        /// </summary>
        public DateTime bgrq { get; set; }
        /// <summary>
        /// 1,038,191,660.43
        /// </summary>
        public string jlrkc { get; set; }
        /// <summary>
        /// 30.76
        /// </summary>
        public string yylrl { get; set; }
        /// <summary>
        /// 39.36
        /// </summary>
        public string xsmll { get; set; }
        /// <summary>
        /// 23.37
        /// </summary>
        public string xsjll { get; set; }
        /// <summary>
        /// 45.33
        /// </summary>
        public string cbfylrl { get; set; }
        /// <summary>
        /// 8.14
        /// </summary>
        public string sxfyl { get; set; }
        /// <summary>
        /// 10.59
        /// </summary>
        public string jzcsyljq { get; set; }
        /// <summary>
        /// 6.25
        /// </summary>
        public string zzclrl { get; set; }
        /// <summary>
        /// 137,471.08
        /// </summary>
        public string xsqlr { get; set; }
        /// <summary>
        /// 1.78
        /// </summary>
        public string fjcxsybl { get; set; }
        /// <summary>
        /// 30.31
        /// </summary>
        public string xsqlrl { get; set; }
    }
    public class YYNL
    {
        /// <summary>
        /// 2016-09-30
        /// </summary>
        public DateTime bgrq { get; set; }
        /// <summary>
        /// 6.24
        /// </summary>
        public string yszkzzl { get; set; }
        /// <summary>
        /// 57.72
        /// </summary>
        public string yszkzzts { get; set; }
        /// <summary>
        /// 38.91
        /// </summary>
        public string chzzl { get; set; }
        /// <summary>
        /// 9.25
        /// </summary>
        public string chzzts { get; set; }
        /// <summary>
        /// --
        /// </summary>
        public string gdzczzl { get; set; }
        /// <summary>
        /// 0.44
        /// </summary>
        public string gdqyzzl { get; set; }
        /// <summary>
        /// 1.55
        /// </summary>
        public string ldzczzl { get; set; }
        /// <summary>
        /// 231.94
        /// </summary>
        public string ldzczzts { get; set; }
        /// <summary>
        /// 0.31
        /// </summary>
        public string zzczzl { get; set; }
        /// <summary>
        /// 1,161.67
        /// </summary>
        public string zzczzts { get; set; }
        /// <summary>
        /// 1.87
        /// </summary>
        public string chzcgcl { get; set; }
    }
    public class CZNL
    {
        /// <summary>
        /// 2015-12-31
        /// </summary>
        public DateTime bgrq { get; set; }
        /// <summary>
        /// 15.20
        /// </summary>
        public string mgsy { get; set; }
        /// <summary>
        /// -229.88
        /// </summary>
        public string mgxj { get; set; }
        /// <summary>
        /// 1.67
        /// </summary>
        public string zysr { get; set; }
        /// <summary>
        /// 10.87
        /// </summary>
        public string yylr { get; set; }
        /// <summary>
        /// 13.08
        /// </summary>
        public string lrze { get; set; }
        /// <summary>
        /// 15.21
        /// </summary>
        public string jlr { get; set; }
        /// <summary>
        /// 10.36
        /// </summary>
        public string xsqlr { get; set; }
        /// <summary>
        /// 11.30
        /// </summary>
        public string zzc { get; set; }
    }
    public class DJCW
    {
        /// <summary>
        /// 2016-09-30
        /// </summary>
        public DateTime bgrq { get; set; }
        /// <summary>
        /// 0.31
        /// </summary>
        public string mgsytb { get; set; }
        /// <summary>
        /// --
        /// </summary>
        public string mgsykctb { get; set; }
        /// <summary>
        /// 3.46
        /// </summary>
        public string jzcsyl { get; set; }
        /// <summary>
        /// --
        /// </summary>
        public string jzcsylkc { get; set; }
        /// <summary>
        /// 2.13
        /// </summary>
        public string zzcsyl { get; set; }
        /// <summary>
        /// 2.75
        /// </summary>
        public string sqzzcsyl { get; set; }
        /// <summary>
        /// 1.35
        /// </summary>
        public string mgxssr { get; set; }
        /// <summary>
        /// 465,133,972.10
        /// </summary>
        public string xsqlr { get; set; }
        /// <summary>
        /// 30.00
        /// </summary>
        public string xsqlrl { get; set; }
        /// <summary>
        /// 39.53
        /// </summary>
        public string xsmll { get; set; }
        /// <summary>
        /// 23.34
        /// </summary>
        public string xsjll { get; set; }
    }
}
