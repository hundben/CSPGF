namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public class Category
    {
        private int oldCat, l, j, k;

        public Category(int oldCat, int l, int j, int k) 
        {
            this.oldCat = oldCat;
            this.l = l;
            this.j = j;
            this.k = k;
        }

        public override bool Equals(Category c)
        {
            if (c.oldCat == this.oldCat && c.l == this.l && c.j == this.j && c.k == this.k) 
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return "[o:" + this.oldCat + "  l:" + this.l + " j:" + this.j + " k:" + this.k + "]";
        }
    }
}