using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Alternative
    {
        // tokens = alt1, prefix = alt2
        public String[] alt1 { get; private set; }
        public String[] alt2 { get; private set; }

        public Alternative(String[] _alt1, String[] _alt2)
        {
            alt1 = _alt1;
            alt2 = _alt2;
        }

        public String ToString()
        {
            String sb = "";
            foreach (String t in alt1) {
                sb += t + " ";
            }
            sb += "/ ";
            foreach (String t in alt2) {
                sb += t + " ";
            }
            return sb;
        }
    }
}