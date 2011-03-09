using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public abstract class Production
    {
        public int sel { get; private set; }
        public int fId { get; private set; }

        public Production(int _sel, int _fId)
        {
            sel = _sel;
            fId = _fId;
        }

        public abstract String ToString();

        // Domain is the domain of the concrete function
        public abstract int[] domain();
    }
}
