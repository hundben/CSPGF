//-----------------------------------------------------------------------
// <copyright file="Bracket.cs" company="None">
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

namespace CSPGF.Linearize
{
    using System.Collections.Generic;

    /// <summary>
    /// Bracketed tokens?
    /// </summary>
    internal class Bracket : BracketedToken
    {
        /// <summary>
        /// Initializes a new instance of the Bracket class.
        /// </summary>
        /// <param name="cId">Concrete id</param>
        /// <param name="index">Index for this class?</param>
        /// <param name="fId">Function id</param>
        /// <param name="bss">List of BracketedTokns</param>
        public Bracket(string cId, int index, int fId, List<BracketedToken> bss)
        {
            this.CId = cId;
            this.LIndex = index;
            this.FId = fId;
            this.Bracketedtoks = bss;
        }

        /// <summary>
        /// Gets the concrete id
        /// </summary>
        public string CId { get; private set; }

        /// <summary>
        /// Gets the LIndex
        /// </summary>
        public int LIndex { get; private set; }

        /// <summary>
        /// Gets the function id
        /// </summary>
        public int FId { get; private set; }

        /// <summary>
        /// Gets list of bracketed tokens
        /// </summary>
        public List<BracketedToken> Bracketedtoks { get; private set; }

        /// <summary>
        /// Pretty prints the contents of the class
        /// </summary>
        /// <returns>String containing debug information</returns>
        public override string ToString()
        {
            string rez = "name : " + this.CId + ", linIndex : " + this.LIndex + ", fId : " + this.FId + ", bracketed tokens : " + this.Bracketedtoks;

            // for(int i=0;i<bss.length;i++)
            //   rez+=(" "+bss[i].toString());
            return rez;
        }
    }
}
