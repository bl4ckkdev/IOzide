![alt text][logo] 
A programming language made with C#.

> ⚠️ This language is in very early stages. While completely functional, it is missing a lot of features and it isn't stable.
## Features
- [x] Supported data types: Float, Boolean & Functions
- [x] Multiple native functions (e.g print(), time())
- [x] Custom functions with nesting support
- [x] ~~Compiler~~, Interpreter & REPL
- [x] Custom structs
- [ ] Multiple file projects
- [ ] Classes
- [ ] Namespaces

## Examples
> Hello world program.
```
print("Hello, World!")
```

> Code that uses variable & function declarations, as well as binary operations and the "print" native function.
```
let a = 5;                   ~ Creates a variable with the value "5"

fn increment(x, amount) {    ~ Function with 2 arguments
    x + amount               ~ Returns x + amount
}

print(increment(a, 5))       ~ Prints value that function returns
```
___
> ### ⚠️ Note:
> The base of the language is made using [tylerlaceby's guide](https://www.youtube.com/playlist?list=PL_2VhOvlMk4UHGqYCLWc6GO8FaPl8fQTh), completely ported from TypeScript to C#. This language does not follow good code practices and is probably very unstable, but I learned a lot of things making it which are likely going to be used when I make my next programming language.
> 
> Compared to the guide, my current version of the language follows all C# norms & has comments for now. In the very near future, it's going to have strings, a lot more native functions and a compiler for executables.

[logo]: Icons/iozide_full.png "IOzide"