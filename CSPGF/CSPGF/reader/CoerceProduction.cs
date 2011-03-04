using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class CoerceProduction : Production
    {
        private int initId;

        public CoerceProduction(int _fId, int _initId)
            : base(1, _fId)
        {
            initId = _initId;
        }

        public int GetInitId()
        {
            return initId;
        }

        public int[] GetDomain()
        {
            return new int[] { this.initId };
        }

        public String ToString()
        {
            return "Coercion(" + this.fId + " -> " + initId + ")";
        }

        public Boolean Equals(Object o)
        {
            if (o is CoerceProduction) {
                return ((CoerceProduction)o).initId == initId;
            }
            return false;
        }

    }
}
