using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.trees.Absyn;

namespace CSPGF.parser
{
    class TreeConverter
    {
        public TreeConverter()
        {

        }

        public CSPGF.trees.Absyn.Tree Intermediate2Abstract(Tree t)
        {
            return C2a(t, null);
        }

        public CSPGF.trees.Absyn.Tree C2a(Tree t, List<String> vars)
        {
            if (t is Lambda) {
                Lambda tmp = (Lambda)t;
                //lvars.foldRight(c2a(body, lvars.map(_._2).reverse ++ vars))(mkELambda)
                
            }
            if (t is Variable) {
                Variable tmp = (Variable)t;
                return new CSPGF.trees.Absyn.Variable(vars.IndexOf(tmp.cid));
            }
            if (t is Application) {
                Application tmp = (Application)t;
                List<CSPGF.trees.Absyn.Tree> tmp2 = new List<CSPGF.trees.Absyn.Tree>();
                foreach (Tree tr in tmp.args) {
                    tmp2.Add(C2a(tr, vars));
                }
                List<CSPGF.trees.Absyn.Tree> tmp3 = new List<trees.Absyn.Tree>();
                foreach (CSPGF.trees.Absyn.Tree tr in tmp2) {
                    //TODO: Detta ska sluta som ett tree...
                    tmp3.Add(MkEApp(new CSPGF.trees.Absyn.Function(tmp.fun), tr));
                }

                 //args.map(c2a(_, vars)).foldLeft(new EFunction(fun):ETree)(mkEApp)
            }
            if (t is Literal) {
                Literal tmp = (Literal)t;
                return new CSPGF.trees.Absyn.Literal(new CSPGF.trees.Absyn.StringLiteral(tmp.value));
            }
            if (t is MetaVariable) {
                MetaVariable tmp = (MetaVariable)t;
                return new CSPGF.trees.Absyn.MetaVariable(tmp.id);
            }
            return null;
        }
        //        def c2a(t : Tree, vars:List[String]): ETree = t match {
        //      // Here we have to convert sugarized λ-abstraction (λx,y,z → ...)
        //      // to canonical ones (λx→λy→λz→...)
        //      case Lambda(lvars, body) =>
        //        lvars.foldRight(c2a(body, lvars.map(_._2).reverse ++ vars))(mkELambda)
        //      // Here variables are index by name but abstract
        //      // syntax uses de Bruijn indices
        //      case Variable(x) => new EVariable(vars.indexOf(x))
        //      // Here we have to desugarized applicaton :
        //      // f a b c becomes (((f a) b) c)
        //      case Application(fun,args) =>
        //        args.map(c2a(_, vars)).foldLeft(new EFunction(fun):ETree)(mkEApp)
        //      case Literal(value) => new ELiteral(new StringLiteral(value))
        //      case MetaVariable(id) => new EMetaVariable(id)
        //    }

        public CSPGF.trees.Absyn.Tree MkEApp(CSPGF.trees.Absyn.Tree f, CSPGF.trees.Absyn.Tree x)
        {
            return new CSPGF.trees.Absyn.Application(f,x);
        }
        //    def mkEApp(f : ETree, x : ETree):ETree = new EApplication(f,x)


        public CSPGF.trees.Absyn.Tree MkELambda(Tuple<Boolean, String> v, CSPGF.trees.Absyn.Tree body)
        {
            if (v != null) {
                return new CSPGF.trees.Absyn.Lambda(v.Item2, body);
            }
            else {
                return null;
            }
        }
        //    def mkELambda(v :(Boolean, String) , body:ETree ):ETree = v match {
        //      case (bindType, name) => new ELambda(name, body)
        //    }

    }
}

//object TreeConverter {
//  def intermediate2abstract (t : Tree): ETree = {
//    def c2a(t : Tree, vars:List[String]): ETree = t match {
//      // Here we have to convert sugarized λ-abstraction (λx,y,z → ...)
//      // to canonical ones (λx→λy→λz→...)
//      case Lambda(lvars, body) =>
//        lvars.foldRight(c2a(body, lvars.map(_._2).reverse ++ vars))(mkELambda)
//      // Here variables are index by name but abstract
//      // syntax uses de Bruijn indices
//      case Variable(x) => new EVariable(vars.indexOf(x))
//      // Here we have to desugarized applicaton :
//      // f a b c becomes (((f a) b) c)
//      case Application(fun,args) =>
//        args.map(c2a(_, vars)).foldLeft(new EFunction(fun):ETree)(mkEApp)
//      case Literal(value) => new ELiteral(new StringLiteral(value))
//      case MetaVariable(id) => new EMetaVariable(id)
//    }
//    def mkEApp(f : ETree, x : ETree):ETree = new EApplication(f,x)
//    def mkELambda(v :(Boolean, String) , body:ETree ):ETree = v match {
//      case (bindType, name) => new ELambda(name, body)
//    }
//    c2a(t, Nil)
//  }