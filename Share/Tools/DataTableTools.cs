using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace Tools
{
    public  class DataTableTools
    {
    	public static List<string> ReadTableNames(Db.ConnDb db)
		{
    		List<string> names = new List<string>();
			try
			{
				string sql = "select [name] from [msysobjects] where type=1 and flags = 0 order by [name]";
				DataSet ds = db.query(sql);
				foreach (DataRow drc in ds.Tables[0].Rows)
					names.Add(drc[0].ToString());
			}
//			catch (System.Data.OleDb.OleDbException e)
//			{
//				
//			}
			finally{
			}
				return names;
		}
        public static DataTable ConstructDataTable(string[] columntitles)
        {
            DataTable dt = new DataTable();
            for (int count = 0; count < columntitles.Length; count++)
            {
                DataColumn dc = new DataColumn(columntitles[count]);
                if ("代码旧名称新名称".Contains(columntitles[count]))
                {
                    dc.DataType = typeof(string);
                }
                else if ("ID".Contains(columntitles[count]))
                {
                    dc.DataType = typeof(int);
                }
                else if ("是否更改是否删除".Contains(columntitles[count]))
                {
                    dc.DataType = typeof(bool);
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }
        public static DataTable ConstructDataTable(List<string> titles,NameTpye nt,Type defaulttype){
			DataTable dt = new DataTable();
			foreach(string s in titles){
				DataColumn dc = new DataColumn(s);
				dc.DataType = null;
				foreach( KeyValuePair<string,List<string>> kvp in nt.dic){
					if(kvp.Value.Contains(s)){
						string key = kvp.Key;
						if(key =="string")
							dc.DataType = typeof(string);
						else if(key == "bool")
							dc.DataType = typeof(bool);
						else if(key == "double")
							dc.DataType = typeof(double);
						else if(key == "float")
						 	dc.DataType = typeof(float); 						
						else if(key == "int")
						 	dc.DataType = typeof(int); 
						else
							dc.DataType = typeof(string); 
						break;
					}
				}
				if(dc.DataType == null){					
					dc.DataType = defaulttype;
				}
				dt.Columns.Add(dc);
			}
			return dt;
		}
        public static DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].HeaderText);
                dc.DataType = dgv.Columns[count].ValueType;
                dt.Columns.Add(dc);
            }
            //DataGridViewColumn[] dcs = new DataGridViewColumn[dt.Columns.Count];
            //dgv.Columns.CopyTo(dcs, 0);
            //List<string> ss = new List<string>(dcs.Select(r => r.ValueType.Name));
            //string s = string.Join("\t", ss);
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    Type type = dgv.Columns[countsub].ValueType;
                    if (dgv.Rows[count].Cells[countsub].Value != DBNull.Value
                       && dgv.Rows[count].Cells[countsub].Value != null)
                    {
                        if (type == typeof(string))
                            dr[countsub] = dgv.Rows[count].Cells[countsub].Value.ToString();
                        else if (type == typeof(double) || type == typeof(float))
                            dr[countsub] = Convert.ToDouble(dgv.Rows[count].Cells[countsub].Value.ToString());
                        else if (type == typeof(int))
                            dr[countsub] = Convert.ToInt32(dgv.Rows[count].Cells[countsub].Value.ToString());
                        else
                        {
                            MessageBox.Show(type.ToString());
                            ;
                        }
                    }
                    else
                    {
                        if (type == typeof(string))
                            dr[countsub] = "";
                        else
                            dr[countsub] = 0;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static string DataTableToTxt(DataTable dtsave)
        {
            StringBuilder outstr = new StringBuilder();
            foreach (DataColumn dc in dtsave.Columns)
            {
                outstr.Append(dc.ColumnName + ",");
            }
            outstr.Append("\r\n");
            foreach (DataRow dr in dtsave.Rows)
            {
                foreach (DataColumn dc in dtsave.Columns)
                {
                    if (dc.DataType != typeof(System.Drawing.Image))
                        outstr.Append(dr[dc.ColumnName] + ",");
                    else
                        outstr.Append("image,");

                }
                outstr.Append("\r\n");
            }
            outstr = outstr.Replace(",\r\n", "\r\n");
            return outstr.ToString();
        }
    }
    public class NameTpye{
    	public NameTpye(){
    		dic = new Dictionary<string, List<string>>();
    	}
    	public Dictionary<string ,List<string>> dic{get;set;} // type, titles
    }
}
