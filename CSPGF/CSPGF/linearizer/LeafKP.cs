using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CSPGF.reader;

namespace CSPGF.linearizer
{
    /**
     * This class represent a 'pre' object.
     * that is either an alternative between multiple lists of tokens
     * with condition on the following words and a default alternative.
     *
     * Example: pre( "parce que", "parce qu'"/"il", "parce qu'"/"on")
     * will be represented by a LeafKP with
     *   defaultTokens = ["parce","que"]
     *   alternatives = [ (["parce", "qu'"], ["il"])
     *                  , (["parce", "qu'"], ["on"]) ]
     **/
    class LeafKP : BracketedTokn
    {
        private String[] defaultTokens;
        private Alternative[] alternatives;

        public LeafKP(String[] _strs, Alternative[] _alts)
        {
            this.defaultTokens = _strs;
            this.alternatives = _alts;
        }

        public String[] getStrs()
        {
            return this.defaultTokens;
        }
        public Alternative[] getAlts()
        {
            return alternatives;
        }
        public String toString()
        {
            String rez = "string names : [";
            for (int i = 0 ; i < defaultTokens.Length ; i++)
            {
                rez += " " + defaultTokens[i];
            }
            rez += "] , Alternatives : [";
            for (int i = 0 ; i < alternatives.Length ; i++)
            {
                rez += " " + alternatives[i].ToString();
            }
            rez += "]";
            return rez;
        }
    }
}
