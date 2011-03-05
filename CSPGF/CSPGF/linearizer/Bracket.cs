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

        public String GetCId()
        {
            return cId;
        }
        public int GetLIndex()
        {
            return lIndex;
        }
        public int GetFId()
        {
            return fId;
        }
        public List<BracketedTokn> GetBracketedToks()
        {
            return bss;
        }
        public String ToString()
        {
            String rez = "name : " + cId + ", linIndex : " + lIndex + ", fId : " + fId + ", bracketed tokens : " + bss.ToString();
            //for(int i=0;i<bss.length;i++)
            //	 rez+=(" "+bss[i].toString());
            return rez;
        }
    }
}
