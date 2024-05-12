using System.Collections.Generic;
#nullable enable
namespace LangTest
{
    public class AST
    {
        public enum NodeType
        {
            Program,
            VariableDeclaration,
            FunctionDeclaration,
            IfStatementDeclaration,
            ForLoopDeclaration,
            WhileLoopDeclaration,
            DieStatement,
            
            AssignmentExpression,
            MemberExpression,
            CallExpression,
            UnaryExpression,
            
            NumericLiteral,
            StringLiteral,
            Identifier,
            BinaryExpr,
            LogicalExpression,
            Property,
            ObjectLiteral,
        }
        public class Statement 
        {
            public NodeType Kind { get; set; }
        }

        public class Program : Statement
        {
            public List<Statement> Body;
            
            public Program() 
            {
                Kind = NodeType.Program;
            }
        }
        
        public class VariableDeclaration : Statement
        {
            public bool Constant;
            public string Ident;
            public Expression? Value;
            
            public VariableDeclaration() 
            {
                Kind = NodeType.VariableDeclaration;
            }
        }
        
        public class FunctionDeclaration : Statement
        {
            public List<string> parameters;
            public string Name;
            public List<Statement> Body;
            
            public FunctionDeclaration()
            {
                parameters = new List<string>();
                Kind = NodeType.FunctionDeclaration;
            }
        }
        
        public class IfStatement : Statement
        {
            public Expression conditions;
            public List<Statement> Body;
            public IfStatement? Else;
            
            public IfStatement()
            {
                Kind = NodeType.IfStatementDeclaration;
            }
        }
        
        public class WhileLoopStatement : Statement
        {
            public Expression conditions;
            public List<Statement> Body;
            
            public WhileLoopStatement()
            {
                Kind = NodeType.WhileLoopDeclaration;
            }
        }
        
        public class ForLoopStatement : Statement
        {
            public Statement arg1, arg3;
            public Expression arg2;
            public List<Statement> Body;
            
            public ForLoopStatement()
            {
                Kind = NodeType.ForLoopDeclaration;
            }
        }
        
        public class DieStatement : Statement
        {
            public int ExitCode;
            public DieStatement()
            {
                Kind = NodeType.DieStatement;
            }
        }

        public class Expression : Statement
        {
        }

        public class AssignmentExpression : Expression
        {
            public Expression Assignee, Value;
            public string Operator;

            public AssignmentExpression()
            {
                Kind = NodeType.AssignmentExpression;
            }
        }
        
        public class BinaryExpression : Expression
        {
            public BinaryExpression() { Kind = NodeType.BinaryExpr; }
            public Expression Left, Right;
            public string Operator;
        }
        
        public class LogicalExpression : Expression
        {
            public LogicalExpression() { Kind = NodeType.LogicalExpression; }
            public Expression Left, Right;
            public string Operator;
        }
        
        public class CallExpression : Expression
        {
            public CallExpression() { Kind = NodeType.CallExpression; }
            public List<Expression> Arguments;
            public Expression Caller;
        }
        
        public class MemberExpression : Expression
        {
            public MemberExpression() { Kind = NodeType.MemberExpression; }
            public List<Expression> Arguments;
            public Expression Property, Object;
            public bool Computed;
        }

        public class Identifier : Expression
        {
            public Identifier() { Kind = NodeType.Identifier; }
            public string Symbol;
            public bool Negative;
        }

        public class NumericLiteral : Expression
        {
            public NumericLiteral() { Kind = NodeType.NumericLiteral; }
            public float Value;
        }
        
        public class StringLiteral : Expression
        {
            public StringLiteral() { Kind = NodeType.StringLiteral; }
            public string Value;
        }
        
        public class Property : Expression
        {
            public Property() { Kind = NodeType.Property; }
            public string Key;
            public Expression? Value;
        }
        
        public class ObjectLiteral : Expression
        {
            public ObjectLiteral() { Kind = NodeType.ObjectLiteral; }
            public List<Property> Properties;
        }
    }
}
