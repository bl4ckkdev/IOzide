using LangTest.Runtime;
using LangTest.Runtime.Eval;
using System;
using Environment = LangTest.Runtime.Environment;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace LangTest
{
    internal class Program
    {
        public static readonly string Version = "v0.1.1";
        
        public static void Main(string[] args)
        {
            // TODO: Add to PATH and run argument
            Console.Write(
                $"— IOzide {Version} ALPHA (AIO) ———————————————————————————\n\n" +
                "Enter Repl: [1]\n" +
                $"Run Script: [2]\n" +
                $"Exit [3]\n\n> "
                );

            string input = Console.ReadLine();
            
            if (input == "1") REPL();
            else if (input == "2") RunFile(args);
            else if (input == "3") System.Environment.Exit(1);
            else
            {
                Console.WriteLine("Invalid command, exiting..");
                System.Environment.Exit(1);
            }
            
            Console.Write("\nPress anything to exit.. ");
            Console.ReadLine();
            
            System.Environment.Exit(0);
        }

        public static void RunFile(string[] args)
        {
            Parser parser = new Parser();
            Environment env = Environment.CreateGlobalEnvironment();
            
            string file = ShowDialog();
            if (file == "") return;
            string input = System.IO.File.ReadAllText(file);
            
            AST.Program program = parser.ProduceAST(input);
            var result = Interpreter.Evaluate(program, env);
            
            FindMainFunction(program, args, env);
        }
        
        public static void FindMainFunction(AST.Program program, string[] args, Environment env)
        {
            foreach (var statement in program.Body)
            {
                if (statement.Kind == AST.NodeType.FunctionDeclaration)
                {
                    var functionDeclaration = statement as AST.FunctionDeclaration;
                    if (functionDeclaration.Name == "Main")
                    {
                        Interpreter.Evaluate(new AST.Program
                        {
                            Body = functionDeclaration.Body,
                            Kind = AST.NodeType.Program
                        }, env);
                        return;
                    }
                }
            }
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct OpenFileName
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public string lpstrFile;
            public int nMaxFile;
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int flagsEx;
        }
        
        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName(ref OpenFileName ofn);

        private static string ShowDialog()
        {
            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            // Define Filter for your extensions (Excel, ...)
            ofn.lpstrFilter = "IOzide Files (*.io)\0*.io\0All Files (*.*)\0*.*\0";
            ofn.lpstrFile = new string(new char[256]);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = new string(new char[64]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "Open File Dialog...";
            if (GetOpenFileName(ref ofn))
                return ofn.lpstrFile;
            return string.Empty;
        }
        
        public static void REPL()
        {
            Parser parser = new Parser();
            Environment env = Environment.CreateGlobalEnvironment();
            
            Console.WriteLine("— IOzide Repl ———————————————————————————");
            Console.WriteLine("Type \"exit\" to exit.");
            while (true)
            {
                Console.Write("\n> "); string input = Console.ReadLine();
                if (input != null && input.ToLower() == "exit") System.Environment.Exit(1);
                
                AST.Program program = parser.ProduceAST(input);
                var result = Interpreter.Evaluate(program, env);
            }
        }

        public static void PrettyPrint(object obj)
        {
            var jsonString = JsonConvert.SerializeObject(
                obj, Formatting.Indented,
                new JsonConverter[] {new StringEnumConverter()});

            Console.WriteLine(jsonString);
        }
    }
}
