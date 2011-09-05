//-----------------------------------------------------------------------
// <copyright file="CncCat.cs" company="None">
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
    /// Concrete category are a maping from category names (abstract-categories)
    /// to multiple, conjoint, concrete categories.
    /// They are represented in the pgf binary by :
    ///  - the name of the abstract category (ex: Adj)
    ///  - the first concrete categoy (ex : C18)
    ///  - the last corresponding concrete category (ex : C21)
    ///  - a list of labels (names of fields in the pmcfg tuples)
    /// Here we will keep only the indices.
    /// </summary>
    [Serializable]
    public class CncCat
    {
        /// <summary>
        /// Initializes a new instance of the CncCat class.
        /// </summary>
        /// <param name="name">Name of category</param>
        /// <param name="firstFId">First id</param>
        /// <param name="lastFId">Last id</param>
        /// <param name="labels">List of labels</param>
        public CncCat(string name, int firstFId, int lastFId, List<string> labels)
        {
            this.Name = name;
            this.FirstFID = firstFId;
            this.LastFID = lastFId;
            this.Labels = labels; // was also commented out.
        }

        /// <summary>
        /// Gets the name of the category
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the first id
        /// </summary>
        public int FirstFID { get; private set; }

        /// <summary>
        /// Gets the last id
        /// </summary>
        public int LastFID { get; private set; }

        /// <summary>
        /// Gets the list of labels
        /// </summary>
        public List<string> Labels { get; private set; }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debuginformation</returns>
        public override string ToString()
        {
            return this.Name + " [C" + this.FirstFID + " ... C" + this.LastFID + "]";
        }
    }
}
