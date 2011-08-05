using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Tuple of active item and int (to simplify)
namespace CSPGF.parser
{
    class ActiveItemInt
    {
        public ActiveItem item;
        public int cons;
        public int cons2;
        public ActiveItemInt(ActiveItem _item, int _cons)
        {
            item = _item;
            cons = _cons;
            cons2 = -1;  //for safety
        }
        public /*override*/ bool Equals(ActiveItemInt i)
        {

            if (i.cons == cons && i.item.Equals(item)) return true;
            return false;
        }
        public /*override*/ bool Equals(ActiveItem _item, int _cons)
        {
            if (cons == _cons && item.Equals(_item)) return true;
            return false;
        }
    }
}
