using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Abstract
    {
        public String name { get; private set; } 
        private Dictionary<String, RLiteral> flags;
        public AbsFun[] absFuns { get; private set; }
        public AbsCat[] absCats { get; private set; }

        public Abstract(String _name, Dictionary<String, RLiteral> _flags, AbsFun[] _absFuns, AbsCat[] _absCats)
        {
            name = _name;
            flags = _flags;
            absFuns = _absFuns;
            absCats = _absCats;
        }

        public String StartCat()
        {
            RLiteral cat = flags["startcat"];
            if (cat == null)
                return "Sentence";
            else
                return ((StringLit)cat).getValue();

        }

        public String ToString()
        {
            String ss = "Name : " + name + " , Flags : (";
            // TODO: Är bortkommenterat i javakoden också kanske borde fixa?
            // for(int i=0; i<flags.length;i++)
            // 	ss+=(" "+flags[i].toString());
            ss += ") , Abstract Functions : (";
            foreach (AbsFun a in absFuns) {
                ss += " " + a.ToString();
            }
            ss += ") , Abstract Categories : (";
            foreach (AbsCat a in absCats) {
                ss += " " + a.ToString();
            }
            ss += ")";
            return ss;
        }
    }
}
