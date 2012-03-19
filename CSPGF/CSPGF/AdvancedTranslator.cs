//-----------------------------------------------------------------------
// <copyright file="AdvancedTranslator.cs" company="None">
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
    using System.Reflection;
    using Grammar;
    using Parse;

    /// <summary>
    /// TODO: Update summary.
    /// Remove most of this later.
    /// </summary>
    public class AdvancedTranslator
    {
        /// <summary>
        /// The pgf file, only one per instance of the translator.
        /// </summary>
        private readonly PGF pgf;

        /// <summary>
        /// The start category
        /// </summary>
        private string startcat;

        /// <summary>
        /// Stack with all the states
        /// </summary>
        private ParseState currentPState;

        /// <summary>
        /// Current linearizer
        /// </summary>
        private Linearize.Linearizer currentLin;

        /// <summary>
        /// The language last used to parse
        /// </summary>
        private string fromLanguage = string.Empty;

        /// <summary>
        /// The language last used to linearize to
        /// </summary>
        private string toLanguage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the AdvancedTranslator class.
        /// </summary>
        /// <param name="filename">The name and path of the pgf file.</param>
        public AdvancedTranslator(string filename)
        {
            PGFReader pr = new PGFReader(filename);
            this.pgf = pr.ReadPGF();
        }

        /// <summary>
        /// Sets the input language, creates a new parsestate if needed.
        /// </summary>
        /// <param name="language">The name of the input language.</param>
        public void SetInputLanguage(string language)
        {
            if (language != this.fromLanguage)
            {
                this.fromLanguage = language;
                Concrete language2 = this.pgf.GetConcrete(this.fromLanguage);
                this.startcat = this.pgf.GetAbstract().StartCat();
                this.currentPState = new ParseState(language2);
            }
            else
            {
                this.Reset();
            }
        }

        /// <summary>
        /// Set the output language
        /// </summary>
        /// <param name="language">The language as a string.</param>
        public void SetOutputLanguage(string language)
        {
            if (language != this.toLanguage)
            {
                this.toLanguage = language;
                this.currentLin = new Linearize.Linearizer(this.pgf, this.pgf.GetConcrete(this.toLanguage));
            }
        }

        /// <summary>
        /// Returns a list of supported languages
        /// </summary>
        /// <returns>List of languages.</returns>
        public List<string> GetLanguages()
        {
            return this.pgf.GetLanguages();
        }

        /// <summary>
        /// Scan one token.
        /// </summary>
        /// <param name="token">The next token</param>
        /// <returns>True if scan was successful.</returns>
        public bool Scan(string token)
        {
            if (this.fromLanguage != string.Empty)
            {
                return this.currentPState.Scan(token);
            }

            return false;
        }

        /// <summary>
        /// Scan a few tokens at a time
        /// </summary>
        /// <param name="tokens">A string with tokens.</param>
        /// <returns>True if scan was successful</returns>
        public bool ScanTokens(string tokens)
        {
            if (this.fromLanguage != string.Empty)
            {
                foreach (string tok in tokens.Split())
                {
                    if (!this.currentPState.Scan(tok))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Translates the scaned sentence to the language set as output language.
        /// </summary>
        /// <returns>A string with the translated sentence.</returns>
        public string Translate()
        {
            if (this.toLanguage != string.Empty)
            {
                List<Trees.Absyn.Tree> lt = this.GetTrees();
                if (lt.Count > 0)
                {
                    Trees.Absyn.Tree t = lt[0];
                    return this.currentLin.LinearizeString(t);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a list of possible tokens.
        /// </summary>
        /// <returns>A list of possible tokens</returns>
        public List<string> Predict()
        {
            return this.currentPState.Predict();
        }

        /// <summary>
        /// Get the trees
        /// </summary>
        /// <returns>Returns the trees.</returns>
        public List<Trees.Absyn.Tree> GetTrees()
        {
            return this.currentPState.GetTrees();
        }

        /// <summary>
        /// Removes last token.
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool RemoveToken()
        {
            return this.currentPState.RemoveToken();
        }

        /// <summary>
        /// Removes all tokens.
        /// </summary>
        public void Reset()
        {
            this.currentPState.Reset();
        }

        /// <summary>
        /// Linearizes a tree using previously set language.
        /// </summary>
        /// <param name="tree">Tree to linearize</param>
        /// <returns>Linearized string</returns>
        public string LinearizeTree(Trees.Absyn.Tree tree)
        {
            if (this.toLanguage != string.Empty)
            {
                if (tree != null)
                {
                    return this.currentLin.LinearizeString(tree);
                }
            }

            return string.Empty;
        }
        
        /// <summary>
        /// Checks if the current OS is *NIX
        /// </summary>
        /// <returns>True if *NIX</returns>
        public bool IsLinux()
        {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
        }

        /// <summary>
        /// Use .NET speech recognition to recognise words.
        /// </summary>
        /// <param name="time">How long to listen</param>
        /// <returns>Recognised words</returns>
        public string Listen(TimeSpan time)
        {
            if (this.IsLinux())
            {
                throw new Exception("Voice recognition doesn't work with mono.");
            }

            Assembly assembly = Assembly.LoadFrom("Speech.dll");
            System.Type speech = assembly.GetType("Speech.Speech");
            object speechobj = Activator.CreateInstance(speech);
            return (string)speech.InvokeMember(
                                               "Listen", 
                                               BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                                               null, 
                                               speechobj, 
                                               new object[] { time });
        }

        /// <summary>
        /// Uses the .NET speech synthesiser to speak a sentence.
        /// </summary>
        /// <param name="sentence">Sentence to speak</param>
        public void Say(string sentence)
        {
            if (this.IsLinux())
            {
                throw new Exception("Voice synthesis isn't supported by mono.");
            }

            Assembly assembly = Assembly.LoadFrom("Speech.dll");
            System.Type speech = assembly.GetType("Speech.Speech");
            object speechobj = Activator.CreateInstance(speech);
            speech.InvokeMember(
                                "Say", 
                                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                                null, 
                                speechobj, 
                                new object[] { sentence, 1, 100 });
        }

        /// <summary>
        /// Uses the .NET speech synthesiser to speak a sentence.
        /// </summary>
        /// <param name="sentence">Sentence to speek</param>
        /// <param name="rate">Rate of speech</param>
        /// <param name="volume">Volume of speech</param>
        public void Say(string sentence, int rate, int volume)
        {
            if (this.IsLinux())
            {
                throw new Exception("Voice synthesis isn't supported by mono.");
            }

            Assembly assembly = Assembly.LoadFrom("Speech.dll");
            System.Type speech = assembly.GetType("Speech.Speech");
            object speechobj = Activator.CreateInstance(speech);
            speech.InvokeMember(
                                "Say", 
                                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                                null, 
                                speechobj, 
                                new object[] { sentence, rate, volume });
        }

        /// <summary>
        /// Returns a string representing the tree.
        /// </summary>
        /// <param name="tree">Tree to print</param>
        /// <returns>String representing the tree</returns>
        public string PrintTree(Trees.Absyn.Tree tree)
        {
            if (tree is Trees.Absyn.Lambda)
            {
                var t = (Trees.Absyn.Lambda)tree;
                return "Lambda " + t.Ident_ + "(" + this.PrintTree(t.Tree_) + ")";
            }
            else if (tree is Trees.Absyn.Variable)
            {
                var t = (Trees.Absyn.Variable)tree;
                return "Variable " + t.Integer_;
            }
            else if (tree is Trees.Absyn.Application)
            {
                var t = (Trees.Absyn.Application)tree;
                return "Application (" + this.PrintTree(t.Tree_1) + ") (" + this.PrintTree(t.Tree_2) + ")";
            }
            else if (tree is Trees.Absyn.Literal)
            {
                var t = (Trees.Absyn.Literal)tree;
                if (t.Lit_ is Trees.Absyn.IntLiteral)
                {
                    var t2 = (Trees.Absyn.IntLiteral)t.Lit_;
                    return "IntLiteral " + t2.Integer_;
                }
                else if (t.Lit_ is Trees.Absyn.FloatLiteral)
                {
                    var t2 = (Trees.Absyn.FloatLiteral)t.Lit_;
                    return "FloatLiteral " + t2.Double_;
                }
                else if (t.Lit_ is Trees.Absyn.StringLiteral)
                {
                    var t2 = (Trees.Absyn.StringLiteral)t.Lit_;
                    return "FloatLiteral " + t2.String_;
                }
                else
                {
                    throw new Exception("Unknown literal");
                }
            }
            else if (tree is Trees.Absyn.MetaVariable)
            {
                var t = (Trees.Absyn.MetaVariable)tree;
                return "MetaVariable " + t.Integer_;
            }
            else if (tree is Trees.Absyn.Function)
            {
                var t = (Trees.Absyn.Function)tree;
                return "Function " + t.Ident_;
            }
            else
            {
                throw new Exception("Fail");
            }
        }
    }
}
