//C# Abstract Syntax Interface generated by the BNF Converter.
namespace CSPGF.Trees.Absyn
{
    #region Token Classes
    public class TokenBaseType
  {
    private string str;
    
    public TokenBaseType(string str)
    {
      this.str = str;
    }
    
    public override string ToString()
    {
      return this.str;
    }
  }
  
  #endregion
  
  #region Abstract Syntax Classes
  public abstract class Tree
  {
    public abstract R Accept<R,A>(Visitor<R,A> v, A arg);
    
    public interface Visitor<R,A>
    {
      R Visit(Lambda p, A arg);

      R Visit(Variable p, A arg);

      R Visit(Application p, A arg);

      R Visit(Literal p, A arg);

      R Visit(MetaVariable p, A arg);

      R Visit(Function p, A arg);
    }
  }
 
  public abstract class Lit
  {
    public abstract R Accept<R,A>(CSPGF.Trees.Absyn.Lit.Visitor<R,A> v, A arg);
    
    public interface Visitor<R,A>
    {
      R Visit(IntLiteral p, A arg);
      R Visit(FloatLiteral p, A arg);
      R Visit(StringLiteral p, A arg);
    }
  }
  
  public class Lambda : Tree
  {
      public Lambda(string p1, Tree p2)
    {
      Ident_ = p1;
      Tree_ = p2;
    }

      public string Ident_ { get; set; }

      public Tree Tree_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is Lambda)
      {
        return this.Equals((Lambda)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(Lambda obj)
    {
      if (this == obj)
      {
        return true;
      }

      return this.Ident_.Equals(obj.Ident_) && this.Tree_.Equals(obj.Tree_);
    }
    
    public override int GetHashCode()
    {
      return 37*this.Ident_.GetHashCode()+this.Tree_.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class Variable : Tree
  {
      public Variable(int p1)
    {
      Integer_ = p1;
    }

      public int Integer_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is Variable)
      {
        return this.Equals((Variable)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(Variable obj)
    {
      if (this == obj)
      {
        return true;
      }

      return this.Integer_.Equals(obj.Integer_);
    }
    
    public override int GetHashCode()
    {
      return this.Integer_.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class Application : Tree
  {
      public Application(Tree p1, Tree p2)
    {
      Tree_1 = p1;
      Tree_2 = p2;
    }

      public Tree Tree_1 { get; set; }

      public Tree Tree_2 { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is Application)
      {
        return this.Equals((Application)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(Application obj)
    {
      if (this == obj)
      {
        return true;
      }
      return this.Tree_1.Equals(obj.Tree_1) && this.Tree_2.Equals(obj.Tree_2);
    }
    
    public override int GetHashCode()
    {
      return 37*this.Tree_1.GetHashCode()+this.Tree_2.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class Literal : Tree
  {
      public Literal(Lit p1)
    {
      Lit_ = p1;
    }

      public Lit Lit_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is Literal)
      {
        return this.Equals((Literal)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(Literal obj)
    {
      if (this == obj)
      {
        return true;
      }
      return this.Lit_.Equals(obj.Lit_);
    }
    
    public override int GetHashCode()
    {
      return this.Lit_.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class MetaVariable : Tree
  {
      public MetaVariable(int p1)
    {
      Integer_ = p1;
    }

      public int Integer_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is MetaVariable)
      {
        return this.Equals((MetaVariable)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(MetaVariable obj)
    {
      if (this == obj)
      {
        return true;
      }

      return this.Integer_.Equals(obj.Integer_);
    }
    
    public override int GetHashCode()
    {
      return this.Integer_.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class Function : Tree
  {
      public Function(string p1)
    {
      Ident_ = p1;
    }

      public string Ident_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is Function)
      {
        return this.Equals((Function)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(Function obj)
    {
      if (this == obj)
      {
        return true;
      }

      return this.Ident_.Equals(obj.Ident_);
    }
    
    public override int GetHashCode()
    {
      return this.Ident_.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class IntLiteral : Lit
  {
      public IntLiteral(int p1)
    {
      Integer_ = p1;
    }

      public int Integer_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is IntLiteral)
      {
        return this.Equals((IntLiteral)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(IntLiteral obj)
    {
      if (this == obj)
      {
        return true;
      }

      return this.Integer_.Equals(obj.Integer_);
    }
    
    public override int GetHashCode()
    {
      return this.Integer_.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class FloatLiteral : Lit
  {
      public FloatLiteral(double p1)
    {
      Double_ = p1;
    }

      public double Double_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is FloatLiteral)
      {
        return this.Equals((FloatLiteral)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(FloatLiteral obj)
    {
      if (this == obj)
      {
        return true;
      }

      return this.Double_.Equals(obj.Double_);
    }
    
    public override int GetHashCode()
    {
      return this.Double_.GetHashCode();
    }
    
    public override R Accept<R,A>(Visitor<R,A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
 
  public class StringLiteral : Lit
  {
      public StringLiteral(string p1)
    {
      String_ = p1;
    }

      public string String_ { get; set; }

      public override bool Equals(object obj)
    {
      if (this == obj)
      {
        return true;
      }

      if (obj is StringLiteral)
      {
        return this.Equals((StringLiteral)obj);
      }

      return base.Equals(obj);
    }
    
    public bool Equals(StringLiteral obj)
    {
      if (this == obj)
      {
        return true;
      }

      return this.String_.Equals(obj.String_);
    }
    
    public override int GetHashCode()
    {
      return this.String_.GetHashCode();
    }
    
    public override R Accept<R, A>(Visitor<R, A> visitor, A arg)
    {
      return visitor.Visit(this, arg);
    }
  }
  
  #region Lists
  #endregion
  #endregion
}