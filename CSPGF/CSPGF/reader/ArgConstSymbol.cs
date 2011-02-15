using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ArgConstSymbol : Symbol
    {
        private int arg;
        private int cons;

        public ArgConstSymbol(int _arg, int _cons)
        {
            arg = _arg;
            cons = _cons;
        }

        public int getArg()
        {
            return arg;
        }
        public int getCons()
        {
            return cons;
        }

        public String toString()
        {
            return "Argument : " + arg + " Constituent : " + cons;
        }
    }

}
