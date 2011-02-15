using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.linearizer
{
    class RezDesc
    {
        private int fid;
        private List<CncType> cncTypes;
        private List<List<List<BracketedTokn>>> bss;

        public RezDesc(int _fid, List<CncType> _cncTypes, List<List<List<BracketedTokn>>> _bss)
        {
            fid = _fid;
            cncTypes = _cncTypes;
            bss = _bss;
        }

        public int getFid()
        {
            return fid;
        }
        public List<CncType> getCncTypes()
        {
            return cncTypes;
        }
        public List<List<List<BracketedTokn>>> getBracketedTokens()
        {
            return bss;
        }
    }
}
