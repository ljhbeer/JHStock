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
    public delegate bool BoolCondition(string s);
    public delegate string ActCondition(string s);
    public class XmlConfig
    {
        public XmlConfig()
        {
            xedbroot = null;
            xesrc = null;
            xedst = null;
            SrcTags = null;
            SrcMode = "";
            inserttemple = "";
            updatetemple = "";
            itemupdatetemple="";
            _SrcPreDealTags=null;
            _MultiSubItemTag = null;
        }

        public bool InitData()
        {
            if (InitSrcData() &&
               InitDataSet() &&
               InitDstData())
            {
                if (SrcTags.Cmd != null)
                {
                    List<TableCmd> listtable = SrcTags.Cmd.ListTable;
                    foreach (FieldSet f in fs)
                    {
                        if (f.BETags != null &&
                           f.BETags.Cmd != null &&
                           f.BETags.Cmd.tablevalueCmd != null)
                            f.BETags.Cmd.tablevalueCmd.InitTable(listtable);
                    }
                    //// Init PreDealTags and MultiSubItemTag
                    if(SrcTags.Cmd.MultiSubItemTags!=null){
                    	BETags ts = SrcTags.Cmd.MultiSubItemTags;
                    	if (ts.tags.Count > 1)
	                    {
	                        _SrcPreDealTags  = ts.SubTags(0,ts.tags.Count - 1);
	                        _MultiSubItemTag = ts.tags[ts.tags.Count - 1];
                    	}else if(ts.tags.Count == 1){
                    		_MultiSubItemTag = ts.tags[ts.tags.Count - 1];
                    	}
                    }
                }
                return true;
            }
            return false;
        }
        private bool InitDataSet()
        {
            fs = new List<FieldSet>();
            foreach (XmlElement xe in xedbroot.ChildNodes)
            {
                fs.Add(new FieldSet(xe.GetAttribute("colname"),
                    xe.GetAttribute("valuetype"),
                    xe.GetAttribute("strdetail")));
            }
            return true;
        }
        private bool InitSrcData()
        {
            SrcID = xesrc.GetAttribute("SrcID");
            SrcExp = xesrc.GetAttribute("SrcExp");
            ProcessMode = xesrc.GetAttribute("ProcessMode");
            SrcTags = new BETags(xesrc.GetAttribute("SrcRule"));
            BETags bts = new BETags(SrcExp);
            if (SrcExp.Contains("]["))
            {
                if (SrcExp.Contains("(*)"))
                {
                    SrcMode = "Net-Number";
                    UrlTemplate = bts.tags[0].Begin;
                    if (bts.tags.Count != 2 || !ValidTools.ValidNumber(bts.tags[1].Begin))
                    {
                        SrcMode = "Net-Err";
                        ErrMsg = "替换（*）的起止序号设置不规范，只能设置1组，且必须是数字";
                        return false;
                    }
                    Beid = new BEId();
                    Beid.B = Convert.ToInt32(bts.tags[1].Begin);
                    Beid.E = int.MaxValue;
                    if (ValidTools.ValidNumber(bts.tags[1].End))
                        Beid.E = Convert.ToInt32(bts.tags[1].End);
                    ///////////
                    Beid.MoveToTop();
                }
                else
                {
                    SrcMode = "Net-NextUrl";
                    NextUrlTags = bts.SubTags(1);
                    if (bts.tags.Count == 0 || NextUrlTags.tags.Count == 0)
                    {
                        SrcMode = "Net-Err";
                        ErrMsg = "没有定义NextUrl的起止标志";
                        return false;
                    }
                    UrlTemplate = bts.tags[0].Begin;
                    Url = UrlTemplate;
                }
            }
            else
            {
                SrcMode = "DB";
                if (db != null)
                {
                    return true;
                }
                else
                {
                    ErrMsg = "cfg.数据库没有初始化！";
                    return false;
                }
            }
            return true;
        }
        private bool InitDstData()
        {
            string sql = ReplaceInsertSqlTemple(xedst, SqlInsertTemplate);
            inserttemple = sql;
            string sqlu = SqlUpdateTemplate;
            if (sqlu.Contains("[tablename]"))
                updatetemple = updatetemple.Replace("[tablename]", xedst.Attributes["dsttablename"].Value);
            return true;
        }

        public CItem ConstructItem()
        {
            if (SrcMode == "DB")
            {
                string BeginID = "0";
                string EndID = "0";
                string path = "";
                if (!ValidTools.ValidName(SrcExp))
                {
                    if (SrcExp.Contains(" as "))
                        SrcExp = SrcExp.Substring(SrcExp.IndexOf(" as ") + 4);
                    else
                    {
                        ErrMsg = "SrcExp 不合规范";
                        return null;
                    }
                }
                if (SrcTags.Cmd != null)
                {
                    if (SrcTags.Cmd.Bedbid != null)
                    {
                        BeginID = SrcTags.Cmd.Bedbid.B.ToString();
                        EndID = SrcTags.Cmd.Bedbid.E.ToString();
                    }
                    if (SrcTags.Cmd.SavePath != null)
                        path = SrcTags.Cmd.SavePath;
                }
                string
                SqlTemplate = "select top 1000 [SrcID],[SrcExp] from [SrcDbTableT] where [SrcID] > [maxid] order by [SrcID]";
                SqlTemplate = SqlTemplate.Replace("[SrcID]", "[" + xesrc.GetAttribute("SrcID") + "]")
                        .Replace("[SrcExp]", xesrc.GetAttribute("SrcExp"))
                        .Replace("[SrcDbTableT]", "[" + xesrc.GetAttribute("SrcDbTableT") + "]");
                if (EndID != "0")
                    SqlTemplate = SqlTemplate.Replace("[maxid]", "[maxid] and [" + SrcID + "]<=" + EndID + " ");
                ////////////////
                if (ProcessMode == "download")
                {
                    CItem ind = new ItemNetDownLoad(db, SrcID, SrcExp, SqlTemplate, BeginID, EndID, path);
                    ind = ind.MoveNext();
                    return ind;
                }
                else
                {
                    if (ProcessMode == "txt")
                    {
                        return new ItemDBTxt(db, SrcID, SrcExp, SqlTemplate, BeginID, EndID).MoveNext();
                    }
                    else if (ProcessMode == "url")
                    {
                        return new ItemDBUrl(db, SrcID, SrcExp, SqlTemplate, BeginID, EndID).MoveNext();
                    }


                }
                return null;
            }
            else if (SrcMode == "Net-NextUrl")
            {
                if (Url == null) return null;
                return new ItemNetNextUrl(Url, UrlTemplate, NextUrlTags);
            }
            else if (SrcMode == "Net-Number")
            {
                if (Beid.HasNext)
                {
                    Url = UrlTemplate.Replace("(*)", Beid.ActiveIndex.ToString());
                    return new ItemNetNumber(Url, UrlTemplate, Beid);
                }
                return null;
            }
            return null;
        }
        public void Read(DataGridView dgvdb, DataGridView dgvsrc, DataGridView dgvdst)
        {
            XmlDocument XmlDb = DataGridToXml(dgvdb, "DbSet");
            XmlDocument XmlSrc = DataGridToXml(dgvsrc, "Src");
            XmlDocument XmlDst = DataGridToXml(dgvdst, "Dst");
            xedbroot = XmlDb.DocumentElement;
            xesrc = (XmlElement)XmlSrc.DocumentElement.FirstChild;
            if (XmlDst != null)
                xedst = (XmlElement)XmlDst.DocumentElement.FirstChild; ;
        }
        public void ImportXml(DataGridView dgvsrc, DataGridView dgvdst, DataGridView dataGridView1)
        {
            XmlDocument XmlDoc = OpenAndLoadXml();
            if (XmlDoc != null)
            {
                XmlToDgv(GetXmlElementByTag(XmlDoc, "Src"), dgvsrc);
                XmlToDgv(GetXmlElementByTag(XmlDoc, "Dst"), dgvdst);
                XmlToDgv(GetXmlElementByTag(XmlDoc, "DbSet"), dataGridView1);
            }
        }
        public void ExportXml(DataGridView dgvsrc, DataGridView dgvdst, DataGridView dataGridView1)
        {
            XmlDocument XmlDb = DataGridToXml(dataGridView1, "DbSet");
            XmlDocument XmlSrc = DataGridToXml(dgvsrc, "Src");
            XmlDocument XmlDst = DataGridToXml(dgvdst, "Dst");

            string rootname = "toolsxml";
            XmlDocument XmlDoc = new XmlDocument();
            XmlElement root = XmlDoc.CreateElement(rootname);
            XmlDoc.AppendChild(root);
            if (XmlSrc != null)
                root.AppendChild(XmlDoc.ImportNode(XmlSrc.DocumentElement, true));
            if (XmlDst != null)
                root.AppendChild(XmlDoc.ImportNode(XmlDst.DocumentElement, true));
            if (XmlDb != null)
                root.AppendChild(XmlDoc.ImportNode(XmlDb.DocumentElement, true));

            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "保存xml配置文件";
            fd.Filter = "配置文件(*.xml)|*.xml";
            //fd.DefaultExt = 
            fd.FileName = "NewCfg.xml";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (fd.FileName.EndsWith(".xml"))
                {
                    XmlDoc.Save(fd.FileName);
                }
            }
        }
        private static string FormatBeginEndID(string SrcBeginEndID)
        {
            string id = "0";
            try
            {
                int startid = Convert.ToInt32(SrcBeginEndID);
                id = startid.ToString();
            }
            catch { }
            return id;
        }
        private static string ConstructInsertSqlTemple(XmlElement xedbroot)
        {
            string inserttemple = "insert into [tablename](";
            string value = "";
            foreach (XmlElement xe in xedbroot.ChildNodes)
            {
                string colname = xe.GetAttribute("colname");
                inserttemple += "[" + colname + "],";
                if (xe.GetAttribute("valuetype") == "int")
                {
                    value += "[-" + colname + "-],";
                }
                else
                {
                    value += "'[-" + colname + "-]',";
                }
            }
            if (inserttemple.EndsWith(","))
                inserttemple = inserttemple.Substring(0, inserttemple.Length - 1);
            if (value.EndsWith(","))
                value = value.Substring(0, value.Length - 1);
            inserttemple = inserttemple + ") values (" + value + " );\r\n";
            return inserttemple;
        }
        private static string ConstructInsertSQL(string inserttemple, string item, XmlElement xedbroot)
        {
            string sql = inserttemple;
            foreach (XmlElement xe in xedbroot.ChildNodes)
            {
                string colname = xe.GetAttribute("colname");
                string strbegin = xe.GetAttribute("strbegin");
                string strend = xe.GetAttribute("strend");
                string multi = xe.GetAttribute("multimatch");
                if (multi == "T")
                    sql = sql.Replace("[-" + colname + "-]", StringTools.GetEqualValueMulti(item, strbegin, strend));
                else
                    sql = sql.Replace("[-" + colname + "-]", StringTools.GetEqualValue(item, strbegin, strend));
            }
            return sql;
        }
        private static string ReplaceInsertSqlTemple(XmlElement xedst, string inserttemple)
        {
            if (xedst == null)
                return inserttemple;
            String DstIsCreate = xedst.Attributes["DstIsCreate"].Value;
            String dstpath = xedst.Attributes["dstpath"].Value;
            String dsttablename = xedst.Attributes["dsttablename"].Value;
            String DstIsCreateID = xedst.Attributes["DstIsCreateID"].Value;
            String DstsaveSrcIDAs = xedst.Attributes["DstsaveSrcIDAs"].Value;

            string sqltemp = "[;DATABASE=dstpath].[dsttablename]";

            String DstIsSameToSrc = "true";
            if (xedst.Attributes["DstIsSameToSrc"] != null)
                DstIsSameToSrc = xedst.Attributes["DstIsSameToSrc"].Value;
            if (DstIsSameToSrc == "True")
                sqltemp = sqltemp.Replace("[;DATABASE=dstpath].", "");
            else
                sqltemp = sqltemp.Replace("dstpath", dstpath);
            if (dsttablename == "") return "";
            sqltemp = sqltemp.Replace("dsttablename", dsttablename);

            string insertinto = "insert into [tablename]([id],";
            if (DstIsCreateID == "False")
                insertinto = "insert into [tablename]([id],";
            else
                insertinto = "insert into [tablename]([" + DstsaveSrcIDAs + "],";

            inserttemple = inserttemple
                .Replace("insert into [tablename](", insertinto)
                .Replace("[tablename]", sqltemp)
                .Replace("values (", "values ([-id-],");
            return inserttemple;
        }
        public static XmlDocument DataGridToXml(DataGridView dgv, string rootname)
        {
            if (dgv == null) return null;
            XmlDocument XmlDoc = new XmlDocument();
            XmlElement root = XmlDoc.CreateElement(rootname);
            XmlDoc.AppendChild(root);
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                if (!dr.IsNewRow)
                {
                    XmlElement xe = XmlDoc.CreateElement("Item");
                    foreach (DataGridViewColumn dc in dgv.Columns)
                    {
                        if (dr.Cells[dc.Index].Value == null)
                        {
                            xe.SetAttribute(dc.Name, "");
                        }
                        else
                        {
                            xe.SetAttribute(dc.Name, dr.Cells[dc.Index].Value.ToString());
                        }
                    }
                    root.AppendChild(xe);
                }
            }
            return XmlDoc;
        }
        private static XmlDocument OpenAndLoadXml()
        {
            XmlDocument XmlDoc = new XmlDocument(); ;
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "打开xml配置文件";
            fd.Filter = "配置文件(*.xml)|*.xml";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                if (fd.FileName.EndsWith(".xml"))
                {
                    XmlDoc.Load(fd.FileName);
                    return XmlDoc;
                }
            }
            return null;
        }

        public bool LoadXml(string FileName)//????????
        {
            if (FileName.EndsWith(".xml"))
            {
                try
                {
                    XmlDocument XmlDoc = new XmlDocument();
                    XmlDoc.Load(FileName);
                    this.xedbroot = GetXmlElementByTag(XmlDoc, "DbSet");
                    this.xesrc = GetXmlElementByTag(XmlDoc, "Src");
                    this.xedst = GetXmlElementByTag(XmlDoc, "Dst");
                    if (xedst.ChildNodes.Count == 1
                      || xesrc.ChildNodes.Count == 1)
                    {
                        this.xedst = (XmlElement)this.xedst.FirstChild;
                        this.xesrc = (XmlElement)this.xesrc.FirstChild;
                        return true;
                    }
                }
                catch
                {

                }
            }
            return false;
        }
        private static XmlElement GetXmlElementByTag(XmlDocument XmlDoc, string tagname)
        {
            if (XmlDoc.GetElementsByTagName(tagname).Count == 1)
            {
                return (XmlElement)(XmlDoc.GetElementsByTagName(tagname)[0]);
            }
            return null;
        }
        private static void XmlToDgv(XmlElement root, DataGridView dgv)
        {
            //XmlElement root = XmlDoc.DocumentElement;
            if (root == null || dgv == null) return;
            dgv.Rows.Clear();
            if (dgv.AllowUserToAddRows)
                dgv.RowCount = root.ChildNodes.Count + 1;
            else
                dgv.RowCount = root.ChildNodes.Count;
            int rowindex = 0;
            foreach (XmlElement xe in root.ChildNodes)
            {
                foreach (DataGridViewColumn dc in dgv.Columns)
                {
                    if (dc.GetType().Name == "DataGridViewCheckBoxColumn")
                    {
                        string value = xe.GetAttribute(dc.Name);
                        if (value != "True" && value != "TRUE")
                            value = "False";
                        dgv[dc.Index, rowindex].Value = value;
                    }
                    else if (dc.GetType().Name == "DataGridViewComboBoxColumn")
                    {
                        string value = xe.GetAttribute(dc.Name);
                        if (((DataGridViewComboBoxCell)dgv[dc.Index, rowindex]).Items.Contains(value))
                        {
                            dgv[dc.Index, rowindex].Value = value;
                        }

                    }
                    else
                    {
                        dgv[dc.Index, rowindex].Value = xe.GetAttribute(dc.Name);
                    }
                }
                rowindex++;
            }
        }
        public string SrcBeginID()
        {
            if (NextUrlTags != null && NextUrlTags.tags.Count > 1)
                return "1";
            else if (Beid != null)/////// && Beid.B!=null)
            {
                return Beid.B.ToString();
            }
            return "0";
        }
        public string SqlInsertTemplate
        {
            get
            {
                if (inserttemple == "")
                {
                    inserttemple = "insert into [tablename](";
                    string value = "";
                    foreach (XmlElement xe in xedbroot.ChildNodes)
                    {
                        string colname = xe.GetAttribute("colname");
                        inserttemple += "[" + colname + "],";
                        if (xe.GetAttribute("valuetype") == "int"
                         || xe.GetAttribute("valuetype") == "double")
                        {
                            value += "[-" + colname + "-],";
                        }
                        else
                        {
                            value += "'[-" + colname + "-]',";
                        }
                    }
                    if (inserttemple.EndsWith(","))
                        inserttemple = inserttemple.Substring(0, inserttemple.Length - 1);
                    if (value.EndsWith(","))
                        value = value.Substring(0, value.Length - 1);
                    inserttemple = inserttemple + ") values (" + value + " );\r\n";
                    //inserttemple = value.Replace("'","").Replace(",","\t") + "\r\n";

                }
                return inserttemple;

            }
        }
        
        
		public void PreDealTxt(ref string txt)
		{
			if(_SrcPreDealTags!=null){
				txt = _SrcPreDealTags.Match(txt);
			}
		}
        public string SqlItemUpdateTemplate
        {
        	get{
        		if(itemupdatetemple==""){
        			string ut = "update [tablename] Set [Item]='[-Item-]'," +
        				"[UpdateNumber]=[-UpdateNumber-]";
        			string tablename = "";
    				if(xedst!=null)
    					tablename ="Item"+ xedst.Attributes["dsttablename"].Value;
        			if (ut.Contains("[tablename]"))
                    	ut = ut.Replace("[tablename]", tablename);     
					ut = ut.Replace("[-UpdateNumber-]",TimeStringTools.UpdateNumber());        			
        			itemupdatetemple = ut+" where id = [-id-];\r\n";
        		}
        		return itemupdatetemple;
        	}
        }
        public string DstTableName{ 
        	get { if(xedst!=null)
        	         return xedst.Attributes["dsttablename"].Value; 
        	         return "";}
        }
        public string SqlUpdateTemplate
        {
            get
            {
                if (updatetemple == "")
                {
                    string ut = "update [tablename] Set ";
                    string value = "";
                    foreach (XmlElement xe in xedbroot.ChildNodes)
                    {
                        string colname = xe.GetAttribute("colname");
                        if (xe.GetAttribute("valuetype") == "int"
                           || xe.GetAttribute("valuetype") == "double")
                        {
                            value = "[-" + colname + "-],";
                        }
                        else
                        {
                            value = "'[-" + colname + "-]',";
                        }
                        ut += "[" + colname + "]=" + value;
                    }
                    if (ut.EndsWith(","))
                        ut = ut.Substring(0, ut.Length - 1);
                    updatetemple = ut + " where id = [-id-];\r\n";
                }
                return updatetemple;
            }
        }      
        public string GetSqlUpdateUrlItemtxtTemplate()
        {
            string SqlTemplate = "select A.[SrcID],[SrcExp],[Item] from [SrcDbTableT] as A,[TableName] as B  "+
            	"where A.[SrcID] = B.[SrcID]  [IDS]"+" order by A.[SrcID]";
            SqlTemplate = SqlTemplate.Replace("[SrcID]", "[" + xesrc.GetAttribute("SrcID") + "]")
            		.Replace("[SrcExp]", xesrc.GetAttribute("SrcExp"))
                    .Replace("[TableName]", "[Item" + xedst.GetAttribute("dsttablename") + "]")
                    .Replace("[SrcDbTableT]", "[" + xesrc.GetAttribute("SrcDbTableT") + "]");
            return SqlTemplate;
        }
        public void SetSqlUpdateTemplate(string t)
        {
            updatetemple = t;
        }

        public string SrcID { get; set; }
        public string SrcExp { get; set; }
        public string ProcessMode { get; set; }
        public string UrlTemplate { get; set; }
        public string SrcMode { get; set; }
        public BETags NextUrlTags { get; set; }
        public BEId Beid { get; set; }
        public List<FieldSet> fs { get; set; }

        public BETags SrcTags { get; set; }
        public string Url { get; set; }
        public Db.ConnDb db { get; set; }
        public string ErrMsg { get; set; }

        public XmlElement xedbroot;
        public XmlElement xesrc;
        public XmlElement xedst;
        private string inserttemple;
        private string updatetemple;
        private string itemupdatetemple;
        
        private BETags _SrcPreDealTags;
        private BETag  _MultiSubItemTag;
    	
    }
}
