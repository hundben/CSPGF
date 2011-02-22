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

        public PGF readPGF()
        {
            Dictionary<String, int> index = null;
            // Reading the PGF version
            int[] ii = new int[4];
            for (int i = 0 ; i < 4 ; i++)
            {
                //ii[i] = mDataInputStream.read();
                // TODO: Reads one character at a time, so should be correct? Check this. Specification says int16 for version number
                ii[i] = inputstream.Read();
            }
            if (DBG)
            {
                System.Console.WriteLine("PGF version : " + ii[0] + "." + ii[1] + "." + ii[2] + "." + ii[3]);
            }
            // Reading the global flags
            Dictionary<String, RLiteral> flags = getListFlag();
            if (flags.ContainsKey("index"))
            {
                index = readIndex(((StringLit)flags["index"]).getValue());
                if (DBG)
                {
                    foreach (KeyValuePair<String, int> kp in index)
                    {
                        System.Console.WriteLine(kp.Key+", "+kp.Value);
                    }
                }
            }
            // Reading the abstract
            Abstract abs = getAbstract();
            String startCat = abs.startcat();
            // Reading the concrete grammars
            int nbConcretes = getInt();
            Concrete[] concretes;
            if (languages != null)
            {
                concretes = new Concrete[languages.Count()];
            }
            else
            {
                concretes = new Concrete[nbConcretes];
            }
            int k = 0;
            for (int i = 0 ; i < nbConcretes ; i++)
            {
                String name = getIdent();
                if (DBG)
                {
                    System.Console.WriteLine("Language " + name);
                }
                if (languages == null || languages.Remove(name))
                {
                    concretes[k] = getConcrete(name, startCat);
                    k++;
                }
                else
                {
                    if (index != null)
                    {
                        // TODO: CHECK! Maybe this will work?
                        inputstream.BaseStream.Seek(index[name], SeekOrigin.Current);
                        //this.mDataInputStream.skip(index[name]);
                        if (DBG)
                        {
                            System.Console.WriteLine("Skiping " + name);
                        }
                    }
                    else
                    {
                        getConcrete(name, startCat);
                    }
                }
            }
            // test that we actually found all the selected languages
            if (languages != null && languages.Count() > 0)
            {
                foreach (String l in languages)
                {
                    throw new UnknownLanguageException(l);
                }
            }

            // builds and returns the pgf object.
            PGF pgf = new PGF(makeInt16(ii[0], ii[1]), makeInt16(ii[2], ii[3]), flags, abs, concretes);
            inputstream.Close();
            return pgf;
        }

        /**
         * This function guess the default start category from the
         * PGF flags: if the startcat flag is set then it is taken as default cat.
         * otherwise "Sentence" is taken as default category.
         */
        private String getStartCat(Dictionary<String, RLiteral> flags)
        {
            RLiteral cat = flags["startcat"];
            if (cat == null)
            {
                return "Sentence";
            }
            else
            {
                return ((StringLit)cat).getValue();
            }
        }

        private Dictionary<String, int> readIndex(String s)
        {
            //TODO: check if new implementation works
            //String[] items = s.Split(" +");
            String[] items = s.Split(new Char[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<String, int> index = new Dictionary<String, int>();
            foreach (String item in items)
            {
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

        private Abstract getAbstract()
        {
            String name = getIdent();
            if (DBG)
            {
                System.Console.WriteLine("Abstract syntax [" + name + "]");
            }
            Dictionary<String, RLiteral> flags = getListFlag();
            AbsFun[] absFuns = getListAbsFun();
            AbsCat[] absCats = getListAbsCat();
            return new Abstract(name, flags, absFuns, absCats);
        }

        private Pattern[] getListPattern()
        {
            int npoz = getInt();
            Pattern[] patts = new Pattern[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                patts[i] = getPattern();
            }
            return patts;
        }

        private Eq getEq()
        {
            Pattern[] patts = getListPattern();
            Expr exp = getExpr();
            return new Eq(patts, exp);
        }

        private AbsFun getAbsFun()
        {
            String name = getIdent();
            if (DBG)
            {
                System.Console.WriteLine("AbsFun: '" + name + "'");
            }
            CSPGF.reader.Type t = getType();
            int i = getInt();
            //TODO: Check!
            int has_equations = inputstream.Read();
            Eq[] equations;
            if (has_equations == 0)
            {
                equations = new Eq[0];
            }
            else
            {
                equations = getListEq();
            }
            double weight = getDouble();
            AbsFun f = new AbsFun(name, t, i, equations, weight);
            if (DBG)
            {
                System.Console.WriteLine("/AbsFun: " + f);
            }
            return f;
        }

        private AbsCat getAbsCat()
        {
            String name = getIdent();
            Hypo[] hypos = getListHypo();
            WeightedIdent[] functions = getListWeightedIdent();
            AbsCat abcC = new AbsCat(name, hypos, functions);
            return abcC;
        }

        private AbsFun[] getListAbsFun()
        {
            int npoz = getInt();
            AbsFun[] absFuns = new AbsFun[npoz];
            if (npoz == 0)
            {
                return absFuns;
            }
            else
            {
                for (int i = 0 ; i < npoz ; i++)
                {
                    absFuns[i] = getAbsFun();
                }
            }
            return absFuns;
        }

        private AbsCat[] getListAbsCat()
        {
            int npoz = getInt();
            AbsCat[] absCats = new AbsCat[npoz];
            if (npoz == 0)
            {
                return absCats;
            }
            else
            {
                for (int i = 0 ; i < npoz ; i++)
                {
                    absCats[i] = getAbsCat();
                }
            }
            return absCats;
        }

        private CSPGF.reader.Type getType()
        {
            Hypo[] hypos = getListHypo();
            String returnCat = getIdent();
            Expr[] exprs = getListExpr();
            CSPGF.reader.Type t = new CSPGF.reader.Type(hypos, returnCat, exprs);
            if (DBG)
            {
                System.Console.WriteLine("Type: " + t);
            }
            return t;
        }

        private Hypo getHypo()
        {
            //TODO: Check!
            int btype = inputstream.Read();
            Boolean b = btype == 0 ? false : true;
            String varName = getIdent();
            CSPGF.reader.Type t = getType();
            Hypo hh = new Hypo(b, varName, t);
            return hh;
        }

        private Hypo[] getListHypo()
        {
            int npoz = getInt();
            Hypo[] hypos = new Hypo[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                hypos[i] = getHypo();
            }
            return hypos;
        }

        private Expr[] getListExpr()
        {
            int npoz = getInt();
            Expr[] exprs = new Expr[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                exprs[i] = getExpr();
            }
            return exprs;
        }

        // Everything below here is not fixed yet.
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //

        private Expr getExpr()
        {
            //TODO: Check!
            int sel = inputstream.Read();
            Expr expr = null;
            switch (sel)
            {
                case 0: //lambda abstraction
                    //TODO: Check!
                    int bt = inputstream.Read();
                    Boolean btype = bt == 0 ? false : true;
                    String varName = getIdent();
                    Expr e1 = getExpr();
                    expr = new LambdaExp(btype, varName, e1);
                    break;
                case 1: //expression application
                    Expr e11 = getExpr();
                    Expr e2 = getExpr();
                    expr = new AppExp(e11, e2);
                    break;
                case 2: //literal expression
                    RLiteral lit = getLiteral();
                    expr = new LiteralExp(lit);
                    break;
                case 3: //meta variable
                    int id = getInt();
                    expr = new MetaExp(id);
                    break;
                case 4: //abstract function name
                    String absFun = getIdent();
                    expr = new AbsNameExp(absFun);
                    break;
                case 5: //variable
                    int v = getInt();
                    expr = new VarExp(v);
                    break;
                case 6: //type annotated expression
                    Expr e = getExpr();
                    CSPGF.reader.Type t = getType();
                    expr = new TypedExp(e, t);
                    break;
                case 7: //implicit argument
                    Expr ee = getExpr();
                    expr = new ImplExp(ee);
                    break;
                default:
                    throw new Exception("Invalid tag for expressions : " + sel);
            }
            return expr;
        }

        private Eq[] getListEq()
        {
            int npoz = getInt();
            Eq[] eqs = new Eq[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                eqs[i] = getEq();
            }
            return eqs;
        }

        private Pattern getPattern()
        {
            //TODO: Check!
            int sel = inputstream.Read();
            Pattern patt = null;
            switch (sel)
            {
                case 0: //application pattern
                    String absFun = getIdent();
                    Pattern[] patts = getListPattern();
                    patt = new AppPattern(absFun, patts);
                    break;
                case 1: //variable pattern
                    String varName = getIdent();
                    patt = new VarPattern(varName);
                    break;
                case 2: //variable as pattern
                    String pVarName = getIdent();
                    Pattern p = getPattern();
                    patt = new VarAsPattern(pVarName, p);
                    break;
                case 3: //wild card pattern
                    patt = new WildCardPattern();
                    break;
                case 4: //literal pattern
                    RLiteral lit = getLiteral();
                    patt = new LiteralPattern(lit);
                    break;
                case 5: //implicit argument
                    Pattern pp = getPattern();
                    patt = new ImpArgPattern(pp);
                    break;
                case 6: //inaccessible pattern
                    Expr e = getExpr();
                    patt = new InaccPattern(e);
                    break;
                default:
                    throw new Exception("Invalid tag for patterns : " + sel);
            }
            return patt;
        }

        private RLiteral getLiteral()
        {
            //TODO: CHECK!
            int sel = inputstream.Read();
            RLiteral ss = null;
            switch (sel)
            {
                case 0:
                    String str = getString();
                    ss = new StringLit(str);
                    break;
                case 1:
                    int i = getInt();
                    ss = new IntLit(i);
                    break;
                case 2:
                    double d = getDouble();
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
        private Concrete getConcrete(String name, String startCat)
        {
            if (DBG)
            {
                System.Console.WriteLine("Concrete: " + name);
            }
            if (DBG)
            {
                System.Console.WriteLine("Concrete: Reading flags");
            }
            Dictionary<String, RLiteral> flags = getListFlag();
            // We don't use the print names, but we need to read them to skip them
            if (DBG)
            {
                System.Console.WriteLine("Concrete: Skiping print names");
            }
            getListPrintName();
            if (DBG)
            {
                System.Console.WriteLine("Concrete: Reading sequences");
            }
            Sequence[] seqs = getListSequence();
            CncFun[] cncFuns = getListCncFun(seqs);
            // We don't need the lindefs for now but again we need to
            // parse them to skip them
            getListLinDef();
            ProductionSet[] prods = getListProductionSet(cncFuns);
            Dictionary<String, CncCat> cncCats = getListCncCat();
            int i = getInt();
            return new Concrete(name, flags, seqs, cncFuns, prods, cncCats, i, startCat);
        }

        /* ************************************************* */
        /* Reading print names                               */
        /* ************************************************* */
        // FIXME : not used, we should avoid creating the objects
        private PrintName getPrintName()
        {
            String absName = getIdent();
            String printName = getString();
            return new PrintName(absName, printName);

        }

        private PrintName[] getListPrintName()
        {
            int npoz = getInt();
            PrintName[] pnames = new PrintName[npoz];
            if (npoz == 0)
            {
                return pnames;
            }
            else
            {
                for (int i = 0 ; i < npoz ; i++)
                {
                    pnames[i] = getPrintName();
                }
            }
            return pnames;
        }

        /* ************************************************* */
        /* Reading sequences                                 */
        /* ************************************************* */
        private Sequence getSequence()
        {
            Symbol[] symbols = getListSymbol();
            return new Sequence(symbols);
        }

        private Sequence[] getListSequence()
        {
            int npoz = getInt();
            Sequence[] seqs = new Sequence[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                seqs[i] = getSequence();
            }
            return seqs;
        }

        private Symbol getSymbol()
        {
            //TODO: Check!
            int sel = inputstream.Read();
            if (DBG)
            {
                System.Console.WriteLine("Symbol: type=" + sel);
            }
            Symbol symb = null;
            switch (sel)
            {
                case 0: // category (non terminal symbol)
                case 1: // Lit (Not implemented properly)
                    int i1 = getInt();
                    int i2 = getInt();
                    symb = new ArgConstSymbol(i1, i2);
                    break;
                case 2: // Variable (Not implemented)
                    //UnsupportedOperationException -> Exception
                    throw new Exception("Var symbols are not supported yet");
                case 3: //sequence of tokens
                    String[] strs = getListString();
                    symb = new ToksSymbol(strs);
                    break;
                case 4: //alternative tokens
                    String[] altstrs = getListString();
                    Alternative[] la = getListAlternative();
                    symb = new AlternToksSymbol(altstrs, la);
                    break;
                // IOException -> Exception
                default:
                    throw new Exception("Invalid tag for symbols : " + sel);
            }
            if (DBG)
            {
                System.Console.WriteLine("/Symbol: " + symb);
            }
            return symb;
        }

        private Alternative[] getListAlternative()
        {
            int npoz = getInt();
            Alternative[] alts = new Alternative[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                alts[i] = getAlternative();
            }
            return alts;
        }

        private Alternative getAlternative()
        {
            String[] s1 = getListString();
            String[] s2 = getListString();
            return new Alternative(s1, s2);
        }

        private Symbol[] getListSymbol()
        {
            int npoz = getInt();
            Symbol[] symbols = new Symbol[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                symbols[i] = getSymbol();
            }
            return symbols;
        }

        /* ************************************************* */
        /* Reading concrete functions                        */
        /* ************************************************* */
        private CncFun getCncFun(Sequence[] sequences)
        {
            String name = getIdent();
            int[] sIndices = getListInt();
            int l = sIndices.Length;
            Sequence[] seqs = new Sequence[l];
            for (int i = 0 ; i < l ; i++)
            {
                seqs[i] = sequences[sIndices[i]];
            }
            return new CncFun(name, seqs);
        }

        private CncFun[] getListCncFun(Sequence[] sequences)
        {
            int npoz = getInt();
            CncFun[] cncFuns = new CncFun[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                cncFuns[i] = getCncFun(sequences);
            }
            return cncFuns;
        }

        /* ************************************************* */
        /* Reading LinDefs                                   */
        /* ************************************************* */
        // LinDefs are stored as an int map (Int -> [Int])

        private LinDef[] getListLinDef()
        {
            int size = getInt();
            LinDef[] linDefs = new LinDef[size];
            for (int i = 0 ; i < size ; i++)
                linDefs[i] = getLinDef();
            return linDefs;
        }

        private LinDef getLinDef()
        {
            int key = getInt();
            int listSize = getInt();
            int[] funIds = new int[listSize];
            for (int i = 0 ; i < listSize ; i++)
            {
                funIds[i] = getInt();
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
        private ProductionSet getProductionSet(CncFun[] cncFuns)
        {
            int id = getInt();
            Production[] prods = getListProduction(id, cncFuns);
            ProductionSet ps = new ProductionSet(id, prods);
            return ps;
        }

        /**
         * Read a list of production set
         * @param is is the input stream to read from
         * @param cncFuns is the list of concrete function
         */
        private ProductionSet[] getListProductionSet(CncFun[] cncFuns)
        {
            int npoz = getInt();
            ProductionSet[] prods = new ProductionSet[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                prods[i] = getProductionSet(cncFuns);
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
        private Production[] getListProduction(int leftCat, CncFun[] cncFuns)
        {
            int npoz = getInt();
            Production[] prods = new Production[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                prods[i] = getProduction(leftCat, cncFuns);
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
        private Production getProduction(int leftCat, CncFun[] cncFuns)
        {
            //TODO: CHECK!
            int sel = inputstream.Read();
            if (DBG)
            {
                System.Console.WriteLine("Production: type=" + sel);
            }
            Production prod = null;
            switch (sel)
            {
                case 0: //application
                    int i = getInt();
                    int[] domain = getDomainFromPArgs();
                    prod = new ApplProduction(leftCat, cncFuns[i], domain);
                    break;
                case 1: //coercion
                    int id = getInt();
                    prod = new CoerceProduction(leftCat, id);
                    break;
                //IOException -> Exception
                default:
                    throw new Exception("Invalid tag for productions : " + sel);
            }
            if (DBG)
            {
                System.Console.WriteLine("/Production: " + prod);
            }
            return prod;
        }

        // This function reads a list of PArgs (Productions arguments)
        // but returns only the actual domain (the category of the argumetns)
        // since we don't need the rest for now...
        private int[] getDomainFromPArgs()
        {
            int size = getInt();
            int[] domain = new int[size];
            for (int i = 0 ; i < size ; i++)
            {
                // Skiping the list of integers
                getListInt();
                domain[i] = getInt();
            }
            return domain;
        }

        /* ************************************************* */
        /* Reading concrete categories                       */
        /* ************************************************* */
        private CncCat getCncCat()
        {
            String sname = getIdent();
            int firstFId = getInt();
            int lastFId = getInt();
            String[] ss = getListString();
            return new CncCat(sname, firstFId, lastFId, ss);
        }

        private Dictionary<String, CncCat> getListCncCat()
        {
            int npoz = getInt();
            Dictionary<String, CncCat> cncCats = new Dictionary<String, CncCat>();
            String name;
            int firstFID, lastFID;
            String[] ss;
            for (int i = 0 ; i < npoz ; i++)
            {
                name = getIdent();
                firstFID = getInt();
                lastFID = getInt();
                ss = getListString();
                cncCats.Add(name, new CncCat(name, firstFID, lastFID, ss));
            }
            return cncCats;
        }

        /* ************************************************* */
        /* Reading flags                                     */
        /* ************************************************* */
        private Dictionary<String, RLiteral> getListFlag()
        {
            int npoz = getInt();
            Dictionary<String, RLiteral> flags = new Dictionary<String, RLiteral>();
            if (npoz == 0)
            {
                return flags;
            }
            for (int i = 0 ; i < npoz ; i++)
            {
                String ss = getIdent();
                RLiteral lit = getLiteral();
                flags.Add(ss, lit);
            }
            return flags;
        }

        /* ************************************************* */
        /* Reading strings                                   */
        /* ************************************************* */
        private String getString()
        {
            // using a byte array for efficiency
            // TODO : FIX! Skita i att köra en outputgrej och bara leka sträng? Kan ju bli segt så kanske någon typ av buffer?
            ByteArrayOutputStream os = null; //new java.io.ByteArrayOutputStream();
            int npoz = getInt();
            int r;
            for (int i = 0 ; i < npoz ; i++)
            {
                r = mDataInputStream.read();
                os.write((byte)r);
                if (r <= 0x7f)
                {
                }                              //lg = 0;
                else if ((r >= 0xc0) && (r <= 0xdf))
                    os.write((byte)mDataInputStream.read());   //lg = 1;
                else if ((r >= 0xe0) && (r <= 0xef))
                {
                    os.write((byte)mDataInputStream.read());   //lg = 2;
                    os.write((byte)mDataInputStream.read());
                }
                else if ((r >= 0xf0) && (r <= 0xf4))
                {
                    os.write((byte)mDataInputStream.read());   //lg = 3;
                    os.write((byte)mDataInputStream.read());
                    os.write((byte)mDataInputStream.read());
                }
                else if ((r >= 0xf8) && (r <= 0xfb))
                {
                    os.write((byte)mDataInputStream.read());   //lg = 4;
                    os.write((byte)mDataInputStream.read());
                    os.write((byte)mDataInputStream.read());
                    os.write((byte)mDataInputStream.read());
                }
                else if ((r >= 0xfc) && (r <= 0xfd))
                {
                    os.write((byte)mDataInputStream.read());   //lg =5;
                    os.write((byte)mDataInputStream.read());
                    os.write((byte)mDataInputStream.read());
                    os.write((byte)mDataInputStream.read());
                    os.write((byte)mDataInputStream.read());
                    //IOException -> Exception
                }
                else
                    throw new Exception("Undefined for now !!! ");
            }
            return os.toString("UTF-8");
        }

        private String[] getListString()
        {
            int npoz = getInt();
            String[] strs = new String[npoz];
            if (npoz == 0)
            {
                return strs;
            }
            else
            {
                for (int i = 0 ; i < npoz ; i++)
                {
                    strs[i] = getString();
                }
            }
            return strs;
        }

        /**
         * Some string (like categories identifiers) are not allowed to
         * use the full utf8 tables but only latin 1 caracters.
         * We can read them faster using this knowledge.
         **/
        private String getIdent()
        {
            int nbChar = getInt();
            //byte[] bytes = new byte[nbChar];
            char[] bytes = new char[nbChar];
            //TODO: Check!
            inputstream.Read(bytes,0,nbChar);
            //TODO: check if we have to change encoding!
            return bytes.ToString();
            //return new String(bytes, "ISO-8859-1");
        }

        private String[] getListIdent()
        {
            int nb = getInt();
            String[] strs = new String[nb];
            for (int i = 0 ; i < nb ; i++)
            {
                strs[i] = getIdent();
            }
            return strs;
        }


        // Weighted idents are a pair of a String (the ident) and a double
        // (the ident).
        private WeightedIdent[] getListWeightedIdent()
        {
            int nb = getInt();
            WeightedIdent[] idents = new WeightedIdent[nb];
            for (int i = 0 ; i < nb ; i++)
            {
                double w = getDouble();
                String s = getIdent();
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
        private int getInt()
        {
            //TODO: Check!
            long rez = (long)inputstream.Read();
            if (rez <= 0x7f)
            {
                return (int)rez;
            }
            else
            {
                int ii = getInt();
                rez = (ii << 7) | (rez & 0x7f);
                return (int)rez;
            }
        }

        private int[] getListInt()
        {
            int npoz = getInt();
            int[] vec = new int[npoz];
            for (int i = 0 ; i < npoz ; i++)
            {
                vec[i] = getInt();
            }
            return vec;
        }

        private int makeInt16(int j1, int j2)
        {
            int i = 0;
            i |= j1 & 0xFF;
            i <<= 8;
            i |= j2 & 0xFF;
            return i;
        }

        // Reading doubles
        private double getDouble()
        {
            //TODO: How to read just a double in c#? D:
            return mDataInputStream.readDouble();
        }
    }
}
    