using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHStock
{
	public class MACDSET
    {
        public MACDSET(int SHORT = 12, int LONG = 26, int MID = 9)
        {
            this.SHORT = SHORT;
            this.LONG = LONG;
            this.MID = MID;
            this.ds = 2.0 / (1 + SHORT);
            this.dl = 2.0 / (1 + LONG);
            this.dm = 2.0 / (1 + MID);
            this.dmvs = 1 - ds;
            this.dmvl = 1 - dl;
            this.dmvm = 1 - dm;
        }
        private int SHORT;
        private int LONG;
        private int MID;
        public double ds;
        public double dl;
        public double dm;
        public double dmvs;
        public double dmvl;
        public double dmvm;
    }
    public class MACD
    {
        double EMAS;
        double EMAL;
        public double DIF;
        double DEA;
        float DATE;
        public static MACDSET macdset = new MACDSET();
        public MACD()
        {
            EMAL = EMAS = DIF = DEA = DATE = 0;
        }
        public MACD Next(int Close)
        {
            MACD mv = new MACD();
            mv.EMAL = EMAL * macdset.dmvl + Close * 0.01 * macdset.dl;
            mv.EMAS = EMAS * macdset.dmvs + Close * 0.01 * macdset.ds;
            mv.DIF = mv.EMAS - mv.EMAL;
            mv.DEA = DEA * macdset.dmvm + DIF * macdset.dm;
            return mv;
        }
        public MACD Next(int Close,int Date)
        {
            MACD mv = new MACD();
            mv.EMAL = EMAL * macdset.dmvl + Close * 0.01 * macdset.dl;
            mv.EMAS = EMAS * macdset.dmvs + Close * 0.01 * macdset.ds;
            mv.DIF = mv.EMAS - mv.EMAL;
            mv.DEA = DEA * macdset.dmvm + DIF * macdset.dm;
            mv.DATE = Date;
            return mv;
        }
        public MACD Next2(int Close1, int Close2)
        {
            this.EMAL = Close1 * 0.01 * macdset.dmvl + Close2 * 0.01 * macdset.dl;
            this.EMAS = Close1 * 0.01 * macdset.dmvs + Close2 * 0.01 * macdset.ds;
            this.DIF = this.EMAS - this.EMAL;
            this.DEA = this.DIF * macdset.dm;

            return this;
        }
        public static MACD[] ComputeMacdArray(int[] close)
        {
            MACD[] mv = new MACD[close.Length];
            for (int i = 1; i < mv.Length; i++)
                mv[i] = mv[i - 1].Next(close[i]);
            return mv;
        }
        public static MACD[] ComputeMacdArray(KData[] kdata)
        {
            if (kdata.Length < 2) return new MACD[0];
            MACD[] mv = new MACD[kdata.Length];
            for (int i = 0; i < mv.Length; i++)
                mv[i] = new MACD();
            mv[1] = mv[1].Next2(kdata[0].close, kdata[1].close);
            for (int i = 2; i < mv.Length; i++)
                mv[i] = mv[i - 1].Next(kdata[i].close);
            return mv;
        }
        public override string ToString()
        {
            return "\t" + Math.Round(EMAL, 2) + "\t" + Math.Round(EMAS, 2) +
                    "\t" + Math.Round(DIF, 2) + "\t" + Math.Round(DEA, 2) +
                    "\t" + Math.Round(2 * (DIF - DEA), 2);
        }

        public int Date()
        {
            return (int)DATE;
        }
        internal void FillMacdFloatData(float[,] filldata, int Id, int pos)
        {
            //filldata[Id, pos] = 0;
            filldata[Id, pos + 1] = (float)EMAS;
            filldata[Id, pos + 2] = (float)EMAL;
            filldata[Id, pos + 3] = (float)DIF;
            filldata[Id, pos + 4] = (float)DEA;
        }
        internal void SetData(float[,] filldata, int Id, int pos)
        {
            DATE = filldata[Id, pos + 0];
            EMAS = filldata[Id, pos + 1];
            EMAL = filldata[Id, pos + 2];
            DIF = filldata[Id, pos + 3];
            DEA = filldata[Id, pos + 4];
        }
        public double  Bar
        {
            get
            {
                return 2 * (DIF - DEA);
            }
        }

        internal void SetDate(int date)
        {
            DATE = date;
        }

        internal MACD[] ComputeMacdArray_DateIndex(KData[] kd,List<int> kdindex)
        {
            if (kd.Length != kdindex.Count) return null;
            MACD[] mv = ComputeMacdArray(kd);
            return ComputeMacdArray_DateIndex(mv, kdindex);
        }

        internal MACD[] ComputeMacdArray_DateIndex(MACD[] mv, List<int> kdindex)
        {
            if (mv.Length != kdindex.Count) return null;
            int kdi = 0;
            mv.Select(r => { r.SetDate(kdindex[kdi]); kdi++; return 0; }).ToList();
            return mv;
        }
    }
}
