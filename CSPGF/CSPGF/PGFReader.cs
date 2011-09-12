//-----------------------------------------------------------------------
// <copyright file="PGFReader.cs" company="None">
//  Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), 
//   Erik Bergström (erktheorc@gmail.com) 
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//   * Redistributions of source code must retain the above copyright
//     notice, this list of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright
//     notice, this list of conditions and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//   * Neither the name of the &lt;organization&gt; nor the
//     names of its contributors may be used to endorse or promote products
//     derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &quot;AS IS&quot; AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL &lt;COPYRIGHT HOLDER&gt; BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
//-----------------------------------------------------------------------

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using CSPGF.Reader;

    /// <summary>
    /// Reads an PGF object
    /// </summary>
    internal class PGFReader
    {
        /// <summary>
        /// True if debuginformation should be written
        /// </summary>
        private static bool debug = false;

        /// <summary>
        /// Stream to write debuginformation to
        /// </summary>
        private StreamWriter dbgwrite;

        /// <summary>
        /// Stream to read from
        /// </summary>
        private BinaryReader inputstream;

        /// <summary>
        /// Desired languages
        /// </summary>
        private List<string> languages = null;

        /// <summary>
        /// Initializes a new instance of the PGFReader class.
        /// </summary>
        /// <param name="inputstream">Stream to read from</param>
        public PGFReader(BinaryReader inputstream)
        {
            this.inputstream = inputstream;
        }

        /// <summary>
        /// Initializes a new instance of the PGFReader class with the desired languages.
        /// </summary>
        /// <param name="inputstream">Stream to read from</param>
        /// <param name="languages">Desired languages</param>
        public PGFReader(BinaryReader inputstream, List<string> languages)
        {
            this.inputstream = inputstream;
            this.languages = languages;
        }

        /// <summary>
        /// Starts reading the PGF object
        /// </summary>
        /// <returns>PGF object</returns>
        public PGF ReadPGF()
        {
            if (debug)
            {
                this.dbgwrite = new StreamWriter("./dbg.txt", false);
            }

            Dictionary<string, int> index = null;
            int[] ii = new int[2];
            for (int i = 0; i < 2; i++) 
            {
                int tmp = this.inputstream.ReadByte();
                tmp = tmp << 8;
                tmp = tmp | this.inputstream.ReadByte();
                ii[i] = tmp;
            }

            if (debug) 
            {
                this.dbgwrite.WriteLine("PGF version : " + ii[0] + "." + ii[1]);
            }

            // Reading the global flags
            Dictionary<string, RLiteral> flags = this.GetListFlag();
            if (flags.ContainsKey("index")) 
            {
                index = this.ReadIndex(((StringLit)flags["index"]).Value);
                if (debug) 
                {
                    foreach (KeyValuePair<string, int> kp in index) 
                    {
                        this.dbgwrite.WriteLine(kp.Key + ", " + kp.Value);
                    }
                }
            }

            // Reading the abstract
            Abstract abs = this.GetAbstract();
            string startCat = abs.StartCat();

            // Reading the concrete grammars
            int numConcretes = this.GetInt();
            Dictionary<string, Concrete> concretes = new Dictionary<string, Concrete>();
            for (int i = 0; i < numConcretes; i++) 
            {
                string name = this.GetIdent();
                if (debug) 
                {
                    this.dbgwrite.WriteLine("Language " + name);
                }

                if (this.languages == null || this.languages.Remove(name)) 
                {
                    Concrete tmp = this.GetConcrete(name, startCat);
                    concretes.Add(tmp.Name, tmp);
                }
                else 
                {
                    if (index != null) 
                    {
                        // TODO: CHECK! Maybe this will work?
                        this.inputstream.BaseStream.Seek(index[name], SeekOrigin.Current);
                        if (debug) 
                        {
                            this.dbgwrite.WriteLine("Skipping " + name);
                        }
                    }
                    else 
                    {
                        this.GetConcrete(name, startCat);
                    }
                }
            }

            // test that we actually found all the selected languages
            if (this.languages != null && this.languages.Count > 0) 
            {
                foreach (string l in this.languages) 
                {
                    throw new UnknownLanguageException(l);
                }
            }

            // builds and returns the pgf object.
            PGF pgf = new PGF(ii[0], ii[1], flags, abs, concretes);
            return pgf;
        }

        /// <summary>
        /// This function guess the default start category from the
        /// PGF flags: if the startcat flag is set then it is taken as default cat.
        /// otherwise "Sentence" is taken as default category.
        /// </summary>
        /// <param name="flags">Flags to use</param>
        /// <returns>Returns the startingcategory</returns>
        private string GetStartCat(Dictionary<string, RLiteral> flags)
        {
            RLiteral cat;
            if (!flags.TryGetValue("startcat", out cat)) 
            {
                return "Sentence";
            }
            else
            {
                return ((StringLit)cat).Value;
            }
        }

        /// <summary>
        /// Reads the index
        /// </summary>
        /// <param name="str">String to read from</param>
        /// <returns>Dictionary with string/int pairs</returns>
        private Dictionary<string, int> ReadIndex(string str)
        {
            string[] items = str.Split('+');
            Dictionary<string, int> index = new Dictionary<string, int>();
            foreach (string item in items) 
            {
                string[] i = item.Split(':');
                index.Add(i[0].Trim(), int.Parse(i[1]));
            }

            return index;
        }

        /// <summary>
        /// Read the abstract grammar.
        /// </summary>
        /// <returns>Abstract grammar</returns>
        private Abstract GetAbstract()
        {
            string name = this.GetIdent();
            if (debug) 
            {
                this.dbgwrite.WriteLine("Abstract syntax [" + name + "]");
            }

            Dictionary<string, RLiteral> flags = this.GetListFlag();
            List<AbsFun> absFuns = this.GetListAbsFun();
            List<AbsCat> absCats = this.GetListAbsCat();
            return new Abstract(name, flags, absFuns, absCats);
        }

        /// <summary>
        /// Reads a list of patterns
        /// </summary>
        /// <returns>List of patterns</returns>
        private List<Pattern> GetListPattern()
        {
            int npoz = this.GetInt();
            List<Pattern> tmp = new List<Pattern>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetPattern());
            }

            return tmp;
        }

        /// <summary>
        /// Reads an Eq
        /// </summary>
        /// <returns>Returns the Eq</returns>
        private Eq GetEq()
        {
            List<Pattern> patts = this.GetListPattern();
            Expr exp = this.GetExpr();
            return new Eq(patts, exp);
        }

        /// <summary>
        /// Reads an AbsFun
        /// </summary>
        /// <returns>Returns the AbsFun</returns>
        private AbsFun GetAbsFun()
        {
            string name = this.GetIdent();
            if (debug)
            {
                this.dbgwrite.WriteLine("AbsFun: '" + name + "'");
            }

            CSPGF.Reader.Type t = this.GetType2();
            int i = this.GetInt();
            int has_equations = this.inputstream.ReadByte();
            List<Eq> equations;
            if (has_equations == 0)
            {
                equations = new List<Eq>();
            }
            else 
            {
                equations = this.GetListEq();
            }

            double weight = this.GetDouble();
            AbsFun f = new AbsFun(name, t, i, equations, weight);
            if (debug) 
            {
                this.dbgwrite.WriteLine("/AbsFun: " + f);
            }

            return f;
        }

        /// <summary>
        /// Reads an AbsCat
        /// </summary>
        /// <returns>Returns the AbsCat</returns>
        private AbsCat GetAbsCat()
        {
            string name = this.GetIdent();
            List<Hypo> hypos = this.GetListHypo();
            List<WeightedIdent> functions = this.GetListWeightedIdent();
            return new AbsCat(name, hypos, functions);
        }

        /// <summary>
        /// Reads an list of AbsFun
        /// </summary>
        /// <returns>List of AbsFun</returns>
        private List<AbsFun> GetListAbsFun()
        {
            int npoz = this.GetInt();
            List<AbsFun> tmp = new List<AbsFun>();
            if (npoz == 0) 
            {
                return tmp;
            }
            else
            {
                for (int i = 0; i < npoz; i++) 
                {
                    tmp.Add(this.GetAbsFun());
                }
            }

            return tmp;
        }

        /// <summary>
        /// Reads a list of AbsCat
        /// </summary>
        /// <returns>List of AbsCat</returns>
        private List<AbsCat> GetListAbsCat()
        {
            int npoz = this.GetInt();
            List<AbsCat> tmp = new List<AbsCat>();
            if (npoz == 0) 
            {
                return tmp;
            }
            else 
            {
                for (int i = 0; i < npoz; i++) 
                {
                    tmp.Add(this.GetAbsCat());
                }
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Type object
        /// </summary>
        /// <returns>Returns the Type object</returns>
        private CSPGF.Reader.Type GetType2()
        {
            List<Hypo> hypos = this.GetListHypo();
            string returnCat = this.GetIdent();
            List<Expr> exprs = this.GetListExpr();
            CSPGF.Reader.Type t = new CSPGF.Reader.Type(hypos, returnCat, exprs);
            if (debug) 
            {
                this.dbgwrite.WriteLine("Type: " + t);
            }

            return t;
        }

        /// <summary>
        /// Reads a Hypo
        /// </summary>
        /// <returns>Returns the Hypo</returns>
        private Hypo GetHypo()
        {
            int btype = this.inputstream.ReadByte();
            bool b = btype == 0 ? false : true;
            string varName = this.GetIdent();
            CSPGF.Reader.Type t = this.GetType2();
            return new Hypo(b, varName, t);
        }

        /// <summary>
        /// Reads a list of Hypos
        /// </summary>
        /// <returns>List of Hypos</returns>
        private List<Hypo> GetListHypo()
        {
            int npoz = this.GetInt();
            List<Hypo> tmp = new List<Hypo>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetHypo());
            }

            return tmp;
        }

        /// <summary>
        /// Reads a list of Exprs
        /// </summary>
        /// <returns>List of Exprs</returns>
        private List<Expr> GetListExpr()
        {
            int npoz = this.GetInt();
            List<Expr> tmp = new List<Expr>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetExpr());
            }

            return tmp;
        }

        /// <summary>
        /// Reads an Expr
        /// </summary>
        /// <returns>Returns the Expr</returns>
        private Expr GetExpr()
        {
            int sel = this.inputstream.ReadByte();
            Expr expr = null;
            switch (sel)
            {
                case 0: // lambda abstraction
                    int bt = this.inputstream.ReadByte();
                    bool btype = bt == 0 ? false : true;
                    string varName = this.GetIdent();
                    Expr e1 = this.GetExpr();
                    expr = new LambdaExp(btype, varName, e1);
                    break;
                case 1: // expression application
                    Expr e11 = this.GetExpr();
                    Expr e2 = this.GetExpr();
                    expr = new AppExp(e11, e2);
                    break;
                case 2: // literal expression
                    RLiteral lit = this.GetLiteral();
                    expr = new LiteralExp(lit);
                    break;
                case 3: // meta variable
                    int id = this.GetInt();
                    expr = new MetaExp(id);
                    break;
                case 4: // abstract function name
                    string absFun = this.GetIdent();
                    expr = new AbsNameExp(absFun);
                    break;
                case 5: // variable
                    int v = this.GetInt();
                    expr = new VarExp(v);
                    break;
                case 6: // type annotated expression
                    Expr e = this.GetExpr();
                    CSPGF.Reader.Type t = this.GetType2();
                    expr = new TypedExp(e, t);
                    break;
                case 7: // implicit argument
                    Expr ee = this.GetExpr();
                    expr = new ImplExp(ee);
                    break;
                default:
                    throw new Exception("Invalid tag for expressions : " + sel);
            }

            return expr;
        }

        /// <summary>
        /// Reads a list of Eqs
        /// </summary>
        /// <returns>List of Eqs</returns>
        private List<Eq> GetListEq()
        {
            int npoz = this.GetInt();
            List<Eq> tmp = new List<Eq>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetEq());
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Pattern
        /// </summary>
        /// <returns>Returns the Pattern</returns>
        private Pattern GetPattern()
        {
            int sel = this.inputstream.ReadByte();
            Pattern patt = null;
            switch (sel) 
            {
                case 0: // application pattern
                    string absFun = this.GetIdent();
                    List<Pattern> patts = this.GetListPattern();
                    patt = new AppPattern(absFun, patts);
                    break;
                case 1: // variable pattern
                    string varName = this.GetIdent();
                    patt = new VarPattern(varName);
                    break;
                case 2: // variable as pattern
                    string patternVarName = this.GetIdent();
                    Pattern p = this.GetPattern();
                    patt = new VarAsPattern(patternVarName, p);
                    break;
                case 3: // wild card pattern
                    patt = new WildCardPattern();
                    break;
                case 4: // literal pattern
                    RLiteral lit = this.GetLiteral();
                    patt = new LiteralPattern(lit);
                    break;
                case 5: // implicit argument
                    Pattern pp = this.GetPattern();
                    patt = new ImpArgPattern(pp);
                    break;
                case 6: // inaccessible pattern
                    Expr e = this.GetExpr();
                    patt = new InaccPattern(e);
                    break;
                default:
                    throw new Exception("Invalid tag for patterns : " + sel);
            }

            return patt;
        }

        /// <summary>
        /// Reads a RLiteral
        /// </summary>
        /// <returns>Returns the RLiteral</returns>
        private RLiteral GetLiteral()
        {
            int sel = this.inputstream.ReadByte();
            RLiteral ss = null;
            switch (sel) 
            {
                case 0:
                    string str = this.GetString();
                    ss = new StringLit(str);
                    break;
                case 1:
                    int i = this.GetInt();
                    ss = new IntLit(i);
                    break;
                case 2:
                    double d = this.GetDouble();
                    ss = new FloatLit(d);
                    break;
                default:
                    throw new Exception("Incorrect literal tag " + sel);
            }

            return ss;
        }

        /// <summary>
        /// Reads a Concrete grammar
        /// </summary>
        /// <param name="name">Name of the grammar</param>
        /// <param name="startCat">Start category</param>
        /// <returns>The concrete grammar</returns>
        private Concrete GetConcrete(string name, string startCat)
        {
            if (debug)
            {
                this.dbgwrite.WriteLine("Concrete: " + name);
                this.dbgwrite.WriteLine("Concrete: Reading flags");
            }

            Dictionary<string, RLiteral> flags = this.GetListFlag();

            // We don't use the print names, but we need to read them to skip them
            if (debug) 
            {
                this.dbgwrite.WriteLine("Concrete: Skiping print names");
            }

            this.GetListPrintName();
            if (debug) 
            {
                this.dbgwrite.WriteLine("Concrete: Reading sequences");
            }

            List<List<Symbol>> seqs = this.GetListSequence();
            List<CncFun> cncFuns = this.GetListCncFun(seqs);

            // We don't need the lindefs for now but again we need to
            // parse them to skip them
            this.GetListLinDef();
            List<ProductionSet> prods = this.GetListProductionSet(cncFuns);
            Dictionary<string, CncCat> cncCats = this.GetListCncCat();
            int i = this.GetInt();
            return new Concrete(name, flags, prods, cncCats, i, startCat);
        }

        /// <summary>
        /// Reads a PrintName
        /// </summary>
        /// <returns>Returns the PrintName</returns>
        private PrintName GetPrintName()
        {
            string absName = this.GetIdent();
            string printName = this.GetString();
            return new PrintName(absName, printName);
        }

        /// <summary>
        /// Reads a list of PrintNames
        /// </summary>
        /// <returns>List of PrintNames</returns>
        private List<PrintName> GetListPrintName()
        {
            int npoz = this.GetInt();
            List<PrintName> tmp = new List<PrintName>();
            if (npoz == 0) 
            {
                return tmp;
            }
            else
            {
                for (int i = 0; i < npoz; i++) 
                {
                    tmp.Add(this.GetPrintName());
                }
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Sequence
        /// </summary>
        /// <returns>Returns the Sequence</returns>
        private List<Symbol> GetSequence()
        {
            List<Symbol> symbols = this.GetListSymbol();
            return symbols;
        }

        /// <summary>
        /// Reads a list of Sequences
        /// </summary>
        /// <returns>List of Sequences</returns>
        private List<List<Symbol>> GetListSequence()
        {
            int npoz = this.GetInt();
            List<List<Symbol>> tmp = new List<List<Symbol>>();
            for (int i = 0; i < npoz; i++)
            {
                tmp.Add(this.GetSequence());
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Symbol
        /// </summary>
        /// <returns>Return the Symbol</returns>
        private Symbol GetSymbol()
        {
            int sel = this.inputstream.ReadByte();
            if (debug) 
            {
                this.dbgwrite.WriteLine("Symbol: type=" + sel);
            }

            Symbol symb = null;
            switch (sel) 
            {
                case 0: // category (non terminal symbol)

                case 1: // Lit (Not implemented properly)
                    int i1 = this.GetInt();
                    int i2 = this.GetInt();
                    symb = new ArgConstSymbol(i1, i2);
                    break;
                case 2: // Variable (Not implemented)
                    // UnsupportedOperationException -> Exception
                    throw new Exception("Var symbols are not supported yet");
                case 3: // sequence of tokens
                    List<string> strs = this.GetListString();
                    symb = new ToksSymbol(strs);
                    break;
                case 4: // alternative tokens
                    List<string> altstrs = this.GetListString();
                    List<Alternative> la = this.GetListAlternative();
                    symb = new AlternToksSymbol(altstrs, la);
                    break;

                // IOException -> Exception
                default:
                    throw new Exception("Invalid tag for symbols : " + sel);
            }

            if (debug) 
            {
                this.dbgwrite.WriteLine("/Symbol: " + symb);
            }

            return symb;
        }

        /// <summary>
        /// Reads a list of Alternatives
        /// </summary>
        /// <returns>List of Alternatives</returns>
        private List<Alternative> GetListAlternative()
        {
            int npoz = this.GetInt();
            List<Alternative> tmp = new List<Alternative>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetAlternative());
            }

            return tmp;
        }

        /// <summary>
        /// Reads an Alternative
        /// </summary>
        /// <returns>Returns the Alternative</returns>
        private Alternative GetAlternative()
        {
            List<string> s1 = this.GetListString();
            List<string> s2 = this.GetListString();
            return new Alternative(s1, s2);
        }

        /// <summary>
        /// Reads a list of Symbols
        /// </summary>
        /// <returns>List of Symbols</returns>
        private List<Symbol> GetListSymbol()
        {
            int npoz = this.GetInt();
            List<Symbol> tmp = new List<Symbol>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetSymbol());
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Concrete Function
        /// </summary>
        /// <param name="sequences">List of sequences</param>
        /// <returns>Returns the Concrete Function</returns>
        private CncFun GetCncFun(List<List<Symbol>> sequences)
        {
            string name = this.GetIdent();
            List<int> seqIndices = this.GetListInt();
            List<List<Symbol>> seqs = new List<List<Symbol>>();
            foreach (int i in seqIndices) 
            {
                seqs.Add(sequences[i]);
            }

            return new CncFun(name, seqs);
        }

        /// <summary>
        /// Read a list of Concrete Functions
        /// </summary>
        /// <param name="sequences">List of Sequences</param>
        /// <returns>List of Concrete Functions</returns>
        private List<CncFun> GetListCncFun(List<List<Symbol>> sequences)
        {
            int npoz = this.GetInt();
            List<CncFun> tmp = new List<CncFun>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetCncFun(sequences));
            }

            return tmp;
        }

        /// <summary>
        /// Reads a list of LinDefs
        /// </summary>
        /// <returns>List of LinDefs</returns>
        private List<LinDef> GetListLinDef()
        {
            int size = this.GetInt();
            List<LinDef> tmp = new List<LinDef>();
            for (int i = 0; i < size; i++) 
            {
                tmp.Add(this.GetLinDef());
            }

            return tmp;
        }

        /// <summary>
        /// Reads a LinDef
        /// </summary>
        /// <returns>Returns the LinDef</returns>
        private LinDef GetLinDef()
        {
            int key = this.GetInt();
            int listSize = this.GetInt();
            List<int> funIds = new List<int>();
            for (int i = 0; i < listSize; i++) 
            {
                funIds.Add(this.GetInt());
            }

            return new LinDef(key, funIds);
        }

        /// <summary>
        /// Reads a ProductionSet
        /// </summary>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>Returns the ProductionSet</returns>
        private ProductionSet GetProductionSet(List<CncFun> cncFuns)
        {
            int id = this.GetInt();
            List<Production> prods = this.GetListProduction(id, cncFuns);
            return new ProductionSet(id, prods);
        }

        /// <summary>
        /// Read a list of ProductionSets
        /// </summary>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>List of ProductionSets</returns>
        private List<ProductionSet> GetListProductionSet(List<CncFun> cncFuns)
        {
            int npoz = this.GetInt();
            List<ProductionSet> tmp = new List<ProductionSet>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetProductionSet(cncFuns));
            }

            return tmp;
        }

        /// <summary>
        /// Read a list of Productions.
        /// </summary>
        /// <param name="leftCat">Left hand side category</param>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>List of Productions</returns>
        private List<Production> GetListProduction(int leftCat, List<CncFun> cncFuns)
        {
            int npoz = this.GetInt();
            List<Production> tmp = new List<Production>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetProduction(leftCat, cncFuns));
            }

            return tmp;
        }

        /// <summary>
        /// Read a Production
        /// </summary>
        /// <param name="leftCat">Left hand side category</param>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>Returns the Production</returns>
        private Production GetProduction(int leftCat, List<CncFun> cncFuns)
        {
            int sel = this.inputstream.ReadByte();
            if (debug)
            {
                this.dbgwrite.WriteLine("Production: type=" + sel);
            }

            Production prod = null;
            switch (sel) 
            {
                case 0: // application
                    int i = this.GetInt();
                    List<int> domain = this.GetDomainFromPArgs();
                    prod = new ApplProduction(leftCat, cncFuns[i], domain);
                    break;
                case 1: // coercion
                    int id = this.GetInt();
                    prod = new CoerceProduction(leftCat, id);
                    break;

                // IOException -> Exception
                default:
                    throw new Exception("Invalid tag for productions : " + sel);
            }

            if (debug) 
            {
                this.dbgwrite.WriteLine("/Production: " + prod);
            }

            return prod;
        }

        /// <summary>
        /// Reads a list of PArgs and returns the domain
        /// </summary>
        /// <returns>Returns the domain</returns>
        private List<int> GetDomainFromPArgs()
        {
            int size = this.GetInt();
            List<int> tmp = new List<int>();
            for (int i = 0; i < size; i++)
            {
                // Skiping the list of integers
                this.GetListInt();
                tmp.Add(this.GetInt());
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Concrete Category
        /// </summary>
        /// <returns>Returns the Concrete Category</returns>
        private CncCat GetCncCat()
        {
            string sname = this.GetIdent();
            int firstFId = this.GetInt();
            int lastFId = this.GetInt();
            List<string> ss = this.GetListString();
            return new CncCat(sname, firstFId, lastFId, ss);
        }

        /// <summary>
        /// Read a list of Concrete Categories
        /// </summary>
        /// <returns>Dictionary of name/category</returns>
        private Dictionary<string, CncCat> GetListCncCat()
        {
            int npoz = this.GetInt();
            Dictionary<string, CncCat> cncCats = new Dictionary<string, CncCat>();
            string name;
            int firstFID, lastFID;
            List<string> ss;
            for (int i = 0; i < npoz; i++) 
            {
                name = this.GetIdent();
                firstFID = this.GetInt();
                lastFID = this.GetInt();
                ss = this.GetListString();
                cncCats.Add(name, new CncCat(name, firstFID, lastFID, ss));
            }

            return cncCats;
        }

        /// <summary>
        /// Reads a list of Flags
        /// </summary>
        /// <returns>Dictionary of string/literal</returns>
        private Dictionary<string, RLiteral> GetListFlag()
        {
            int npoz = this.GetInt();
            Dictionary<string, RLiteral> flags = new Dictionary<string, RLiteral>();
            if (npoz == 0) 
            {
                return flags;
            }

            for (int i = 0; i < npoz; i++) 
            {
                string ss = this.GetIdent();
                RLiteral lit = this.GetLiteral();
                flags.Add(ss, lit);
            }

            return flags;
        }

        /// <summary>
        /// Reads a string
        /// </summary>
        /// <returns>Returns the string</returns>
        private string GetString()
        {
            int npoz = this.GetInt();
            List<char> bytes = new List<char>();
            foreach (char c in this.inputstream.ReadChars(npoz))
            {
                bytes.Add(c);
            }

            return new string(bytes.ToArray());
        }

        /// <summary>
        /// Reads a list of strings
        /// </summary>
        /// <returns>List of strings</returns>
        private List<string> GetListString()
        {
            int npoz = this.GetInt();
            List<string> tmp = new List<string>();
            if (npoz == 0) 
            {
                return tmp;
            }
            else 
            {
                for (int i = 0; i < npoz; i++) 
                {
                    tmp.Add(this.GetString());
                }
            }

            return tmp;
        }

        /// <summary>
        /// Reads an identifier
        /// </summary>
        /// <returns>Returns the identifier</returns>
        private string GetIdent()
        {
            int numChar = this.GetInt();
            byte[] bytes = new byte[numChar];
            this.inputstream.Read(bytes, 0, numChar);

            // TODO: check if we have to change encoding or let String fix it instead!
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }

        /// <summary>
        /// Reads a list of identifiers
        /// </summary>
        /// <returns>List of identifiers</returns>
        private List<string> GetListIdent()
        {
            int nb = this.GetInt();
            List<string> tmp = new List<string>();
            for (int i = 0; i < nb; i++)
            {
                tmp.Add(this.GetIdent());
            }

            return tmp;
        }

        /// <summary>
        /// Reads a list of WeightedIdents
        /// </summary>
        /// <returns>List of WeightedIdents</returns>
        private List<WeightedIdent> GetListWeightedIdent()
        {
            int nb = this.GetInt();
            List<WeightedIdent> tmp = new List<WeightedIdent>();
            for (int i = 0; i < nb; i++)
            {
                double w = this.GetDouble();
                string s = this.GetIdent();
                tmp.Add(new WeightedIdent(s, w));
            }

            return tmp;
        }

        /// <summary>
        /// Reads an integer
        /// </summary>
        /// <returns>Returns the integer</returns>
        private int GetInt()
        {
            // long -> int
            int rez = this.inputstream.ReadByte();
            if (rez <= 0x7f) 
            {
                return rez;
            }
            else 
            {
                int ii = this.GetInt();
                rez = (ii << 7) | (rez & 0x7f);
                return (int)rez;
            }
        }

        /// <summary>
        /// Reads a list of integers
        /// </summary>
        /// <returns>List of integers</returns>
        private List<int> GetListInt()
        {
            int npoz = this.GetInt();
            List<int> tmp = new List<int>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetInt());
            }

            return tmp;
        }

        /// <summary>
        /// Read a double
        /// </summary>
        /// <returns>Returns the double</returns>
        private double GetDouble()
        {
            return this.inputstream.ReadDouble();
        }
    }
}