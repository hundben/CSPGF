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

        public Abstract(String _name, Dictionary<String, RLiteral> _flags,
            AbsFun[] _absFuns, AbsCat[] _absCats)
        {
            name = _name;
            flags = _flags;
            absFuns = _absFuns;
            absCats = _absCats;
        }
        public String getName()
        {
            return name;
        }

        public String startcat()
        {
            RLiteral cat = flags["startcat"];
            if (cat == null)
                return "Sentence";
            else
                // TODO: OMG RETARDED, får fundera på hur man kan fixa...
                //return ((StringLit)cat).getValue();
                return null;
        }

        public AbsFun[] getAbsFuns()
        {
            return absFuns;
        }
        public AbsCat[] getAbsCats()
        {
            return absCats;
        }

        public String toString()
        {
            String ss = "Name : " + name + " , Flags : (";
            // Är bortkommenterat i javakoden också
            // for(int i=0; i<flags.length;i++)
            // 	ss+=(" "+flags[i].toString());
            ss += ") , Abstract Functions : (";
            for (int i = 0 ; i < absFuns.Length ; i++)
            {
                ss += (" " + absFuns[i].toString());
            }
            ss += ") , Abstract Categories : (";
            for (int i = 0 ; i < absCats.Length ; i++)
            {
                ss += (" " + absCats[i].toString());
            }
            ss += ")";
            return ss;
        }
    }
}
