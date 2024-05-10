using System;
using System.Collections.Generic;

namespace LangTest.Runtime.Eval;

public class Statements
{
    public static Values.RuntimeValue EvaluateProgram(AST.Program program, Environment environment)
    {
        Values.RuntimeValue lastEvaluated = Values.Null();
            
        foreach (var statement in program.Body)
        {
            lastEvaluated = Interpreter.Evaluate(statement, environment);
        }

        return lastEvaluated;
    }

    public static Values.RuntimeValue EvaluateVariableDeclaration(AST.VariableDeclaration declaration, Environment environment)
    {
        var value = declaration.Value != null ? Interpreter.Evaluate(declaration.Value, environment) : Values.Null();
        return environment.DeclareVariable(declaration.Ident, value, declaration.Constant);
    }
    
    public static Values.RuntimeValue EvaluateFunctionDeclaration(AST.FunctionDeclaration declaration, Environment environment)
    {
        Values.FunctionValue function = new Values.FunctionValue
        {
            Type = Values.ValueType.Function,
            Name = declaration.Name,
            Parameters = declaration.parameters,
            DeclarationEnvironment = environment,
            Body = declaration.Body
        };

        return environment.DeclareVariable(declaration.Name, function, true);
    }
    
    public static Values.RuntimeValue EvaluateIfStatementDeclaration(AST.IfStatement declaration, Environment environment)
    {
        // declaration.conditions are all the conditions inside of the if statement
        // declarations.Or is a list of each operation and if it's or or not, example: true && true \\ true && true -> false, true, false
        
        Values.IfStatementValue statement = new Values.IfStatementValue
        {
            Type = Values.ValueType.IfStatement,
            Conditions = declaration.conditions,
            DeclarationEnvironment = environment,
            Body = declaration.Body,
        };
    
        bool overallResult = true; // Initialize with true, as it's the neutral element for AND operation
        bool orOperation = false; // Indicates whether the next condition should be evaluated with OR operation
    
        foreach (AST.Expression expression in statement.Conditions) // The loop that checks if all the conditions are true
        {
            Values.RuntimeValue val = Interpreter.Evaluate(expression, statement.DeclarationEnvironment);
            try
            {
                bool conditionResult = Convert.ToBoolean(val.Value);
                if (orOperation) // Apply OR operation
                {
                    overallResult |= conditionResult;
                    orOperation = false; // Reset the flag after using it
                }
                else // Apply AND operation
                {
                    overallResult &= conditionResult;
                }
            }
            catch (InvalidCastException)
            {
                throw new Exception("Expected boolean inside of if statement.");
            }
    
            if (!overallResult && !orOperation) // If AND operation result is false and OR operation not pending
            {
                // If AND operation result is false, and OR operation not pending, skip the rest of the conditions
                break;
            }
            else if (overallResult && orOperation) // If OR operation result is true, and OR operation is pending
            {
                // If OR operation result is true and OR operation is pending, skip the rest of the conditions
                break;
            }
    
            if (!overallResult) // If AND operation result is false, and OR operation not pending
            {
                // If AND operation result is false, and OR operation not pending, switch to OR operation
                orOperation = true;
            }
        }
    
        if (overallResult) // If the overall result is true, execute the body of the if statement
        {
            statement.Body.ForEach(x => Interpreter.Evaluate(x, statement.DeclarationEnvironment));
        }
    
        return statement;
    }
}
