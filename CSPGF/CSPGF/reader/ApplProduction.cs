//-----------------------------------------------------------------------
// <copyright file="ApplProduction.cs" company="None">
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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Application production
    /// </summary>
    [Serializable]
    internal class ApplProduction : Production
    {
        /// <summary>
        /// List of domains
        /// </summary>
        private int[] dom;

        /// <summary>
        /// Initializes a new instance of the ApplProduction class.
        /// </summary>
        /// <param name="fId">Function id</param>
        /// <param name="function">Concrete function</param>
        /// <param name="domain">List of domains</param>
        public ApplProduction(int fId, CncFun function, int[] domain) : base(0, fId)
        {
            this.Function = function;
            this.dom = domain;
        }

        /// <summary>
        /// Gets the concrete function.
        /// </summary>
        public CncFun Function { get; private set; }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debuginformation</returns>
        public override string ToString()
        {
            // Was commented out in the java-code.
            string s = FId + " -> " + this.Function.Name + "[ ";
            foreach (int c in this.dom) 
            {
                s += c + " ";
            }

            s += "]";
            return s;
        }

        /// <summary>
        /// Get the list of domains
        /// </summary>
        /// <returns>List of domains</returns>
        public override int[] Domain()
        {
            return this.dom;
        }

        /// <summary>
        /// Checks if the contents of two ApplProductions are equal.
        /// </summary>
        /// <param name="o">Object to compare to</param>
        /// <returns>True if equal</returns>
        /*public override bool Equals(object o)
        {
            // TODO: Fix? 
            if (o is ApplProduction) 
            {
                ApplProduction newo = (ApplProduction)o;

                if (!newo.Function.Equals(this.Function)) 
                {
                    return false;
                }

                if (this.dom.Length != newo.dom.Length) 
                {
                    return false;
                }

                for (int i = 0; i < this.dom.Length; i++) 
                {
                    if (this.dom[i] != newo.dom[i]) 
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }*/
        
        /// <summary>
        /// Returns the hashcode for this object.
        /// </summary>
        /// <returns>Returns the hashcode for this object</returns>
        /*public override int GetHashCode()
        {
            return base.GetHashCode();
        }*/
    }
}
