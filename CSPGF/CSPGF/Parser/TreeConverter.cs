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
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Converts trees. Add a better description here later.
    /// </summary>
    internal class TreeConverter
    {
        /// <summary>
        /// Converts a Intermediate tree to an abstract one.
        /// </summary>
        /// <param name="t">A tree.</param>
        /// <returns>The abstract tree.</returns>
        public Trees.Absyn.Tree Intermediate2Abstract(Tree t)
        {
            return this.C2A(t, new List<string>());
        }

        /// <summary>
        /// Converts a tree to an abstract tree. Change the description here later.
        /// </summary>
        /// <param name="t">A tree.</param>
        /// <param name="vars">Some variables.</param>
        /// <returns>The abstract tree.</returns>
        private Trees.Absyn.Tree C2A(Tree t, List<string> vars)
        {
            if (t is Lambda) 
            {
                // lvars.foldRight(c2a(body, lvars.map(_._2).reverse ++ vars))(mkELambda)
                Lambda lt = (Lambda)t;

                List<Tuple<bool, string>> lvars = lt.Vars;
                Tree body = lt.Body;

                // lvars.map(_._2)
                List<string> tmp2 = new List<string>();
                foreach (Tuple<bool, string> tup in lvars) 
                {
                    tmp2.Add(tup.Item2);
                }

                tmp2.Reverse();
                tmp2.AddRange(vars);

                Trees.Absyn.Tree tree = this.C2A(body, tmp2);
                lvars.Reverse();
                foreach (Tuple<bool, string> tup in lvars) 
                {
                    tree = this.MkELambda(tup, tree);
                }

                lvars.Reverse();    
                return tree;
            }
            else if (t is Variable) 
            {
                Variable var = (Variable)t;
                return new Trees.Absyn.Variable(vars.IndexOf(var.Cid));
            }
            else if (t is Application) 
            {
                Application app = (Application)t;
                List<Trees.Absyn.Tree> trees = new List<Trees.Absyn.Tree> { new Trees.Absyn.Function(app.Fun) };
                foreach (Tree tr in app.Args) 
                {
                    trees.Add(this.C2A(tr, vars));
                }

                return trees.Aggregate<Trees.Absyn.Tree>(this.MkEApp);
            }
            else if (t is Literal) 
            {
                // TODO WRONG! should check type
                Literal lit = (Literal)t;
                if (lit.Type == -1)
                {
                    return new Trees.Absyn.Literal(new Trees.Absyn.StringLiteral(lit.Value));
                }
                else if (lit.Type == -2)
                {
                    return new Trees.Absyn.Literal(new Trees.Absyn.IntLiteral(int.Parse(lit.Value, NumberFormatInfo.InvariantInfo)));   // we have already parsed it once so should be ok
                }
                else if (lit.Type == -3)
                {
                    return new Trees.Absyn.Literal(new Trees.Absyn.FloatLiteral(float.Parse(lit.Value, NumberFormatInfo.InvariantInfo)));
                }
            }
            else if (t is MetaVariable) 
            {
                MetaVariable meta = (MetaVariable)t;
                return new Trees.Absyn.MetaVariable(meta.ID);
            }

            return null;
        }

        /// <summary>
        /// Creates a new application.
        /// </summary>
        /// <param name="f">The first tree.</param>
        /// <param name="x">The second tree.</param>
        /// <returns>An application with both trees.</returns>
        private Trees.Absyn.Tree MkEApp(Trees.Absyn.Tree f, Trees.Absyn.Tree x)
        {
            return new Trees.Absyn.Application(f, x);
        }
        
        /// <summary>
        /// Creates a lambda node in the abstract syntax tree.
        /// </summary>
        /// <param name="v">A tuple with the boolean and a string.</param>
        /// <param name="body">The body of the tree.</param>
        /// <returns>A new tree.</returns>
        private Trees.Absyn.Tree MkELambda(Tuple<bool, string> v, Trees.Absyn.Tree body)
        {
            if (v != null) 
            {
                return new Trees.Absyn.Lambda(v.Item2, body);
            }

            return null;
        }
    }
}
