using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Environment = LangTest.Runtime.Environment;

namespace LangTest
{
    public class Parser
    {
        private List<Lexer.Token> Tokens = new List<Lexer.Token>();

        public int col;
        private bool NotEOF() => Tokens[0].Type != Lexer.TokenType.EOF;

        private Lexer.Token At()
        {
            if (Tokens.Count == 0) return null;
            return Tokens[0];
        }
        private Lexer.Token Eat()
        {
            Lexer.Token token = At();
            col += token.Value.Length;
            Tokens.RemoveAt(0);
            return token;
        }

        private Lexer.Token Expect(Lexer.TokenType type, string error)
        {
            try
            {
                Lexer.Token token = Eat();

                if (token == null || token.Type != type)
                    throw new Exception($"{error}\nExpects {type} at {At().Value} at column {col}");
                return token;
            }
            catch
            {
                throw new Exception($"{error}\nExpects {type} at {At().Value} and {At().Type} at column {col}");
            }
        }
        
        public AST.Program ProduceAST(string code)
        {
            try
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
            catch (Exception ex)
            {
                var currentColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error — ");
                Console.ForegroundColor = currentColor;
                Console.Write(ex.Message);
                System.Environment.Exit(1);
                throw;
            }
        }

        private AST.Statement ParseStatement(bool semi = true)
        {
            AST.Statement ret;
            bool needsSemicolon = false;

            switch (At().Type)
            {
                case Lexer.TokenType.Let or Lexer.TokenType.Const:
                    ret = ParseVariableDeclaration();
                    needsSemicolon = true;
                    break;
                case Lexer.TokenType.Function:
                    ret = ParseFunctionDeclaration();
                    break;
                case Lexer.TokenType.If:
                    ret = ParseIfStatementDeclaration();
                    break;
                case Lexer.TokenType.While:
                    ret = ParseWhileLoopDeclaration();
                    break;
                case Lexer.TokenType.Die:
                    ret = ParseDieStatement();
                    needsSemicolon = true;
                    break;
                case Lexer.TokenType.For:
                    ret = ParseForLoopDeclaration();
                    break;
                default:
                    ret = ParseExpression();
                    needsSemicolon = true;
                    break;
            }

            if (needsSemicolon && semi && Eat().Type != Lexer.TokenType.Semicolon)
                throw new Exception($"Expected semicolon after Statement."); 

            return ret;
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
        
        public AST.Statement ParseWhileLoopDeclaration()
        {
            Lexer.Token t = Eat();

            AST.Expression args;
            Expect(Lexer.TokenType.OpenParen, "Expected open parenthesis following if keyword.");
            args = ParseConditions();
            Expect(Lexer.TokenType.CloseParen, "Expected closing parenthesis following if statement");
            Expect(Lexer.TokenType.OpenBrace, "Expected opening brace after function declaration.");
            var body = new List<AST.Statement>();
            while (At().Type != Lexer.TokenType.EOF && At().Type != Lexer.TokenType.CloseBrace)
            {
                body.Add(ParseStatement());
            }

            Expect(Lexer.TokenType.CloseBrace, "Closing brace expected inside function declaration.");
            
            return new AST.WhileLoopStatement
            {
                Kind = AST.NodeType.WhileLoopDeclaration,
                Body = body,
                conditions = args,
            };
        }

        public AST.Statement ParseIfStatementDeclaration()
        {
            Lexer.Token t = Eat();

            AST.Expression args;
            if (t.Type == Lexer.TokenType.Else)
            {
                args = new AST.Identifier
                {
                    Kind = AST.NodeType.Identifier,
                    Symbol = "true"
                };
            }
            else
            {
                Expect(Lexer.TokenType.OpenParen, "Expected open parenthesis following while keyword.");
                args = ParseConditions();

                Expect(Lexer.TokenType.CloseParen, "Expected closing parenthesis following while statement");
            }
            Expect(Lexer.TokenType.OpenBrace, "Expected opening brace after function declaration.");
            var body = new List<AST.Statement>();
            while (At().Type != Lexer.TokenType.EOF && At().Type != Lexer.TokenType.CloseBrace)
            {
                body.Add(ParseStatement());
            }

            Expect(Lexer.TokenType.CloseBrace, "Closing brace expected inside function declaration.");

            AST.IfStatement right = null;
            if (At().Type == Lexer.TokenType.Else || At().Type == Lexer.TokenType.ElseIf) right = ParseIfStatementDeclaration() as AST.IfStatement;
            
            return new AST.IfStatement
            {
                Kind = AST.NodeType.IfStatementDeclaration,
                Body = body,
                conditions = args,
                Else = right
            };
        }
        
        public AST.Statement ParseForLoopDeclaration()
        {
            Lexer.Token t = Eat();
            
            AST.Statement arg1;
            AST.Expression arg2;
            AST.Statement arg3;
            
            Expect(Lexer.TokenType.OpenParen, "Expected open parenthesis following if keyword.");
            arg1 = ParseStatement();
            if (arg1.Kind != AST.NodeType.VariableDeclaration && arg1.Kind != AST.NodeType.AssignmentExpression && arg1.Kind != AST.NodeType.CallExpression)
                throw new Exception("Expected declaration, assignment or call inside of 1st argument of for loop"); // 1. declaration, assignment, call, unary incr / decr
            arg2 = ParseExpression();
            if (arg2.Kind != AST.NodeType.BinaryExpr)
                throw new Exception("Expected logical expression inside of 2nd argument of for loop"); // 2. comparison, 3. assignment, call, unary incr / decr 1
            Expect(Lexer.TokenType.Semicolon, "Expected closing parenthesis following second expression in for loop.");
            arg3 = ParseStatement(false);
            if (arg3.Kind != AST.NodeType.AssignmentExpression && arg3.Kind != AST.NodeType.CallExpression)
                throw new Exception("Expected assignment or call inside of 3nd argument of for loop"); // 3. assignment, call, unary incr / decr 1
            
            Expect(Lexer.TokenType.CloseParen, "Expected closing parenthesis following if statement");
            Expect(Lexer.TokenType.OpenBrace, "Expected opening brace after function declaration.");
            var body = new List<AST.Statement>();
            while (At().Type != Lexer.TokenType.EOF && At().Type != Lexer.TokenType.CloseBrace)
            {
                body.Add(ParseStatement());
            }

            Expect(Lexer.TokenType.CloseBrace, "Closing brace expected inside function declaration.");
            
            return new AST.ForLoopStatement
            {
                Kind = AST.NodeType.ForLoopDeclaration,
                Body = body,
                arg1 = arg1,
                arg2 = arg2,
                arg3 = arg3,
            };
        }
        
        public AST.Statement ParseDieStatement()
        {
            Eat();

            int exitCode = 0;
            if (At().Type == Lexer.TokenType.Number) exitCode = Convert.ToInt32(Eat().Value);
            
            return new AST.DieStatement()
            {
                Kind = AST.NodeType.DieStatement,
                ExitCode = exitCode
            };
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
                var t = Eat();
                
                var value = ParseAssignmentExpression();
                return new AST.AssignmentExpression()
                {
                    Value = value,
                    Assignee = left,
                    Kind = AST.NodeType.AssignmentExpression,
                    Operator = t.Value
                };
            }
            
            return left;
        }

        private AST.Expression ParseLogicalExpression()
        {
            AST.Expression left = ParseComparisonExpression();

            if (At().Type == Lexer.TokenType.And || At().Type == Lexer.TokenType.Or)
            {
                string op = Eat().Value;
                AST.Expression right = ParseComparisonExpression();

                left = new AST.LogicalExpression
                {
                    Kind = AST.NodeType.LogicalExpression,
                    Left = left,
                    Right = right,
                    Operator = op
                };

                while (At().Type == Lexer.TokenType.And || At().Type == Lexer.TokenType.Or)
                {
                    left = new AST.LogicalExpression
                    {
                        Kind = AST.NodeType.LogicalExpression,
                        Left = left,
                        Operator = Eat().Value,
                        Right = ParseExpression(),
                    };
                }
            }

            return left;
        }
        
        private AST.Expression ParseComparisonExpression()
        {
            AST.Expression left = ParseAdditiveExpression();

            if (At().Type == Lexer.TokenType.ComparisonOperator)
            {
                string op = Eat().Value;
                AST.Expression right = ParseAssignmentExpression();

                left = new AST.BinaryExpression
                {
                    Kind = AST.NodeType.BinaryExpr,
                    Left = left,
                    Right = right,
                    Operator = op
                };

                while (At().Type == Lexer.TokenType.ComparisonOperator)
                {
                    left = new AST.BinaryExpression
                    {
                        Kind = AST.NodeType.BinaryExpr,
                        Left = left,
                        Operator = Eat().Value,
                        Right = ParseExpression(),
                    };
                }
            }

            return left;
        }
        
        private AST.Expression ParseObjectExpression()
        {
            if (At().Type != Lexer.TokenType.OpenBrace)
            {
                return ParseLogicalExpression();
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
            
            while (At().Value == "*" || At().Value == "/" || At().Value == "%" || At().Value == "^")
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

        private AST.Expression ParseConditions()
        {
            var args = ParseLogicalExpression();
            
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
                case Lexer.TokenType.Not:
                case Lexer.TokenType.Identifier:
                    bool n = token == Lexer.TokenType.Not;
                    if (n) Eat();
                    return new AST.Identifier { Kind = AST.NodeType.Identifier, Negative = n, Symbol = Eat().Value};
                case Lexer.TokenType.Number:
                    return new AST.NumericLiteral { Kind = AST.NodeType.NumericLiteral, Value = float.Parse(Eat().Value)};
                case Lexer.TokenType.String:
                    return new AST.StringLiteral { Kind = AST.NodeType.StringLiteral, Value = Eat().Value };
                case Lexer.TokenType.OpenParen:
                    Eat();
                    AST.Expression value = ParseExpression();
                    Eat();
                    return value;
                case Lexer.TokenType.BinaryOperator:
                    Lexer.Token t = At();
                    if (t.Value == "+" || t.Value == "-")
                    {
                        Eat();

                        if (At().Type == Lexer.TokenType.Number)
                        {
                            return new AST.NumericLiteral
                            {
                                Kind = AST.NodeType.NumericLiteral, Value = t.Value == "+" ? float.Parse(Eat().Value) : -float.Parse(Eat().Value),
                            };
                        }
                        if (At().Type == Lexer.TokenType.Identifier)
                        {
                            if (Lexer.IsInteger(Eat().Value)) return new AST.Identifier { Kind = AST.NodeType.Identifier, Symbol = Eat().Value, Negative = true};
                        }
                    }
                    goto default;
                default: throw new Exception($"Unexpected Token: {At().Type}");
            }
        }
    }
}
