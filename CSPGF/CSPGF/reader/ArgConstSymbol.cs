using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ArgConstSymbol : Symbol
    {
        public int arg { get; private set; }
        public int cons { get; private set; }

        public ArgConstSymbol(int _arg, int _cons)
        {
            arg = _arg;
            cons = _cons;
        }
        
        public String ToString()
        {
            return "Argument : " + arg + " Constituent : " + cons;
        }
    }
}
