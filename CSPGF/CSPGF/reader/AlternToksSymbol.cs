using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AlternToksSymbol : ToksSymbol
    {
        Alternative[] alts;

        public AlternToksSymbol(String[] _toks, Alternative[] _alts)
        {
            base.setTokens(_toks);
            alts = _alts;
        }

        public Boolean isTerminal()
        {
            return true;
        }

        public String toString()
        {
            String sb = "pre { ";
            foreach (String s in base.getTokens())
            {
                sb += s + " ";
            }
            sb += ("; ");
            foreach (Alternative a in alts)
            {
                sb += a + "; ";
            }
            sb += "}";
            return sb;
        }

        public String[] getToks()
        {
            return base.getTokens();
        }
        public Alternative[] getAlternatives()
        {
            return alts;
        }

    }
}