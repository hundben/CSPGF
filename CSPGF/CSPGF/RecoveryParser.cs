//-----------------------------------------------------------------------
// <copyright file="RecoveryParser.cs" company="None">
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
    using System.Collections.Generic;
    using CSPGF.Parse;
    using CSPGF.Reader;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RecoveryParser
    {
        /// <summary>
        /// The language
        /// </summary>
        private Concrete language;

        /// <summary>
        /// The start category
        /// </summary>
        private string startcat;

        /// <summary>
        /// Stack with all the states
        /// </summary>
        private Stack<ParseState> parseStates;

        /// <summary>
        /// Initializes a new instance of the RecoveryParser class.
        /// </summary>
        /// <param name="pgf">The current pgf class.</param>
        /// <param name="language">The language.</param>
        public RecoveryParser(PGF pgf, Concrete language)
        {
            this.language = language;
            this.startcat = pgf.GetAbstract().StartCat();
            this.parseStates = new Stack<ParseState>();
            ParseState ps = new ParseState(this.language);
            this.parseStates.Push(ps);
        }

        /// <summary>
        /// Initializes a new instance of the RecoveryParser class.
        /// </summary>
        /// <param name="pgf">The current pgf instance.</param>
        /// <param name="language">The language as a string.</param>
        public RecoveryParser(PGF pgf, string language) : this(pgf, pgf.GetConcrete(language))
        {
        }

        /// <summary>
        /// Scan one token.
        /// </summary>
        /// <param name="token">The next token</param>
        /// <returns>True if scan was successful.</returns>
        public bool Scan(string token)
        {
            if (this.parseStates.Count == 0)
            {
                this.parseStates.Push(new ParseState(this.language));
            }

            ParseState ps = this.parseStates.Peek();
            ParseState copy = ObjectCopier.Clone<ParseState>(ps);
            bool result = copy.Scan(token);
            if (!result)
            {
                return false;
            }

            // if scan is successful store the copy and return true
            this.parseStates.Push(copy);
            return true;
        }

        /// <summary>
        /// Removes one token
        /// </summary>
        /// <returns>True if one is removed.</returns>
        public bool RemoveOne()
        {
            if (this.parseStates.Count > 0)
            {
                this.parseStates.Pop();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a list of possible tokens.
        /// </summary>
        /// <returns>A list of possible tokens</returns>
        public List<string> Predict()
        {
            ParseState ps = this.parseStates.Peek();
            return ps.Predict();
        }

        /// <summary>
        /// Prints the current predictions.
        /// </summary>
        public void Debug()
        {
            Console.WriteLine("Prediction");
            foreach (string tok in this.Predict())
            {
                Console.WriteLine(tok);
            }
        }
    }
}
