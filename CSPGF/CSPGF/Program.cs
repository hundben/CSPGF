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
            AdvancedTranslator at = new AdvancedTranslator("..\\..\\pgf examples\\Phrasebook.pgf");
            at.SetInputLanguage("PhrasebookEng");

            foreach (string s in at.Predict())
            {
                // Console.WriteLine(s + " ,");
            }

            //at.Scan("flt");
            //at.Scan("(");
            //at.Scan("1.2");
            //at.Scan(")");

            //at.Scan("a");
            //at.Scan("b");
            //at.Scan("c");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("Italian");
            at.RemoveToken();
            at.RemoveToken();
            at.RemoveToken();
            at.Scan("wine");
            at.Scan("is");
            at.Scan("Italian");
            at.Scan(".");

            Console.WriteLine(" --- Translation ---");
            at.SetOutputLanguage("PhrasebookSwe");
            Console.WriteLine(at.Translate());

            // Wait for a keypress.

            Console.ReadKey();
        }
    }
}