using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace LangTest.Runtime.Eval;

using Runtime;

public class Expressions
{
     public static Values.NumberValue EvaluateNumericBinaryExpression(Values.RuntimeValue left, Values.RuntimeValue right, string op)
     {
         double result = 0;

         double l = Math.Round(Convert.ToDouble(left.Value), 7);
         double r = Math.Round(Convert.ToDouble(right.Value), 7);
         
         switch (op)
         {
             case "+":
                 result = l + r;
                 break;
             case "-":
                 result = l - r;
                 break;
             case "*":
                 result = l * r;
                 break;
             case "/":
                 result = l / r;
                 break;
             case "%":
                 result = l % r;
                 break;
             case "^":
                 result = Math.Pow(l, r);
                 break;
                 
         }
    
         return new Values.NumberValue
         {
             Value = result,
             Type = Values.ValueType.Number
         };
     }
     
     public static Values.RuntimeValue EvaluateComparisonExpression(Values.RuntimeValue left, Values.RuntimeValue right, string op)
     {
         bool result = false;
         
         switch (op)
         {
             case "==":
                 result = left.Type.ToString() + left.Value.ToString() == right.Type.ToString() + right.Value.ToString();
                 break;
             case "!=":
                 result = left.Type.ToString() + left.Value.ToString() != right.Type.ToString() + right.Value.ToString();
                 break;
             case ">":
                 result = Convert.ToDouble(left.Value) > Convert.ToDouble(right.Value);
                 break;
             case ">=":
                 result = Convert.ToDouble(left.Value) >= Convert.ToDouble(right.Value);
                 break;
             case "<":
                 result = Convert.ToDouble(left.Value) < Convert.ToDouble(right.Value);
                 break;
             case "<=":
                 result = Convert.ToDouble(left.Value) <= Convert.ToDouble(right.Value);
                 break;
         }
         
         return new Values.BooleanValue
         {
             Value = Convert.ToBoolean(result),
             Type = Values.ValueType.Boolean
         };
     }
     
     public static Values.StringValue EvaluateConcatExpression(Values.RuntimeValue left, Values.RuntimeValue right, string op)
     {
         string result = "";
         
         switch (op)
         {
             case "+":
                 result = left.Value.ToString() + right.Value.ToString();
                 break;
             case "*":
                 try
                 {
                     double mul = Convert.ToDouble(right.Value);
                     
                     for (int i = 0; i < mul; i++) result += left.Value.ToString();
                 }
                 catch
                 {
                     throw new Exception("Expected number in right hand side of multiplicative string concatenation.");
                 }
                 break;
             default: return new Values.StringValue { Value = null, Type = Values.ValueType.String};
         }
    
         return new Values.StringValue
         {
             Value = result,
             Type = Values.ValueType.String
         };
     }
     
     public static Values.RuntimeValue EvaluateBinaryExpression(AST.BinaryExpression binop, Environment environment)
     {
         Values.RuntimeValue left = Interpreter.Evaluate(binop.Left, environment);
         Values.RuntimeValue right = Interpreter.Evaluate(binop.Right, environment);

         if (binop.Operator == "==" || binop.Operator == "!=")
         {
             return EvaluateComparisonExpression(left, right, binop.Operator);
         }

         if ((binop.Operator == ">" || binop.Operator == ">=" || binop.Operator == "<=" || binop.Operator == "<") && left.Type == Values.ValueType.Number && right.Type == Values.ValueType.Number)
         {
             return EvaluateComparisonExpression(left, right, binop.Operator);
         }
         
         if (left.Type == Values.ValueType.Number && right.Type == Values.ValueType.Number)
         {
             return EvaluateNumericBinaryExpression(left as Values.NumberValue, right as Values.NumberValue, binop.Operator);
         }
         
         if (left.Type == Values.ValueType.String || right.Type == Values.ValueType.String)
         {
             return EvaluateConcatExpression(left, right, binop.Operator);
         }
    
         return Values.Null();
     }
     
     public static Values.RuntimeValue EvaluateLogicalExpression(AST.LogicalExpression logop, Environment environment)
     {
         Values.RuntimeValue left = Interpreter.Evaluate(logop.Left, environment);
         Values.RuntimeValue right = Interpreter.Evaluate(logop.Right, environment);
         
         if (left.Type == Values.ValueType.Boolean && right.Type == Values.ValueType.Boolean)
         {
             return new Values.BooleanValue
             {
                 Type = Values.ValueType.Boolean,
                 Value = logop.Operator == "&&" ? Convert.ToBoolean(left.Value) && Convert.ToBoolean(right.Value) : Convert.ToBoolean(left.Value) || Convert.ToBoolean(right.Value),
             };
         }
    
         return Values.Null();
     }

    
     public static Values.RuntimeValue EvaluateIdentifier(AST.Identifier identifier, Environment environment)
     {
         var val = environment.LookupVariable(identifier.Symbol);
         if (identifier.Negative && val.Type == Values.ValueType.Number) val.Value = -Convert.ToDouble(val.Value);
         if (identifier.Negative && val.Type == Values.ValueType.Boolean) val.Value = !Convert.ToBoolean(val.Value);
         return val;
     }

     public static Values.RuntimeValue EvaluateAssignment(AST.AssignmentExpression node, Environment environment)
     {
         if (node.Assignee.Kind != AST.NodeType.Identifier)
             throw new Exception($"Invalid left side inside assignment expression.");
         
         string name = (node.Assignee as AST.Identifier).Symbol;
         return environment.AssignVariable(name, Interpreter.Evaluate(node.Operator == "=" ? node.Value : new AST.BinaryExpression
             {
                 Kind = AST.NodeType.BinaryExpr,
                 Left = node.Assignee,
                 Operator = node.Operator[0].ToString(),
                 Right = node.Operator[1].ToString() == "=" ? node.Value : new AST.NumericLiteral
                 {
                     Kind = AST.NodeType.NumericLiteral,
                     Value = 1,
                 },
                 
             }, environment));
     }

     public static Values.RuntimeValue EvaluateObjectExpression(AST.ObjectLiteral _object, Environment environment)
     {
         var obj = new Values.ObjectValue
         {
             Type = Values.ValueType.Object,
             Properties = new Dictionary<string, Values.RuntimeValue>()
         };

         foreach (AST.Property property in _object.Properties)
         {
             var runtimeVal = (property.Value == null) ? environment.LookupVariable(property.Key)
                 : Interpreter.Evaluate(property.Value, environment);

             obj.Properties[property.Key] = runtimeVal;
         }
         
         return obj;
     }
     
     public static Values.RuntimeValue EvaluateCallExpression(AST.CallExpression expression, Environment environment)
     {
         var args = expression.Arguments.Select((arg) => Interpreter.Evaluate(arg, environment)).ToList();
         Values.RuntimeValue fn = Interpreter.Evaluate(expression.Caller, environment);

         if (fn.Type == Values.ValueType.NativeFunction)
         {
             var res = (fn as Values.NativeFunctionValue).Call(args, environment);
             return res;
         } 
         if (fn.Type == Values.ValueType.Function)
         {
             var function = fn as Values.FunctionValue;
             Environment scope = new Environment(function.DeclarationEnvironment);

             for (int i = 0; i < function.Parameters.Count; i++)
             {
                 // TODO: Check the bounds
                 string name = function.Parameters[i];
                 scope.DeclareVariable(name, args[i], false);
             }

             Values.RuntimeValue result = Values.Null();
             
             foreach (var statement in function.Body)
             {
                 result = Interpreter.Evaluate(statement, scope);
             }

             return result;
         }
         
         throw new Exception("Invalid Function.");
     }
}
