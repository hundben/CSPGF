
namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public class Category
    {
        private int OldCat, l, j, k;
        public Category(int oldCat, int l, int j, int k) 
        {
            this.OldCat = oldCat;
            this.l = l;
            this.j = j;
            this.k = k;
        }

        public override bool Equals(Category c)
        {
            if (c.OldCat == this.OldCat && c.l == this.l && c.j == this.j && c.k == this.k) 
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return "[o:" + this.OldCat + "  l:" + this.l + " j:" + this.j + " k:" + this.k + "]";
        }
    }
}