using LangTest.Runtime;
using System;
using Environment = LangTest.Runtime.Environment;
using System.Runtime.InteropServices;

namespace LangTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Add to PATH and run argument
            Console.Write(
                "— IOzide ALPHA (AIO) ———————————————————————————\n\n" +
                "Enter Repl: [1]\n" +
                $"Run Script: [2]\n" +
                $"Exit [3]\n\n> "
                );

            string input = Console.ReadLine();
            
            if (input == "1") Repl();
            else if (input == "2") RunFile();
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

        public static readonly string File = @"./Tests/test.io";

        public static void RunFile()
        {
            Parser parser = new Parser();
            Environment env = Environment.CreateGlobalEnvironment();
            
            string file = ShowDialog();
            if (file == "") return;
            string input = System.IO.File.ReadAllText(file);
            
            AST.Program program = parser.ProduceAST(input);
            var result = Interpreter.Evaluate(program, env);
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
        
        public static void Repl()
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
    }
}
