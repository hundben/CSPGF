// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="N/A">
//   Hej hej
// </copyright>
// <summary>
//   Used for debugging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


// TODO: Släng in LinDefs och PrintNames i Concrete.
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
            var at = new AdvancedTranslator("..\\..\\pgf examples\\Phrasebook.pgf");
            at.SetInputLanguage("PhrasebookEng");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");
            at.SetOutputLanguage("PhrasebookGer");
            Console.WriteLine(at.Translate());
            Console.WriteLine(at.PrintTree(at.GetTrees()[0]));
            Console.ReadKey();
        }
    }
}