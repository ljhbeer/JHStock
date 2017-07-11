using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Newtonsoft.Json;
using Tools;

namespace JHStock.Update
{

	public class CQFITool{
		public CQFITool(string qqfinpath,Stocks stocks){
			if(!qqfinpath.EndsWith("\\"))
				qqfinpath+="\\";
			this._qqfinpath = qqfinpath;
			this._stocks = stocks;
		}
		public QQfinItem[] QFI{
			get {
				if(_qfi==null)
					LoadDataQQfin();
				return _qfi;
			}
		}
		private void LoadDataQQfin(){
			 _qfi = new QQfinItem[2000];
			 foreach(Stock s in _stocks.stocks){
			 	if(_qfi[s.ID]==null)
			 		_qfi[s.ID]=new QQfinItem();
			 	_qfi[s.ID].LoadData(_qqfinpath+s.Code+".txt" );
			 }
		}
		private QQfinItem[] _qfi;
		private string _qqfinpath;
		private Stocks _stocks;
		#region Check UnUse
		private void ButtonQQFinTestClick(object sender, EventArgs e)
		{
			DateTime dt1 = System.DateTime.Now;
			//        	JHStock.Update.QQFin qff = new Update.QQFin(_stocks);
			//        	qff.Test();
			//CheckCorrectFin();
			//OutPutNewistDate();
			//ConstructNFI();
			//输出			
			//OutPutJZCSYL();
			//CNFITool nfi  = new CNFITool( cfg.WorkPath+"\\data\\NFT.txt");
			//string a = nfi.NFI[6].NCZNL[0].jlr;
			DateTime dt2 = System.DateTime.Now;
			TimeSpan ts = dt2.Subtract(dt1);
			MessageBox.Show ( "已完成 耗时"+ts.TotalSeconds + "秒 ");
		}
		private void CheckCorrectFin(GlobalConfig cfg)
		{
			string msg = "";
			StringBuilder sb = new StringBuilder();
			foreach (Stock s in cfg.Stocks.stocks) {
				if (cfg.QFI.QFI[s.ID] != null) {
					QQfinItem qf = cfg.QFI.QFI[s.ID];
					StringBuilder st = new StringBuilder();
					DateTime dt = qf.CZNL[0].bgrq;
					foreach (CZNL cznl in qf.CZNL) {
						if( dt.Year - cznl.bgrq.Year > 1){
							st.Append( "CZNL: "+dt.ToShortDateString()+"\t"+cznl.bgrq.ToShortDateString()+"\t");
							break;
						}
						dt = cznl.bgrq;
					}
					
					dt = qf.YYNL[0].bgrq;
					foreach (YYNL yynl in qf.YYNL) {
						if( dt.Year - yynl.bgrq.Year > 1){
							st.Append( "YYNL: "+dt.ToShortDateString()+"\t"+yynl.bgrq.ToShortDateString()+"\t");
							break;
						}
						dt = yynl.bgrq;
					}
					
					dt = qf.YLNL[0].bgrq;
					foreach (YLNL ylnl in qf.YLNL) {
						if( dt.Year - ylnl.bgrq.Year > 1){
							st.Append( "YLNL: "+dt.ToShortDateString()+"\t"+ylnl.bgrq.ToShortDateString()+"\t");
							break;
						}
						dt = ylnl.bgrq;
					}
					
					dt = qf.DJCW[0].bgrq;
					foreach (DJCW djcw in qf.DJCW) {
						if( dt.Year - djcw.bgrq.Year > 1){
							st.Append( "DJCW: "+dt.ToShortDateString()+"\t"+djcw.bgrq.ToShortDateString()+"\t");
							break;
						}
						dt = djcw.bgrq;
					}
					
					if(st.Length>0)
						sb.AppendLine( s.ID+"\t"+s.Code+"\t"+st);
				}else{
					msg += s.ID+",";
				}
			}
			MFile.WriteAllText("JZCSYL.log", sb.ToString());
		}
		private void OutPutNewistDate(GlobalConfig cfg)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Stock s in cfg.Stocks.stocks) {
				if (cfg.QFI.QFI[s.ID] != null) {
					QQfinItem qf = cfg.QFI.QFI[s.ID];
					sb.Append(s.ID + "\t" + s.Code + "\t" + qf.CZNL[0].bgrq.ToShortDateString()+"\t");
					sb.Append( qf.YYNL[0].bgrq.ToShortDateString());
					sb.Append( qf.YLNL[0].bgrq.ToShortDateString());
					sb.Append( qf.DJCW[0].bgrq.ToShortDateString());					
					sb.AppendLine();
				}
			}
			MFile.WriteAllText("JZCSYL.log", sb.ToString());
		}
		private void OutPutJZCSYL(GlobalConfig cfg)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Stock s in cfg.Stocks.stocks) {
				if (cfg.QFI.QFI[s.ID] != null) {
					QQfinItem qf = cfg.QFI.QFI[s.ID];
					sb.Append(s.ID + "\t" + s.Code + "\t");
					foreach (DJCW djcw in qf.DJCW) {
						sb.Append(djcw.bgrq + "\t" + djcw.jzcsyl + "\t");
					}
					sb.AppendLine();
				}
			}
			MFile.WriteAllText("JZCSYL.log", sb.ToString());
		}
        private void ConstructNFI( GlobalConfig cfg )
        {
			CNFITool nft = new CNFITool();
			foreach (Stock s in cfg.Stocks.stocks) {
				if (cfg.QFI.QFI[s.ID] != null) {
					QQfinItem qf = cfg.QFI.QFI[s.ID];
					
					if(nft.NFI[s.ID] == null)
						nft.NFI[s.ID] = new NFinItem();
					int count = 0;
					foreach (CZNL c in qf.CZNL ) { //n.mgsy,n.yylr,n.zysr,n.jlr,n.zzc
						nft.NFI[s.ID].NCZNL.Add(
						  new NCZNL( c.bgrq, c.mgsy, c.jlr,c.zzc,c.yylr,c.zysr)
						);
						if(++count>7) break;
					}
					count=0;
					foreach (DJCW c in qf.DJCW) { //n.mgsy,n.yylr,n.zysr,n.jlr,n.zzc
						nft.NFI[s.ID].NDJCW.Add(
						  new NDJCW( c.bgrq, c.mgsytb,c.jzcsyl,c.zzcsyl)
						);
						if(++count>11) break;
					}
				}
			}
			string str = JsonConvert.SerializeObject(nft);
			MFile.WriteAllText( "NFT.txt",str);

			CNFITool nnft = JsonConvert.DeserializeObject<CNFITool>(str);
			int cccc = nnft.NFI.Length;
			MFile.WriteAllText("NFT1.txt", JsonConvert.SerializeObject(nnft));
		}		
        #endregion
	}
}
