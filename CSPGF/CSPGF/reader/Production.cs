using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public abstract class Production
    {
        protected int sel;
        protected int fId;

        public Production(int selector, int fId)
        {
            this.sel = selector;
            this.fId = fId;
        }

        public int getCategory()
        {
            return this.fId;
        }

        public int range()
        {
            return this.fId;
        }

        public abstract String toString();

        // Domain is the domain of the concrete function
        public abstract int[] domain();

        public int getSel()
        {
            return sel;
        }
        public int getFId()
        {
            return fId;
        }
    }
}
