//-----------------------------------------------------------------------
// <copyright file="CSPGF.cs" company="None">
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

namespace CSPGF
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    
    /// <summary>
    /// API for others to use :D
    /// </summary>
    public class CSPGFAPI
    {
        /// <summary>
        /// PGF grammar
        /// </summary>
        private PGF pgf;

        /// <summary>
        /// Internal ParseState
        /// </summary>
        Parse.ParseState ps;

        /// <summary>
        /// Initializes a new instance of the CSPGF class.
        /// </summary>
        /// <param name="filename">Filename with path to read</param>
        public CSPGFAPI(string filename)
        {
            PGFReader pgfr = new PGFReader(new BinaryReader(new FileStream(filename, FileMode.Open)));
            this.pgf = pgfr.ReadPGF();
        }

        /// <summary>
        /// Returns a list of available languages
        /// </summary>
        /// <returns>List of languages</returns>
        public List<string> GetLanguages()
        {
            return this.pgf.GetLanguages();
        }

        /// <summary>
        /// Translates a sentence from one language to another. Separates tokens with ' '
        /// </summary>
        /// <param name="from">Language to translate from</param>
        /// <param name="to">Language to translate to</param>
        /// <param name="sentence">Sentence to translate</param>
        /// <returns>Translates sentence</returns>
        public string Translate(string from, string to, string sentence)
        {
            Parse.ParseState ps2 = new Parse.ParseState(this.pgf.GetConcrete(from));
            foreach (string str in sentence.Split(' '))
            {
                ps2.Scan(str);
            }
            Linearizer lin = new Linearizer(this.pgf, this.pgf.GetConcrete(to));
            return lin.LinearizeString(ps2.GetTrees()[0]);
        }

        /// <summary>
        /// Creates a new Parse state.
        /// </summary>
        /// <param name="from">Name of grammar to parse</param>
        /// <returns>True if language exists</returns>
        public bool NewParseState(string from)
        {
            try
            {
                this.ps = new Parse.ParseState(this.pgf.GetConcrete(from));
            }
            catch (UnknownLanguageException e)
            {
                System.Console.WriteLine(e.ToString());
                this.ps = null;
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Linearizes the contents of the current parse state if complete, otherwise returns string.Empty.
        /// </summary>
        /// <param name="to">Language to linearize to</param>
        /// <returns>Linearizes string</returns>
        public string Linearize(string to)
        {
            if (this.ps != null && this.ps.Predict().Count == 0)
            {
                Linearizer lin = new Linearizer(this.pgf, this.pgf.GetConcrete(to));
                return lin.LinearizeString(this.ps.GetTrees()[0]);
            }
            return string.Empty;
        }

        /// <summary>
        /// Scans a token and returns a list of available words to continue with.
        /// Returns null if no parse state is available.
        /// </summary>
        /// <param name="token">Token to scan</param>
        /// <returns>List of strings</returns>
        public List<string> ParseToken(string token)
        {
            if (this.ps != null)
            {
                this.ps.Scan(token);
                return this.ps.Predict();
            }
            else
            {
                return null;
            }
        }
    }
}
