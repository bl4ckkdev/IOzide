using LangTest.Runtime.Eval;

using System;

namespace LangTest.Runtime
{
    public class Interpreter
    {
        public static Values.RuntimeValue Evaluate(AST.Statement ASTNode, Environment environment)
        {
            switch (ASTNode.Kind)
            {
                case AST.NodeType.NumericLiteral:
                    return new Values.NumberValue { 
                        Value = ((AST.NumericLiteral)ASTNode).Value,
                        Type = Values.ValueType.Number};
                case AST.NodeType.Identifier:
                    return Expressions.EvaluateIdentifier(ASTNode as AST.Identifier, environment);
                case AST.NodeType.ObjectLiteral:
                    return Expressions.EvaluateObjectExpression(ASTNode as AST.ObjectLiteral, environment);
                case AST.NodeType.BinaryExpr:
                    return Expressions.EvaluateBinaryExpression(ASTNode as AST.BinaryExpression, environment);
                case AST.NodeType.AssignmentExpression:
                    return Expressions.EvaluateAssignment(ASTNode as AST.AssignmentExpression, environment);
                case AST.NodeType.Program:
                    return Statements.EvaluateProgram(ASTNode as AST.Program, environment);
                case AST.NodeType.VariableDeclaration:
                    return Statements.EvaluateVariableDeclaration(ASTNode as AST.VariableDeclaration, environment);
                default:
                    throw new NotImplementedException($"This node has not been implemented. {ASTNode.Kind}");
            }
        }
    }
}
