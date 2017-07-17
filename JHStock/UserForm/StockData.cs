using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using JHStock.Update;
using System.Threading;

namespace JHStock
{
    public class StocksData
    {
        public StocksData(JSConfig _jscfg)
        {
            this._jscfg = _jscfg; 
            this._stocks = _jscfg.globalconfig.Stocks;
            _ssestock = new Stock(0, "SH000001", "上证指数", _jscfg.globalconfig);
            isrunning = false;
        }
        public bool HasKdata(int sID)
        {
            if (_tagstocksdata != null && _tagstocksdata[sID] != null)
                return true;
            return false;
        }
        public List<KData> GetKD(int sID)
        {
            if (HasKdata(sID))
                return _tagstocksdata[sID].kd;
            return new List<KData>();
        }
        public bool LoadData(JSConfig _jscfg)
        {
            String Msg = "";
            if (!exchangestatus.StatusCheck(_stocks, ref Msg))
            {
                //MessageBox.Show(Msg);
                return false;
            }

            if (File.Exists(_jscfg.baseconfig.WorkPath + "data\\ExpPrice.dat"))
            {
                string txt = File.ReadAllText(_jscfg.baseconfig.WorkPath + "data\\ExpPrice.dat");
                SaveTag sst = JsonConvert.DeserializeObject<SaveTag>(txt);
                this._tagstocksdata = sst.Tag;
                if (sst.now.Date == DateTime.Now.Date)  //for Debug
                {
                }
            }
            else
            {
                return false;
            }
            return true; //应改为 false
        }
        public void DownDataFromNet(ShowDeleGate ThreadShowMsg, CompleteDeleGate ThreadCompleteRun)
        {
            if(!isrunning){
				isrunning = false;
				NetKData nkd = new NetKData(_jscfg, _ssestock);
                nkd.ThreadShowMsg = ThreadShowMsg;
                nkd.CompleteRun = ThreadCompleteRun;
				System.Threading.Thread nonParameterThread = 
					new Thread( new ThreadStart( nkd.GetNetKData ));
				nonParameterThread.Start();
			}
        }

        private ExChangeStatusCheck exchangestatus = new ExChangeStatusCheck();
        private JSConfig _jscfg;
        private Stocks _stocks;
        private Stock _ssestock;
        private tagstock[] _tagstocksdata;
        private  bool isrunning;

    }
}
