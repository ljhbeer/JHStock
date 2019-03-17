using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using JHStock.Update;
using Newtonsoft.Json;
using System.Threading;
using Tools;
using System.Text.RegularExpressions;

namespace JHStock.UserForm
{
    class DealClass
    {
        private JSConfig _jscfg;
        private Stocks _stocks;
        private Form1 form1;
        private bool _isrunning;
        private Button _completebtn;

        public DealClass(JSConfig _jscfg, Stocks _stocks, Form1 form1)
        {
            this._jscfg = _jscfg;
            this._stocks = _stocks;
            this.form1 = form1;
        }
        public void TestDownLoads(System.Windows.Forms.Button btn)
        {
            //DownLoadsAllKdata(btn);
            //ConstructCQ();
            LoadFHSG();

        }

        private void LoadFHSG()
        {
            string template = @"insert into [Kbouns](
stockid,  PDate,   SG,   ZZ,  PX, progress,CQDate, DJDate,SSDate) 
Values( [stockid],  [PDate],   [SG],   [ZZ],  [PX], '实施',[CQDate], '[DJDate]','[SSDate]');";
            List<Fhsgs> ls = JsonConvert.DeserializeObject<List<Fhsgs>>(File.ReadAllText(_jscfg.baseconfig.WorkPath + "Data\\AllFin.Dat"));
            List<string> lss1 = ls.Select(r1 =>
            {
                StringBuilder sb = new StringBuilder();
                //10派0.125元送0.5股转9.5股
                Regex re = new Regex("10派([0-9.]*)元(送([0-9.]*)股)?(转([0-9.]*)股)?");
                Stock s = _stocks.StockByNumCode(r1.stockcode.Substring(2));
                string t1 = template.Replace("[stockid]", s.ID.ToString());
                List<string> lss2 = r1.FHSG.Select(r2 =>
                {
                    string t2 = t1.Replace("[PDate]", r2.nd)
                        .Replace("[CQDate]", r2.cqr.Replace("-",""))
                        .Replace("[DJDate]", r2.djr)
                        .Replace("[SSDate]", "-");
                    string PX = "0";
                    string SG = "0";
                    string ZZ = "0";
                    Match m = re.Match(r2.FHcontent);
                    if (m.Success)
                    {
                        PX = m.Groups[1].Value;
                        if (m.Groups[3].Success)
                           SG = m.Groups[3].Value;
                        if (m.Groups[5].Success)
                            ZZ = m.Groups[5].Value;
                    }
                    t2 = t2.Replace("[PX]", PX).Replace("[SG]", SG).Replace("[ZZ]", ZZ);
                    return t2;
                }).ToList();
                
                sb.Append(  string.Join("\r\n",lss2));
                return sb.ToString();
            }).ToList();

            MFile.WriteAllText(_jscfg.baseconfig.WorkPath + "Data\\Test.Dat",string.Join("\r\n===========\r\n",lss1));
        }

        private void ConstructCQ()
        {
            string undeal = "";
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (Stock s in _stocks.stocks)
            {

                string filetemplate = "{\"stockcode\":\"[stockcode]\"\r\n,FHSG:[[value]]}";
                string path = _jscfg.baseconfig.WorkPath + "\\Data\\AllKdata\\" + s.Code + ".txt";

                if (File.Exists(path))
                {
                    string txt = File.ReadAllText(path);
                    txt = CutJsonStringHead(txt);
                    string ss = ConstructKdata(s.Code, txt);

                    filetemplate = filetemplate.Replace("[stockcode]", s.Code)
                        .Replace("[value]", ss);
                    sb.Append(filetemplate);
                    sb.AppendLine(",");
                }
                else
                {
                    undeal += s.Code + "\t";
                }
                if (count++ % 50 == 0)
                    form1.showfiletxt("已完成" + count + "/" + _stocks.stocks.Count + "中");
            }
            string path2 = _jscfg.baseconfig.WorkPath + "\\Data\\AllFin.dat";
            MFile.WriteAllText(path2, "[" + sb.ToString() + "]");
            if (undeal != "")
                MessageBox.Show(undeal);
            form1.showfiletxt("已保存文件到" + path2 + "中");
        }
        private string ConstructKdata(string stockcode, string txt)
        {
            QQStocks qs = JsonConvert.DeserializeObject<QQStocks>(txt);
            List<string> kd = qs.data[stockcode.ToLower()].day
                .Where(r1 => r1.Count == 7)
               .Select(r2 => r2[6].ToString()).ToList();
            return string.Join(",\r\n", kd);
        }
        private string CutJsonStringHead(string txt)
        {
            if (txt.IndexOf("=") != -1)
                txt = txt.Substring(txt.IndexOf("=") + 1);
            txt = txt.Replace("dayly", "day");
            txt = txt.Replace("qfqday", "day");
            return txt;
        }
        private void DownLoadsAllKdata(Button btn)
        {
            string Type = "dayly";
            Stocks _stocks = _jscfg.globalconfig.Stocks;
            if (_stocks == null || _stocks.stocks.Count == 0)
                return;
            if (_stocks.Gcfg.db == null) return;
            if (!_isrunning)
            {
                //_bshowtime = false;// checkBoxShowTimeOut.Checked;
                _isrunning = true;
                _completebtn = btn;
                _completebtn.Enabled = false;
                UpdateFin updatefin = new UpdateFin(_stocks);
                updatefin.SetDateType(Type); ;
                updatefin.MaxThreadSum = 20;
                updatefin.showmsg = new ShowDeleGate(form1.ThreadShowMsg);
                updatefin.ThreadCompleteRun = new CompleteDeleGate(form1.ThreadCompleteRun);
                ////qf.DealStocks.Add(_stocks.StockByIndex(2));
                ////qf.DealStocks.Add(_stocks.StockByIndex(3))
                updatefin.DealStocks = _stocks.stocks;
                form1._updatetime = DateTime.Now;
                System.Threading.Thread nonParameterThread = new Thread(updatefin.DownLoadAllKData);
                nonParameterThread.Start();
            }
        }

    }
    public class FHSGItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string nd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fh_sh { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string djr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cqr { get; set; }
        /// <summary>
        /// 10派1元
        /// </summary>
        public string FHcontent { get; set; }
    }
    public class Fhsgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string stockcode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FHSGItem> FHSG { get; set; }
    }
}
