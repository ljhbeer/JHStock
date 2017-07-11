using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Tools;

namespace ToolsCXml
{
    public class CItem
    {
        public CItem( )
        {
            htmltxt = "";
        }
        public virtual  CItem MoveNext()
        {
            throw new NotImplementedException();
        }
        public int ID { get; set; }
        public string Url { get; set; }
        //public string FileName { get; set; }

        public string Text {
            get
            {
                if (htmltxt == "")
                {
                    if (Url != null)
                        htmltxt = web.GetOKUrl(Url);
                    else
                        htmltxt = "";
                }
                return htmltxt;
            }
        }
        protected string htmltxt;
        public  static CWeb web = new CWeb();

        public string UpdateConditionTxt { get; set; }

        public BETag BECondition { get; set; }
    }
    public class ItemDB : CItem 
    {
        public ItemDB(Db.ConnDb db, string SrcID,string SrcExp, string SqlTemplate, string BeginID, string EndID)
        {
            this.db = db;
            this.activeindex = 0;
            this.maxid = "0";
            if (BeginID != "")
                maxid = BeginID;
            this.SrcID = SrcID;
            this.SrcExp = SrcExp;
            this.SqlTemplate = SqlTemplate;
            Rows = null;
            ActiveDatarow = null;
            this.db = db;
            this.BeginID = BeginID;
            this.EndID = EndID;
            this.UpdateConditionTxt = "";
        }
        public override CItem MoveNext()
        {
            if (Rows == null || activeindex == Rows.Count)
            {
                string sql = SqlTemplate.Replace("[maxid]", maxid);
                DataSet ds = db.query(sql);
                Rows = ds.Tables[0].Rows;
                if (Rows.Count == 0) return null;
                maxid = Rows[Rows.Count - 1][SrcID].ToString();
                activeindex = 0;
            }
            if (activeindex < 0)
                activeindex = 0;
            if (activeindex < Rows.Count)
            {
                activeindex++;
                ActiveDatarow = Rows[activeindex - 1];
                //return ActiveDatarow[SrcID].ToString();
                return this;
            }
            return null;
        }


        protected int activeindex;
        protected string maxid;
        protected string SrcID;
        protected string SrcExp;
        protected string SqlTemplate;
        private string BeginID;
        private string EndID;
        protected DataRowCollection Rows;
        protected DataRow ActiveDatarow;
        protected Db.ConnDb db;
    }
    public class ItemDBTxt : ItemDB
    {
        public ItemDBTxt(Db.ConnDb db, string SrcID, string SrcExp, string SqlTemplate, string BeginID, string EndID)
            : base(db, SrcID, SrcExp, SqlTemplate, BeginID, EndID)
        {

        }
        public override CItem MoveNext()
        {
            if (base.MoveNext() != null)
            {
                ID = (int)ActiveDatarow[SrcID];
                //Url = ActiveDatarow[SrcExp].ToString();
                htmltxt = ActiveDatarow[SrcExp].ToString();
                return this;
            }
            return null;
        }
    }
    public class ItemDBUrl : ItemDB
    {
        public ItemDBUrl(Db.ConnDb db, string SrcID, string SrcExp, string SqlTemplate, string BeginID, string EndID)
            : base(db, SrcID, SrcExp, SqlTemplate, BeginID, EndID)
        {

        }
        public override CItem MoveNext()
        {
            if (base.MoveNext() != null)
            {
                ID = (int)ActiveDatarow[SrcID];
                Url = ActiveDatarow[SrcExp].ToString();
                //UpdateConditionTxt = ActiveDatarow["净资产"].ToString();
                htmltxt = "";
                return this;
            }
            return null;
        }
    }
    public class ItemNetDownLoad : ItemDB
    {
        public ItemNetDownLoad(Db.ConnDb db, string SrcID,string SrcExp, string SqlTemplate, string BeginID, string EndID,string path="")
            : base(db,SrcID,SrcExp,SqlTemplate,BeginID,EndID)
        {
            if(path!="" && !path.EndsWith("\\"))
                path=path+"\\";
            this.Path = path;
        }
        public override CItem MoveNext()
        {
            if (base.MoveNext() != null)
            {
                ID =(int)ActiveDatarow[SrcID];
                Url = ActiveDatarow[SrcExp].ToString();
                FileName =Path + ID + MFile.GetFileNameExt(Url);
                return this;
            }
            return null;
        }
        public string FileName { get; set; }
        private string Path;
    }
    public class ItemNetNumber : CItem
    {
        public ItemNetNumber(string Url, string UrlTemplate, BEId Beid)
        {
            this.Url = Url;
            this.UrlTemplate = UrlTemplate;
            this.Beid = Beid;
            ID = Beid.ActiveIndex;
        }
        public override CItem MoveNext()
        {
            Beid.MoveNext();
            if (Beid.HasNext)
            {
                Url = UrlTemplate.Replace("(*)", Beid.ActiveIndex.ToString());
                ID = Beid.ActiveIndex;
                htmltxt = "";
                return this;
            }
            return null;
        }
        private BEId Beid;
        private string UrlTemplate;
    }
    public class ItemNetNextUrl : CItem
    {
        public ItemNetNextUrl(string Url, string UrlTemplate, BETags NextUrlTags)
        {
            ID = 0;
            this.Url = Url;
            this.UrlTemplate = UrlTemplate;
            this.NextUrlTags = NextUrlTags;
        }
        public override CItem MoveNext()
        {
            string nexturl = ComputeNextUrl(htmltxt);
            if (nexturl != null)
            {
                Url = nexturl;
                ID++;
                htmltxt = "";
                return this;
            }              
            return null;
        }
        private string ComputeNextUrl(string txt)
        {
            string url = NextUrlTags.Match(txt);
            if (ValidTools.ValidUrl(url))
            {
                if (url.Contains("/"))
                {
                    string[] paths = url.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in paths)
                    {
                        if (UrlTemplate.Contains(s))
                        {
                            return UrlTemplate.Substring(0, UrlTemplate.LastIndexOf("/" + s)) + url;                          
                        }
                    }
                }
            }
            return null;
        }
        private string UrlTemplate;
        private BETags NextUrlTags;
    }
}
