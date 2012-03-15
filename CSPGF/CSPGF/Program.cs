// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace CSPGF
{
    class Program
    {
        static void Main(string[] args)
        {
            // Change this path to where your pgf file resides.
            AdvancedTranslator at = new AdvancedTranslator("..\\..\\pgf examples\\Phrasebook.pgf");

            Console.WriteLine(" --- Supported languages ---");
            /*foreach (string lang in at.GetLanguages())
            {
                Console.WriteLine(lang);
            }*/

            at.SetInputLanguage("PhrasebookEng");

            at.Scan("this");

            // Predict valid continuations of the sentence.
            /*Console.WriteLine(" --- Prediction ---");
            foreach (String token in at.Predict())
            {
                Console.WriteLine(token);
            }*/

            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");

            /*Console.WriteLine(" --- Translation ---");

            at.SetOutputLanguage("PhrasebookIta");
            Console.WriteLine(at.Translate());*/

            System.Console.WriteLine(at.PrintTree(at.GetTrees()[0]));

            // Wait for a keypress.
            Console.ReadKey();
        }
    }
}