using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class CncFun
    {
        public String name { get; private set; }
        public Sequence[] sequences { get; private set; }

        public CncFun(String _name, Sequence[] _sequences)
        {
            name = _name;
            sequences = _sequences;
        }

        public Sequence GetSequence(int index)
        {
            return sequences[index];
        }

        public Symbol GetSymbol(int seqIndex, int symbIndex)
        {
            return sequences[seqIndex].symbs[symbIndex];
        }

        public int Length()
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