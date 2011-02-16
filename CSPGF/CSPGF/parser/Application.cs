using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    public class Application : Tree
    {
        public String fun;
        public List<Tree> args;
        public Application(String fun, List<Tree> args)
        {
            this.fun = fun;
            this.args = args;
        }
    }
}

