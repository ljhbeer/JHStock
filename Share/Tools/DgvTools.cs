using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Tools
{
    public class DgvTools
    {
        public static int FindIndex(string[] colums, string indexname)
        {
            int roomidindex = -1;
            for (int i = 0; i < colums.Length; i++)
                if (colums[i] == indexname)
                {
                    roomidindex = i;
                    break;
                }
            return roomidindex;
        }       
        public static string DataTableToTxt(DataTable dtsave )
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
                    if (dc.DataType != typeof(Image))
                        outstr.Append(dr[dc.ColumnName] + ",");
                    else 
                        outstr.Append( "image,");
    
                }
                outstr.Append("\r\n");
            }
            outstr = outstr.Replace(",\r\n", "\r\n");
            return  outstr.ToString();
        }
        public static void InitDataGridViewStyle(DataGridView dgv, string cfg)
        {
            if (dgv == null || cfg == null)
                return;
            string[] c = cfg.Split(new string[] { "{", "}", "(", ")", }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in c)
            {
                string colname = StringTools.GetEqualValue(s, "\"colname\":[", "]");
                string columset = StringTools.GetEqualValue(s, "\"columset\":[", "]");
                if (dgv.Columns.Contains(colname))
                {
                    string[] cs = columset.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string r in cs)
                    {
                        string[] vs = r.Split(new string[] { ":", "\"" }, StringSplitOptions.RemoveEmptyEntries);
                        if (vs.Length != 2) continue;
                        if (vs[0] == "visible")
                        {
                            if (vs[1] == "true")
                                dgv.Columns[colname].Visible = true;
                            else if (vs[1] == "false")
                                dgv.Columns[colname].Visible = false;
                        }
                        else if (vs[0] == "width")
                        {
                            dgv.Columns[colname].Width = Convert.ToInt32(vs[1]);
                        }
                    }
                }
            }

        }
        public static void InitDataGridViewColumns(DataGridView dgv, string strcolums)
        {
            if (dgv == null || strcolums == null)
                return;
            strcolums = strcolums.Replace("\r\n", "");
            string[] c = strcolums.Split(new string[] { "{", "}", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in c)
            {
                //string colname = s.Trim(); //GetEqualValue(s, "\"columname\":\"", "\"");
                if (s.Trim() == "")
                    continue;
                string colname = StringTools.GetEqualValue(s, "\"colname\":[", "]");
                string coltitle = StringTools.GetEqualValue(s, "\"coltitle\":[", "]");
                dgv.Columns.Add(colname, coltitle);
            }
        }
        public static void ChangeDataGridViewColumns(DataGridView dgv, string strcolums)
        {
            if (dgv == null || strcolums == null)
                return;
            strcolums = strcolums.Replace("\r\n", "");
            string[] c = strcolums.Split(new string[] { "{", "}", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in c)
            {
               
                if (s.Trim() == "")
                    continue;
                string colname = StringTools.GetEqualValue(s, "\"colname\":[", "]");
                string coltitle = StringTools.GetEqualValue(s, "\"coltitle\":[", "]");
                //dgv.Columns.Add(colname, coltitle);
                ChangeDgvColumns(dgv,colname, coltitle);
            }
        }
        private static void InitDataGridViewComBoBox(int rowNumber, DataGridView dgv)
        {
            DataRow drRows;
            //数据通道下来列表的绑定
            DataTable dtPrint_Data = new DataTable();
            dtPrint_Data.Columns.Add("Print_Data");
            drRows = dtPrint_Data.NewRow();
            drRows.ItemArray = new string[] { "Print_data" };
            dtPrint_Data.Rows.Add(drRows);
            DataGridViewComboBoxCell dgvComBoBoxPrint_Data = new DataGridViewComboBoxCell();
            dgvComBoBoxPrint_Data.DisplayMember = "Print_Data";
            dgvComBoBoxPrint_Data.ValueMember = "Print_Data";
            dgvComBoBoxPrint_Data.DataSource = dtPrint_Data;
            //字体的绑定
            DataTable dtFontNames = new DataTable();
            dtFontNames.Columns.Add("FontNames");
            foreach (System.Drawing.FontFamily ff in System.Drawing.FontFamily.Families)
            {
                drRows = dtFontNames.NewRow();
                drRows.ItemArray = new string[] { ff.Name };
                dtFontNames.Rows.Add(drRows);
            }
            DataGridViewComboBoxCell dgvComBoBoxFontNames = new DataGridViewComboBoxCell();
            dgvComBoBoxFontNames.DisplayMember = "FontNames";
            dgvComBoBoxFontNames.ValueMember = "FontNames";
            dgvComBoBoxFontNames.DataSource = dtFontNames;
            //绑定类型
            DataTable dtType = new DataTable();
            dtType.Columns.Add("Type");

            drRows = dtType.NewRow();
            drRows.ItemArray = new string[] { "文本" };
            dtType.Rows.Add(drRows);

            drRows = dtType.NewRow();
            drRows.ItemArray = new string[] { "字段" };
            dtType.Rows.Add(drRows);

            drRows = dtType.NewRow();
            drRows.ItemArray = new string[] { "国标码" };
            dtType.Rows.Add(drRows);

            drRows = dtType.NewRow();
            drRows.ItemArray = new string[] { "内部码" };
            dtType.Rows.Add(drRows);

            drRows = dtType.NewRow();
            drRows.ItemArray = new string[] { "图片" };
            dtType.Rows.Add(drRows);

            DataGridViewComboBoxCell dgvComBoBoxTypes = new DataGridViewComboBoxCell();
            dgvComBoBoxTypes.DisplayMember = "Type";
            dgvComBoBoxTypes.ValueMember = "Type";
            dgvComBoBoxTypes.DataSource = dtType;

            //字段名称绑定
            //DataTable dtDBText = new DataTable();
            //dtDBText.Columns.Add("DBTextX");

            //string strSelectPrint_FieldNames = "select chinesename from Print_FieldNames;";
            //DataSet dtSelectPrint_FieldNames = myCom.DataSelectReader(strSelectPrint_FieldNames, sqlLocalhost);
            //if (dtSelectPrint_FieldNames.Tables[0].Rows.Count > 0)
            //{
            //    for (int i = 0; i < dtSelectPrint_FieldNames.Tables[0].Rows.Count; i++)
            //    {
            //        drRows = dtDBText.NewRow();
            //        drRows.ItemArray = new string[] { dtSelectPrint_FieldNames.Tables[0].Rows[i][0].ToString() };
            //        dtDBText.Rows.Add(drRows);
            //    }
            //}
            //DataGridViewComboBoxCell dgvComBoBoxDBTextX = new DataGridViewComboBoxCell();
            //dgvComBoBoxDBTextX.DisplayMember = "DBTextX";
            //dgvComBoBoxDBTextX.ValueMember = "DBTextX";
            //dgvComBoBoxDBTextX.DataSource = dtDBText;

            //将它绑定到表格控件里去
            dgv.Rows[rowNumber].Cells[3] = dgvComBoBoxPrint_Data;
            dgv.Rows[rowNumber].Cells[2] = dgvComBoBoxFontNames;
            dgv.Rows[rowNumber].Cells[1] = dgvComBoBoxTypes;
            //dgv.Rows[rowNumber].Cells[4] = dgvComBoBoxDBTextX;

        }
        private static void ChangeDgvColumns(DataGridView dgv, string colname, string coltitle)
        {
            foreach (DataGridViewColumn dvc in dgv.Columns)
            {
                if (dvc.Name == colname)
                    dvc.HeaderText = coltitle;
            }
        }
        private static void ChangeDgvColumns(string colname, string coltitle)
        {
            throw new NotImplementedException();
        }           
    }

}
