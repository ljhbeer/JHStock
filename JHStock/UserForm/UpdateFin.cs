using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Tools;
using System.Text.RegularExpressions;

namespace JHStock.Update
{
    public delegate void ShowDeleGate(string file);
    public delegate void CompleteDeleGate();
    public class UpdateFin
    {
        public UpdateFin(Stocks stocks)
        {
            bshowtimeout = false;
            _stocks = stocks;
            _cfg = _stocks.Gcfg;
            _exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";                       
            showmsg = null;
            
        }
        public int MaxThreadSum { get; set; }
        public List<Stock> DealStocks { get; set; } 
        public void DownLoadFinData()
        {
            string datetype = "maincwfx";
            ThreadUpdateStocksConfig TSC1 = new ThreadUpdateStocksConfig(datetype, 8);
            FormatDataFunction FD = new FormatDataFunction(datetype);
            TSC1.UrlT = new ThreadUpdateUrlTemplate(datetype);
            TSC1.UrlT.SetParam("[type]", _NumType);
            TSC1.Files = new ThreadUpdateJsonFiles();
            TSC1.MaxThreadSum =50;
            TSC1.FormatData = FD.FormatKDData;
            ThreadUpdateStocksJson TS = new ThreadUpdateStocksJson(_stocks, TSC1);
            if (ThreadShowMsg != null)
                TS.showmsg = new ShowDeleGate(ThreadShowMsg);

            List<Stock> Dealstocks = new List<Stock>(_stocks.stocks);
            if (showmsg!= null)
                TS.showmsg = showmsg;
            //Dealstocks = Dealstocks.Take(10).ToList();
            // Add 上证指数
            //TS.Tag[0].Init(_ssestock);
            //Dealstocks.Add(_ssestock);

            TS.UpdateItem(Dealstocks);
            List<Stock> stocks = Dealstocks;
            int stockcountb = stocks.Count;


            stocks = TS.Tag.Where(r => r.value < 1 && r.value > -10).Select(r => r.s).ToList();
            if (stocks.Count > 0)
            {
                TS.UpdateItem(stocks);
            }
            CompleteRun(TS);
            ThreadCompleteRun(); //输出信息
        }
        public void DownLoadAllKData()
        {
            string datetype = "dayly";
            int dayscount = _stocks.ListDate.Count;
            ThreadUpdateStocksConfig TSC1 = new ThreadUpdateStocksConfig(datetype, dayscount);
            JHStock.FormatDataFunction FD = new JHStock.FormatDataFunction(datetype);
            TSC1.UrlT = new ThreadUpdateUrlTemplate(datetype);
        
            TSC1.Files = new ThreadUpdateJsonFiles();
            TSC1.Files.OutPutMode = "singlefile";
            TSC1.Files.ThreadOutTxt = ThreadOutTxt;
            TSC1.MaxThreadSum =20;
            TSC1.FormatData = null;//FD.FormatKDData;
            ThreadUpdateStocksJson TS = new ThreadUpdateStocksJson(_stocks, TSC1);
            if (ThreadShowMsg != null)
                TS.showmsg = new ShowDeleGate(ThreadShowMsg);

            List<Stock> Dealstocks = new List<Stock>(_stocks.stocks);
            if (showmsg!= null)
                TS.showmsg = showmsg;
            //Dealstocks = Dealstocks.Take(10).ToList();
            // Add 上证指数
            //TS.Tag[0].Init(_ssestock);
            //Dealstocks.Add(_ssestock);

            TS.UpdateItem(Dealstocks);
            List<Stock> stocks = Dealstocks;
            int stockcountb = stocks.Count;


            stocks = TS.Tag.Where(r => r.value < 1 && r.value > -10).Select(r => r.s).ToList();
            if (stocks.Count > 0)
            {
                TS.UpdateItem(stocks);
            }
            CompleteRunAllKData(TS);
            ThreadCompleteRun(); //输出信息
        } //暂时不用
        public void ThreadOutTxt(Tagstock tagstock, Stock s, string txt)
        {
            MFile.WriteAllText(_cfg.Baseconfig.WorkPath + "\\Data\\AllKdata\\" + s.Code + ".txt",txt);

            //string str = JsonConvert.SerializeObject(tagstock.Tag);
            //MFile.WriteAllText(_cfg.Baseconfig.WorkPath + "\\Data\\AllKdata\\" + s.Code + "_1.txt",str);
            //str = ChangeDecodeUxxx(txt);
            //MFile.WriteAllText(_cfg.Baseconfig.WorkPath + "\\Data\\AllKdata\\" + s.Code + "_2.txt", str);
        }
        private string ChangeDecodeUxxx(string input)
        {
            Regex regex = new Regex(@"\\u(\w{4})");
            string result = regex.Replace(input, delegate(Match m)
            {
                string hexStr = m.Groups[1].Value;
                string charStr = ((char)int.Parse(hexStr, System.Globalization.NumberStyles.HexNumber)).ToString();
                return charStr;
            });
            return result;
        }
        private void CompleteRunAllKData(ThreadUpdateStocksJson TS)
        {
            ;
        }
        private void CompleteRun(ThreadUpdateStocksJson TS)
        {     
            SaveJsonTag _savetag = new SaveJsonTag( System.DateTime.Now ,TS.Tag);
            _savetag.StoreDate = System.DateTime.Now;
            _savetag.Save(_cfg.Baseconfig.WorkPath + _cfg.Baseconfig.CWFilePath);	
           
        }
        
        private Stocks _stocks;
        private GlobalConfig _cfg;
        public int threadsum;
        public int threadcompletesum;       
        public ShowDeleGate showmsg;      
        public ShowDeleGate ThreadShowMsg;
        public CompleteDeleGate ThreadCompleteRun;
        public bool bshowtimeout;
        
        private string _exceptionfilename;

        public string StringDateType { get; set; }
        private string _NumType;
        internal void SetDateType(string Type)
        {
            StringDateType = Type;
            if (Type == "years")
                _NumType = "1";
            else
                _NumType = "0";
        }
    }
    public class SaveJsonTag
    {
        public SaveJsonTag(DateTime StoreDate,Tagstock[] Tag)
        {
            this.Tag = Tag;
            this.StoreDate = StoreDate;
        }
        public void Save(string path)
        {
            MFile.WriteAllText(path, JsonConvert.SerializeObject(this));
        }
        public Tagstock[] Tag { get; set; }	
        public DateTime StoreDate { get; set; }
    }
    public class FormatDataFunction
    {
        private string _datetype;
        public FormatDataFunction(string datetype)
        {
            if (datetype.EndsWith("ly"))
                datetype = datetype.Replace("ly", "");
            this._datetype = datetype;
        }
        public void FormatKDData(Tagstock tagstock, Stock s, string txt)
        {
            tagstock.Tag = ConstructKdata(s.Code, txt);
            tagstock.value = 1;
        }
        public static List<JsonMainCWFX> ConstructKdata(string stockcode, string txt)
        {
            List<Object> lso = JsonConvert.DeserializeObject<List<Object>>(txt);
            List<JsonMainCWFX> ls = lso.Select(r =>
                {
                    JsonMainCWFX k = JsonConvert.DeserializeObject<JsonMainCWFX>(r.ToString());
                    return k;
                }).ToList();

            return ls;
        }
    }
}
