using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class CncCat
    {
        /**
         * Concrete category are a maping from category names (abstract-categories)
         * to multiple, conjoint, concrete categories.
         * They are represented in the pgf binary by :
         *  - the name of the abstract category (ex: Adj)
         *  - the first concrete categoy (ex : C18)
         *  - the last corresponding concrete category (ex : C21)
         *  - a list of labels (names of fields in the pmcfg tuples)
         * Here we will keep only the indices.
         */
        public String name { get; private set; }
        public int firstFID { get; private set; }
        public int lastFID { get; private set; }
        private String[] labels; //TODO: was commented out, have to check later.

        public CncCat(String _name, int _firstFId, int _lastFId, String[] _labels)
        {
            name = _name;
            firstFID = _firstFId;
            lastFID = _lastFId;
            labels = _labels; // was also commented out.
        }

        public String ToString()
        {
            return name + " [C" + firstFID + " ... C" + lastFID + "]";
        }
    }
}
