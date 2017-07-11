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
    public class TableRowValueCmd
    {
        public TableRowValueCmd(string str)
        {
            Valid = false;
            ColIndex = -1;
            string[] items = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {
                if (s.Contains("name="))
                    Name = s.Substring(s.IndexOf("name=") + 5).Trim();
                else if (s.Contains("colindex="))
                    ColIndex = Convert.ToInt32(s.IndexOf("colindex=") + s.Substring(9));
            }
        }
        public int ColIndex { get; set; }
        public bool Valid { get; set; }
        public string Name { get; set; }
    }
}
