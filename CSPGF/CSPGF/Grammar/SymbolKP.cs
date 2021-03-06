﻿//-----------------------------------------------------------------------
// <copyright file="SymbolKP.cs" company="None">
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

namespace CSPGF.Grammar
{
    using System;

    /// <summary>
    /// Alternative Token Symbol
    /// </summary>
    [Serializable]
    internal class SymbolKP : Symbol // SymKP
    {
        /// <summary>
        /// Initializes a new instance of the SymbolKP class.
        /// </summary>
        /// <param name="toks">List of tokens</param>
        /// <param name="alts">List of alternatives</param>
        public SymbolKP(Symbol[] toks, Alternative[] alts)
        {
            this.Tokens = toks;
            this.Alts = alts;
        }

        /// <summary>
        /// Gets a list of Symbols
        /// </summary>
        public Symbol[] Tokens { get; private set; }

        /// <summary>
        /// Gets the list of Alternatives
        /// </summary>
        public Alternative[] Alts { get; private set; }

        /// <summary>
        /// Returns true if terminal.
        /// </summary>
        /// <returns>Returns true if terminal</returns>
        public bool IsTerminal()
        {
            return true;
        }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debug information</returns>
        public override string ToString()
        {
            string sb = "SymbolKP[pre { ";
            foreach (Symbol s in this.Tokens) 
            {
                sb += s + " ";
            }

            sb += "; ";
            foreach (Alternative a in this.Alts) 
            {
                sb += a + "; ";
            }

            sb += "}]";
            return sb;
        }
    }
}