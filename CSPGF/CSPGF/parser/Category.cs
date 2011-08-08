using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.Parser
{
    class Category
    {
        public int oldCat, l, j, k;
        public Category(int _oldCat, int _l, int _j, int _k) 
        {
            oldCat = _oldCat;
            l = _l;
            j = _j;
            k = _k;
        }
        public /*override*/ bool Equals(Category c)
        {
            if (c.oldCat == oldCat && c.l == l && c.j == j && c.k == k) return true;
            return false;
        }
        public override string ToString()
        {
            return "[o:"+oldCat+"  l:"+l+" j:"+j+" k:"+k+"]";
        }
    }
}
