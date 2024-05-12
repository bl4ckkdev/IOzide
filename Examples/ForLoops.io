fn Main() {
    let message = input("Input the message you want to output: ");
    let times = num(input("Input the times you want it to output: "));

    for (let i = 0; i < times; i = i + 1) {
        output(message);
    }
}