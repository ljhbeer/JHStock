using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using Tools;
using ToolsCXml;

namespace JHStock
{
	public class UpdateStocks{  // Update to Database
		public UpdateStocks(Stocks stocks){	
			_stocks  = stocks;
			_cfg = _stocks.Gcfg;
            ItemTxt = new string[2000];
            //UpdateUrl = new string[2000];
			_exceptionfilename = TimeStringTools.NowDateMin() + "_UpdateException.log";
		}
		public bool InitXmlConfig(string xmlfilename){
			_xcfg = new XmlConfig();
			_xcfg.db =_cfg.db;
			if (_xcfg.LoadXml(xmlfilename) && _xcfg.InitData())
				if (_xcfg.SrcMode != "DB"|| _xcfg.ProcessMode == "txt" || _xcfg.ProcessMode == "url")
					return true;
			return false;
		}
		public void UpdateHexin(List<Stock> usstocks)
		{			
			string tsql="";
			foreach(Stock s  in usstocks){  
				try{
	                tsql = _xcfg.SqlUpdateTemplate.Replace("[-id-]", s.ID.ToString());
				    string txt = ItemTxt[s.ID];
	                TableCmd.txt = txt;
	                foreach (FieldSet f in _xcfg.fs)
	                {
	                    string str = f.GetValue(txt, "");
	                    tsql = tsql.Replace("[-" + f.Name + "-]", str.Replace("'", "''"));
	                }
	                MFile.AppendAllText("UpdateHexin", tsql);
	                _cfg.db.update(tsql);
				} catch(Exception e){
			   		MFile.WriteAllText(_exceptionfilename,s.ID+"\t"+e.Message+"\t"+tsql+"\r\n");
			    }
			}
		}
		public void UpdateStockBouns(List<Stock> usstocks)
		{			
	            BETag bt = new BETag("");               
	            if (_xcfg.SrcTags.Cmd != null
	             && _xcfg.SrcTags.Cmd.MultiSubItemTags !=null
	             && _xcfg.SrcTags.Cmd.MultiSubItemTags.tags.Count > 0)               
	            {
	            	BETags ts = _xcfg.SrcTags.Cmd.MultiSubItemTags;
				    bt = ts.tags[ts.tags.Count - 1];
	            }else{
	            	throw new Exception("配置文件不正确，没有 MultiSubItemTags");
	            }
			TableRowCmd trc = _xcfg.SrcTags.Cmd.tableRowCmd;
	            BETag trcbt = trc.Cols.tags[0];
	            string insertsql = _xcfg.SqlInsertTemplate;
	            insertsql = insertsql.Replace("'[-CQDate-]'","[-CQDate-]").Replace("sinabouns","bouns");
			foreach(Stock s  in usstocks){    
				FQItem fi = this._cfg.FQT.FQ[s.ID];            	
	                string tsql = insertsql.Replace("[-id-]", s.ID.ToString());
				string txt = ItemTxt[s.ID];
				BEPos bp = new BEPos(0, txt.Length, txt);
	                bp = bt.BEPos(txt);
	                while (bp.Valid())
	                {
	                	string sql = tsql;
	                	string stritem = bp.String;
	                    if (trc.Replace != null)
	                    {
	                        foreach (BETag betag in trc.Replace.tags)
	                            stritem = stritem.Replace(betag.Begin, betag.End);
	                    }
	                    List<BEPos> itembps = new List<BEPos>();
	                    BEPos itembp = trcbt.BEPos(stritem);
	                    while (itembp.Valid())
	                    {
	                        itembps.Add(itembp);
	                        itembp = trcbt.NextBEPos(itembp);
	                    }
	                    //int sum = 0;
	                    if (itembps.Count == 9)
	                    {
	                        if (itembps[4].String == "实施")
	                        {
	                            if (fi != null && fi._bouns.Count > 0)
	                            {
	                                string date = itembps[5].String.Replace("-", "");
	                                if(date=="")
	                                    date = itembps[6].String.Replace("-", "");
	                                if (Convert.ToInt32( date )
	                                    <= fi._bouns[0].date)
	                                    break;
	                            }
	                            foreach (ToolsCXml.FieldSet f in _xcfg.fs)
	                                sql = sql.Replace("[-" + f.Name + "-]", f.GetItemRowValue(txt, itembps).Replace("'", "''"));
	                            sql = sql.Replace("-","");
	                            if(_cfg.UpdateDebug)
	                            	MFile.AppendAllText(TimeStringTools.NowDate()+"_UpdateSinaBoun_sql.log", sql);
	                            _cfg.db.update(sql);
	                        }
	                    }
	                    bp = bt.NextBEPos(bp);
	                }
			}
		}
		public void UpdateTopTen(List<Stock> usstocks){
			string sql="";
			foreach(Stock s  in usstocks){
			   try{                   
	                string txt = ItemTxt[s.ID];
	                sql =_xcfg.SqlUpdateTemplate.Replace("[-id-]", s.ID.ToString());                    
	                TableCmd.txt = txt;
	                foreach (ToolsCXml.FieldSet f in _xcfg.fs)
	                {
                        string ss = f.GetValue(txt, ""); //UpdateUrl[s.ID]
	                    sql = sql.Replace("[-" + f.Name + "-]", ss.Replace("'", "''"));
	                }
	                sql = sql.Replace("--","0");
	                MFile.AppendAllText("UpdateTopTen", sql);
	              	_xcfg.db.update(sql);
	           }catch(Exception e){
			   	   MFile.WriteAllText(_exceptionfilename,s.ID+"\t"+e.Message+"\t"+sql+"\r\n");
			   }
			}
		}
		public List<Stock> InitUsstocks(string sql){
			List<Stock> usstocks = new List<Stock>();
			DataTable dt =_stocks.Gcfg.db.query(sql).Tables[0];
			foreach (DataRow dr in dt.Rows){
				Stock s = _stocks.StockByIndex((int)dr["id"]);
				usstocks.Add(s);
				ItemTxt[s.ID] = dr["Item"].ToString();
			}
			return usstocks;
		}
		
		private Stocks _stocks;
		private GlobalConfig _cfg;
		private XmlConfig _xcfg;
		private string _exceptionfilename;
        private string[] ItemTxt;
        //private string[] UpdateUrl; //////////////没有初始化
	}
}
