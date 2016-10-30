//-----------------------------------------------------------------------
// <copyright file="AbstractCategory.cs" company="None">
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
    /// <summary>
    /// Abstract Category
    /// </summary>
    internal class AbstractCategory
    {
        /// <summary>
        /// Initializes a new instance of the AbstractCategory class.
        /// </summary>
        /// <param name="name">Name of category</param>
        /// <param name="hypos">List of hypos</param>
        /// <param name="functions">List of weightedidents</param>
        public AbstractCategory(string name, Hypo[] hypos, CategoryFunction[] functions, double weight)
        {
            this.Name = name;
            this.Hypos = hypos;
            this.Functions = functions;
            this.Weight = weight;
        }

        /// <summary>
        /// Gets the name of the abstract category
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a list of hypos.
        /// </summary>
        public Hypo[] Hypos { get; private set; }

        /// <summary>
        /// Gets a list of WeightedIdents
        /// </summary>
        public CategoryFunction[] Functions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debug information</returns>
        public override string ToString()
        {
            string ss = "Name : " + this.Name + " , Hypotheses : (";
            foreach (Hypo h in this.Hypos) 
            {
                ss += " " + h;
            }

            ss += ") , String Names : (";
            foreach (CategoryFunction w in this.Functions) 
            {
                ss += " " + w;
            }

            ss += ")";
            return ss;
        }
    }
}