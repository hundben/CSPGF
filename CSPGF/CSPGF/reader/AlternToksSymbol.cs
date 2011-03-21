using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AlternToksSymbol : ToksSymbol
    {
        public Alternative[] alts { get; private set; }

        public AlternToksSymbol(String[] _toks, Alternative[] _alts)
            : base(_toks)
        {
            alts = _alts;
        }

        public Boolean IsTerminal()
        {
            return true;
        }

        public override String ToString()
        {
            String sb = "pre { ";
            foreach (String s in base.tokens) {
                sb += s + " ";
            }
            sb += ("; ");
            foreach (Alternative a in alts) {
                sb += a + "; ";
            }
            sb += "}";
            return sb;
        }
    }
}