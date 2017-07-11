using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using JHStock.Update;
using Tools;

namespace JHStock
{
    public class Config
    {
        public Config()
        {
            web = new CWeb();
            daypath = "";
            _fq = null;
            _qfi = null;
            _stocks = null;
            ErrMsg = "";
        }
        public void Load(string filename)
        {
            string str = File.ReadAllText(filename);
            szpath = StringTools.GetEqualValue(str, "szpath=", "\r\n");
            shpath = StringTools.GetEqualValue(str, "shpath=", "\r\n");
            dbpath = StringTools.GetEqualValue(str, "dbpath=", "\r\n");
            //Daypath = StringTools.GetEqualValue(str, "daypath=", "\r\n");
            WorkPath= StringTools.GetEqualValue(str, "workpath=", "\r\n").Trim();
            if (!WorkPath.EndsWith(""))
                WorkPath = WorkPath + "\\";
            Daypath = WorkPath;
            HexinXml = StringTools.GetEqualValue(str, "hexinxmlpath=", "\r\n");
            TopTenXml = StringTools.GetEqualValue(str, "toptenxmlpath=", "\r\n");
            SinaBounsXml = StringTools.GetEqualValue(str, "sinabounsxmlpath=", "\r\n");
            enddate=StringTools.GetEqualValue(str, "enddate=","\r\n");
            begindate=StringTools.GetEqualValue(str, "begindate=","\r\n");
            wout=StringTools.GetEqualValue(str, "wout=","\r\n"); 
            days=StringTools.GetEqualValue(str, "days=","\r\n");
            ExtBtn=StringTools.GetEqualValue(str, "extbtn=","\r\n");
            BaseColumn = StringTools.GetEqualValue(str, "BaseColumn=", "\r\n");
            NowBaseColumn = StringTools.GetEqualValue(str, "NowBaseColumn=", "\r\n");
            ActiveColumn = StringTools.GetEqualValue(str, "ActiveColumn=", "\r\n");
            FinColunm = StringTools.GetEqualValue(str, "FinColunm=", "\r\n");
            Conditionstr = StringTools.GetEqualValue(str, "<CONDITION>", "</CONDITION>");
            Memostr = StringTools.GetEqualValue(str, "<MEMO>", "</MEMO>");
            StaticDays =0;
            string StaticDaysstr = StringTools.GetEqualValue(str, "StaticDays=", "\r\n");
            if (ValidTools.ValidNumber(StaticDaysstr))
                StaticDays = Convert.ToInt32(StaticDaysstr);
            if (ExtBtn.ToLower() == "on")
                ExtBtn = "on";
            else
                ExtBtn = "off";
            this.filename = filename;
            try
            {
                OutFlag = Convert.ToInt64(StringTools.GetEqualValue(str, "OutFlag=", "\r\n"));
                Fin=Convert.ToInt64(StringTools.GetEqualValue(str, "Fin=", "\r\n"));
            }
            catch { }
        }
        public void Save(string tfilename = "")
        {
            string str = "";
            str += AppendTag("TXD");
            str += AppendItem("szpath", szpath);
            str += AppendItem("shpath", shpath);
            str += AppendItem("dbpath", dbpath);
            str += AppendItem("hexinxmlpath",HexinXml);
            str += AppendItem("toptenxmlpath",TopTenXml );
            str += AppendItem("sinabounsxmlpath",SinaBounsXml );
            str += AppendItem("workpath",WorkPath);
            str += AppendTag("WIGHT");
            str += AppendItem("StaticDays", StaticDays.ToString());
            str += AppendItem("enddate", enddate);
            str += AppendItem("begindate",begindate);
            str += AppendItem("wout",wout);
            str += AppendItem("days",days);
            str += AppendItem("OutFlag", OutFlag.ToString());
            str += AppendItem("Fin", Fin.ToString());
            str += AppendTag("EXT");
            str += AppendItem("extbtn",ExtBtn.ToString());
            str += AppendItem("BaseColumn", BaseColumn.ToString());
            str += AppendItem("NowBaseColumn", NowBaseColumn.ToString());
            str += AppendItem("ActiveColumn", ActiveColumn.ToString());
            str += AppendItem("FinColunm",FinColunm.ToString());
            str += AppendRichItem("CONDITION", Conditionstr);
            str += AppendRichItem("MEMO", Memostr);
            if(tfilename == "")
                File.WriteAllText(filename, str);           
        }
        public int FirstYear()
        {
            return 2013;
        }
        public string BlockPath()
        {
            if (szpath != null)
            {
                return szpath.Substring(0, szpath.IndexOf("Vipdoc")) + "T0002\\blocknew\\TXDZ.blk";
            }
            return "TXDZ.blk";
        }
        public string IndexPath()
        {
            return dbpath.Replace("Stock.mdb", "allindex.day");
        }
        public string MainPath()
        {
            return dbpath.Replace("Stock.mdb", "").Replace("Data\\", "");
        }
        public string FinErrorPath()
        {
            return dbpath.Replace("Stock.mdb", "error.xls").Replace("Data\\", "fin\\");
        }
        public List<string> ColumnTitles()
        {
            List<string> Titles = new List<string>();
            AddStringToList(Titles, BaseColumn);
            AddStringToList(Titles, NowBaseColumn);
            AddStringToList(Titles, ActiveColumn);
            return Titles;
        }
        public List<string> ColumnBaseTitles()
        {
            List<string> Titles = new List<string>();
            AddStringToList(Titles, BaseColumn);
            return Titles;
        }
        public List<string> ColumnFinTitles()
        {
            List<string> Titles = new List<string>();
            AddStringToList(Titles, FinColunm);
            return Titles;
        }
        private void AddStringToList(List<string> Titles, string strColumn)
        {
            if (strColumn != null)
            {
                string[] Items = strColumn.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in Items)
                    Titles.Add(s);
            }
        }
        public bool SameFilename(string filename)
        {
            return filename == this.filename;
        }
        private string AppendItem(string name, string value)
        {
            return name + "=" + value + "\r\n";
        }
        private string AppendRichItem(string name, string value)
        {
            return "<"+name +">\r\n" + value +"\r\n</"+name+">\r\n";
        }
        private string AppendTag(string tag)
        {
            return "[" + tag + "]\r\n";
        }

        public bool CheckWorkPath(){ 
            //if(!File.Exists(NowWorkPath())){
            //  File.Create(NowWorkPath());
            //}
            MFile.ClearEmptyPath(WorkPath+"data\\workpath\\");
            DirectoryInfo dir = new DirectoryInfo(NowWorkPath());
            dir.Create();
        	return true;
        }
		public string  NowWorkPath()
		{
			return WorkPath+"data\\workpath\\"+TimeStringTools.NowDate()+"\\";
		}
        

        private string filename;
        public string HexinXml { get; set; }
        public string TopTenXml { get; set; }
        public string szpath { get; set; }
        public string shpath { get; set; }
        public string dbpath { get; set; }  
        public string WorkPath{ get; set; } 
        public string wout { get; set; }
        public string days { get; set; }
        public long OutFlag { get; set; }
        public long Fin { get; set; }
        public string ExtBtn { get; set; }
        public string BaseColumn { get; set; }
        public string NowBaseColumn { get; set; }
        public string ActiveColumn { get; set; }
        public string FinColunm { get; set; }
        public string SinaBounsXml { get; set; }

        private  CFQTool _fq;
        private  CQFITool _qfi;
        private  CNFITool _nfi;
        public Db.ConnDb db { get; set; }
        public CWeb web { get; set; }
        private string daypath;
        private string enddate;
        private string begindate;
        public string Daypath
        {
            get { return daypath; }
            set
            {
                if (value != null && value != "" && !value.EndsWith("\\"))
                    value = value + "\\";
                daypath = value;
            }
        }
       
        public CNFITool NFI{
        	get
        	{
        	if(_stocks==null)
            		return null;
                if (_nfi == null  )
                    _nfi = new CNFITool(  WorkPath+"\\data\\NFT.txt");
                return _nfi;
        	}
        }
        public CQFITool QFI
        {
            get
            {
            	if(_stocks==null)
            		return null;
                if (_qfi == null  )
                    _qfi = new CQFITool( WorkPath+ "data\\QQFin\\" ,_stocks );
                return _qfi;
            }
        }
        public CFQTool FQ
        {
            get
            {
                if (_fq == null && db!=null)
                    _fq = new CFQTool(db);
                return _fq;
            }
        }
        //int beginday = Convert.ToInt32(textBoxBdate.Text.Replace("-", ""));
        //int endday = Convert.ToInt32(textBoxEdate.Text.Replace("-", ""));      
        public int BeginDay { get { return Convert.ToInt32(begindate); } }
        public int EndDay { get { return Convert.ToInt32(enddate); } }
        public int StaticDays { get; set; } //120
        public bool Debug { get; set; }
        public string Conditionstr { get; set; }
        public string Memostr { get; set; }

        public void SetBeginDay(string p)
        {
            begindate = p;
        }
        public void SetEndDay(string p)
        {
            enddate = p;
        }    

		public string ErrMsg { get; set; }     
		public Stocks _stocks;		
    }
}
