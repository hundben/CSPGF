using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class WeightedIdent
    {
        private Double weight;
        private String ident;

        public WeightedIdent(String _ident, Double _weight)
        {
            ident = _ident;
            weight = _weight;
        }

        public String getIdent()
        {
            return ident;
        }

        public double getWeight()
        {
            return weight;
        }

        //CHECK LATER
        // Did not exist in the java-code
        public String toString()
        {
            return "Ident: " + ident + " Weight: " + weight;
        }
    }
}
