
Grammatical framework runtime for C#
====================================

CSPGF is an implementation of the [Grammatical Framework][1] runtime i C#.

Features
--------
* All the features from [JPGF][2].
* Works in both mono and .NET.
* Literal categories

Known Issues
------------
We are currently fixing these.

* Predict does not work
* Recovery does not work at the moment
* Wrong tree-structure with literals inside of linearizer (LeafKS instead of Literal). Should not matter for normal use.
* No support for pgf 2.x

Missing features
----------------
The missing features below will most likely never be implemented.

* High-order syntax
* Dependent types
* External functions

How to build
------------
Open the project file in Visual Studio 2010 (or newer) or in Mono Develop and build it there.

How to use
----------
Look at the Tests in the UnitTests folder for some examples.

[1]: http://www.grammaticalframework.org/	"Grammatical Framework"
[2]: https://github.com/gdetrez/JPGF		"JPGF"
