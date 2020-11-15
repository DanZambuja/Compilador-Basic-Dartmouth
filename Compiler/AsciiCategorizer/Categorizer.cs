using System;

namespace Compiler.AsciiCategorizer
{
    public enum AtomType 
    {
        LETTER,
        DIGIT,
        SPECIAL,
        CONTROL
    }

    public class AsciiAtom
    {
        public char Symbol { get; set; }
        public AtomType Category { get; set; }
    }

    public delegate void CategorizedSymbol(AsciiAtom symbol);

    public class Categorizer {
        public event CategorizedSymbol NotifySymbolCategorization;
        protected virtual void OnCategorizedSymbol(AsciiAtom symbol) {
            NotifySymbolCategorization?.Invoke(symbol);
        }
        public void Categorize(char symbol) {
            int numericalValue = (int)symbol;
            if ((numericalValue >= 97 && numericalValue <= 122) || 
                 numericalValue >= 65 && numericalValue <= 90) {

                OnCategorizedSymbol(new AsciiAtom {
                    Symbol = symbol,
                    Category = AtomType.LETTER
                });
            }                
            else if ((numericalValue >= 48 && numericalValue <= 57)) {

                OnCategorizedSymbol(new AsciiAtom {
                    Symbol = symbol,
                    Category = AtomType.DIGIT
                });
            }
            else if ((numericalValue >= 33 && numericalValue <= 47) || 
                     (numericalValue >= 58 && numericalValue <= 64) ||
                     (numericalValue >= 91 && numericalValue <= 96) ||
                     (numericalValue >= 124 && numericalValue <= 126)) {

                OnCategorizedSymbol(new AsciiAtom {
                    Symbol = symbol,
                    Category = AtomType.SPECIAL
                });
            }
            else if ((numericalValue == 10 || numericalValue == 32)) {

                OnCategorizedSymbol(new AsciiAtom {
                    Symbol = symbol,
                    Category = AtomType.CONTROL
                });
            }
            else {
                
                throw new Exception("Unrecognized symbol");
            }
        }
    }
}