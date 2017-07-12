using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tools;
using JHStock.Update;

namespace JHStock
{
    public partial class FormMonit : Form
    {
        private Stocks _stocks;
        private ExChangeStatusCheck exchangestatus;
        private string type;
        private DataTable dt;
        private JHStock.Update.tagstock[] tags;

        public FormMonit(Stocks _stocks, ExChangeStatusCheck exchangestatus, string type, JHStock.Update.tagstock[] tags)
        {
            this._stocks = _stocks;
            this.exchangestatus = exchangestatus;
            this.type = type;
            this.tags = tags;
            selectstockindex = new List<int>();
            InitializeComponent();
            InitMaDataTable();
        }        
        private void buttonAddToTXDBlock_Click(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;
            string str = "";
            foreach (DataGridViewRow dr in dgv.Rows)
            {

                string code = (string)dr.Cells["代码"].Value;
                if (code != null && code.StartsWith("SZ"))
                    str += "\r\n0" + code.Substring(2);
                else if (code != null && code.StartsWith("SH"))
                    str += "\r\n1" + code.Substring(2);
            }

            MFile.WriteAllText(_stocks.Gcfg.Baseconfig.BlockPath(), str);
            MessageBox.Show("已输出");
            ((Button)sender).Enabled = true;
        }
        private void buttonToTxt_Click(object sender, EventArgs e)
        {
            StringBuilder outstr = DataTableToString(dt);
            string filesavename = Tools.TimeStringTools.NowDateMin() + "_Monit" + type + ".txt";
            if (checkBoxTable.Checked)
                outstr.Replace(',', '\t');
            MFile.AppendAllText(filesavename, outstr.ToString());
            MessageBox.Show("已输出到文件中：" + filesavename);
        }
        private void buttonReCompute_Click(object sender, EventArgs e)
        {
            selectstockindex.Clear();
            foreach (Stock s in _stocks.stocks)
            {
                TestStock(s);
            }
            ///show selectstock in the Table
        }
        private StringBuilder DataTableToString(DataTable dtsave)
        {
            StringBuilder outstr = new StringBuilder();
            foreach (DataColumn dc in dtsave.Columns)
            {
                outstr.Append(dc.ColumnName + ",");
            }
            outstr.Append("\r\n");
            foreach (DataRow dr in dtsave.Rows)
            {
                foreach (DataColumn dc in dtsave.Columns)
                {
                    if (dc.DataType != typeof(Image))
                        outstr.Append(dr[dc.ColumnName] + ",");
                    else
                        outstr.Append("image,");

                }
                outstr.Append("\r\n");
            }
            outstr = outstr.Replace(",\r\n", "\r\n");
            return outstr;
        }
        private void InitMaDataTable()
        {
            dt = new DataTable();
            List<string> columntitles = new List<string>() { "名称", "代码", "日期", "上一状态持续天数", "状态", "持续日期", "均值", "杂项" };//,"杂项"
            //columntitles = new List<string>() { "名称", "代码","杂项" };//,"杂项"
            for (int count = 0; count < columntitles.Count; count++)
            {
                DataColumn dc = new DataColumn(columntitles[count]);
                if ("代码名称状态杂项".Contains(columntitles[count]))
                {
                    dc.DataType = typeof(string);
                    //dc.MaxLength = 60;
                }
                else if ("日期上一状态持续天数持续日期".Contains(columntitles[count]))
                {
                    dc.DataType = typeof(int);
                    //dc.MaxLength = 20;
                }
                else
                {
                    dc.DataType = typeof(string);
                    //dc.MaxLength = 30;
                }
                dt.Columns.Add(dc);
            }
            dgv.DataSource = dt;
        }
  //List<KData> listclose = kd.Skip(0).Take(60).ToList();
            //string str = kd.Select(r => r.date + "\t" + r.close + "\n").Aggregate((r1, r2) => r1 + r2)
            //             +"\r\nma60\t"+ ma60.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2) 
            //             +"\r\nma20\t"+  ma20.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
            //             +"\r\nma10\t"+  ma10.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
            //             +"\r\nma5\t"+  ma5.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2);
            //MFile.WriteAllText( stock.Name+ stock.NumCode + ".txt", str);
            //MFile.WriteAllText(stock.Name + stock.NumCode + ".txt", dma60.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2));
        public void TestStock(Stock s,int staticdaylenght = 200)
        {   
            tagstock t = tags[s.ID];
            if (t == null)
                return;
            // if select  then selectstockindex.add 

            KData[] kd = t.kd.ToArray();
          
            int skipday = kd.Length - staticdaylenght;
            List<double> ma60 = MA(0, 60, kd);
            List<double> ma20 = MA(40, 20, kd);
            List<double> ma10 = MA(50, 10, kd);
            List<double> ma5 = MA(55, 5, kd);

          
            double pclose = ma60[0];
            List<double> dma60 = ma60.Select(r =>
            { double ret = r - pclose; pclose = r; return ret; }).ToList();
            double hafavedev = StaticsTools.avedev(dma60) * .5;
            List<int> intdma60 = dma60.Select(r => (int)(r / hafavedev)).ToList();

            pclose = ma60[0];
            List<double> logma60 = ma60.Select(r =>
            { double ret = Math.Log10(r / pclose); pclose = r; return ret; }).ToList();
            //MFile.WriteAllText(stock.Name + stock.NumCode + "log.txt", logma60.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2));
            //MFile.WriteAllText(stock.Name + stock.NumCode + "ldv.txt", ldv.Select(r => r.index + "\t" + r.length + "\t" + r.value + "\r\n").Aggregate((r1, r2) => r1 + r2));
            //MFile.WriteAllText(stock.Name + stock.NumCode + "lddv.txt", lddv.Select(r => r.index + "\t" + r.length + "\t" + r.value + "\r\n").Aggregate((r1, r2) => r1 + r2));             
            //序列分段
            //lddv = DivInforDouble.reduceAnalyse(lddv);
            //MFile.WriteAllText(stock.Name + stock.NumCode + "rlddv.txt", lddv.Select(r => r.index + "\t" + r.length + "\t" + r.value + "\r\n").Aggregate((r1, r2) => r1 + r2));

            //求序列最大和子序列
            int b = 0, e = 0;
            double maxsum = maxsublinear(logma60, out b, out e);
            double maxzf = (Math.Pow(10, maxsum) - 1) * 100;
            MFile.AppendAllText(Tools.TimeStringTools.NowDateMin() + "MonitMAIndicator最大涨幅和区间log.txt", s.Name + "\t" + s.Code + "\t" + maxzf + "\t " + (e - b) + "\t" + b + "\t " + e + "\t" + kd[60 + b].date + "\t" + kd[60 + e].date + "\t");

            List<double> flogma60 = logma60.Select(r => -r).ToList();
            double fmaxsum = maxsublinear(flogma60, out b, out e);
            double fmaxzf = (1 - Math.Pow(10, -fmaxsum)) * 100;
            //离今天的交易天数

        }
        double maxsublinear(List<double> a, out int b, out int e)
        {
            int i;
            int begin = 0, end = 0, lbegin = 0;
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
                        lbegin = lbegin + 0;
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
            List<double> MA = new List<double>();
            int sum = kd.Skip(skip).Take(daylength).Sum(r => r.close);
            for (int i = skip + daylength; i < kd.Length; i++)
            {
                double avg = sum * 1.0 / daylength;
                MA.Add(avg);
                sum += kd[i].close - kd[i - daylength].close;
            }

            return MA;
        }
        private List<int> selectstockindex;
    }
}
#region runnet
/*
ublic void RunNet()
        {
            bClosed = false;
            isruning = true;           
            ThreadShowMsg("Begining....");
            while (true)
            {        
                    ThreadShowMsg("刷新时间："+System.DateTime.Now.ToString() + "\r\n");
                    ProcessItem( ); 
                try
                {
                    if (!exchangestatus.InLine || !exchangestatus.ExChanging || !exchangestatus.ExChangDay)
                    {
                        ThreadShowMsg("脱机状态或者非交易时段，退出刷新监视");
                        break;
                    }
                    Thread.Sleep(10000);
                }
                catch (Exception e)
                {
                    MFile.AppendAllText("Exception1.log", e.Message + " " +  ""+ " \r\n");
                    ThreadShowMsg("出现故障，退出刷新监视"+e.Message + " " + "" + " \r\n");
                    break;
                }
                if (bClosed) break;
            }
            isruning = false;
        }
        private void ProcessItem()
        {
            bool HasData = exchangestatus.InLine && exchangestatus.ExChangDay;
            if (exchangestatus.InLine && exchangestatus.ExChangDay )
            ds.InitPriceFromNet(_stocks.stocks);
            bool Change = false;
            foreach (Stock s in _stocks.stocks)
            {
               ////////////////////
               //dosomething
            }
            if (Change)
                ThreadShowSelectStocks();
        } 
//*/
#endregion