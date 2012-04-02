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
            // Change this path to where your pgf file resides.
            AdvancedTranslator at = new AdvancedTranslator("..\\..\\pgf examples\\ABC.pgf");

            Console.WriteLine(" --- Supported languages ---");
            foreach (string lang in at.GetLanguages())
            {
                Console.WriteLine(lang);
            }

            at.SetInputLanguage("ABCCnc");
            // at.ScanTokens("str ( a )");

            //at.Scan("1.2");

            at.Scan("a"); 
            at.Scan("b");
            at.Scan("c");

            // Predict valid continuations of the sentence.
            /*Console.WriteLine(" --- Prediction ---");
            foreach (String token in at.Predict())
            {
                Console.WriteLine(token);
            }*/


            Console.WriteLine(" --- Translation ---");

            at.SetOutputLanguage("ABCCnc");
            Console.WriteLine(at.Translate());

            System.Console.WriteLine(at.PrintTree(at.GetTrees()[0]));

            // Wait for a keypress.
            Console.ReadKey();
        }
    }
}