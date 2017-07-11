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
    public class TableCmd
    {
        public TableCmd(string str)
        {
            this.str = null;
            cols = null;
            string[] items = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {
                if (s.Contains("name="))
                    Name = s.Substring(s.IndexOf("name=") + 5).Trim();
                else if (s.Contains("rowgap="))
                    RowGag = s.Substring(s.IndexOf("rowgap=") + 7).Trim().Split('|');
                else if (s.Contains("colgap="))
                    ColGap = s.Substring(s.IndexOf("colgap=") + 7).Trim().Split('|');
                else if (s.Contains("location="))
                    Location = new BETags(s.Substring(s.IndexOf("location=") + 9).Trim());
            }
        }
        public string Table(int RowIndex, int ColIndex)
        {
            if (txt != null)
            {
                if (str == null || !string.ReferenceEquals(str, txt))
                {
                    str = txt;
                    string s = Location.Match(txt);
                    rows = s.Split(RowGag, StringSplitOptions.RemoveEmptyEntries);
                    cols = new string[rows.Length][];
    
                }
                if (RowIndex < rows.Length)
                {
                    if (cols[RowIndex] == null)
                        cols[RowIndex] = rows[RowIndex]
                            .Split(ColGap, StringSplitOptions.RemoveEmptyEntries);
                    if (ColIndex < cols[RowIndex].Length)
                        return cols[RowIndex][ColIndex];
                }
            }
            return "";
        }
        public string Name { get; set; }
        public string[] RowGag { get; set; }
        public string[] ColGap { get; set; }
        public BETags Location { get; set; }
        private string[] rows;
        private string[][] cols;
        private string str;
        public static string txt;
    
    }
}
