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
        env.DeclareVariable("null", new Values.NullValue { Type = Values.ValueType.Null, Value = null, }, true);
        env.DeclareVariable("false", new Values.BooleanValue { Type = Values.ValueType.Boolean, Value = false }, true);
        env.DeclareVariable("true", new Values.BooleanValue { Type = Values.ValueType.Boolean, Value = true }, true);
        env.DeclareVariable("output", Values.NativeFunction(Output, env), true);
        env.DeclareVariable("write", Values.NativeFunction(Write, env), true);
        env.DeclareVariable("time", Values.NativeFunction(Time, env), true);
        env.DeclareVariable("input", Values.NativeFunction(Input, env), true);
        env.DeclareVariable("num", Values.NativeFunction(Num, env), true);
        env.DeclareVariable("str", Values.NativeFunction(Str, env), true);
        env.DeclareVariable("bool", Values.NativeFunction(Bool, env), true);
        return env;
    }

    public static Values.RuntimeValue Output(List<Values.RuntimeValue> args, Environment env)
    {
        if (args.Count != 1) throw new Exception("Expected number of arguments of native function \"output()\" to be 1.");
        Console.WriteLine(args[0].Value);
        return Values.Null();
    }
    
    public static Values.RuntimeValue Write(List<Values.RuntimeValue> args, Environment env)
    {
        if (args.Count != 1) throw new Exception("Expected number of arguments of native function \"write()\" to be 1.");
        Console.Write(args[0].Value);
        return Values.Null();
    }
    
    public static Values.RuntimeValue Time(List<Values.RuntimeValue> args, Environment env)
    {
        if (args.Count != 0) throw new Exception("Expected number of arguments of native function \"time()\" to be 0.");
        return new Values.NumberValue
        {
            Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Type = Values.ValueType.Number
        };
    }
    
    public static Values.RuntimeValue Input(List<Values.RuntimeValue> args, Environment env)
    {
        if (args.Count > 1) throw new Exception("Expected number of arguments of native function \"input()\" to be 0 or 1.");
        Console.Write(args.Count > 0 ? args[0].Value.ToString() : "");
        return new Values.StringValue
        {
            Value = Console.ReadLine(),
            Type = Values.ValueType.String
        };
    }
    
    public static Values.RuntimeValue Num(List<Values.RuntimeValue> args, Environment env)
    {
        if (args.Count != 1) throw new Exception("Expected number of arguments of native function \"num()\" to be 1.");
        return new Values.NumberValue
        {
            Value = Convert.ToDouble(args[0].Value),
            Type = Values.ValueType.Number
        };
    }
    
    public static Values.RuntimeValue Bool(List<Values.RuntimeValue> args, Environment env)
    {
        if (args.Count != 1) throw new Exception("Expected number of arguments of native function \"bool()\" to be 1.");
        return new Values.BooleanValue
        {
            Value = Convert.ToBoolean(args[0].Value),
            Type = Values.ValueType.Boolean
        };
    }
    
    public static Values.RuntimeValue Str(List<Values.RuntimeValue> args, Environment env)
    {
        if (args.Count != 1) throw new Exception("Expected number of arguments of native function \"str()\" to be 1.");
        return new Values.BooleanValue
        {
            Value = args[0].Value.ToString(),
            Type = Values.ValueType.String
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