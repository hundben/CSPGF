using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.linearizer
{
    class CncType
    {
        private String cId;
        private int fId;

        public CncType(String _cId, int _fId)
        {
            cId = _cId;
            fId = _fId;
        }
        public String GetCId()
        {
            return cId;
        }
        public int GetFId()
        {
            return fId;
        }
        public String ToString()
        {
            return "name : " + cId + " , fId : " + fId;
        }

    }
}
