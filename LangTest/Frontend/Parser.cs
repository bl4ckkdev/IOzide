using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LangTest
{
    public class Parser
    {
        private List<Lexer.Token> Tokens = new List<Lexer.Token>();

        private bool NotEOF() => Tokens[0].Type != Lexer.TokenType.EOF;

        private Lexer.Token At() => Tokens[0];
        private Lexer.Token Eat()
        {
            Lexer.Token token = At();
            Tokens.RemoveAt(0);
            return token;
        }

        private Lexer.Token Expect(Lexer.TokenType type, string error)
        {
            Lexer.Token token = Eat();

            if (token == null || token.Type != type) 
                throw new Exception($"{error}\nExpects {type} at {At().Value} with {Tokens.Count}");

            return token;
        }
        
        public AST.Program ProduceAST(string code)
        {
            Tokens = Lexer.Tokenize(code);
            AST.Program program = new AST.Program
            {
                Kind = AST.NodeType.Program,
                Body = new List<AST.Statement>()
            };

            while (NotEOF())
            {
                program.Body.Add(ParseStatement());
            }

            return program;
        }

        private AST.Statement ParseStatement()
        {
            switch (At().Type)
            {
                case Lexer.TokenType.Let or Lexer.TokenType.Const:
                    return ParseVariableDeclaration();
                case Lexer.TokenType.Function:
                    return ParseFunctionDeclaration();
                default:
                    return ParseExpression();
            }
            return ParseExpression();
        }

        private AST.Statement ParseVariableDeclaration()
        {
            bool isConstant = Eat().Type == Lexer.TokenType.Const;
            string identifier = Expect(Lexer.TokenType.Identifier, 
                "Expected identifier name following variable declaration.").Value;

            if (At().Type == Lexer.TokenType.Semicolon)
            {
                Eat();
                if (isConstant) throw new Exception("Must assign a value to constant expression.");

                return new AST.VariableDeclaration
                {
                    Kind = AST.NodeType.VariableDeclaration,
                    Ident = identifier,
                    Constant = false,
                };
            }

            Expect(Lexer.TokenType.Equals, "Expected equals token following identifier in variable declaration.");
            var declaration = new AST.VariableDeclaration
            {
                Kind = AST.NodeType.VariableDeclaration,
                Value = ParseExpression(),
                Ident = identifier,
                Constant = isConstant,
            };

            Expect(Lexer.TokenType.Semicolon, "Statement must end in semicolon.");

            return declaration;
        }

        public AST.Statement ParseFunctionDeclaration()
        {
            Eat();

            Lexer.Token name = Expect(Lexer.TokenType.Identifier, "Expected function name following fn keyword.");

            var args = ParseArguments();
            var parameters = new List<string>();

            foreach (AST.Expression argument in args)
            {
                if (argument.Kind != AST.NodeType.Identifier)
                    throw new Exception("Inside function declaration expected parameters to be of type string");
                parameters.Add((argument as AST.Identifier).Symbol);
            }

            Expect(Lexer.TokenType.OpenBrace, "Expected opening brace after function declaration.");

            var body = new List<AST.Statement>();
            while (At().Type != Lexer.TokenType.EOF && At().Type != Lexer.TokenType.CloseBrace)
            {
                body.Add(ParseStatement());
            }

            Expect(Lexer.TokenType.CloseBrace, "Closing brace expected inside function declaration.");

            AST.FunctionDeclaration function = new AST.FunctionDeclaration()
            {
                Body = body,
                Name = name.Value,
                parameters = parameters,
                Kind = AST.NodeType.FunctionDeclaration
            };

            return function;
        }
        
        private AST.Expression ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        private AST.Expression ParseAssignmentExpression()
        {
            
            var left = ParseObjectExpression();
            if (At().Type == Lexer.TokenType.Equals)
            {
                Eat();
                var value = ParseAssignmentExpression();
                return new AST.AssignmentExpression()
                {
                    Value = value,
                    Assignee = left,
                    Kind = AST.NodeType.AssignmentExpression
                };
            }
            
            return left;
        }
        private AST.Expression ParseObjectExpression()
        {
            if (At().Type != Lexer.TokenType.OpenBrace)
            {
                return ParseAdditiveExpression();
            }

            Eat();
            var properties = new List<AST.Property>();

            while (NotEOF() && At().Type != Lexer.TokenType.CloseBrace)
            {
                string key = Expect(Lexer.TokenType.Identifier, "Object literal key expected.").Value;

                if (At().Type == Lexer.TokenType.Comma)
                {
                    Eat();
                    properties.Add(new AST.Property { Key = key, Kind = AST.NodeType.Property });
                    continue;
                }
                else if (At().Type == Lexer.TokenType.CloseBrace)
                {
                    properties.Add(new AST.Property { Key = key, Kind = AST.NodeType.Property});
                    break; // Exit the loop when encountering the closing brace
                }
                else if (At().Type == Lexer.TokenType.Colon)
                {
                    Eat();
                    if (At().Type == Lexer.TokenType.OpenBrace)
                    {
                        // Parse nested object expression recursively
                        AST.Expression nestedObject = ParseObjectExpression();
                        properties.Add(new AST.Property { Key = key, Value = nestedObject, Kind = AST.NodeType.Property });
                    }
                    else
                    {
                        // Parse value expression for non-nested property
                        AST.Expression value = ParseExpression();
                        properties.Add(new AST.Property { Kind = AST.NodeType.Property, Value = value, Key = key});
                    }
                }
                if (At().Type != Lexer.TokenType.CloseBrace)
                {
                    Expect(Lexer.TokenType.Comma, "Expected comma or closing bracket following property.");
                }
            }

            Expect(Lexer.TokenType.CloseBrace, "Object literal missing closing brace.");
            
            
            var a = new AST.ObjectLiteral
            {
                Kind = AST.NodeType.ObjectLiteral,
                Properties = properties
            };

            Console.WriteLine(Program.PrettyPrint(a));
           
            
            return a;
        }

        private AST.Expression ParseAdditiveExpression()
        {
            AST.Expression left = ParseMultiplicativeExpression();

            while (At().Value == "+" || At().Value == "-")
            {
                string op = Eat().Value;
                AST.Expression right = ParseMultiplicativeExpression();

                left = new AST.BinaryExpression
                {
                    Kind = AST.NodeType.BinaryExpr,
                    Left = left,
                    Right = right,
                    Operator = op
                };
            }
            
            return left;
        }
        
        private AST.Expression ParseMultiplicativeExpression()
        {
            AST.Expression left = ParseCallMemberExpression();

            while (At().Value == "*" || At().Value == "/" || At().Value == "%")
            {
                string op = Eat().Value;
                AST.Expression right = ParseCallMemberExpression();

                left = new AST.BinaryExpression
                {
                    Kind = AST.NodeType.BinaryExpr,
                    Left = left,
                    Right = right,
                    Operator = op
                };
            }
            
            return left;
        }
        private AST.Expression ParseCallMemberExpression()
        {
            var member = ParseMemberExpression();

            if (At().Type == Lexer.TokenType.OpenParen)
            {
                return ParseCallExpression(member);
            }

            return member;
        }

        private AST.Expression ParseCallExpression(AST.Expression caller)
        {
            AST.CallExpression callExpression = new AST.CallExpression
            {
                Kind = AST.NodeType.CallExpression,
                Caller = caller,
                Arguments = ParseArguments(),
            };

            if (At().Type == Lexer.TokenType.OpenParen)
            {
                callExpression = ParseCallExpression(callExpression) as AST.CallExpression;
            }

            return callExpression;
        }

        private List<AST.Expression> ParseArguments()
        {
            Expect(Lexer.TokenType.OpenParen, "Expected open parenthesis");
            var args = At().Type == Lexer.TokenType.CloseParen
                ? new List<AST.Expression>()
                : ParseArgumentsList();

            Expect(Lexer.TokenType.CloseParen, "Missing closing parenthesis inside of args.");
            return args;
        }
        
        private List<AST.Expression> ParseArgumentsList()
        {
            List<AST.Expression> args = new List<AST.Expression>();
            args.Add(ParseAssignmentExpression());

            while (NotEOF() && At().Type == Lexer.TokenType.Comma && Eat() != null)
            {
                args.Add(ParseAssignmentExpression());
            }

            return args;
        }
        
        private AST.Expression ParseMemberExpression()
        {
            AST.Expression obj = ParsePrimaryExpression();

            while (At().Type == Lexer.TokenType.Dot || At().Type == Lexer.TokenType.OpenBracket)
            {
                Lexer.Token op = Eat();
                AST.Expression property;
                bool computed;

                if (op.Type == Lexer.TokenType.Dot)
                {
                    computed = false;
                    property = ParsePrimaryExpression();

                    if (property.Kind != AST.NodeType.Identifier)
                    {
                        throw new Exception("Expected identifier next to dot");
                    }
                }
                else
                {
                    computed = true;
                    property = ParseExpression();
                    Expect(Lexer.TokenType.CloseBracket, "Missing closing bracket in computed value.");
                }
                obj = new AST.MemberExpression
                {
                    Kind = AST.NodeType.MemberExpression,
                    Object = obj,
                    Property = property,
                    Computed = computed,
                };
            }

            return obj;
        }
        
        private AST.Expression ParsePrimaryExpression()
        {
            Lexer.TokenType token = At().Type;
            
            switch (token)
            {
                case Lexer.TokenType.Identifier:
                    return new AST.Identifier { Kind = AST.NodeType.Identifier, Symbol = Eat().Value};
                case Lexer.TokenType.Number:
                    return new AST.NumericLiteral { Kind = AST.NodeType.NumericLiteral, Value = float.Parse(Eat().Value)};
                case Lexer.TokenType.OpenParen:
                    Eat();
                    AST.Expression value = ParseExpression();
                    Eat();
                    return value;
                default: throw new Exception($"Unexpected Token: {At().Type}");
            }
        }
    }
}
