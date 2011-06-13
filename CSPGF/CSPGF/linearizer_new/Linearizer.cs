using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;
using CSPGF.parser_new;

namespace CSPGF.linearizer_new
{
    class Linearizer
    {
        String sentence = "";
        List<parser_new.ParseTrie> curLvl;
        public Linearizer()
        {

        }

        public String Linearize(ParseTrie tree)
        {
            curLvl = new List<parser_new.ParseTrie>();
            curLvl.Add(tree);
            while (curLvl.Count != 0) {
                sentence += Linearizer2(curLvl);
                NextLevel();
            }
            return sentence;
        }

        private void NextLevel()
        {
            List<ParseTrie> newlvl = new List<ParseTrie>();
            foreach (ParseTrie pt in curLvl) {
                foreach (ParseTrie pt2 in pt.child) {
                    if (pt2 != null) {
                        newlvl.Add(pt2);
                    }
                }
            }
            curLvl = newlvl;
        }

        private String Linearizer2(List<ParseTrie> trees)
        {
            String tmp = "";
            for (int i = 0; i < trees.Count; i++) {
                if (trees[i].symbol is AlternToksSymbol) {
                    if (trees[i + 1].symbol is ToksSymbol) {
                        tmp += ATSym2St((AlternToksSymbol)trees[i].symbol, (ToksSymbol)trees[i + 1].symbol);
                    }
                    else {
                        throw new LinearizerException("fail med altsymbol");
                    }
                }
                else if (trees[i].symbol is ToksSymbol) {
                    tmp += ((ToksSymbol)trees[i].symbol).tokens[0];
                }
            }
            return tmp;
            /*foreach (ParseTrie pt in trees) {
                if (pt.symbol is AlternToksSymbol) {
                    // We need to find the next symbol to be able to determine the alternatives.
                    tmp += ATSym2St((AlternToksSymbol)pt.symbol,null);
                }
                else if (pt.symbol is ToksSymbol) {
                    tmp += ((ToksSymbol)pt.symbol).tokens[0];
                }
            }*/

        }

        private String ATSym2St(AlternToksSymbol s, ToksSymbol nextToken)
        {
            // Should be possible to optimise this...
            String tmp="";
            foreach (Alternative alt in s.alts) {
                foreach (String str in nextToken.tokens) {
                    foreach (String str2 in alt.alt2) {
                        if (str.StartsWith(str2)) {
                            foreach (String str3 in alt.alt1) {
                                tmp += str3 + " ";
                            }
                            return tmp.TrimEnd();
                        }
                    }
                }
            }
            foreach (String str4 in s.tokens) {
                tmp += str4 + " ";
            }
            return tmp.TrimEnd();
        }
    }
}
