using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ToksSymbol : Symbol
    {
        public String[] tokens { get; private set; }

        public ToksSymbol(String[] _toks)
        {
            tokens = _toks;
        }

        public String ToString()
        {
            String s = "Tokens : ";
            foreach (String st in tokens)
            {
                s += " " + st;
            }
            return s;
        }
    }
}
