// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="N/A">
//   Hej hej
// </copyright>
// <summary>
//   Used for debugging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


// TODO: Släng in LinDefs och PrintNames i Concrete. Eller är det bara slöseri på minne?
namespace CSPGF
{
    using System;
    using System.Diagnostics;

    using CSPGF.Parse;

    /// <summary>
    /// Used for debugging.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Starting class.
        /// </summary>
        public static void Main()
        {
            /*PGF pgf = new Grammar.PGFReader("..\\..\\pgf examples\\ABC.pgf").ReadPGF();
            ParseState ps = new ParseState(pgf.GetConcrete("ABCCnc"));
            ps.Scan("a");
            ps.Scan("b");
            ps.Scan("c");
            
            Lin.Lin lin = new Lin.Lin(pgf, pgf.GetConcrete("ABCCnc"));
            lin.Linearize(ps.GetTrees()[0]);*/
            /*Stopwatch sw = new Stopwatch();
            sw.Start();
            var at = new AdvancedTranslator("..\\..\\pgf examples\\Phrasebook.pgf");
            at.SetInputLanguage("PhrasebookEng");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");
            at.SetOutputLanguage("PhrasebookSwe");
            Console.WriteLine(at.Translate());
            sw.Stop();*/

            // Console.WriteLine(at.PrintTree(at.GetTrees()[0]));
            // Console.WriteLine(sw.ElapsedMilliseconds);
            // Wait for a keypress.

            AdvancedTranslator at = new AdvancedTranslator("..\\..\\pgf examples\\ABC.pgf");
            at.SetInputLanguage("ABCCnc");
            at.Scan("a");
            at.Scan("b");
            at.Scan("c");
            at.SetOutputLanguage("ABCCnc");
            Console.WriteLine(at.Translate());
            Console.WriteLine(at.PrintTree(at.GetTrees()[0]));
            Console.ReadKey();
        }
    }
}