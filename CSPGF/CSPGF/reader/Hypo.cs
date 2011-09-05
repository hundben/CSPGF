//-----------------------------------------------------------------------
// <copyright file="Hypo.cs" company="None">
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
    /// <summary>
    /// Hypo class
    /// </summary>
    public class Hypo
    {
        /// <summary>
        /// Initializes a new instance of the Hypo class.
        /// </summary>
        /// <param name="bind">Bind type</param>
        /// <param name="str">Name of hypo</param>
        /// <param name="type">Typ of hypo</param>
        public Hypo(bool bind, string str, Type type)
        {
            this.Bind = bind;
            this.Name = str;
            this.Type = type;
        }

        /// <summary>
        /// Gets a value indicating whether it binds or not
        /// </summary>
        public bool Bind { get; private set; }

        /// <summary>
        /// Gets the name of the hypo
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the hypo
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Pretty prints the contents of this class
        /// </summary>
        /// <returns>Returns a string containing debuginformation</returns>
        public override string ToString()
        {
            return "Bound Type : " + this.Bind + " , Name : " + this.Name + " , Type : (" + Type + ")";
        }
    }
}