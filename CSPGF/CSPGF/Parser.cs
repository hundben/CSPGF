/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Parse;
    using CSPGF.Reader;

    public class Parser
    {
        private Concrete language;
        private string startcat;
        /* ******************************** API ******************************** */
        public Parser(PGF pgf, Concrete language)
        {
            this.language = language;
            this.startcat = pgf.GetAbstract().StartCat();
        }
        
        public Parser(PGF pgf, string language) : this(pgf, pgf.GetConcrete(language))
        {
        }

        public void SetStartcat(string startcat)
        {
            this.startcat = startcat;
        }

        /**
         * Parse the given list of tokens
         * @param tokens the input tokens
         * @return the corresponding parse-state
         **/
        // FIXME: not using the start category ??
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

        /**
         * Parse the given list of tokens
         * @param tokens the input tokens
         * @return an array of trees
         **/
        // FIXME: not using the start category ??
        public List<CSPGF.Trees.Absyn.Tree> ParseToTrees(string[] tokens)
        {
            return this.Parse(tokens).GetTrees();
        }

        /**
         * Parse the given string
         * uses a very basic tokenizer that split on whitespaces.
         * @param phrase the input string
         * @return the corresponding parse-state
         **/
        public ParseState Parse(string phrase)
        {
            return this.Parse(phrase.Split(' '));
        }

        /**
         * Parses the empty string
         * (usefull for completion)
         * @param startcat the start category
         * @return the corresponding parse-state
         **/
        public ParseState Parse()
        {
            return this.Parse(new string[0]);
        }
    }
}
