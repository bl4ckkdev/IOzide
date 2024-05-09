using LangTest.Runtime.Eval;
using System;
using System.Collections.Generic;

namespace LangTest.Runtime;
#nullable enable
public class Environment
{
    public Environment(Environment? parent)
    {
        Parent = parent;
        Global = parent == null;
    }
    
    public static Environment CreateGlobalEnvironment()
    {
        Environment env = new Environment(null);
        env.DeclareVariable("null", new Values.NullValue
        {
            Type = Values.ValueType.Null,
            Value = null,
        }, true);
        env.DeclareVariable("false", new Values.BooleanValue
        {
            Type = Values.ValueType.Boolean,
            Value = false
        }, true);
        env.DeclareVariable("true", new Values.BooleanValue
        {
            Type = Values.ValueType.Boolean,
            Value = true
        }, true);

        env.DeclareVariable("print", Values.NativeFunction(Print, env), true);
        env.DeclareVariable("time", Values.NativeFunction(Time, env), true);
        
        return env;
    }

    public static Values.RuntimeValue Print(List<Values.RuntimeValue> args, Environment env)
    {
        Console.WriteLine(args[0].Value);
        return Values.Null();
    }
    
    public static Values.RuntimeValue Time(List<Values.RuntimeValue> args, Environment env)
    {
        return new Values.NumberValue
        {
            Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Type = Values.ValueType.Number
        };
    }

    private Environment? Parent;
    public Dictionary<string, Values.RuntimeValue> Variables 
        = new Dictionary<string, Values.RuntimeValue>();
    public List<string> Constants = new List<string>();
    public bool Global;
    
    public Values.RuntimeValue DeclareVariable(string name, Values.RuntimeValue value, bool constant)
    {
        if (Variables.ContainsKey(name))
            throw new Exception($"Cannot declare variable {name} as it already exists.");
                
        Variables.Add(name, value);
        
        if (constant) Constants.Add(name);
        return value;
    }

    public Values.RuntimeValue AssignVariable(string name, Values.RuntimeValue value)
    {
        Environment env = Resolve(name);

        if (env.Constants.Contains(name))
        {
            throw new Exception($"Cannot reassign to constant {name}.");
        }
        env.Variables[name] = value; // could be wrong
        return value;
    }

    public Values.RuntimeValue LookupVariable(string name)
    {
        var env = Resolve(name);
        return env.Variables[name];
    }

    public Environment Resolve(string name)
    {
        if (Variables.ContainsKey(name)) return this;

        if (Parent == null) throw new Exception($"Cannot resolve {name}.");
        return Parent.Resolve(name);
    }
}