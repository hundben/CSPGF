/*** BNFC-Generated Visitor Design Pattern Skeleton. ***/
/* This implements the common visitor design pattern. To make sure that
   compile errors occur when code in the Visitor don't match the abstract
   syntaxt, the "abstract visit skeleton" is used.
   
   Replace the R and A parameters with the desired return
   and context types.*/

namespace CSPGF.trees.VisitSkeleton
{
  #region Classes
  public class TreeVisitor<R,A> : AbstractTreeVisitor<R,A>
  {
    public override R Visit(CSPGF.trees.Absyn.Lambda lambda_, A arg)
    {
      /* Code For Lambda Goes Here */
      // lambda_.Ident_
      lambda_.Tree_.Accept(new TreeVisitor<R,A>(), arg);
      return default(R);
    }
 
    public override R Visit(CSPGF.trees.Absyn.Variable variable_, A arg)
    {
      /* Code For Variable Goes Here */
      // variable_.Integer_
      return default(R);
    }
 
    public override R Visit(CSPGF.trees.Absyn.Application application_, A arg)
    {
      /* Code For Application Goes Here */
      application_.Tree_1.Accept(new TreeVisitor<R,A>(), arg);
      application_.Tree_2.Accept(new TreeVisitor<R,A>(), arg);
      return default(R);
    }
 
    public override R Visit(CSPGF.trees.Absyn.Literal literal_, A arg)
    {
      /* Code For Literal Goes Here */
      literal_.Lit_.Accept(new LitVisitor<R,A>(), arg);
      return default(R);
    }
 
    public override R Visit(CSPGF.trees.Absyn.MetaVariable metavariable_, A arg)
    {
      /* Code For MetaVariable Goes Here */
      // metavariable_.Integer_
      return default(R);
    }
 
    public override R Visit(CSPGF.trees.Absyn.Function function_, A arg)
    {
      /* Code For Function Goes Here */
      // function_.Ident_
      return default(R);
    }
  }
 
  public class LitVisitor<R,A> : AbstractLitVisitor<R,A>
  {
    public override R Visit(CSPGF.trees.Absyn.IntLiteral intliteral_, A arg)
    {
      /* Code For IntLiteral Goes Here */
      // intliteral_.Integer_
      return default(R);
    }
 
    public override R Visit(CSPGF.trees.Absyn.FloatLiteral floatliteral_, A arg)
    {
      /* Code For FloatLiteral Goes Here */
      // floatliteral_.Double_
      return default(R);
    }
 
    public override R Visit(CSPGF.trees.Absyn.StringLiteral stringliteral_, A arg)
    {
      /* Code For StringLiteral Goes Here */
      // stringliteral_.String_
      return default(R);
    }
  }
  #endregion
  
  #region Token types

  #endregion
}
