//-----------------------------------------------------------------------
// <copyright file="TreeConverter.cs" company="None">
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

namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Trees.Absyn;

    /// <summary>
    /// Converts trees. Add a better description here later.
    /// </summary>
    public class TreeConverter
    {
        /// <summary>
        /// Initializes a new instance of the TreeConverter class.
        /// </summary>
        public TreeConverter()
        {
        }

        /// <summary>
        /// Converts a Intermediate tree to an abstract one.
        /// </summary>
        /// <param name="t">A tree.</param>
        /// <returns>The abstract tree.</returns>
        public CSPGF.Trees.Absyn.Tree Intermediate2Abstract(Tree t)
        {
            return this.C2a(t, null);
        }

        /// <summary>
        /// Converts a tree to an abstract tree. Change the description here later.
        /// </summary>
        /// <param name="t">A tree.</param>
        /// <param name="vars">Some variables.</param>
        /// <returns>The abstract tree.</returns>
        public CSPGF.Trees.Absyn.Tree C2a(Tree t, List<string> vars)
        {
            if (t is Lambda) 
            {
                Lambda tmp = (Lambda)t;
                List<string> tmp2 = new List<string>();
                foreach (Tuple<bool, string> tup in tmp.Vars) 
                {
                    tmp2.Add(tup.Item2);
                }

                tmp2.Reverse();
                foreach (string s in vars) 
                {
                    tmp2.Add(s);
                }

                tmp.Vars.Reverse();
                CSPGF.Trees.Absyn.Tree tmptree = this.C2a(tmp.Body, tmp2);
                foreach (Tuple<bool, string> tup in tmp.Vars) 
                {
                    tmptree = this.MkELambda(tup, tmptree);
                }

                return tmptree;
            }
            else if (t is Variable) 
            {
                Variable tmp = (Variable)t;
                return new CSPGF.Trees.Absyn.Variable(vars.IndexOf(tmp.Cid));
            }
            else if (t is Application) 
            {
                Application tmp = (Application)t;
                List<CSPGF.Trees.Absyn.Tree> tmp2 = new List<CSPGF.Trees.Absyn.Tree>();
                tmp2.Add(new CSPGF.Trees.Absyn.Function(tmp.Fun));
                foreach (Tree tr in tmp.Args) 
                {
                    tmp2.Add(this.C2a(tr, vars));
                }

                // TODO: Check if it works :D
                return tmp2.Aggregate<CSPGF.Trees.Absyn.Tree>(this.MkEApp);
            }
            else if (t is Literal) 
            {
                Literal tmp = (Literal)t;
                return new CSPGF.Trees.Absyn.Literal(new CSPGF.Trees.Absyn.StringLiteral(tmp.Value));
            }
            else if (t is MetaVariable) 
            {
                MetaVariable tmp = (MetaVariable)t;
                return new CSPGF.Trees.Absyn.MetaVariable(tmp.ID);
            }

            return null;
        }

        /// <summary>
        /// Creates a new aplication.
        /// </summary>
        /// <param name="f">The first tree.</param>
        /// <param name="x">The second tree.</param>
        /// <returns>An application with both trees.</returns>
        public CSPGF.Trees.Absyn.Tree MkEApp(CSPGF.Trees.Absyn.Tree f, CSPGF.Trees.Absyn.Tree x)
        {
            return new CSPGF.Trees.Absyn.Application(f, x);
        }
        
        /// <summary>
        /// Creates a lambda node in the absyn tree.
        /// </summary>
        /// <param name="v">A tuple with the boolean and a string.</param>
        /// <param name="body">The body of the tree.</param>
        /// <returns>A new tree.</returns>
        public CSPGF.Trees.Absyn.Tree MkELambda(Tuple<bool, string> v, CSPGF.Trees.Absyn.Tree body)
        {
            if (v != null) 
            {
                return new CSPGF.Trees.Absyn.Lambda(v.Item2, body);
            }
            else 
            {
                return null;
            }
        }
    }
}

// object TreeConverter {
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