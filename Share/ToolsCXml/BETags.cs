using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;

namespace ToolsCXml
{
    public class BETags
    {
        public BETags(List<BETag> rbts)
        {
            this.tags = rbts;
        }
        public BETags(string Rule)
        {
            tags = new List<BETag>();
            BEPos bp;
            if (Rule.StartsWith("{"))
            {
                bp = BETag.FormatCmd(Rule, '{', '}');
                if (bp.Valid())
                {
                    string cmdstr = bp.String;/////////////////////////////////
                    this.Cmd = new Cmd(cmdstr);
                    Rule = Rule.Substring(bp.E + 1);
                }
            }
            //Compute Rule
            bp = BETag.FormatCmd(Rule, '[', ']', 0);
            while (bp.Valid())
            {
                BETag bt = new BETag(bp.String);
                if (bt.OK)
                    tags.Add(bt);
                bp = BETag.FormatCmd(Rule, '[', ']', bp.E + 1);
            }
            //Compute Cmd
        }
        public void Add(BETag bt)
        {
            tags.Add(bt);
        }
        public BETags SubTags(int b, int length = -1)
        {
            List<BETag> rbts = new List<BETag>();
            if (b >= 0)
                for (int i = b, len = 0; i < tags.Count && len != length; i++, len++)
                    rbts.Add(tags[i]);
            return new BETags(rbts);
        }
        public List<BETag> tags { get; set; }
        public Cmd Cmd { get; set; }
        public string Match(string txt)
        {
            BEPos bp = null;
            if (tags.Count > 0)
                bp = tags[0].BEPos(txt);
            for (int i = 1; i < tags.Count; i++)
                bp = tags[i].BEPos(bp);
            if (bp == null)
                return "";
            return bp.String;
        }
    }
}
