using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser_new
{
    class Chart
    {
        private int nextCat;
        public Chart(int nextCat)
        {

        }

        //We need a way to handle all the categories we create (COMPLETE rule), also this will check so we don't create duplicates
        //N = category, l = constituent, j = start, k = end
        //returns new category
        public int GetNewCategory(int N, int l, int j, int k) 
        {
            //TODO make this one, one question, should we use the old categories at all here since we just copy them?
            return 0;
        }
    }
}
