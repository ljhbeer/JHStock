using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;
//using JHStock.UpdateStocks;

namespace ToolsCXml
{
    public class CaseCmd
    {
        public CaseCmd(string str)
        {
            conditions = new List<Condition>();
            string[] items = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {
                if (s.Contains("(") && s.Contains(")=>"))
                    conditions.Add(new Condition(s));
            }
        }
        public List<Condition> conditions;
    }
}
