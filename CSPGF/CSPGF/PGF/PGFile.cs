//-----------------------------------------------------------------------
// <copyright file="PGFile.cs" company="None">
//  Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), 
//   Erik Bergström (erktheorc@gmail.com) 
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//   * Redistributions of source code must retain the above copyright
//     notice, this list of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright
//     notice, this list of conditions and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//   * Neither the name of the &lt;organization&gt; nor the
//     names of its contributors may be used to endorse or promote products
//     derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &quot;AS IS&quot; AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL &lt;COPYRIGHT HOLDER&gt; BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
//-----------------------------------------------------------------------

namespace CSPGF
{
    using System.Collections.Generic;
    using CSPGF.PGF;

    /// <summary>
    /// Portable Grammar Format
    /// </summary>
    internal class PGFile
    {
        /// <summary>
        /// Major version number
        /// </summary>
        private int majorVersion;

        /// <summary>
        /// Minor version number
        /// </summary>
        private int minorVersion;

        /// <summary>
        /// Flags for the PGF-object
        /// </summary>
        private Dictionary<string, RLiteral> flags;

        /// <summary>
        /// Abstract grammar
        /// </summary>
        private Abstract abstr;

        /// <summary>
        /// Dictionary containing concrete grammars and their names.
        /// </summary>
        private Dictionary<string, Concrete> concretes;

        /// <summary>
        /// Initializes a new instance of the PGF class.
        /// </summary>
        /// <param name="majorVersion">Major version number</param>
        /// <param name="minorVersion">Minor version number</param>
        /// <param name="flags">Flags for the PGF object</param>
        /// <param name="abstr">Abstract grammar</param>
        /// <param name="concretes">Concrete grammars</param>
        public PGFile(int majorVersion, int minorVersion, Dictionary<string, RLiteral> flags, Abstract abstr, Dictionary<string, Concrete> concretes)
        {
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
            this.flags = flags;
            this.abstr = abstr;
            this.concretes = concretes;
        }

        /// <summary>
        /// Returns the concrete with the inputname.
        /// </summary>
        /// <param name="name">Name of grammar</param>
        /// <returns>Concrete grammar</returns>
        public Concrete GetConcrete(string name)
        {
            Concrete conc;
            if (!this.concretes.TryGetValue(name, out conc)) 
            {
                throw new UnknownLanguageException(name);
            }

            return conc;
        }

        /// <summary>
        /// Returns major version number
        /// </summary>
        /// <returns>Major version number</returns>
        public int GetMajorVersion()
        {
            return this.majorVersion;
        }

        /// <summary>
        /// Returns minor version number
        /// </summary>
        /// <returns>Minor version number</returns>
        public int GetMinorVersion()
        {
            return this.minorVersion;
        }

        /// <summary>
        /// Returns the abstract grammar.
        /// </summary>
        /// <returns>Abstract grammar</returns>
        public Abstract GetAbstract()
        {
            return this.abstr;
        }

        /**
        * Return true if the given name correspond to a concrete grammar
        * in the pgf, false otherwise.
        */

        /// <summary>
        /// Returns true if a concrete grammar with the name exists.
        /// </summary>
        /// <param name="name">Name of concrete grammar</param>
        /// <returns>True if the grammar exists</returns>
        public bool HasConcrete(string name)
        {
            return this.concretes.ContainsKey(name);
        }

        /// <summary>
        /// Pretty prints information about the PGF-object
        /// </summary>
        /// <returns>String with debuginformation</returns>
        public override string ToString()
        {
            string ss = "PGF : \nMajor version : " + this.majorVersion + ", Minor version : " + this.minorVersion + "\n" + "Flags : (";
            foreach (string flagName in this.flags.Keys) 
            {
                ss += flagName + ": " + this.flags[flagName].ToString() + "\n";
            }

            ss += ")\nAbstract : (" + this.abstr.ToString() + ")\nConcretes : (";
            foreach (string name in this.concretes.Keys) 
            {
                ss += name + ", ";
            }

            ss += ")";
            return ss;
        }

        /// <summary>
        /// Returns all the available languages in this PGF grammar.
        /// </summary>
        /// <returns>List of languages</returns>
        public List<string> GetLanguages()
        {
            List<string> tmp = new List<string>();
            foreach (KeyValuePair<string, Concrete> kvp in this.concretes)
            {
                tmp.Add(kvp.Key);
            }

            return tmp;
        }
    }
}