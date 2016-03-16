abstract Test = {

cat
  S; Symb; Var;
  
fun
  str : String -> S;
  int : Int   -> S;
  flt : Float -> S;
  
  name : Symb -> S;
  double : Float -> S;
  
  forall : (Var -> S) -> S;
  var : Var -> S;

fun
  MkSymb : String -> Symb;

}
