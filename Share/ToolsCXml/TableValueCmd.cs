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
    public class TableValueCmd
    {
        private int RowIndex;
        private int ColIndex;
        public TableValueCmd(string str)
        {
            Valid = false;
            ColIndex = RowIndex = -1;
            string[] items = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in items)
            {
                if (s.Contains("name="))
                    Name = s.Substring(s.IndexOf("name=") + 5).Trim();
                else if (s.Contains("rowindex="))
                    RowIndex = Convert.ToInt32(s.Substring(s.IndexOf("rowindex=") + 9));
                else if (s.Contains("colindex="))
                    ColIndex = Convert.ToInt32(s.IndexOf("colindex=") + s.Substring(9));
            }
    
        }
        public void InitTable(List<TableCmd> listtable)
        {
            if (RowIndex >= 0 && ColIndex >= 0)
                foreach (TableCmd t in listtable)
                    if (Name == t.Name)
                    {
                        Tablecmd = t;
                        Valid = true;
                        break;
                    }
        }
        public bool Valid { get; set; }
        public string Name { get; set; }
        public TableCmd Tablecmd { get; set; }
        public string GetValue() //RowIndex,ColIndex必定 >=0
        {
            if (Tablecmd != null)
                return Tablecmd.Table(RowIndex, ColIndex);
            return "";
        }
    }
}
