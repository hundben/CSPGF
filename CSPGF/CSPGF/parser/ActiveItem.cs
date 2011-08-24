//-----------------------------------------------------------------------
// <copyright file="ActiveItem.cs" company="None">
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

namespace CSPGF.Parse
{
    using System.Collections.Generic;
    using CncFun = CSPGF.Reader.CncFun;
    using Symbol = CSPGF.Reader.Symbol;
    using System;

    /// <summary>
    /// One active item.
    /// </summary>
    [Serializable]
    public class ActiveItem
    {
        /// <summary>
        /// Initializes a new instance of the ActiveItem class.
        /// </summary>
        /// <param name="begin">Insert a description for begin.</param>
        /// <param name="category">The category.</param>
        /// <param name="function">The function.</param>
        /// <param name="domain">A list of the domains.</param>
        /// <param name="constituent">Insert a description for constituent.</param>
        /// <param name="position">Insert a description for position.</param>
        public ActiveItem(int begin, int category, CncFun function, List<int> domain, int constituent, int position)
        {
            this.Begin = begin;
            this.Category = category;
            this.Function = function;
            this.Domain = domain;
            this.Constituent = constituent;
            this.Position = position;
        }

        /// <summary>
        /// Gets the value of begin.
        /// </summary>
        public int Begin { get; private set; }

        /// <summary>
        /// Gets the value of category.
        /// </summary>
        public int Category { get; private set; }

        /// <summary>
        /// Gets the value of function.
        /// </summary>
        public CncFun Function { get; private set; }

        /// <summary>
        /// Gets the value of domain.
        /// </summary>
        public List<int> Domain { get; private set; }

        /// <summary>
        /// Gets the value of constituent.
        /// </summary>
        public int Constituent { get; private set; }

        /// <summary>
        /// Gets the value of position.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the next symbol.
        /// </summary>
        /// <returns>The next symbol.</returns>
        public Symbol NextSymbol()
        {
            if (this.Position < this.Function.Sequences[this.Constituent].Symbs.Count) 
            {
                Symbol sym = this.Function.Sequences[this.Constituent].GetSymbol(this.Position);
                return sym;
            }

            return null;    // this might be dangerous
        }

        /// <summary>
        /// Equals method.
        /// </summary>
        /// <param name="ai">An instance of ActiveItem.</param>
        /// <returns>True if content is equal.</returns>
        public /*override*/ bool Equals(ActiveItem ai)
        {
            if (this.Begin == ai.Begin &&
                this.Category == ai.Category &&
                this.Function == ai.Function &&
                this.Constituent == ai.Constituent &&
                this.Position == ai.Position) 
            {
                // Since there is no deep method in c# that we know of, use a crappy forloop
                if (this.Domain.Count == ai.Domain.Count) 
                {
                    for (int i = 0; i < this.Domain.Count; i++) 
                    {
                        if (this.Domain[i] != ai.Domain[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            string str = "[" + this.Begin.ToString() + ";" + this.Category.ToString() +
                "->" + this.Function.Name + "[" + this.DomainToString() + "];" +
                this.Constituent.ToString() + ";" + this.Position.ToString() + "]";
            return str;
        }

        /// <summary>
        /// Converts the list of domains to a string.
        /// </summary>
        /// <returns>The string with the domains.</returns>
        public string DomainToString()
        {
            string tot = string.Empty;
            foreach (int d in this.Domain) 
            {
                tot += d.ToString();
            }

            return tot;
        }
    }
}
