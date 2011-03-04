using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ApplProduction : Production
    {
        private CncFun function;
        private int[] domain;

        public ApplProduction(int _fId, CncFun _function, int[] _domain)
            : base(0, _fId)
        {
            function = _function;
            domain = _domain;
        }

        public int[] GetDomain()
        {
            return domain;
        }

        public CncFun GetFunction()
        {
            return function;
        }

        public int[] GetArgs()
        {
            return domain;
        }

        public String ToString()
        {
            // Was commented out in the java-code.
            // String ss =  "Fuction : "+ function + " Arguments : [";
            // for(int i=0; i<domain.length; i++)
            //     ss+=(" " + domain[i]);
            // ss+="]";
            // return ss;
            String s = fId + " -> " + function.GetName() + "[ ";
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

                if (!newo.GetFunction().Equals(function)) {
                    return false;
                }
                if (domain.Length != newo.GetArgs().Length) {
                    return false;
                }
                for (int i = 0 ; i < domain.Length ; i++) {
                    if (domain[i] != newo.GetArgs()[i]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
