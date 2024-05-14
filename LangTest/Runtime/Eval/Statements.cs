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
        Values.IfStatementValue statement = new Values.IfStatementValue
        {
            Type = Values.ValueType.IfStatement,
            Conditions = declaration.conditions,
            DeclarationEnvironment = environment,
            Body = declaration.Body,
        };
        
        var cond = Interpreter.Evaluate(statement.Conditions, environment);
        
        if (cond.Type != Values.ValueType.Boolean) throw new Exception("Expected boolean inside of if statement.");
        else if (cond.Type == Values.ValueType.Boolean && Convert.ToBoolean(cond.Value))
        {
            Environment scope = new Environment(statement.DeclarationEnvironment);
            foreach (AST.Statement st in statement.Body) Interpreter.Evaluate(st, scope);
        }
        else if (declaration.Else != null)
        {
            Interpreter.Evaluate(declaration.Else, environment);
        }
        
        return statement;
    }
    
    public static Values.RuntimeValue EvaluateWhileLoopDeclaration(AST.WhileLoopStatement declaration, Environment environment)
    {
        Values.WhileLoopValue statement = new Values.WhileLoopValue
        {
            Type = Values.ValueType.IfStatement,
            Conditions = declaration.conditions,
            DeclarationEnvironment = environment,
            Body = declaration.Body,
        };
        
        var cond = Interpreter.Evaluate(statement.Conditions, environment);
        if (cond.Type != Values.ValueType.Boolean) throw new Exception("Expected boolean inside of while statement.");
        else
        {
            while (Convert.ToBoolean(Interpreter.Evaluate(statement.Conditions, environment).Value))
            {
                Environment scope = new Environment(statement.DeclarationEnvironment);
                foreach (AST.Statement st in statement.Body) Interpreter.Evaluate(st, scope);
            }
        }
        
        return statement;
    }
    
    public static Values.RuntimeValue EvaluateForLoopDeclaration(AST.ForLoopStatement declaration, Environment environment)
    {
        Values.ForLoopValue statement = new Values.ForLoopValue
        {
            Type = Values.ValueType.IfStatement,
            Arg1 = declaration.arg1,
            Arg2 = declaration.arg2,
            Arg3 = declaration.arg3,
            DeclarationEnvironment = environment,
            Body = declaration.Body,
        };
        
        Interpreter.Evaluate(statement.Arg1, environment);
        
        Values.RuntimeValue cond = Interpreter.Evaluate(statement.Arg2, environment); // for (let i = 0; i < 5; i = i + 1) { }
        
        if (cond.Type != Values.ValueType.Boolean) throw new Exception("Expected logical expression or boolean inside second argument of for loop.");
        else
        {
            while (Convert.ToBoolean(Interpreter.Evaluate(statement.Arg2, environment).Value))
            {
                Environment scope = new Environment(statement.DeclarationEnvironment);
                foreach (AST.Statement st in declaration.Body) Interpreter.Evaluate(st, scope);
                Interpreter.Evaluate(statement.Arg3, environment);
            }
        }
        
        return statement;
    }

    
    public static Values.RuntimeValue EvaluateDieStatement(AST.DieStatement declaration, Environment environment)
    {
        Values.DieStatementValue statement = new Values.DieStatementValue
        {
            Type = Values.ValueType.DieStatement,
            ExitCode = declaration.ExitCode
        };

        System.Environment.Exit(statement.ExitCode);
        return statement;
    }
}
