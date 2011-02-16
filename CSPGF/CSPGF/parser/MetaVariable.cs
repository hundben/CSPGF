using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class MetaVariable : Tree
    {
        public int id;
        public MetaVariable(int _id)
        {
            id = _id;
        }
    }
}
