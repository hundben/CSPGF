﻿//-----------------------------------------------------------------------
// <copyright file="SymbolVar.cs" company="None">
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
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class SymbolVar : Symbol
    {
        /// <summary>
        /// Initializes a new instance of the SymbolVar class.
        /// </summary>
        /// <param name="arg">Argument index</param>
        /// <param name="var">Variable number</param>
        public SymbolVar(int arg, int var)
        {
            this.Arg = arg;
            this.Var = var;
        }

        /// <summary>
        /// Gets the argument index
        /// </summary>
        public int Arg { get; private set; }     // Int argument index

        /// <summary>
        /// Gets the variable number.
        /// </summary>
        public int Var { get; private set; }     // Int variable number

        /// <summary>
        /// Pretty prints the data contained in the class.
        /// </summary>
        /// <returns>String with data</returns>
        public override string ToString() 
        {
            return "SymbolVar[Arg: " + this.Arg + " Var: " + this.Var + "]";
        }
    }
}
