using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Newtonsoft.Json;
using Tools;

namespace JHStock.Update  //更新 未做
{
	[JsonObject(MemberSerialization.OptOut)]
	public class CNFITool{
		public CNFITool(string path=""){
			this.path = path;
			_nfi = null; // new NFinItem[2000]
		}
		public void LoadNFI( ){ // 大约 2s 时间
			if(File.Exists(path)){
				string str = File.ReadAllText(path);
				CNFITool nnft = JsonConvert.DeserializeObject<CNFITool>(str);
				if(nnft.NFI!=null)
					_nfi = nnft.NFI;
			}
		}
		public NFinItem[] NFI{
			get {
				if(_nfi==null){
					LoadNFI();
				}
				return _nfi;
			}
			set {_nfi = value;}
		}
		private NFinItem[] _nfi;
		 [JsonIgnore]
		private string path;
	}
}
