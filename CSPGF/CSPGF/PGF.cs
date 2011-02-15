using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;

namespace CSPGF
{
    class PGF
    {
        private int majorVersion;
        private int minorVersion;
        private Dictionary<String, RLiteral> flags;
        private Abstract abstr;
        private Dictionary<String, Concrete> concretes;
        public PGF(int _majorVersion, int _minorVersion, Dictionary<String, RLiteral> _flags, Abstract _abstr, Concrete[] _concretes)
        {
            majorVersion = _majorVersion;
            minorVersion = _minorVersion;
            flags = _flags;
            abstr = _abstr;
            concretes = new Dictionary<String, Concrete>();
            foreach (Concrete cnc in _concretes)
                concretes.Add(cnc.getName(), cnc);
        }

        /* ******************************** API ******************************** */
        /**
         * access the concrete grammar by its name
         * @param name the name of the concrete grammar
         * @return the concrete grammar of null if there is no grammr with
         *             that name.
         */
        public Concrete concrete(String name)
        {
            Concrete l = concretes[name];
            if (l == null)
                throw new UnknownLanguageException(name);
            return l;
        }

        /* ************************************************* */
        /* Accessing the fields                              */
        /* ************************************************* */

        public int getMajorVersion()
        {
            return majorVersion;
        }

        public int getMinorVersion()
        {
            return minorVersion;
        }

        public Abstract getAbstract()
        {
            return abstr;
        }
        /**
        * Return true if the given name crrespond to a concrete grammar
        * in the pgf, false otherwise.
        */
        public Boolean hasConcrete(String name)
        {
            return concretes.ContainsKey(name);
        }

        //TODO : cleanups
        public String toString()
        {
            String ss = "PGF : \nmajor version : " + majorVersion + ", minor version : " + minorVersion + "\n" + "flags : (";
            foreach (String flagName in flags.Keys)
            {
                ss += flagName + ": " + flags[flagName].toString() + "\n";
            }
            ss += (")\nabstract : (" + abstr.toString() + ")\nconcretes : (");
            foreach (String name in concretes.Keys)
            {
                ss += name + ", ";
            }
            ss += ")";
            return ss;
        }
    }
}
