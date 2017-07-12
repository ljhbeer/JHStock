using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using JHStock.Update;
using Tools;

namespace JHStock
{
    public class Stock
    {
        public Stock(int id, string code, string name, GlobalConfig jscfg)
        {
            this.ID = id;
            this.Code = code;
            this.Name = name;
            this.NumCode = code.Substring(2);
            this.PYCode = PYTool.GetChineseSpell(name);
            this._gcfg = jscfg;
            this.Bmp = null;
            //this.InDicator = null;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string NumCode { get; set; }
        public string PYCode { get; set; }
        //public void InDicator { get; set; } //InDicator 指针
        public Bitmap Bmp { get; set; }
        public GlobalConfig Gcfg { get { return _gcfg; } }
        public bool ExistKDfile()
        {
            return File.Exists(KDataFileName());
        }
        public KData[] GetKData()
        {
            string file = KDataFileName();
            if (File.Exists(file))
            {
                return CKData.GetKData(file);
            }
            return new KData[0];
        }
        public KData[] GetKData(int bpos, int epos)
        {
            string file = KDataFileName();
            if (File.Exists(file))
            {
                return CKData.GetKData(file, bpos, epos);
            }
            return new KData[0];
        }
        public KData[] GetKData(int bpos, int length, bool blast)  //逆序
        {
            string file = KDataFileName();
            if (File.Exists(file))
            {
                return CKData.GetKData(file, bpos, length, blast);
            }
            return new KData[0];
        }        

        private string KDataFileName()
        {
            BaseConfig cfg = _gcfg.Baseconfig;
            string file = "";
            if (Code.StartsWith("SH"))
                file = cfg.shpath + "/" + Code + ".day";
            else if (Code.StartsWith("SZ"))
                file = cfg.szpath + "/" + Code + ".day";
            return file;
        }
        private GlobalConfig _gcfg;
    }
    public class DQX
    {
        public DQX(int D, double Q, double x)
        {
            this.D = D;
            this.Q = Q;
            this.X = x;
        }
        public int D { get; set; }
        public double Q { get; set; }
        public double X { get; set; }
    }
}
