using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser_new
{
    class PArg
    {
        public List<Tuple<int, int>> list { get; private set; }
        public int fid { get; private set; }
        PArg(List<Tuple<int, int>> _list, int _fid)
        {
            list = _list;
            fid = _fid;
        }
    }
}
