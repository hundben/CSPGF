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

        /**
         * Accessors
         */
        public String getName()
        {
            return name;
        }

        public Sequence[] getSequences()
        {
            return sequences;
        }

        public Sequence getSequence(int index)
        {
            return sequences[index];
        }

        public Symbol getSymbol(int seqIndex, int symbIndex)
        {
            return sequences[seqIndex].symbol(symbIndex);
        }

        public int getSize()
        {
            return sequences.Length;
        }

        public String toString()
        {
            String ss = "Name : " + name + " , Indices : ";
            foreach (Sequence s in sequences)
            {
                ss += " " + s;
            }
            //for (int i = 0 ; i < sequences.Length ; i++)
            //    ss += (" " + sequences[i]);
            return ss;
        }
    }
}
