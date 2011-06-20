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
        List<LinTrie> curLvl;
        List<ParseTrie> curParse;
        public Linearizer()
        {
            curLvl = new List<LinTrie>();
            curParse = new List<ParseTrie>();
        }

        public String Linearize(LinTrie tree)
        {
            curLvl.Clear();
            curLvl.Add(tree);
            while (curLvl.Count != 0) {
                sentence += Linearizer2(curLvl);
                NextLevel(curLvl);
            }
            return sentence;
        }

        private void NextLevel(Object trie)
        {
            if (trie is LinTrie) {
                List<LinTrie> newlvl = new List<LinTrie>();
                foreach (LinTrie lt in curLvl) {
                    foreach (LinTrie lt2 in lt.child) {
                        if (lt2 != null) {
                            newlvl.Add(lt2);
                        }
                    }
                }
                curLvl = newlvl;
            }
            else if (trie is ParseTrie) {
                List<ParseTrie> newlvl = new List<ParseTrie>();
                foreach (ParseTrie lt in curParse) {
                    foreach (ParseTrie lt2 in lt.child) {
                        if (lt2 != null) {
                            newlvl.Add(lt2);
                        }
                    }
                }
                curParse = newlvl;
            }
        }

        private String Linearizer2(List<LinTrie> trees)
        {
            String tmp = "";
            for (int i = 0 ; i < trees.Count ; i++) {
                if (trees[i].symbol is AlternToksSymbol) {
                    if (i < (trees.Count - 1) && trees[i + 1].symbol is ToksSymbol) {
                        tmp += ATSym2St((AlternToksSymbol)trees[i].symbol, (ToksSymbol)trees[i + 1].symbol);
                        i++;
                    } else {
                        throw new LinearizerException("fail med altsymbol, i = "+i+ " nextSym = "+trees[i+1].symbol.GetType());
                    }
                } else if (trees[i].symbol is ToksSymbol) {
                    foreach (String str in ((ToksSymbol)trees[i].symbol).tokens) {
                        tmp += str + " ";
                    }
                    tmp = tmp.TrimEnd();
                } else {
                    throw new LinearizerException("ohanterad typ av symbol: "+trees[i].symbol.GetType());
                }
            }
            return tmp;
        }

        private String ATSym2St(AlternToksSymbol s, ToksSymbol nextToken)
        {
            // Should be possible to optimise this...
            String tmp = "";
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

        public LinTrie Parse2Lin(ParseTrie pt)
        {
            LinTrie lt = new LinTrie();
            curParse.Clear();
            curParse.Add(pt);
            foreach (ParseTrie ptt in pt.child) {
                lt.child.Add(P2L(ptt));
            }
            return null;
        }

        public LinTrie P2L(ParseTrie pt) {
            
            return null;
        }
    }

    class LinTrie
    {
        public List<LinTrie> child { get; set; }
        public reader.Symbol symbol { get; set; }
        public LinTrie()
        {
            child = new List<LinTrie>();
        }
    }
}