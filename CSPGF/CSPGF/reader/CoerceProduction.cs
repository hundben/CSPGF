using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class CoerceProduction : Production
    {
        public int initId { get; private set; }

        public CoerceProduction(int _fId, int _initId)
            : base(1, _fId)
        {
            initId = _initId;
        }

        public override int[] Domain()
        {
            int[] tmp = new int[1];
            tmp[0] = initId;
            return tmp;
        }

        public int[] GetDomain()
        {
            return new int[] { initId };
        }

        public override String ToString()
        {
            return "Coercion(" + this.fId + " -> " + initId + ")";
        }

        public override Boolean Equals(Object o)
        {
            if (o is CoerceProduction) {
                return ((CoerceProduction)o).initId == initId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
