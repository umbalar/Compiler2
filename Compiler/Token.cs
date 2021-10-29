namespace Compiler
{
    class Token
    {
        public int lineNumber;
        public int charNumber;
        public Lexer.Type_lex lexType;
        public string sourceCode;
        public dynamic meaning;
        public Token(int lN, int cN, Lexer.Type_lex lT, string sC, dynamic m)
        {
            lineNumber = lN;
            charNumber = cN;
            lexType = lT;
            sourceCode = sC;
            meaning = m;
        }
    }
}
