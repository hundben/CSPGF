using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ImplExp : Expr
    {
        public Expr exp { get; private set; }

        public ImplExp(Expr _arg)
        {
            exp = _arg;
        }

        public String ToString()
        {
            return "Implicit Arguments Expression : [ Argument : " + exp.ToString() + "]";
        }
    }

}