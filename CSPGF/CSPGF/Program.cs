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
            PGF pgf = new Grammar.PGFReader("..\\..\\pgf examples\\ABC.pgf").ReadPGF();
            ParseState ps = new ParseState(pgf.GetConcrete("ABCCnc"));
            ps.Scan("a");
            ps.Scan("b");
            ps.Scan("c");
            
            Lin.Lin lin = new Lin.Lin(pgf, pgf.GetConcrete("ABCCnc"));
            lin.Linearize(ps.GetTrees()[0]);
            
            /*var at = new AdvancedTranslator("..\\..\\pgf examples\\ABC.pgf");
            at.SetInputLanguage("ABCCnc");
            at.Scan("a");
            at.Scan("b");
            at.Scan("c");

            at.SetOutputLanguage("ABCCnc");
            Console.WriteLine(at.Translate());

            System.Console.WriteLine(at.PrintTree(at.GetTrees()[0]));*/

            // Wait for a keypress.
            Console.ReadKey();
        }
    }
}