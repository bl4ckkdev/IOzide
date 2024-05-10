using System;
using System.Collections.Generic;

namespace LangTest.Runtime
{
    public class Values
    {
        public enum ValueType
        {
            Null,
            Number,
            String,
            Boolean,
            Object,
            NativeFunction,
            Function,
            IfStatement,
        }

        public class RuntimeValue
        {
            public object Value = null;
            public ValueType Type;
        }
        
        public class NullValue : RuntimeValue
        {
            public NullValue()
            {
                Value = null;
                Type = ValueType.Null;
            }
        }

        public class BooleanValue : RuntimeValue
        {
            public ValueType Type;

            public BooleanValue()
            {
                Value = false;
                Type = ValueType.Boolean;
            }
        }
        
        public class NumberValue : RuntimeValue
        {
            public NumberValue()
            {
                Value = (double)0;
                Type = ValueType.Number;
            }
        }
        
        public class StringValue : RuntimeValue
        {
            public StringValue()
            {
                Value = "";
                Type = ValueType.String;
            }
        }
        
        public class ObjectValue : RuntimeValue
        {
            public Dictionary<string, RuntimeValue> Properties = new Dictionary<string, RuntimeValue>();
            
            public ObjectValue() => Type = ValueType.Object;
        }

        public static StringValue String(string s = "") => new StringValue
        {
            Type = ValueType.String,
            Value = s
        };
        
        
        public static NullValue Null() => new NullValue
        {
            Type = ValueType.Null,
            Value = null,
        };
        
        public static BooleanValue Boolean(bool b = false) => new BooleanValue
        {
            Type = ValueType.Null,
            Value = b,
        };

        
        public delegate RuntimeValue FunctionCall(List<RuntimeValue> args, Environment env);
        
        public class NativeFunctionValue : RuntimeValue
        {
            public FunctionCall Call;
            public NativeFunctionValue()
            {
                Type = ValueType.NativeFunction;
            }
        }
        
        public static NativeFunctionValue NativeFunction(FunctionCall call, Environment env)
        {
            return new NativeFunctionValue
            {
                Type = ValueType.NativeFunction,
                Call = call
            };
        }
        
        public class FunctionValue : RuntimeValue
        {
            public string Name;
            public List<string> Parameters;
            public Environment DeclarationEnvironment;
            public List<AST.Statement> Body;
            
            public FunctionValue()
            {
                Type = ValueType.Function;
            }
        }
        
        public class IfStatementValue : RuntimeValue
        {
            public List<AST.Expression> Conditions;
            public List<AST.Statement> Body;
            public Environment DeclarationEnvironment;
            
            public IfStatementValue()
            {
                Type = ValueType.Function;
            }
        }
    }
}
