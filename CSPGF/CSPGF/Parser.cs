//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="None">
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
    using CSPGF.Parse;
    using CSPGF.Reader;

    /// <summary>
    /// Used for parsing
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Concrete grammar
        /// </summary>
        private Concrete language;
        
        /// <summary>
        /// Starting category
        /// </summary>
        private string startcat;
        
        /// <summary>
        /// Initializes a new instance of the Parser class.
        /// </summary>
        /// <param name="pgf">PGF object</param>
        /// <param name="language">Concrete language</param>
        public Parser(PGF pgf, Concrete language)
        {
            this.language = language;
            this.startcat = pgf.GetAbstract().StartCat();
        }
        
        /// <summary>
        /// Initializes a new instance of the Parser class.
        /// </summary>
        /// <param name="pgf">PGF object</param>
        /// <param name="language">Concrete language</param>
        public Parser(PGF pgf, string language) : this(pgf, pgf.GetConcrete(language))
        {
        }

        /// <summary>
        /// Sets the staring category
        /// </summary>
        /// <param name="startcat">Starting category</param>
        public void SetStartcat(string startcat)
        {
            this.startcat = startcat;
        }

        /// <summary>
        /// Parse the given list of tokens
        /// </summary>
        /// <param name="tokens">Array of tokens</param>
        /// <returns>Returns the current ParseState</returns>
        public ParseState Parse(string[] tokens)
        {
            ParseState ps = new ParseState(this.language);
            foreach (string w in tokens) 
            {
                if (!ps.Scan(w)) 
                {
                    break;
                }
            }

            return ps;
        }

        /// <summary>
        /// Parse the given list of tokens
        /// </summary>
        /// <param name="tokens">Array of tokens</param>
        /// <returns>List of trees</returns>
        public List<CSPGF.Trees.Absyn.Tree> ParseToTrees(string[] tokens)
        {
            return this.Parse(tokens).GetTrees();
        }

        /// <summary>
        /// Parse the given string, splitting on whitespace to tokenize it.
        /// </summary>
        /// <param name="phrase">String to parse</param>
        /// <returns>Current ParseState</returns>
        public ParseState Parse(string phrase)
        {
            return this.Parse(phrase.Split(' '));
        }

        /// <summary>
        /// Parses an empty string
        /// </summary>
        /// <returns>Current ParseState</returns>
        public ParseState Parse()
        {
            return this.Parse(new string[0]);
        }
    }
}
