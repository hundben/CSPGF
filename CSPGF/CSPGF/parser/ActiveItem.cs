using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class ActiveItem
    {
    }
}

//private class ActiveItem(val begin : Int,
//                 val category:Int,
//                 val function:CncFun,
//                 val domain:Array[Int],
//                 val constituent:Int,
//                 val position:Int) {

//  // get next symbol
//  def nextSymbol():Option[Symbol] =
//    if (position < function.sequence(constituent).length) {
//      val symbol = function.sequence(constituent).symbol(position)
//      return Some(symbol)
//    }
//    else
//      return None

//  /* ************************************ *
//   * Overrides
//   * ************************************ */

//  override def equals(o:Any):Boolean = o match {
//      case (o:ActiveItem) => this.begin == o.begin &&
//                             this.category == o.category &&
//                             this.function == o.function && // CncFun,
//                             this.domain.deep.equals(o.domain.deep) &&
//                             this.constituent == o.constituent &&
//                             this.position == o.position
//      case _ => false
//  }

//  override def toString() =
//    "[" + this.begin + ";" +
//          this.category + "->" + this.function.name +
//          "[" + this.domainToString + "];" + this.constituent + ";" +
//          this.position + "]"
          
//  def domainToString():String = {
//    val buffer = new StringBuffer()
//    for( d <- domain ) {
//      buffer.append(d.toString)
//    }
//    buffer.toString
//  }
//}