using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class VarExp : Expr
    {
        public int ind { get; private set; }

        public VarExp(int _ind)
        {
            ind = _ind;
        }

        public String ToString()
        {
            return "Variable Expression : [Index : " + ind + "]";
        }
    }
}