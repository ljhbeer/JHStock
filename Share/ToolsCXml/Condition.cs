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
    public class Condition
    {
        public Condition(string strcdt)
        {
            strcdt = strcdt.Trim();
            int te = strcdt.IndexOf("(");
            int ve = strcdt.IndexOf(")=>");
            ctype = strcdt.Substring(0, te);
            cvalue = strcdt.Substring(te + 1, ve - te - 1);
            string actstr = strcdt.Substring(ve + 3);
            if (ctype == "contains")
                b = bc_contains;
            else if (ctype == "startswith")
            {
                b = bc_starswith;
            }
            else if (ctype == "endswith")
            {
                b = bc_Endswith;
            }
            else if (ctype == "equals")
            {
                b = bc_Equals;
            }
            else if (ctype == "default")
            {
                b = bc_default;
            }
            else
            {
                b = bc_false;
            }
    
            if (actstr.Contains("N"))
            {
                actvalue = actstr;
                a = act_getNcompute;
            }
            else
            {
                if (actstr == "null" || ValidTools.ValidDoubleNumber(actstr))
                {
                    actvalue = actstr;
                    a = act_getactvalue;
                }
                else
                {
                    a = act_getSame;
                }
            }
        }
        public bool bc_contains(string s)
        {
            return s.Contains(cvalue);
        }
        public bool bc_starswith(string s)
        {
            return s.StartsWith(cvalue);
        }
        public bool bc_Endswith(string s)
        {
            return s.EndsWith(cvalue);
        }
        public bool bc_Equals(string s)
        {
            return s.Equals(cvalue);
        }
        public bool bc_default(string s)
        {
            return true;
        }
        public bool bc_false(string s)
        {
            return false;
        }
    
        public string act_getnumber(string s)
        {//不能识别复杂数字
            StringBuilder number = new StringBuilder();
            bool bn = false;
            if (".-0123456789".Contains(s[0]))
            {
                bn = true;
            }
            foreach (char c in s)
            {
                if (".-0123456789".Contains(c))
                {
                    if (!bn) number.Clear();
                    else
                        number.Append(c);
                    bn = true;
                }
                else
                {
                    bn = false;
                }
            }
            return number.ToString();
        }
        public string act_getactvalue(string s)
        {
            return actvalue;
        }
        public string act_getNcompute(string s)
        {
            string N = act_getnumber(s);
            return actvalue.Replace("N", N);
        }
        public string act_getSame(string s)
        {
            return s;
        }
        public string ctype;
        public string cvalue;
        public string actvalue;
        public ActCondition a;
        public BoolCondition b;
    }
}
