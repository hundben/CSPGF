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
    using CSPGF.Grammar;

    /// <summary>
    /// ActiveItem class used by the parser to store the current active item.
    /// </summary>
    internal class ActiveItem
    {
        /// <summary>
        /// Initializes a new instance of the ActiveItem class.
        /// </summary>
        /// <param name="offset">The offset</param>
        /// <param name="dot">The position</param>
        /// <param name="fun">The concrete function used</param>
        /// <param name="seq">A list of symbols</param>
        /// <param name="args">A list of arguments</param>
        /// <param name="fid">The current FId</param>
        /// <param name="lbl">A label</param>
        public ActiveItem(int offset, int dot, ConcreteFunction fun, List<Symbol> seq, List<int> args, int fid, int lbl) 
        {
            this.Offset = offset;
            this.Dot = dot;
            this.Fun = fun;
            this.Seq = seq;
            this.Args = args;
            this.FId = fid;
            this.Lbl = lbl;
        }

        /// <summary>
        /// Gets the offset
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        public int Dot { get; private set; }

        /// <summary>
        /// Gets the concrete function we currently use.
        /// </summary>
        public ConcreteFunction Fun { get; private set; }

        /// <summary>
        /// Gets the list of symbols
        /// </summary>
        public List<Symbol> Seq { get; private set; }

        /// <summary>
        /// Gets the list of arguments.
        /// </summary>
        public List<int> Args { get; private set; }

        /// <summary>
        /// Gets the current FId
        /// </summary>
        public int FId { get; private set; }

        /// <summary>
        /// Gets the current label
        /// </summary>
        public int Lbl { get; private set; }

        /// <summary>
        /// A new equals function
        /// </summary>
        /// <param name="obj">The other object which we want to compare with.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ActiveItem)
            {
                var obj2 = (ActiveItem)obj;
                return this.Offset == obj2.Offset &&
                    this.Dot == obj2.Dot &&
                    this.Fun == obj2.Fun &&
                    this.Seq == obj2.Seq &&
                    this.Args == obj2.Args &&
                    this.FId == obj2.FId &&
                    this.Lbl == obj2.Lbl;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Shift over the arguments
        /// </summary>
        /// <param name="i">The argument</param>
        /// <param name="fid">The current fid</param>
        /// <returns>A new ActiveItem</returns>
        public ActiveItem ShiftOverArg(int i, int fid)
        {
            var nargs = new List<int>();
            foreach (int k in this.Args)
            {
                nargs.Add(k);
            }

            nargs[i] = fid;
            return new ActiveItem(this.Offset, this.Dot + 1, this.Fun, this.Seq, nargs, this.FId, this.Lbl);
        }

        /// <summary>
        /// Shift over tokens.
        /// </summary>
        /// <returns>A new ActiveItem</returns>
        public ActiveItem ShiftOverTokn()
        {
            return new ActiveItem(this.Offset, this.Dot + 1, this.Fun, this.Seq, this.Args, this.FId, this.Lbl);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code as an integer.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
