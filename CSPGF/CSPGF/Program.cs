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
            AdvancedTranslator at = new AdvancedTranslator("..\\..\\pgf examples\\MiniLit.pgf");
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

            Console.WriteLine(" --- Translation ---");
            at.SetOutputLanguage("MiniLitCnc");
            Console.ReadKey();
        }
    }
}