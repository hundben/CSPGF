//-----------------------------------------------------------------------
// <copyright file="ProductionConst.cs" company="None">
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
    using System.Collections.Generic;

    /// <summary>
    /// Constant production. Used for Literals.
    /// </summary>
    internal class ProductionConst : Production
    {
        /// <summary>
        /// Initializes a new instance of the ProductionConst class.
        /// </summary>
        /// <param name="fId">Function id.</param>
        /// <param name="function">The corresponding concrete function.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <param name="type">The type of the literal, same as FId for literals.</param>
        public ProductionConst(int fId, ConcreteFunction function, List<string> tokens, int type) : base(0, fId)
        {
            this.Type = type;
            this.Tokens = tokens;
            this.Fun = function;
        }

        /// <summary>
        /// Initializes a new instance of the ProductionConst class.
        /// </summary>
        /// <param name="function">The corresponding concrete function.</param>
        /// <param name="tokens">List of tokens.</param>
        /// <param name="type">The type of the literal, same as FId for literals.</param>
        public ProductionConst(ConcreteFunction function, List<string> tokens, int type) : base(0, -4)
        {
            this.Type = type;
            this.Tokens = tokens;
            this.Fun = function;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the list of tokens.
        /// </summary>
        public List<string> Tokens { get; set; }

        /// <summary>
        /// Gets or sets the concrete function.
        /// </summary>
        public ConcreteFunction Fun { get; set; }

        /// <summary>
        /// A new to string function
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return "ProductionConst(" + this.Tokens.ToString() + ")";
        }
    }
}
