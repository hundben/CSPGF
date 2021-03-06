
Grammatical framework runtime for C#
====================================

CSPGF is an implementation of the [Grammatical Framework][1] runtime i C#.

Features
--------
* All the features from [JPGF][2].
* Works in both mono and .NET.
* Literal categories
* Supports PGF 2.x (not the older 1.x)

Known Issues
------------
We are currently fixing these.

* Predict is only tested for simple grammars (might work but not tested)
* Wrong tree-structure with literals inside of linearizer (LeafKS instead of Literal). Should not matter for normal use.

Missing features
----------------
The missing features below will most likely never be implemented.

* High-order abstract syntax variables
* Type checking and inference
* Dependent types
* External functions
* Random syntax trees
* Linearization of incomplete trees

How to build
------------
Open the project file in Visual Studio 2010 (or newer) or in Mono Develop and build it there.

How to use
----------
Look at the Tests in the UnitTests folder for some examples.

[1]: http://www.grammaticalframework.org/	"Grammatical Framework"
[2]: https://github.com/gdetrez/JPGF		"JPGF"
