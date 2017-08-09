using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace JHStock
{
    public partial class FormCustomLog : Form
    {
        public FormCustomLog(JSConfig _jscfg, StocksCustomLog _stockslog)
        {
            InitializeComponent();
			this._jscfg = _jscfg;
			this._stockscustomlog = _stockslog;
			Init();
        }
		private void Init(){
			 _tn = new TreeNode();
             _tn.Text = "入手日期";
             TreeNode[] vt = new TreeNode[2];
             for (int i = 0; i < vt.Count(); i++)
                 vt[i] = new TreeNode();
             vt[0].Name = vt[0].Text = "2016";
             vt[1].Name = vt[1].Text = "2017";
             vt[0].Tag = 2016;
             vt[1].Tag = 2017;
             _tn.Nodes.AddRange(vt);
             InitMonth();
             treeView1.Nodes.Add(_tn);
             treeView1.ExpandAll();
		}
		private void InitMonth()
        {
            List<int> years =  _stockscustomlog._dailystocks.Select(r => r.Date.Year).Distinct().ToList();
            List<List<int>> yearmonths = new List<List<int>>();
            foreach (int year in years)
                yearmonths.Add(
                       _stockscustomlog._dailystocks.Where(r => r.Date.Year == year)
                           .Select(r => r.Date.Month).Distinct().ToList()
                );

            if (years.Count == 0 || yearmonths.Count == 0) return;
            foreach (TreeNode n in _tn.Nodes)
            {
                int year = (int)n.Tag;
                int pos = years.IndexOf(year);
                if (pos == -1) continue;
                List<TreeNode> ltn = new List<TreeNode>();
                foreach (int m in yearmonths[pos])
                {
                    TreeNode t = new TreeNode((year * 100 + m).ToString());
                    t.Tag = (year * 100 + m);
                    ltn.Add(t);
                }
                n.Nodes.AddRange(ltn.ToArray());
            }
        }
       
        private void buttonSaveLog_Click(object sender, EventArgs e)
        {
            string title = textBoxtitle.Text.Trim();
            string content = textBox1.Text.Trim();
            if (title == "" || content == "" || title.Length <10 || content.Length<50)
            {
                MessageBox.Show("请完成标题 和 内容,标题不得少于10个字， 内容不得少于50个字");
                return;
            }
            string msg = 
                _stockscustomlog.SaveLog(title, content); //
            if (msg != "")
                MessageBox.Show(msg);
            else
                _stockscustomlog.Save();
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            string title = textBoxtitle.Text.Trim();
            string content = textBox1.Text.Trim();
            if (title == "" || content == "" || title.Length < 10 || content.Length < 50)
            {
                MessageBox.Show("请完成标题 和 内容,标题不得少于10个字， 内容不得少于50个字");
                return;
            }
            string msg =
                _stockscustomlog.SaveLog(title, content,true); //
            if (msg != "")
                MessageBox.Show(msg);
            else
                _stockscustomlog.Save();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode t = treeView1.SelectedNode;
            if (t != null)
                if (t.Text.Length == 6)
                {
                    if (t.Nodes.Count == 0)
                    {
                        int yearmonth = (int)t.Tag;
                        int year = yearmonth / 100;
                        int month = yearmonth % 100;
                        List<DateTime> days = _stockscustomlog._dailystocks.Where(r => (r.Date.Year == year && r.Date.Month == month))
                            .Select(r => r.Date).Distinct().ToList();

                        List<TreeNode> ltn = new List<TreeNode>();
                        foreach (DateTime dr in days)
                        {
                            int date = dr.Year * 10000 + dr.Month * 100 + dr.Day;
                            TreeNode tt = new TreeNode(date.ToString());
                            tt.Tag = dr;
                            ltn.Add(tt);
                        }
                        t.Nodes.AddRange(ltn.ToArray());

                    }
                }
                else if (t.Text.Length == 8)
                {
                    DateTime day = (DateTime)t.Tag;
                    List<DailyLogStocks> ds = _stockscustomlog._dailystocks.Where(r => r.Date.ToShortDateString() == day.ToShortDateString()).ToList();
                    if (ds.Count == 1 )
                    {
                        List<TreeNode> ltn = new List<TreeNode>();
                        foreach (DailyLogStock d in ds[0]._stocks)
                        {
                            TreeNode tt = new TreeNode(d.Title);
                            tt.Tag = day;
                            ltn.Add(tt);
                        }
                        t.Nodes.AddRange(ltn.ToArray());
                    }
                }
                else if (t.Text.Length > 8)
                {
                    DateTime day = (DateTime)t.Tag;
                    List<DailyLogStocks> ds = _stockscustomlog._dailystocks.Where(r => r.Date.ToShortDateString() == day.ToShortDateString()).ToList();
                    if (ds.Count == 1)
                    {
                        DailyLogStock d = ds[0]._stocks.Find( r=> r.Title == t.Text);
                        //TODO: 测试是否保存当前为完成文档
                        textBoxtitle.Text = d.Title;
                        textBox1.Text = d.Content;
                        
                    }
                }

        }
        private void FormCustomLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text == "浏览")
            {
                string txt = textBox1.Text;
                MatchCollection  mc = Regex.Matches(txt, "(?<=\\[img src=\")[a-z0-9_-]*(?=\"\\])");
                List<string> imgnames = new List<string>();
                int cnt = 0;
                foreach (Match m in mc)
                {
                    imgnames.Add(m.Value);
                    txt = txt.Replace( "[img src=\""+m.Value+"\"]","[img"+cnt+"]");
                    cnt++;
                }
                txt = System.Web.HttpUtility.HtmlEncode(txt);
                txt = txt.Replace("\r\n", "<br>\r\n").Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                //string imgtemp = "<img height=600 width=800 src=\"[src]\" alt=\"[alt]\">";
                string imgtemp = "<img  src=\"[src]\" >";
                imgtemp = imgtemp.Replace("[alt]", "");

                string imgpath = "file:///" + _jscfg.baseconfig.WorkPath + "Data\\imgs\\";
                for (int i = 0; i < cnt; i++)
                {
                    txt = txt.Replace("[img" + i + "]", imgtemp.Replace("[src]",imgpath.Replace("\\","/") + imgnames[i] + ".jpg"));
                }
                CreateImgs(imgnames);
                string htmltemplate = "<html>\r\n<head>[head]</head>\r\n<body>\r\n[body]</body>\r\n</html>";
                string meta = @"<meta http-equiv=""content-type"" content=""text/html;charset=utf-8"">";
                htmltemplate = htmltemplate.Replace("[head]", meta);
                string html = htmltemplate.Replace("[body]", txt);
                File.WriteAllText("tempshow.html", html);
                webBrowser1.DocumentText = html;
            }
        }
        private void CreateImgs(List<string> imgnames)
        {
            string path = _jscfg.baseconfig.WorkPath + "Data\\imgs\\";
            foreach (string imgname in imgnames)
            {
                string filename = path+ imgname+".jpg";
                if(File.Exists(filename)) continue;
                string para = imgname.Replace("_", ",");
                string numcode = imgname.Substring(2, 6);
                Stock s = _jscfg.globalconfig.Stocks.StockByNumCode(numcode);
                string url = urlt.Replace("[para]", para);
                StockDraw.DrawDaily(url,filename,s);
            }
        }

        private TreeNode _tn;	
		private JSConfig _jscfg;
		private StocksCustomLog _stockscustomlog;
        private static string urlt = @"http://web.ifzq.gtimg.cn/appstock/app/fqkline/get?_var=kline_day&param=[para]";
    }
}
