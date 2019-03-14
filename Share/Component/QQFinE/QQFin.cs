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
    public class QQFin
    {
        public QQFin(Stocks stocks)
        {
            _stocks = stocks;
            _cfg = _stocks.Gcfg;      
            tupdate = new ThreadUpdateStocksQQFin(stocks);
        }
        public void Test(){
        	string path = @"E:\Project\Source\Stock\Data\QQFin\SH60000[id].txt"; //  5  6 7 8
            string txt = File.ReadAllText(path.Replace("[id]","4"));
		            txt ="["+ btItemDJCW.BEPos(txt).String+"]";
		            txt = txt.Replace("--","0");           
            List<DJCW> c5 = JsonConvert.DeserializeObject<List<DJCW>>(txt);
            txt +="";
        }
        public  ThreadUpdateStocksQQFin tupdate;
        private Stocks _stocks;
        private GlobalConfig _cfg;  

        private ToolsCXml.BETag btyear = new ToolsCXml.BETag("[nflb\":#@#@-@#@#]");
        private ToolsCXml.BETag btItemDJCW =  new ToolsCXml.BETag("[item=#@#@-@#@#]".Replace("item","DJCW"));
        private List<string> UpdateItemNames = new List<string>(){"MGZB","YLNL","YYNL","CZNL","DJCW"};
//        private string  UrlTemplate = "http://comdata.finance.gtimg.cn/data/[itemname]/[scode]/[year]";        
    }

}
