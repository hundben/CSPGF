using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class CoerceProduction : Production
    {
        private int initId;

        public CoerceProduction(int _fId, int _initId) : base(1, _fId)
        {
            // Should not be needed anymore
            //base.fId = _fId;
            //base.sel = 1;
            initId = _initId;
        }

        public int getInitId()
        {
            return initId;
        }

        public int[] getDomain()
        {
            return new int[] { this.initId };
        }

        public String toString()
        {
            return "Coercion(" + this.fId + " -> " + initId + ")";
        }

        public Boolean equals(Object o)
        {
            if (o is CoerceProduction)
            {
                return ((CoerceProduction)o).initId == initId;
            }
            return false;
        }

    }
}
