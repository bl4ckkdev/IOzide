![alt text][logo] 
A programming language made with C#. The name is inspired by an old program by IBM called "IOzone".

> ⚠️ This language is in very early stages. While completely functional, it is missing a lot of features and it isn't stable.
## Features
- [x] Supported data types: Float, String, Boolean, Structs & Functions
- [x] Print & input functions, as well as multiple other native functions
- [x] Custom functions with nesting support
- [x] ~~Compiler~~, Interpreter & REPL
- [x] Custom structs
- [x] Comments
- [x] For/while loops, if statements
- [ ] Multiple file projects
- [ ] Classes
- [ ] Namespaces

## Examples
> Hello world
```
output("Hello, World!")
```
> Code that uses variable & function declarations, as well as binary operations and the "print" native function.
```
let number = num(input("Enter a number: ")); ~ Creates a variable with a value provided by the user

fn increment(x, amount) {                    ~ Function with 2 arguments
    x + amount;                              ~ Returns x + amount
}

output(increment(number, 5));                ~ Prints value that function returns
```

## Limitations
Currently, the language is missing crucial features while also not being stable.

___
> ### ⚠️ Note:
> The base of the language is made using [tylerlaceby's guide](https://www.youtube.com/playlist?list=PL_2VhOvlMk4UHGqYCLWc6GO8FaPl8fQTh), completely ported from TypeScript to C#. This language does not follow good code practices and is probably very unstable, but I learned a lot of things making it which are likely going to be used when I make my next programming language.
> 
> Compared to the guide, my current version of the language follows all C# norms and has a TON more features.

[logo]: Icons/iozide_full.png "IOzide"
