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
            // Change this path to where your pgf file resides.
            AdvancedTranslator at = new AdvancedTranslator("..\\..\\pgf examples\\MiniLit.pgf");

            Console.WriteLine(" --- Supported languages ---");
            foreach (string lang in at.GetLanguages())
            {
                Console.WriteLine(lang);
            }

            at.SetInputLanguage("MiniLitCnc");

            foreach (string s in at.Predict())
            {
                Console.WriteLine(s + " ,");
            }

            at.Scan("flt");
            at.Scan("(");
            at.Scan("1.2");
            at.Scan(")");

            //at.Scan("a");
            //at.Scan("b");
            //at.Scan("c");

            // Predict valid continuations of the sentence.
            /*Console.WriteLine(" --- Prediction ---");
            foreach (String token in at.Predict())
            {
                Console.WriteLine(token);
            }*/

            // Lin.Lin lin = new Lin.Lin(pgf, pgf.GetConcrete("ABCCnc"));
            // lin.Linearize(ps.GetTrees()[0]);

            Console.WriteLine(" --- Translation ---");

            at.SetOutputLanguage("MiniLitCnc");
            Console.WriteLine(at.Translate());

            // Wait for a keypress.
            Console.ReadKey();
        }
    }
}