/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;

    class PGFReader
    {
        private static bool debug = false;

        private StreamWriter Dbgwrite;
        private BinaryReader Inputstream;
        private List<string> Languages = null;

        public PGFReader(BinaryReader inputstream)
        {
            if (debug) 
            {
                this.Dbgwrite = new StreamWriter("./dbg.txt", false);
            }

            this.Inputstream = inputstream;
        }

        public PGFReader(BinaryReader inputstream, List<string> languages)
        {
            if (debug) 
            {
                this.Dbgwrite = new StreamWriter("./dbg.txt", false);
            }

            this.Inputstream = inputstream;
            this.Languages = languages;
        }

        public PGF ReadPGF()
        {
            Dictionary<string, int> index = null;
            int[] ii = new int[2];
            for (int i = 0; i < 2; i++) 
            {
                int tmp = this.Inputstream.ReadByte();
                tmp = tmp << 8;
                tmp = tmp | this.Inputstream.ReadByte();
                ii[i] = tmp;
            }
            if (debug) 
            {
                this.Dbgwrite.WriteLine("PGF version : " + ii[0] + "." + ii[1]);
            }
            // Reading the global flags
            Dictionary<string, RLiteral> flags = this.GetListFlag();
            if (flags.ContainsKey("index")) 
            {
                index = this.ReadIndex(((StringLit)flags["index"]).value);
                if (debug) 
                {
                    foreach (KeyValuePair<string, int> kp in index) 
                    {
                        this.Dbgwrite.WriteLine(kp.Key + ", " + kp.Value);
                    }
                }
            }
            // Reading the abstract
            Abstract abs = this.GetAbstract();
            string startCat = abs.StartCat();
            // Reading the concrete grammars
            int nbConcretes = this.GetInt();
            Dictionary<string, Concrete> concretes = new Dictionary<string, Concrete>();
            for (int i = 0; i < nbConcretes; i++) 
            {
                string name = GetIdent();
                if (debug) {
                    this.Dbgwrite.WriteLine("Language " + name);
                }
                if (this.Languages == null || this.Languages.Remove(name)) 
                {
                    Concrete tmp = this.GetConcrete(name, startCat);
                    concretes.Add(tmp.Name, tmp);
                }
                else 
                {
                    if (index != null) 
                    {
                        // TODO: CHECK! Maybe this will work?
                        this.Inputstream.BaseStream.Seek(index[name], SeekOrigin.Current);
                        if (debug) 
                        {
                            this.Dbgwrite.WriteLine("Skipping " + name);
                        }
                    }
                    else 
                    {
                        this.GetConcrete(name, startCat);
                    }
                }
            }
            // test that we actually found all the selected languages
            if (this.Languages != null && this.Languages.Count() > 0) 
            {
                foreach (string l in this.Languages) 
                {
                    throw new UnknownLanguageException(l);
                }
            }

            // builds and returns the pgf object.
            PGF pgf = new PGF(ii[0], ii[1], flags, abs, concretes);
            this.Inputstream.Close();
            return pgf;
        }

        /**
         * This function guess the default start category from the
         * PGF flags: if the startcat flag is set then it is taken as default cat.
         * otherwise "Sentence" is taken as default category.
         */
        private string GetStartCat(Dictionary<string, RLiteral> flags)
        {
            RLiteral cat;
            if (!flags.TryGetValue("startcat", out cat)) 
            {
                return "Sentence";
            }
            else
            {
                return ((StringLit)cat).value;
            }
        }

        private Dictionary<string, int> ReadIndex(string str)
        {
            //Original javacode: String[] items = s.Split(" +");
            string[] items = str.Split('+');
            Dictionary<string, int> index = new Dictionary<string, int>();
            foreach (string item in items) 
            {
                string[] i = item.Split(':');
                index.Add(i[0].Trim(), int.Parse(i[1]));
            }
            return index;
        }

        /* ************************************************* */
        /* Reading abstract grammar                          */
        /* ************************************************* */
        private Abstract GetAbstract()
        {
            string name = this.GetIdent();
            if (debug) 
            {
                this.Dbgwrite.WriteLine("Abstract syntax [" + name + "]");
            }
            Dictionary<string, RLiteral> flags = this.GetListFlag();
            List<AbsFun> absFuns = this.GetListAbsFun();
            List<AbsCat> absCats = this.GetListAbsCat();
            return new Abstract(name, flags, absFuns, absCats);
        }

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

        private Eq GetEq()
        {
            List<Pattern> patts = this.GetListPattern();
            Expr exp = this.GetExpr();
            return new Eq(patts, exp);
        }

        private AbsFun GetAbsFun()
        {
            string name = this.GetIdent();
            if (debug)
            {
                this.Dbgwrite.WriteLine("AbsFun: '" + name + "'");
            }
            CSPGF.Reader.Type t = this.GetType2();
            int i = this.GetInt();
            int has_equations = this.Inputstream.ReadByte();
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
                this.Dbgwrite.WriteLine("/AbsFun: " + f);
            }
            return f;
        }

        private AbsCat GetAbsCat()
        {
            string name = this.GetIdent();
            List<Hypo> hypos = this.GetListHypo();
            List<WeightedIdent> functions = this.GetListWeightedIdent();
            return new AbsCat(name, hypos, functions);
        }

        private List<AbsFun> GetListAbsFun()
        {
            int npoz = this.GetInt();
            List<AbsFun> tmp = new List<AbsFun>();
            if (npoz == 0) 
            {
                return tmp;
            }
            else {
                for (int i = 0; i < npoz; i++) 
                {
                    tmp.Add(this.GetAbsFun());
                }
            }
            return tmp;
        }

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

        private CSPGF.Reader.Type GetType2()
        {
            List<Hypo> hypos = this.GetListHypo();
            string returnCat = this.GetIdent();
            List<Expr> exprs = this.GetListExpr();
            CSPGF.Reader.Type t = new CSPGF.Reader.Type(hypos, returnCat, exprs);
            if (debug) 
            {
                this.Dbgwrite.WriteLine("Type: " + t);
            }
            return t;
        }

        private Hypo GetHypo()
        {
            int btype = this.Inputstream.ReadByte();
            bool b = btype == 0 ? false : true;
            string varName = this.GetIdent();
            CSPGF.Reader.Type t = this.GetType2();
            return new Hypo(b, varName, t);
        }

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

        private Expr GetExpr()
        {
            int sel = this.Inputstream.ReadByte();
            Expr expr = null;
            switch (sel)
            {
                case 0: //lambda abstraction
                    int bt = this.Inputstream.ReadByte();
                    bool btype = bt == 0 ? false : true;
                    string varName = this.GetIdent();
                    Expr e1 = this.GetExpr();
                    expr = new LambdaExp(btype, varName, e1);
                    break;
                case 1: //expression application
                    Expr e11 = this.GetExpr();
                    Expr e2 = this.GetExpr();
                    expr = new AppExp(e11, e2);
                    break;
                case 2: //literal expression
                    RLiteral lit = this.GetLiteral();
                    expr = new LiteralExp(lit);
                    break;
                case 3: //meta variable
                    int id = this.GetInt();
                    expr = new MetaExp(id);
                    break;
                case 4: //abstract function name
                    string absFun = this.GetIdent();
                    expr = new AbsNameExp(absFun);
                    break;
                case 5: //variable
                    int v = this.GetInt();
                    expr = new VarExp(v);
                    break;
                case 6: //type annotated expression
                    Expr e = this.GetExpr();
                    CSPGF.Reader.Type t = this.GetType2();
                    expr = new TypedExp(e, t);
                    break;
                case 7: //implicit argument
                    Expr ee = this.GetExpr();
                    expr = new ImplExp(ee);
                    break;
                default:
                    throw new Exception("Invalid tag for expressions : " + sel);
            }

            return expr;
        }

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

        private Pattern GetPattern()
        {
            int sel = this.Inputstream.ReadByte();
            Pattern patt = null;
            switch (sel) 
            {
                case 0: //application pattern
                    string absFun = this.GetIdent();
                    List<Pattern> patts = this.GetListPattern();
                    patt = new AppPattern(absFun, patts);
                    break;
                case 1: //variable pattern
                    string varName = this.GetIdent();
                    patt = new VarPattern(varName);
                    break;
                case 2: //variable as pattern
                    string pVarName = this.GetIdent();
                    Pattern p = this.GetPattern();
                    patt = new VarAsPattern(pVarName, p);
                    break;
                case 3: //wild card pattern
                    patt = new WildCardPattern();
                    break;
                case 4: //literal pattern
                    RLiteral lit = this.GetLiteral();
                    patt = new LiteralPattern(lit);
                    break;
                case 5: //implicit argument
                    Pattern pp = this.GetPattern();
                    patt = new ImpArgPattern(pp);
                    break;
                case 6: //inaccessible pattern
                    Expr e = this.GetExpr();
                    patt = new InaccPattern(e);
                    break;
                default:
                    throw new Exception("Invalid tag for patterns : " + sel);
            }

            return patt;
        }

        private RLiteral GetLiteral()
        {
            int sel = this.Inputstream.ReadByte();
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

        /* ************************************************* */
        /* Reading concrete grammar                          */
        /* ************************************************* */
        private Concrete GetConcrete(string name, string startCat)
        {
            if (debug)
            {
                this.Dbgwrite.WriteLine("Concrete: " + name);
                this.Dbgwrite.WriteLine("Concrete: Reading flags");
            }
            Dictionary<string, RLiteral> flags = this.GetListFlag();
            // We don't use the print names, but we need to read them to skip them
            if (debug) 
            {
                this.Dbgwrite.WriteLine("Concrete: Skiping print names");
            }
            this.GetListPrintName();
            if (debug) 
            {
                this.Dbgwrite.WriteLine("Concrete: Reading sequences");
            }
            List<Sequence> seqs = this.GetListSequence();
            List<CncFun> cncFuns = this.GetListCncFun(seqs);
            // We don't need the lindefs for now but again we need to
            // parse them to skip them
            this.GetListLinDef();
            List<ProductionSet> prods = this.GetListProductionSet(cncFuns);
            Dictionary<string, CncCat> cncCats = this.GetListCncCat();
            int i = this.GetInt();
            return new Concrete(name, flags, seqs, cncFuns, prods, cncCats, i, startCat);
        }

        /* ************************************************* */
        /* Reading print names                               */
        /* ************************************************* */
        // FIXME : not used, we should avoid creating the objects
        private PrintName GetPrintName()
        {
            string absName = this.GetIdent();
            string printName = this.GetString();
            return new PrintName(absName, printName);
        }

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

        /* ************************************************* */
        /* Reading sequences                                 */
        /* ************************************************* */
        private Sequence GetSequence()
        {
            List<Symbol> symbols = this.GetListSymbol();
            return new Sequence(symbols);
        }

        private List<Sequence> GetListSequence()
        {
            int npoz = this.GetInt();
            List<Sequence> tmp = new List<Sequence>();
            for (int i = 0; i < npoz; i++)
            {
                tmp.Add(this.GetSequence());
            }
            return tmp;
        }

        private Symbol GetSymbol()
        {
            int sel = this.Inputstream.ReadByte();
            if (debug) 
            {
                this.Dbgwrite.WriteLine("Symbol: type=" + sel);
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
                    //UnsupportedOperationException -> Exception
                    throw new Exception("Var symbols are not supported yet");
                case 3: //sequence of tokens
                    List<string> strs = this.GetListString();
                    symb = new ToksSymbol(strs);
                    break;
                case 4: //alternative tokens
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
                this.Dbgwrite.WriteLine("/Symbol: " + symb);
            }

            return symb;
        }

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

        private Alternative GetAlternative()
        {
            List<string> s1 = this.GetListString();
            List<string> s2 = this.GetListString();
            return new Alternative(s1, s2);
        }

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

        /* ************************************************* */
        /* Reading concrete functions                        */
        /* ************************************************* */
        private CncFun GetCncFun(List<Sequence> sequences)
        {
            string name = this.GetIdent();
            List<int> sIndices = this.GetListInt();
            List<Sequence> seqs = new List<Sequence>();
            foreach (int i in sIndices) 
            {
                seqs.Add(sequences[i]);
            }

            return new CncFun(name, seqs);
        }

        private List<CncFun> GetListCncFun(List<Sequence> sequences)
        {
            int npoz = this.GetInt();
            List<CncFun> tmp = new List<CncFun>();
            for (int i = 0; i < npoz; i++) 
            {
                tmp.Add(this.GetCncFun(sequences));
            }

            return tmp;
        }

        /* ************************************************* */
        /* Reading LinDefs                                   */
        /* ************************************************* */
        // LinDefs are stored as an int map (Int -> [Int])

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

        /* ************************************************* */
        /* Reading productions and production sets           */
        /* ************************************************* */
        /**
         * Read a production set
         * @param is is the input stream to read from
         * @param cncFuns is the list of concrete function
         */
        private ProductionSet GetProductionSet(List<CncFun> cncFuns)
        {
            int id = this.GetInt();
            List<Production> prods = this.GetListProduction(id, cncFuns);
            return new ProductionSet(id, prods);
        }

        /**
         * Read a list of production set
         * @param is is the input stream to read from
         * @param cncFuns is the list of concrete function
         */
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

        /**
         * Read a list of production
         * @param is is the input stream to read from
         * @param leftCat is the left hand side category of this production (
         * read only once for the whole production set)
         * @param cncFuns is the list of concrete function
         */
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

        /**
         * Read a production
         * @param is is the input stream to read from
         * @param leftCat is the left hand side category of this production
         *                (read only once for the whole production set)
         * @param cncFuns is the list of concrete function, used here to set the
         *                function of the production (only given by its index in
         *                the list)
         */
        private Production GetProduction(int leftCat, List<CncFun> cncFuns)
        {
            int sel = this.Inputstream.ReadByte();
            if (debug)
            {
                this.Dbgwrite.WriteLine("Production: type=" + sel);
            }

            Production prod = null;
            switch (sel) 
            {
                case 0: //application
                    int i = this.GetInt();
                    List<int> domain = this.GetDomainFromPArgs();
                    prod = new ApplProduction(leftCat, cncFuns[i], domain);
                    break;
                case 1: //coercion
                    int id = this.GetInt();
                    prod = new CoerceProduction(leftCat, id);
                    break;
                //IOException -> Exception
                default:
                    throw new Exception("Invalid tag for productions : " + sel);
            }

            if (debug) 
            {
                this.Dbgwrite.WriteLine("/Production: " + prod);
            }

            return prod;
        }

        // This function reads a list of PArgs (Productions arguments)
        // but returns only the actual domain (the category of the argumetns)
        // since we don't need the rest for now...
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

        private CncCat GetCncCat()
        {
            string sname = this.GetIdent();
            int firstFId = this.GetInt();
            int lastFId = this.GetInt();
            List<string> ss = this.GetListString();
            return new CncCat(sname, firstFId, lastFId, ss);
        }

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

        private string GetString()
        {
            int npoz = this.GetInt();
            List<char> bytes = new List<char>();
            foreach (char c in this.Inputstream.ReadChars(npoz))
            {
                bytes.Add(c);
            }

            return new string(bytes.ToArray());
        }

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

        /**
         * Some string (like categories identifiers) are not allowed to
         * use the full utf8 tables but only latin 1 caracters.
         * We can read them faster using this knowledge.
         **/
        private string GetIdent()
        {
            int nbChar = this.GetInt();
            byte[] bytes = new byte[nbChar];
            this.Inputstream.Read(bytes, 0, nbChar);
            // TODO: check if we have to change encoding or let String fix it instead!
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
        }

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


        // Weighted idents are a pair of a String (the ident) and a double
        // (the ident).
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

        /* ************************************************* */
        /* Reading integers                                  */
        /* ************************************************* */
        // this reads a 'Int' in haskell serialized by the pgf serializer.
        // Those are srialized with a variable length (like some strings)
        // to gain space.
        private int GetInt()
        {
            // TODO: Check! WTF? Int räcker gott och väl!
            // long -> int
            int rez = this.Inputstream.ReadByte();
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

        private double GetDouble()
        {
            return this.Inputstream.ReadDouble();
        }
    }
}