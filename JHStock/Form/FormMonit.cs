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
        public List<Stock> DebugStocks;
        private List<Stock> selectstock;

        public FormMonit(Stocks _stocks, ExChangeStatusCheck exchangestatus, string type, JHStock.Update.tagstock[] tags)
        {
            this._stocks = _stocks;
            DebugStocks = this._stocks.stocks;
            this.exchangestatus = exchangestatus;
            this.type = type;
            this.tags = tags;
            selectstock = new List<Stock>();
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
            selectstock.Clear();
            foreach (Stock s in DebugStocks)
            {
                TestStock(s);                
            }
            ///show selectstock in the Table
            ShowSelectedStocks();
        }

        private void ShowSelectedStocks()
        {
            dt.Rows.Clear();
            foreach (Stock s in selectstock)
            {
                DataRow dr = dt.NewRow();
                dr["名称"] = s.Name;
                dr["代码"] = s.Code;
                dt.Rows.Add(dr);
            }
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
            List<string> columntitles = new List<string>() { "名称", "代码", "日期" };//,"杂项"
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
        public void TestStock(Stock s,int staticdaylenght = 200)
        {   
            tagstock t = tags[s.ID];
            if (t == null)
                return;
            // if select  then selectstockindex.add 
            selectstock.Add(s);
            int[] kdclose = t.kd.Select(r => r.close).ToArray();
            int[] kdvol = t.kd.Select(r => r.vol).ToArray();
            List<double> ma60 = MA(0, 60, kdclose); //skip 60
            List<double> ma20 = MA(40, 20, kdclose);
            List<double> ma10 = MA(50, 10, kdclose);
            List<double> ma5 = MA(55, 5, kdclose);
            
            List<double> vma10 = MA(50, 10, kdvol);
            List<double> vma5 = MA(55, 5, kdvol);

            double now = vma5[0];
            List<double> dvma5rate  = vma5.Select( r =>
                { double ret = (r - now)/now; now = r; return ret; }).ToList();

            List<double> vdvma5rate = MA(0, 5, dvma5rate.ToArray());
            List<int> intvdvma5rate = vdvma5rate.Select(r => (int)(r * 100 * 5)).ToList();
            string str = "date\tclose\tvol\n"
                         +t.kd.Select(r => r.date + "\t" + r.close +"\t"+r.vol+ "\n").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nma60\t" + ma60.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nma20\t" + ma20.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nma10\t" + ma10.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nma5\t" + ma5.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nvma10\t" + vma10.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nvma5\t" + vma5.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)

                         + "\r\ndvma5rate\t" + dvma5rate.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nvdvma5rate\t" + vdvma5rate.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2)
                         + "\r\nintvdvma5rate\t" + intvdvma5rate.Select(r => r + "\t").Aggregate((r1, r2) => r1 + r2);
            MFile.WriteAllText(s.Name + s.NumCode + ".txt", str);

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
        
        //type = 0: close  type = 1: vol type =  2: open type=3 high  type=4 low
        private List<double> MA(int skip, int daylength, int[] listdata)
        {
            List<double> MA = new List<double>();
            double sum = listdata.Skip(skip).Take(daylength).Sum();
            for (int i = skip + daylength; i < listdata.Length; i++)
            {
                double avg = sum * 1.0 / daylength;
                MA.Add(avg);
                sum += listdata[i] - listdata[i - daylength];
            }
            return MA;
        }
        private List<double> MA(int skip, int daylength, double[] listdata)
        {
            List<double> MA = new List<double>();
            double sum = listdata.Skip(skip).Take(daylength).Sum();
            for (int i = skip + daylength; i < listdata.Length; i++)
            {
                double avg = sum * 1.0 / daylength;
                MA.Add(avg);
                sum += listdata[i] - listdata[i - daylength];
            }
            return MA;
        }
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