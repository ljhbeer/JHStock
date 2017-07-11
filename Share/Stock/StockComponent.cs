using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tools;

namespace JHStock
{
	public class CompareItem : IComparable
    {
        public CompareItem(KData r, double v)
        {
            this.obj = r;
            this.v = v;
        }
        public int CompareTo (object obj)
        {
            CompareItem ci = (CompareItem) obj;
            if (v > ci.v)
                return 1;
            else if (v < ci.v)
                return -1;
            return 0;
        }
        public object obj;
        public double v;
    }
    public class FinItem
    {       
        public struct Fin
        {
            public int Date;
            //public int JLR;
            //public int SR;
            public double MGJZC;
            public double JZCTBROE;
            //public double JZCROE;
            //public double MGSY;
            //public double JLRGRATE;
            //public double SRGRATE;
            //public double ZCFZB;
            //public double MGZBGJJ;
            //public double MGWFPLR;
            //public double MGCASH;
            public Fin(int date, double mgjzc, double jzctbroe)
            {
                // TODO: Complete member initialization
                this.Date = date;
                this.MGJZC = mgjzc;
                this.JZCTBROE = jzctbroe;
            }
        }
        public List<Fin> _fin = new List<Fin>();
        public void Add(int date, double mgjzc, double jzctbroe)
        {
            _fin.Add(new Fin(date, mgjzc, jzctbroe));
        }
        public string OutFin(int years, int firstyear)
        {
            string str = _fin[0].MGJZC + "\t";
            int startpos = -1;
            for (int i = 0; i < _fin.Count; i++)
            {
                if (_fin[i].Date / 10000 == firstyear)
                {
                    startpos = i;
                    break;
                }
            }
            if (startpos == -1)
            {
                for (int i = 0; i < years; i++)
                {
                    str += "-\t";
                }
            }
            else
            {
                for (int i = startpos; i < years + startpos; i++)
                {
                    if (_fin.Count > i)
                        str += _fin[i].JZCTBROE + "\t";
                    else
                        str += "-\t";
                }
            }
            return str + "\r\n";
        }
        public List<string> OutFinL(int years, int firstyear)
        {
            List<string> str = new List<string>();
            str.Add((int)(_fin[0].MGJZC*100)/100.0 + "");
            int startpos = -1;
            for (int i = 0; i < _fin.Count; i++)
            {
                if (_fin[i].Date / 10000 == firstyear)
                {
                    startpos = i;
                    break;
                }
            }
            if (startpos == -1)
            {
                for (int i = 0; i < years; i++)
                {
                    str.Add("");
                }
            }
            else
            {
                for (int i = startpos; i < years + startpos; i++)
                {
                    if (_fin.Count > i)
                        str.Add((int)(_fin[i].JZCTBROE*100)/100.0 + "");
                    else
                        str.Add("");
                }
            }
            return str;
        }
        public string OutNewFin( )
        {
            if (_fin.Count == 0)
                return "\t-\t-\t-\r\n";              
            return _fin[0].Date+ "\t"+_fin[0].MGJZC +"\t"+_fin[0].JZCTBROE + "\r\n";
        }

    }
    public class FQItem
    {        
        public struct Bouns
        {
            public int date;
            public double SG;
            public double PX;
            public bool bSG;
            public bool bPX;
            public Bouns(int date, double SG, double PX, bool bSG, bool bPX)
            {
                this.date = date;
                this.SG = SG;
                this.PX = PX;
                this.bSG = bSG;
                this.bPX = bPX;
            }
        }
        public void SetStartDate(int begindate)
        {
            activesg = 1.0;
            activebsg = false;
            //activepx = 0.0;
            //activebpx = false;
            activepos = -1;
            for (int i = 0; i < _bouns.Count; i++)
            {
                if (begindate < _bouns[i].date && _bouns[i].bSG)
                {
                    activebdate = begindate;
                    activeedate = _bouns[i].date;
                    activepos = i;
                    break;
                }
            }
            if (activepos < 0)
            {
                activeedate = 30001230;
                activepos = 0;
            }
        }
        public int GetFQSGValue(int price, int date)
        {
            if (price==0)
                return price;
            if (date >= activeedate)
            {
                activebsg = true;
                activesg = activesg * (1 + _bouns[activepos].SG);
                int pos = activepos;
                for (; pos < _bouns.Count; pos++)
                {
                    if (date < _bouns[pos].date && _bouns[pos].bSG)
                    {
                        activeedate = _bouns[pos].date;
                        activepos = pos;
                        break;
                    }
                }
                //未找到
                if (pos == _bouns.Count)
                {
                    activeedate = 30001230;
                }
            }
            if (activebsg)
                price =(int)(price * activesg);
            return price;
        }
        public void Add(int date, double SG, double PX)
        {
            bool bSG = false, bPX = false;
            if (SG > 0.0001)
                bSG = true;
            if (PX > 0.0001)
                bPX = true;

            _bouns.Add(new Bouns(date, SG /10, PX / 10, bSG, bPX));
        }
        public List<Bouns> _bouns = new List<Bouns>();
        private int activepos;
        private int activeedate;
        private int activebdate;
        private double activesg;
        private bool activebsg;
        //private double activepx;
        //private bool activebpx;
        
        public List<DQX> GetFQT(int beginDate, int endDate)
        {
            List<DQX> dqx = new List<DQX>();
            double  Q = 1.0;
            double  X = 0;
            foreach (Bouns b in _bouns)
            {
                if (b.date >= beginDate && b.date <= endDate)
                {
                    if (b.bPX)
                        X = Q * b.PX;
                    if (b.bSG)
                        Q *= 1 + b.SG;
                    dqx.Add(new DQX(b.date, Q, X));
                }
            }
            //for (int i = 0; i < _bouns.Count; i++)
            //{
            //    Bouns b = _bouns[i];
            //    if (b.date >= beginDate && b.date <= endDate)
            //    {
            //        if (b.bPX)
            //            X = Q * b.PX;
            //        if (b.bSG)
            //            Q *= 1 + b.SG;
            //        dqx.Add(new DQX(b.date, Q, X));
            //    }
            //}
            return dqx;
        }
        public bool ExistFqDate(int date)
        {
            foreach (Bouns b in _bouns)
                if (b.date == date)
                    return true;
            return false;
        }
        public bool ExistFqDate(int dateb, int datel)
        {
            foreach (Bouns b in _bouns)
                if (b.date>= datel && b.date <=dateb)
                    return true;
            return false;
        }    	
		public bool ExistduplicateDate()
		{
			int count = 
				_bouns.Distinct( new Compare<Bouns>( (r1,r2)=> r1.date==r2.date)).Count();
			if(count == _bouns.Count)
				return false;
			return true;
		}
    }
    public class CFQTool
    {
        public CFQTool(Db.ConnDb db)
        {
            this.db = db;
        }
        public FQItem[] FQ
        {
            get
            {
                if (fq == null)
                {
                    LoadDataBouns();
                }
                return fq;
            }
        }
        private void LoadDataBouns()
        {
            db.TestConnect();
            string sql = "select stockid,(SG+ZZ) as SG,PX + 0 as PX,CQDate from bouns order by stockid, CQDate desc";
            DataSet ds = db.query(sql);
            DataTable dt = ds.Tables[0];
            fq = new FQItem[2000];
            foreach (DataRow dr in dt.Rows)
            {
                int stockid = (int)dr["stockid"];
                if (fq[stockid] == null)
                    fq[stockid] = new FQItem();
                fq[stockid].Add((int)dr["CQDate"], (double)dr["SG"], (double)dr["PX"]);
            }
        }
        public static string DisTinctDataBouns(Stocks stocks, ref int cnt)
        {
            List<Stock> updatestock = new List<Stock>();
            foreach (Stock s in stocks.stocks)
            {
                if (s.ExistDuplicateFQDate())
                {
                    updatestock.Add(s);
                }
            }
            List<string> dupli = new List<string>();
            string sql = @"select id,stockid,CQdate
							from bouns
							where stockid = [stockid] and CQDate in (
							  select CQDate from bouns as A
							  where stockid = [stockid]
							  group by CQDate
							  having Count(CQDate)>1
							  order by CQDate
							) order by ID desc";

            foreach (Stock s in updatestock)
            {
                DataTable dt = stocks.Gcfg.db.query(sql.Replace("[stockid]", s.ID.ToString())).Tables[0];
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    if ((int)dt.Rows[i]["CQDate"] == (int)dt.Rows[i + 1]["CQDate"])
                        dupli.Add(dt.Rows[i]["id"].ToString());
                }
            }
            // 删除重复
            string ids = "";
            cnt = dupli.Count;
            if (dupli.Count > 0)
            {
                ids = dupli.Aggregate((r1, r2) => r1 + "," + r2);
                MessageBox.Show("存在重复id" + ids + " 即将被删除");
                sql = "delete from bouns where id in (" + ids + ")";
                stocks.Gcfg.db.update(sql);
            }
            return ids;
        }
        private FQItem[] fq;                              
        private Db.ConnDb db;
        #region unusing
        public string OutFin(int index, int years,int firstyear)
        {
            if (fin == null)
                LoadDataFin(db);
            if (fin[index] == null)
                return "-\t-\t-\t-\t-\t-\t\r\n";
            return fin[index].OutFin(years,firstyear);
        }
        public List<string> OutFinL(int index, int years,int firstyear)
        {
            if (fin == null)
                LoadDataFin(db);
            if (fin[index] == null)
                return new List<string>(){"0","0","0","0"};
            return fin[index].OutFinL(years,firstyear);
        }
        public string OutNewFin(int index)
        {
            if (newfin == null)
                LoadDataNewFin(db);
            if (newfin[index] == null)
                return "-\t-\t-\t\r\n";
            return newfin[index].OutNewFin();
        }
        private void LoadDataFin(Db.ConnDb db)
        {
            db.TestConnect();
            string sql = "select * from Fin  where ( Fdate mod 10000 ) = 1231 order by id  ";//where FDate%10000 = 1231
            DataSet ds = db.query(sql);
            DataTable dt = ds.Tables[0];
            fin = new FinItem[2000];
            foreach (DataRow dr in dt.Rows)
            {
                int stockid = (int)dr["stockid"];
                if (fin[stockid] == null)
                    fin[stockid] = new FinItem();
                fin[stockid].Add((int)dr["FDate"], (float)dr["MGJZC"], (float)dr["JZCTBROE"]);
            }
        }
        private void LoadDataNewFin(Db.ConnDb db)
        {
            db.TestConnect();
            string sql = "select * from newFin";//where FDate%10000 = 1231
            DataSet ds = db.query(sql);
            DataTable dt = ds.Tables[0];

            newfin = new FinItem[2000];
            foreach (DataRow dr in dt.Rows)
            {
                int stockid = (int)dr["stockid"];
                if (newfin[stockid] == null)
                    newfin[stockid] = new FinItem();
                newfin[stockid].Add((int)dr["FDate"], (float)dr["MGJZC"], (float)dr["JZCTBROE"]);
            }
        }     
        private FinItem[] fin;
        private FinItem[] newfin;
        #endregion
    }
    public class FQStock
    {
        public FQStock(Stock s)
        {
            this.s = s;
            B = E = -2;
        }
        public Stock s;
        public double FQ(KData r) //对close进行处理
        {
            GlobalConfig _gcfg = s.Gcfg;
            int beginDate = _gcfg.Staticsconfig.BeginDay;
            int endDate = _gcfg.Staticsconfig.EndDay;
            CFQTool FQT = _gcfg.FQT;
            if (!(B == beginDate && E == endDate))
            {
                B = beginDate; E = endDate;
                //提取ActiveFQ
                this.activeFq = new List<DQX>();
                if (FQT != null && FQT.FQ[s.ID] != null)
                {
                    this.activeFq = FQT.FQ[s.ID].GetFQT(beginDate, endDate);
                    if (activeFq.Count > 0)
                    {
                        activeFq.Reverse();
                        activeFq.Add(new DQX(beginDate, 1, 0));
                    }
                }
            }
            if (activeFq.Count == 0) // ||activeFq == null 
                return r.close;
            foreach (DQX d in activeFq)
                if (r.date >= d.D)
                    return r.close * d.Q + d.X * 100;
            return r.close;
        }
        private int B;
        private int E;
        private List<DQX> activeFq;
    }
    public struct KData
    {
        public int date;
        public int open;
        public int high;
        public int low;
        public int close;
        public float amount;
        public int vol;
        public int reservation;
    };
    public class CKData
    {
        public static KData[] GetKData(byte[] buffer)
        {
            int size = (int)(buffer.Length / StructSize);
            KData[] kdata = new KData[size];
            unsafe
            {
                IntPtr bytePtr = Marshal.AllocHGlobal(StructSize);
                for (int i = 0; i < kdata.Length; i++)
                {
                    Marshal.Copy(buffer, i * StructSize, bytePtr, StructSize);
                    kdata[i] = *((KData*)bytePtr);
                }
                Marshal.FreeHGlobal(bytePtr);
            }
            return kdata;
        }
        public static KData[] GetKData(string ifname)
        {
            if (File.Exists(ifname))
            {
                return GetKData(File.ReadAllBytes(ifname));
            }
            return new KData[0];
        }
        public static KData[] GetKData(string ifname, int bpos, int epos)
        {
            if (File.Exists(ifname))
            {
                byte[] buffer = File.ReadAllBytes(ifname);
                int size = (int)(buffer.Length / StructSize);
                KData[] kdata = new KData[2];
                unsafe
                {
                    IntPtr bytePtr = Marshal.AllocHGlobal(StructSize);                   
                    Marshal.Copy(buffer, bpos * StructSize, bytePtr, StructSize);
                    kdata[0] = *((KData*)bytePtr);               
                    Marshal.Copy(buffer, epos * StructSize, bytePtr, StructSize);
                    kdata[1] = *((KData*)bytePtr);                   
                    Marshal.FreeHGlobal(bytePtr);
                }
                return kdata;
            }
            return new KData[2];
        }
        public static KData[] GetKData(string ifname, int bpos, int length, bool blast)
        {
             if (File.Exists(ifname))
            {
                byte[] buffer = File.ReadAllBytes(ifname);
                int size = (int)(buffer.Length / StructSize);
                if (bpos == -1)
                    bpos = size - 1;
                KData[] kdata = new KData[length];
                unsafe
                {
                    IntPtr bytePtr = Marshal.AllocHGlobal(StructSize);
                    for (int i = 0; i < length; i++)
                    {
                        if (blast)
                            Marshal.Copy(buffer, (bpos - i) * StructSize, bytePtr, StructSize);
                        else
                            Marshal.Copy(buffer, (bpos - length + i) * StructSize, bytePtr, StructSize);
                        kdata[i] = *((KData*)bytePtr);
                    }
                    Marshal.FreeHGlobal(bytePtr);
                }
                if (blast)
                    return kdata.Reverse().ToArray();
                return kdata;
            }
            return new KData[0];
           
        }
        private static KData[] GetKData(List<int> days, byte[] buffer)
        {
            int size = (int)(buffer.Length / StructSize);
            KData[] kdata = new KData[days.Count];
            unsafe
            {
                int sum = 0;
                IntPtr bytePtr = Marshal.AllocHGlobal(StructSize);
                for (int i = 0; i < size; i++)
                {
                    Marshal.Copy(buffer, i * StructSize, bytePtr, StructSize);
                    KData* k = ((KData*)bytePtr);
                    if (k->date == days[sum]) // 判断0  ???????未判断
                    {
                        kdata[sum] = *k;
                        sum++;
                    }
                    if (sum == days.Count || k->date > days[days.Count - 1])
                        break;
                }
                Marshal.FreeHGlobal(bytePtr);
            }
            return kdata;
        }
        private static KData GetKData(int date, byte[] buffer)
        {
            int size = (int)(buffer.Length / StructSize);
            KData kdata = new KData();
            unsafe
            {
                IntPtr bytePtr = Marshal.AllocHGlobal(StructSize);
                for (int i = size - 1; i > 0; i--)
                {
                    Marshal.Copy(buffer, i * StructSize, bytePtr, StructSize);
                    KData* k = ((KData*)bytePtr);
                    if (k->date == date) // 判断0  ???????未判断
                    {
                        kdata = *k;
                        return kdata;
                    }
                    else if (k->date < date)
                    {
                        break;
                    }
                }
                Marshal.FreeHGlobal(bytePtr);
            }
            return kdata;
        }
        private static KData GetKData(int date, string ifname)
        {
            if (File.Exists(ifname))
            {
                return GetKData(date, File.ReadAllBytes(ifname));
            }
            return new KData();
        }
        public static KData[] GetKData(List<int> days, string ifname)
        {
            if (File.Exists(ifname))
            {
                return GetKData(days, File.ReadAllBytes(ifname));
            }
            return new KData[0];
        }
        public static KData[] GetKData(int startday, int daycount, string ifname)
        {
            if (File.Exists(ifname))
            {
                byte[] buffer = File.ReadAllBytes(ifname);
                int size = (int)(buffer.Length / StructSize);
                KData[] kdata = new KData[daycount];
                unsafe
                {
                    int sum = 0;
                    IntPtr bytePtr = Marshal.AllocHGlobal(StructSize);
                    for (int i = size - 1; i >= 0; i--)
                    {
                        Marshal.Copy(buffer, i * StructSize, bytePtr, StructSize);
                        KData* k = ((KData*)bytePtr);
                        if (k->date <= startday) // 判断0  ???????未判断
                        {
                            kdata[sum] = *k;
                            sum++;
                        }
                        if (sum == daycount)
                            break;
                    }
                    Marshal.FreeHGlobal(bytePtr);
                }
                return kdata;
            }
            return new KData[0];
        }
        public static List<string> KDataFileToListSql(string file, string tablename, string stockid)
        {
            string inserttemplate = "insert into " + tablename + "(stockid,[date],[open],[close],high,low,vol,amount) values  ([stockid],'[-date-]',[-open-],[-close-],[high],[low],[vol],[amount])";
            inserttemplate = inserttemplate.Replace("[stockid]", stockid);
            KData[] kdata = GetKData(file);
            List<string> items = new List<string>();
            for (int i = 0; i < kdata.Length; i++)
            {
                string strdate = kdata[i].date.ToString();
                string date = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6);
                string sql = inserttemplate.Replace("[-open-]", kdata[i].open.ToString())
                    .Replace("[-close-]", kdata[i].close.ToString())
                    .Replace("[high]", kdata[i].high.ToString())
                    .Replace("[low]", kdata[i].low.ToString())
                    .Replace("[vol]", kdata[i].vol.ToString())
                    .Replace("[amount]", kdata[i].amount.ToString())
                    .Replace("[-date-]", date);
                //string  str += kdata[i].date + "\t" + kdata[i].open / 100.0 + "\t" + kdata[i].close / 100.0 + "\t" + kdata[i].high / 100.0 + "\t" + kdata[i].low / 100.0 + "\t" + kdata[i].vol + "\t" + kdata[i].amount + "\r\n";
                //db.update(sql);
                items.Add(sql);
            }
            return items;
        }
        public static List<int> GetKDataDate(string file)
        {
            KData[] kdata = GetKData(file);
            List<int> items = new List<int>();
            for (int i = 0; i < kdata.Length; i++)
            {
                items.Add(kdata[i].date);
            }
            return items;
        }
        public static string KDataToString(KData[] kdata)
        {
            string str = "日期\t开盘\t收盘\t最高\t最低\t成交量\t成交金额\r\n";
            for (int i = 0; i < kdata.Length; i++)
            {
                str += kdata[i].date + "\t" + kdata[i].open / 100.0 + "\t" + kdata[i].close / 100.0 + "\t" + kdata[i].high / 100.0 + "\t" + kdata[i].low / 100.0 + "\t" + kdata[i].vol + "\t" + kdata[i].amount + "\r\n";
            }
            return str;
        }
        public static byte[] IntArryToByteArry(int[] intary)
        {
            MemoryStream int32mem = new MemoryStream();
            BinaryWriter int32bytewr = new BinaryWriter(int32mem);
            foreach (int i in intary)
                int32bytewr.Write(i);
            byte[] b = int32mem.ToArray();
            int32bytewr.Dispose();
            int32mem.Dispose();
            return int32mem.ToArray();
        }
        public static byte[] IntArryToByteArry(int[,] intary)
        {
            MemoryStream int32mem = new MemoryStream();
            BinaryWriter int32bytewr = new BinaryWriter(int32mem);
            foreach (int i in intary)
                int32bytewr.Write(i);
            byte[] b =int32mem.ToArray();
            int32bytewr.Dispose();
            int32mem.Dispose();
            return b;        
        }
        public static byte[] FloatArryToByteArry(float[,] floatary)
        {
            MemoryStream float32mem = new MemoryStream();
            BinaryWriter float32bytewr = new BinaryWriter(float32mem);
            foreach (float i in floatary)
                float32bytewr.Write(i);
            byte[] b = float32mem.ToArray();
            float32bytewr.Dispose();
            float32mem.Dispose();
            return b;
		}

        public static int[] GetKDataClose(byte[] buffer)
        {
            int size = (int)(buffer.Length / sizeof(int));
            int[] kdata = new int[size];
            unsafe
            {
                IntPtr bytePtr = Marshal.AllocHGlobal(sizeof(int));
                for (int i = 0; i < kdata.Length; i++)
                {
                    Marshal.Copy(buffer, i * sizeof(int), bytePtr, sizeof(int));
                    kdata[i] = *((int*)bytePtr);
                }
                Marshal.FreeHGlobal(bytePtr);
            }
            return kdata;
        }
        public static int[] GetKDataClose(string ifname)
        {
            if (File.Exists(ifname))
            {
                return GetKDataClose(File.ReadAllBytes(ifname));
            }
            return new int[0];
        }
        public static int GetKDataClose(int date, string file) //k.close = 0;
        {
            KData k = GetKData(date, file);
            return k.close;
        }
        public static int[] GetKDataClose(List<int> dayposs, string file)
        {
            int[] r = new int[dayposs.Count];
            //KData[] kdata = GetKData(days, file);
            int[] allclose = GetKDataClose(file);
            for (int i = 0; i < dayposs.Count; i++)
            {
                if (dayposs[i] < allclose.Length)
                    r[i] = allclose[dayposs[i]];
                else
                    r[i] = 0;
            }
            return r;
        }
        public static int FindFirstItem(int[] items, int dstitem, string express, int startpos = 0)
        {
            if (express == "=")
            {
                for (int i = startpos; i < items.Length; i++)
                    if (items[i] == dstitem)
                        return i;
                return -1;
            }
            else if (express == "!=")
            {
                for (int i = startpos; i < items.Length; i++)
                    if (items[i] != dstitem)
                        return i;
                return -1;
            }
            else if (express == ">")
            {
                for (int i = startpos; i < items.Length; i++)
                    if (items[i] > dstitem)
                        return i;
                return -1;
            }
            else if (express == "<")
            {
                for (int i = startpos; i < items.Length; i++)
                    if (items[i] < dstitem)
                        return i;
                return -1;
            }
            return -2;
        }

        public static void KDataFileToTxtFile(string ofname, string ifname)
        {
            string str = KDataToString(GetKData(ifname));
            MFile.WriteAllText(ofname, str);
        }
        public static void KDataFileToDB(Db.ConnDb db, string file, string tablename, string stockid)
        {
            List<string> items = KDataFileToListSql(file, tablename, stockid);
            InsertMultipleSQL(items, db.AccessConnStr);
        }
        public static void SeekListDate(List<int> listdate, ref int idate, KData k)
        {
            for (int i = idate; i < listdate.Count; i++)
            {
                if (listdate[i] == k.date)
                {
                    idate = i;
                    return;
                }
            }

            for (int i = idate; i > 0; i--)
            {
                if (listdate[i] == k.date)
                {
                    idate = i;
                    return;
                }
            }
            return;
        }
        public static void KDataFileToFormatFile(string path, string file, List<int> listdate, string id)
        {
            int[] closearry = new int[listdate.Count];
            KData[] kdata = GetKData(file);
            int idate = 0;
            foreach (KData k in kdata)
            {
                SeekListDate(listdate, ref idate, k);
                closearry[idate] = k.close;
                idate++;
            }
            MFile.WriteAllBytes(path + id + ".day", IntArryToByteArry(closearry));
        }
        public static void InsertMultipleSQL(List<string> items, string AccessConnStr)
        {

            using (OleDbConnection conn = new OleDbConnection(AccessConnStr))
            {
                OleDbCommand cmd = conn.CreateCommand();
                OleDbTransaction trans = null;
                try
                {
                    conn.Open();
                    trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    foreach (string item in items)
                    {
                        cmd.CommandText = item;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    if (items.Count > 0)
                        MFile.AppendAllText("Trans.log", items[0] + "\t" + ex.Message + "\r\n");
                }
            }
        }
        
        private static int StructSize = System.Runtime.InteropServices.Marshal.SizeOf(new KData());
        public static int[] GetDateIndex(KData[] ks, List<int> listdate)
        {
            int[] index = new int[listdate.Count];
            for (int i = 0; i < index.Length; i++)
                index[i] = -1;
            if (ks.Length > 0)
            {
                int start = 0;
                SeekListDate(listdate,ref start, ks[0]);
                if (start == -1) return index;
                index[0] = start;
                int j = 0;
                for (int i = start; i < index.Length && j<ks.Length; i++)
                {
                    if (listdate[i] == ks[j].date)
                    {
                        index[i] = j;
                        j++;
                    }
                    else if (listdate[i] < ks[j].date)
                    {
                        index[i] = -j;
                    }
                    else
                    {
                        index[i] = j;
                        //Console.Write( "index["+i+"]="+listdate[i]+"  ks["+j+"].data="+ks[j].date);
                        i--;
                        j++;
                    }
                }
            }
            return index;
        }        
    }
}
