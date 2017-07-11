using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Tools;

namespace JHStock
{
    public abstract  class InDicator
    {
        public InDicator(Stock s)
        {
            this.stock = s;
            this.Msg = "";
            this.Isselect = false;
        }
        public abstract void Init( );
        public abstract void Compute(bool HasData,bool UnSelected);
        public virtual  bool PreSelect()
        {
            return true;
        }
        public virtual Boolean IsSelect() { return Isselect; }
        public string Msg { get; set; }
        public Stock stock;
        public Boolean Isselect;

    }
#region memo
/*
    public class MACDIndicator : InDicator
    {
        public MACDIndicator(Stock s):base(s)            
        {    
        }
        public void Init(int staticdaylenght = 200,int SHORT = 12, int LONG = 26, int MID = 9)
        {
            MACD.macdset = new MACDSET(SHORT, LONG, MID);
            this.StaticDayLength = staticdaylenght;
            LoadMv();
            int length = mv.Length > staticdaylenght + 50 ? staticdaylenght + 50 : mv.Length;
            if (mv.Length > length)
                mv = mv.Skip(mv.Length - length).ToArray();
            listmv = new List<MACD>(mv);
        }
        public override void Init()
        {
            this.Init(150);
        }
        public override bool PreSelect()//分析 预选则
        {
            return base.PreSelect();
        }
        public override void Compute(bool HasData, bool UnSelected) // 改变isSelect
        {
            Isselect = false;
            if (HasData)
            {
               ActivePrice APrice = new ActivePrice(stock.ActivePriceData);
               if (APrice.vol != 0)
               {      
                   if (listmv.Count == mv.Count())
                       listmv.Add(LatestSaveMv.Next((int)(APrice.price * 100), stock.ListData().Count - 1));
                   else
                   {
                       MACD m = LatestSaveMv.Next((int)(APrice.price * 100), stock.ListData().Count - 1);
                       listmv[listmv.Count - 1] = m;
                   }
               }
               else //停牌股 应该在预选中排除           
               {
                   if (!UnSelected)
                       return;
               }
            }

            mana = MACDACCOUNT.AnalyseMv(listmv);
            if (LatestM.DayLength < G_NOWDAYS
                && LatestM.stateChange == STATECHANGE.GTR
                && PreM.DayLength > G_PREDAYS)
            {
                Isselect = true;
                if (stock.IsGiveStocks(200))
                {
                    Isselect = false;
                }
            }
            if (UnSelected) Isselect = true;
        }

        public string OUTALLACCOUNT(List<int> listdata)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine();
            foreach(MACDACCOUNT m in mana)
                str.AppendLine(m.ToString(listdata));
            return str.ToString();
        }
        private void LoadMv()
        { // 直接计算， 不load
            float[,] macddata = stock .MacdData;
            mv = new MACD[macddata.GetLength(1) / 5];
            int sum = -1;
            for (int i = 0; i < mv.Length; i++)
            {
                mv[i] = new MACD();
                mv[i].SetData(macddata,stock.ID, 5 * i);
                if (mv[i].Date() == 0)
                {
                    sum = i;
                    break; //
                }
            }
            if (sum != -1)
            {
                Array.Resize(ref  mv, sum);
            }
        }

        public MACD LatestSaveMv { get { if (mv != null && mv.Length > 0)  return mv[mv.Count() - 1]; return null; } }
        public List<MACD> ListMv { get { return listmv; } }
        private int ActiveDayLength;
        private MACD[] mv;
        private List<MACD> listmv;
        private int StaticDayLength;
        ///////////////
        private List<MACDACCOUNT> mana;
        public static int G_NOWDAYS = 4;
        public static int G_PREDAYS = 10;
        public MACDACCOUNT LatestM { get { return mana[mana.Count - 1]; } }
        public MACDACCOUNT PreM { get { return mana[mana.Count - 2]; } }
    }
    public class MAIndicator : InDicator
    {
        public MAIndicator(Stock s):base(s)            
        {    
        }
        public void Init(int staticdaylenght = 200)
        {            
            this.StaticDayLength = staticdaylenght;
            KData[] kd = stock.GetKData(-1,staticdaylenght + 60,true);
            Stock.beginDate = kd[0].date;
            for (int i = 0; i < kd.Length; i++)
                kd[i].close =(int)( stock.FQ(kd[i]));
            //int length = kd.Length > staticdaylenght + 60 ? staticdaylenght + 60 : kd.Length;
            int skipday = kd.Length - staticdaylenght;
            List<double > ma60 = MA(0, 60, kd);
            List<double> ma20 = MA(40, 20, kd);
            List<double> ma10 = MA(50, 10, kd);
            List<double> ma5 = MA(55, 5, kd);

            //List<KData> listclose = kd.Skip(0).Take(60).ToList();
            //string str = kd.Select(r => r.date + "\t" + r.close + "\n").Aggregate((r1, r2) => r1 + r2)
            //             +"\r\nma60\t"+ ma60.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2) 
            //             +"\r\nma20\t"+  ma20.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
            //             +"\r\nma10\t"+  ma10.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
            //             +"\r\nma5\t"+  ma5.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2);
            //MFile.WriteAllText( stock.Name+ stock.NumCode + ".txt", str);
            double  pclose = ma60[0];           
            List<double> dma60 = ma60.Select( r =>
                { double  ret = r  - pclose; pclose = r ; return ret; }).ToList();
            //MFile.WriteAllText(stock.Name + stock.NumCode + ".txt", dma60.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2));
            double hafavedev = StaticsTools.avedev(dma60)*.5;
            List<int> intdma60 =  dma60.Select( r => (int)( r/hafavedev)).ToList();
            List<DivInfor> ldv = DivInfor.Analyse(intdma60);
            ldv = DivInfor.reduceAnalyse(ldv);
            List<DivInforDouble> lddv = DivInforDouble.Analyse(dma60);
            ldv = DivInfor.reduceAnalyse(ldv);

            pclose = ma60[0];
            List<double> logma60 =  ma60.Select( r =>
                { double  ret = Math.Log10( r/pclose); pclose = r ; return ret; }).ToList();
            //MFile.WriteAllText(stock.Name + stock.NumCode + "log.txt", logma60.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2));
            //MFile.WriteAllText(stock.Name + stock.NumCode + "ldv.txt", ldv.Select(r => r.index + "\t" + r.length + "\t" + r.value + "\r\n").Aggregate((r1, r2) => r1 + r2));
            //MFile.WriteAllText(stock.Name + stock.NumCode + "lddv.txt", lddv.Select(r => r.index + "\t" + r.length + "\t" + r.value + "\r\n").Aggregate((r1, r2) => r1 + r2));             
            //序列分段
            //lddv = DivInforDouble.reduceAnalyse(lddv);
            //MFile.WriteAllText(stock.Name + stock.NumCode + "rlddv.txt", lddv.Select(r => r.index + "\t" + r.length + "\t" + r.value + "\r\n").Aggregate((r1, r2) => r1 + r2));
            
            //求序列最大和子序列
            int b = 0, e = 0;
            double maxsum = maxsublinear(logma60,out b, out e);
            double maxzf = (Math.Pow(10,maxsum) -1)*100;
            MFile.AppendAllText(Config.NowDateMin()+"MonitMAIndicator最大涨幅和区间log.txt", stock.Name + "\t" + stock.Code + "\t" + maxzf + "\t " + (e - b) + "\t" + b + "\t " + e + "\t" + kd[60 + b].date + "\t" + kd[60 + e].date + "\t");
            
            List<double> flogma60 = logma60.Select(r => -r).ToList();
            double fmaxsum = maxsublinear(flogma60, out b, out e);
            double fmaxzf = (1 - Math.Pow(10, -fmaxsum) ) * 100;
            //离今天的交易天数
            int nowfardays = stock.GetNowFarDays(kd[kd.Length - 1].date);
            int leftdaymaxclose = kd.Skip(60 + e).Take(kd.Length - 60 - e).Max(r => r.close);
            MFile.AppendAllText(Config.NowDateMin()+"MonitMAIndicator最大涨幅和区间log.txt", -fmaxzf + "\t " + (e - b) + "\t" + b + "\t " + e + "\t" + kd[60 + b].date + "\t" + kd[60 + e].date + "\t" + kd[kd.Length - 1].date
                +"\t"+nowfardays+"\t"+ kd[60 + e].close +"\t"+kd[kd.Length-1].close+"\t"+leftdaymaxclose+ "\r\n");            
        }
        double maxsublinear(List<double>  a,out int b,out int e)
        {
            int i;
            int begin =0, end = 0,lbegin=0;
            double curSum = 0; ///* 当前序列和 //
            double maxSum = 0; ///* 最大序列和 //

            ////* 开始循环求子序列和 
            for (i = 0; i < a.Count; i++)
            {
                curSum = curSum + a[i];

                ///* 与最大子序列和比较，更新最大子序列和 /
                if (curSum > maxSum)
                {
                    maxSum = curSum;
                    end = i;
                    if (begin < end)
                        lbegin = begin;
                    else 
                        lbegin =lbegin+0;
                }

                ///* 动态规划部分，舍弃当前和为负的子序列 /
                if (curSum < 0)
                {
                    curSum = 0;
                    begin = i + 1 >= a.Count ? i : i + 1;

                }
            }
            b = lbegin;
            e = end;
            return maxSum;
        }
        private List<double> MA(int skip, int daylength, KData[] kd)
        {
            List<double > MA = new List<double>();
            int sum = kd.Skip(skip).Take(daylength).Sum(r => r.close);
            for (int i = skip + daylength; i < kd.Length; i++)
            {
                double  avg = sum * 1.0 / daylength;
                MA.Add(avg );
                sum += kd[i].close - kd[i - daylength].close;
            }

            return MA;
        }
        public override void Init()
        {
            this.Init();
        }
        public override bool PreSelect()//分析 预选则
        {
            return base.PreSelect();
        }
        public override void Compute(bool HasData, bool UnSelected) // 改变isSelect
        {
            Isselect = false;
            if (HasData)
            {
               ActivePrice APrice = new ActivePrice(stock.ActivePriceData);
               if (APrice.vol != 0)
               {      
                   //if (listmv.Count == mv.Count())
                   //    listmv.Add(LatestSaveMv.Next((int)(APrice.price * 100), stock.ListData().Count - 1));
                   //else
                   //{
                   //    MACD m = LatestSaveMv.Next((int)(APrice.price * 100), stock.ListData().Count - 1);
                   //    listmv[listmv.Count - 1] = m;
                   //}
               }
               else //停牌股 应该在预选中排除           
               {
                   if (!UnSelected)
                       return;
               }
            }

            //mana = MACDACCOUNT.AnalyseMv(listmv);
            //if (LatestM.DayLength < G_NOWDAYS
            //    && LatestM.stateChange == STATECHANGE.GTR
            //    && PreM.DayLength > G_PREDAYS)
            //{
            //    Isselect = true;
            //    if (stock.IsGiveStocks(200))
            //    {
            //        Isselect = false;
            //    }
            //}
            if (UnSelected) Isselect = true;
        }
        public string OUTALLACCOUNT(List<int> listdata)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine();
            //foreach(MACDACCOUNT m in mana)
            //    str.AppendLine(m.ToString(listdata));
            return str.ToString();
        }
        private int StaticDayLength;
    }
*/
#endregion

    public enum STATECHANGE { NONE, RTG, GTR };
    public struct DivInforDouble
    {
        public int index;
        public int length;
        public double  value;

        public DivInforDouble(double  value, int index)
        {
            this.index = index;
            this.value = value;
            this.length = 1;
        }
        public static List<DivInforDouble> Analyse(List<double> data)
        {
            return Analyse(data.ToArray());
        }

        private static List<DivInforDouble> Analyse(double[] data)
        {
            List<DivInforDouble> lr = new List<DivInforDouble>();
            if (data.Length > 0)
            {
                DivInforDouble r = new DivInforDouble(data[0], 0);
                for (int i = 1; i < data.Length; i++)
                {
                    if (r.IsSameState(data[i]))
                    {
                        r.length++;
                    }
                    else
                    {
                        lr.Add(r);
                        r = new DivInforDouble(data[i], i);
                    }
                }
                if (!lr.Contains(r))
                    lr.Add(r);
            }
            return lr;
        }
        private bool IsSameState(double  data)
        {
            if (data == value)
                return true;
            if (data * value == 0)
                return false;
            if (data * value > 0) return true;
            return false;
        }


        internal static List<DivInforDouble> reduceAnalyse(List<DivInforDouble> ldv)
        {
            List<DivInforDouble> r = new List<DivInforDouble>();
            int i = 0;
            for (; i < ldv.Count - 2; i++)
            {
                if (ldv[i + 1].length == 1)
                {
                    if(  ldv[i].value*ldv[i+2].value >0 ){
                        DivInforDouble n = new DivInforDouble(ldv[i].value,ldv[i].index);
                        double value = ldv[i].value;
                        n.length = ldv[i].length + 1 + ldv[i + 2].length;
                        i += 2;
                        while (true)
                        {
                            if (i < ldv.Count - 2 && ldv[i + 1].length == 1 && value * ldv[i + 2].value > 0)
                            {
                                n.length += 1 + ldv[i + 2].length;
                                i += 2;
                            }
                            else
                            {
                                r.Add(n);
                                break;
                            }
                        }
                    }else{
                        r.Add( ldv[i]);
                    }
                }
                else
                {
                    r.Add( ldv[i]);
                }

            }
            if (i == ldv.Count - 2)
            {
                r.Add(ldv[i]);
                r.Add(ldv[i + 1]);
            }
            return r;
         
        }
    }
    public struct DivInfor
    {
        public int index;
        public int length;
        public int value;
       
        public DivInfor(int value,int index)
        {
            this.index = index;
            this.value = value;
            this.length = 1;
        }
        public static List<DivInfor> Analyse(List<int> data)
        {
            return Analyse(data.ToArray());
        }

        private static List<DivInfor> Analyse(int[] data)
        {
            List<DivInfor> lr = new List<DivInfor>();
            if (data.Length > 0)
            {
                DivInfor r = new DivInfor(data[0],0);
                for (int i = 1; i < data.Length; i++)
                {
                    if (r.IsSameState(data[i]))
                    {
                        r.length++;
                    }
                    else 
                    {
                        lr.Add(r);
                        r = new DivInfor(data[i],i);
                    }
                }
                if (!lr.Contains(r))
                    lr.Add(r);
            }
            return lr;
        }
        private bool IsSameState(int data)
        {
            if (data == value)
                return true;
            if (data * value == 0)
                return false;
            if (data * value > 0) return true;
            return false;
        }


        internal static List<DivInfor> reduceAnalyse(List<DivInfor> ldv)
        {
            return ldv;
            throw new NotImplementedException();
        }
    }
    public struct MACDACCOUNT
    {
        public MACDACCOUNT(MACD macd, STATECHANGE sTATECHANGE= STATECHANGE.NONE)
        {
            lm = new List<MACD>(){macd};         
            stateChange = sTATECHANGE;
        }
        public void PushMacd(MACD m)
        {
            lm.Add(m);
        }
        public void PushMacd(MACD[] m)
        {
            lm.AddRange(m);
        }
        public void PushMacd(List<MACD> m)
        {
        	lm.AddRange(m.ToArray());
        }
        public MACD PopMacd()
        {
            if (lm.Count > 0)
            {
                MACD m = lm[lm.Count - 1];
                lm.RemoveAt(lm.Count - 1);
                return m;
            }
            return null;
        }

        public static List<MACDACCOUNT> AnalyseMv(MACD[] mv)
        {
            
            List<MACDACCOUNT> macdcnt = new List<MACDACCOUNT>();
            MACDACCOUNT mcnt = new MACDACCOUNT(mv[0]);
            for (int i = 1; i <mv.Length; i++)
            {
                if (mcnt.State * mv[i].Bar > 0)
                {
                    mcnt.PushMacd(mv[i]);
                }
                else if (mcnt.State * mv[i].Bar < 0)
                {
                    macdcnt.Add(mcnt);
                    if (mv[i].Bar > 0)
                        mcnt = new MACDACCOUNT(mv[i], STATECHANGE.GTR);
                    else
                        mcnt = new MACDACCOUNT(mv[i], STATECHANGE.RTG);
                }
            }
            if (!macdcnt.Contains(mcnt))
                macdcnt.Add(mcnt);
            return macdcnt;
        }
        public static List<MACDACCOUNT> AnalyseMv(List<MACD> listmv)
        {
            return AnalyseMv(listmv.ToArray());
        }
        public static List<MACDACCOUNT> AnalyseMv(MACD[] mv, int startpos, int length)
        {
            if (startpos < 0)
                startpos = 0;
            if (startpos + length < mv.Length)
                length = mv.Length - startpos;

            List<MACDACCOUNT> macdcnt = new List<MACDACCOUNT>();
            MACDACCOUNT mcnt = new MACDACCOUNT(mv[startpos]);
            for (int i = startpos + 1; i < startpos + length; i++)
            {
                if (mcnt.State * mv[i].Bar > 0)
                {
                    mcnt.PushMacd(mv[i]);
                }
                else if (mcnt.State * mv[i].Bar < 0)
                {
                    macdcnt.Add(mcnt);
                    if (mv[i].Bar > 0)
                        mcnt = new MACDACCOUNT(mv[i], STATECHANGE.GTR);
                    else
                        mcnt = new MACDACCOUNT(mv[i], STATECHANGE.RTG);
                }
            }
            if (!macdcnt.Contains(mcnt))
                macdcnt.Add(mcnt);
            return macdcnt;
        }
        public static List<MACDACCOUNT> reduceAnalyse(List<MACDACCOUNT> ldv)
        {
            List<MACDACCOUNT> r = new List<MACDACCOUNT>();
            int i = 0;
            for (; i < ldv.Count - 2; i++)
            {
                if (ldv[i + 1].DayLength == 1)
                {
                    if(  ldv[i].State*ldv[i+2].State >0 ){
                		MACDACCOUNT n = ldv[i];
                		double value = ldv[i].State;
                		n.PushMacd(ldv[i+1].lm);
                		n.PushMacd(ldv[i+2].lm);                       
                        i += 2;
                        while (true)
                        {
                            if (i < ldv.Count - 2 && ldv[i + 1].DayLength == 1 && value * ldv[i + 2].State > 0)
                            {
                                n.PushMacd(ldv[i+1].lm);
                				n.PushMacd(ldv[i+2].lm);    
                                i += 2;
                            }
                            else
                            {
                                r.Add(n);
                                break;
                            }
                        }
                    }else{
                        r.Add( ldv[i]);
                    }
                }
                else
                {
                    r.Add( ldv[i]);
                }

            }
            if (i == ldv.Count - 2)
            {
                r.Add(ldv[i]);
                r.Add(ldv[i + 1]);
            }
            return r;
         
        }
        public override string ToString()
        {
            return "\t" + StartPos + "\t" + DayLength + "\t" + stateChange.ToString();
        }
        public string ToString(List<int> listdata)
        {
            if(StartPos <listdata.Count)
                return listdata[StartPos] + "\t" + DayLength + "\t" + stateChange.ToString();
            return listdata[listdata.Count-1] + "\t" + DayLength + "\t" + stateChange.ToString();
        }
        public string ToString(int ActiveDayLength)
        {
            return "\t" + (State>0) + "\t" + (DayLength + ActiveDayLength) + "\t" + stateChange.ToString();
        }

        private double State { get { return lm[0].Bar; } }
        public int StartPos { get { return lm[0].Date(); } }
        public int DayLength { get { return lm.Count; } } //积累天数
        public STATECHANGE stateChange; //变化
        public List<MACD> lm;


    }
    public class ExChangeStatusCheck
    {
        public ExChangeStatusCheck()
        {
            Init();
        }
        private void Init()
        {
            InLine = false;
            ExChanging = false;
            ExChangDay = false;
            SinaLastestDate = XmlLatestDate = 0;
        }
        public bool StatusCheck(Stocks _stocks, ref string Msg)
        {
            Init();
            if (_stocks.MacdData == null)
            {
                Msg = "无法载入MacdData";
                return false;
            }
//            if (!_stocks.CheckMacdSameToSHDate())
//            {
//                Msg = "Macd最新日期与上证数据日期不符，请确认更新盘后数据，并从新生成macdData 后重试，";  //无需测试
//                return false;
//            }
//            TestNet();
//            if (!InLine)
//                Msg = "目前处于脱机使用状态";
//            else
//            {
//                Msg = "在线";
//                if (SinaLastestDate == 0 || XmlLatestDate == 0)
//                {
//                    Msg = "网络获取障碍，无法获取正确的网络数据，请检查网络";
//                    return false;
//                }
//
//                if (SinaLastestDate == XmlLatestDate)
//                    ExChangDay = false;
//                else if (SinaLastestDate > XmlLatestDate) 
//                    ExChangDay = true;
//
//                if (_stocks.MacdLatestDay == SinaLastestDate || _stocks.MacdLatestDay== XmlLatestDate )
//                {
//                    if (ExChangDay)
//                    {
//                        _stocks.ListDate.Add(SinaLastestDate);////
//                    }
//                    Msg = "数据为最新数据";
//                    return true;
//                }
//                else
//                {
//                    Msg = "上证数据没有更新，请确认更新盘后数据，并从新生成macdData 后重试";
//                    return false;
//                }
//            }
            return true;
        }        
        public bool InLine { get; set; }
        public bool ExChanging { get; set; }
        public bool ExChangDay { get; set; }
        public bool bError { get; set; }
        public int XmlLatestDate { get; set; }
        public int SinaLastestDate { get; set; }
        private CWeb web = new CWeb();
        private void TestNet()
        {
            try
            {
                InLine = false;
                ExChanging = false;
                string html = web.GetOKUrl("http://datapic.eastmoney.com/xml/rzrq/sh.xml");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(html);
                string newistday = doc.DocumentElement.ChildNodes[0].LastChild.InnerText;
                System.DateTime dt = System.DateTime.Parse(newistday);
                XmlLatestDate = dt.Year * 10000 + dt.Month * 100 + dt.Day;
                //string prenewistday = doc.DocumentElement.ChildNodes[0].LastChild.PreviousSibling.InnerText;
                //dt = System.DateTime.Parse(prenewistday);
                //XmlPreLatestDate = dt.Year * 10000 + dt.Month * 100 + dt.Day;

                html = web.GetOKUrl("http://hq.sinajs.cn/list=sh000001");
                string[] ss = html.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length < 30)
                {
                    bError = true;
                    return;
                }
                dt = System.DateTime.Parse(ss[30] + " " + ss[31]);
                SinaLastestDate = dt.Year * 10000 + dt.Month * 100 + dt.Day;
                int time = dt.Hour * 100 + dt.Minute;

                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday
                  && (time > 930 && time < 1500))
                    ExChanging = true;
                InLine = true;
            }
            catch
            {
                bError = true;
            }
            bError = false;
        }       
    }
}
