using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    public class Variable : Tree
    {
        public String cid;
        public Variable(String _cid)
        {
            cid = _cid;
        }
    }
}

