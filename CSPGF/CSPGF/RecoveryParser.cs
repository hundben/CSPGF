// -----------------------------------------------------------------------
// <copyright file="RecoveryParser.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public RecoveryParser(PGF pgf, Concrete language)
        {
            this.language = language;
            this.startcat = pgf.GetAbstract().StartCat();
            parseStates = new Stack<ParseState>();
            ParseState ps = new ParseState(this.language);
            parseStates.Push(ps);
        }

        public RecoveryParser(PGF pgf, String language) : this(pgf, pgf.GetConcrete(language))
        {
        }

        /// <summary>
        /// Scan one token.
        /// </summary>
        /// <param name="token">The next token</param>
        /// <returns>True if scan was successful.</returns>
        public bool Scan(string token)
        {
            if (parseStates.Count == 0)
            {
                parseStates.Push(new ParseState(this.language));
            }

            ParseState ps = parseStates.Peek();
            ParseState copy = ObjectCopier.Clone<ParseState>(ps);
            bool result = copy.Scan(token);
            if (!result)
            {
                return false;
            }

            //if scan is successful store the copy and return true
            parseStates.Push(copy);
            return true;
        }

        /// <summary>
        /// Removes one token
        /// </summary>
        /// <returns>True if one is removed.</returns>
        public bool RemoveOne()
        {
            if (parseStates.Count > 0)
            {
                parseStates.Pop();
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
            ParseState ps = parseStates.Peek();
            return ps.Predict();
        }

        public void debug()
        {
            Console.WriteLine("Prediction");
            foreach (string tok in this.Predict())
            {
                Console.WriteLine(tok);
            }
        }
    }
}
