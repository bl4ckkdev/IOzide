using System;
using System.Collections.Generic;
using System.Linq;

namespace LangTest.Runtime.Eval;

using Runtime;

public class Expressions
{
     public static Values.NumberValue EvaluateNumericBinaryExpression(Values.NumberValue left, Values.NumberValue right, string op)
     {
         double result = 0;
         
         switch (op)
         {
             case "+":
                 result = Convert.ToDouble(left.Value) + Convert.ToDouble(right.Value);
                 break;
             case "-":
                 result = Convert.ToDouble(left.Value) - Convert.ToDouble(right.Value);
                 break;
             case "*":
                 result = Convert.ToDouble(left.Value) * Convert.ToDouble(right.Value);
                 break;
             case "/":
                 result = Convert.ToDouble(left.Value) / Convert.ToDouble(right.Value);
                 break;
             case "%":
                 result = Convert.ToDouble(left.Value) % Convert.ToDouble(right.Value);
                 break;
         }
    
         return new Values.NumberValue
         {
             Value = result,
             Type = Values.ValueType.Number
         };
     }
     
     public static Values.RuntimeValue EvaluateBinaryExpression(AST.BinaryExpression binop, Environment environment)
     {
         Values.RuntimeValue left = Interpreter.Evaluate(binop.Left, environment);
         Values.RuntimeValue right = Interpreter.Evaluate(binop.Right, environment);

         if (left.Type == Values.ValueType.Number && right.Type == Values.ValueType.Number)
         {
             return EvaluateNumericBinaryExpression(left as Values.NumberValue, right as Values.NumberValue, binop.Operator);
         }
    
         return Values.Null();
     }
    
     public static Values.RuntimeValue EvaluateIdentifier(AST.Identifier identifier, Environment environment)
     {
         var val = environment.LookupVariable(identifier.Symbol);
         return val;
     }

     public static Values.RuntimeValue EvaluateAssignment(AST.AssignmentExpression node, Environment environment)
     {
         if (node.Assignee.Kind != AST.NodeType.Identifier)
             throw new Exception($"Invalid left side inside assignment expression. {Program.PrettyPrint(node.Assignee)}");
         
         string name = (node.Assignee as AST.Identifier).Symbol;
         return environment.AssignVariable(name, Interpreter.Evaluate(node.Value, environment));
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
             
             // Evaluates the function body, line by line
             foreach (var statement in function.Body)
             {
                 result = Interpreter.Evaluate(statement, scope);
             }

             return result;
         }
         
         throw new Exception("Invalid Function: " + Program.PrettyPrint(fn));
     }
}