using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace JHStock
{
    public class StockNowBase
    {
        public StockNowBase(GlobalConfig cfg)
        {
            this.Gcfg = cfg;
            this.db = cfg.db;
            _stocks =cfg.Stocks;
            _drnowbase = null;
        }
        public DataRow[] DRnowBase
        {
            get
            {
                if (_drnowbase == null)
                    LoadNowBase();
                return _drnowbase;
            }
        }
       
        private void LoadNowBase()
        {
            if (_drnowbase == null)
            {
                string sql = "select StockHexin.*,StockTopTen.TopTenRate,StockTopTen.TopTenLiuRate from "
                            + "StockHexin,StockTopTen  where StockHexin.id=StockTopTen.id order by StockHexin.ID";
                DataSet ds = db.query(sql);
                _drnowbase = new DataRow[2000];                
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    _drnowbase[(int)dr["ID"]] = dr;
                }
            }
        }
        private DataRow[] _drnowbase;
        private GlobalConfig Gcfg;
        private Db.ConnDb db;
        private Stocks _stocks;
    }
}
