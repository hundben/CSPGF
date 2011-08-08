
namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public class Category
    {
        public int oldCat, l, j, k;
        public Category(int _oldCat, int _l, int _j, int _k) 
        {
            this.oldCat = _oldCat;
            this.l = _l;
            this.j = _j;
            this.k = _k;
        }
        public /*override*/ bool Equals(Category c)
        {
            if (c.oldCat == this.oldCat && c.l == this.l && c.j == this.j && c.k == this.k) return true;
            return false;
        }
        public override string ToString()
        {
            return "[o:" + this.oldCat + "  l:" + this.l + " j:" + this.j + " k:" + this.k + "]";
        }
    }
}
