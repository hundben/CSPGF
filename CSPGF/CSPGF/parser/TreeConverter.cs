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
                List<String> tmp2 = new List<String>();
                foreach (Tuple<Boolean, String> tup in tmp.vars) {
                    tmp2.Add(tup.Item2);
                }
                tmp2.Reverse();
                foreach (String s in vars) {
                    tmp2.Add(s);
                }
                tmp.vars.Reverse();
                CSPGF.trees.Absyn.Tree tmptree = C2a(tmp.body, tmp2);
                foreach (Tuple<Boolean, String> tup in tmp.vars) {
                    tmptree = MkELambda(tup, tmptree);
                }
                return tmptree;
            }
            else if (t is Variable) {
                Variable tmp = (Variable)t;
                return new CSPGF.trees.Absyn.Variable(vars.IndexOf(tmp.cid));
            }
            else if (t is Application) {
                Application tmp = (Application)t;
                List<CSPGF.trees.Absyn.Tree> tmp2 = new List<CSPGF.trees.Absyn.Tree>();
                tmp2.Add(new CSPGF.trees.Absyn.Function(tmp.fun));
                foreach (Tree tr in tmp.args) {
                    tmp2.Add(C2a(tr, vars));
                }
                //TODO: Check if it works :D
                return tmp2.Aggregate<CSPGF.trees.Absyn.Tree>(MkEApp);
            }
            else if (t is Literal) {
                Literal tmp = (Literal)t;
                return new CSPGF.trees.Absyn.Literal(new CSPGF.trees.Absyn.StringLiteral(tmp.value));
            }
            else if (t is MetaVariable) {
                MetaVariable tmp = (MetaVariable)t;
                return new CSPGF.trees.Absyn.MetaVariable(tmp.id);
            }
            return null;
        }

        public CSPGF.trees.Absyn.Tree MkEApp(CSPGF.trees.Absyn.Tree f, CSPGF.trees.Absyn.Tree x)
        {
            return new CSPGF.trees.Absyn.Application(f, x);
        }
        
        public CSPGF.trees.Absyn.Tree MkELambda(Tuple<Boolean, String> v, CSPGF.trees.Absyn.Tree body)
        {
            if (v != null) {
                return new CSPGF.trees.Absyn.Lambda(v.Item2, body);
            }
            else {
                return null;
            }
        }
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