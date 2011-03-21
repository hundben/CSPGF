using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Sequence
    {
        public Symbol[] symbs { get; private set; }

        public Sequence(Symbol[] _symbs)
        {
            symbs = _symbs;
        }

        public Symbol GetSymbol(int index)
        {
            return symbs[index];
        }

        public int GetLength()
        {
            return symbs.Length;
        }

        public override String ToString()
        {
            String ss = "Symbols : [";
            foreach (Symbol s in symbs)
            {
                ss += " " + s.ToString();
            }
            ss += "]";
            return ss;
        }
    }
}
