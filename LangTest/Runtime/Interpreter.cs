using LangTest.Runtime.Eval;

using System;

namespace LangTest.Runtime
{
    public class Interpreter
    {
        public static Values.RuntimeValue Evaluate(AST.Statement ASTNode, Environment environment)
        {
            try
            {
                switch (ASTNode.Kind)
                {
                    case AST.NodeType.NumericLiteral:
                        return new Values.NumberValue { Value = ((AST.NumericLiteral)ASTNode).Value, Type = Values.ValueType.Number};
                    case AST.NodeType.StringLiteral:
                        return new Values.StringValue { Value = ((AST.StringLiteral)ASTNode).Value, Type = Values.ValueType.String};
                    case AST.NodeType.Identifier:
                        return Expressions.EvaluateIdentifier(ASTNode as AST.Identifier, environment);
                    case AST.NodeType.ObjectLiteral:
                        return Expressions.EvaluateObjectExpression(ASTNode as AST.ObjectLiteral, environment);
                    case AST.NodeType.CallExpression:
                        return Expressions.EvaluateCallExpression(ASTNode as AST.CallExpression, environment);
                    case AST.NodeType.BinaryExpr:
                        return Expressions.EvaluateBinaryExpression(ASTNode as AST.BinaryExpression, environment);
                    case AST.NodeType.LogicalExpression:
                        return Expressions.EvaluateLogicalExpression(ASTNode as AST.LogicalExpression, environment);
                    case AST.NodeType.AssignmentExpression:
                        return Expressions.EvaluateAssignment(ASTNode as AST.AssignmentExpression, environment);
                    case AST.NodeType.IfStatementDeclaration:
                        return Statements.EvaluateIfStatementDeclaration(ASTNode as AST.IfStatement, environment);
                    case AST.NodeType.Program:
                        return Statements.EvaluateProgram(ASTNode as AST.Program, environment);
                    case AST.NodeType.VariableDeclaration:
                        return Statements.EvaluateVariableDeclaration(ASTNode as AST.VariableDeclaration, environment);
                    case AST.NodeType.FunctionDeclaration:
                        return Statements.EvaluateFunctionDeclaration(ASTNode as AST.FunctionDeclaration, environment);
                    default:
                        throw new NotImplementedException($"This node has not been implemented. {ASTNode.Kind}");
                }
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
    }
}
