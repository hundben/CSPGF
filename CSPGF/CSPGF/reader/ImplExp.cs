using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ImplExp : Expr
    {
        private Expr arg;

        public ImplExp(Expr _arg)
        {
            arg = _arg;
        }

        public String toString()
        {
            return "Implicit Arguments Expression : [ Argument : " + arg.toString() + "]";
        }

        public Expr getExp()
        {
            return arg;
        }
    }

}