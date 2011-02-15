using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ToksSymbol : Symbol
    {
        String[] toks;

        public ToksSymbol(String[] _toks)
        {
            toks = _toks;
        }

        public String[] getTokens()
        {
            return toks;
        }

        public void setTokens(String[] _toks)
        {
            toks = _toks;
        }

        public Boolean isTerminal()
        {
            return true;
        }

        public String toString()
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
