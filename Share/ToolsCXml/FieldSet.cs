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
    public class FieldSet
    {
        public FieldSet(string Name, string ValueType, string Rule)
        {
            this.Name = Name;
            this.ValueType = ValueType;
            this.BETags = new BETags(Rule);
        }
        public string Name { get; set; }
        public string ValueType { get; set; }
        public BETags BETags { get; set; }
        public string GetValue(string txt, CItem item)
        {
            return GetValue(txt, item.Url);
        }
        public string GetValue(string txt, string Url)
        {
            string retstr = BETags.Match(txt);
            if (BETags.Cmd != null)
            {    
                if (BETags.Cmd.TxtAsUrl)
                {
                    StaticsTools.GetCorrectUrl(Url, ref  txt);
                    if (ValidTools.ValidUrl(txt))
                    {
                        CWeb web = CItem.web;
                        txt = web.GetOKUrl(txt);
                        retstr = BETags.Match(txt);
                    }
                }
                if (BETags.Cmd.tablevalueCmd != null)
                {
                    if (BETags.Cmd.tablevalueCmd.Valid)
                        retstr = BETags.Cmd.tablevalueCmd.GetValue();
                }
                if (BETags.Cmd.ReplaceTemplate != null)
                {
                    retstr = BETags.Cmd.ReplaceTemplate.Replace("[" + Name + "]", retstr);
                }
                if (BETags.Cmd.NextUrlTags != null)
                {
                    CWeb web = CItem.web;
                    BETags Nu = BETags.Cmd.NextUrlTags;
                    string str = "";
                    StringBuilder sb = new StringBuilder("");
                    while (true)
                    {
                        if (!txt.Contains(BETags.Cmd.NextExist))
                            break;
                        string url = Nu.Match(txt);
                        StaticsTools.GetCorrectUrl(Url, ref  url);
                        txt = web.GetOKUrl(url);
                        str = BETags.Match(txt);
                        if (BETags.Cmd.ReplaceTemplate != null)
                        {
                            str = BETags.Cmd.ReplaceTemplate.Replace("[" + Name + "]", str);
                        }
                        sb.Append(str);
                    }
                    retstr += sb;
                }
                if (BETags.Cmd.ReplacetoNullTags != null)
                {
                    foreach (BETag t in BETags.Cmd.ReplacetoNullTags.tags)
                        retstr = retstr.Replace(t.Begin, t.End);
                }
                if (BETags.Cmd.caseCmd != null)
                {
                    foreach (Condition c in BETags.Cmd.caseCmd.conditions)
                    {
                        if (c.b(retstr))
                        {
                            retstr = c.a(retstr);
                            break;
                        }
                    }
                }
            }
            return retstr;
        }
        public string GetItemRowValue(string txt, List<BEPos> itembps)
        {
            string retstr = BETags.Match(txt);
            if (BETags.Cmd != null)
            {                   
                if (BETags.Cmd.tablerowvalueCmd != null)
                {
                    if (BETags.Cmd.tablerowvalueCmd.ColIndex < itembps.Count)
                        retstr = itembps[BETags.Cmd.tablerowvalueCmd.ColIndex].String;
                    else
                        retstr = "";
                }
                if (BETags.Cmd.ReplaceTemplate != null)
                {
                    retstr = BETags.Cmd.ReplaceTemplate.Replace("[" + Name + "]", retstr);
                }                   
                if (BETags.Cmd.ReplacetoNullTags != null)
                {
                    foreach (BETag t in BETags.Cmd.ReplacetoNullTags.tags)
                        retstr = retstr.Replace(t.Begin, t.End);
                }
                if (BETags.Cmd.caseCmd != null)
                {
                    foreach (Condition c in BETags.Cmd.caseCmd.conditions)
                    {
                        if (c.b(retstr))
                        {
                            retstr = c.a(retstr);
                            break;
                        }
                    }
                }
            }
            return retstr;
        }        
    }
}
