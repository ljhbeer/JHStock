using System.Linq;

namespace Tools
{
    public class ValidTools
    {
        public static bool ValidNumber(string ms)
        {
            if (ms == "") return false;
            if (!"-0123456789".Contains(ms[0])) return false;
            foreach (char c in ms.Substring(1))
                if (!"0123456789".Contains(c)) return false;
            return true;
        }
        public static bool ValidDoubleNumber(string ms)
        {
            if (ms == "") return false;
            if (!".-0123456789".Contains(ms[0])) return false;
            int point = 0;
            if (ms[0] == '.') point++;
            foreach (char c in ms.Substring(1))
            {
                if (!"0123456789".Contains(c)) return false;
                if (c == '.')
                {
                    point++;
                    if (point == 2) return false;
                }
            }
            return true;
        }
        public static bool ValidName(string name)
        {
            if (name == "") return false;
            foreach (char c in name.ToLower())
                if (!"0123456789abcdefghikjlmnopqrstuvwxyz-_".Contains(c))
                    return false;
            if ("0123456789-_".Contains(name[0]))
                return false;
            return true;
        }
        public static bool ValidUrl(string name)
        {
            if (name == "") return false;
            foreach (char c in name.ToLower())
                if (!"0123456789abcdefghikjlmnopqrstuvwxyz-_/:.".Contains(c))
                    return false;
            if ("0123456789-_:.".Contains(name[0]))
                return false;
            return true;
        }
    }
}
