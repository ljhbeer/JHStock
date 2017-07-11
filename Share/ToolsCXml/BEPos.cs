using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;

namespace ToolsCXml
{
    public class BEPos
    {
        public BEPos()
        {
            str = null;
            B = -1;
            E = -1;
        }
        public BEPos(int b, int e, string str)
        {
            this.str = str;
            B = b;
            E = e;
        }
        public bool Valid()
        {
            return B > -1 && str != null && (E > -1 && B < E || E == -1);
        }
        public string String
        {
            get
            {
                if (!Valid()) return "";
                return str.Substring(B, E == -1 ? -1 : E - B);
            }
        }
        public int B { get; set; }
        public int E { get; set; }
        public string InnerStr { get { return str; } }
        private string str;
    }
}
