// This is useless and unfinished, for now




//using System.Collections.Generic;
//using System.Linq;
//
//namespace LangTest
//{
//    public class Lexerv2
//    {
//        public class Token
//        {
//            public string Value { get; set; }
//            public TokenType Type { get; set; }
//        }
//    
//        private enum TokenType 
//        {
//            Number,
//            Identifier,
//            Equals,
//            OpenParen, 
//            CloseParen,
//            BinaryOperator,
//            Let,
//        }
//
//        public class TokenIdentifier
//        {
//            public string[] Identifiers;
//        }
//
//        public Dictionary<TokenIdentifier, TokenType> Tokens = new Dictionary<TokenIdentifier, TokenType>()
//        {
//            { new TokenIdentifier {Identifiers = new[] {"0","1","2","3","4","5","6","7","8","9"}}, TokenType.Number },
//            { new TokenIdentifier {Identifiers = new[] {"?"}}, TokenType.Identifier },
//            { new TokenIdentifier {Identifiers = new[] {"="}}, TokenType.Equals },
//            { new TokenIdentifier {Identifiers = new[] {"("}}, TokenType.OpenParen },
//            { new TokenIdentifier {Identifiers = new[] {")"}}, TokenType.CloseParen },
//            { new TokenIdentifier {Identifiers = new[] {"+","-","*","/"}}, TokenType.BinaryOperator },
//            { new TokenIdentifier {Identifiers = new[] {"let"}}, TokenType.Let },
//        };
//
//        public bool IsAlphabetic(string text) => text.ToUpper() == text.ToLower();
//        public bool IsInteger(string text) => text.All(char.IsDigit);
//
//        public void SortTokenTypes()
//        {
//            List<KeyValuePair<TokenIdentifier, TokenType>> list = Tokens.ToList();
//
//            list.Sort((firstPair, nextPair) =>
//                {
//                    return firstPair.Key.Identifiers.Length.CompareTo(nextPair.Key.Identifiers.Length);
//                }
//            );
//
//            Tokens = list.ToDictionary(pair => pair.Key, pair => pair.Value);
//        }
//        
//        public Token[] Tokenize(string code)
//        {
//            List<Token> tokens = new List<Token>();
//            List<string> source = code.Split().ToList();
//
//            while (source.Count > 0)
//            {
//                foreach (KeyValuePair<TokenIdentifier, TokenType> values in Tokens)
//                {
//                    
//                }
//                if (source[0] == "(")
//                {
//                    tokens.Add(CreateToken(source[0], TokenType.OpenParen));
//                    source.RemoveAt(0);
//                }
//                else if (source[0] == ")")
//                {
//                    tokens.Add(CreateToken(source[0], TokenType.CloseParen));
//                    source.RemoveAt(0);
//                }
//                else if (source[0] == "+" || source[0] == "-" || source[0] == "*" || source[0] == "/")
//                {
//                    tokens.Add(CreateToken(source[0], TokenType.BinaryOperator));
//                    source.RemoveAt(0);
//                }
//                else if (source[0] == "=")
//                {
//                    tokens.Add(CreateToken(source[0], TokenType.Equals));
//                    source.RemoveAt(0);
//                }
//                else
//                {
//                    
//                }
//            }
//            return null;
//        }
//
//        public Token CreateToken(string value, TokenType type)
//        {
//            return new Token() {Value = value, Type = type};
//        }
//    }
//}
//