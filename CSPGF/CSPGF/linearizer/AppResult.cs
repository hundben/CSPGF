using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;

namespace CSPGF.linearizer
{
    class AppResult
    {
        private CncFun cncFun;
        private CncType cncType;
        private List<CncType> cncTypes;

        public AppResult(CncFun _cncFun, CncType _cncType, List<CncType> _cncTypes)
        {
            cncFun = _cncFun;
            cncType = _cncType;
            cncTypes = _cncTypes;
        }
        public CncFun GetCncFun()
        {
            return cncFun;
        }
        public CncType GetCncType()
        {
            return cncType;
        }
        public List<CncType> GetCncTypes()
        {
            return cncTypes;
        }
    }
}
