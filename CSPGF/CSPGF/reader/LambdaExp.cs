using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class LambdaExp : Expr
    {
        public Boolean bType { get; private set; }
        public String vName { get; private set; }
        public Expr body { get; private set; }

        public LambdaExp(Boolean _bType, String _vName, Expr _body)
        {
            bType = _bType;
            vName = _vName;
            body = _body;
        }

        public override String ToString()
        {
            return "Lambda Expression : [Bound Type : " + bType + " , Name : " + vName + " , Body : " + body.ToString() + "]";
        }
    }
}