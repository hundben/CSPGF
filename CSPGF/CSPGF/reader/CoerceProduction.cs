//-----------------------------------------------------------------------
// <copyright file="CoerceProduction.cs" company="None">
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
    /// Coerce Production
    /// </summary>
    [Serializable]
    internal class CoerceProduction : Production
    {
        /// <summary>
        /// Initializes a new instance of the CoerceProduction class.
        /// </summary>
        /// <param name="fId">Function id</param>
        /// <param name="initId">Initial id</param>
        public CoerceProduction(int fId, int initId) : base(1, fId)
        {
            this.InitId = initId;
        }

        /// <summary>
        /// Gets the initial id
        /// </summary>
        public int InitId { get; private set; }

        /// <summary>
        /// Returns the domains
        /// </summary>
        /// <returns>Returns a list of domains</returns>
        public override List<int> Domain()
        {
            List<int> tmp = new List<int>();
            tmp.Add(this.InitId);
            return tmp;
        }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debuginformation</returns>
        public override string ToString()
        {
            return "Coercion(" + this.FId + " -> " + this.InitId + ")";
        }

        /// <summary>
        /// Checks if the contents of two CoerceProductions is equal.
        /// </summary>
        /// <param name="o">Production to compare to</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object o)
        {
            if (o is CoerceProduction) 
            {
                return ((CoerceProduction)o).InitId == this.InitId;
            }

            return false;
        }

        /// <summary>
        /// Returns the hashcode for this object.
        /// </summary>
        /// <returns>Returns the hashcode for this object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
