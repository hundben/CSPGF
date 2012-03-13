concrete TestCnc of Test = {

lincat
  S,Symb,Var  = {s : Str};

lin
  str s = {s="str (" ++ s.s ++ ")"} ;
  int s = {s="int (" ++ s.s ++ ")"} ;
  flt s = {s="flt (" ++ s.s ++ ")"} ;

  name s = {s="name (" ++ s.s ++ ")"} ;
  double s = {s=s.s ++ s.s} ;

  forall s = {s="forall" ++ s.$0 ++ "(" ++ s.s ++ ")"} ;
  var v = {s="var (" ++ v.s ++ ")"} ;

  MkSymb s = s;

}
