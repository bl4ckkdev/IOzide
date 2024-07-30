using System;

namespace LangTest;

public class RuntimeException : Exception
{
    public string message;
    public RuntimeException(string msg)
    {
        message = msg;
    }
}

public class ParsingException : Exception
{
    public string message;
    public ParsingException(string msg)
    {
        message = msg;
    }
}
