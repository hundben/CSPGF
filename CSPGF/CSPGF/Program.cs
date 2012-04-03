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
            var at = new AdvancedTranslator("..\\..\\pgf examples\\ABC.pgf");
            at.SetInputLanguage("ABCCnc");
            at.Scan("a");
            at.Scan("b");
            at.Scan("c");

            // at.Scan("this");
            // at.Scan("wine");
            // at.Scan("is");
            // at.Scan("delicious");
            at.SetOutputLanguage("ABCCnc");
            Console.WriteLine(at.Translate());
            Console.WriteLine(at.PrintTree(at.GetTrees()[0]));
            Console.ReadKey();
        }
    }
}