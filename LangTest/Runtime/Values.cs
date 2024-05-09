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
            Boolean,
            Object,
            NativeFunction
        }

        public class RuntimeValue
        { 
            public ValueType Type;
        }
        
        public class NullValue : RuntimeValue
        {
            public string Value = null;

            public NullValue() => Type = ValueType.Null;
        }

        public class BooleanValue : RuntimeValue
        {
            public ValueType Type;
            public bool Value ;

            public BooleanValue()
            {
                Type = ValueType.Boolean;
            }
        }
        
        public class NumberValue : RuntimeValue
        {
            public float Value;

            public NumberValue() => Type = ValueType.Number;
        }
        
        public class ObjectValue : RuntimeValue
        {
            public Dictionary<string, RuntimeValue> Properties = new Dictionary<string, RuntimeValue>();
            
            public ObjectValue() => Type = ValueType.Object;
        }

        public static NumberValue Number(float n = 0) => new NumberValue
        {
            Type = ValueType.Number,
            Value = n
        };
        
        
        public static NullValue Null() => new NullValue
        {
            Type = ValueType.Null,
            Value = null,
        };
        
        public static BooleanValue Boolean(bool b = true) => new BooleanValue
        {
            Type = ValueType.Null,
            Value = b,
        };

        
        public delegate RuntimeValue FunctionCall(List<RuntimeValue> args, Environment env);
        public static RuntimeValue MyFunction(List<RuntimeValue> args, Environment env)
        {
            return new RuntimeValue();
        }
        
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
    }
}
