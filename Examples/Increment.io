let number = num(input("Enter a number: ")); ~ Creates a variable with a value provided by the user

fn increment(x, amount) {                    ~ Function with 2 arguments
    x + amount;                              ~ Returns x + amount
}

output(increment(number, 5));                ~ Prints value that function returns