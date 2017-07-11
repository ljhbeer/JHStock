using System.Collections.Generic;

namespace Tools
{
	public class StringTools
	{
		public static string GetEqualValue(string src, string begin, string end)
	    {
	        if (!src.Contains(begin))
	            return "";
	        src = src.Substring(src.IndexOf(begin) + begin.Length) + end;
	        src = src.Substring(0, src.IndexOf(end)).Trim();
	        return src;
	    }
	    public static string GetLastEqualValue(string src, string begin, string end)
	    {
	        if (!src.Contains(end))
	            return "";
	        src = src.Substring(0,src.IndexOf(end));
	        int pos = src.LastIndexOf(begin);
	        if (pos == -1)
	        {
	            return "";
	        }
	        src = src.Substring(pos + begin.Length).Trim();
	        return src;
	    }
	    public static string GetEqualValue(string src, string begin, string end, ref int pos)
	    {//包含begin
	        pos = src.IndexOf(begin, pos);
	        if (pos == -1)
	        {
	            pos = -1;
	            return "";
	        }
	        int pos2 = src.IndexOf(end, pos + begin.Length);
	        if (pos2 == -1)
	        {
	            pos = -1;
	            return "";
	        }
	        src = src.Substring(pos, pos2 - pos);
	        pos = pos2 + end.Length;
	        return src;
	    }
	    public static string GetEqualValue2(string src, string begin, string end, ref int pos)
	    {
	        pos = src.IndexOf(begin, pos);
	        if (pos == -1)
	        {
	            pos = -1;
	            return "";
	        }
	        int pos2 = src.IndexOf(end, pos + begin.Length);
	        if (pos2 == -1)
	        {
	            pos = -1;
	            return "";
	        }
	        src = src.Substring(pos + begin.Length, pos2 - pos - begin.Length);
	        pos = pos2 + end.Length;
	        return src;
	    }
	    public static string GetEqualValueMulti(string item, string strbegin, string strend)
	    {
	        int pos = 0;
	        string src = "";
	        while (true)
	        {
	            string tempsrc = GetEqualValue2(item, strbegin, strend, ref pos);
	            if (pos < 0) break;
	            src += tempsrc;
	        }
	        return src;
	    }
	    public static string GetEqualValueMulti(string item, string strbegin, string strend, string mmrt)
	    {
	        int pos = 0;
	        string src = "";
	        while (true)
	        {
	            string tempsrc = GetEqualValue2(item, strbegin, strend, ref pos);
	            if (pos < 0) break;
	            src += mmrt.Replace("[mm]", tempsrc);
	        }
	        return src;
	    }
	    public static List<string> GetEqualValueList(string item, string strbegin, string strend)
	    {
	        int pos = 0;
	        List<string> src = new List<string>();
	        try
	        {
	            while (true)
	            {
	                string tempsrc = GetEqualValue2(item, strbegin, strend, ref pos);
	                if (pos < 0) break;
	                src.Add(tempsrc);
	            }
	        }
	        catch
	        {
	            pos = 0;
	            while (true)
	            {
	                string tempsrc = GetEqualValue2(item, strbegin, strend, ref pos);
	                if (pos < 0) break;
	                src.Add(tempsrc);
	            }
	        }
	        return src;
	    }
	    public static string CutStr(string str, int len)
	    {
	        if (str == null || str.Length == 0 || len <= 0)
	        {
	            return string.Empty;
	        }
	
	        int l = str.Length;
	
	        #region 计算长度
	        int clen = 0;
	        while (clen < len && clen < l)
	        {
	            //每遇到一个中文，则将目标长度减一。
	            if ((int)str[clen] > 128) { len--; }
	            clen++;
	        }
	        #endregion
	
	        if (clen < l)
	        {
	            return str.Substring(0, clen) + "...";
	        }
	        else
	        {
	            return str;
	        }
	    }
	}
}
