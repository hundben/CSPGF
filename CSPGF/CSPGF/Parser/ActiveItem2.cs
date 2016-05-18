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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPGF.Parse
{
    using CSPGF.Grammar;


    class ActiveItem
    {
        public int offset;

        public int dot;

        public ConcreteFunction fun;

        public List<Symbol> seq;

        public List<int> args;

        public int fid;

        public int lbl;

        public ActiveItem(int offset, int dot, ConcreteFunction fun, List<Symbol> seq, List<int> args, int fid, int lbl) 
        {
            this.offset = offset;
            this.dot = dot;
            this.fun = fun;
            this.seq = seq;
            this.args = args;
            this.fid = fid;
            this.lbl = lbl;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ActiveItem)
            {
                var obj2 = (ActiveItem)obj;
                return (this.offset == obj2.offset &&
                    this.dot == obj2.dot &&
                    this.fun == obj2.fun &&
                    this.seq == obj2.seq &&
                    this.args == obj2.args &&
                    this.fid == obj2.fid &&
                    this.lbl == obj2.lbl);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public ActiveItem shiftOverArg(int i, int fid)
        {
            var nargs = new List<int>();
            foreach (int k in this.args)
            {
                nargs.Add(k);
            }

            nargs[i] = fid;
            return new ActiveItem(this.offset, this.dot + 1, this.fun, this.seq, nargs, this.fid, this.lbl);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActiveItem shiftOverTokn()
        {
            return new ActiveItem(this.offset, this.dot + 1, this.fun, this.seq, this.args, this.fid, this.lbl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
