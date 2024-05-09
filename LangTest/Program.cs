using LangTest.Runtime;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Environment = LangTest.Runtime.Environment;

namespace LangTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.Write(
                "— IOzide ———————————————————————————\n\n" +
                "Enter Repl: [1]\n" +
                $"Read File ({File}) [2]\n" +
                $"Exit [3]\n\n> "
                );

            string input = Console.ReadLine();
            
            if (input == "1") Repl();
            else if (input == "2") TestFile();
            else if (input == "3") System.Environment.Exit(1);
            else
            {
                Console.WriteLine("Invalid command, exiting..");
                System.Environment.Exit(1);
            }
            
            
            Repl();
        }

        public static readonly string File = @"./Tests/test.io";

        public static void TestFile()
        {
            Parser parser = new Parser();
            Environment env = Environment.CreateGlobalEnvironment();
            
            Console.WriteLine("Running Test..\n\n");
            string input = System.IO.File.ReadAllText(File);
            
            AST.Program program = parser.ProduceAST(input);
            var result = Interpreter.Evaluate(program, env);
            Console.WriteLine(PrettyPrint(result));
            System.Environment.Exit(1);
        }
        
        public static void Repl()
        {
            Parser parser = new Parser();
            Environment env = Environment.CreateGlobalEnvironment();
            
            Console.WriteLine("— Repl ———————————————————————————");
            while (true)
            {
                Console.Write("\n> "); string input = Console.ReadLine();
                if (input != null && input.ToLower() == "exit") System.Environment.Exit(1);
                
                AST.Program program = parser.ProduceAST(input);
                
               Console.WriteLine(PrettyPrint(program));

                var result = Interpreter.Evaluate(program, env);
                //Console.WriteLine(PrettyPrint(result));
            }
        }

        public static string PrettyPrint(object obj)
        {
            return JsonConvert.SerializeObject(
                obj, Formatting.Indented,
                new JsonConverter[] {new StringEnumConverter()});
        }
    }
}
