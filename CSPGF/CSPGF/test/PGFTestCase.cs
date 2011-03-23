using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.trees;
using CSPGF.trees.Absyn;
using CSPGF.trees.VisitSkeleton;

namespace CSPGF.test
{
    class PGFTestCase
    {
        String name;
        public PGFTestCase(String _name)
        {
            name = _name;
        }

        protected PGF getPGF(String filename)
        {
            //String fullname = this.getClass().getResource(filename).getFile();
            PGF pgf = PGFBuilder.FromFile(filename);
            return pgf;
        }

        protected Tree parseTree(String s)
        {
            //Yylex l = new Yylex(new StringReader(s));
            //parser p = new parser(l);
            //try {
            //    Tree parse_tree = p.pTree();
            //    return parse_tree;
            //}
            //catch (Exception e) {
            //    return null;
            //}
            return null;
        }
    }
}
