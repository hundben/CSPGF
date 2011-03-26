/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;

namespace CSPGF.parser
{
    class TreeBuilder
    {
        public TreeBuilder() { }
        public List<Tree> BuildTrees(Chart chart, CSPGF.reader.CncCat startCat, int length) {
            List<Tree> temp = new List<Tree>();
            for (int catID = startCat.firstFID; catID < startCat.lastFID + 1; catID++)
            {
                int cat = chart.GetCategory(catID, 0, 0, length);
                if (cat != -1)
                {
                    temp.AddRange(MkTreesForCat(cat, chart)); //unsure about this???
                }
            }
            return temp;
        }
        public List<Tree> MkTreesForCat(int cat, Chart chart)
        {
            List<Tree> temp = new List<Tree>();
            foreach (Production p in chart.GetProductions(cat))
            {
                foreach (Tree t in MkTreesForProduction(p, chart))
                {
                    temp.Add(t);
                }
            }
            return temp;
        }
        public List<Tree> MkTreesForProduction(CSPGF.reader.Production p, Chart chart)
        {
            List<Tree> temp = new List<Tree>();
            if (p is ApplProduction)
            {
                ApplProduction prod = (ApplProduction)p;
                
                if (p.Domain().Length == 0)
                {
                    temp.Add(new Application(prod.function.name, new List<Tree>()));
                }
                else
                {
                //retard code >P
                    foreach (int i in p.Domain())
                    {
                        List<Tree> t2 = MkTreesForCat(i, chart);
                        temp.Add(new Application(prod.function.name, t2));
                        //Use listmixer above? or what does listmixer actually do?

                        // for (args <- listMixer( p.domain.toList.map(mkTreesForCat(_,chart)) ) )
                        //         yield new Application(p.function.name, args)
                    }
                }
            }
            return temp;
        }
        public List<List<Tree>> ListMixer(List<List<Tree>> l)
        {
            return l;   //TODO check what this thing actually does
            //foreach (List<Tree> lt in l)
            //{
                //  def listMixer(l:List[List[Tree]]):List[List[Tree]] = l match {
                //    case Nil => Nil
                //    case List(subL) => subL.map(List(_))
                //    case head::tail => {
                //      for {first <- head;
                //           then <- listMixer(tail)}
                //      yield first::then
            //}
            //TODO
            //return new List<List<Tree>>();
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
