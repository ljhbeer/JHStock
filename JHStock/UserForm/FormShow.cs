using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tools;
using System.Text.RegularExpressions;
using JHStock.Update;
using Newtonsoft.Json;
using System.IO;

namespace JHStock.UserForm
{
    public partial class FormShow : Form
    {
        private Stock _s;
        private JSConfig _jscfg;
        private List<string> columntitles;
        private string _qqfinpath;
        public FormShow(Stock s, JSConfig _jscfg, List<string> columntitles)
        {
            InitializeComponent();
            this._s = s;
            this._jscfg = _jscfg;
            this.columntitles = columntitles;
            _qqfinpath = _jscfg.baseconfig.WorkPath +"Data\\QQFin\\";
            Init();
        }
        private void Init()
        {
            string urlt = "http://quote.eastmoney.com/[scode].html";
            string url = urlt.Replace("[scode]", _s.Code);
            string html = CWeb.GetWebClient(url);
            string pattern = @"(?<=公司核心数据[^01]*)<div class=\""box-x1 mb10\"">[\S\s]*(?=<div class=\""title1 nonal ie rtab\"">)";
            //html = html.
            html = Regex.Match(html, pattern).Value;


            QQfinItem qf = new QQfinItem();
            qf.LoadData(_qqfinpath + _s.Code + ".txt");
            
            string txt = File.ReadAllText(@"E:\Project\Source\Stock\Data\QQFinConfig.txt");
            Component.QQFinConfig.QQfinConfig fc = new Component.QQFinConfig.QQfinConfig();


            NFinItem nfi = _jscfg .globalconfig.NFI.NFI[_s.ID];
            if (nfi != null)
            {
                foreach (string ss in columntitles)
                {
                    if (ss == "代码" || ss == "名称") continue;
                    string sDate = ss.Substring(ss.IndexOf("_") + 1, ss.LastIndexOf("_") - ss.IndexOf("_") - 1);
                    int year = Convert.ToInt32(sDate);
                    string it = ss.Substring(ss.LastIndexOf("_") + 1);
                    if (ss.StartsWith("D_"))
                    {
                        foreach (NDJCW n in nfi.NDJCW)
                        {
                            if (n.bgrq.Year * 100 + n.bgrq.Month == year)
                            {
                                //if (it == "mgsy")
                                //    dr[ss] = n.mgsy;
                                //else if (it == "jzcsyl")
                                //    dr[ss] = n.jzcsyl;
                                //else if (it == "zzcsyl")
                                //    dr[ss] = n.zzcsyl;
                                //break;
                            }
                        }
                    }
                    else if (ss.StartsWith("C_"))
                    {
                        foreach (NCZNL n in nfi.NCZNL)
                        {
                            if (n.bgrq.Year == year)
                            {
                                //if (it == "mgsy")
                                //    dr[ss] = n.mgsy;
                                //else if (it == "jlr")
                                //    dr[ss] = n.jlr;
                                //else if (it == "zysr")
                                //    dr[ss] = n.zysr;
                                //else if (it == "yylr")
                                //    dr[ss] = n.yylr;
                                //else if (it == "zzc")
                                //    dr[ss] = n.zzc;
                                //break;
                            }
                        }
                    }
                }
            }                     
         

            webBrowser1.DocumentText = "<H>" + _s.Name + _s.Code + "</H>" + html;
        }
        private DataTable JsonToDataTable(string strJson)
        {
            //转换json格式
            strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
            //取出表名  
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名  
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));
            //获取数据  
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('*');
                //创建表  
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');
                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                //增加内容  
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
        }
    }
}
