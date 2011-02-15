using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;
using CSPGF.trees.Absyn;

namespace CSPGF
{
    class Generator
    {
        private Random random;
        private PGF pgf;
        private Dictionary<String, HashSet<String>> dirRules;
        private Dictionary<String, HashSet<String>> indirRules;

        /** generates a random expression of a given category
         * does not handle dependent categories or categories with implicit arguments
         **/
        public Generator(PGF _pgf)
        {
            random = new Random();
            pgf = _pgf;
            dirRules = new Dictionary<String, HashSet<String>>();
            indirRules = new Dictionary<String, HashSet<String>>();
            AbsCat[] absCats = pgf.getAbstract().getAbsCats();
            AbsFun[] absFuns = pgf.getAbstract().getAbsFuns();
            HashSet<String> dirFuns = new HashSet<String>();
            HashSet<String> indirFuns = new HashSet<String>();
            for (int i = 0 ; i < absCats.Length ; i++)
            {
                dirFuns = new HashSet<String>();
                indirFuns = new HashSet<String>();
                WeightedIdent[] functions = absCats[i].getFunctions();
                for (int j = 0 ; j < functions.Length ; j++)
                    for (int k = 0 ; k < absFuns.Length ; k++)
                        if (functions[j].getIdent().Equals(absFuns[k].getName()))
                        {
                            if (absFuns[k].getType().getHypos().Length == 0)
                                dirFuns.Add(functions[j].getIdent());
                            else
                                indirFuns.Add(functions[j].getIdent());
                            break;
                        }
                dirRules.Add(absCats[i].getName(), dirFuns);
                indirRules.Add(absCats[i].getName(), indirFuns);
            }
        }

        
        public Tree gen() {
	        return gen(pgf.getAbstract().startcat());
        }

        /** generates a category with a random direct rule
         * suitable for simple expressions
         **/
        // FIXME what is 'type' for ???
        // FIXME couldn't dirFuns be an array ?
        public Tree getDirect(String type, HashSet<String> dirFuns) {
	    int rand = this.random.Next(dirFuns.Count());
	    return new Function((String)dirFuns.ToArray()[rand]);
        }

        /** generates a category with a random indirect rule
         * creates more complex expressions
         **/
        public Tree getIndirect(String type, HashSet<String> indirFuns) {
	        //Iterator<String> it = indirFuns.iterator();
	        //Vector<String> vs = new Vector<String>();
            /*while (it.hasNext())
            {
                vs.add(it.next());
            }*/
            List<String> vs = new List<String>();
            foreach (String it in indirFuns)
            {
                vs.Add(it);
            }
            
	        int rand = random.Next(vs.Count());
	        String funcName = vs.ElementAt(rand);
	        AbsFun[] absFuns = pgf.getAbstract().getAbsFuns();
            foreach (AbsFun a in absFuns) 
            //for (int i = 0 ; i < absFuns.Length ; i++)
            {
                if (a.getName().Equals(funcName))
                {
                    Hypo[] hypos = a.getType().getHypos();
                    String[] tempCats = new String[hypos.Length];
                    Tree[] exps = new Tree[hypos.Length];
                    // TODO: Går detta att göra om?
                    for (int k = 0 ; k < hypos.Length ; k++)
                    {
                        tempCats[k] = hypos[k].getType().getName();
                        exps[k] = gen(tempCats[k]);
                        if (exps[k] == null)
                        {
                            return null;
                        }
                    }
                    Tree rez = new Function(funcName);
                    foreach (Tree t in exps)
                    //for (int j = 0 ; j < exps.Length ; j++)
                    {
                        rez = new Application(rez, t);
                    }
                    return rez;
                }
            }
	        return null;
        }


        /** generates a random expression of a given category
         * the empirical probability of using direct rules is 60%
         * this decreases the probability of having infinite trees for infinite grammars
         **/
        public Tree gen(String type)
        {
            if (type.Equals("Integer"))
            {
                return new Literal(new IntLiteral(generateInt()));
            }
            else if (type.Equals("Float"))
            {
                return new Literal(new FloatLiteral(generateFloat()));
            }
            else if (type.Equals("String"))
            {
                return new Literal(new StringLiteral(generateString()));
            }
            int depth = random.Next(5); //60% constants, 40% functions
            HashSet<String> dirFuns = dirRules[type];
            HashSet<String> indirFuns = indirRules[type];
            // TODO: Check if it should be inverted?
            Boolean isEmptyDir = dirFuns.Any();
            Boolean isEmptyIndir = indirFuns.Any();
            if (isEmptyDir && isEmptyIndir)
            {
                throw new Exception("Cannot generate any expression of type " + type);
            }
            if (isEmptyDir)
            {
                return getIndirect(type, indirFuns);
            }
            if (isEmptyIndir)
            {
                return getDirect(type, dirFuns);
            }
            if (depth <= 2)
            {
                return getDirect(type, dirFuns);
            }
            return getIndirect(type, indirFuns);
        }


         /** generates a number of expressions of a given category
         * the expressions are independent
         * the probability of having simple expressions is higher
         **/
        public List<Tree> generateMany(String type, int count)
        {
            int contor = 0;
            List<Tree> rez = new List<Tree>();
            if (contor >= count)
            {
                return rez;
            }
            HashSet<String> dirFuns = dirRules[type];
            HashSet<String> indirFuns = indirRules[type];
            foreach (String it in dirFuns)
            {
                Tree interm = getDirect(type, dirFuns);
                if (interm != null)
                {
                    contor++;
                    rez.Add(interm);
                    if (contor == count)
                    {
                        return rez;
                    }
                }
            }
            foreach (String it in indirFuns)
            {
                Tree interm = getIndirect(type, indirFuns);
                if (interm != null)
                {
                    contor++;
                    rez.Add(interm);
                    if (contor == count)
                    {
                        return rez;
                    }
                }
            }
            return rez;
        }


        /** generates a random string
        **/

        public String generateString()
        {
            String[] ss = { "x", "y", "foo", "bar" };
            return ss[random.Next(ss.Length)];
        }

        /** generates a random integer
         **/
        public int generateInt()
        {
            return random.Next(100000);
        }

        /** generates a random float
         **/
        public double generateFloat()
        {
            return random.NextDouble();
        }
    }
}
