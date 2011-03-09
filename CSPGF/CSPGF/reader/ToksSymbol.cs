using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ToksSymbol : Symbol
    {
        public String[] toks { get; private set; }

        public ToksSymbol(String[] _toks)
        {
            toks = _toks;
        }

        public String ToString()
        {
            String s = "Tokens : ";
            foreach (String st in toks)
            {
                s += " " + st;
            }
            //for (int i = 0 ; i < toks.Length ; i++)
            //    s += (" " + toks[i]);
            return s;
        }
    }
}
