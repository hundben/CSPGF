using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.linearizer
{
    class LinTriple
    {
        private int fId;
        private CncType cncType;
        private List<List<BracketedTokn>> linTable;

        public LinTriple(int _fId, CncType _cncType, List<List<BracketedTokn>> _linTable)
        {
            fId = _fId;
            cncType = _cncType;
            linTable = _linTable;
        }

        public int GetFId()
        {
            return fId;
        }
        public CncType GetCncType()
        {
            return cncType;
        }
        public List<List<BracketedTokn>> GetLinTable()
        {
            return linTable;
        }
        public override String ToString()
        {
            String rez = "id : " + fId + " cncType : (" + cncType.ToString() + ") bracketedToken :[" + linTable.ToString() + "]";
            return rez;
        }
    }
}
