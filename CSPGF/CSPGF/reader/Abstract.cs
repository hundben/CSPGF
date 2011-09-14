//-----------------------------------------------------------------------
// <copyright file="Abstract.cs" company="None">
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

namespace CSPGF.Reader
{
    using System.Collections.Generic;

    /// <summary>
    /// The Abstract class
    /// </summary>
    internal class Abstract
    {
        /// <summary>
        /// Abstract flags
        /// </summary>
        private Dictionary<string, RLiteral> flags;
        
        /// <summary>
        /// Initializes a new instance of the Abstract class.
        /// </summary>
        /// <param name="name">Abstract name</param>
        /// <param name="flags">Abstract flags</param>
        /// <param name="absFuns">Abstract functions</param>
        /// <param name="absCats">Abstract categories</param>
        public Abstract(string name, Dictionary<string, RLiteral> flags, AbsFun[] absFuns, AbsCat[] absCats)
        {
            this.Name = name;
            this.flags = flags;
            this.AbsFuns = absFuns;
            this.AbsCats = absCats;
        }

        /// <summary>
        /// Gets the name of the abstract
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a list of abstract functions
        /// </summary>
        public AbsFun[] AbsFuns { get; private set; }

        /// <summary>
        /// Gets a list of abstract categories
        /// </summary>
        public AbsCat[] AbsCats { get; private set; }

        /// <summary>
        /// Returns the starting category, or "Sentence" if it doesn't exist.
        /// </summary>
        /// <returns>Returns the starting category</returns>
        public string StartCat()
        {
            RLiteral cat = null;
            if (this.flags.TryGetValue("startcat", out cat)) 
            {
                return ((StringLit)cat).Value;
            }
            else 
            {
                return "Sentence";
            }
        }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debuginformation</returns>
        public override string ToString()
        {
            string ss = "Name : " + this.Name + " , Flags : (";

            // TODO: Är bortkommenterat i javakoden också kanske borde fixa?
            // for(int i=0; i<flags.length;i++)
            // ss+=(" "+flags[i].toString());
            foreach (KeyValuePair<string, RLiteral> kvp in this.flags) 
            {
                ss += "String: " + kvp.Key + "RLiteral: " + kvp.Value.ToString();
            }

            ss += ") , Abstract Functions : (";
            foreach (AbsFun a in this.AbsFuns) 
            {
                ss += " " + a.ToString();
            }

            ss += ") , Abstract Categories : (";
            foreach (AbsCat a in this.AbsCats) 
            {
                ss += " " + a.ToString();
            }

            ss += ")";
            return ss;
        }
    }
}
