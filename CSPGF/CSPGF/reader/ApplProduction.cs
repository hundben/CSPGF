using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ApplProduction : Production
    {
        public CncFun function { get; private set; }
        public int[] domain { get; private set; }

        public ApplProduction(int _fId, CncFun _function, int[] _domain)
            : base(0, _fId)
        {
            function = _function;
            domain = _domain;
        }

        public String ToString()
        {
            // Was commented out in the java-code.
            String s = fId + " -> " + function.name + "[ ";
            foreach (int c in domain) {
                s += c + " ";
            }
            s += "]";
            return s;
        }

        public Boolean Equals(Object o)
        {
            // TODO: Fix?
            if (o is ApplProduction) {
                ApplProduction newo = (ApplProduction)o;

                if (!newo.function.Equals(function)) {
                    return false;
                }
                if (domain.Length != newo.domain.Length) {
                    return false;
                }
                for (int i = 0 ; i < domain.Length ; i++) {
                    if (domain[i] != newo.domain[i]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
