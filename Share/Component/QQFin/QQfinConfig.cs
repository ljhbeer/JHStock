/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2016-12-01
 * 时间: 15:39
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace JHStock.Component.QQFinConfig
{
	public class QQfinConfig
	{
		public QQfinConfig()
		{
			string str = File.ReadAllText(@"E:\Project\Source\Stock\Data\QQFinConfig.txt");			
			 fincfg = JsonConvert.DeserializeObject<Dictionary<string,Detail>>(str);
		}
		public Dictionary<string,Detail> fincfg {get;set;}
	}	
	public class Detail
	{ 
	    public string name { get; set; } 
	    public Dictionary<string,List<string>> col {get; set;}
	}
}
