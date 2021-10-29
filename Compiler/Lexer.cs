﻿using System;
using System.IO;

namespace Compiler
{
    class Lexer
    {
        StreamReader sr;
        private enum States { S, ID, NUM, FNUM, CH, STR, OP, ER};
        private States state;
        private enum Key_words
        {
            @as, @bool, @break, @byte, @case, @catch, @char, @checked, @const, @continue, @decimal, @default, @do, @double, @else, @enum, @false,
            @finally, @float, @for, @foreach, @goto, @if, @in, @int, @is, @long, @namespace, @new, @null, @out, @true, @try, @typeof, @params, @ref,
            @return, @sbyte, @short, @sizeof, @static, @string, @switch, @throw, @uint, @ulong, @unchecked, @unsafe, @ushort, @void, @while

        };

        public enum Type_lex
        {
            key_word, identifier, integer_literal_int, err, real_literal_double, char_literal, string_literal, delimiter, Operator, end_of_file
        };

        private static string[] control_sequence = { "\\\'", "\\\"", "\\\\", "\\0", "\\a", "\\b", "\\f", "\\n", "\\r", "\\t", "\\v"};
        private static char[] insign = { ' ', '\n', '\r', '\t', '\v' };
        private readonly string[] control_sequence_value = { "\'", "\"", "\\", "\0", "\a", "\b", "\f", "\n", "\r", "\t", "\v"};

        private static char[] delimiter = { '(', ')', '[', ']', '{', '}', '.', ',', ';' };

        private static string[] operators = { "+", "-", "*", "/", "%", "&", "|", "^", "!", "~", "=", "<", ">", "++", "--", "&&", "||", "==",
                                            "!=", "<=", ">=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<", "<<=", "=>", ">>", ">>="};
        private string buffer = "";
        char currentChar;

        private int numOfString = 1;
        private int numOfColumn = 1;
        Token currentToken;
        public void Analysis()
        {
            switch (state)
            {
                case States.S:
                    if (buffer == "")
                    {
                        if (sr.Peek() == -1)
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.end_of_file, "", "EndOfFile");
                            break;
                        }
                        else
                        {
                            buffer += (char)sr.Read();
                            numOfColumn++;
                        }
                    }
                    if (Array.IndexOf(insign, buffer[0]) != -1)
                    {
                        if (buffer[0] == '\n')
                        {
                            numOfColumn = 1;
                            numOfString++;
                        }
                        buffer = "";
                        Analysis();
                    }
                    else if (Char.IsLetter(buffer[0]) || buffer[0] == '_')
                    {
                        state = States.ID;
                        Analysis();
                    }
                    else if (char.IsDigit(buffer[0]))
                    {
                        state = States.NUM;
                        Analysis();
                    }
                    else if (buffer[0] == '\'')
                    {
                        state = States.CH;
                        Analysis();
                    }
                    else if (buffer[0] == '"')
                    {
                        state = States.STR;
                        Analysis();
                    }
                    else if (buffer[0] == '.')
                    {
                        state = States.FNUM;
                        Analysis();
                    }
                    else if (Array.IndexOf(delimiter, buffer[0]) != -1)
                    {
                        currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.delimiter, buffer, buffer);
                        buffer = "";
                    }
                    else if (Array.IndexOf(operators, buffer) != -1)
                    {
                        state = States.OP;
                        Analysis();
                    }
                    else
                    {
                        state = States.ER;
                        Analysis();
                    }
                    break;

                case States.ID:
                    if (sr.Peek() != -1)
                    {
                        currentChar = (char)sr.Read();
                        numOfColumn++;
                        if ((Array.IndexOf(insign, currentChar) != -1) || (Array.IndexOf(delimiter, currentChar) != -1) || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                        {
                            currentToken = KWorIdentToken(buffer, false);
                            state = States.S;
                            buffer = "";
                            if (Array.IndexOf(delimiter, currentChar) != -1 || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                            {
                                buffer = "" + currentChar;
                            }
                        }
                        else if (Char.IsLetterOrDigit(currentChar) || currentChar == '_')
                        {
                            buffer += currentChar;
                            Analysis();
                        }
                        else
                        {
                            state = States.ER;
                            Analysis();
                        }
                    }
                    else
                    {
                        currentToken = KWorIdentToken(buffer, true);
                        buffer = "";
                        state = States.S;
                    }
                    break;
                case States.NUM:
                    if (sr.Peek() != -1)
                    {
                        currentChar = (char)sr.Read();
                        numOfColumn++;
                        
                        if (currentChar == '.')
                        {
                            buffer += currentChar;
                            state = States.FNUM;
                            Analysis();
                        }
                        else if ((Array.IndexOf(insign, currentChar) != -1) || (Array.IndexOf(delimiter, currentChar) != -1) || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.integer_literal_int, buffer, Int32.Parse(buffer.Replace("_", "")));
                            state = States.S;
                            buffer = "";
                            if (Array.IndexOf(delimiter, currentChar) != -1 || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                            {
                                buffer = "" + currentChar;
                            }

                        }
                        else if (Char.IsDigit(currentChar) || currentChar == '_')
                        {
                            buffer += currentChar;
                            Analysis();
                        }
                        else
                        {
                            buffer += currentChar;
                            state = States.ER;
                            Analysis();
                        }
                    }
                    else
                    {
                        if (buffer == "-")
                        {
                            currentToken = new Token(numOfString, numOfColumn - 1, Type_lex.Operator, buffer, buffer);
                            buffer = "";
                            state = States.S;
                        }
                        else
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.integer_literal_int, buffer, Int32.Parse(buffer.Replace("_", "")));
                            buffer = "";
                            state = States.S;
                        }
                    }
                    break;
                case States.FNUM:
                    if (sr.Peek() != -1)
                    {
                        currentChar = (char)sr.Read();
                        numOfColumn++;
                        if ((Array.IndexOf(insign, currentChar) != -1) || (Array.IndexOf(delimiter, currentChar) != -1) || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                        {
                            if ((Array.IndexOf(operators, currentChar.ToString()) != -1) && (buffer == "-"))
                            {
                                buffer += currentChar;
                                state = States.OP;
                                Analysis();
                            }
                            else
                            {
                                if (buffer == ".")
                                {
                                    dotActions();
                                }
                                else
                                {
                                    currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.real_literal_double, buffer, double.Parse(buffer.Replace('.', ',').Replace("_", "")));
                                    state = States.S;
                                    buffer = "";
                                }
                                if (Array.IndexOf(delimiter, currentChar) != -1 || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                                {
                                    buffer = "" + currentChar;
                                }
                            }
                        }
                        else if (Char.IsDigit(currentChar) || currentChar == '_')
                        {
                            buffer += currentChar;
                            Analysis();
                        }
                        else if (currentChar == 'd' || currentChar == 'D')
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.real_literal_double, buffer + currentChar, double.Parse(buffer.Replace('.', ',').Replace("_", "")));
                            state = States.S;
                            buffer = "";
                        }
                        else
                        {
                            if (buffer == ".")
                            {
                                dotActions();
                                buffer += currentChar;
                            }
                            else
                            {
                                buffer += currentChar;
                                state = States.ER;
                                Analysis();
                            }
                        }
                    }
                    else
                    {
                        if (buffer == ".")
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.delimiter, buffer, buffer);
                            buffer = "";
                            state = States.S;
                        }
                        else
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.real_literal_double, buffer, double.Parse(buffer.Replace('.', ',').Replace("_", "")));
                            buffer = "";
                            state = States.S;
                        }
                    }
                    break;
                case States.OP:
                    if (sr.Peek() != -1)
                    {
                        currentChar = (char)sr.Read();
                        numOfColumn++;
                        if (Array.IndexOf(operators, currentChar.ToString()) == -1)
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.Operator, buffer, buffer);
                            state = States.S;
                            buffer = "" + currentChar;
                        }
                        else if (Array.IndexOf(operators, buffer + currentChar) != -1)
                        {
                            buffer += currentChar;
                            Analysis();
                        }
                        else
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.Operator, buffer, buffer);
                            buffer = "" + currentChar;
                            state = States.S;
                        }
                    }
                    else
                    {
                        currentToken = new Token(numOfString, numOfColumn, Type_lex.Operator, buffer, buffer);
                        buffer = "";
                        state = States.S;
                    }
                    break;
                case States.CH:
                    if (sr.Peek() != -1)
                    {
                        currentChar = (char)sr.Read();
                        numOfColumn++;
                        if (currentChar == '\'')
                        {
                            buffer += currentChar;
                            string chMeaning = buffer;
                            for (int i = 0; i < control_sequence.Length; i++)
                            {
                                if (buffer.Contains(control_sequence[i]))
                                {
                                    chMeaning = buffer.Replace(control_sequence[i], control_sequence_value[i]);
                                }
                            }
                            if (chMeaning.Length != 3)
                            {
                                currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.err, buffer, "");
                            }
                            else
                            {
                                currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.char_literal, buffer, char.Parse(chMeaning.Trim('\'')));
                            }
                            state = States.S;
                            buffer = "";
                        }
                        else
                        {
                            buffer += currentChar;
                            Analysis();
                        }
                    }
                    else
                    {
                        state = States.ER;
                        Analysis();
                    }
                    break;
                case States.STR:
                    if (sr.Peek() != -1)
                    {
                        currentChar = (char)sr.Read();
                        numOfColumn++;
                        if (currentChar == '\"')
                        {
                            buffer += currentChar;
                            string strMeaning = buffer;
                            for (int i = 0; i < control_sequence.Length; i++)
                            {
                                if (buffer.Contains(control_sequence[i]))
                                {
                                    strMeaning = buffer.Replace(control_sequence[i], control_sequence_value[i]);
                                }
                            }
                            currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.string_literal, buffer, strMeaning.Trim('\"'));
                            state = States.S;
                            buffer = "";
                        }
                        else
                        {
                            buffer += currentChar;
                            Analysis();
                        }
                    }
                    else
                    {
                        state = States.ER;
                        Analysis();
                    }
                    break;
                case States.ER:
                    if (sr.Peek() != -1)
                    {
                        currentChar = (char)sr.Read();
                        numOfColumn++;
                        if ((Array.IndexOf(insign, currentChar) != -1) || (Array.IndexOf(delimiter, currentChar) != -1) || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                        {
                            currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.err, buffer, "");
                            state = States.S;
                            buffer = "";
                            if (Array.IndexOf(delimiter, currentChar) != -1 || (Array.IndexOf(operators, currentChar.ToString()) != -1))
                            {
                                buffer += currentChar;
                            }
                        }
                        else
                        {
                            buffer += currentChar;
                            Analysis();
                        }
                    }
                    else
                    {
                        currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.err, buffer, "");
                        buffer = "";
                        state = States.S;
                    }
                    break;
            }
        }

        public int ColumnNumberCalc()
            => numOfColumn - buffer.Length;
        public void dotActions()
        {
            currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.delimiter, buffer, buffer);
            buffer = "";
            state = States.S;
        }
        private Token KWorIdentToken(string buffer, bool eof)
        {
            Token currentToken;
            bool srch = false;
            foreach (Key_words key_word in Enum.GetValues(typeof(Key_words)))
            {
                if (Convert.ToString(key_word) == buffer)
                {
                    srch = true;
                }
            }
            if (srch)
            {
                if (eof)
                {
                    currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.key_word, buffer, "KWName");
                }
                else
                {
                    currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.key_word, buffer, "KWName");
                }
            }
            else
            {
                if (eof)
                {
                    currentToken = new Token(numOfString, ColumnNumberCalc(), Type_lex.identifier, buffer, "IDName");
                }
                else
                {
                    currentToken = new Token(numOfString, ColumnNumberCalc() - 1, Type_lex.identifier, buffer, "IDName");
                }
                
            }
            return currentToken;
        }
        public Token GetNextToken()
        {
            NextToken();
            return GetToken();
        }
        public void NextToken()
        {
            Analysis();
        }
        public Token GetToken()
            => currentToken;
        public Lexer(string iF)
        {
            sr = new StreamReader(iF);
        }
    }
}