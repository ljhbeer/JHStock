using System;

namespace Tools
{
	public class TimeStringTools{
		public static  string NowDate()
	    {
	        return DateTime.Now.ToShortDateString().Replace("/","");
	    }
	    public static  string NowDateMin()
	    {
	        return DateTime.Now.ToShortDateString().Replace("/","") +"_"+ DateTime.Now.ToShortTimeString().Replace(":","");
	    }
	    public static string NowDateMinSec()
	    {
	        return DateTime.Now.ToShortDateString().Replace("/","") + "_" + DateTime.Now.ToLongTimeString().Replace(":", "");
	    }
		public static string UpdateNumber()
		{
			int un = DateTime.Now.Year*10000+DateTime.Now.Month*100+DateTime.Now.Day;
			return un.ToString();
		}
	}
}
