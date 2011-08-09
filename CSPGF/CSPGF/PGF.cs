/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;

    class PGF
    {
        private int majorVersion;
        private int minorVersion;
        private Dictionary<string, RLiteral> flags;
        private Abstract abstr;
        private Dictionary<string, Concrete> concretes;
        public PGF(int _majorVersion, int _minorVersion, Dictionary<string, RLiteral> _flags, Abstract _abstr, Dictionary<string, Concrete> _concretes)
        {
            this.majorVersion = _majorVersion;
            this.minorVersion = _minorVersion;
            this.flags = _flags;
            this.abstr = _abstr;
            this.concretes = _concretes;
        }

        /* ******************************** API ******************************** */
        /**
         * access the concrete grammar by its name
         * @param name the name of the concrete grammar
         * @return the concrete grammar of null if there is no grammr with
         *             that name.
         */
        public Concrete GetConcrete(String name)
        {
            Concrete conc;
            if (!concretes.TryGetValue(name, out conc))
                throw new UnknownLanguageException(name);
            return conc;
        }

        /* ************************************************* */
        /* Accessing the fields                              */
        /* ************************************************* */

        public int GetMajorVersion()
        {
            return this.majorVersion;
        }

        public int GetMinorVersion()
        {
            return this.minorVersion;
        }

        public Abstract GetAbstract()
        {
            return this.abstr;
        }
        /**
        * Return true if the given name correspond to a concrete grammar
        * in the pgf, false otherwise.
        */
        public bool HasConcrete(string name)
        {
            return this.concretes.ContainsKey(name);
        }

        public override string ToString()
        {
            string ss = "PGF : \nMajor version : " + majorVersion + ", Minor version : " + minorVersion + "\n" + "Flags : (";
            foreach (string flagName in flags.Keys) {
                ss += flagName + ": " + flags[flagName].ToString() + "\n";
            }
            ss += ")\nAbstract : (" + abstr.ToString() + ")\nConcretes : (";
            foreach (string name in concretes.Keys) {
                ss += name + ", ";
            }
            ss += ")";
            return ss;
        }
    }
}