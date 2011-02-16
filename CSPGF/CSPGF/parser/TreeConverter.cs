using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class Class1
    {
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