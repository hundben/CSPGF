/*** BNFC-Generated Abstract Visitor Design Pattern Skeleton. ***/
/* This implements the common visitor design pattern.
   Replace the R and A parameters with the desired return
   and context types.*/

namespace CSPGF.trees.VisitSkeleton
{
  #region Classes
  public abstract class AbstractTreeVisitor<R,A> : CSPGF.trees.Absyn.Tree.Visitor<R,A>
  {
    public abstract R Visit(CSPGF.trees.Absyn.Lambda lambda_, A arg);
 
    public abstract R Visit(CSPGF.trees.Absyn.Variable variable_, A arg);
 
    public abstract R Visit(CSPGF.trees.Absyn.Application application_, A arg);
 
    public abstract R Visit(CSPGF.trees.Absyn.Literal literal_, A arg);
 
    public abstract R Visit(CSPGF.trees.Absyn.MetaVariable metavariable_, A arg);
 
    public abstract R Visit(CSPGF.trees.Absyn.Function function_, A arg);
  }
 
  public abstract class AbstractLitVisitor<R,A> : CSPGF.trees.Absyn.Lit.Visitor<R,A>
  {
    public abstract R Visit(CSPGF.trees.Absyn.IntLiteral intliteral_, A arg);
 
    public abstract R Visit(CSPGF.trees.Absyn.FloatLiteral floatliteral_, A arg);
 
    public abstract R Visit(CSPGF.trees.Absyn.StringLiteral stringliteral_, A arg);
  }
  #endregion
  
  #region Token types

  #endregion
}
