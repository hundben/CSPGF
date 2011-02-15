using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Alternative
    {
        private String[] tokens;
        private String[] prefix;

        public Alternative(String[] _alt1, String[] _alt2)
        {
            tokens = _alt1;
            prefix = _alt2;
        }

        public String toString()
        {
            String sb = "";
            foreach (String t in tokens)
            {
                sb += t + " ";
            }
            sb += "/ ";
            foreach (String t in prefix)
            {
                sb += t + " ";
            }
            return sb;
        }

        public String[] getAlt1()
        {
            return tokens;
        }

        public String[] getAlt2()
        {
            return prefix;
        }
    }
}