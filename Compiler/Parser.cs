using System;

namespace Compiler
{
    class Parser
    {
        private Lexer lexer;
        private Token currentToken;
        private Token err;

        private enum Type_lex
        {
            identifier, integer_literal_int, real_literal_double
        };

        private readonly string tree;
        private Lexer Lexer
        {
            get => lexer;
            set
            {
                lexer = value;
            }
        }

        public Parser(Lexer lexer)
        {
            Lexer = lexer;
            currentToken = Lexer.GetNextToken();
            if (currentToken.lexType != Lexer.Type_lex.end_of_file)
            {
                tree = ParseExpression().OutputTree(err);
            }
        }

        public string GetTree()
        {
            return tree;
        }

        private bool IsOperation(string str)
            => str == "+" || str == "-" || str == "*" || str == "/";

        private void OutputER(Token token)
        {
            err = new Token(token.lineNumber, token.charNumber, Lexer.Type_lex.err, "Expected Identificator or Number", "");
        }
        private Node ParseExpression()
        {
            dynamic left = ParseTerm();
            Token operation = currentToken;

            while ((Convert.ToString(operation.meaning) == "+") || (Convert.ToString(operation.meaning) == "-"))
            {
                currentToken = Lexer.GetNextToken();
                dynamic right = ParseTerm();

                left = new BinOpNode(operation, left, right);

                operation = currentToken;
            }
            if (Convert.ToString(left.GetType()).Contains("BinOpNode") || Convert.ToString(left.GetType()).Contains("NumNode"))
            {
                return left;
            }
            else
            {
                OutputER(operation);
                return new BinOpNode(operation);
            }
        }

        private dynamic ParseTerm()
        {
            dynamic left = ParseFactor();
            Token operation = currentToken;

            while ((Convert.ToString(operation.meaning) == "*") || (Convert.ToString(operation.meaning) == "/"))
            {
                currentToken = Lexer.GetNextToken();

                dynamic right = ParseFactor();

                left = new BinOpNode(operation, left, right);
                operation = currentToken;
            }

            return left;
        }

        private dynamic ParseFactor()
        {
            Token token = currentToken;
            currentToken = Lexer.GetNextToken();
            if (token.lexType == Lexer.Type_lex.integer_literal_int || token.lexType == Lexer.Type_lex.real_literal_double)
            {
                return new NumNode(token);
            } 
            else if (Convert.ToString(token.meaning) == "(")
            {
                dynamic left = ParseExpression();
                token = currentToken;

                currentToken = Lexer.GetNextToken();

                if (Convert.ToString(token.meaning) != ")")
                {
                    err = new Token(token.lineNumber, token.charNumber + token.sourceCode.Length, Lexer.Type_lex.err, "Expected \")\"", "");
                }
                return left;
            }
            else
            {
                OutputER(token);
                return 0;
            }
        }

    }
}
