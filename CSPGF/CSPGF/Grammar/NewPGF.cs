//-----------------------------------------------------------------------
// <copyright file="NewPGF.cs" company="None">
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

namespace CSPGF.Grammar
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Reads an PGF object
    /// </summary>
    internal class NewPGF : IDisposable
    {
        /// <summary>
        /// Main input stream to read from
        /// </summary>
        private readonly MemoryStream inputstream;

        /// <summary>
        /// Binary stream used by this class
        /// </summary>
        private readonly BinaryReader binreader;

        /// <summary>
        /// Desired languages
        /// </summary>
        private readonly List<string> languages;

        /// <summary>
        /// If disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the NewPGF class.
        /// </summary>
        /// <param name="ms">MemoryStream for reading the PGF file</param>
        /// <param name="br">BinaryReader for reading the PGF file</param>
        /// <param name="lang">List of languages</param>
        public NewPGF(MemoryStream ms, BinaryReader br, List<string> lang)
        {
            this.inputstream = ms;
            this.binreader = br;
            this.languages = lang;
        }

        /// <summary>
        /// Finalizes an instance of the NewPGF class.
        /// </summary>
        ~NewPGF()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Implements disposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the actual disposing
        /// </summary>
        /// <param name="disposing">If disposing should be done or not</param>
        public void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    this.binreader.Close();
                    this.inputstream.Close();
                    this.binreader.Dispose();
                    this.inputstream.Dispose();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }

            this.disposed = true;
        }

        /// <summary>
        ///  Gets a list of flags in the PGF file. Will also modify the dictionary.
        /// </summary>
        /// <param name="index">Dictionary set to null.</param>
        /// <returns>Returns a list of flags for the PGF object.</returns>
        internal Dictionary<string, Literal> GetFlags(Dictionary<string, int> index)
        {
            // Reading the global flags
            Dictionary<string, Literal> flags = this.GetListFlag();
            if (flags.ContainsKey("index"))
            {
                index = this.ReadIndex(((LitString)flags["index"]).Value);
            }

            return flags;
        }

        /// <summary>
        /// Reads the concrete grammars from the PGF file.
        /// </summary>
        /// <param name="startCat">Starting category</param>
        /// <param name="index">Index to use to read the concrete grammars</param>
        /// <returns>Dictionary containing the concrete grammars</returns>
        internal Dictionary<string, Concrete> GetConcretes(string startCat, Dictionary<string, int> index)
        {
            int numConcretes = this.GetInt();
            Dictionary<string, Concrete> concretes = new Dictionary<string, Concrete>();
            for (int i = 0; i < numConcretes; i++)
            {
                string name = this.GetIdent();
                if (this.languages == null || this.languages.Remove(name))
                {
                    Concrete tmp = this.GetConcrete(name, startCat);
                    concretes.Add(tmp.Name, tmp);
                }
                else
                {
                    if (index != null)
                    {
                        this.inputstream.Seek(index[name], SeekOrigin.Current);
                    }
                    else
                    {
                        this.GetConcrete(name, startCat);
                    }
                }
            }

            return concretes;
        }

        /// <summary>
        /// Read the abstract grammar.
        /// </summary>
        /// <returns>Abstract grammar</returns>
        internal Abstract GetAbstract()
        {
            return new Abstract(this.GetIdent(), this.GetListFlag(), this.GetListAbsFun(), this.GetListAbsCat());
        }

        /// <summary>
        /// Reads a list of patterns
        /// </summary>
        /// <returns>List of patterns</returns>
        private Pattern[] GetListPattern()
        {
            int npoz = this.GetInt();
            Pattern[] patts = new Pattern[npoz];
            for (int i = 0; i < npoz; i++)
            {
                patts[i] = this.GetPattern();
            }

            return patts;
        }

        /*
        /// <summary>
        /// This function guess the default start category from the
        /// PGF flags: if the startcat flag is set then it is taken as default cat.
        /// otherwise "Sentence" is taken as default category.
        /// TODO: Isn't used at the moment!
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

            return ((StringLit)cat).Value;
        }*/

        /// <summary>
        /// Reads the index
        /// </summary>
        /// <param name="str">String to read from</param>
        /// <returns>Dictionary with (string,int) pairs</returns>
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
        /// Reads an Eq
        /// </summary>
        /// <returns>Returns the Eq</returns>
        private Equation GetEq()
        {
            return new Equation(this.GetListPattern(), this.GetExpr());
        }

        /// <summary>
        /// Reads an AbsFun
        /// </summary>
        /// <returns>Returns the AbsFun</returns>
        private AbsFun GetAbsFun()
        {
            return new AbsFun(this.GetIdent(), this.GetType2(), this.GetInt(), this.inputstream.ReadByte() == 0 ? new Equation[0] : this.GetListEq(), this.GetDouble());
        }

        /// <summary>
        /// Reads an AbsCat
        /// </summary>
        /// <returns>Returns the AbsCat</returns>
        private AbsCat GetAbsCat()
        {
            return new AbsCat(this.GetIdent(), this.GetListHypo(), this.GetListCatFun());
        }

        /// <summary>
        /// Reads an list of AbsFun
        /// </summary>
        /// <returns>List of AbsFun</returns>
        private AbsFun[] GetListAbsFun()
        {
            int npoz = this.GetInt();
            AbsFun[] tmp = new AbsFun[npoz];
            if (npoz == 0)
            {
                return tmp;
            }

            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetAbsFun();
            }

            return tmp;
        }

        /// <summary>
        /// Reads a list of AbsCat
        /// </summary>
        /// <returns>List of AbsCat</returns>
        private AbsCat[] GetListAbsCat()
        {
            int npoz = this.GetInt();
            AbsCat[] tmp = new AbsCat[npoz];
            if (npoz == 0)
            {
                return tmp;
            }

            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetAbsCat();
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Type object
        /// </summary>
        /// <returns>Returns the Type object</returns>
        private Type GetType2()
        {
            Type t = new Type(this.GetListHypo(), this.GetIdent(), this.GetListExpr());
            return t;
        }

        /// <summary>
        /// Reads a Hypo
        /// </summary>
        /// <returns>Returns the Hypo</returns>
        private Hypo GetHypo()
        {
            return new Hypo(this.inputstream.ReadByte() != 0, this.GetIdent(), this.GetType2());
        }

        /// <summary>
        /// Reads a list of Hypos
        /// </summary>
        /// <returns>List of Hypos</returns>
        private Hypo[] GetListHypo()
        {
            int npoz = this.GetInt();
            Hypo[] hypos = new Hypo[npoz];
            for (int i = 0; i < npoz; i++)
            {
                hypos[i] = this.GetHypo();
            }

            return hypos;
        }

        /// <summary>
        /// Reads a list of Exprs
        /// </summary>
        /// <returns>List of Exprs</returns>
        private Expr[] GetListExpr()
        {
            int npoz = this.GetInt();
            Expr[] exps = new Expr[npoz];
            for (int i = 0; i < npoz; i++)
            {
                exps[i] = this.GetExpr();
            }

            return exps;
        }

        /// <summary>
        /// Reads an Expr
        /// </summary>
        /// <returns>Returns the Expr</returns>
        private Expr GetExpr()
        {
            int sel = this.inputstream.ReadByte();
            Expr expr;
            switch (sel)
            {
                case 0: // lambda abstraction
                    int bt = this.inputstream.ReadByte();
                    bool btype = bt != 0;
                    expr = new ExprLambda(btype, this.GetIdent(), this.GetExpr());
                    break;
                case 1: // expression application
                    expr = new ExprApp(this.GetExpr(), this.GetExpr());
                    break;
                case 2: // literal expression
                    expr = new ExprLiteral(this.GetLiteral());
                    break;
                case 3: // meta variable
                    expr = new ExprMeta(this.GetInt());
                    break;
                case 4: // abstract function name
                    expr = new ExprFun(this.GetIdent());
                    break;
                case 5: // variable
                    expr = new ExprVar(this.GetInt());
                    break;
                case 6: // type annotated expression
                    expr = new ExprTyped(this.GetExpr(), this.GetType2());
                    break;
                case 7: // implicit argument
                    Expr ee = this.GetExpr();
                    expr = new ExprImpl(ee);
                    break;
                default:
                    throw new PGFException("Invalid tag for expressions : " + sel);
            }

            return expr;
        }

        /// <summary>
        /// Reads a list of Eqs
        /// </summary>
        /// <returns>List of Eqs</returns>
        private Equation[] GetListEq()
        {
            int npoz = this.GetInt();
            Equation[] eqs = new Equation[npoz];
            for (int i = 0; i < npoz; i++)
            {
                eqs[i] = this.GetEq();
            }

            return eqs;
        }

        /// <summary>
        /// Reads a Pattern
        /// </summary>
        /// <returns>Returns the Pattern</returns>
        private Pattern GetPattern()
        {
            int sel = this.inputstream.ReadByte();
            Pattern patt;
            switch (sel)
            {
                case 0: // application pattern
                    string absFun = this.GetIdent();
                    Pattern[] patts = this.GetListPattern();
                    patt = new AppPattern(absFun, patts);
                    break;
                case 1: // variable pattern
                    patt = new PatternVar(this.GetIdent());
                    break;
                case 2: // variable as pattern
                    patt = new PatternVariableAt(this.GetIdent(), this.GetPattern());
                    break;
                case 3: // wild card pattern
                    patt = new PatternWildCard();
                    break;
                case 4: // literal pattern
                    patt = new PatternLiteral(this.GetLiteral());
                    break;
                case 5: // implicit argument
                    patt = new PatternImplicit(this.GetPattern());
                    break;
                case 6: // inaccessible pattern
                    patt = new PatternTilde(this.GetExpr());
                    break;
                default:
                    throw new PGFException("Invalid tag for patterns : " + sel);
            }

            return patt;
        }

        /// <summary>
        /// Reads a RLiteral
        /// </summary>
        /// <returns>Returns the RLiteral</returns>
        private Literal GetLiteral()
        {
            int sel = this.inputstream.ReadByte();
            Literal ss;
            switch (sel)
            {
                case 0:
                    ss = new LitString(this.GetString());
                    break;
                case 1:
                    ss = new LitInt(this.GetInt());
                    break;
                case 2:
                    ss = new LitFloat(this.GetDouble());
                    break;
                default:
                    throw new PGFException("Incorrect literal tag " + sel);
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
            Dictionary<string, Literal> flags = this.GetListFlag();

            // We don't use the print names, but we need to read them to skip them
            this.GetListPrintName();
            Symbol[][] seqs = this.GetListSequence();
            CncFun[] cncFuns = this.GetListCncFun(seqs);

            // We don't need the lindefs for now but again we need to
            // parse them to skip them
            return new Concrete(name, flags, this.GetListLinDef(), this.GetListProductionSet(cncFuns), this.GetListCncCat(), this.GetInt(), startCat);
        }

        /// <summary>
        /// Reads a PrintName
        /// </summary>
        /// <returns>Returns the PrintName</returns>
        private PrintName GetPrintName()
        {
            return new PrintName(this.GetIdent(), this.GetString());
        }

        /// <summary>
        /// Reads a list of PrintNames
        /// </summary>
        /// <returns>List of PrintNames</returns>
        private PrintName[] GetListPrintName()
        {
            int npoz = this.GetInt();
            PrintName[] tmp = new PrintName[npoz];
            if (npoz == 0)
            {
                return tmp;
            }

            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetPrintName();
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Sequence
        /// </summary>
        /// <returns>Returns the Sequence</returns>
        private Symbol[] GetSequence()
        {
            return this.GetListSymbol();
        }

        /// <summary>
        /// Reads a list of Sequences
        /// </summary>
        /// <returns>List of Sequences</returns>
        private Symbol[][] GetListSequence()
        {
            int npoz = this.GetInt();
            Symbol[][] tmp = new Symbol[npoz][];
            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetSequence();
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
            Symbol symb;
            switch (sel)
            {
                case 0: // category (non terminal symbol)
                    symb = new SymbolCat(this.GetInt(), this.GetInt());
                    break;
                case 1: // Literal category
                    symb = new SymbolLit(this.GetInt(), this.GetInt());
                    break;
                case 2: // Variable
                    symb = new SymbolVar(this.GetInt(), this.GetInt());
                    break;
                case 3: // sequence of tokens
                    symb = new SymbolKS(this.GetListString());
                    break;
                case 4: // alternative tokens
                    symb = new SymbolKP(this.GetListString(), this.GetListAlternative());
                    break;
                default:
                    throw new PGFException("Invalid tag for symbols : " + sel);
            }

            return symb;
        }

        /// <summary>
        /// Reads a list of Alternatives
        /// </summary>
        /// <returns>List of Alternatives</returns>
        private Alternative[] GetListAlternative()
        {
            int npoz = this.GetInt();
            Alternative[] alts = new Alternative[npoz];
            for (int i = 0; i < npoz; i++)
            {
                alts[i] = this.GetAlternative();
            }

            return alts;
        }

        /// <summary>
        /// Reads an Alternative
        /// </summary>
        /// <returns>Returns the Alternative</returns>
        private Alternative GetAlternative()
        {
            return new Alternative(this.GetListString(), this.GetListString());
        }

        /// <summary>
        /// Reads a list of Symbols
        /// </summary>
        /// <returns>List of Symbols</returns>
        private Symbol[] GetListSymbol()
        {
            int npoz = this.GetInt();
            Symbol[] tmp = new Symbol[npoz];
            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetSymbol();
            }

            return tmp;
        }

        /// <summary>
        /// Reads a Concrete Function
        /// </summary>
        /// <param name="sequences">List of sequences</param>
        /// <returns>Returns the Concrete Function</returns>
        private CncFun GetCncFun(Symbol[][] sequences)
        {
            string name = this.GetIdent();
            int[] seqIndices = this.GetListInt();
            Symbol[][] seqs = new Symbol[seqIndices.Length][];
            for (int i = 0; i < seqIndices.Length; i++)
            {
                seqs[i] = sequences[seqIndices[i]];
            }

            return new CncFun(name, seqs);
        }

        /// <summary>
        /// Read a list of Concrete Functions
        /// </summary>
        /// <param name="sequences">List of Sequences</param>
        /// <returns>List of Concrete Functions</returns>
        private CncFun[] GetListCncFun(Symbol[][] sequences)
        {
            int npoz = this.GetInt();
            CncFun[] cncfuns = new CncFun[npoz];
            for (int i = 0; i < npoz; i++)
            {
                cncfuns[i] = this.GetCncFun(sequences);
            }

            return cncfuns;
        }

        /// <summary>
        /// Reads a list of LinDefs
        /// </summary>
        /// <returns>List of LinDefs</returns>
        private LinDef[] GetListLinDef()
        {
            int size = this.GetInt();
            LinDef[] tmp = new LinDef[size];
            for (int i = 0; i < size; i++)
            {
                tmp[i] = this.GetLinDef();
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
            int[] funIds = new int[listSize];
            for (int i = 0; i < listSize; i++)
            {
                funIds[i] = this.GetInt();
            }

            return new LinDef(key, funIds);
        }

        /// <summary>
        /// Reads a ProductionSet
        /// </summary>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>Returns the ProductionSet</returns>
        private ProductionSet GetProductionSet(CncFun[] cncFuns)
        {
            int id = this.GetInt();
            return new ProductionSet(id, this.GetListProduction(id, cncFuns));
        }

        /// <summary>
        /// Read a list of ProductionSets
        /// </summary>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>List of ProductionSets</returns>
        private ProductionSet[] GetListProductionSet(CncFun[] cncFuns)
        {
            int npoz = this.GetInt();
            ProductionSet[] tmp = new ProductionSet[npoz];
            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetProductionSet(cncFuns);
            }

            return tmp;
        }

        /// <summary>
        /// Read a list of Productions.
        /// </summary>
        /// <param name="leftCat">Left hand side category</param>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>List of Productions</returns>
        private Production[] GetListProduction(int leftCat, CncFun[] cncFuns)
        {
            int npoz = this.GetInt();
            Production[] tmp = new Production[npoz];
            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetProduction(leftCat, cncFuns);
            }

            return tmp;
        }

        /// <summary>
        /// Read a Production
        /// </summary>
        /// <param name="leftCat">Left hand side category</param>
        /// <param name="cncFuns">List of Concrete Functions</param>
        /// <returns>Returns the Production</returns>
        private Production GetProduction(int leftCat, CncFun[] cncFuns)
        {
            int sel = this.inputstream.ReadByte();
            Production prod;
            switch (sel)
            {
                case 0: // application
                    prod = new ProductionApply(leftCat, cncFuns[this.GetInt()], this.GetDomainFromPArgs());
                    break;
                case 1: // coercion
                    prod = new ProductionCoerce(leftCat, this.GetInt());
                    break;
                default:
                    throw new PGFException("Invalid tag for productions : " + sel);
            }

            return prod;
        }

        /// <summary>
        /// Reads a list of PArgs and returns the domain
        /// </summary>
        /// <returns>Returns the domain</returns>
        private int[] GetDomainFromPArgs()
        {
            int size = this.GetInt();
            int[] tmp = new int[size];
            for (int i = 0; i < size; i++)
            {
                // Skiping the list of integers
                this.GetListInt();
                tmp[i] = this.GetInt();
            }

            return tmp;
        }

        /// <summary>
        /// Read a list of Concrete Categories
        /// </summary>
        /// <returns>Dictionary of name/category</returns>
        private Dictionary<string, CncCat> GetListCncCat()
        {
            int npoz = this.GetInt();
            Dictionary<string, CncCat> cncCats = new Dictionary<string, CncCat>();
            for (int i = 0; i < npoz; i++)
            {
                string name = this.GetIdent();
                cncCats.Add(name, new CncCat(name, this.GetInt(), this.GetInt(), this.GetListString()));
            }

            return cncCats;
        }

        /// <summary>
        /// Reads a list of Flags
        /// </summary>
        /// <returns>Dictionary of string/literal</returns>
        private Dictionary<string, Literal> GetListFlag()
        {
            int npoz = this.GetInt();
            Dictionary<string, Literal> flags = new Dictionary<string, Literal>();
            if (npoz == 0)
            {
                return flags;
            }

            for (int i = 0; i < npoz; i++)
            {
                flags.Add(this.GetIdent(), this.GetLiteral());
            }

            return flags;
        }

        /// <summary>
        /// Reads a string
        /// </summary>
        /// <returns>Returns the string</returns>
        private string GetString()
        {
            /*int npoz = this.GetInt();
            char[] bytes = new char[npoz];
            for (int i = 0; i < npoz; i++)
            {
                bytes[i] = this.binreader.ReadChar();
            }

            return new string(bytes);*/

            // This might work?
            return new string(this.binreader.ReadChars(this.GetInt()));
        }

        /// <summary>
        /// Reads a list of strings
        /// </summary>
        /// <returns>List of strings</returns>
        private string[] GetListString()
        {
            int npoz = this.GetInt();
            string[] strs = new string[npoz];
            if (npoz == 0)
            {
                return strs;
            }

            for (int i = 0; i < npoz; i++)
            {
                strs[i] = this.GetString();
            }

            return strs;
        }

        /// <summary>
        /// Reads an identifier
        /// </summary>
        /// <returns>Returns the identifier</returns>
        private string GetIdent()
        {
            /*
            int numChar = this.GetInt();
            byte[] bytes = new byte[numChar];
            this.inputstream.Read(bytes, 0, numChar);
            System.Text.Encoding enc = System.Text.Encoding.ASCII;  
            return enc.GetString(bytes);*/
            return System.Text.Encoding.UTF8.GetString(this.binreader.ReadBytes(this.GetInt()));
        }

        /*
        /// <summary>
        /// Reads a list of identifiers
        /// </summary>
        /// <returns>List of identifiers</returns>
        private string[] GetListIdent()
        {
            int nb = this.GetInt();
            string[] tmp = new string[nb];
            for (int i = 0; i < nb; i++)
            {
                tmp[i] = this.GetIdent();
            }

            return tmp;
        } */

        /// <summary>
        /// Reads a list of CatFuns
        /// </summary>
        /// <returns>List of CatFuns</returns>
        private CatFun[] GetListCatFun()
        {
            int nb = this.GetInt();
            CatFun[] wids = new CatFun[nb];
            for (int i = 0; i < nb; i++)
            {
                double w = this.GetDouble();
                string s = this.GetIdent();
                wids[i] = new CatFun(s, w);
            }

            return wids;
        }

        /// <summary>
        /// Reads an integer
        /// </summary>
        /// <returns>Returns the integer</returns>
        private int GetInt()
        {
            // return this.binreader.ReadInt32();
            // long -> int
            int rez = this.inputstream.ReadByte();
            if (rez <= 0x7f)
            {
                return rez;
            }

            int ii = this.GetInt();
            rez = (ii << 7) | (rez & 0x7f);
            return rez;
            //return (int)this.binreader.ReadUInt32();
        }

        /// <summary>
        /// Reads a list of integers
        /// </summary>
        /// <returns>List of integers</returns>
        private int[] GetListInt()
        {
            int npoz = this.GetInt();
            int[] tmp = new int[npoz];
            for (int i = 0; i < npoz; i++)
            {
                tmp[i] = this.GetInt();
            }

            return tmp;
        }

        /// <summary>
        /// Read a double
        /// </summary>
        /// <returns>Returns the double</returns>
        private double GetDouble()
        {
            return this.binreader.ReadDouble();
        }
    }
}