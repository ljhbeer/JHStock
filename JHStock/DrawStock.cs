using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace JHStock
{
    public class StockDraw
    {
        public static JHStock.JSConfig Jscfg;

        public static void DrawDaily(string url, string imgname, Stock s, ColorStyle cs ,Size size) 
        {
            // if (_stockdata.Netdate.Inline)
            //TODO: DrawDaily by url
            KData[] kd = ThreadUpdateStocksQQDayly.DownLoadData(url, s);
          
            Bitmap bmp = DrawDaily(kd, s.Name + s.NumCode,size,cs); //size(1200,840)
            if (kd != null && kd.Length != 0)
            {
                bmp = DrawDaily(kd, s.Name + s.Code + "(日线" + kd[0].date + "-" + kd[kd.Length - 1].date + ")",bmp.Size, cs);
                string baseinfor = "";
                try
                {
                     baseinfor = UpdateMonitInfor.GetCWXXS(s);
                }
                catch { }

                Rectangle rectinformation = new Rectangle((int)(bmp.Width * 0.70 + 16), 20, (int)(bmp.Width * 0.30 - 20), bmp.Height - 40); //informationrect
                DrawBaseInformation(s, baseinfor, bmp,rectinformation, cs);
            }
            bmp.Save(imgname);
        }
        public static Bitmap DrawDailyLocal(Stock s, ColorStyle cs)
        {
            Bitmap bmp = new Bitmap(1200, 840);
            if (Jscfg == null)
                return bmp;
            StocksData _stockdata = Jscfg.globalconfig.StocksData;
            if (s != null && _stockdata.SaveTag.Tag[s.ID] != null)
            {
                KData[] kd = _stockdata.SaveTag.Tag[s.ID].kd.ToArray();
                if (kd != null && kd.Length != 0)
                {
                    bmp=DrawDaily(kd, s.Name + s.Code + "(日线" + kd[0].date + "-" + kd[kd.Length - 1].date + ")",bmp.Size, cs);
                    string baseinfor = "";
                    try
                    {
                        if (_stockdata.Netdate.Inline)
                            baseinfor = UpdateMonitInfor.GetCWXXS(s);
                    }
                    catch{ }
                    Rectangle rectinformation = new Rectangle((int)(bmp.Width * 0.70 + 16), 20, (int)(bmp.Width * 0.30 - 20), bmp.Height - 40); //informationrect
                    DrawBaseInformation(s, baseinfor, bmp,rectinformation, cs);
                }
            }
            return bmp;
        }
        public static Bitmap DrawDaily(KData[] kd, string namecode,Size size, ColorStyle cs) //标准划分
        {
            int width = size.Width;
            int height = size.Height;
            Rectangle r = new Rectangle(0, 0, width, height);
            Rectangle ir = new Rectangle(20, 20, width - 40, height - 40);
           
            Rectangle pricerect = new Rectangle(ir.Location,new Size( (int)( ir.Width*0.70), (int)(ir.Height*0.70)) );
            Rectangle volrect = new Rectangle(ir.X, pricerect.Bottom + 10, pricerect.Width, (int)(ir.Height * 0.30 - 35));//new Rectangle(20, 630, 920, 160);
            Rectangle daterect =new Rectangle(ir.X, volrect.Bottom , pricerect.Width, 25);// new Rectangle(20, 790, 920, 25);
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp)) //drawground
            {
                g.FillRectangle(cs.backgroundbrush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                g.DrawString(namecode, cs.textfont, cs.textlablebrush, 20, 0);
                g.DrawRectangle(cs.textpricelinepen, pricerect);
                g.DrawRectangle(cs.textpricelinepen, volrect);
                g.DrawRectangle(cs.textpricelinepen, daterect);
            }

            DrawBitmapPrice(kd, bmp, pricerect, cs); // Drawprice  
            DrawBitmapVol(kd, bmp, volrect, cs);//
            DrawBitmapDate(kd, bmp, daterect, cs);

            return bmp;
        }

        private static void DrawBitmap(Bitmap bmp, List<Rectangle> ListItem, List<bool> isred, ColorStyle cs)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < ListItem.Count; i++)
                {
                    g.FillRectangle(cs.backgroundbrush, ListItem[i]);

                    if (isred[i])
                    {
                        if (ListItem[i].Width == 1)
                            g.DrawLine(cs.klinerosepen, ListItem[i].X, ListItem[i].Y, ListItem[i].X, ListItem[i].Bottom);
                        else
                            g.DrawRectangle(cs.klinerosepen, ListItem[i]);
                    }
                    else
                    {
                        if (ListItem[i].Width == 1)
                            g.DrawLine(cs.klinedeclinepen, ListItem[i].X, ListItem[i].Y, ListItem[i].X, ListItem[i].Bottom);
                        else
                            g.FillRectangle(cs.klinedeclinebush, ListItem[i]);
                    }
                }
            }
        }
        private static void DrawBitmapLinesAndPriceText(Bitmap bmp, List<Rectangle> Lines, List<string> txts, ColorStyle cs)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < Lines.Count; i++)
                {
                    g.DrawLine(cs.textpricelinepen2, Lines[i].X, Lines[i].Y, Lines[i].Right, Lines[i].Bottom);
                    g.DrawString(txts[i], cs.textfont2, cs.textpricelinebrush, Lines[i].Right, Lines[i].Top-7);
                }
            }
        }
        private static bool DrawBitmapPrice(KData[] kd, Bitmap bmp, Rectangle rect, ColorStyle cs)
        {
            //TODO: UnDraw Date
            if (bmp == null || kd == null || kd.Count() == 0 || rect.X + rect.Width > bmp.Width || rect.Y + rect.Height > bmp.Height)
                return false;
            int startx = rect.X;
            int starty = rect.Y;
            int width = rect.Width;
            int height = rect.Height;

            int maxprice = kd.Max(r => r.high);
            int minprice = kd.Min(r => r.low);
            int pricemaxplusmin = maxprice - minprice;
            double dx = width * 1.0 / kd.Count();
            double dy = height * 1.0 / pricemaxplusmin;
            int i = 0;
            List<Rectangle> hlr = kd
                .Select(r =>
                {
                    int x = (int)(i * dx + dx * 0.5) + startx;
                    int y = (int)((maxprice - r.high) * dy) + starty;
                    int h = (int)((r.high - r.low) * dy);
                    h = h > 0 ? h : 1;
                    Rectangle trect = new Rectangle(x, y, 1, h);
                    i++;
                    return trect;
                }).ToList();
            i = 0;
            List<Rectangle> ocr = kd
                .Select(r =>
                {
                    int x = (int)(i * dx + dx * 0.2) + startx;
                    int y = (int)((maxprice - Math.Max(r.open, r.close)) * dy) + starty;
                    int h = (int)((Math.Abs(r.open - r.close)) * dy);
                    h = h > 0 ? h : 1;
                    int w = (int)(dx * 0.6 + 0.5) > 0 ? (int)(dx * 0.6 + 0.5) : 1;
                    Rectangle trect = new Rectangle(x, y, w, h);
                    i++;
                    return trect;
                }).ToList();

            //Drawprice
            List<Rectangle> Lines = new List<Rectangle>();
            List<string> pricetxt = new List<string>();
            for (i = 0; i < 11; i++)
            {
                Lines.Add(new Rectangle(rect.X, rect.Y + (int)(rect.Height * i / 10.0), rect.Width, 1));
                pricetxt.Add(((maxprice - pricemaxplusmin * i / 10.0) / 100.0).ToString("0.00"));
            }
            DrawBitmapLinesAndPriceText(bmp, Lines, pricetxt, cs);
            // DrawKLine
            List<bool> lb = kd.Select(r => r.open < r.close).ToList();
            DrawBitmap(bmp, hlr.Concat(ocr).ToList(), lb.Concat(lb).ToList(), cs);
            return true;
        }
        private static bool DrawBitmapVol(KData[] kd, Bitmap bmp, Rectangle rect, ColorStyle cs)
        {
            //TODO: DrawVol
            if (bmp == null || kd == null || kd.Count() == 0 || rect.X + rect.Width > bmp.Width || rect.Y + rect.Height > bmp.Height)
                return false;
            int startx = rect.X;
            int starty = rect.Y;
            int width = rect.Width;
            int height = rect.Height;

            int max = kd.Max(r => r.vol);
            int min = kd.Min(r => r.vol);
            int max10_e = (int)( Math.Log10(max));
            //int min10_e = (int)( Math.Log10(min));
            //int e_10 = max10_e - 2;
            //if (min10_e > e_10)
            //    e_10++;
            //if (min10_e > e_10)
            //    e_10++;
            double pow_10 =  Math.Pow(10,max10_e);

            double dx = width * 1.0 / kd.Count();
            double dy = height * 1.0 / max;
            int i = 0;
            List<Rectangle> hlr = kd
                .Select(r =>
                {
                    int x = (int)(i * dx) + startx;
                    int y = (int)((max - r.vol) * dy) + starty;
                    int h = (int)(r.vol * dy);
                    h = h > 0 ? h : 1;
                    int w = (int)(dx + 0.5);
                    int gap = w / 4 > 0 ? w / 4 : 1;
                    w = w - gap > 0 ? w - gap : 1;
                    Rectangle trect = new Rectangle(x, y, w, h);
                    i++;
                    return trect;
                }).ToList();
            //DrawVolTest
            List<Rectangle> Lines = new List<Rectangle>();
            List<string> pricetxt = new List<string>();
            for (i = 0; i < 4; i++)
            {
                Lines.Add(new Rectangle(rect.X, rect.Y + (int)(rect.Height * i / 4.0), rect.Width, 1));
                pricetxt.Add(((max -  max * i / 4.0) /pow_10).ToString("0.00"));
            }
            Lines.Add(new Rectangle(rect.X, rect.Bottom, rect.Width, 1));
            pricetxt.Add("x10^" + max10_e);
            
            DrawBitmapLinesAndPriceText(bmp, Lines, pricetxt, cs);
            // 
            //pricetxt.Add("x" + pow_10.ToString("0"));
            // DrawKLine
            List<bool> redgreen = kd.Select(r => r.open < r.close).ToList();
            DrawBitmap(bmp, hlr, redgreen, cs);
            return true;
        }
        private static bool DrawBitmapDate(KData[] kd, Bitmap bmp, Rectangle rect, ColorStyle cs)
        {
            if (bmp == null || kd == null || kd.Count() == 0 || rect.X + rect.Width > bmp.Width || rect.Y + rect.Height > bmp.Height)
                return false;
            //List<int> years = kd.Select(r => r.date/10000).Distinct().ToList();            
            List<List<int>> monthcount = ComputeYearMonthPostion(kd);           
            IFormatProvider format = new System.Globalization.CultureInfo("zh-CN");
            List<DayOfWeek> dayofweek = kd.Select(r => DateTime.ParseExact(r.date.ToString(), "yyyyMMdd", format).DayOfWeek).ToList();

            int sum = 0;
            double dx = rect.Width * 1.0 / kd.Length;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int k = 0; k < monthcount.Count; k++)
                {
                    int X = (int)(dx * sum + 0.5) + rect.X;
                    string str = monthcount[k][0].ToString();
                    g.DrawLine(cs.textpricelinepen2, X, rect.Top, X, rect.Bottom);
                    g.DrawString(str, cs.textfont, cs.textpricelinebrush, X, rect.Y);
                    sum += monthcount[k][1];
                }
                DayOfWeek nowdofw = dayofweek[0];
                for (int k = 1; k < dayofweek.Count; k++)
                {
                    if (nowdofw > dayofweek[k]) //new week
                    {
                        int X = (int)(dx * k + 0.5) + rect.X;
                        string str = (kd[k].date % 100).ToString();
                        g.DrawLine(cs.textpricelinepen2, X, rect.Top, X, rect.Top + rect.Height / 2);
                        g.DrawString(str, cs.textfont2, cs.textpricelinebrush, X, rect.Top + rect.Height / 2);
                    }
                    nowdofw = dayofweek[k];
                }
            }
            return true;
        }
        private static List<List<int>> ComputeYearMonthPostion(KData[] kd)
        {
            var query = from p in kd
                        group p by p.date / 100 into g
                        select g;
            List<List<int>> monthcount = new List<List<int>>();
            foreach (var qu in query)
                monthcount.Add(new List<int>() { qu.Key, qu.Count() });
            int count = monthcount.Sum(r => r[1]) - kd.Count();
            ////string str= count+"\r\n"+ monthcount.Select( r => r[0]+"\t"+r[1]).Aggregate((r1, r2) => r1 + "\r\n" + r2);
            ////MessageBox.Show(str);
            if (count != 0) return new List<List<int>>();
            int nowyear = 0;
            for (int k = 0; k < monthcount.Count; k++)
            {
                if (nowyear == monthcount[k][0] / 100)
                    monthcount[k][0] = monthcount[k][0] % 100;
                else
                {
                    nowyear = monthcount[k][0] / 100;
                    monthcount[k][0] = nowyear;
                }
            }
            return monthcount;
        }
        private static void DrawBaseInformation(Stock s, string baseinfor, Bitmap bmp, Rectangle rect, ColorStyle cs)
        {
            int width = bmp.Width;
            int height = bmp.Height;           
            Rectangle rectinfor = new Rectangle(rect.X, rect.Y + 30, rect.Width, rect.Height - 30);
            using (Graphics g = Graphics.FromImage(bmp)) //drawground
            {
                g.DrawRectangle(cs.textpricelinepen, rect);
                g.DrawString(s.Name + s.Code, cs.textfont, cs.textlablebrush, rect.Location);
                g.DrawString(baseinfor, cs.textfont2, cs.textlablebrush, rectinfor);
            }

        }
    }
    public class ColorStyle
    {
        public ColorStyle(Color background, Color klinerose, Color klinedecline, Color textprice, Color textlable, Font defaultfont = null)
        {
            backgroundbrush = new SolidBrush(background);
            backgroundpen = new Pen(background);
            klinedeclinebush = new SolidBrush(klinedecline);
            klinedeclinepen = new Pen(klinedecline);
            klinerosebrush = new SolidBrush(klinerose);
            klinerosepen = new Pen(klinerose);
            textpricelinepen = new Pen(textprice);
            textpricelinepen2 = new Pen(textprice);
            textpricelinepen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            textpricelinepen2.DashPattern = new float[] { 1f, 1f };

            textpricelinebrush = new SolidBrush(textprice);
            textlablepen = new Pen(textlable);
            textlablebrush = new SolidBrush(textlable);
            if (defaultfont == null)
                textfont = new Font("宋体", 12);
            else
                textfont = defaultfont;

            textfont2 = new Font("宋体", 9);
        }
        public Brush backgroundbrush;
        public Pen backgroundpen;
        public Brush klinerosebrush;
        public Pen klinerosepen;
        public Brush klinedeclinebush;
        public Pen klinedeclinepen;
        public Brush textpricelinebrush;
        public Pen textpricelinepen;
        public Pen textpricelinepen2; //绘制虚线
        public Brush textlablebrush;
        public Pen textlablepen;
        public Font textfont;
        public Font textfont2;
    }
    public class ColorStyles
    {
        public static ColorStyle classic = new ColorStyle(Color.Black, Color.Red, Color.FromArgb(128, 255, 255), Color.Red, Color.White);
        public static ColorStyle print = new ColorStyle(Color.White, Color.Black, Color.Gray, Color.Black, Color.Gray);
    }
}
