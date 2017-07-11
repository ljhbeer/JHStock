using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Tools;

namespace ToolsCXml
{
    public class Cmd
    {
        public Cmd(string str)
        {
            ListTable = new List<TableCmd>();
            ReverseMatch = false;
            TxtAsUrl = false;
            string[] items = str.Split(new string[] { ";\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            cmdkeyvalue = new Dictionary<string, string>();
            foreach (string s in items)
            {
                if (s.Contains("="))
                {
                    string name = s.Substring(0, s.IndexOf('=')).Trim();
                    if (ValidTools.ValidName(name) && !cmdkeyvalue.ContainsKey(name))
                        cmdkeyvalue[name] = s.Substring(s.IndexOf('=') + 1);
                }
            }
            if (cmdkeyvalue.ContainsKey("nexturl"))
                NextUrlTags = new BETags(cmdkeyvalue["nexturl"]);
            if (cmdkeyvalue.ContainsKey("nextexist"))
                NextExist = cmdkeyvalue["nextexist"];
            if (cmdkeyvalue.ContainsKey("replacetemplate"))
            {
                ReplaceTemplate = cmdkeyvalue["replacetemplate"];
                ReplaceTemplate = ReplaceTemplate.Replace("\\r\\n", "\r\n")
                    .Replace("\\t", "\t");
            }
            if (cmdkeyvalue.ContainsKey("replacetonull"))
            {
                ReplacetoNullTags = new BETags(cmdkeyvalue["replacetonull"]);
            }
            if (cmdkeyvalue.ContainsKey("multisubitem"))
                MultiSubItemTags = new BETags(cmdkeyvalue["multisubitem"]);
            if (cmdkeyvalue.ContainsKey("reversematch"))
            {
                if (cmdkeyvalue["reversematch"] == "true")
                    ReverseMatch = true;
            }
            if (cmdkeyvalue.ContainsKey("txtasurl"))
            {
                if (cmdkeyvalue["txtasurl"] == "true")
                    TxtAsUrl = true;
            }
            if (cmdkeyvalue.ContainsKey("dbidbeginend"))
            {
                BETags bts = new BETags(cmdkeyvalue["dbidbeginend"]);
                Bedbid = null;
                if (bts.tags.Count == 1 && ValidTools.ValidNumber(bts.tags[0].Begin)
                                        && ValidTools.ValidNumber(bts.tags[0].End))
                {
                    Bedbid = new BEId();
                    Bedbid.B = Convert.ToInt32(bts.tags[0].Begin);
                    Bedbid.E = int.MaxValue;
                    if (ValidTools.ValidNumber(bts.tags[0].End))
                        Bedbid.E = Convert.ToInt32(bts.tags[0].End);
                    Bedbid.MoveToTop();
                }//else 未设置BeDbid
            }
            if (cmdkeyvalue.ContainsKey("savepath"))
                SavePath = cmdkeyvalue["savepath"];
            if (cmdkeyvalue.ContainsKey("casecmd"))
            {
                BEPos bp = BETag.FormatCmd(cmdkeyvalue["casecmd"], '{', '}');
                if (bp.Valid())
                {
                    string cmdstr = bp.String;
                    caseCmd = new CaseCmd(cmdstr);
                }
            }
            if (cmdkeyvalue.ContainsKey("table"))
            {
                BEPos bp = BETag.FormatCmd(cmdkeyvalue["table"], '{', '}');
                if (bp.Valid())
                {
                    string tablecmdstr = bp.String;
                    tableCmd = new TableCmd(tablecmdstr);
                    ListTable.Add(tableCmd); // 以后可以添加多个Table
                }
            }
            if (cmdkeyvalue.ContainsKey("tablevalue"))
            {
                BEPos bp = BETag.FormatCmd(cmdkeyvalue["tablevalue"], '{', '}');
                if (bp.Valid())
                {
                    string tablecmdstr = bp.String;
                    tablevalueCmd = new TableValueCmd(tablecmdstr);
    
                }
            }
            /////////////////////
            if (cmdkeyvalue.ContainsKey("tablerow"))
            {
                BEPos bp = BETag.FormatCmd(cmdkeyvalue["tablerow"], '{', '}');
                if (bp.Valid())
                {
                    string tablecmdstr = bp.String;
                    tableRowCmd = new TableRowCmd(tablecmdstr);
                    ListTable.Add(tableCmd); // 以后可以添加多个Table
                }
            }
            if (cmdkeyvalue.ContainsKey("tablerowvalue"))
            {
                BEPos bp = BETag.FormatCmd(cmdkeyvalue["tablerowvalue"], '{', '}');
                if (bp.Valid())
                {
                    string tablecmdstr = bp.String;
                    tablerowvalueCmd = new TableRowValueCmd(tablecmdstr);
    
                }
            }
        }
        private Dictionary<string, string> cmdkeyvalue;
        public BETags MultiSubItemTags { get; set; }
        public BETags NextUrlTags { get; set; }
        public BETags ReplacetoNullTags { get; set; }
        public bool TxtAsUrl { get; set; }
        public bool ReverseMatch { get; set; }
        public BEId Bedbid { get; set; }
        public string ReplaceTemplate { get; set; }
        public string NextExist { get; set; }
        public string SavePath { get; set; }
        public CaseCmd caseCmd{ get; set; }
        public TableCmd tableCmd { get; set; }
        public TableValueCmd tablevalueCmd { get; set; }
        public List<TableCmd> ListTable { get; set; }
        public TableRowCmd tableRowCmd { get; set; }
        public TableRowValueCmd tablerowvalueCmd { get; set; }
    }
}
