using System;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Array.Exists(args, element => element.Equals("-la")))
            {
                if (Array.Exists(args, element => element.Equals("-auto")))
                {
                    TestModule LexerAutoTM = new TestModule();
                    LexerAutoTM.LexerAutoTest();
                }
                else
                {
                    TestModule LexerManualTM = new TestModule();
                    LexerManualTM.LexerManualTest();
                }
            }
            else if (Array.Exists(args, element => element.Equals("-pa")))
            {
                if (Array.Exists(args, element => element.Equals("-auto")))
                {
                    TestModule ParserAutoTM = new TestModule();
                    ParserAutoTM.ParserAutoTest();
                }
                else
                {
                    TestModule ParserManualTM = new TestModule();
                    ParserManualTM.ParserManualTest();
                }
            }
            else
            {
                Console.WriteLine("1)Lexer\n2)Parser");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("Auto Test?(1-yes, 2-no)");
                        switch ((Console.ReadLine()))
                        {
                            case "1":
                                TestModule LexerAutoTM = new TestModule();
                                LexerAutoTM.LexerAutoTest();
                                break;
                            case "2":
                                TestModule LexerManualTM = new TestModule();
                                LexerManualTM.LexerManualTest();
                                break;
                            default:
                                Console.WriteLine("Incorrect input");
                                break;
                        }
                        break;
                    case "2":
                        Console.WriteLine("Auto Test?(1-yes, 2-no)");
                        switch ((Console.ReadLine()))
                        {
                            case "1":
                                TestModule ParserAutoTM = new TestModule();
                                ParserAutoTM.ParserAutoTest();
                                break;
                            case "2":
                                TestModule ParserManualTM = new TestModule();
                                ParserManualTM.ParserManualTest();
                                break;
                            default:
                                Console.WriteLine("Incorrect input");
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Incorrect input");
                        break;
                }
            
            }
        }
    }
}
