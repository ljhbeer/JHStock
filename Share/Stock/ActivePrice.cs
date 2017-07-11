using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;

namespace JHStock
{
    public class CActivePrices
    {
        public CActivePrices(Stocks stocks)
        {
            this.stocks = stocks;
            this.cfg = stocks.Gcfg;
            Dicprice = new Dictionary<int, CActivePrice>();
        }
        public void InitPriceFromNet(List<Stock> ls)
        {
            Dicprice.Clear();
            string html = "";
            int sum = 0;
            string url = "";
            foreach (Stock s in ls)
            {
                url += s.Code + ",";
                sum++;
                if (sum % 900 == 0)
                {
                    html += CWeb.GetWebClient("http://hq.sinajs.cn/list=" + url.ToLower());
                    url = "";
                }
            }
            if (sum % 900 != 0)
                html += CWeb.GetWebClient("http://hq.sinajs.cn/list=" + url.ToLower());
            html = html.Replace(",", "\t").Replace("var hq_str_", "")
                  .Replace("=\"", "").Replace("\";", "");
            string[] item = html.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (item.Length >= 1)
            {
                string time = item[0].Split(new string[]{"\t"}, StringSplitOptions.RemoveEmptyEntries)[0];
                string[] value = item[0].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (value.Length == 33)
                {
                    time = value[value.Length - 2];
                    if (time.Contains(":"))
                    {
                        //int mins = GetMins(time);
                        int h = Convert.ToInt32(time.Substring(0, 2));
                        int m = Convert.ToInt32(time.Substring(3, 2));
                        int mins = 0;
                        int now = h*100+m ;
                        if (now > 930 && now < 1131)
                        {
                            mins = (h - 9) * 60 + m - 30+5;
                        }
                        else if (now > 1130 && now < 1300)
                        {
                            mins = 120;
                        }
                        else if (now > 1300 && now <= 1500)
                        {
                            mins = (h - 11) * 60 + m;
                        }
                        else if (now > 1500)
                        {
                            mins = 240;
                        }
                        else
                        {
                            mins = 0;
                        }
                        cfg.Tempconfig.NowMins = mins;
                    }
                }
            }
            foreach (string str in item)
            {
                Stock s = stocks.StockByNumCode(str.Trim().Substring(2, 6));
                Dicprice[s.ID] = new CActivePrice(cfg,s,str);
            }
        }
        public string ActivePriceFromNet()
        {           
            //string str = "Index\t股票代码\t股票名称\tNow\r\n";
            string html = "";
            int sum = 0;
            string url = "";
            foreach (Stock s in stocks.stocks)
            {
                url += s.Code + ",";
                sum++;
                if (sum % 900 == 0)
                {
                    html +=  CWeb.GetWebClient("http://hq.sinajs.cn/list=" + url.ToLower());
                    url = "";
                }
            }
            if(sum%900!=0)
                html += CWeb.GetWebClient("http://hq.sinajs.cn/list=" + url.ToLower());
            //string[] item = html.Split('\r');//,StringSplitOptions.RemoveEmptyEntries
            html = html.Replace("\n", "\r\n").Replace(",", "\t").Replace("var hq_str_", "")
                  .Replace("=\"", "").Replace("\";", "");
            return html;
        }
       
        private Stocks stocks;
        private GlobalConfig  cfg;
        public Dictionary<int, CActivePrice> Dicprice;
    }
    public class CActivePrice
    {
        private Stock _s;
        private GlobalConfig _gcfg;
        public CActivePrice(GlobalConfig gcfg,Stock s,string apdstr="")
        {
            this._gcfg = gcfg;
            this._s = s;
            this.ActivePriceData = apdstr;
            if (this.ActivePriceData != "")
                InitActivePrice();
        }
        public void InitActivePrice()
        {
            HasActivePrice = false;
            if (ActivePriceData == null) return;
            APrice = new ActivePrice(ActivePriceData);
            if (!APrice.OK) return;
            HasActivePrice = APrice.OK;
            //重新计算Vol
            APrice.vol = (long)(APrice.vol * 240.0 / _gcfg.Tempconfig.NowMins);
            //
            HasOldPrice = false;
            KData[] allclose =_s.GetKData(-1, 20, true);
            if (allclose.Length < 19) return;
            float wrate = 0;
            int wrank = 0;
            float mrate = 0;
            float mrank = 0;
            int E = allclose.Length - 1;
            for (int i = 0; i < 5; i++)
            {
                wrate += allclose[E - i].vol;
                if (allclose[E - i].vol <= APrice.vol)
                    wrank++;
            }
            for (int i = 0; i < 20; i++)
            {
                mrate += allclose[i].vol;
                if (allclose[i].vol <= APrice.vol)
                    mrank++;
                //Console.WriteLine(allclose[i].date + "\t" + allclose[i].vol);
            }
            APrice.wrank = wrank;
            APrice.wrate = wrate;
            APrice.mrank = mrank;
            APrice.mrate = mrate;
            APrice.yesdayvol = allclose[E].vol;
            HasOldPrice = true;
        }
        public bool HasOldPrice { get; set; }
        public bool HasActivePrice { get; set; }
        public ActivePrice APrice { get; set; }
        public string ActivePriceData { get; set; }
    }
    public class ActivePrice
    {
        public ActivePrice(string ActivePriceData)
        {
            OK = false;
            string[] itemvalues = ActivePriceData.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            if (itemvalues.Count() < 20) return;
            if (itemvalues[1] == "0") return;
            price = Convert.ToDouble(itemvalues[3]);
            o = Convert.ToSingle(itemvalues[1]);
            zc = Convert.ToSingle(itemvalues[2]);
            c = Convert.ToSingle(itemvalues[3]);
            h = Convert.ToSingle(itemvalues[4]);
            l = Convert.ToSingle(itemvalues[5]);
            vol = Convert.ToInt64(itemvalues[8]);
            amount = Convert.ToSingle(itemvalues[9]);
            OK = true;
        }

        public double price { get; set; }
        public float o { get; set; }
        public float zc { get; set; }
        public float c { get; set; }
        public float h { get; set; }
        public float l { get; set; }
        public long vol { get; set; }
        public float amount { get; set; }
        public bool OK { get; set; }

        public int wrank { get; set; }
        public float wrate { get; set; }
        public float mrank { get; set; }
        public float mrate { get; set; }
        public double yesdayvol { get; set; }
    }
}
