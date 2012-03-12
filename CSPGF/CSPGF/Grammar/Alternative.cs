//-----------------------------------------------------------------------
// <copyright file="Alternative.cs" company="None">
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
    /// Alternative class
    /// </summary>
    [Serializable]
    internal class Alternative
    {
        /// <summary>
        /// Initializes a new instance of the Alternative class.
        /// </summary>
        /// <param name="alt1">Normal tokens</param>
        /// <param name="alt2">List of prefixes</param>
        public Alternative(string[] alt1, string[] alt2)
        {
            this.Alt1 = alt1;
            this.Alt2 = alt2;
        }

        // tokens = alt1, prefix = alt2

        /// <summary>
        /// Gets a list of tokens
        /// </summary>
        public string[] Alt1 { get; private set; }  // Check: Rename to tokens instead?

        /// <summary>
        /// Gets a list of prefixes
        /// </summary>
        public string[] Alt2 { get; private set; }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debuginformation</returns>
        public override string ToString()
        {
            string sb = string.Empty;
            foreach (string t in this.Alt1)
            {
                sb += t + " ";
            }

            sb += "/ ";
            foreach (string t in this.Alt2)
            {
                sb += t + " ";
            }

            return sb;
        }
    }
}