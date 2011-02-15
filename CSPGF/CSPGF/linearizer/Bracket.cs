using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.linearizer
{
    class Bracket
    {
        private String cId;
        private int lIndex;
        private int fId;
        private List<BracketedTokn> bss;

        public Bracket(String _cId, int _lIndex, int _fId, List<BracketedTokn> _bss)
        {
            cId = _cId;
            lIndex = _lIndex;
            fId = _fId;
            bss = _bss;
        }

        public String getCId()
        {
            return cId;
        }
        public int getLIndex()
        {
            return lIndex;
        }
        public int getFId()
        {
            return fId;
        }
        public List<BracketedTokn> getBracketedToks()
        {
            return bss;
        }
        public String toString()
        {
            String rez = "name : " + cId + ", linIndex : " + lIndex + ", fId : " + fId + ", bracketed tokens : " + bss.toString();
            //for(int i=0;i<bss.length;i++)
            //	 rez+=(" "+bss[i].toString());
            return rez;
        }
    }
}
