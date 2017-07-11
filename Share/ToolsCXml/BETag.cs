using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;

namespace ToolsCXml
{
    public class BETag
    {
        public BETag(string s)
        {
            OK = false;
            if (s == null)
                return;
            if (s.StartsWith("{"))
            {
                BEPos bp = BETag.FormatCmd(s, '{', '}');
                if (bp.Valid())
                {
                    Cmd = new Cmd( bp.String);//////////???????   
                    s = s.Substring(bp.E + 1);
                }
            }
            string[] items = s.Split(new string[] { "[", "]", "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 2)
            {
            	Begin = items[0].Replace("#@#@","[").Replace("@#@#","]").Replace("@@", "-");
            	End = items[1].Replace("#@#@","[").Replace("@#@#","]").Replace("@@", "-");
            }
            else if (items.Length == 1)
            {
                Begin = items[0].Replace("#@#@","[").Replace("@#@#","]").Replace("@@", "-");
                End = "";
            }
            else
                return;
            OK = true;
        }
        public BEPos BEPos(string s)
        {
            if (OK)
            {
                int B = s.IndexOf(Begin);
                if (B != -1)
                {
                    B = B + Begin.Length;
                    int E = s.IndexOf(End, B);
                    if (E != -1)
                        return new BEPos(B, E, s);
                }
            }
            return new BEPos(-1, -1, s);
        }
        public BEPos BEPos(BEPos bp)//ReverseMatch
        {
            string s = bp.InnerStr;
            if (!OK || s == null || !bp.Valid())
                return new BEPos(-1, -1, s);
    
            int B = s.IndexOf(Begin, bp.B);
            if (Cmd != null && Cmd.ReverseMatch)
                B = s.LastIndexOf(Begin, bp.B, bp.E - bp.B);
            if (B != -1 && B < bp.E)
            {
                B = B + Begin.Length;
                int E = s.IndexOf(End, B, bp.E - B);
                if (E != -1)
                    return new BEPos(B, E, s);
            }
    
            B = s.LastIndexOf(Begin, bp.E, bp.E - bp.B);
            if (B != -1)
            {
                B = B + Begin.Length;
                int E = s.IndexOf(End, B, bp.E - B);
                if (E != -1)
                    return new BEPos(B, E, s);
            }
            return new BEPos(-1, -1, s);
        }
        public BEPos NextBEPos(BEPos bp) //同一个才能NextBEPos
        {
            string s = bp.InnerStr;
            if (!OK || !bp.Valid() || s == null)
                return new BEPos(-1, -1, s);
            int B = s.IndexOf(Begin, bp.E + End.Length);
            if (B == -1)
                return new BEPos(-1, -1, s);
            B = B + Begin.Length;
            int E = s.IndexOf(End, B);
            if (E == -1)
                return new BEPos(-1, -1, s);
            return new BEPos(B, E, s);
        }
        public static BEPos FormatCmd(string Rule, char begin, char end, int startIndex = 0)
        {
            int B = 0;
            int Pos = Rule.IndexOfAny(new char[] { begin, end }, startIndex);
            if (Pos != -1 && Rule[Pos] == begin)
            {
                B = Pos + 1;
                int stack = 1;
                while (Pos != -1)
                {
                    Pos = Rule.IndexOfAny(new char[] { begin, end }, Pos + 1); //??
                    if (Rule[Pos] == begin)
                        stack++;
                    else if (Rule[Pos] == end)
                        stack--;
                    if (stack == 0)
                        return new BEPos(B, Pos, Rule);
                    if (stack < 0)
                        break;
                }
            }
            return new BEPos(-1, -1, Rule);
        }
        public string Begin { get; set; }
        public string End { get; set; }
        public Cmd Cmd { get; set; }
        public Boolean OK { get; set; }
    }
}
