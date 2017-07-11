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
    public class TableRowCmd
    {
        public TableRowCmd(string str)
        {
            string[] items = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {
                if (s.Contains("name="))
                    Name = s.Substring(s.IndexOf("name=") + 5).Trim();
                else if (s.Contains("cols"))
                    Cols = new BETags(s.Substring(s.IndexOf("cols=") + 5).Trim());
                else if (s.Contains("replace"))
                    Replace = new BETags(s.Substring(s.IndexOf("replace=") + 8).Trim());
            }
        }
        public string Table(int RowIndex, int ColIndex)
        {
            if (txt != null)
            {
                //if (str == null || !string.ReferenceEquals(str, txt))
                //{
                //    str = txt;
                //    string s = Location.Match(txt);
                //    rows = s.Split(RowGag, StringSplitOptions.RemoveEmptyEntries);
                //    cols = new string[rows.Length][];
    
                //}
                //if (RowIndex < rows.Length)
                //{
                //    if (cols[RowIndex] == null)
                //        cols[RowIndex] = rows[RowIndex]
                //            .Split(ColGap, StringSplitOptions.RemoveEmptyEntries);
                //    if (ColIndex < cols[RowIndex].Length)
                //        return cols[RowIndex][ColIndex];
                //}
            }
            return "";
        }
        public string Name { get; set; }
        public BETags Replace { get; set; }
        public BETags Cols { get; set; }           
        public static string txt;
    }
}
