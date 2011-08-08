//-----------------------------------------------------------------------
// <copyright file="ActiveItemInt.cs" company="None">
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

namespace CSPGF.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Tuple of active item and int to simplify the usage.
    /// </summary>
    public class ActiveItemInt
    {
        /// <summary>
        /// Initializes a new instance of the ActiveItemInt class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cons">The cons value. Set cons2 manualy for now.</param>
        public ActiveItemInt(ActiveItem item, int cons)
        {
            this.Item = item;
            this.Cons = cons;
            this.Cons2 = -1;  // for safety
        }
        
        /// <summary>
        /// Gets an Active item.
        /// </summary>
        public ActiveItem Item { get; private set; }

        /// <summary>
        /// Gets the value of Cons.
        /// </summary>
        public int Cons { get; private set; }

        /// <summary>
        /// Gets the value of Cons2.
        /// </summary>
        public int Cons2 { get; private set; }

        /// <summary>
        /// Compares to ActiveItemInts.
        /// </summary>
        /// <param name="i">An ActiveItemInt.</param>
        /// <returns>True if equal.</returns>
        public /*override*/ bool Equals(ActiveItemInt i)
        {
            if (i.Cons == this.Cons && i.Item.Equals(this.Item))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compares an ActiveItem and a cons value with this ActiveItemInt.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="cons">The value of cons.</param>
        /// <returns>True if the values are equal.</returns>
        public /*override*/ bool Equals(ActiveItem item, int cons)
        {
            if (this.Cons == cons && this.Item.Equals(item))
            {
                return true;
            }

            return false;
        }
    }
}
