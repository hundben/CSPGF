using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser_new
{
    class PK
    {
        public int fid { get; private set; }
        public int lindex { get; private set; }
        public int _int { get; private set; }
        PK(int _fid, int _lindex, int __int)
        {
            fid = _fid;
            lindex = _lindex;
            _int = __int;
        }
    }
    class PassiveChart
    {
        public Dictionary<PK, int> pc { get; private set; }
        PassiveChart(Dictionary<PK, int> _pc)
        {
            pc = _pc;
        }
    }
}
