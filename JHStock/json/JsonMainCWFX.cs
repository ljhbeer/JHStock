
using System.Collections.Generic;
using System.Reflection;
public class JsonMainCWFX
{
    /// <summary>
    /// 
    /// </summary>
    public string date { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string jbmgsy { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string kfmgsy { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string xsmgsy { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string mgjzc { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string mggjj { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string mgwfply { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string mgjyxjl { get; set; }
    /// <summary>
    /// 39.9亿
    /// </summary>
    public string yyzsr { get; set; }
    /// <summary>
    /// 4.12亿
    /// </summary>
    public string mlr { get; set; }
    /// <summary>
    /// 7200万
    /// </summary>
    public string gsjlr { get; set; }
    /// <summary>
    /// 5981万
    /// </summary>
    public string kfjlr { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string yyzsrtbzz { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string gsjlrtbzz { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string kfjlrtbzz { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string yyzsrgdhbzz { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string gsjlrgdhbzz { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string kfjlrgdhbzz { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string jqjzcsyl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string tbjzcsyl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string tbzzcsyl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string mll { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string jll { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string sjsl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string yskyysr { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string xsxjlyysr { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string jyxjlyysr { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string zzczzy { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string yszkzzts { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string chzzts { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string zcfzl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ldzczfz { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ldbl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string sdbl { get; set; }
    public static List<string> GetNameList()
    {
        List<string> ls = new List<string>();
        System.Type t =  (new JsonMainCWFX()).GetType();
        foreach (PropertyInfo pi in t.GetProperties())
        {
            //object value1 = pi.GetValue(tc, null));//用pi.GetValue获得值
            ls.Add(pi.Name);//获得属性的名字,后面就可以根据名字判断来进行些自己想要的操作
            //获得属性的类型,进行判断然后进行以后的操作,例如判断获得的属性是整数
            //if(value1.GetType() == typeof(int))
            //{
            //    //进行你想要的操作
            //}
        }
        return ls;
    }
}
public class ChineseName
{
    public ChineseName()
    {
        initstring();
    }
    public string GetChineseName(string key)
    {
        if (_chinesedic.ContainsKey(key))
        {
            return _chinesedic[key];
        }
        return "未知";
    }
    public string GetProperName(string chinesename)
    {
        if (_properdic.ContainsKey(chinesename))
            return _properdic[chinesename];
        return "";
    }
    private void initstring()
    {
        List<string> arr = new List<string>(){"date","日期",
"jbmgsy","基本每股收益(元)",
"kfmgsy","扣非每股收益(元)",
"xsmgsy","稀释每股收益(元)",
"mgjzc","每股净资产(元)",
"mggjj","每股公积金(元)",
"mgwfply","每股未分配利润(元)",
"mgjyxjl","每股经营现金流(元)",
"cznlzb","成长能力指标",
"yyzsr","营业总收入(元)",
"mlr","毛利润(元)",
"gsjlr","归属净利润(元)",
"kfjlr","扣非净利润(元)",
"yyzsrtbzz","营业总收入同比增长(%)",
"gsjlrtbzz","归属净利润同比增长(%)",
"kfjlrtbzz","扣非净利润同比增长(%)",
"yyzsrgdhbzz","营业总收入滚动环比增长(%)",
"gsjlrgdhbzz","归属净利润滚动环比增长(%)",
"kfjlrgdhbzz","扣非净利润滚动环比增长(%)",
"ylnlzb","盈利能力指标",
"jqjzcsyl","加权净资产收益率(%)",
"tbjzcsyl","摊薄净资产收益率(%)",
"tbzzcsyl","摊薄总资产收益率(%)",
"mll","毛利率(%)",
"jll","净利率(%)",
"sjsl","实际税率(%)",
"ylzlzb","盈利质量指标",
"yskyysr","预收款/营业收入",
"xsxjlyysr","销售现金流/营业收入",
"jyxjlyysr","经营现金流/营业收入",
"yynlzb","运营能力指标",
"zzczzy","总资产周转率(次)",
"yszkzzts","应收账款周转天数(天)",
"chzzts","存货周转天数(天)",
"cwfxzb","财务风险指标",
"zcfzl","资产负债率(%)",
"ldzczfz","流动负债/总负债(%)",
"ldbl","流动比率",
"sdbl","速动比率",
        };
        _chinesedic = new Dictionary<string, string>();
        _properdic = new Dictionary<string, string>();
        for (int i = 0; i < arr.Count; i += 2)
        {
            _chinesedic[arr[i]] = arr[i+1];
            _properdic[arr[i + 1]] = arr[i];
        }
    }
    private Dictionary<string, string> _chinesedic;
    private Dictionary<string, string> _properdic;

}