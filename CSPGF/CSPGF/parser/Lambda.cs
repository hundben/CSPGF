using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    public class Lambda : Tree
    {
        public List<Tuple<Boolean, String>> vars;   //går detta att göra smidigare kanske?
        public Tree body;
        public Lambda(List<Tuple<Boolean, String>> _vars, Tree _body) {
            vars = _vars;
            body = _body;
        }
    }
}
