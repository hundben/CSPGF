using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AppPattern : Pattern
    {
        String name;
        Pattern[] patts;

        public AppPattern(String _name, Pattern[] _patts)
        {
            name = _name;
            patts = _patts;
        }

        public String toString()
        {
            String ss = "Application pattern [ Name : " + name + " , Patterns : (";
            for (int i = 0 ; i < patts.Length ; i++)
            {
                ss += (" " + patts[i].toString());
            }
            ss += ")]";
            return ss;
        }

    }
}