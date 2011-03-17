using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class TreeBuilder
    {

        public List<Tree> buildTrees(Chart chart, CSPGF.reader.CncCat startCat, int length) {
            //TODO
            return new List<Tree>();
        }
        public List<Tree> MkTreesForCat(int cat, Chart chart)
        {
            //TODO
            return new List<Tree>();
        }
        public List<Tree> MkTreesForProduction(CSPGF.reader.Production p, Chart chart)
        {
            //TODO
            return new List<Tree>();
        }
        public List<List<Tree>> ListMixer(List<List<Tree>> l)
        {
            //TODO
            return new List<List<Tree>>();
        }
    }
}

//object TreeBuilder {

//  //val log = Logger.getLogger("org.grammaticalframework.parser.TreeBuilder")

//  def buildTrees( chart:Chart, startCat:CncCat, length:Int ):List[Tree] = {
//    //log.fine("Building trees with start category " + (0, startCat, 0, length))
//    (startCat.firstID until startCat.lastID + 1).flatMap( catID =>
//      chart.getCategory(catID, 0, 0, length) match {
//        case None => Nil
//        case Some(cat) => mkTreesForCat(cat, chart)
//      }).toList
//  }

//  def mkTreesForCat(cat : Int, chart:Chart):List[Tree] = {
//    //log.fine("Making trees for category "+ cat)
//    for {p <- chart.getProductions(cat).toList;
//         t <- mkTreesForProduction(p, chart)}
//    yield t
//  }

//  def mkTreesForProduction( p:Production, chart:Chart):List[Tree] = {
//      if (p.domain.length == 0)
//         List(new Application(p.function.name, Nil))
//      else
//         for (args <- listMixer( p.domain.toList.map(mkTreesForCat(_,chart)) ) )
//         yield new Application(p.function.name, args)
//  }

//  def listMixer(l:List[List[Tree]]):List[List[Tree]] = l match {
//    case Nil => Nil
//    case List(subL) => subL.map(List(_))
//    case head::tail => {
//      for {first <- head;
//           then <- listMixer(tail)}
//      yield first::then
//    }
//  }
    
  
//}