/*** BNFC-Generated Abstract Visitor Design Pattern Skeleton. ***/
/* This implements the common visitor design pattern.
   Replace the R and A parameters with the desired return
   and context types.*/

namespace CSPGF.Trees.VisitSkeleton
{
  #region Classes
  public abstract class AbstractTreeVisitor<R, A> : CSPGF.Trees.Absyn.Tree.Visitor<R, A>
  {
    public abstract R Visit(CSPGF.Trees.Absyn.Lambda lambda_, A arg);
 
    public abstract R Visit(CSPGF.Trees.Absyn.Variable variable_, A arg);
 
    public abstract R Visit(CSPGF.Trees.Absyn.Application application_, A arg);
 
    public abstract R Visit(CSPGF.Trees.Absyn.Literal literal_, A arg);
 
    public abstract R Visit(CSPGF.Trees.Absyn.MetaVariable metavariable_, A arg);
 
    public abstract R Visit(CSPGF.Trees.Absyn.Function function_, A arg);
  }
 
  public abstract class AbstractLitVisitor<R, A> : CSPGF.Trees.Absyn.Lit.Visitor<R, A>
  {
    public abstract R Visit(CSPGF.Trees.Absyn.IntLiteral intliteral_, A arg);
 
    public abstract R Visit(CSPGF.Trees.Absyn.FloatLiteral floatliteral_, A arg);
 
    public abstract R Visit(CSPGF.Trees.Absyn.StringLiteral stringliteral_, A arg);
  }
  #endregion
  
  #region Token types

  #endregion
}
