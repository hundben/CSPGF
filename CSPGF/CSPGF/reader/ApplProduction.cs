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

        public ApplProduction(int _fId, CncFun _function, int[] _domain) : base(0,_fId)
        {
            // Should not be needed anymore
            //base.fId = _fId;
            //base.sel = 0;
            function = _function;
            domain = _domain;
        }

        public int[] getDomain()
        {
            return domain;
        }

        public CncFun getFunction()
        {
            return function;
        }

        public int[] getArgs()
        {
            return domain;
        }

        public String toString()
        {
            // String ss =  "Fuction : "+ function + " Arguments : [";
            // for(int i=0; i<domain.length; i++)
            //     ss+=(" " + domain[i]);
            // ss+="]";
            // return ss;
            String s = fId + " -> " + function.getName() + "[ ";
            foreach (int c in domain)
            {
                s += c + " ";
            }
            s += "]";
            return s;
        }

        public Boolean equals(Object o)
        {
            // TODO: Fix?
            if (o is ApplProduction)
            {
                ApplProduction newo = (ApplProduction)o;

                if (!newo.getFunction().Equals(function))
                {
                    return false;
                }
                if (domain.Length != newo.getArgs().Length)
                {
                    return false;
                }
                for (int i = 0 ; i < domain.Length ; i++)
                {
                    if (domain[i] != newo.getArgs()[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
