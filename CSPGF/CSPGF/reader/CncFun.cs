using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class CncFun
    {
        private String name;
        private Sequence[] sequences;

        public CncFun(String _name, Sequence[] _sequences)
        {
            name = _name;
            sequences = _sequences;
        }

        public String GetName()
        {
            return name;
        }

        public Sequence[] GetSequences()
        {
            return sequences;
        }

        public Sequence GetSequence(int index)
        {
            return sequences[index];
        }

        public Symbol GetSymbol(int seqIndex, int symbIndex)
        {
            return sequences[seqIndex].getSymbol(symbIndex);
        }

        public int GetSize()
        {
            return sequences.Length;
        }

        public String ToString()
        {
            String ss = "Name : " + name + " , Indices : ";
            foreach (Sequence s in sequences) {
                ss += " " + s;
            }
            return ss;
        }
    }
}