using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Sequence
    {
        Symbol[] symbs;

        public Sequence(Symbol[] _symbs)
        {
            symbs = _symbs;
        }

        public Symbol symbol(int index)
        {
            return symbs[index];
        }

        public int length()
        {
            return symbs.Length;
        }

        public Symbol[] getSymbols()
        {
            return symbs;
        }

        public String toString()
        {
            String ss = "Symbols : [";
            for (int i = 0 ; i < symbs.Length ; i++)
            {
                ss += (" " + symbs[i].toString());
            }
            ss += "]";
            return ss;
        }
    }
}
