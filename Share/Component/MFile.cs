/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2016-8-9
 * 时间: 11:12
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using JHStock;

namespace Tools
{
	/// <summary>
	/// Description of MFile.
	/// </summary>
	public class MFile
	{
		public MFile()
		{
			
		}	
		public static void ClearEmptyPath(string path){
			DirectoryInfo di = new DirectoryInfo(path);
			foreach( DirectoryInfo d in di.GetDirectories()){
				if( d.GetDirectories().Length == 0 && d.GetFiles().Length==0)
					d.Delete();
			}
		}
		public static void WriteAllBytes(string path, byte[] bytes)
		{
		 	checkPath(ref path);
		 	File.WriteAllBytes(path,bytes);
		}	
		public static void WriteAllText(string path, string contents)
		{
		 	checkPath(ref path);
			File.WriteAllText(path,contents);
		}		
		public static void AppendAllText(string path, string contents)
		{
		 	checkPath(ref path);
			File.AppendAllText(path,contents);
		}
		private static void checkPath(ref string path)
		{
			if(cfg!=null && !path.Contains(":"))
			{
				path = cfg.NowWorkPath()+path;
			}
		}	
		
		public static string GetFileNameExt(string p)
        {
            if (p.Contains("."))
            {
                return p.Substring(p.LastIndexOf("."));
            }
            return ".None";
        }       
		public static BaseConfig cfg = null;
	}
}
