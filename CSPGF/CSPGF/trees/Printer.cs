/*** BNFC-Generated Pretty Printer and Abstract Syntax Viewer ***/
 
using System;
using System.Text; // for StringBuilder
using CSPGF.trees.Absyn;
 
namespace CSPGF.trees
{
  #region Pretty-printer class
  public class PrettyPrinter
  {
    #region Misc rendering functions
    // You may wish to change these:
    private const int BUFFER_INITIAL_CAPACITY = 2000;
    private const string LEFT_PARENTHESIS = "(";
    private const string RIGHT_PARENTHESIS = ")";
    private static System.Globalization.NumberFormatInfo InvariantFormatInfo = System.Globalization.NumberFormatInfo.InvariantInfo;
    
    private static int _n_ = 0;
    private static StringBuilder buffer = new StringBuilder(BUFFER_INITIAL_CAPACITY);
    
    //You may wish to change render
    private static void Render(String s)
    {
      if(s == "{")
      {
        buffer.Append("\n");
        Indent();
        buffer.Append(s);
        _n_ = _n_ + 2;
        buffer.Append("\n");
        Indent();
      }
      else if(s == "(" || s == "[")
        buffer.Append(s);
      else if(s == ")" || s == "]")
      {
        Backup();
        buffer.Append(s);
        buffer.Append(" ");
      }
      else if(s == "}")
      {
        _n_ = _n_ - 2;
        Backup();
        Backup();
        buffer.Append(s);
        buffer.Append("\n");
        Indent();
      }
      else if(s == ",")
      {
        Backup();
        buffer.Append(s);
        buffer.Append(" ");
      }
      else if(s == ";")
      {
        Backup();
        buffer.Append(s);
        buffer.Append("\n");
        Indent();
      }
      else if(s == "") return;
      else
      {
        // Make sure escaped characters are printed properly!
        if(s.StartsWith("\"") && s.EndsWith("\""))
        {
          buffer.Append('"');
          StringBuilder sb = new StringBuilder(s);
          // Remove enclosing citation marks
          sb.Remove(0,1);
          sb.Remove(sb.Length-1,1);
          // Note: we have to replace backslashes first! (otherwise it will "double-escape" the other escapes)
          sb.Replace("\\", "\\\\");
          sb.Replace("\n", "\\n");
          sb.Replace("\t", "\\t");
          sb.Replace("\"", "\\\"");
          buffer.Append(sb.ToString());
          buffer.Append('"');
        }
        else
        {
          buffer.Append(s);
        }
        buffer.Append(" ");
      }
    }
    
    private static void PrintInternal(int n, int _i_)
    {
      buffer.Append(n.ToString(InvariantFormatInfo));
      buffer.Append(' ');
    }
    
    private static void PrintInternal(double d, int _i_)
    {
      buffer.Append(d.ToString(InvariantFormatInfo));
      buffer.Append(' ');
    }
    
    private static void PrintInternal(string s, int _i_)
    {
      Render(s);
    }
    
    private static void PrintInternal(char c, int _i_)
    {
      PrintQuoted(c);
    }
    
    
    private static void ShowInternal(int n)
    {
      Render(n.ToString(InvariantFormatInfo));
    }
    
    private static void ShowInternal(double d)
    {
      Render(d.ToString(InvariantFormatInfo));
    }
    
    private static void ShowInternal(char c)
    {
      PrintQuoted(c);
    }
    
    private static void ShowInternal(string s)
    {
      PrintQuoted(s);
    }
    
    
    private static void PrintQuoted(string s)
    {
      Render("\"" + s + "\"");
    }
    
    private static void PrintQuoted(char c)
    {
      // Makes sure the character is escaped properly before printing it.
      string str = c.ToString();
      if(c == '\n') str = "\\n";
      if(c == '\t') str = "\\t";
      Render("'" + str + "'");
    }
    
    private static void Indent()
    {
      int n = _n_;
      while (n > 0)
      {
        buffer.Append(' ');
        n--;
      }
    }
    
    private static void Backup()
    {
      if(buffer[buffer.Length - 1] == ' ')
      {
        buffer.Length = buffer.Length - 1;
      }
    }
    
    private static void Trim()
    {
      while(buffer.Length > 0 && buffer[0] == ' ')
        buffer.Remove(0, 1); 
      while(buffer.Length > 0 && buffer[buffer.Length-1] == ' ')
        buffer.Remove(buffer.Length-1, 1);
    }
    
    private static string GetAndReset()
    {
      Trim();
      string strReturn = buffer.ToString();
      Reset();
      return strReturn;
    }
    
    private static void Reset()
    {
      buffer.Remove(0, buffer.Length);
    }
    #endregion
    
    #region Print Entry Points
    public static string Print(CSPGF.trees.Absyn.Tree cat)
    {
      PrintInternal(cat, 0);
      return GetAndReset();
    }
 
    public static string Print(CSPGF.trees.Absyn.Lit cat)
    {
      PrintInternal(cat, 0);
      return GetAndReset();
    }
    #endregion
    
    #region Show Entry Points
    public static String Show(CSPGF.trees.Absyn.Tree cat)
    {
      ShowInternal(cat);
      return GetAndReset();
    }
 
    public static String Show(CSPGF.trees.Absyn.Lit cat)
    {
      ShowInternal(cat);
      return GetAndReset();
    }
    #endregion
    
    #region (Internal) Print Methods
    private static void PrintInternal(CSPGF.trees.Absyn.Tree p, int _i_)
    {
      if(p is CSPGF.trees.Absyn.Lambda)
      {
        CSPGF.trees.Absyn.Lambda _lambda = (CSPGF.trees.Absyn.Lambda)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        Render("\\");
        PrintInternal(_lambda.Ident_, 0);
        Render("->");
        PrintInternal(_lambda.Tree_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
      else if(p is CSPGF.trees.Absyn.Variable)
      {
        CSPGF.trees.Absyn.Variable _variable = (CSPGF.trees.Absyn.Variable)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        Render("$");
        PrintInternal(_variable.Integer_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
      else if(p is CSPGF.trees.Absyn.Application)
      {
        CSPGF.trees.Absyn.Application _application = (CSPGF.trees.Absyn.Application)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        Render("(");
        PrintInternal(_application.Tree_1, 0);
        PrintInternal(_application.Tree_2, 0);
        Render(")");
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
      else if(p is CSPGF.trees.Absyn.Literal)
      {
        CSPGF.trees.Absyn.Literal _literal = (CSPGF.trees.Absyn.Literal)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        PrintInternal(_literal.Lit_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
      else if(p is CSPGF.trees.Absyn.MetaVariable)
      {
        CSPGF.trees.Absyn.MetaVariable _metavariable = (CSPGF.trees.Absyn.MetaVariable)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        Render("META_");
        PrintInternal(_metavariable.Integer_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
      else if(p is CSPGF.trees.Absyn.Function)
      {
        CSPGF.trees.Absyn.Function _function = (CSPGF.trees.Absyn.Function)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        PrintInternal(_function.Ident_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
    }
 
    private static void PrintInternal(CSPGF.trees.Absyn.Lit p, int _i_)
    {
      if(p is CSPGF.trees.Absyn.IntLiteral)
      {
        CSPGF.trees.Absyn.IntLiteral _intliteral = (CSPGF.trees.Absyn.IntLiteral)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        PrintInternal(_intliteral.Integer_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
      else if(p is CSPGF.trees.Absyn.FloatLiteral)
      {
        CSPGF.trees.Absyn.FloatLiteral _floatliteral = (CSPGF.trees.Absyn.FloatLiteral)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        PrintInternal(_floatliteral.Double_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
      else if(p is CSPGF.trees.Absyn.StringLiteral)
      {
        CSPGF.trees.Absyn.StringLiteral _stringliteral = (CSPGF.trees.Absyn.StringLiteral)p;
        if(_i_ > 0) Render(LEFT_PARENTHESIS);
        PrintInternal(_stringliteral.String_, 0);
        if(_i_ > 0) Render(RIGHT_PARENTHESIS);
      }
    }
    #endregion
    
    #region (Internal) Show Methods
    private static void ShowInternal(CSPGF.trees.Absyn.Tree p)
    {
      if(p is CSPGF.trees.Absyn.Lambda)
      {
        CSPGF.trees.Absyn.Lambda _lambda = (CSPGF.trees.Absyn.Lambda)p;
        Render("(");
        Render("Lambda");
        ShowInternal(_lambda.Ident_);
        ShowInternal(_lambda.Tree_);
        Render(")");
      }
      if(p is CSPGF.trees.Absyn.Variable)
      {
        CSPGF.trees.Absyn.Variable _variable = (CSPGF.trees.Absyn.Variable)p;
        Render("(");
        Render("Variable");
        ShowInternal(_variable.Integer_);
        Render(")");
      }
      if(p is CSPGF.trees.Absyn.Application)
      {
        CSPGF.trees.Absyn.Application _application = (CSPGF.trees.Absyn.Application)p;
        Render("(");
        Render("Application");
        ShowInternal(_application.Tree_1);
        ShowInternal(_application.Tree_2);
        Render(")");
      }
      if(p is CSPGF.trees.Absyn.Literal)
      {
        CSPGF.trees.Absyn.Literal _literal = (CSPGF.trees.Absyn.Literal)p;
        Render("(");
        Render("Literal");
        ShowInternal(_literal.Lit_);
        Render(")");
      }
      if(p is CSPGF.trees.Absyn.MetaVariable)
      {
        CSPGF.trees.Absyn.MetaVariable _metavariable = (CSPGF.trees.Absyn.MetaVariable)p;
        Render("(");
        Render("MetaVariable");
        ShowInternal(_metavariable.Integer_);
        Render(")");
      }
      if(p is CSPGF.trees.Absyn.Function)
      {
        CSPGF.trees.Absyn.Function _function = (CSPGF.trees.Absyn.Function)p;
        Render("(");
        Render("Function");
        ShowInternal(_function.Ident_);
        Render(")");
      }
    }
 
    private static void ShowInternal(CSPGF.trees.Absyn.Lit p)
    {
      if(p is CSPGF.trees.Absyn.IntLiteral)
      {
        CSPGF.trees.Absyn.IntLiteral _intliteral = (CSPGF.trees.Absyn.IntLiteral)p;
        Render("(");
        Render("IntLiteral");
        ShowInternal(_intliteral.Integer_);
        Render(")");
      }
      if(p is CSPGF.trees.Absyn.FloatLiteral)
      {
        CSPGF.trees.Absyn.FloatLiteral _floatliteral = (CSPGF.trees.Absyn.FloatLiteral)p;
        Render("(");
        Render("FloatLiteral");
        ShowInternal(_floatliteral.Double_);
        Render(")");
      }
      if(p is CSPGF.trees.Absyn.StringLiteral)
      {
        CSPGF.trees.Absyn.StringLiteral _stringliteral = (CSPGF.trees.Absyn.StringLiteral)p;
        Render("(");
        Render("StringLiteral");
        ShowInternal(_stringliteral.String_);
        Render(")");
      }
    }
    #endregion
  }
  #endregion
}