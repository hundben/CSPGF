using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.linearizer
{
    class LeafKS : BracketedTokn
    {
        private String[] tokens;

        public LeafKS(String[] _tokens)
        {
            tokens = _tokens;
        }

        public String[] GetStrs()
        {
            return tokens;
        }

        public String ToString()
        {
            String rez = "string names : [";
            for (int i = 0 ; i < tokens.Length ; i++)
            {
                rez += (" " + this.tokens[i]);
            }
            rez += "]";
            return rez;
        }
    }
}
