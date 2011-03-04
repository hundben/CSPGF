using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Abstract
    {
        private String name;
        private Dictionary<String, RLiteral> flags;
        private AbsFun[] absFuns;
        private AbsCat[] absCats;

        public Abstract(String _name, Dictionary<String, RLiteral> _flags, AbsFun[] _absFuns, AbsCat[] _absCats)
        {
            name = _name;
            flags = _flags;
            absFuns = _absFuns;
            absCats = _absCats;
        }
        public String GetName()
        {
            return name;
        }

        public String StartCat()
        {
            RLiteral cat = flags["startcat"];
            if (cat == null)
                return "Sentence";
            else
                return ((StringLit)cat).getValue();

        }

        public AbsFun[] GetAbsFuns()
        {
            return absFuns;
        }
        public AbsCat[] GetAbsCats()
        {
            return absCats;
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
