using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AppPattern : Pattern
    {
        // Should be able to get?
        String name;
        Pattern[] patts;

        public AppPattern(String _name, Pattern[] _patts)
        {
            name = _name;
            patts = _patts;
        }

        public String ToString()
        {
            String ss = "Application pattern [ Name : " + name + " , Patterns : (";
            foreach (Pattern p in patts) {
                ss += " " + p.ToString();
            }
            ss += ")]";
            return ss;
        }
    }
}