using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class VarExp : Expr
    {
        private int ind;

        public VarExp(int _ind)
        {
            ind = _ind;
        }

        public String toString()
        {
            return "Variable Expression : [Index : " + ind + "]";
        }

        public int getVarInd()
        {
            return ind;
        }
    }
}