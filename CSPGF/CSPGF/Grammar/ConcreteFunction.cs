//-----------------------------------------------------------------------
// <copyright file="ConcreteFunction.cs" company="None">
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
    /// Concrete function
    /// </summary>
    [Serializable]
    internal class ConcreteFunction
    {
        /// <summary>
        /// Initializes a new instance of the ConcreteFunction class.
        /// </summary>
        /// <param name="name">Name of function</param>
        /// <param name="sequences">List of list of symbols</param>
        public ConcreteFunction(string name, Symbol[][] sequences)
        {
            this.Name = name;
            this.Sequences = sequences;
        }

        /// <summary>
        /// Gets the name of the concrete function
        /// </summary>
        public string Name { get; private set; } // TODO: This might not be needed. Isn't used in the haskell-version?

        /// <summary>
        /// Gets a list of list of symbols
        /// </summary>
        public Symbol[][] Sequences { get; private set; }

        /// <summary>
        /// Only for testing
        /// </summary>
        public void FixSymbols()
        {
            List<List<Symbol>> syms = new List<List<Symbol>>();
            foreach(Symbol[] st in this.Sequences)
            {
                var temp = new List<Symbol>();
                foreach(Symbol sym in st)
                {
                    if (sym is SymbolKS)
                    {
                        foreach(string token in ((SymbolKS)sym).Tokens)
                        {
                            string[] tmp2 = { token };
                            temp.Add(new SymbolKS(tmp2));
                        }
                    }
                    else
                    {
                        temp.Add(sym);
                    }
                }

                syms.Add(temp);
            }

            Symbol[][] seqs = new Symbol[syms.Count][];
            int i = 0;
            foreach (List<Symbol> t in syms)
            {
                seqs[i] = t.ToArray();
                i++;
            }

            this.Sequences = seqs;

        }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debug information</returns>
        public override string ToString()
        {
            string ss = "Name : " + this.Name + " , Indices : ";
            foreach (Symbol[] s in this.Sequences) 
            {
                foreach (Symbol sym in s)
                {
                    ss += " " + sym;
                }
            }

            return ss;
        }

        /*public override bool Equals(object obj)
        {
            if (obj is ConcreteFunction)
            {
                ConcreteFunction fun = (ConcreteFunction)obj;
                return this.Name.Equals(fun.Name) && this.Sequences.Equals(fun.Sequences);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }*/
    }
}