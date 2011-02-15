using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class LinDef
    {
        private int key;
        private int[] funIds;

        public LinDef(int _key, int[] _funIds)
        {
            key = _key;
            funIds = _funIds;
        }
    }
}
