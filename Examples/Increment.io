fn Main() {
    fn increment(x, amount) {                     ~ Function with 2 arguments
        x + amount;                               ~ Returns x + amount
    }

    let number = num(input("Enter a number: "));  ~ Creates a variable with a value provided by the user
    output(increment(number, 5));                 ~ Prints value that function returns
}