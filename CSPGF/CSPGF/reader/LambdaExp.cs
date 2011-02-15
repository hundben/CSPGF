using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class LambdaExp : Expr
    {
        private Boolean bType;
        private String vName;
        private Expr body;

        public LambdaExp(Boolean _bType, String _vName, Expr _body)
        {
            bType = _bType;
            vName = _vName;
            body = _body;
        }

        public String toString()
        {
            return "Lambda Expression : [Bound Type : " + bType + " , Name : " + vName + " , Body : " + body.toString() + "]";
        }

        public Boolean getType()
        {
            return bType;
        }
        public String getVarName()
        {
            return vName;
        }
        public Expr getBody()
        {
            return body;
        }


    }
}