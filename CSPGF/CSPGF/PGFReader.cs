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

        private static Boolean DBG = false;


        private StreamReader inputstream; // Maybe filestream or streamreader instead?
        //private DataInputStream mDataInputStream;
        // Was Set<String> before, but does not exist in c#
        private List<String> languages;

        // Visual Studio fails to check the rest of the code when the two constructors are uncommented.
        public PGFReader(StreamReader _inputstream)
        {
            inputstream = _inputstream;
            //this.mDataInputStream = new DataInputStream(is);
        }

        public PGFReader(StreamReader _inputstream, String[] _languages)
        {
            inputstream = _inputstream;
            //this.mDataInputStream = new DataInputStream(is);
            //TODO: Convert from String[] to List<String>
            //languages = _languages;
            languages = _languages.ToList<String>();

        }

        public PGF ReadPGF()
        {
            Dictionary<String, int> index = null;
            // Reading the PGF version
            int[] ii = new int[4];
            for (int i = 0 ; i < 4 ; i++) {
                //ii[i] = mDataInputStream.read();
                // TODO: Reads one character at a time, so should be correct? Check this. Specification says int16 for version number so maybe 2 bytes added together instead?
                ii[i] = inputstream.Read();
            }
            if (DBG) {
                System.Console.WriteLine("PGF version : " + ii[0] + "." + ii[1] + "." + ii[2] + "." + ii[3]);
            }
            // Reading the global flags
            Dictionary<String, RLiteral> flags = GetListFlag();
            if (flags.ContainsKey("index")) {
                index = ReadIndex(((StringLit)flags["index"]).getValue());
                if (DBG) {
                    foreach (KeyValuePair<String, int> kp in index) {
                        System.Console.WriteLine(kp.Key + ", " + kp.Value);
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
                    System.Console.WriteLine("Language " + name);
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
                            System.Console.WriteLine("Skiping " + name);
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
            PGF pgf = new PGF(MakeInt16(ii[0], ii[1]), MakeInt16(ii[2], ii[3]), flags, abs, concretes);
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
                return ((StringLit)cat).getValue();
            }
        }

        private Dictionary<String, int> ReadIndex(String str)
        {
            //TODO: check if new implementation works
            // Note: WTH did i think when i wrote this? :D
            // Split on '+' and trim? Chech what the input really is.
            //String[] items = s.Split(" +");
            String[] items = str.Split(new Char[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
                System.Console.WriteLine("Abstract syntax [" + name + "]");
            }
            Dictionary<String, RLiteral> flags = GetListFlag();
            AbsFun[] absFuns = GetListAbsFun();
            AbsCat[] absCats = GetListAbsCat();
            return new Abstract(name, flags, absFuns, absCats);
        }

        private Pattern[] GetListPattern()
        {
            int npoz = GetInt();
            Pattern[] patts = new Pattern[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                patts[i] = GetPattern();
            }
            return patts;
        }

        private Eq GetEq()
        {
            Pattern[] patts = GetListPattern();
            Expr exp = GetExpr();
            return new Eq(patts, exp);
        }

        private AbsFun GetAbsFun()
        {
            String name = GetIdent();
            if (DBG) {
                System.Console.WriteLine("AbsFun: '" + name + "'");
            }
            CSPGF.reader.Type t = GetType();
            int i = GetInt();
            //TODO: Check!
            int has_equations = inputstream.Read();
            Eq[] equations;
            if (has_equations == 0) {
                equations = new Eq[0];
            } else {
                equations = GetListEq();
            }
            double weight = GetDouble();
            AbsFun f = new AbsFun(name, t, i, equations, weight);
            if (DBG) {
                System.Console.WriteLine("/AbsFun: " + f);
            }
            return f;
        }

        private AbsCat GetAbsCat()
        {
            String name = GetIdent();
            Hypo[] hypos = GetListHypo();
            WeightedIdent[] functions = GetListWeightedIdent();
            AbsCat abcC = new AbsCat(name, hypos, functions);
            return abcC;
        }

        private AbsFun[] GetListAbsFun()
        {
            int npoz = GetInt();
            AbsFun[] absFuns = new AbsFun[npoz];
            if (npoz == 0) {
                return absFuns;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    absFuns[i] = GetAbsFun();
                }
            }
            return absFuns;
        }

        private AbsCat[] GetListAbsCat()
        {
            int npoz = GetInt();
            AbsCat[] absCats = new AbsCat[npoz];
            if (npoz == 0) {
                return absCats;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    absCats[i] = GetAbsCat();
                }
            }
            return absCats;
        }

        private CSPGF.reader.Type GetType()
        {
            Hypo[] hypos = GetListHypo();
            String returnCat = GetIdent();
            Expr[] exprs = GetListExpr();
            CSPGF.reader.Type t = new CSPGF.reader.Type(hypos, returnCat, exprs);
            if (DBG) {
                System.Console.WriteLine("Type: " + t);
            }
            return t;
        }

        private Hypo GetHypo()
        {
            //TODO: Check!
            int btype = inputstream.BaseStream.ReadByte();
            Boolean b = btype == 0 ? false : true;
            String varName = GetIdent();
            CSPGF.reader.Type t = GetType();
            Hypo hh = new Hypo(b, varName, t);
            return hh;
        }

        private Hypo[] GetListHypo()
        {
            int npoz = GetInt();
            Hypo[] hypos = new Hypo[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                hypos[i] = GetHypo();
            }
            return hypos;
        }

        private Expr[] GetListExpr()
        {
            int npoz = GetInt();
            Expr[] exprs = new Expr[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                exprs[i] = GetExpr();
            }
            return exprs;
        }

        // Everything below here is not fixed yet.
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //

        private Expr GetExpr()
        {
            //TODO: Check!
            int sel = inputstream.BaseStream.ReadByte();
            Expr expr = null;
            switch (sel) {
                case 0: //lambda abstraction
                    //TODO: Check!
                    int bt = inputstream.BaseStream.ReadByte();
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
                    CSPGF.reader.Type t = GetType();
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

        private Eq[] GetListEq()
        {
            int npoz = GetInt();
            Eq[] eqs = new Eq[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                eqs[i] = GetEq();
            }
            return eqs;
        }

        private Pattern GetPattern()
        {
            //TODO: Check!
            int sel = inputstream.BaseStream.ReadByte();
            Pattern patt = null;
            switch (sel) {
                case 0: //application pattern
                    String absFun = GetIdent();
                    Pattern[] patts = GetListPattern();
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
            int sel = inputstream.BaseStream.ReadByte();
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
                System.Console.WriteLine("Concrete: " + name);
                System.Console.WriteLine("Concrete: Reading flags");
            }
            Dictionary<String, RLiteral> flags = GetListFlag();
            // We don't use the print names, but we need to read them to skip them
            if (DBG) {
                System.Console.WriteLine("Concrete: Skiping print names");
            }
            GetListPrintName();
            if (DBG) {
                System.Console.WriteLine("Concrete: Reading sequences");
            }
            Sequence[] seqs = GetListSequence();
            CncFun[] cncFuns = GetListCncFun(seqs);
            // We don't need the lindefs for now but again we need to
            // parse them to skip them
            GetListLinDef();
            ProductionSet[] prods = GetListProductionSet(cncFuns);
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

        private PrintName[] GetListPrintName()
        {
            int npoz = GetInt();
            PrintName[] pnames = new PrintName[npoz];
            if (npoz == 0) {
                return pnames;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    pnames[i] = GetPrintName();
                }
            }
            return pnames;
        }

        /* ************************************************* */
        /* Reading sequences                                 */
        /* ************************************************* */
        private Sequence GetSequence()
        {
            Symbol[] symbols = GetListSymbol();
            return new Sequence(symbols);
        }

        private Sequence[] GetListSequence()
        {
            int npoz = GetInt();
            Sequence[] seqs = new Sequence[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                seqs[i] = GetSequence();
            }
            return seqs;
        }

        private Symbol GetSymbol()
        {
            //TODO: Check!
            int sel = inputstream.BaseStream.ReadByte();
            if (DBG) {
                System.Console.WriteLine("Symbol: type=" + sel);
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
                    String[] strs = GetListString();
                    symb = new ToksSymbol(strs);
                    break;
                case 4: //alternative tokens
                    String[] altstrs = GetListString();
                    Alternative[] la = GetListAlternative();
                    symb = new AlternToksSymbol(altstrs, la);
                    break;
                // IOException -> Exception
                default:
                    throw new Exception("Invalid tag for symbols : " + sel);
            }
            if (DBG) {
                System.Console.WriteLine("/Symbol: " + symb);
            }
            return symb;
        }

        private Alternative[] GetListAlternative()
        {
            int npoz = GetInt();
            Alternative[] alts = new Alternative[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                alts[i] = GetAlternative();
            }
            return alts;
        }

        private Alternative GetAlternative()
        {
            String[] s1 = GetListString();
            String[] s2 = GetListString();
            return new Alternative(s1, s2);
        }

        private Symbol[] GetListSymbol()
        {
            int npoz = GetInt();
            Symbol[] symbols = new Symbol[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                symbols[i] = GetSymbol();
            }
            return symbols;
        }

        /* ************************************************* */
        /* Reading concrete functions                        */
        /* ************************************************* */
        private CncFun GetCncFun(Sequence[] sequences)
        {
            String name = GetIdent();
            int[] sIndices = GetListInt();
            int l = sIndices.Length;
            Sequence[] seqs = new Sequence[l];
            for (int i = 0 ; i < l ; i++) {
                seqs[i] = sequences[sIndices[i]];
            }
            return new CncFun(name, seqs);
        }

        private CncFun[] GetListCncFun(Sequence[] sequences)
        {
            int npoz = GetInt();
            CncFun[] cncFuns = new CncFun[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                cncFuns[i] = GetCncFun(sequences);
            }
            return cncFuns;
        }

        /* ************************************************* */
        /* Reading LinDefs                                   */
        /* ************************************************* */
        // LinDefs are stored as an int map (Int -> [Int])

        private LinDef[] GetListLinDef()
        {
            int size = GetInt();
            LinDef[] linDefs = new LinDef[size];
            for (int i = 0 ; i < size ; i++)
                linDefs[i] = GetLinDef();
            return linDefs;
        }

        private LinDef GetLinDef()
        {
            int key = GetInt();
            int listSize = GetInt();
            int[] funIds = new int[listSize];
            for (int i = 0 ; i < listSize ; i++) {
                funIds[i] = GetInt();
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
        private ProductionSet GetProductionSet(CncFun[] cncFuns)
        {
            int id = GetInt();
            Production[] prods = GetListProduction(id, cncFuns);
            ProductionSet ps = new ProductionSet(id, prods);
            return ps;
        }

        /**
         * Read a list of production set
         * @param is is the input stream to read from
         * @param cncFuns is the list of concrete function
         */
        private ProductionSet[] GetListProductionSet(CncFun[] cncFuns)
        {
            int npoz = GetInt();
            ProductionSet[] prods = new ProductionSet[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                prods[i] = GetProductionSet(cncFuns);
            }
            return prods;
        }

        /**
         * Read a list of production
         * @param is is the input stream to read from
         * @param leftCat is the left hand side category of this production (
         * read only once for the whole production set)
         * @param cncFuns is the list of concrete function
         */
        private Production[] GetListProduction(int leftCat, CncFun[] cncFuns)
        {
            int npoz = GetInt();
            Production[] prods = new Production[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                prods[i] = GetProduction(leftCat, cncFuns);
            }
            return prods;
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
        private Production GetProduction(int leftCat, CncFun[] cncFuns)
        {
            //TODO: CHECK!
            int sel = inputstream.BaseStream.ReadByte();
            if (DBG) {
                System.Console.WriteLine("Production: type=" + sel);
            }
            Production prod = null;
            switch (sel) {
                case 0: //application
                    int i = GetInt();
                    int[] domain = GetDomainFromPArgs();
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
                System.Console.WriteLine("/Production: " + prod);
            }
            return prod;
        }

        // This function reads a list of PArgs (Productions arguments)
        // but returns only the actual domain (the category of the argumetns)
        // since we don't need the rest for now...
        private int[] GetDomainFromPArgs()
        {
            int size = GetInt();
            int[] domain = new int[size];
            for (int i = 0 ; i < size ; i++) {
                // Skiping the list of integers
                GetListInt();
                domain[i] = GetInt();
            }
            return domain;
        }

        /* ************************************************* */
        /* Reading concrete categories                       */
        /* ************************************************* */
        private CncCat GetCncCat()
        {
            String sname = GetIdent();
            int firstFId = GetInt();
            int lastFId = GetInt();
            String[] ss = GetListString();
            return new CncCat(sname, firstFId, lastFId, ss);
        }

        private Dictionary<String, CncCat> GetListCncCat()
        {
            int npoz = GetInt();
            Dictionary<String, CncCat> cncCats = new Dictionary<String, CncCat>();
            String name;
            int firstFID, lastFID;
            String[] ss;
            for (int i = 0 ; i < npoz ; i++) {
                name = GetIdent();
                firstFID = GetInt();
                lastFID = GetInt();
                ss = GetListString();
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
            // using a byte array for efficiency
            // TODO : FIX! Skita i att köra en outputgrej och bara leka sträng? Kan ju bli segt så kanske någon typ av buffer?
            // ByteArrayOutputStream os = null; //new java.io.ByteArrayOutputStream();
            int npoz = GetInt();
            //TODO: Check :D Kan funka, antagligen inte ;D i värsta fall blir det sträng =+
            BinaryWriter os = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
            int r;
            for (int i = 0 ; i < npoz ; i++) {
                r = inputstream.BaseStream.ReadByte();
                os.Write((byte)r);
                if (r <= 0x7f) {
                }                              //lg = 0;
                else if ((r >= 0xc0) && (r <= 0xdf))
                    os.Write((byte)inputstream.BaseStream.ReadByte());   //lg = 1;
                else if ((r >= 0xe0) && (r <= 0xef)) {
                    os.Write((byte)inputstream.BaseStream.ReadByte());   //lg = 2;
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                } else if ((r >= 0xf0) && (r <= 0xf4)) {
                    os.Write((byte)inputstream.BaseStream.ReadByte());   //lg = 3;
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                } else if ((r >= 0xf8) && (r <= 0xfb)) {
                    os.Write((byte)inputstream.BaseStream.ReadByte());   //lg = 4;
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                } else if ((r >= 0xfc) && (r <= 0xfd)) {
                    os.Write((byte)inputstream.BaseStream.ReadByte());   //lg =5;
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                    os.Write((byte)inputstream.BaseStream.ReadByte());
                    //IOException -> Exception
                } else
                    throw new Exception("Undefined for now !!! ");
            }
            return os.ToString();
        }

        private String[] GetListString()
        {
            int npoz = GetInt();
            String[] strs = new String[npoz];
            if (npoz == 0) {
                return strs;
            } else {
                for (int i = 0 ; i < npoz ; i++) {
                    strs[i] = GetString();
                }
            }
            return strs;
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
            char[] bytes = new char[nbChar];
            //TODO: Check!
            inputstream.Read(bytes, 0, nbChar);
            //TODO: check if we have to change encoding!
            return bytes.ToString();
            //return new String(bytes, "ISO-8859-1");
        }

        private String[] GetListIdent()
        {
            int nb = GetInt();
            String[] strs = new String[nb];
            for (int i = 0 ; i < nb ; i++) {
                strs[i] = GetIdent();
            }
            return strs;
        }


        // Weighted idents are a pair of a String (the ident) and a double
        // (the ident).
        private WeightedIdent[] GetListWeightedIdent()
        {
            int nb = GetInt();
            WeightedIdent[] idents = new WeightedIdent[nb];
            for (int i = 0 ; i < nb ; i++) {
                double w = GetDouble();
                String s = GetIdent();
                idents[i] = new WeightedIdent(s, w);
            }
            return idents;
        }

        /* ************************************************* */
        /* Reading integers                                  */
        /* ************************************************* */
        // this reads a 'Int' in haskell serialized by the pgf serializer.
        // Those are srialized with a variable length (like some strings)
        // to gain space.
        private int GetInt()
        {
            //TODO: Check!
            long rez = (long)inputstream.BaseStream.ReadByte();
            if (rez <= 0x7f) {
                return (int)rez;
            } else {
                int ii = GetInt();
                rez = (ii << 7) | (rez & 0x7f);
                return (int)rez;
            }
        }

        private int[] GetListInt()
        {
            int npoz = GetInt();
            int[] vec = new int[npoz];
            for (int i = 0 ; i < npoz ; i++) {
                vec[i] = GetInt();
            }
            return vec;
        }

        private int MakeInt16(int j1, int j2)
        {
            int i = 0;
            i |= j1 & 0xFF;
            i <<= 8;
            i |= j2 & 0xFF;
            return i;
        }

        // Reading doubles
        private double GetDouble()
        {
            //TODO: How to read just a double in c#? D:
            //return mDataInputStream.readDouble();
            //Might work? Maybe use the binaryreader everywhere?
            BinaryReader tmp = new BinaryReader(inputstream.BaseStream);
            return tmp.ReadDouble();
        }
    }
}
    