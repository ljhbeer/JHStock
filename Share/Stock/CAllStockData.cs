using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Tools;

namespace JHStock
{
	public class CAllStockData
	{
		public CAllStockData()
		{
			alldata = null;
			allfqdata = null;
		}
		public static void CreateAllStockData(GlobalConfig gcfg)
		{
			BaseConfig cfg = gcfg.Baseconfig;
			int szcnt = gcfg.Stocks.ListDate.Count;
			if (File.Exists(cfg.IndexPath()))
			{
				long cnt = new FileInfo(cfg.IndexPath()).Length /( 2048*sizeof(int));
				if (cnt == szcnt)
				{
					gcfg.ErrMsg = "index文件已经同步，无需再次更新";
					return;
				}
			}
			CreateIndexData(gcfg);
		}

		// [y,x] means [date,index]  [0,index] == -1 means none stock
	 	private static void CreateIndexData(GlobalConfig gcfg)
		{
	 		BaseConfig cfg = gcfg.Baseconfig;
			List<int> listdate = gcfg.Stocks.ListDate;
			int[][] alldata = new int[2048][];
			foreach (Stock s in gcfg.Stocks.stocks) {
				if (!s.ExistKDfile()) {
					System.Windows.Forms.MessageBox.Show("Error:" + s.Name + s.Code);
					return;
				}
				try {
					KData[] ks = s.GetKData();
					alldata[s.ID] = CKData.GetDateIndex(ks, listdate);
				} catch (Exception ee) {
					MFile.AppendAllText("Exception1.log", ee.Message + " " + s.ID + " \r\n");
				}
			}
			int[,] savedaddata = new int[listdate.Count, 2048];
			for (int k = 1; k < alldata.Length; k++) {
				if (alldata[k] == null)
					savedaddata[0, k] = -1; else if (alldata[k].Length != listdate.Count)
					savedaddata[0, k] = -2;
			}
			for (int i = 0; i < listdate.Count; i++) {
				savedaddata[i, 0] = listdate[i];
				for (int k = 1; k < alldata.Length; k++) {
					if (alldata[k] == null || alldata[k].Length != listdate.Count)
						continue;
					savedaddata[i, k] = alldata[k][i];
				}
			}
			//for (int i = 0; i < listdate.Count; i++)
			//    Console.WriteLine( "savedata["+i+",0]="+savedaddata[i,0]);
			MFile.WriteAllBytes(cfg.IndexPath(), CKData.IntArryToByteArry(savedaddata));
		}
		public void LoadAllData(string allstockfile = "", int StructSize = 2048*4)
		{
			if (allstockfile == "")
				allstockfile = "E:\\Backup\\STOCK\\" + "allstock.day";
			if (File.Exists(allstockfile))
			{
				byte[] buffer =  File.ReadAllBytes(allstockfile);
				int intsize = sizeof(int);
				int itemintsize = StructSize / intsize;
				int itemcount = (int)(buffer.Length / StructSize);
				alldata = new int[itemcount,itemintsize];
				unsafe
				{
					IntPtr bytePtr = Marshal.AllocHGlobal(intsize);
					for (int i = 0; i < alldata.Length; i++)
					{
						Marshal.Copy(buffer, i * intsize, bytePtr, intsize);
						alldata[i/itemintsize,i%itemintsize] = *((int*)bytePtr);
					}
					Marshal.FreeHGlobal(bytePtr);
				}
			}
		}
		public void LoadAllFQData(string allstockfile, int StructSize=2048*4)
		{
			if (File.Exists(allstockfile))
			{
				byte[] buffer = File.ReadAllBytes(allstockfile);
				int intsize = sizeof(int);
				int itemintsize = StructSize / intsize;
				int itemcount = (int)(buffer.Length / StructSize);
				allfqdata = new int[itemcount, itemintsize];
				unsafe
				{
					IntPtr bytePtr = Marshal.AllocHGlobal(intsize);
					for (int i = 0; i < allfqdata.Length; i++)
					{
						Marshal.Copy(buffer, i * intsize, bytePtr, intsize);
						allfqdata[i / itemintsize, i % itemintsize] = *((int*)bytePtr);
					}
					Marshal.FreeHGlobal(bytePtr);
				}
			}
		}
		public void ClosePriceToTxtFile(string ofname, int stockpos)
		{
			string str = ClosePriceToString(stockpos,alldata);
			MFile.WriteAllText(ofname, str);
		}
		public void CloseFQPriceToTxtFile(string ofname, int stockpos)
		{
			string str = ClosePriceToString(stockpos,allfqdata);
			MFile.WriteAllText(ofname, str);
		}
		public string ClosePriceToString(int stockpos, int[,] alldata)
		{
			if (alldata == null)
				return "";
			string str = "日期\t收盘\r\n";
			int startdate = GetStartDatePos(stockpos,alldata);
			for (int i = startdate; i <alldata.GetLength(0); i++)
			{
				str += alldata[i, 0] + "\t" + alldata[i, stockpos] / 100.0 + "\r\n";
			}
			return str;
		}
		public string GetMaxMin(int index, int bpos, int epos)
		{
			int maxpos = 0;
			int minpos = 0;
			int min = 10000000;
			int max = 0;
			for (int k = bpos; k <= epos; k++)
			{
				if (allfqdata[k,index] > max)
				{
					maxpos = k;
					max  = allfqdata[k, index];
				}
				if (allfqdata[k,index]!=0  &&allfqdata[k,index] < min  )
				{
					minpos = k;
					min  = allfqdata[k, index];
				}
			}
			if (maxpos == 0 || maxpos == 0)
			{
				return "-\t-\t-\t-\t-\t\r\n";
			}
			return allfqdata[maxpos,0] + "\t" + max + "\t" +allfqdata[minpos,0] + "\t" + min + "\t" + max*1.0/min + "\r\n";
		}
		public string GetTwoDays(int index, int bpos, int epos)
		{
			int b = alldata[bpos, index];
			int e = alldata[epos, index];
			if (b == 0)
				b = RemoveZeroPrice(index, bpos );
			if (e == 0)
				e = RemoveZeroPrice(index, epos);
			//int maxpos = 0;
			//int minpos = 0;
			//int min = 10000000;
			//int max = 0;
			//for (int k = bpos; k <= epos; k++)
			//{
			//    if (allfqdata[k, index] > max)
			//    {
			//        maxpos = k;
			//        max = allfqdata[k, index];
			//    }
			//    if (allfqdata[k, index] != 0 && allfqdata[k, index] < min)
			//    {
			//        minpos = k;
			//        min = allfqdata[k, index];
			//    }
			//}

			if (b == 0)
			{
				return "-\t-\t-\t\r\n";
			}
			return b/100.0 + "\t" + e/100.0 + "\t" + (e * 100.0 / b -100)+ "\r\n";
			
		}
		private int RemoveZeroPrice(int index, int pos)
		{
			for (int i = pos; i > pos - 200 && i > 0; i--)
			{
				if (alldata[i, index] != 0)
				{
					return alldata[i, index];
				}
			}
			return 0;
		}
		public int GetStartDatePos(int stockpos, int[,] alldata)
		{
			int startdate = 0;
			for (int i = 0; i < alldata.GetLength(0); i++)
			{
				if (alldata[i, stockpos] > 0)
				{
					startdate = i;
					break;
				}
			}
			return startdate;
		}
		public List<int> GetDataDate()
		{
			List<int> items = new List<int>();
			for (int i = 0; i <alldata.GetLength(0); i++)
			{
				items.Add(alldata[i,0]);
			}
			return items;
		}
		public List<int> GetLastRecord()
		{
			if (alldata.GetLength(0) == 0)
				return new List<int>();
			int index = alldata.GetLength(0) - 1;
			List<int> r = new List<int>();
			for (int i = 0; i < alldata.GetLength(1); i++)
				r.Add(alldata[index, i]);
			return r;
		}
		
		public int GetLastDate()
		{
			return alldata[GetLastDatePos(), 0];
		}
		public int GetLastDatePos()
		{
			return alldata.GetLength(0) - 1;
		}
		public int GetStartDatePos(int day)
		{
			for (int i = 0; i < alldata.GetLength(0); i++)
			{
				if (alldata[i, 0] >= day)
					return i;
			}
			return -1;
		}
		public int[,] alldata { get; set; }
		public int[,] allfqdata{ get; set; }
	}
}
