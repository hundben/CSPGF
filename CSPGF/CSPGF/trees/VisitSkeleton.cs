//-----------------------------------------------------------------------
// <copyright file="VisitSkeleton.cs" company="None">
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

/*** BNFC-Generated Visitor Design Pattern Skeleton. ***/
/* This implements the common visitor design pattern. To make sure that
   compile errors occur when code in the Visitor don't match the abstract
   syntaxt, the "abstract visit skeleton" is used.
   
   Replace the R and A parameters with the desired return
   and context types.*/

namespace CSPGF.Trees.VisitSkeleton
{
  #region Classes
  /// <summary>
  /// A visitor class for the trees.
  /// </summary>
  /// <typeparam name="R">Insert description for R.</typeparam>
  /// <typeparam name="A">Insert description for A.</typeparam>
  public class TreeVisitor<R, A> : AbstractTreeVisitor<R, A>
  {
    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="lambda_">Insert description for lambda_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.Lambda lambda_, A arg)
    {
      // Code For Lambda Goes Here
      // lambda_.Ident_
      lambda_.Tree_.Accept(new TreeVisitor<R, A>(), arg);
      return default(R);
    }

    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="variable_">Insert description for variable_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.Variable variable_, A arg)
    {
      // Code For Variable Goes Here 
      // variable_.Integer_
      return default(R);
    }
    
    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="application_">Insert description for application_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.Application application_, A arg)
    {
      // Code For Application Goes Here 
      application_.Tree_1.Accept(new TreeVisitor<R, A>(), arg);
      application_.Tree_2.Accept(new TreeVisitor<R, A>(), arg);
      return default(R);
    }

    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="literal_">Insert description for literal_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.Literal literal_, A arg)
    {
      /* Code For Literal Goes Here */
      literal_.Lit_.Accept(new LitVisitor<R, A>(), arg);
      return default(R);
    }
    
    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="metavariable_">Insert description for metavariable_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.MetaVariable metavariable_, A arg)
    {
      // Code For MetaVariable Goes Here 
      // metavariable_.Integer_
      return default(R);
    }

    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="function_">Insert description for function_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.Function function_, A arg)
    {
      // Code For Function Goes Here
      // function_.Ident_
      return default(R);
    }
  }

  /// <summary>
  /// No description.
  /// </summary>
  /// <typeparam name="R">Insert description for R.</typeparam>
  /// <typeparam name="A">Insert description for A.</typeparam>
  public class LitVisitor<R, A> : AbstractLitVisitor<R, A>
  {
    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="intliteral_">Insert description for intliteral_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.IntLiteral intliteral_, A arg)
    {
      // Code For IntLiteral Goes Here
      // intliteral_.Integer_
      return default(R);
    }

    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="floatliteral_">Insert description for floatliteral_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.FloatLiteral floatliteral_, A arg)
    {
      // Code For FloatLiteral Goes Here 
      // floatliteral_.Double_
      return default(R);
    }
 
    /// <summary>
    /// Insert description for Visit.
    /// </summary>
    /// <param name="stringliteral_">Insert description for stringliteral_.</param>
    /// <param name="arg">Insert description for arg.</param>
    /// <returns>No description.</returns>
    public override R Visit(CSPGF.Trees.Absyn.StringLiteral stringliteral_, A arg)
    {
      // Code For StringLiteral Goes Here 
      // stringliteral_.String_
      return default(R);
    }
  }
  #endregion
  
  #region Token types

  #endregion
}
