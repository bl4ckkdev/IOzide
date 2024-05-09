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
}
