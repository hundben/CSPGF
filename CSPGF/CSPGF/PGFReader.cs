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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;
using System.IO;

namespace CSPGF
{
    class PGFReader
    {

        private static Boolean DBG = true;

        private StreamWriter dbgwrite;
        private BinaryReader inputstream; // Maybe filestream or streamreader instead?
        //private DataInputStream mDataInputStream;
        // Was Set<String> before, but does not exist in c#
        private List<String> languages;

        // Visual Studio fails to check the rest of the code when the two constructors are uncommented.
        public PGFReader(BinaryReader _inputstream)
        {
            if (DBG) {
                dbgwrite = new StreamWriter("./dbg.txt", false);
            }
            inputstream = _inputstream;
            //this.mDataInputStream = new DataInputStream(is);
        }

        public PGFReader(BinaryReader _inputstream, List<String> _languages)
        {
            if (DBG) {
                dbgwrite = new StreamWriter("./dbg.txt", false, Encoding.UTF8);
            }
            inputstream = _inputstream;
            //this.mDataInputStream = new DataInputStream(is);
            //TODO: Convert from String[] to List<String>
            //languages = _languages;
            languages = _languages;
            
        }

        public PGF ReadPGF()
        {
            Dictionary<String, int> index = null;
            // Reading the PGF version
            int[] ii = new int[2];
            for (int i = 0 ; i < 2 ; i++) {
                //ii[i] = mDataInputStream.read();
                // TODO: Specification says int16 for version number so maybe 2 bytes added together instead?
                int tmp = inputstream.ReadByte();
                tmp = tmp << 8;
                tmp = tmp | inputstream.ReadByte(); 
                ii[i] = tmp;
            }
            if (DBG) {
                dbgwrite.WriteLine("PGF version : " + ii[0] + "." + ii[1]);
            }
            // Reading the global flags
            Dictionary<String, RLiteral> flags = GetListFlag();
            if (flags.ContainsKey("index")) {
                index = ReadIndex(((StringLit)flags["index"]).value);
                if (DBG) {
                    foreach (KeyValuePair<String, int> kp in index) {
                        dbgwrite.WriteLine(kp.Key + ", " + kp.Value);
                    }
                }
            }
            // Reading the abstract
            Abstract abs = GetAbstract();
            String startCat = abs.StartCat();
            // Reading the concrete grammars
            int nbConcretes = GetInt();
            Concrete[] concretes;
            if (languages != null) {
                concretes = new Concrete[languages.Count()];
            } else {
                concretes = new Concrete[nbConcretes];
            }
            int k = 0;
            for (int i = 0 ; i < nbConcretes ; i++) {
                String name = GetIdent();
                if (DBG) {
                    dbgwrite.WriteLine("Language " + name);
                }
                if (languages == null || languages.Remove(name)) {
                    concretes[k] = GetConcrete(name, startCat);
                    k++;
                } else {
                    if (index != null) {
                        // TODO: CHECK! Maybe this will work?
                        inputstream.BaseStream.Seek(index[name], SeekOrigin.Current);
                        //this.mDataInputStream.skip(index[name]);
                        if (DBG) {
                            dbgwrite.WriteLine("Skiping " + name);
                        }
                    } else {
                        GetConcrete(name, startCat);
                    }
                }
            }
            // test that we actually found all the selected languages
            if (languages != null && languages.Count() > 0) {
                foreach (String l in languages) {
                    throw new UnknownLanguageException(l);
                }
            }

            // builds and returns the pgf object.
            PGF pgf = new PGF(ii[0], ii[1], flags, abs, concretes);
            inputstream.Close();
            return pgf;
        }

        /**
         * This function guess the default start category from the
         * PGF flags: if the startcat flag is set then it is taken as default cat.
         * otherwise "Sentence" is taken as default category.
         */
        private String GetStartCat(Dictionary<String, RLiteral> flags)
        {
            RLiteral cat = flags["startcat"];
            if (cat == null) {
                return "Sentence";
            } else {
                return ((StringLit)cat).value;
            }
        }

        private Dictionary<String, int> ReadIndex(String str)
        {
            //TODO: check if new implementation works
            // Note: WTH did i think when i wrote this? :D
            // Split on '+' and trim? Chech what the input really is.
            //String[] items = s.Split(" +");
            String[] items = str.Split('+');
            Dictionary<String, int> index = new Dictionary<String, int>();
            foreach (String item in items) {
                String[] i = item.Split(':');
                index.Add(i[0], Int32.Parse(i[1]));
            }
            return index;
        }

        /* ************************************************* */
        /* Reading abstract grammar                          */
        /* ************************************************* */
        /**
         * This function reads the part of the pgf binary corresponding to
         * the abstract grammar.
         * @param is the stream to read from.
         */

        private Abstract GetAbstract()
        {
            String name = GetIdent();
            if (DBG) {
                dbgwrite.WriteLine("Abstract syntax [" + name + "]");
            }
            Dictionary<String, RLiteral> flags = GetListFlag();
            List<AbsFun> absFuns = GetListAbsFun();
            List<AbsCat> absCats = GetListAbsCat();
            return new Abstract(name, flags, absFuns, absCats);
        }

        private List<Pattern> GetListPattern()
        {
            int npoz = GetInt();
            List<Pattern> tmp = new List<Pattern>();
            //Pattern[] patts = new Pattern[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetPattern());
            }
            return tmp;
        }

        private Eq GetEq()
        {
            List<Pattern> patts = GetListPattern();
            Expr exp = GetExpr();
            return new Eq(patts, exp);
        }

        private AbsFun GetAbsFun()
        {
            String name = GetIdent();
            if (DBG) {
                dbgwrite.WriteLine("AbsFun: '" + name + "'");
            }
            CSPGF.reader.Type t = GetType2();
            int i = GetInt();
            //TODO: Check!
            int has_equations = inputstream.ReadByte();
            List<Eq> equations;
            if (has_equations == 0) {
                equations = new List<Eq>();
            } else {
                equations = GetListEq();
            }
            double weight = GetDouble();
            AbsFun f = new AbsFun(name, t, i, equations, weight);
            if (DBG) {
                dbgwrite.WriteLine("/AbsFun: " + f);
            }
            return f;
        }

        private AbsCat GetAbsCat()
        {
            String name = GetIdent();
            List<Hypo> hypos = GetListHypo();
            List<WeightedIdent> functions = GetListWeightedIdent();
            AbsCat abcC = new AbsCat(name, hypos, functions);
            return abcC;
        }

        private List<AbsFun> GetListAbsFun()
        {
            int npoz = GetInt();
            List<AbsFun> tmp = new List<AbsFun>();
            //AbsFun[] absFuns = new AbsFun[npoz];
            if (npoz == 0) {
                return tmp;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    tmp.Add(GetAbsFun());
                    //absFuns[i] = GetAbsFun();
                }
            }
            return tmp;
        }

        private List<AbsCat> GetListAbsCat()
        {
            int npoz = GetInt();
            List<AbsCat> tmp = new List<AbsCat>();
            //AbsCat[] absCats = new AbsCat[npoz];
            if (npoz == 0) {
                return tmp;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    tmp.Add(GetAbsCat());
                    //absCats[i] = GetAbsCat();
                }
            }
            return tmp;
        }

        private CSPGF.reader.Type GetType2()
        {
            List<Hypo> hypos = GetListHypo();
            String returnCat = GetIdent();
            List<Expr> exprs = GetListExpr();
            CSPGF.reader.Type t = new CSPGF.reader.Type(hypos, returnCat, exprs);
            if (DBG) {
                dbgwrite.WriteLine("Type: " + t);
            }
            return t;
        }

        private Hypo GetHypo()
        {
            //TODO: Check!
            int btype = inputstream.ReadByte();
            Boolean b = btype == 0 ? false : true;
            String varName = GetIdent();
            CSPGF.reader.Type t = GetType2();
            Hypo hh = new Hypo(b, varName, t);
            return hh;
        }

        private List<Hypo> GetListHypo()
        {
            int npoz = GetInt();
            List<Hypo> tmp = new List<Hypo>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetHypo());
            }
            return tmp;
        }

        private List<Expr> GetListExpr()
        {
            int npoz = GetInt();
            List<Expr> tmp = new List<Expr>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetExpr());
            }
            return tmp;
        }

        private Expr GetExpr()
        {
            //TODO: Check!
            int sel = inputstream.ReadByte();
            Expr expr = null;
            switch (sel) {
                case 0: //lambda abstraction
                    //TODO: Check!
                    int bt = inputstream.ReadByte();
                    Boolean btype = bt == 0 ? false : true;
                    String varName = GetIdent();
                    Expr e1 = GetExpr();
                    expr = new LambdaExp(btype, varName, e1);
                    break;
                case 1: //expression application
                    Expr e11 = GetExpr();
                    Expr e2 = GetExpr();
                    expr = new AppExp(e11, e2);
                    break;
                case 2: //literal expression
                    RLiteral lit = GetLiteral();
                    expr = new LiteralExp(lit);
                    break;
                case 3: //meta variable
                    int id = GetInt();
                    expr = new MetaExp(id);
                    break;
                case 4: //abstract function name
                    String absFun = GetIdent();
                    expr = new AbsNameExp(absFun);
                    break;
                case 5: //variable
                    int v = GetInt();
                    expr = new VarExp(v);
                    break;
                case 6: //type annotated expression
                    Expr e = GetExpr();
                    CSPGF.reader.Type t = GetType2();
                    expr = new TypedExp(e, t);
                    break;
                case 7: //implicit argument
                    Expr ee = GetExpr();
                    expr = new ImplExp(ee);
                    break;
                default:
                    throw new Exception("Invalid tag for expressions : " + sel);
            }
            return expr;
        }

        private List<Eq> GetListEq()
        {
            int npoz = GetInt();
            List<Eq> tmp = new List<Eq>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetEq());
            }
            return tmp;
        }

        private Pattern GetPattern()
        {
            //TODO: Check!
            int sel = inputstream.ReadByte();
            Pattern patt = null;
            switch (sel) {
                case 0: //application pattern
                    String absFun = GetIdent();
                    List<Pattern> patts = GetListPattern();
                    patt = new AppPattern(absFun, patts);
                    break;
                case 1: //variable pattern
                    String varName = GetIdent();
                    patt = new VarPattern(varName);
                    break;
                case 2: //variable as pattern
                    String pVarName = GetIdent();
                    Pattern p = GetPattern();
                    patt = new VarAsPattern(pVarName, p);
                    break;
                case 3: //wild card pattern
                    patt = new WildCardPattern();
                    break;
                case 4: //literal pattern
                    RLiteral lit = GetLiteral();
                    patt = new LiteralPattern(lit);
                    break;
                case 5: //implicit argument
                    Pattern pp = GetPattern();
                    patt = new ImpArgPattern(pp);
                    break;
                case 6: //inaccessible pattern
                    Expr e = GetExpr();
                    patt = new InaccPattern(e);
                    break;
                default:
                    throw new Exception("Invalid tag for patterns : " + sel);
            }
            return patt;
        }

        private RLiteral GetLiteral()
        {
            //TODO: CHECK!
            int sel = inputstream.ReadByte();
            RLiteral ss = null;
            switch (sel) {
                case 0:
                    String str = GetString();
                    ss = new StringLit(str);
                    break;
                case 1:
                    int i = GetInt();
                    ss = new IntLit(i);
                    break;
                case 2:
                    double d = GetDouble();
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
        private Concrete GetConcrete(String name, String startCat)
        {
            if (DBG) {
                dbgwrite.WriteLine("Concrete: " + name);
                dbgwrite.WriteLine("Concrete: Reading flags");
            }
            Dictionary<String, RLiteral> flags = GetListFlag();
            // We don't use the print names, but we need to read them to skip them
            if (DBG) {
                dbgwrite.WriteLine("Concrete: Skiping print names");
            }
            GetListPrintName();
            if (DBG) {
                dbgwrite.WriteLine("Concrete: Reading sequences");
            }
            List<Sequence> seqs = GetListSequence();
            List<CncFun> cncFuns = GetListCncFun(seqs);
            // We don't need the lindefs for now but again we need to
            // parse them to skip them
            GetListLinDef();
            List<ProductionSet> prods = GetListProductionSet(cncFuns);
            Dictionary<String, CncCat> cncCats = GetListCncCat();
            int i = GetInt();
            return new Concrete(name, flags, seqs, cncFuns, prods, cncCats, i, startCat);
        }

        /* ************************************************* */
        /* Reading print names                               */
        /* ************************************************* */
        // FIXME : not used, we should avoid creating the objects
        private PrintName GetPrintName()
        {
            String absName = GetIdent();
            String printName = GetString();
            return new PrintName(absName, printName);

        }

        private List<PrintName> GetListPrintName()
        {
            int npoz = GetInt();
            List<PrintName> tmp = new List<PrintName>();
            if (npoz == 0) {
                return tmp;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    tmp.Add(GetPrintName());
                }
            }
            return tmp;
        }

        /* ************************************************* */
        /* Reading sequences                                 */
        /* ************************************************* */
        private Sequence GetSequence()
        {
            List<Symbol> symbols = GetListSymbol();
            return new Sequence(symbols);
        }

        private List<Sequence> GetListSequence()
        {
            int npoz = GetInt();
            List<Sequence> tmp = new List<Sequence>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetSequence());
            }
            return tmp;
        }

        private Symbol GetSymbol()
        {
            //TODO: Check!
            int sel = inputstream.ReadByte();
            if (DBG) {
                dbgwrite.WriteLine("Symbol: type=" + sel);
            }
            Symbol symb = null;
            switch (sel) {
                case 0: // category (non terminal symbol)
                case 1: // Lit (Not implemented properly)
                    int i1 = GetInt();
                    int i2 = GetInt();
                    symb = new ArgConstSymbol(i1, i2);
                    break;
                case 2: // Variable (Not implemented)
                    //UnsupportedOperationException -> Exception
                    throw new Exception("Var symbols are not supported yet");
                case 3: //sequence of tokens
                    List<String> strs = GetListString();
                    symb = new ToksSymbol(strs);
                    break;
                case 4: //alternative tokens
                    List<String> altstrs = GetListString();
                    List<Alternative> la = GetListAlternative();
                    symb = new AlternToksSymbol(altstrs, la);
                    break;
                // IOException -> Exception
                default:
                    throw new Exception("Invalid tag for symbols : " + sel);
            }
            if (DBG) {
                dbgwrite.WriteLine("/Symbol: " + symb);
            }
            return symb;
        }

        private List<Alternative> GetListAlternative()
        {
            int npoz = GetInt();
            List<Alternative> tmp = new List<Alternative>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetAlternative());
            }
            return tmp;
        }

        private Alternative GetAlternative()
        {
            List<String> s1 = GetListString();
            List<String> s2 = GetListString();
            return new Alternative(s1, s2);
        }

        private List<Symbol> GetListSymbol()
        {
            int npoz = GetInt();
            List<Symbol> tmp = new List<Symbol>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetSymbol());
            }
            return tmp;
        }

        /* ************************************************* */
        /* Reading concrete functions                        */
        /* ************************************************* */
        private CncFun GetCncFun(List<Sequence> sequences)
        {
            String name = GetIdent();
            List<int> sIndices = GetListInt();
            //int l = sIndices.Count;
            List<Sequence> seqs = new List<Sequence>();
            foreach(int i in sIndices) {
                seqs.Add(sequences[i]);
            }
            /*for (int i = 0 ; i < l ; i++) {
                seqs[i] = sequences[sIndices[i]];
            }*/
            return new CncFun(name, seqs);
        }

        private List<CncFun> GetListCncFun(List<Sequence> sequences)
        {
            int npoz = GetInt();
            List<CncFun> tmp = new List<CncFun>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetCncFun(sequences));
            }
            return tmp;
        }

        /* ************************************************* */
        /* Reading LinDefs                                   */
        /* ************************************************* */
        // LinDefs are stored as an int map (Int -> [Int])

        private List<LinDef> GetListLinDef()
        {
            int size = GetInt();
            List<LinDef> tmp = new List<LinDef>();
            for (int i = 0 ; i < size ; i++)
                tmp.Add(GetLinDef());
            return tmp;
        }

        private LinDef GetLinDef()
        {
            int key = GetInt();
            int listSize = GetInt();
            List<int> funIds = new List<int>();
            for (int i = 0 ; i < listSize ; i++) {
                funIds.Add(GetInt());
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
            int id = GetInt();
            List<Production> prods = GetListProduction(id, cncFuns);
            ProductionSet ps = new ProductionSet(id, prods);
            return ps;
        }

        /**
         * Read a list of production set
         * @param is is the input stream to read from
         * @param cncFuns is the list of concrete function
         */
        private List<ProductionSet> GetListProductionSet(List<CncFun> cncFuns)
        {
            int npoz = GetInt();
            List<ProductionSet> tmp = new List<ProductionSet>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetProductionSet(cncFuns));
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
            int npoz = GetInt();
            List<Production> tmp = new List<Production>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetProduction(leftCat, cncFuns));
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
            //TODO: CHECK!
            int sel = inputstream.ReadByte();
            if (DBG) {
                dbgwrite.WriteLine("Production: type=" + sel);
            }
            Production prod = null;
            switch (sel) {
                case 0: //application
                    int i = GetInt();
                    List<int> domain = GetDomainFromPArgs();
                    prod = new ApplProduction(leftCat, cncFuns[i], domain);
                    break;
                case 1: //coercion
                    int id = GetInt();
                    prod = new CoerceProduction(leftCat, id);
                    break;
                //IOException -> Exception
                default:
                    throw new Exception("Invalid tag for productions : " + sel);
            }
            if (DBG) {
                dbgwrite.WriteLine("/Production: " + prod);
            }
            return prod;
        }

        // This function reads a list of PArgs (Productions arguments)
        // but returns only the actual domain (the category of the argumetns)
        // since we don't need the rest for now...
        private List<int> GetDomainFromPArgs()
        {
            int size = GetInt();
            List<int> tmp = new List<int>();
            for (int i = 0 ; i < size ; i++) {
                // Skiping the list of integers
                GetListInt();
                tmp.Add(GetInt());
            }
            return tmp;
        }

        /* ************************************************* */
        /* Reading concrete categories                       */
        /* ************************************************* */
        private CncCat GetCncCat()
        {
            String sname = GetIdent();
            int firstFId = GetInt();
            int lastFId = GetInt();
            List<String> ss = GetListString();
            return new CncCat(sname, firstFId, lastFId, ss);
        }

        private Dictionary<String, CncCat> GetListCncCat()
        {
            int npoz = GetInt();
            Dictionary<String, CncCat> cncCats = new Dictionary<String, CncCat>();
            String name;
            int firstFID, lastFID;
            List<String> ss;
            for (int i = 0 ; i < npoz ; i++) {
                name = GetIdent();
                firstFID = GetInt();
                lastFID = GetInt();
                ss = GetListString();
                //System.Console.WriteLine(name + " " + firstFID + " " + lastFID + " " + ss);
                cncCats.Add(name, new CncCat(name, firstFID, lastFID, ss));
            }
            return cncCats;
        }

        /* ************************************************* */
        /* Reading flags                                     */
        /* ************************************************* */
        private Dictionary<String, RLiteral> GetListFlag()
        {
            int npoz = GetInt();
            Dictionary<String, RLiteral> flags = new Dictionary<String, RLiteral>();
            if (npoz == 0) {
                return flags;
            }
            for (int i = 0 ; i < npoz ; i++) {
                String ss = GetIdent();
                RLiteral lit = GetLiteral();
                flags.Add(ss, lit);
            }
            return flags;
        }

        /* ************************************************* */
        /* Reading strings                                   */
        /* ************************************************* */
        private String GetString()
        {
            int npoz = GetInt();
            // Should work now
            List<char> bytes = new List<char>();
            foreach (char c in inputstream.ReadChars(npoz)) {
                bytes.Add(c);
            }
            return new String(bytes.ToArray());
        }

        private List<String> GetListString()
        {
            int npoz = GetInt();
            List<String> tmp = new List<String>();
            //String[] strs = new String[npoz];
            if (npoz == 0) {
                return tmp;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    tmp.Add(GetString());
                }
            }
            return tmp;
        }

        /**
         * Some string (like categories identifiers) are not allowed to
         * use the full utf8 tables but only latin 1 caracters.
         * We can read them faster using this knowledge.
         **/
        private String GetIdent()
        {
            int nbChar = GetInt();
            //byte[] bytes = new byte[nbChar];
            byte[] bytes = new byte[nbChar];
            //TODO: Check!
            inputstream.Read(bytes, 0, nbChar);
            //TODO: check if we have to change encoding!
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            return enc.GetString(bytes);
            //return bytes.ToString();
            //return new String(bytes, "ISO-8859-1");
        }

        private List<String> GetListIdent()
        {
            int nb = GetInt();
            List<String> tmp = new List<String>();
            for (int i = 0 ; i < nb ; i++) {
                tmp.Add(GetIdent());
            }
            return tmp;
        }


        // Weighted idents are a pair of a String (the ident) and a double
        // (the ident).
        private List<WeightedIdent> GetListWeightedIdent()
        {
            int nb = GetInt();
            List<WeightedIdent> tmp = new List<WeightedIdent>();
            for (int i = 0 ; i < nb ; i++) {
                double w = GetDouble();
                String s = GetIdent();
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
            //TODO: Check! WTF? Int räcker gott och väl!
            // long -> int
            int rez = inputstream.ReadByte();
            if (rez <= 0x7f) {
                return rez;
            } else {
                int ii = GetInt();
                rez = (ii << 7) | (rez & 0x7f);
                return (int)rez;
            }
        }

        private List<int> GetListInt()
        {
            int npoz = GetInt();
            List<int> tmp = new List<int>();
            for (int i = 0 ; i < npoz ; i++) {
                tmp.Add(GetInt());
            }
            return tmp;
        }

        // Reading doubles
        private double GetDouble()
        {
            return inputstream.ReadDouble();
        }
    }
}
    