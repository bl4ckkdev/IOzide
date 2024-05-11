fn Main() {
    let in = input("Enter the password: ");

    if (in == "password") {
        output("Correct password! " + in);
    } elseif (in == "second password") {
        output("Correct password! " + in);
    } else {
        output("Wrong password! " + in);
    }
}