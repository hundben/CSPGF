//-----------------------------------------------------------------------
// <copyright file="AbsFun.cs" company="None">
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
    /// Abstract Function
    /// </summary>
    internal class AbsFun
    {
        /// <summary>
        /// Initializes a new instance of the AbsFun class.
        /// </summary>
        /// <param name="str">Name of the function</param>
        /// <param name="type">Type of function</param>
        /// <param name="arit">Some integer</param>
        /// <param name="eqs">List of Eqs</param>
        /// <param name="weight">Weight of function</param>
        public AbsFun(string str, Type type, int arit, Eq[] eqs, double weight)
        {
            this.Name = str;
            this.Type = type;
            this.Arit = arit;
            this.Eqs = eqs;
            this.Weight = weight;
        }

        /// <summary>
        /// Gets the name of the functions
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the function
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets some integer
        /// </summary>
        public int Arit { get; private set; }

        /// <summary>
        /// Gets a list of Eqs
        /// </summary>
        public Eq[] Eqs { get; private set; }

        /// <summary>
        /// Gets the weight of the functions
        /// </summary>
        public double Weight { get; private set; }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debuginformation</returns>
        public override string ToString()
        {
            string sb = "<function name = " + this.Name + " type = " + this.Type + " arity = " + this.Arit + " equations = [";
            foreach (Eq e in this.Eqs) 
            {
                sb += e + ", ";
            }

            sb += "] weight = " + this.Weight + " > ";
            return sb;
        }
    }
}
