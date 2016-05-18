//-----------------------------------------------------------------------
// <copyright file="SimpleTranslator.cs" company="None">
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
    using System.Collections.Generic;
    using Grammar;
    
    /// <summary>
    /// API for others to use :D
    /// </summary>
    public class SimpleTranslator
    {
        /// <summary>
        /// Version of the API
        /// </summary>
        private const int Version = 1;

        /// <summary>
        /// PGF grammar
        /// </summary>
        private readonly PGF pgf;

        /// <summary>
        /// Internal ParseState
        /// </summary>
        private Parse.ParseState ps;

        /// <summary>
        /// Internal Linearizer
        /// </summary>
        private Linearize.Linearizer lin;

        /// <summary>
        /// Language to translate to
        /// </summary>
        private string to = string.Empty;

        /// <summary>
        /// Language to translate from
        /// </summary>
        private string from = string.Empty;

        /// <summary>
        /// Initializes a new instance of the SimpleTranslator class.
        /// </summary>
        /// <param name="filename">PGF-file to read from</param>
        public SimpleTranslator(string filename)
        {
            PGFReader pgfr = new PGFReader(filename);
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
            if (!this.from.Equals(from))
            {
                this.ps = new Parse.ParseState(this.pgf.GetConcrete(from));
                this.from = from;
            }
            else
            {
                this.ps.Reset();
            }

            foreach (string str in sentence.Split(' '))
            {
                this.ps.Next(str);
            }

            if (!this.to.Equals(to))
            {
                this.lin = new Linearize.Linearizer(this.pgf, this.pgf.GetConcrete(to));
                this.to = to;
            }

            List<Trees.Absyn.Tree> trees = this.ps.GetTrees();
            if (trees.Count != 0)
            {
                return this.lin.LinearizeString(trees[0]);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the version of the API
        /// </summary>
        /// <returns>Integer containing the version number</returns>
        public int GetVersion()
        {
            return Version;
        }
    }
}
