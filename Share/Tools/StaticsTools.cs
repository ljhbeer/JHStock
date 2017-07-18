using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JHStock;

namespace Tools
{
    public class StaticsTools
    {
        public static double avedev(IEnumerable<double> data)
        {
            double avg = data.Average();
            return data.Select(r => r - avg > 0 ? r - avg : avg - r).Average();
        }
        public static void GetCorrectUrl(string UrlTemplate, ref string url)
        {
            if (ValidTools.ValidUrl(url) && !url.StartsWith("http://"))
            {
                if (url.Contains("/"))
                {
                    string[] paths = url.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in paths)
                    {
                        if (UrlTemplate.Contains(s))
                        {
                            url = UrlTemplate.Substring(0, UrlTemplate.LastIndexOf("/" + s)) + url;
                            break;
                        }
                    }
                }
            }
        }
        public static void DrawVol(Bitmap bmp, Stock s)
        {
            KData[] kd = s.GetKData(-1,120,true); //获得60天
            int maxvol = 0;
            foreach (KData k in kd)
                if (maxvol < k.vol)
                    maxvol = k.vol;         
            int rw = bmp.Width / kd.Length ;
            int W = bmp.Width - 10;
            int H = bmp.Height - 4;
            Pen pr = Pens.Red;
            Brush br = Brushes.Cyan;
            
            using (Graphics g = Graphics.FromImage(bmp))
            {
                double a = H*1.0 / maxvol;
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, bmp.Width, bmp.Height));
                for (int i = 0; i < kd.Length; i++)
                {
                    int y =(int)( a * kd[i].vol );
                    int x = i * W / kd.Length + 5;
                    Rectangle r = new Rectangle(x , H + 2 - y, rw, y);
                    //Console.WriteLine(i + " (" + r.Left + "," + r.Top + "," + r.Right + "," + r.Bottom + ")");
                    if (kd[i].close >= kd[i].open)
                        g.DrawRectangle(pr, r);
                    else
                        g.FillRectangle(br, r);
                }
            }
            //循环处理 
            //Console.WriteLine(s.Code + s.Name);
        }
        public static int IntDate(DateTime dt)
        {
            return dt.Year * 10000 + dt.Month * 100 + dt.Day;
        }
        
    }

}
