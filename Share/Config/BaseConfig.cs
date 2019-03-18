using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using JHStock.Update;
using Tools;

namespace JHStock
{
	public class BaseConfig
	{		// daypath dayspath means workpath//day
        public string IndexPath()
        {
            return dbpath.Replace("Stock.mdb", "allindex.day");
        }
        public string MainPath()
        {
            return dbpath.Replace("Stock.mdb", "").Replace("Data\\", "");
        }
        public string BlockPath()
        {
            if (szpath != null)
            {
                return szpath.Substring(0, szpath.IndexOf("Vipdoc")) + "T0002\\blocknew\\TXDZ.blk";
            }
            return "TXDZ.blk";
        }                
		public string NowWorkPath()
		{
			return WorkPath+"data\\workpath\\"+TimeStringTools.NowDate()+"\\";
		}
        public bool   CheckWorkPath(){ 
            MFile.ClearEmptyPath(WorkPath+"data\\workpath\\");
            DirectoryInfo dir = new DirectoryInfo(NowWorkPath());
            dir.Create();
        	return true;
        }
        public string FinErrorPath()
        {
            return dbpath.Replace("Stock.mdb", "error.xls").Replace("Data\\", "fin\\");
        }        
        public string szpath { get; set; }
        public string shpath { get; set; }
        public string dbpath { get; set; }  
        public string WorkPath{ get; set; }
        public string CWFilePath { get { return _cwfile; } }
        public void SetCWFilePath(string filetype)
        {
            _cwfile = "data\\"+filetype+"_cwfx.dat";
        }
        private string _cwfile;

        public bool ReLoadCWFX { get; set; }
    }
	public class UpdateXmlPathConfig
	{
        public string HexinXml { get; set; }
        public string TopTenXml { get; set; }
        public string SinaBounsXml { get; set; }		
	}
	public class OutShowConfig{
		public List<string> ColumnTitles()
        {
            return BaseColumn.Concat(NowBaseColumn).Concat(ActiveColumn).ToList();
        }
        public List<string> ColumnBaseTitles()
        {
            return BaseColumn;
        }
        public List<string> ColumnFinTitles()
        {
            return FinColunm;
        }		
        public List<string> BaseColumn { get; set; }
        public List<string> NowBaseColumn { get; set; }
        public List<string> ActiveColumn { get; set; }
        public List<string> FinColunm { get; set; }
	}
	public class ConditionConfig{	//? List<Conditions	
        public List<string> Conditions { get; set; }
	}	
	public class GlobalConfig{	
		public GlobalConfig(BaseConfig baseconfig){
			this._baseconfig = baseconfig;
			UpdateDebug = true;
		}
        public bool InitStocks( )
        {
            try
            {
                if (_baseconfig!=null && File.Exists( _baseconfig.dbpath))
                {
                    db = new Db.ConnDb(_baseconfig.dbpath);
                    Stocks = new Stocks(this);
                }
                else
                {
                    ErrMsg = "数据库文件不存在，请更正后重试";
                    return false;
                }
            }
            catch(Exception e)
            {
                ErrMsg = "读取数据库文件时发生错误，代码为："+e.Message+"请更正后重试";
                return false;
            }
            return true;
        }

        public bool UpdateDebug{ get; set; } //For ThreadUpdateStocks.RunNetTime
        public bool Debug { get; set; } //没有 == statics.debug
        public string ErrMsg { get; set; }
        public StocksData StocksData { get; set; }	
        public Stocks Stocks { get; set; }	
        public Db.ConnDb db { get; set; }
        public CWeb web { get; set; }
        public BaseConfig Baseconfig { get { return _baseconfig; } }
        public StaticsConfig Staticsconfig { get; set; }
        public TempConfig Tempconfig { get; set; }
        public void ChangeBaseConfig(BaseConfig bcfg)
        {
            _baseconfig = bcfg;
        }
        //public CNFITool NFI{
        //    get
        //    {
        //    if(Stocks==null)
        //            return null;
        //        if (_nfi == null  )
        //            _nfi = new CNFITool( _baseconfig.WorkPath+"\\data\\NFT.txt");
        //        return _nfi;
        //    }
        //}
        //public CQFITool QFI
        //{
        //    get
        //    {
        //        if(Stocks==null)
        //            return null;
        //        if (_qfi == null  )
        //            _qfi = new CQFITool(_baseconfig.WorkPath+ "data\\QQFin\\" ,Stocks );
        //        return _qfi;
        //    }
        //}
        public CFQTool FQT
        {
            get
            {
                if (_fq == null && db!=null)
                    _fq = new CFQTool(db);
                return _fq;
            }
        }
		private BaseConfig _baseconfig;
        private  CFQTool _fq;
        //private  CQFITool _qfi;
        //private  CNFITool _nfi;

        public int DaysCount { get { return Staticsconfig.KDataDaysCount;  } }
    }
	public class StaticsConfig{
        public bool Debug { get; set; }
        public int BeginDay { get; set; }
        public int EndDay { get; set; }
        public int StaticDays { get; set; }
        public int KDataDaysCount { get; set; }  
        public string wout { get; set; }
    }
    public class TempConfig
    {
        public TempConfig()
        {
            NowMins = 0;
        }

        public int NowMins { get; set; }
        //public bool Debug { get; set; }
    }
}
