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
            : base(_toks)
        {
            alts = _alts;
        }

        public Boolean IsTerminal()
        {
            return true;
        }

        public String ToString()
        {
            String sb = "pre { ";
            foreach (String s in base.getTokens()) {
                sb += s + " ";
            }
            sb += ("; ");
            foreach (Alternative a in alts) {
                sb += a + "; ";
            }
            sb += "}";
            return sb;
        }

        public String[] GetToks()
        {
            return base.getTokens();
        }
        public Alternative[] GetAlternatives()
        {
            return alts;
        }

    }
}