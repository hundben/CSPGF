using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class WeightedIdent
    {
        public Double weight { get; private set; }
        public String ident { get; private set; }

        public WeightedIdent(String _ident, Double _weight)
        {
            ident = _ident;
            weight = _weight;
        }

        //CHECK LATER
        // Did not exist in the java-code
        public String ToString()
        {
            return "Ident: " + ident + " Weight: " + weight;
        }
    }
}
