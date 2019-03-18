using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using System.Drawing;

namespace JHStock
{
    public static class MyExtend
    {
        public static void OutMacd(this Stock s, int daylength)
        {
            StringBuilder outstr = new StringBuilder();
            KData[] kd = s.GetKData();
            MACD[] md = MACD.ComputeMacdArray(kd);
            int i = 0;
            outstr.AppendLine("日    期\tEMAS\tEMAL\tDIF\tDEA\tMACD");
            foreach (MACD m in md)
            {
                outstr.AppendLine(kd[i].date + m.ToString());
                i++;
            }
            outstr.AppendLine("====================================End");
            MFile.WriteAllText("MACD详细数据\\" + s.Name.Replace("*", "") + s.NumCode + "debug_MACD每日数据.txt", outstr.ToString());
        }
        public static string OutMacd(this Stock s, List<KData> kd)
        {
            StringBuilder outstr = new StringBuilder();
            int daylength = kd.Count;
            MACD[] md = MACD.ComputeMacdArray(kd.ToArray());
            int i = 0;
            outstr.AppendLine("日    期\tEMAS\tEMAL\tDIF\tDEA\tMACD");
            foreach (MACD m in md)
            {
                outstr.AppendLine(kd[i].date + m.ToString());
                i++;
            }
            outstr.AppendLine();
            return outstr.ToString();
            //outstr.AppendLine("====================================End");
            //MFile.WriteAllText("MACD详细数据\\" + s.Name.Replace("*", "") + s.NumCode + "debug_MACD每日数据.txt", outstr.ToString());
        }
        public static void TestFuQuan(this Stock s, int daylength, Stocks stocks)
        {
            StringBuilder outstr = new StringBuilder();
            KData[] kd = s.GetKData(-1, daylength, true);
            for (int i = 1; i < kd.Count(); i++)
            {
                if (kd[i].low * 1.0 / kd[i - 1].close < 0.895)
                {
                    if (!s.ExistFQDate(kd[i].date) && !s.ExistFQDate(kd[i].date, kd[i - 1].date))
                    {
                        if (stocks.GetStartDatePos(kd[i].date) - stocks.GetStartDatePos(kd[i - 1].date) >= 3)
                            outstr.AppendLine(s.ID + "\t" + s.NumCode + s.Name + "\t" + kd[i].date + "\t" + kd[i].low + "\t" + kd[i - 1].date + "\t" + kd[i - 1].close + "\t" + kd[i].low * 1.0 / kd[i - 1].close + "\t停牌");
                        else
                            outstr.AppendLine(s.ID + "\t" + s.NumCode + s.Name + "\t" + kd[i].date + "\t" + kd[i].low + "\t" + kd[i - 1].date + "\t" + kd[i - 1].close + "\t" + kd[i].low * 1.0 / kd[i - 1].close);
                    }
                }
            }
            if (outstr.Length > 0)
            {
                MFile.AppendAllText(TimeStringTools.NowDate() + "复权测试-不匹配的结果.txt", outstr.ToString());
            }
        }

        public static bool ExistFQDate(this Stock s, int Date)
        {
            CFQTool FQT = s.Gcfg.FQT;
            if (FQT != null && FQT.FQ[s.ID] != null)
            {
                return FQT.FQ[s.ID].ExistFqDate(Date);
            }
            return false;
        }
        public static bool ExistDuplicateFQDate(this Stock s)
        {
            CFQTool FQT = s.Gcfg.FQT;
            if (FQT != null && FQT.FQ[s.ID] != null)
            {
                return FQT.FQ[s.ID].ExistduplicateDate();
            }
            return false;
        }
        public static bool ExistFQDate(this Stock s,int dateb, int datel)
        {
            CFQTool FQT = s.Gcfg.FQT;
            if (FQT != null && FQT.FQ[s.ID] != null)
            {
                return FQT.FQ[s.ID].ExistFqDate(dateb, datel);
            }
            return false;
        }
        public static Bitmap DrawZhenfuBmp(this Stock s, ref double avg)
        {
            Bitmap Bmp = new Bitmap(400, 50, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            KData[] kd =s. GetKData(-1, s.Gcfg.Staticsconfig.StaticDays, true); //获得60天
            List<double> ListItem = new List<double>(kd.Select(r =>
            {
                if (r.close > r.open)
                    return (r.high * 100.0 / r.low - 100);
                return -(r.high * 100.0 / r.low - 100);
            }));
            avg = ListItem.Select(r => r > 0 ? r : -r).Average();
            double max = 22;// ListItem.Max(r => r > 0 ? r : -r);
            BitmapTools.DrawBitmap(Bmp, ListItem, max);
            return Bmp;
        }
        public static Bitmap DrawZhangfuBmp(this Stock s, ref double avg)
        {
            Bitmap Bmp = new Bitmap(400, 50, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            KData[] kd = s.GetKData(-1,s.Gcfg.Staticsconfig.StaticDays, true); //获得60天
            List<double> ListItem = new List<double>();
            for (int i = 1; i < kd.Length; i++)
            {
                double vol = kd[i].close * 100.0 / kd[i - 1].close - 100;
                if (kd[i].close > kd[i - 1].close)
                    ListItem.Add(vol);
                ListItem.Add(-vol);
            }
            avg = ListItem.Average();
            double max = 10;// ListItem.Max(r => r > 0 ? r : -r);
            BitmapTools.DrawBitmap(Bmp, ListItem, max);
            return Bmp;
        }
        public static void DrawBmp(this Stock s)
        {
            if (s.Bmp == null)
            {
                s.Bmp = new Bitmap(400, 50, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                KData[] kd = s.GetKData(-1, s.Gcfg.Staticsconfig.StaticDays, true); //获得60天
                List<double> ListItem = new List<double>(kd.Select(r =>
                {
                    if (r.close > r.open)
                        return (double)(r.vol);
                    return (double)(-r.vol);
                }));
                double max = ListItem.Max(r => r > 0 ? r : -r);
                BitmapTools.DrawBitmap(s.Bmp, ListItem, max);
            }
        }
    }
}
