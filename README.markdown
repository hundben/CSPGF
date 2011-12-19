
Grammatical framework runtime for C#
====================================

CSPGF is an implementation of the [Grammatical Framework][1] runtime i C#.

Features
--------
* All the features from [JPGF][2].
* Ability to remove tokens without parsing the whole sentence again.
* Works in both mono and .NET.

Missing features
----------------
* High-order syntax
* Literal categories
* Dependent types
* Immutable parse state

How to build
------------
Open the project file in Visual Studio 2010 or in Mono Develop 2.6 and build it there.

Example
-------
A small example using Phrasebook.pgf, from the pgf examples folder, that demonstrates the basic functionality of the library.

    using System;
    using CSPGF;

    namespace GF_Test
    {
        class Program
        {
            static void Main(string[] args)
            {
                // Change this path to where your pgf file resides.
                AdvancedTranslator at = new AdvancedTranslator("Phrasebook.pgf");

                Console.WriteLine(" --- Supported languages ---");
                foreach (string lang in at.GetLanguages())
                {
                    Console.WriteLine(lang);
                }

                at.SetInputLanguage("PhrasebookEng");

                at.Scan("this");
                
                // Predict valid continuations of the sentence.
                Console.WriteLine(" --- Prediction ---");
                foreach (String token in at.Predict())
                {
                    Console.WriteLine(token);
                }

                at.Scan("wine");
                at.Scan("is");
                at.Scan("delicious");

                Console.WriteLine(" --- Translation ---");

                at.SetOutputLanguage("PhrasebookIta");
                Console.WriteLine(at.Translate());
	    
                // Wait for a keypress.
                Console.ReadKey();
            }
        }
    }

[1]: http://www.grammaticalframework.org/	"Grammatical Framework"
[2]: https://github.com/gdetrez/JPGF		"JPGF"
