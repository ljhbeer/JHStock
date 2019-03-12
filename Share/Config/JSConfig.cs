using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using JHStock.Update;
using Tools;

namespace JHStock
{	
	[JsonObject(MemberSerialization.OptOut)]
    public class JSConfig
    {   
        public JSConfig()
        {
            KdataType = "dayly";
        	baseconfig = new BaseConfig();
        	updatexmlpathconfig = new UpdateXmlPathConfig();
        	outshowconfig = new OutShowConfig();
        	conditionconfig = new ConditionConfig();
        	globalconfig = new GlobalConfig(baseconfig);
        	staticsconfig = new StaticsConfig();
        	filename = "";
        }
        public int FirstYear()
        {
            return 2013;
        }   

        public void Load(string filename){
        	this.filename = filename;
        	string str = File.ReadAllText(filename);
        	JSConfig jc = JsonConvert.DeserializeObject<JSConfig>(str);
        	this.baseconfig  = jc.baseconfig;
        	this.updatexmlpathconfig = jc.updatexmlpathconfig;
        	this.outshowconfig = jc.outshowconfig;
        	this.globalconfig = jc.globalconfig;
        	this.conditionconfig = jc.conditionconfig;
        	this.staticsconfig = jc.staticsconfig;
        }
        public void Save(string tfilename = "")
        {
        	string str = JsonFormatTool.ConvertJsonString( JsonConvert.SerializeObject(this) );
            if(tfilename == "")
                File.WriteAllText(filename, str);
            else            	
                File.WriteAllText(tfilename, str);
        }
        public BaseConfig baseconfig {get;set;}
        public UpdateXmlPathConfig updatexmlpathconfig  {get;set;}
        public OutShowConfig outshowconfig {get;set;}
        public ConditionConfig conditionconfig {get;set;}
        public StaticsConfig staticsconfig {get;set;}
        public List<string> Memostr { get; set; }
        [JsonIgnore]
        public GlobalConfig globalconfig {get;set;}
        [JsonIgnore]
        private string filename;
        [JsonIgnore]
        public string KdataType { get; set; }  //dayly,weekly, monthly
    }    
}
