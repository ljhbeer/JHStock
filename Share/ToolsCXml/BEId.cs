using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;

namespace ToolsCXml
{
    public class BEId
    {
        public BEId()
        {
            B = E = activeindex = -1;
        }
        public void MoveNext()
        {
            activeindex++;
        }
        public void MoveToTop()
        {
            activeindex = 0;
        }
        public bool OK()
        {
            return B > 0 && E > 0 && B < E;
        }
        public bool HasNext
        {
            get
            {
                return activeindex >= 0 && activeindex < E - B;
            }
        }
        public int B { get; set; }
        public int E { get; set; }
        public int ActiveIndex { get { return activeindex + B; } }
        private int activeindex;
    }
}
