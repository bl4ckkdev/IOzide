using System;
using System.Collections.Generic;
using System.Linq;

namespace LangTest
{
    public class Lexer
    {
        public static int line, col;
        
        public class Token
        {
            public string Value { get; set; }
            public TokenType Type { get; set; }
        }
    
        public enum TokenType 
        {
            Number,
            Identifier,
            String,
            Equals,
            Comma,
            Dot,
            Colon,
            Semicolon,
            OpenBrace,
            CloseBrace,
            OpenBracket,
            CloseBracket,
            OpenParen, 
            CloseParen,
            BinaryOperator,
            Let,
            Const,
            Function,
            EOF,
        }

        public static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
        {
            {"let", TokenType.Let},
            {"const", TokenType.Const},
            {"fn", TokenType.Function},
        };
        
        public static List<Token> Tokenize(string code)
        {
            List<Token> tokens = new List<Token>();
            List<string> source = code.Select(c => c.ToString()).ToList();
            
            while (source.Count > 0)
            {
                if (source[0] == "(")
                {
                    tokens.Add(CreateToken(source[0], TokenType.OpenParen));
                    source.RemoveAt(0);
                }
                else if (source[0] == ")")
                {
                    tokens.Add(CreateToken(source[0], TokenType.CloseParen));
                    source.RemoveAt(0);
                }
                else if (source[0] == "{")
                {
                    tokens.Add(CreateToken(source[0], TokenType.OpenBrace));
                    source.RemoveAt(0);
                }
                else if (source[0] == "}")
                {
                    tokens.Add(CreateToken(source[0], TokenType.CloseBrace));
                    source.RemoveAt(0);
                }
                else if (source[0] == "[")
                {
                    tokens.Add(CreateToken(source[0], TokenType.OpenBracket));
                    source.RemoveAt(0);
                }
                else if (source[0] == "]")
                {
                    tokens.Add(CreateToken(source[0], TokenType.CloseBracket));
                    source.RemoveAt(0);
                }
                else if (source[0] == "+" || source[0] == "-" || source[0] == "*" || source[0] == "/" || source[0] == "%")
                {
                    tokens.Add(CreateToken(source[0], TokenType.BinaryOperator));
                    source.RemoveAt(0);
                }
                else if (source[0] == "=")
                {
                    tokens.Add(CreateToken(source[0], TokenType.Equals));
                    source.RemoveAt(0);
                }
                else if (source[0] == ";")
                {
                    tokens.Add(CreateToken(source[0], TokenType.Semicolon));
                    source.RemoveAt(0);
                }
                else if (source[0] == ":")
                {
                    tokens.Add(CreateToken(source[0], TokenType.Colon));
                    source.RemoveAt(0);
                }
                else if (source[0] == ",")
                {
                    tokens.Add(CreateToken(source[0], TokenType.Comma));
                    source.RemoveAt(0);
                }
                else if (source[0] == ".")
                {
                    tokens.Add(CreateToken(source[0], TokenType.Dot));
                    source.RemoveAt(0);
                }
                else if (source[0] == "~")
                {
                    source.RemoveAt(0);
                    while (source.Count > 0 && source[0] != "\n")
                    {
                        col++;
                        source.RemoveAt(0);
                    }
                }
                else if (source[0] == "\"")
                {
                    string str = "";

                    Console.WriteLine("test");
                    
                    source.RemoveAt(0);
                    while (source[0] != "\"")
                    {
                        str += source[0];
                        col++;
                        
                        source.RemoveAt(0);
                    }
                    source.RemoveAt(0);
                    tokens.Add(CreateToken(str, TokenType.String));
                }
                else
                {
                    if (IsInteger(source[0]))
                    {
                        string number = "";

                        while (source.Count > 0 && IsInteger(source[0]))
                        {
                            number += source[0];
                            col++;
                            source.RemoveAt(0);
                        }
                        col--;
                        tokens.Add(CreateToken(number, TokenType.Number));
                    } 
                    else if (IsAlphabetic(source[0]))
                    {
                        string identifier = "";

                        string current = source[0];
                        while (source.Count > 0 && IsAlphabetic(current))
                        {
                            identifier += source[0];
                            source.RemoveAt(0);
                            col++;
                            if (source.Count > 0) current = source[0];
                        }
                        col--;
                        if (!Keywords.ContainsKey(identifier)) tokens.Add(CreateToken(identifier, TokenType.Identifier));
                        else
                        {
                            if (identifier == "let") tokens.Add(CreateToken(identifier, TokenType.Let));
                            if (identifier == "const") tokens.Add(CreateToken(identifier, TokenType.Const));
                            if (identifier == "fn") tokens.Add(CreateToken(identifier, TokenType.Function));
                        }
                    }
                    else if (IsSkippable(source[0]))
                    {
                        if (source[0] == "\n") line++;
                        source.RemoveAt(0);
                    }
                    else throw new Exception($"Unrecognized Character: ${source[0]} at line {line} col {col}");
                }
                col++;
            }
            
            tokens.Add(CreateToken("EndOfFile", TokenType.EOF));
            return tokens;
        }
        
        public static bool IsAlphabetic(string text) => text.ToUpper() != text.ToLower() && !IsSkippable(text);
        public static bool IsInteger(string text) => text.All(char.IsDigit);
        public static bool IsSkippable(string text) => text == " " || text == "\n" || text == "\t" || text == "\r";

        public static Token CreateToken(string value, TokenType type)
        {
            return new Token { Value = value, Type = type };
        }
    }
}
