using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Failklass som används för test?

namespace CSPGF.reader
{
    public class Reader
    {
        static int readPGFInt()
        {
            System.IO.FileStream filestream = new System.IO.FileStream("test.pgf", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            // attach filestream to binary reader
            System.IO.BinaryReader binaryreader = new System.IO.BinaryReader(filestream);
            int ii = 0;
            int rez = binaryreader.ReadByte();
            if (rez <= 0x7f)
            {
            }
            else
            {
                while (true)
                {
                    ii = binaryreader.ReadByte();
                    rez = (ii << 7) | (rez & 0x7f);
                    if (ii <= 0x7f)
                        return rez;
                }
            }
            return rez;
        }

        static int readSimplePGF()
        {
            // TODO skriva klart
            return 0;
        }
    }
}
