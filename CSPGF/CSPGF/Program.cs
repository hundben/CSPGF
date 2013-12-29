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
    using System.Threading;
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
            Thread.MemoryBarrier();
            long initialMemory = System.GC.GetTotalMemory(true);
            for (int i = 0; i < 10; i++)
            {

                /*
                AdvancedTranslator at = new AdvancedTranslator("../../pgf examples/Phrasebook.pgf");
                at.SetInputLanguage("PhrasebookEng");

                at.Scan("this");
                at.Scan("wine");
                at.Scan("is");
                at.Scan("delicious");
                at.Scan(".");
                Console.WriteLine(" --- Translation ---");
                at.SetOutputLanguage("PhrasebookEng");
                Console.WriteLine(at.Translate());
                */
                // Console.WriteLine(at.PrintTree(at.GetTrees()[0]));
                // Wait for a keypress.

                AdvancedTranslator at2 = new AdvancedTranslator("../../pgf examples/MiniLit.pgf");
                at2.SetInputLanguage("MiniLitCnc");
                at2.SetOutputLanguage("MiniLitCnc");
                at2.Scan("flt");
                at2.Scan("(");
                at2.Scan("1.2");
                at2.Scan(")");
                Console.WriteLine(at2.Translate());
            }
            
            Thread.MemoryBarrier();
            long finalMemory = System.GC.GetTotalMemory(true);
            Console.WriteLine(finalMemory - initialMemory);
            Console.ReadKey();
        }
    }
}